using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MFramework
{
    /// <summary>
    /// UI����Ļ��࣬��Ϊһ��UI��������а�����Ϊһ��UI����ر�������
    /// </summary>
    public class UIView
    {
        internal string viewID;//UI���ID������˵����������
        public GameObject gameObject;//������GameObject
        public RectTransform rectTransform;//������Transform
        public Transform parentTrans;//������Transform��������������ĸ���
        protected UIViewBehaviour viewBehaviour;//ͨ��Inspector�����ռ�����Ϣ

        public string prefabName { private set; get; }//Ԥ��������
        public bool isOpen { protected set; get; }//Panel/Widget����״̬

        private CanvasGroup canvasGroup;
        public CanvasGroup CanvasGroup 
        {
            internal set
            {
                canvasGroup = value;
            }
            get 
            {
                if (canvasGroup == null)
                {
                    canvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();
                }
                return canvasGroup;
            } 
        }

        protected Dictionary<string, UIWidget> widgetDic { private set; get; }

        private UIShowState showState = UIShowState.Off;
        public UIShowState ShowState
        {
            protected set
            {
                showState = value;
            }
            get
            {
                if (viewBehaviour.AnimSwitch == UIAnimSwitch.On)
                {
                    //MLog.Print($"UI��<{viewID}>�ѿ��������������AnimState���", MLogType.Warning);
                    return UIShowState.None;
                }
                return showState;
            }
        }
        private UIAnimState animState = UIAnimState.Idle;
        public UIAnimState AnimState
        {
            protected set
            {
                animState = value;
            }
            get
            {
                if (viewBehaviour.AnimSwitch != UIAnimSwitch.On)
                {
                    //MLog.Print($"UI��<{viewID}>δ���������������ShowState���", MLogType.Warning);
                    return UIAnimState.None;
                }
                return animState;
            }
        }

        #region �������
        protected void Create(string id, Transform parent, string prefabPath)
        {
            InstantiateAndCollectFields(id, parent, prefabPath, null);

            CreatingInternal();//�ڲ�����
            OnBindCompsAndEvents();//��(��Base�����)
            OnCreating();//����ʱ�¼�

            CreatedInternal();//�ڲ�����
            OnCreated();//�������¼�
        }
        protected void Create(string id, Transform parent, UIViewBehaviour behaviour)
        {
            InstantiateAndCollectFields(id, parent, null, behaviour);

            CreatingInternal();//�ڲ�����
            OnBindCompsAndEvents();//��(��Base�����)
            OnCreating();//����ʱ�¼�

            CreatedInternal();//�ڲ�����
            OnCreated();//�������¼�
        }

        protected void Destroy()
        {
            OnDestroying();//ɾ��ʱ�¼�
            OnUnbindCompsAndEvents();//���(��Base�����)
            DestroyingInternal();//�ڲ�����

            DestroyedInternal();//�ڲ�����
            OnDestroyed();//ɾ�����¼�
        }

        private bool InstantiateAndCollectFields(string id, Transform parent, string prefabPath, UIViewBehaviour inputBehaviour)
        {
            //��Ϣ�ռ�
            viewID = id;
            parentTrans = parent;

            UIViewBehaviour behaviour = null;
            //ʵ����
            if (prefabPath != null)//�ṩ·��ģʽ
            {
                GameObject prefab = null;
#if UNITY_EDITOR
                if (MCore.Instance.UICustomLoadState)//����UI�Զ������
                {
                    prefab = LoadPrefab(prefabPath);
                    if (prefab == null) { MLog.Print($"{typeof(UIView)}��δ��ȡ��{prefabPath}�µ�Prefab������·������д��LoadPrefab()�Ƿ���ȷ", MLogType.Error); return false; }
                }
                else
                {
                    prefabPath = prefabPath.Replace('\\', '/');
                    prefabPath = DealEditorPath(prefabPath);
                    prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                    if (prefab == null) { MLog.Print($"{typeof(UIView)}��δ��ȡ��{prefabPath}�µ�Prefab������", MLogType.Error); return false; }
                }
#else
                prefab = LoadPrefab(prefabPath);
                if (prefab == null) { MLog.Print($"{typeof(UIView)}��δ��ȡ��{prefabPath}�µ�Prefab������·������д��LoadPrefab()�Ƿ���ȷ", MLogType.Error); return false; }
#endif
                GameObject go = GameObject.Instantiate(prefab, parentTrans, false);
                //���
                behaviour = go.GetComponent<UIViewBehaviour>();
                if (behaviour == null) { MLog.Print($"{typeof(UIView)}��\"{id}\"��δ����Behaviour���������", MLogType.Error); return false; }
            }
            else if (inputBehaviour != null)//�ṩUIViewBehaviourģʽ
            {
                behaviour = inputBehaviour;
            }

            //��Ϣ�ռ�
            viewBehaviour = behaviour;
            rectTransform = viewBehaviour.gameObject.GetComponent<RectTransform>();
            gameObject = viewBehaviour.gameObject;

            if (prefabPath != null) prefabName = Path.GetFileNameWithoutExtension(prefabPath);
            else prefabName = inputBehaviour.gameObject.name;//����ѡ���������

            return true;
        }
        /// <summary>
        /// ֧��2�����룺����·��/����Assets��·��
        /// </summary>
        /// <param playerName="path"></param>
        private string DealEditorPath(string path)
        {
            if (Path.IsPathRooted(path))//����·��
            {
                string firstPath = Application.dataPath;
                if (path.StartsWith(firstPath))
                {
                    path = path.Substring(firstPath.Length - "Assets".Length);
                }
            }
            else
            {
                if (!path.StartsWith("Assets"))//�ǻ���Assets��·��
                {
                    MLog.Print($"{typeof(UIView)}��·��{path}����ȷ�����ṩ|����·��/����Assets��·��|����֮һ", MLogType.Error);
                    return null;
                }
            }

            if (!path.EndsWith(".prefab"))
            {
                path += ".prefab";
            }

            return path;
        }

        /// <summary>
        /// ������Դ��ʽ��Ĭ��ʹ��Resource.Load()���м���
        /// </summary>
        protected virtual GameObject LoadPrefab(string prefabPath)
        {
            GameObject prefab = Resources.Load<GameObject>(prefabPath);
            return prefab;
        }
        #endregion

        //֮������Ҫ��Widget�Ĵ���д���������Ϊ��
        //������Panel����Widget�������ܴ���/ɾ��Widget��
        //����Panel�����Ǵ�����һ���ڲ�Widget������Widget�����Ǵ�����Widget����Widget
        //Ҳ����˵**UIPanel���UIWidget�඼��Ҫ����Widget**
        #region Widget�����ӿ�
        //���ṩid---ʹ��T������(ֻ������ΨһWdiget���)
        //���ṩparent---ʹ�ø�UIView��transform(����ζ�Ž�����Ϊһ������)
        //�ṩprefabPath---Prefab·����ʽ
        //�ṩUIWidgetBehaviour---����GameObject��ʽ
        public T CreateWidget<T>(string id, Transform parent, string prefabPath, bool autoEnter = false) where T : UIWidget
        {
            T widget = Activator.CreateInstance<T>() as T;
            widget.Create(id, parent, prefabPath, this, autoEnter);

            if (widgetDic == null) widgetDic = new Dictionary<string, UIWidget>();
            widgetDic.Add(id, widget);

            return widget;
        }
        public T CreateWidget<T>(string id, string prefabPath, bool autoEnter = false) where T : UIWidget
        {
            return CreateWidget<T>(id, rectTransform, prefabPath, autoEnter);
        }
        public T CreateWidget<T>(Transform parent, string prefabPath, bool autoEnter = false) where T : UIWidget
        {
            if (widgetDic != null && widgetDic.ContainsKey(typeof(T).Name))
            {
                MLog.Print($"{typeof(UIView)}����id����ֻ������һ��һ��������и��ã��봫��id", MLogType.Warning);
                return null;
            }
            return CreateWidget<T>(typeof(T).Name, parent, prefabPath, autoEnter);
        }
        public T CreateWidget<T>(string prefabPath, bool autoEnter = false) where T : UIWidget
        {
            if (widgetDic != null && widgetDic.ContainsKey(typeof(T).Name))
            {
                MLog.Print($"{typeof(UIView)}����id����ֻ������һ��һ��������и��ã��봫��id", MLogType.Warning);
                return null;
            }
            return CreateWidget<T>(typeof(T).Name, rectTransform, prefabPath, autoEnter);
        }

        public T CreateWidget<T>(string id, UIWidgetBehaviour behaviour, bool autoEnter = false) where T : UIWidget
        {
            T widget = Activator.CreateInstance<T>() as T;
            widget.Create(id, behaviour.transform.parent, behaviour, this, autoEnter);

            if (widgetDic == null) widgetDic = new Dictionary<string, UIWidget>();
            widgetDic.Add(id, widget);

            return widget;
        }
        public T CreateWidget<T>(UIWidgetBehaviour behaviour, bool autoEnter = false) where T : UIWidget
        {
            if (widgetDic != null && widgetDic.ContainsKey(typeof(T).Name))
            {
                MLog.Print($"{typeof(UIView)}����id����ֻ������һ��һ��������и��ã��봫��id", MLogType.Warning);
                return null;
            }
            return CreateWidget<T>(typeof(T).Name, behaviour, autoEnter);
        }

        public bool DestroyWidget(string id)
        {
            if (widgetDic == null) 
            {
                MLog.Print($"{typeof(UIView)}��ID-<{viewID}>��û����Widget��ɾ��ʧ�ܣ�����", MLogType.Warning);
                return false;
            }
            if (!widgetDic.ContainsKey(id))
            {
                MLog.Print($"{typeof(UIView)}��ID-<{viewID}>��û����ID-<{id}>��ɾ��ʧ�ܣ�����", MLogType.Warning);
                return false;
            } 


            UIWidget widget = widgetDic[id];
            widgetDic.Remove(id);

            widget.Destroy();
            widget = null;
            return true;
        }
        public bool DestroyWidget<T>() where T : UIWidget
        {
            return DestroyWidget(typeof(T).Name);
        }
        public void DestroyAllWidgets()
        {
            if (widgetDic == null || widgetDic.Count <= 0) { return; }

            //ɾ��һ������(����ÿһ��Widget��Destroy()���������ɾ�����Բ���Ҫ��ȥ����������)
            //ע�⣺����foreach����ʱ���鲻�ܷ����䶯��������Ҫ�Ƚ�idȡ�����ܽ���
            List<string> ids = new List<string>();
            foreach (string id in widgetDic.Keys)
            {
                ids.Add(id);
            }
            foreach (string id in ids)
            {
                DestroyWidget(id);
            }

            widgetDic = null;
        }

        public T GetWidget<T>(string id) where T : UIWidget
        {
            if (widgetDic == null)
            {
                MLog.Print($"{typeof(UIView)}��ID-<{viewID}>��û����Widget����ȡʧ�ܣ�����", MLogType.Warning);
                return null;
            }
            if (!widgetDic.ContainsKey(id))
            {
                MLog.Print($"{typeof(UIView)}��ID-<{viewID}>��û����ID-<{id}>����ȡʧ�ܣ�����", MLogType.Warning);
                return null;
            }

            return (T)widgetDic[id];
        }
        public T GetWidget<T>() where T : UIWidget
        {
            return GetWidget<T>(typeof(T).Name);
        }

        public bool ExistWidget(string id)
        {
            if (widgetDic == null || !widgetDic.ContainsKey(id)) 
            {
                return false;
            }

            return true;
        }

        //����ҪSetVisible()������Open()/Close()�Ͱ�������һ����
        //public void SetPanelVisible(string id, bool visible)
        //{
        //    if (!widgetDic.ContainsKey(id))
        //    {
        //        MLog.Print($"UI��View-{viewID}��û��<{id}>������ʧ�ܣ�����", MLogType.Warning);
        //        return;
        //    }

        //    UIWidget widget = widgetDic[id];
        //    widget.SetVisible(visible);
        //}
        //public void SetPanelVisible<T>(bool visible)
        //{
        //    SetPanelVisible(typeof(T).Name, visible);
        //}

        public void OpenWidget(string id, Action onFinish = null)
        {
            if (!widgetDic.ContainsKey(id))
            {
                MLog.Print($"{typeof(UIView)}��ID-<{viewID}>��û����ID-<{id}>����ʧ�ܣ�����", MLogType.Warning);
                return;
            }

            UIWidget widget = widgetDic[id];
            widget.Open(onFinish);
        }
        public void OpenWidget<T>(Action onFinish = null)
        {
            OpenWidget(typeof(T).Name, onFinish);
        }

        public void CloseWidget(string id, Action onFinish = null)
        {
            if (!widgetDic.ContainsKey(id))
            {
                MLog.Print($"{typeof(UIView)}��ID-<{viewID}>��û����ID-<{id}>���ر�ʧ�ܣ�����", MLogType.Warning);
                return;
            }

            UIWidget widget = widgetDic[id];
            widget.Close(onFinish);
        }
        public void CloseWidget<T>(Action onFinish = null)
        {
            CloseWidget(typeof(T).Name, onFinish);
        }

        public void SetWidgetSibiling(string id, SiblingMode mode)
        {
            if (!widgetDic.ContainsKey(id))
            {
                MLog.Print($"{typeof(UIView)}��ID-<{viewID}>��û����ID-<{id}>������ʧ�ܣ�����", MLogType.Warning);
                return;
            }

            UIWidget widget = widgetDic[id];
            widget.SetSibling(mode);
        }
        public void SetWidgetSibiling<T>(SiblingMode mode)
        {
            SetWidgetSibiling(typeof(T).Name, mode);
        }

        public void SetSortingOrder(string id, int order)
        {
            if (!widgetDic.ContainsKey(id))
            {
                MLog.Print($"{typeof(UIView)}��ID-<{viewID}>��û����ID-<{id}>������ʧ�ܣ�����", MLogType.Warning);
                return;
            }

            UIWidget widget = widgetDic[id];
            widget.SetSortingOrder(id, order);
        }
        public void SetSortingOrder<T>(int order)
        {
            SetSortingOrder(typeof(T).Name, order);
        }
        #endregion

        #region �ڲ���������
        protected internal virtual void CreatingInternal() { }
        protected internal virtual void DestroyingInternal()
        {
            GameObject.Destroy(gameObject);
            viewID = null;
            gameObject = null;
            rectTransform = null;
            parentTrans = null;
            viewBehaviour = null;
            widgetDic = null;
        }
        protected internal virtual void CreatedInternal() 
        {
            isOpen = true;
        }
        protected internal virtual void DestroyedInternal() { }
        #endregion

        #region ������������
        public virtual void Init() { }
        public virtual void Update() { }

        protected virtual void OnCreating() { }
        protected virtual void OnCreated() { }
        protected virtual void OnDestroying() { }
        protected virtual void OnDestroyed() { }
        protected virtual void OnBindCompsAndEvents() { }
        protected virtual void OnUnbindCompsAndEvents() { }
        protected virtual void OnClicked(Button button) { }
        protected virtual void OnValueChanged(Toggle toggle, bool value) { }
        protected virtual void OnValueChanged(Dropdown dropdown, int value) { }
        protected virtual void OnValueChanged(InputField inputField, string value) { }
        protected virtual void OnValueChanged(Slider slider, float value) { }
        protected virtual void OnValueChanged(Scrollbar scrollbar, float value) { }
        protected virtual void OnValueChanged(ScrollRect scrollRect, Vector2 value) { }
        protected virtual void OnValueChanged(TMP_Dropdown dropdown, int value) { }
        protected virtual void OnValueChanged(TMP_InputField inputField, string value) { }

        protected virtual void OnVisibleChanged(bool visible) { }
        #endregion

        #region ����¼���
        protected void BindEvent(Button button)
        {
            button.onClick.AddListener(() =>
            {
                OnClicked(button);
            });
        }
        protected void BindEvent(Toggle toggle)
        {
            toggle.onValueChanged.AddListener((value) =>
            {
                OnValueChanged(toggle, value);
            });
        }
        protected void BindEvent(Dropdown dropdown)
        {
            dropdown.onValueChanged.AddListener((value) =>
            {
                OnValueChanged(dropdown, value);
            });
        }
        protected void BindEvent(TMP_Dropdown tmpDropdown)
        {
            tmpDropdown.onValueChanged.AddListener((value) =>
            {
                OnValueChanged(tmpDropdown, value);
            });
        }
        protected void BindEvent(InputField inputField)
        {
            inputField.onValueChanged.AddListener((value) =>
            {
                OnValueChanged(inputField, value);
            });
        }
        protected void BindEvent(TMP_InputField tmpInputField)
        {
            tmpInputField.onValueChanged.AddListener((value) => { OnValueChanged(tmpInputField, value); });
        }
        protected void BindEvent(Slider slider)
        {
            slider.onValueChanged.AddListener((value) => 
            { 
                OnValueChanged(slider, value);
            });
        }
        protected void BindEvent(Scrollbar scrollbar)
        {
            scrollbar.onValueChanged.AddListener((value) => 
            {
                OnValueChanged(scrollbar, value); 
            });
        }
        protected void BindEvent(ScrollRect scrollRect)
        {
            scrollRect.onValueChanged.AddListener((value) =>
            { 
                OnValueChanged(scrollRect, value); 
            });
        }
        protected void UnbindEvent(Button button)
        {
            button.onClick.RemoveAllListeners();
        }
        protected void UnbindEvent(Toggle toggle)
        {
            toggle.onValueChanged.RemoveAllListeners();
        }
        protected void UnbindEvent(Dropdown dropdown)
        {
            dropdown.onValueChanged.RemoveAllListeners();
        }
        protected void UnbindEvent(TMP_Dropdown tmpDropdown)
        {
            tmpDropdown.onValueChanged.RemoveAllListeners();
        }
        protected void UnbindEvent(InputField inputField)
        {
            inputField.onValueChanged.RemoveAllListeners();
        }
        protected void UnbindEvent(TMP_InputField tmpInputField)
        {
            tmpInputField.onValueChanged.RemoveAllListeners();
        }
        protected void UnbindEvent(Slider slider)
        {
            slider.onValueChanged.RemoveAllListeners();
        }
        protected void UnbindEvent(Scrollbar scrollbar)
        {
            scrollbar.onValueChanged.RemoveAllListeners();
        }
        protected void UnbindEvent(ScrollRect scrollRect)
        {
            scrollRect.onValueChanged.RemoveAllListeners();
        }
        #endregion
    }

    public enum SiblingMode
    {
        Top,
        Bottom
    }
}