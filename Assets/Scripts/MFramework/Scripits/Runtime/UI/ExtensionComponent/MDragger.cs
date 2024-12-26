using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace MFramework
{
    [AddComponentMenu("MFramework/AdvancedUI/MDragger")]
    public class MDragger : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        //public int draggerID = -1;

        [SerializeField] private bool centerMove = false;

        [SerializeField] private RectTransform bound;//�߽�
        [SerializeField] private RectTransform selfBound;//����߽�

        [Header("Events")]
        [Space(5)]
        //Tip:ֻ�ֿܷ�д����ΪHeader������������д����
        [SerializeField] private UnityEvent onBeginDrag;
        [SerializeField] private UnityEvent onDrag, onEndDrag, onExitBound, onEnterBound;

        public bool isDragging { get; private set; } = false;

        private RectTransform rectTransform;
        private Vector4 boundParmas;//x-��߽� y-�ұ߽� z-�±߽� w-�ϱ߽�

        private Vector2 centerMoveOffset;

        private bool isOutOfBoundLastFrame = false;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();

            if (selfBound == null)
            {
                selfBound = GetComponent<RectTransform>();
            }

            if (bound != null)
            {
                SetBoundParmas(bound.rect);
            }
        }

        private void SetBoundParmas(Rect boundRect)
        {
            boundParmas.x = boundRect.xMin + selfBound.rect.width / 2;
            boundParmas.y = boundRect.xMax - selfBound.rect.width / 2;
            boundParmas.z = boundRect.yMin + selfBound.rect.height / 2;
            boundParmas.w = boundRect.yMax - selfBound.rect.height / 2;
        }

        public void SetBound(RectTransform rect)
        {
            bound = rect;
            SetBoundParmas(bound.rect);
        }
        //public void SetSelfBound(RectTransform rect)
        //{
        //    selfBound = rect;
        //}

        public void AddListener(MDraggerEvent mEvent, Action action)
        {
            switch (mEvent)
            {
                case MDraggerEvent.BeginDrag:
                    onBeginDrag.AddListener(()=> { action(); });
                    break;
                case MDraggerEvent.Drag:
                    onDrag.AddListener(() => { action(); });
                    break;
                case MDraggerEvent.EndDrag:
                    onEndDrag.AddListener(() => { action(); });
                    break;
                case MDraggerEvent.ExitBound:
                    onExitBound.AddListener(() => { action(); });
                    break;
                case MDraggerEvent.EnterBound:
                    onEnterBound.AddListener(() => { action(); });
                    break;
                default:
                    MLog.Print($"{typeof(MDragger)}:��֧�ֵ�����<{mEvent}>,����", MLogType.Warning);
                    break;
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            onBeginDrag.Invoke();

            //����centerMoveʱ��ǿ�ƽ�ͼƬ������괦
            Vector2 pointPos = eventData.position - MUtility.ScreenMidPos;
            Vector2 objPos = rectTransform.anchoredPosition;
            centerMoveOffset = centerMove ? pointPos - objPos : Vector2.zero;
            rectTransform.anchoredPosition += centerMoveOffset;

            isDragging = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
            onDrag.Invoke();

            MoveBasedPointer(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            onEndDrag.Invoke();

            isDragging = false;
        }

        private void MoveBasedPointer(PointerEventData eventData)
        {
            if (bound == null)//�����ƶ�
            {
                rectTransform.anchoredPosition += eventData.delta;
            }
            else//�߽������ƶ�
            {
                LimitedMove(eventData);
            }
        }

        private void LimitedMove(PointerEventData eventData)
        {
            Vector2 pointerPos = eventData.position - MUtility.ScreenMidPos;
            Vector2 delta = eventData.delta;

            bool isOutOfBound = false;

            //Tip:��������д������Ϊ:
            //���������/���ºϲ�(Խ���ұ߽�Ͳ��ú����ƶ�)�������ᵼ���������Ƴ�������ͱ������ƶ��ˣ�
            //�������������д�������Ƴ��󣬱���˵���Ͻ磬��ʱ����Ȼ�����������ƶ�����ôֻ�������������ƶ��������ܹ������ƶ���
            if (pointerPos.x < boundParmas.x)//Խ��߽�
            {
                isOutOfBound = true;
                if (delta.x > 0) delta.x = 0;//�����������ƶ�
            }
            else if (pointerPos.x > boundParmas.y)//Խ�ұ߽�
            {
                isOutOfBound = true;
                if (delta.x < 0) delta.x = 0;//�����������ƶ�
            }

            if (pointerPos.y < boundParmas.z)//Խ�±߽�
            {
                isOutOfBound = true;
                if (delta.y > 0) delta.y = 0;//�����������ƶ�
            }
            else if (pointerPos.y > boundParmas.w)//Խ�ϱ߽�
            {
                isOutOfBound = true;
                if (delta.y < 0) delta.y = 0;//�����������ƶ�
            }

            if (isOutOfBound && !isOutOfBoundLastFrame)
            {
                onExitBound.Invoke();
            }
            if (!isOutOfBound && isOutOfBoundLastFrame)
            {
                onEnterBound.Invoke();
            }
            isOutOfBoundLastFrame = isOutOfBound;

            rectTransform.anchoredPosition += delta;
            float rangeX = Mathf.Clamp(rectTransform.anchoredPosition.x, boundParmas.x, boundParmas.y);
            float rangeY = Mathf.Clamp(rectTransform.anchoredPosition.y, boundParmas.z, boundParmas.w);
            rectTransform.anchoredPosition = new Vector2(rangeX, rangeY);
        }
    }

    public enum MDraggerEvent
    {
        BeginDrag,
        Drag,
        EndDrag,
        ExitBound,
        EnterBound
    }
}