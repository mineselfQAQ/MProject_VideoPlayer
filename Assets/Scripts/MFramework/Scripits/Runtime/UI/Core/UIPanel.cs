
using System;
using UnityEngine;
using UnityEngine.UI;

namespace MFramework
{
    /// <summary>
    /// UI�Ļ������������UICanvas�¼�
    /// </summary>
    public class UIPanel : UIView
    {
        //UIView�ֶι�������
        public string panelID { get { return viewID; } }
        public UIPanelBehaviour panelBehaviour { get { return (UIPanelBehaviour)viewBehaviour; } }
        //UIPanel�ֶ�
        public UIRoot parentRoot { private set; get; }//UIPanel���ڵ�UIRoot
        public Canvas canvas { private set; get; }
        public GraphicRaycaster graphicRaycaster { private set; get; }

        //��̫����prefab���ڸ������(�米��)��Ӧ�����û�����
        //public static HashSet<string> panelPrefabSet = new HashSet<string>();//����Ƿ��Ѿ���Ź�ĳ��prefab

        public int sortingOrder => canvas.sortingOrder;

        private bool firstEnter;

        internal void Create(string id, UIRoot root, string prefabPath, bool autoEnter)
        {
            base.Create(id, UIManager.Instance.UICanvas.transform, prefabPath);
            parentRoot = root;
            //panelPrefabSet.Add(prefabName);

            //����Ҫ��Ĭ������(��Ҫalpha0-1�Ļ��Ƕ����н��еģ�������ĵĻ�alpha��Ӧ��Ϊ1��interactable/blocksRaycasts���ڶ���ִ����Ϻ����)
            CanvasGroup.interactable = false;
            CanvasGroup.blocksRaycasts = false;
            if (!autoEnter)
            {
                firstEnter = true;

                //UIPanelUtility.SetCanvasGroupActive(CanvasGroup, false);
                CanvasGroup.alpha = 0;
                ShowState = UIShowState.Off;
                AnimState = UIAnimState.Idle;
            }
            else
            {
                CanvasGroup.alpha = 1;
                if (panelBehaviour.AnimSwitch == UIAnimSwitch.On)
                {
                    PlayOpenAnim();
                }
                else
                {
                    SetVisible(true);
                }
            }
        }
        internal void Destroy(Action onFinish = null)
        {
            PlayCloseAnim(() =>
            {
                base.Destroy();
                onFinish?.Invoke();
            });
        }

        internal bool Open(Action onFinish = null)
        {
            if (panelBehaviour.AnimSwitch == UIAnimSwitch.On) 
            {
                //if (CanvasGroup.alpha == 0)
                //    UIPanelUtility.SetCanvasGroupActive(CanvasGroup, true);//autoEnter���µĵ�һ�ν���

                return PlayOpenAnim(() =>
                {
                    isOpen = true;
                    OnVisibleChanged(true);
                    onFinish?.Invoke();
                });
            }
            else
            {
                isOpen = true;
                bool flag = SetVisible(true);
                if (flag) OnVisibleChanged(true);
                onFinish?.Invoke();
                return flag;
            }
        }
        internal bool Close(Action onFinish = null)
        {
            if (panelBehaviour.AnimSwitch == UIAnimSwitch.On)
            {
                return PlayCloseAnim(() =>
                {
                    isOpen = false;
                    OnVisibleChanged(false);
                    onFinish?.Invoke();
                });
            }
            else
            {
                isOpen = false;
                bool flag = SetVisible(false);
                if (flag) OnVisibleChanged(false);
                onFinish?.Invoke();
                return flag;
            }
        }

        //Tip:����������������ֹRoot�������
        internal void SetSortingOrder(int order)
        {
            canvas.sortingOrder = order;//��������Canvas��sortingOrder
        }
        internal void SetPanelSibling(SiblingMode mode)
        {
            UIRoot root = parentRoot;
            if (mode == SiblingMode.Top)
            {
                int order = root.topOrder + root.topPanel.panelBehaviour.Thickness;
                SetSortingOrder(order);
                if (order > root.endOrder) UIPanelUtility.ResetOrder(root);
            }
            else if (mode == SiblingMode.Bottom)
            {
                UIPanel bottomPanel = UIPanelUtility.FilterBottommostPanel(root);
                int order = bottomPanel.sortingOrder - panelBehaviour.Thickness;
                SetSortingOrder(order);
                if (order < root.startOrder) UIPanelUtility.ResetOrder(root);
            }
        }

        //TODO:���Լ�һ���򵥵Ĺ�������Ч��
        internal bool SetVisible(bool visible, bool enableTransition = false)
        {
            if (ShowState == UIShowState.On && visible) { return false; }
            if (ShowState == UIShowState.Off && !visible) { return false; }

            UIPanelUtility.SetCanvasGroupActive(CanvasGroup, visible);

            ShowState = visible ? UIShowState.On : UIShowState.Off;

            return true;
        }

