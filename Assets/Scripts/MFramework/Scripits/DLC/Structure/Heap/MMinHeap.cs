using System;
using System.Collections.Generic;

namespace MFramework.DLC
{
    public class MMinHeap<T>
    {
        private MList<T> _heap;

        private IComparer<T> comparer;

        public int Count
        {
            get
            {
                return _heap.Count;
            }
        }

        public MMinHeap()
        {
            _heap = new MList<T>();

            this.comparer = Comparer<T>.Default;
        }
        public MMinHeap(IComparer<T> comparer)
        {
            _heap = new MList<T>();

            this.comparer = comparer;
        }
        public MMinHeap(IEnumerable<T> nums)
        {
            this.comparer = Comparer<T>.Default;

            _heap = new MList<T>(nums);

            int size = GetParent(Count - 1);//��ȡ��������(���ڵ㵽���һ����Ҷ�ӽڵ�)
            //�����ÿһ����Ҷ�ӽڵ���жѻ�����
            for (int i = size; i >= 0; i--)
            {
                SiftDown(i);
            }
        }
        public MMinHeap(IEnumerable<T> nums, IComparer<T> comparer)
        {
            this.comparer = comparer;

            _heap = new MList<T>(nums);

            int size = GetParent(Count - 1);//��ȡ��������(���ڵ㵽���һ����Ҷ�ӽڵ�)
            //�����ÿһ����Ҷ�ӽڵ���жѻ�����
            for (int i = size; i >= 0; i--)
            {
                SiftDown(i);
            }
        }

        public T Peek()
        {
            return _heap[0];
        }

        public void Push(T item)
        {
            _heap.Add(item);//��Ԫ������ѵ�
            SiftUp(Count - 1);//�Ե����ϵĶѻ�����
        }

        public T Pop()
        {
            if (Count == 0) throw new Exception();

            Swap(0, Count - 1);//�����Ѷ�Ԫ����ѵ�Ԫ��

            //�ݴ�󵯳��ѵ�Ԫ��
            T item = _heap[Count - 1];
            _heap.RemoveAt(Count - 1);

            SiftDown(0);//�Զ����¶ѻ�����

            return item;
        }

        private void SiftUp(int i)
        {
            while (true)
            {
                int p = GetParent(i);

                //�������㣬����ִ��(���ڵ��������� �� ��Ԫ�ش��ڵ��ڸ�Ԫ��)
                if (p < 0 || comparer.Compare(_heap[i], _heap[p]) >= 0)
                {
                    break;
                }

                Swap(i, p);//�����ڵ��е�ֵ
                i = p;//����һ��
            }
        }

        private void SiftDown(int i)
        {
            while (true)
            {
                //��ȡ���ҽڵ��뵱ǰ�ڵ�
                int l = GetLeft(i), r = GetRight(i), max = i;

                //�ӽڵ�δ���� �� ���ڵ㲢����󣬴�ʱ����max
                if (l < Count && comparer.Compare(_heap[l], _heap[max]) < 0)
                {
                    max = l;
                }
                if (r < Count && comparer.Compare(_heap[r], _heap[max]) < 0)
                {
                    max = r;
                }

                if (max == i)//�������㣬����ִ��(���ڵ�Ϊ���ֵ)
                {
                    break;
                }

                Swap(i, max);//�����ڵ��е�ֵ
                i = max;//����һ��
            }
        }

        private void Swap(int i1, int i2)
        {
            T temp = _heap[i1];
            _heap[i1] = _heap[i2];
            _heap[i2] = temp;
        }

        private int GetLeft(int i)
        {
            return 2 * i + 1;
        }
        private int GetRight(int i)
        {
            return 2 * i + 2;
        }
        private int GetParent(int i)
        {
            return (i - 1) / 2;
        }
    }
}