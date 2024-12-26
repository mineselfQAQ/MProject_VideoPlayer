using System;
using System.Collections;
using System.Collections.Generic;

namespace MFramework.DLC
{
    public class MList<T> : IEnumerable<T>
    {
        private const int _deafultCapacity = 4;

        private T[] _items;

        private int _count;

        public int Count
        {
            get
            {
                return _count;
            }
        }

        public int Capacity
        {
            get
            {
                return _items.Length;
            }
            set
            {
                if (value < _count)
                {
                    throw new Exception();
                }

                if (value == _items.Length)
                {
                    return;
                }

                T[] newArray = new T[value];
                if (_count > 0)
                {
                    Array.Copy(_items, 0, newArray, 0, _count);
                }
                _items = newArray;
            }
        }

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= _count)
                {
                    throw new Exception();
                }

                return _items[index];
            }
            set
            {
                if (index < 0 || index >= _count)
                {
                    throw new Exception();
                }

                _items[index] = value;
            }
        }

        public MList()
        {
            _items = new T[_deafultCapacity];
        }
        public MList(int capacity)
        {
            if (capacity < 0) throw new Exception();

            _items = new T[capacity];
        }
        public MList(IEnumerable<T> nums)
        {
            if (nums == null) throw new Exception();

            _count = 0;
            _items = new T[0];
            foreach (T item in nums)
            {
                Add(item);
            }
        }

        public void Add(T item)
        {
            if (_count == _items.Length)//��Ҫ����
            {
                EnsureCapacity(_count + 1);
            }

            _items[_count++] = item;
        }

        public void Insert(int index, T item)
        {
            //ע��:���ﲻ��index>=_size����Ϊ���index=_sizeʱ����һ�ֿ���ֱ����β����ӵ���ʽ
            if (index < 0 || index > _count) throw new Exception();

            if (_count == _items.Length)//��Ҫ����
            {
                EnsureCapacity(_count + 1);
            }

            //һ���������(���Ųһλ)�����index=_size��Ҳ����˵����λ��ΪβԪ�غ�һλ����ô����Ų
            if (index < _count)
            {
                Array.Copy(_items, index, _items, index + 1, _count - index);
            }

            _items[index] = item;
            _count++;
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= _count) throw new Exception();

            _count--;//�ȼ�
            if (index < _count)//�ų�indexΪβԪ�����
            {
                Array.Copy(_items, index + 1, _items, index, _count - index);
            }
            _items[_count] = default(T);
        }

        /// <summary>
        /// ���Ԫ�أ���������
        /// </summary>
        public void Clear()
        {
            if (_count > 0)
            {
                Array.Clear(_items, 0, _count);
                _count = 0;
            }
        }

        public bool Contains(T item)
        {
            //����nullԪ�����
            if (item == null)
            {
                for (int i = 0; i < _count; i++)
                {
                    if (_items[i] == null)
                    {
                        return true;
                    }
                }
                return false;
            }

            //����һ��Ԫ�����
            for (int i = 0; i < _count; i++)
            {
                if (_items[i].Equals(item))
                {
                    return true;
                }
            }
            return false;
        }

        public int IndexOf(T item)
        {
            return Array.IndexOf(_items, item, 0, _count);
        }

        public void Reverse()
        {
            Reverse(0, _count);
        }
        public void Reverse(int index, int count)
        {
            //ֻҪȷ����ֵ����
            if (index < 0) throw new Exception();
            if (count < 0) throw new Exception();

            //�ɲ���Ԫ�رȽ�Ҫ����Ԫ���٣�������
            if (_count - index < count) throw new Exception();

            Array.Reverse(_items, index, count);
        }

        public void Sort()
        {
            Sort(0, _count, null);
        }
        public void Sort(int index, int count, IComparer<T> comparer)
        {
            //ֻҪȷ����ֵ����
            if (index < 0) throw new Exception();
            if (count < 0) throw new Exception();

            //�ɲ���Ԫ�رȽ�Ҫ����Ԫ���٣�������
            if (_count - index < count) throw new Exception();

            Array.Sort(_items, index, count, comparer);
        }

        public void TrimExcess()
        {
            int num = (int)((double)_items.Length * 0.9);//��ֵ��0.9������

            //���ռ���ʲ���(��û�г�����ֵ)�����Խ��вü�
            //Ҳ����˵�������Ѿ������ˣ���ô�������������Զ�����
            if (_count < num)
            {
                Capacity = _count;
            }
        }

        /// <summary>
        /// ȷ���������ڵ���min
        /// </summary>
        /// <param playerName="min"></param>
        private void EnsureCapacity(int min)
        {
            if (_items.Length < min)//ȷʵ����������
            {
                int num = ((_items.Length == 0) ? 4 : (_items.Length * 2));

                if ((uint)num > 2146435071u)//�������ֵ
                {
                    num = 2146435071;
                }

                if (num < min)//���ݺ��Ǳ���СֵС
                {
                    num = min;//ʹ����Сֵ
                }

                Capacity = num;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int i = 0; i < _count; i++)
            {
                yield return _items[i];
            }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            for (int i = 0; i < _count; i++)
            {
                yield return _items[i];
            }
        }
    }

    public static class MListExtension
    {
        public static void Print<T>(this MList<T> list)
        {
            MLog.Print("���: ");

            if (list.Count == 0)
            {
                MLog.Print("��Ԫ��");
                return;
            }

            string outputStr = "";
            foreach (T item in list)
            {
                outputStr += $"{item} ";
            }
            MLog.Print(outputStr);
        }
    }
}