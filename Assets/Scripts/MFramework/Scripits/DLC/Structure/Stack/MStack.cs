using System;
using System.Collections;

namespace MFramework.DLC
{
    public class MStack : IEnumerable
    {
        private const int _defaultCapacity = 10;//Ĭ��������

        private object[] _array;//Stack����

        private int _size;//��ǰԪ�ظ���

        public int Count => _size;//���ڹ����ĵ�ǰԪ�ظ���

        public MStack()
        {
            _array = new object[_defaultCapacity];
            _size = 0;
        }

        public MStack(int capacity)
        {
            if (capacity < 0)
            {
                throw new Exception();
            }

            if (capacity < 10)
            {
                capacity = 10;
            }

            _array = new object[capacity];
            _size = 0;
        }

        public bool IsEmpty => _size == 0;

        public void Push(object o)
        {
            //Ԫ����������Ҫ����
            if (_size == _array.Length)
            {
                object[] temp = new object[_array.Length * 2];
                Array.Copy(_array, 0, temp, 0, _size);
                _array = temp;
            }

            _array[_size] = o;
            _size++;
        }

        public object Pop()
        {
            //û��Ԫ�ز��ܵ���
            if (IsEmpty)
            {
                throw new Exception();
            }

            _size--;
            object result = _array[_size];
            _array[_size] = null;//�����һ��Ԫ����Ϊ��(���Բ��ã���Ϊ���ʲ���)
            return result;
        }

        public object Peek()
        {
            //û��Ԫ�ز��ܲ鿴
            if (IsEmpty)
            {
                throw new Exception();
            }

            return _array[_size - 1];
        }

        public void Clear()
        {
            Array.Clear(_array, 0, _size);
            _size = 0;
        }

        public bool Contains(object o)
        {
            int size = _size;
            //����˼����
            //��size=0��ʱ��whileѭ���˳������ǶԵ�
            //��size=1ʱ���ڲ����ʵ���_array[0]����������һ��Ԫ�أ���Ҳ�ǶԵ�
            while (size-- > 0)
            {
                if (o == null)
                {
                    if (_array[size] == null)
                    {
                        return true;
                    }
                }
                else if (_array[size] != null && _array[size].Equals(o))
                {
                    return true;
                }
            }
            return false;
        }

        public IEnumerator GetEnumerator()
        {
            for (int i = _size - 1; i >= 0; i--)
            {
                yield return _array[i];
            }
        }
    }
    public static class MStackExtension
    {
        public static void Print(this MStack stack)
        {
            MLog.Print("���: ");

            if (stack.Count == 0)
            {
                MLog.Print("��Ԫ��");
                return;
            }

            string outputStr = "";
            foreach (var item in stack)
            {
                outputStr += $"{item} ";
            }
            MLog.Print(outputStr);
        }
    }
}