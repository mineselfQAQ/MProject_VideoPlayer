using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MFramework
{
    /// <summary>
    /// �����࣬һ�ֻ���ScrollView��ѭ���б�
    /// </summary>
    /// <typeparam name="Cell">Unityʵ��</typeparam>
    /// <typeparam name="Data">Cell��������</typeparam>
    public abstract class MScrollView<Cell, Data> : MonoBehaviour where Cell : MonoBehaviour
    {
        public MScrollViewDirection direction;
        public ICollection<Data> datas { get; private set; }

        [SerializeField] protected Cell cellPrefab;//Prefab(�������Cell�����͵����)
        [SerializeField] private RectTransform viewRange;//ScrollView����
        [SerializeField] protected RectTransform content;//ScrollView�е�Content
        [SerializeField] private Vector2 cellSpace;//Cell֮��ļ��
        [SerializeField] private int itemCellCount;//Bundle��Cell����

        private RectTransform cellRectTrans;
        private ScrollRect viewRangeScrollRect;

        private readonly Vector2 horizontalContentAnchorMin = new Vector2(0, 0);
        private readonly Vector2 horizontalContentAnchorMax = new Vector2(0, 1);
        private readonly Vector2 horizontalContentPivot = new Vector2(0, 0.5f);
        private readonly Vector2 verticalContentAnchorMin = new Vector2(0, 1);
        private readonly Vector2 verticalContentAnchorMax = new Vector2(1, 1);
        private readonly Vector2 verticalContentPivot = new Vector2(0.5f, 1);

        private readonly Vector2 cellPivot = new Vector2(0, 1);
        private readonly Vector2 cellAnchorMin = new Vector2(0, 1);
        private readonly Vector2 cellAnchorMax = new Vector2(0, 1);

        private readonly LinkedList<CellBundle<Cell>> cellBundles = new LinkedList<CellBundle<Cell>>();
        private readonly Queue<CellBundle<Cell>> cellBundlePool = new Queue<CellBundle<Cell>>();

        public Vector2 ContentPos => content.position;
        public Vector2 ContentSize => content.sizeDelta;
        public Vector2 CellSize => cellRectTrans.sizeDelta;//һ��Cell(����)
        public Vector2 ItemSize => CellSize + cellSpace;//һ��Cell(����+���)

        //����
        public int ItemCount
        {
            get
            {
                int cellCount = datas.Count;

                int itemCount = cellCount / itemCellCount;//һ�����
                if (cellCount % itemCellCount != 0)//��/�� �������(��10��Ԫ�أ�3��һ�ţ���3�ţ�����1��)
                {
                    itemCount += 1;
                }

                return itemCount;
            }
        }
        //Ԫ�ظ���
        public int CellCount => datas.Count;

        #region �������
        /// <summary>
        /// ��ʼ������
        /// </summary>
        protected virtual void Init(ICollection<Data> datas)
        {
            if (datas == null)
            {
                MLog.Print($"{typeof(MScrollView<Cell,Data>)}û���������ڳ�ʼ��������", MLogType.Error);
                return;
            }

            cellRectTrans = cellPrefab.GetComponent<RectTransform>();
            viewRangeScrollRect = viewRange.GetComponent<ScrollRect>();
            this.datas = datas;

            //���¼���Content��Size����ʵ���Ǹ��ݵ�ǰʵ����������"��Χ��"�Ĵ�С
            RecalculateContentSize(true);//������ʾ����(��Ļ�е�������ЩviewCellBundle������)

            //����---ˢ����ͼ
            RefreshAllCellInViewRange();
        }

        /// <summary>
        /// ���ݸ���ʱ�ֶ�ˢ��
        /// </summary>
        public void Refresh()
        {
            RecalculateContentSize();
            RefreshViewRangeData();
        }
        /// <summary>
        /// ���ݸ���ʱ�ֶ�ˢ��
        /// </summary>
        public void Refresh(ICollection<Data> datas)
        {
            if (datas.Count == 0) return;
            this.datas = datas;

            RecalculateContentSize();
            RefreshViewRangeData();
        }

        protected virtual void Update()
        {
            if (datas == null) return;
            UpdateDisplay();
        }

        /// <summary>
        /// ˲���ƶ���ĳ��(ȡֵ��Χ[0,1])
        /// </summary>
        public void MoveTo(float x)
        {
            x = Mathf.Clamp01(x);

            if (direction == MScrollViewDirection.Vertical)
            {
                //float max = content.sizeDelta.y - viewRange.sizeDelta.y;
                float max = content.sizeDelta.y - viewRange.rect.height;
                if (max < 0) return;
                content.anchoredPosition = new Vector2(content.anchoredPosition.x, max * x);
            }
            else if (direction == MScrollViewDirection.Horizontal)
            {
                //float max = content.sizeDelta.x - viewRange.sizeDelta.x;
                float max = content.sizeDelta.x - viewRange.rect.width;
                if (max < 0) return;
                content.anchoredPosition = new Vector2(-max * x, content.anchoredPosition.y);
            }
            viewRangeScrollRect.velocity = Vector2.zero;//��ֹ����λ�ú�ScrollView���ڻ���״̬
        }
        /// <summary>
        /// ˲���ƶ���ĳһBundle(��/��)
        /// </summary>
        /// <param playerName="count"></param>
        public void MoveToBundle(int count)
        {
            //�Դ�ֱģʽΪ����
            //�ƶ�һ��Ҳ�����ƶ�ItemSize.y���룬ֻҪ֪��"�������"����
            //Tip��"�������"�������һ�У����ǵ������ö�ʱ��������ʾ������Ԫ��
            if (direction == MScrollViewDirection.Vertical)
            {
                //int viewRangeMax = (int)(viewRange.sizeDelta.y / ItemSize.y) + 1;
                int viewRangeMax = (int)(viewRange.rect.height / ItemSize.y) + 1;
                int max = ItemCount - viewRangeMax + 1;
                if (max <= 0) return;
                count = Mathf.Clamp(count, 0, max);//���Ƶ������/��
                content.anchoredPosition = new Vector2(content.anchoredPosition.x, ItemSize.y * (count - 1));
            }
            else if (direction == MScrollViewDirection.Horizontal)
            {
                //int viewRangeMax = (int)(viewRange.sizeDelta.x / ItemSize.x) + 1;
                int viewRangeMax = (int)(viewRange.rect.width / ItemSize.x) + 1;
                int max = ItemCount - viewRangeMax + 1;
                if (max <= 0) return;
                count = Mathf.Clamp(count, 0, max);//���Ƶ������/��

                content.anchoredPosition = new Vector2(-ItemSize.x * (count - 1), content.anchoredPosition.y);
            }
            viewRangeScrollRect.velocity = Vector2.zero;//��ֹ����λ�ú�ScrollView���ڻ���״̬
        }
        /// <summary>
        /// ˲���ƶ���ĳһ��Cell����Bundle(��/��)
        /// </summary>
        /// <param playerName="count"></param>
        public void MoveToCell(int count)
        {
            int bundleCount = count / itemCellCount;
            if (count % itemCellCount != 0) bundleCount++;

            MoveToBundle(bundleCount);
        }

        /// <summary>
        /// ����CellԤ����
        /// </summary>
        protected abstract void ResetCellData(Cell cell, Data data, int dataIndex);
        #endregion

        #region һ������
        private void UpdateDisplay()
        {
            RemoveOutOfRangeBundles();
            if (cellBundles.Count == 0) RefreshAllCellInViewRange();
            AddInOfRangeBundles();
        }

        private void RemoveOutOfRangeBundles()
        {
            if (direction == MScrollViewDirection.Vertical)
            {
                //Խ�Ͻ�
                if (cellBundles.Count == 0) return;

                CellBundle<Cell> bundle = cellBundles.First.Value;
                while (AboveViewRange(bundle.position))
                {
                    ReleaseViewBundle(bundle);//��������
                    cellBundles.RemoveFirst();

                    if (cellBundles.Count == 0) break;

                    bundle = cellBundles.First.Value;
                }
                //Խ�½�
                if (cellBundles.Count == 0) return;

                bundle = cellBundles.Last.Value;
                while (UnderViewRange(bundle.position))
                {
                    ReleaseViewBundle(bundle);//��������
                    cellBundles.RemoveLast();

                    if (cellBundles.Count == 0) break;

                    bundle = cellBundles.Last.Value;
                }
            }
            else if (direction == MScrollViewDirection.Horizontal)
            {
                //Խ���
                if (cellBundles.Count == 0) return;

                CellBundle<Cell> bundle = cellBundles.First.Value;
                while (InViewRangeLeft(bundle.position))
                {
                    ReleaseViewBundle(bundle);//��������
                    cellBundles.RemoveFirst();

                    if (cellBundles.Count == 0) break;

                    bundle = cellBundles.First.Value;
                }
                //Խ�ҽ�
                if (cellBundles.Count == 0) return;

                bundle = cellBundles.Last.Value;
                while (InViewRangeRight(bundle.position))
                {
                    ReleaseViewBundle(bundle);//��������
                    cellBundles.RemoveLast();

                    if (cellBundles.Count == 0) break;

                    bundle = cellBundles.Last.Value;
                }
            }
        }

        private void AddInOfRangeBundles()
        {
            AddHeadBundles();
            AddTailBundles();
        }
        private void AddHeadBundles()
        {
            if (cellBundles.Count == 0) return;

            //��ͷ��Ԫ������������ͷ����λ��,�����λ���Ƿ�����ʾ�����������ʾ���������ɶ�Ӧ��Ŀ
            CellBundle<Cell> bundle = cellBundles.First.Value;

            Vector2 offset = default;
            if (direction == MScrollViewDirection.Vertical)
                offset = new Vector2(0, ItemSize.y);
            else if (direction == MScrollViewDirection.Horizontal)
                offset = new Vector2(-ItemSize.x, 0);

            Vector2 newHeadBundlePos = bundle.position + offset;//�жϵ㣬�ô�Ϊ��ǰԽ��λ��

            while (OnViewRange(newHeadBundlePos))
            {
                int caculatedIndex = GetIndex(newHeadBundlePos);
                int index = bundle.index - 1;

                if (index < 0) break;
                if (caculatedIndex != index)//����
                {
                    MLog.Print($"{typeof(MScrollView<Cell, Data>)}������ó�����-<{caculatedIndex}>��ʵ�ʼ�������-<{index}>��������ֵ����ȣ�����", MLogType.Error);
                    break;
                } 

                bundle = GetViewBundle(index, newHeadBundlePos, CellSize, cellSpace);
                cellBundles.AddFirst(bundle);

                newHeadBundlePos = bundle.position + offset;
            }
        }
        private void AddTailBundles()
        {
            if (cellBundles.Count == 0) return;

            //��β��Ԫ������������ͷ����λ��,�����λ���Ƿ�����ʾ�����������ʾ���������ɶ�Ӧ��Ŀ
            CellBundle<Cell> bundle = cellBundles.Last.Value;
            Vector2 offset = default;
            if (direction == MScrollViewDirection.Vertical)
                offset = new Vector2(0, -ItemSize.y);
            else if (direction == MScrollViewDirection.Horizontal)
                offset = new Vector2(ItemSize.x, 0);

            Vector2 newTailBundlePos = bundle.position + offset;

            while (OnViewRange(newTailBundlePos))
            {
                int caculatedIndex = GetIndex(newTailBundlePos);
                int index = bundle.index + 1;

                if (index >= ItemCount) break;
                if (caculatedIndex != index)//����
                {
                    MLog.Print($"{typeof(MScrollView<Cell, Data>)}������ó�����-<{caculatedIndex}>��ʵ�ʼ�������-<{index}>��������ֵ����ȣ�����", MLogType.Error);
                    break;
                }

                bundle = GetViewBundle(index, newTailBundlePos, CellSize, cellSpace);
                cellBundles.AddLast(bundle);

                newTailBundlePos = bundle.position + offset;
            }
        }

        private void RefreshAllCellInViewRange()
        {
            int itemCount = ItemCount;//����
            //Vector2 viewRangeSize = viewRange.sizeDelta;//�����С
            //ע�⣺sizeDelta������������ģʽ������ֻ����rect��ȡ����
            Vector2 viewRangeSize = new Vector2(viewRange.rect.width, viewRange.rect.height);//�����С
            Vector2 itemSize = ItemSize;//Cell��С(����)
            Vector2 cellSize = CellSize;//Cell��С(ȥ�����)
            Vector2 cellSpace = this.cellSpace;//���

            if (direction == MScrollViewDirection.Vertical)
            {
                //��ȡScrollView�Ķ��͵�
                Vector2 topPos = -content.anchoredPosition;
                Vector2 bottomPos = new Vector2(topPos.x, topPos.y - viewRangeSize.y);

                //��ȡ���͵���������
                int startIndex = GetIndex(topPos);
                int endIndex = GetIndex(bottomPos);
                //forѭ��������е�bundle
                for (int i = startIndex; i <= endIndex && i < itemCount; i++)
                {
                    Vector2 pos = new Vector2(content.anchoredPosition.x, -i * itemSize.y);
                    var bundle = GetViewBundle(i, pos, cellSize, cellSpace);//ÿ��bundle֮��ֻ��λ�ò�ͬ
                    cellBundles.AddLast(bundle);
                }
            }
            else if (direction == MScrollViewDirection.Horizontal)//ͬ��
            {
                Vector2 leftPos = -content.anchoredPosition;
                Vector2 rightPos = new Vector2(leftPos.x + viewRangeSize.x, leftPos.y);

                int startIndex = GetIndex(leftPos);
                int endIndex = GetIndex(rightPos);

                for (int i = startIndex; i <= endIndex && i < itemCount; i++)
                {
                    Vector2 pos = new Vector2(i * itemSize.x, content.anchoredPosition.y);
                    var bundle = GetViewBundle(i, pos, cellSize, cellSpace);
                    cellBundles.AddLast(bundle);
                }
            }
        }

        private void RefreshViewRangeData()
        {
            if (cellBundles.Count() == 0) return;

            bool flag = false;
            int count = 0;

            //������ǰ��ʾ������Bundle
            foreach (var bundle in cellBundles)
            {
                if (flag == true) break;

                count++;
                int startIndex = bundle.index * itemCellCount;
                int endIndex = startIndex + bundle.Cells.Length - 1;

                //��ֹԽ��
                //���������һ��Ԫ�ز�����Ҳ������ˢ�����ݵ���datas�����½��Ӷ�����
                if (endIndex >= datas.Count)
                {
                    flag = true;
                    endIndex = datas.Count - 1;
                }

                int i = startIndex, j = 0;
                for (; i <= endIndex && j < bundle.Cells.Length; i++, j++)
                {
                    ResetCellData(bundle.Cells[j], datas.ElementAt(i), i);//Ϊÿ��Cellˢ��
                }

                //��������һ�У���Ҫ�����е�ʣ��Cell����
                if (flag)
                {
                    while (j < bundle.Cells.Length)
                    {
                        try
                        {
                            bundle.Cells[j++].gameObject.SetActive(false);
                        }
                        catch (System.Exception)
                        {
                            throw;
                        }
                    }
                }
            }

            //�����ǰ�˳�(flag=true)����ʣ�������Ƴ�����
            int remainCount = cellBundles.Count() - count;
            while (remainCount > 0)
            {
                remainCount--;
                ReleaseViewBundle(cellBundles.Last.Value);
                cellBundles.RemoveLast();
            }
        }

        private void RecalculateContentSize(bool reset = false)
        {
            int itemCount = ItemCount;//���������������100��Ԫ��ÿ�з�3������ô�ͻ���34��

            //����ê���Լ���С
            //��С---�Դ�ֱ�ƶ�Ϊ��������x����������yΪ����*�߶�-1�ݼ��
            if (direction == MScrollViewDirection.Vertical)
            {
                if (reset)
                {
                    content.anchorMin = verticalContentAnchorMin;
                    content.anchorMax = verticalContentAnchorMax;
                    content.pivot = verticalContentPivot;
                    content.anchoredPosition = Vector2.zero;
                    content.offsetMin = Vector2.zero;
                    content.offsetMax = Vector2.zero;
                }

                //Tip��ItemSize�Ǳ���+�������Ҫ��CellSize�����
                //��һ�ݼ������Ϊ����3�У���ô��ʵֻ��2�ݼ��������3��
                content.sizeDelta = new Vector2(content.sizeDelta.x, itemCount * ItemSize.y - cellSpace.y);
            }
            else if (direction == MScrollViewDirection.Horizontal)
            {
                if (reset)
                {
                    content.anchorMin = horizontalContentAnchorMin;
                    content.anchorMax = horizontalContentAnchorMax;
                    content.pivot = horizontalContentPivot;
                    content.anchoredPosition = Vector2.zero;
                    content.offsetMin = Vector2.zero;
                    content.offsetMax = Vector2.zero;
                }

                content.sizeDelta = new Vector2(itemCount * ItemSize.x - cellSpace.x, content.sizeDelta.y);
            }
        }

        /// <summary>
        /// ����Bundle(��ʼ��������)
        /// </summary>
        private CellBundle<Cell> GetViewBundle(int itemIndex, Vector2 postion, Vector2 cellSize, Vector2 cellSpace)
        {
            CellBundle<Cell> bundle;
            Vector2 cellOffset = default;

            //��������ģʽ��ÿ��Bundle�е�Cell�Ǻ����Ų��ģ���Ҫ����ƫ��ֵ(������/������)
            if (direction == MScrollViewDirection.Vertical)
            {
                cellOffset = new Vector2(cellSize.x + cellSpace.x, 0);
            }
            //ͬ��
            else if (direction == MScrollViewDirection.Horizontal)
            {
                cellOffset = new Vector2(0, -(cellSize.y + cellSpace.y));
            }

            if (cellBundlePool.Count == 0)//������ִ��ʱ�����г�ʼ������
            {
                //��ʼ��
                bundle = new CellBundle<Cell>(itemCellCount);
                bundle.position = postion;
                bundle.index = itemIndex;
                //j---bundle�е�index��i---Ԫ���ݵ�index
                int i = itemIndex * itemCellCount, j = 0;
                int end = itemIndex * itemCellCount + bundle.Cells.Length;

                for (; j < bundle.Cells.Length && i < end; j++, i++)
                {
                    bundle.Cells[j] = Instantiate(cellPrefab, content);//����---��ʼ��
                    bundle.Cells[j].gameObject.SetActive(false);
                    //����λ��
                    RectTransform rectTransform = bundle.Cells[j].GetComponent<RectTransform>();
                    ResetRectTransform(rectTransform);
                    rectTransform.anchoredPosition = postion + j * cellOffset;

                    if (i < 0 || i >= datas.Count)
                    {
                        continue;
                    }

                    ResetCellData(bundle.Cells[j], datas.ElementAt(i), i);
                }
            }
            else//�Ƴ�Bundle������
            {
                bundle = cellBundlePool.Dequeue();
                bundle.position = postion;
                bundle.index = itemIndex;
                int i = itemIndex * itemCellCount, j = 0;
                int end = itemIndex * itemCellCount + bundle.Cells.Length;
                for (; j < bundle.Cells.Length && i < end; j++, i++)
                {
                    //����λ��
                    RectTransform rectTransform = bundle.Cells[j].GetComponent<RectTransform>();
                    ResetRectTransform(rectTransform);
                    rectTransform.anchoredPosition = postion + j * cellOffset;

                    if (i < 0 || i >= datas.Count)
                    {
                        continue;
                    }

                    ResetCellData(bundle.Cells[j], datas.ElementAt(i), i);
                }
            }
            return bundle;
        }
        #endregion

        #region ���ܺ���
        private void ReleaseViewBundle(CellBundle<Cell> viewCellBundle)
        {
            viewCellBundle.Clear();
            cellBundlePool.Enqueue(viewCellBundle);
        }

        private void ResetRectTransform(RectTransform rectTransform)
        {
            rectTransform.pivot = cellPivot;
            rectTransform.anchorMin = cellAnchorMin;
            rectTransform.anchorMax = cellAnchorMax;
        }

        private int GetIndex(Vector2 position)
        {
            int index = -1;
            if (direction == MScrollViewDirection.Vertical)
            {
                index = Mathf.RoundToInt(-position.y / ItemSize.y);
                return index;
            }
            else if (direction == MScrollViewDirection.Horizontal)
            {
                index = Mathf.RoundToInt(position.x / ItemSize.x);
            }
            return index;
        }

        private bool AboveViewRange(Vector2 position)
        {
            Vector2 relativePos = CaculateRelativePostion(position);
            return relativePos.y >= ItemSize.y;
        }
        private bool UnderViewRange(Vector2 position)
        {
            Vector2 relativePos = CaculateRelativePostion(position);
            //return relativePos.y <= -viewRange.sizeDelta.y;
            return relativePos.y <= -viewRange.rect.height;
        }
        private bool InViewRangeLeft(Vector2 position)
        {
            Vector2 relativePos = CaculateRelativePostion(position);
            return relativePos.x <= -ItemSize.x;
        }
        private bool InViewRangeRight(Vector2 position)
        {
            Vector2 relativePos = CaculateRelativePostion(position);
            //return relativePos.x >= viewRange.sizeDelta.x;
            return relativePos.x >= viewRange.rect.width;
        }
        private bool OnViewRange(Vector2 position)
        {
            if (direction == MScrollViewDirection.Horizontal)
            {
                return !InViewRangeLeft(position) && !InViewRangeRight(position);
            }
            else if (direction == MScrollViewDirection.Vertical)
            {
                return !AboveViewRange(position) && !UnderViewRange(position);
            }
            return false;
        }

        private Vector2 CaculateRelativePostion(Vector2 curPosition)
        {
            Vector2 relativePosition = default;
            if (direction == MScrollViewDirection.Horizontal)
            {
                relativePosition = new Vector2(curPosition.x + content.anchoredPosition.x, curPosition.y);
            }
            else if (direction == MScrollViewDirection.Vertical)
            {
                relativePosition = new Vector2(curPosition.x, curPosition.y + content.anchoredPosition.y);
            }
            return relativePosition;
        }
        #endregion
    }
}