        internal void SetFocus(bool focus)
        {
            OnFocusChanged(focus);
        }

        protected virtual bool PlayOpenAnim(Action onFinish = null)
        {
            if (panelBehaviour.AnimSwitch == UIAnimSwitch.Off)
            {
                onFinish?.Invoke();
                return false;
            }

            if (panelBehaviour.OpenAnimMode == UIOpenAnimMode.AutoPlay)
            {
                //���ڲ����������޷��ٴ�ִ��(�Ѿ��򿪵�Ҳ�����ٴ�ִ��)
                if (AnimState == UIAnimState.Opening || AnimState == UIAnimState.Closing || AnimState == UIAnimState.Opened)
                    return false;

                AnimState = UIAnimState.Opening;
                //Tip��ֻ�е�һ�ν���ʱ�Ż�����alphaֵ��alphaֵӦ����Animation����
                if (firstEnter)
                {
                    firstEnter = false;
                    CanvasGroup.alpha = 1;
                }
                panelBehaviour.PlayOpenAnim(() => 
                { 
                    AnimState = UIAnimState.Opened;
                    //���ڿ����������Ҫ�ȶ���������ʹ���弤���ֹ�ٴε��
                    CanvasGroup.interactable = true;
                    CanvasGroup.blocksRaycasts = true;
                    onFinish?.Invoke(); 
                });
            }
            else
            {
                MLog.Print($"{typeof(UIPanel)}��ID-<{panelID}>��дPlayOpenAnim()�����ʹ��SelfControlģʽ������", MLogType.Warning);
                onFinish?.Invoke();
            }
            return true;
        }
        protected virtual bool PlayCloseAnim(Action onFinish = null)
        {
            if (panelBehaviour.AnimSwitch == UIAnimSwitch.Off) 
            {
                onFinish?.Invoke();
                return false;
            }

            if (panelBehaviour.CloseAnimMode == UICloseAnimMode.AutoPlay)
            {
                //���ڲ����������޷��ٴ�ִ��(�Ѿ��رյ�Ҳ�����ٴ�ִ��)
                if (AnimState == UIAnimState.Opening || AnimState == UIAnimState.Closing || AnimState == UIAnimState.Closed)
                    return false;

                AnimState = UIAnimState.Closing;
                //���ڹر��������Ҫ��ǰʹ����ʧ���ֹ�ٴε��
                CanvasGroup.interactable = false;
                CanvasGroup.blocksRaycasts = false;
                panelBehaviour.PlayCloseAnim(() =>
                {
                    //CanvasGroup.alpha = 0;
                    AnimState = UIAnimState.Closed;
                    onFinish?.Invoke(); 
                });
            }
            else
            {
                MLog.Print($"{typeof(UIPanel)}��ID-<{panelID}>��дPlayCloseAnim()�����ʹ��SelfControlģʽ������", MLogType.Warning);
                onFinish?.Invoke();
            }
            return true;
        }

        #region �������
        public void DestroySelf()
        {
            parentRoot.panelDic.Remove(panelID);
            Destroy();
        }

        //protected void SetVisibleSelf(bool visible)
        //{
        //    SetVisible(visible);
        //}
        public void SetPanelSiblingSelf(SiblingMode mode)
        {
            SetPanelSibling(mode);
        }

        public void OpenSelf(Action onFinish = null)
        {
            Open(onFinish);
        }
        public void CloseSelf(Action onFinish = null)
        {
            Close(onFinish);
        }
        #endregion

        #region �ڲ���������
        protected internal override void CreatingInternal()
        {
            base.CreatingInternal();

            //û��Ҫǿ������RectTransform��PanelӦ�ø���ԭ��Prefab��������
            //SetRectStretchMode();

            canvas = panelBehaviour.gameObject.GetOrAddComponent<Canvas>();
            graphicRaycaster = gameObject.GetOrAddComponent<GraphicRaycaster>();
            
            //ʹ����Panel������������(��Ϊ��Ƕ�׵�)
            canvas.overrideSorting = true;

            panelBehaviour.view = this;//���������
        }
        protected internal override void DestroyingInternal()
        {
            CanvasGroup = null;
            graphicRaycaster = null;
            canvas = null;
            parentRoot = null;

            base.DestroyingInternal();
        }
        protected internal override void CreatedInternal() { base.CreatedInternal(); }
        protected internal override void DestroyedInternal() { base.DestroyedInternal(); }

        private void SetRectStretchMode()
        {
            rectTransform.anchorMin = new Vector2(0f, 0f);
            rectTransform.anchorMax = new Vector2(1f, 1f);
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            
            //trans.sizeDelta = trans.parent.GetComponent<RectTransform>().sizeDelta;
            rectTransform.anchoredPosition = Vector2.zero;
        }
        #endregion

        #region ������������
        protected virtual void OnFocusChanged(bool focus) { }
        #endregion
    }
}