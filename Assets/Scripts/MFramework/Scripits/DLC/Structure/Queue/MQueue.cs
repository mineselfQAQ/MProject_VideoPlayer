using System;
using System.Collections;

namespace MFramework.DLC
{
    public class MQueue : IEnumerable
    {
        private object[] _array;//ѭ�����飬ͨ��_head/_tailʵ��

        private int _head;//һ��ʼ��0����ɾ��Ԫ�غ�+1
        private int _tail;//��_size��(ĩԪ��λ�õĺ�һ��λ��)

        private int _size;

        private int _growFactor;

        public int Count => _size;

        public MQueue() : this(32, 2.0f) { }
        public MQueue(int capacity) : this(capacity, 2.0f) { }
        public MQueue(int capacity, float growFactor)
        {
            //����������С��0
            if (capacity < 0)
            {
                throw new Exception();
            }

            //������������GrowFactor��Χ---[1,10]
            if (!((double)growFactor >= 1.0) || !((double)growFactor <= 10.0))
            {
                throw new Exception();
            }

            _array = new object[capacity];
            _head = 0;
            _tail = 0;
            _size = 0;
            _growFactor = (int)(growFactor * 100f);//�������Ǳ���2λС����2.11��211��2.1111��211

        }

        private void SetCapacity(int capacity)
        {
            object[] temp = new object[capacity];

            //��Ԫ�ؾͰ�Ǩ
            if (_size > 0)
            {
                if (_head < _tail)
                {
                    Array.Copy(_array, _head, temp, 0, _size);
                }
                else
                {
                    Array.Copy(_array, _head, temp, 0, _array.Length - _head);
                    Array.Copy(_array, 0, temp, _array.Length - _head, _tail);
                }
            }

            _array = temp;
            //��Ǩ���ͬʱ����������ͷָ���βָ���λ��������
            _head = 0;
            _tail = ((_size != capacity) ? _size : 0);//Ԫ�����˻�ָ����Ԫ��(��һ�����Դ�ŵ�λ�þ���0)
        }

        public void Enqueue(object o)
        {
            //������Ҫ����
            if (_size == _array.Length)
            {
                //���ݴ�С---����ԭ����*���캯���е�growFactor
                //ע�⣺����˳��Ϊ���ң�����/100��������ȡ�������Ծ��Ȼ����һ��
                //ע�⣺��long��int����Ҫ�ģ���Ϊ���ܷ���_array.Length*_growFactor���ñ�int��һ�㣬�������Ȳ�����ʧ
                int num = (int)((long)_array.Length * (long)_growFactor / 100);
                //�����������������4��û�У���ô��ʹ������޶�����4��Ԫ��
                if (num < _array.Length + 4)
                {
                    num = _array.Length + 4;
                }

                SetCapacity(num);
            }

            _array[_tail] = o;
            _tail = (_tail + 1) % _array.Length;//ѭ������
            _size++;
        }

        public object Dequeue()
        {
            //û��Ԫ�ز��ܳ���
            if (_size == 0)
            {
                throw new Exception();
            }

            //FIFO��ȡ��ͷָ���Ԫ��
            object result = _array[_head];
            //����
            _array[_head] = null;
            _head = (_head + 1) % _array.Length;//ͷָ�����(������Ԫ��)
            _size--;

            return result;
        }

        public object Peek()
        {
            //û��Ԫ�ز��ܲ鿴
            if (_size == 0)
            {
                throw new Exception();
            }

            return _array[_head];
        }

        public void Clear()
        {
            if (_head < _tail)//����������磺null 1(ͷ) 2 3 4 5 null(β)
            {
                Array.Clear(_array, _head, _size);
            }
            else//�ߵ�������磺4 5 null(β) null 1(ͷ) 2 3
            {
                Array.Clear(_array, _head, _array.Length - _head);
                Array.Clear(_array, 0, _tail);
            }

            _head = 0;
            _tail = 0;
            _size = 0;
        }


        public bool Contains(object o)
        {
            int num = _head;//���Ķ���(�Ӷ��׿�ʼ)
            int size = _size;//��Ҫ���ĸ���

            while (size-- > 0)
            {
                if (o == null)
                {
                    if (_array[num] == null)
                    {
                        return true;
                    }
                }
                else if (_array[num] != null && _array[num].Equals(o))
                {
                    return true;
                }

                num = (num + 1) % _array.Length;//�����һ������
            }
            return false;
        }

        /// <summary>
        /// ��������(��һ��Ԫ�ع�λ��0)
        /// </summary>
        public void TrimToSize()
        {
            SetCapacity(_size);
        }

        public IEnumerator GetEnumerator()
        {
            int num = _head;
            int size = _size;

            while (size-- > 0)
            {
                yield return _array[num];

                num = (num + 1) % _array.Length;
            }
        }
    }

    public static class MQueueExtension
    {
        public static void Print(this MQueue queue)
        {
            MLog.Print("���: ");

            if (queue.Count == 0)
            {
                MLog.Print("��Ԫ��");
                return;
            }

            string outputStr = "";
            foreach (var item in queue)
            {
                outputStr += $"{item} ";
            }
            MLog.Print(outputStr);
        }
    }
}