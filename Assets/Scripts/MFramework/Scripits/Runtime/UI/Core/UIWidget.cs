using System;
using UnityEngine;

namespace MFramework
{
    /// <summary>
    /// UIPanel�������壬�����Ƕ��
    /// </summary>
    public class UIWidget : UIView
    {
        protected string widgetID { get { return viewID; } }

        protected UIView parentView;
        protected UIPanel panel;//����Panel

        public UIWidgetBehaviour widgetBehaviour { get { return (UIWidgetBehaviour)viewBehaviour; } }

        private bool firstEnter;

        protected internal void Create(string id, Transform parentTrans, string prefabPath, UIView parent, bool autoEnter)
        {
            parentView = parent;
            base.Create(id, parentTrans, prefabPath);

            CanvasGroup.interactable = false;
            CanvasGroup.blocksRaycasts = false;
            if (!autoEnter)
            {
                firstEnter = true;

                CanvasGroup.alpha = 0;
                ShowState = UIShowState.Off;
                AnimState = UIAnimState.Idle;
            }
            else
            {
                CanvasGroup.alpha = 1;
                if (widgetBehaviour.AnimSwitch == UIAnimSwitch.On)
                {
                    PlayOpenAnim();
                }
                else
                {
                    SetVisible(true);
                }
            }
        }
        protected internal void Create(string id, Transform parentTrans, UIViewBehaviour behaviour, UIView parent, bool autoEnter)
        {
            parentView = parent;
            base.Create(id, parentTrans, behaviour);

            CanvasGroup.interactable = false;
            CanvasGroup.blocksRaycasts = false;
            if (!autoEnter)
            {
                CanvasGroup.alpha = 0;
                ShowState = UIShowState.Off;
                AnimState = UIAnimState.Idle;
            }
            else
            {
                CanvasGroup.alpha = 1;
                if (widgetBehaviour.AnimSwitch == UIAnimSwitch.On)
                {
                    PlayOpenAnim();
                }
                else
                {
                    SetVisible(true);
                }
            }
        }

        #region �������
        public void DestroySelf()
        {
            parentView.DestroyWidget(widgetID);
        }
        public void SetSiblingSelf(SiblingMode mode)
        {
            parentView.SetWidgetSibiling(widgetID, mode);
        }
        public void OpenSelf()
        {
            parentView.OpenWidget(widgetID);
        }
        public void CloseSelf()
        {
            parentView.CloseWidget(widgetID);
        }
        #endregion

        #region ���Ĳ���
        internal void SetSibling(SiblingMode mode)
        {
            if (mode == SiblingMode.Top) SetToTop();
            else if (mode == SiblingMode.Bottom) SetToBottom();
        }
        internal void SetToTop()
        {
            rectTransform.SetAsLastSibling();
        }
        internal void SetToBottom()
        {
            rectTransform.SetAsLastSibling();
        }

        internal bool Open(Action onFinish = null)
        {
            //Simpleģʽ�Զ�����SetVisible()
            if (widgetBehaviour.WidgetMode == UIWidgetMode.Simple)
            {
                bool flag = SetVisible(true);
                if (flag)
                {
                    isOpen = true;
                    OnVisibleChanged(true);
                    onFinish?.Invoke();
                }
                return flag;
            }

            if (widgetBehaviour.AnimSwitch == UIAnimSwitch.On)
            {
                if (CanvasGroup.alpha == 0) 
                    UIPanelUtility.SetCanvasGroupActive(CanvasGroup, true);//autoEnter���µĵ�һ�ν���

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
            //Simpleģʽ�Զ�����SetVisible()
            if (widgetBehaviour.WidgetMode == UIWidgetMode.Simple)
            {
                bool flag = SetVisible(false);
                if (flag)
                {
                    isOpen = false;
                    OnVisibleChanged(false);
                    onFinish?.Invoke();
                } 

                return flag;
            }

            if (widgetBehaviour.AnimSwitch == UIAnimSwitch.On)
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

        internal bool SetVisible(bool visible, bool enableTransition = false)
        {
            if (ShowState == UIShowState.On && visible) { return false; }
            if (ShowState == UIShowState.Off && !visible) { return false; }

            UIPanelUtility.SetCanvasGroupActive(CanvasGroup, visible);

            ShowState = visible ? UIShowState.On : UIShowState.Off;

            return true;
        }

        protected virtual bool PlayOpenAnim(Action onFinish = null)
        {
            if (widgetBehaviour.AnimSwitch == UIAnimSwitch.Off)
            {
                onFinish?.Invoke();
                return false;
            } 

            if (widgetBehaviour.OpenAnimMode == UIOpenAnimMode.AutoPlay)
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
                widgetBehaviour.PlayOpenAnim(() => 
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
                MLog.Print($"{typeof(UIWidget)}��ID-<{widgetID}>��дPlayOpenAnim()�����ʹ��SelfControlģʽ������", MLogType.Warning);
                onFinish?.Invoke();
            }
            return true;
        }
        protected virtual bool PlayCloseAnim(Action onFinish = null)
        {
            if (widgetBehaviour.AnimSwitch == UIAnimSwitch.Off) 
            {
                onFinish?.Invoke();
                return false;
            }

            if (widgetBehaviour.CloseAnimMode == UICloseAnimMode.AutoPlay)
            {
                //���ڲ����������޷��ٴ�ִ��(�Ѿ��رյ�Ҳ�����ٴ�ִ��)
                if (AnimState == UIAnimState.Opening || AnimState == UIAnimState.Closing || AnimState == UIAnimState.Closed)
                    return false;

                AnimState = UIAnimState.Closing;
                //���ڹر��������Ҫ��ǰʹ����ʧ���ֹ�ٴε��
                CanvasGroup.interactable = false;
                CanvasGroup.blocksRaycasts = false;
                widgetBehaviour.PlayCloseAnim(() =>
                {
                    AnimState = UIAnimState.Closed;
                    onFinish?.Invoke(); 
                });
            }
            else
            {
                MLog.Print($"{typeof(UIWidget)}��ID-<{widgetID}>��дPlayCloseAnim()�����ʹ��SelfControlģʽ������", MLogType.Warning);
                onFinish?.Invoke();
            }
            return true;
        }
        #endregion

        #region �ڲ���������
        protected internal override void CreatingInternal()
        {
            base.CreatingInternal();

            widgetBehaviour.view = this;//���������
            panel = (UIPanel)gameObject.GetComponentInParent<UIPanelBehaviour>().view;
        }
        protected internal override void DestroyingInternal()
        {
            parentView = null;
            panel = null;

            base.DestroyingInternal();
        }
        protected internal override void CreatedInternal() { }
        protected internal override void DestroyedInternal() { }
        #endregion
    }
}