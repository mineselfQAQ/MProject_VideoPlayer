using System;
using System.Collections;

namespace MFramework.DLC
{
    public class MArrayList : IEnumerable
    {
        //Ĭ�ϳ�ʼ����
        private const int _defaultCapacity = 4;

        //������ŵ�����
        private object[] _items;
        //��ǰԪ�ظ���(�ڲ�ʹ��)
        private int _size;

        //����(���ɴ������)
        public int Capacity
        {
            get
            {
                return _items.Length;//ͨ��_items��ȷ������
            }
            set
            {
                //�������---�����ֵС�ڵ�ǰԪ�ظ���
                //����ζ�����ڼ�����5��Ԫ�أ���Capacity��Ϊ3��
                //��ô�͵�ɾ���������ڵ�Ԫ�أ����ǲ������
                if (value < _size)
                {
                    throw new Exception("Capacity�������ֵС�ڵ�ǰԪ�ظ���");
                }

                //�ų����---���������������ͬ����ô��������Ҫ�޸�
                if (value == _items.Length)
                {
                    return;
                }

                //һ�����---����ֵ����0��
                //�����Ϸ��ų���value < _size��
                //�����ۺ���������ֻ��value >= _size��Ҳ���������ֵ���ڵ�ǰԪ�ظ�����
                //��ʱ�ų��˵�����������������
                //��ô������С���������(����)������������(����)�����
                if (value > 0)
                {
                    //������Ǩ�Ƶ���������
                    object[] array = new object[value];
                    if (_size > 0)
                    {
                        Array.Copy(_items, 0, array, 0, _size);
                    }
                    _items = array;
                }
                else//һ����˵Ӧ�ò����������Է���һ���ﴴ����һ����ʼ����
                {
                    _items = new object[4];
                }
            }
        }

        //��ǰԪ�ظ���(���������ģ��ȼ���_size)
        public int Count => _size;

        //this������
        public object this[int index]
        {
            get
            {
                //�������
                if (index < 0 || index >= _size)
                {
                    throw new Exception("this��������index������Χ");
                }

                return _items[index];
            }
            set
            {
                //�������
                if (index < 0 || index >= _size)
                {
                    throw new Exception("this��������index������Χ");
                }

                _items[index] = value;
            }
        }

        //�޲ι��캯��
        public MArrayList()
        {
            _items = new object[0];
        }
        //�вι��캯��(��������)
        public MArrayList(int capacity)
        {
            //�������
            if (capacity < 0)
            {
                throw new Exception("���캯������������С��0");
            }

            if (capacity == 0)//�ȼ����޲ι��캯��
            {
                _items = new object[0];
            }
            else//һ�����
            {
                _items = new object[capacity];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int i = 0; i < _size; i++)
            {
                yield return _items[i];
            }
        }

        //*************
        //***��������***
        //*************
        private void EnsureCapacity()
        {
            //��Ԫ����������·������ݲ���
            if (_size == _items.Length)
            {
                //���ݲ�����һ����˵���Ƿ�����ֻ�е�����Ϊ0Ҳ����û�д�����ݵ�ʱ����Ƚ��г�ʼ��(4���ռ�)
                //Ҫע����ǣ�
                //��������Ǵ�����һ��int����
                //��Ϊ���ݲ���ֱ�Ӿ��ܽ��еģ���Ҫͨ��������������д����������Ҫͨ��Capacity��Set�������
                int num = ((_items.Length == 0) ? 4 : (_items.Length * 2));

                //������������
                if ((uint)num > 2146435071u)
                {
                    num = 2146435071;
                }

                //���������ݲ���
                Capacity = num;
            }
        }

        public int Add(object value)
        {
            //�������������ǰԪ�ظ���������������ʱ��
            //��ʱ�ٽ���Add��ʵ����Ҫ�������ݲ�����Ҳ����EnsureCapacity()��
            if (_size == _items.Length)
            {
                EnsureCapacity();
            }

            //������˵�����Ǻ���ͨ�����һ��Ԫ�أ�
            //��ô�����±�Ϊ_size��λ����Ӹ�Ԫ��
            _items[_size] = value;

            return _size++;//ע��Ԫ�ظ���������
        }

        public void Insert(int index, object obj)
        {
            //�������
            if (index < 0 || index >= _size)
            {
                throw new Exception("Insert()��index������Χ");
            }

            //��Add()��ͬ����Ȼ�ǲ��룬��ô���п��ܻᳬ������
            if (_size == _items.Length)
            {
                EnsureCapacity();
            }

            //��������£�ֻ��Ҫ��index�����Ԫ������Ƽ���
            //�ڴ�֮ǰ��ʹ����EnsureCapacity()ȷ���пռ���в���
            Array.Copy(_items, index, _items, index + 1, _size - index);

            //��indexλ�ò���
            _items[index] = obj;
            _size++;//ע��Ԫ�ظ���������
        }

        public void RemoveAt(int index)
        {
            //�������
            if (index < 0 || index >= _size)
            {
                throw new Exception("RemoveAt()��index������Χ");
            }

            _size--;
            //�˴���if�������õģ�
            //�ò������Ƶ����Ƴ����һ��Ԫ�ص��������ʱindex=_size��
            //��_size�Ѿ���ǰ�ƶ�һλ�����һ��Ԫ�ر�"�Ƴ�"��
            if (index < _size)
            {
                //��ʵ����Insert()�ķ������︴������������ݲ���
                Array.Copy(_items, index + 1, _items, index, _size - index);
            }

            //�����һ��Ԫ����Ϊnull
            _items[_size] = null;
        }

        public void Clear()
        {
            //ֻҪ�����л�����Ԫ�أ�����Ҫ����ȫ�����
            if (_size > 0)
            {
                //����Ԫ���ÿգ�����ʱ������ȫ���
                Array.Clear(_items, 0, _size);
                _size = 0;//ֻҪ_sizeΪ0���������������ϵ����
            }
        }

        public bool Contains(object obj)
        {
            //�������---��Ҫ��Ѱ��ֵΪnull
            if (obj == null)
            {
                //�����������飬�鿴�Ƿ���Ԫ��Ϊnull
                for (int i = 0; i < _size; i++)
                {
                    if (_items[i] == null)
                    {
                        return true;
                    }
                }
                return false;
            }

            //һ�����
            for (int j = 0; j < _size; j++)
            {
                //�����������飬
                //�ȿ�����Ԫ���Ƿ�Ϊnull(Ӧ����Ϊ�˽�ʡ���ܣ�Ϊnull�Ͳ��ü����ж���)
                //������㣬�������ж������Ƿ����
                if (_items[j] != null && _items[j].Equals(obj))
                {
                    return true;
                }
            }
            return false;
        }

        public int IndexOf(object value)
        {
            //ֱ��ʹ��Array�е�IndexOf()����ȡ����
            return Array.IndexOf((Array)_items, value, 0, _size);
        }

        public void Reverse()
        {
            //������˵�ᷭתȫ��
            Reverse(0, Count);
        }
        public void Reverse(int index, int count)
        {
            if (index < 0)
            {
                throw new Exception("Reverse()��index�ض���С��0");
            }
            if (count < 0)
            {
                //��Ȼ0Ҳ�ǲ�̫��ȷ�ģ����������ǿ��е�
                throw new Exception("Reverse()��count�ض���С��0");
            }

            if (_size - index < count)
            {
                //�������������������Χ�ˣ�����˵��
                //��ǰԪ����9������ʱ���õ�index=8,count=2��Ҳ����ζ����Ҫ������Ϊ8��λ�úͺ�һ��λ�ý��з�ת
                //������ʵ��ʱ�Ѿ�����Ԫ�ظ�����
                throw new Exception("Reverse()����������ȷ");
            }

            //ֱ��ʹ��Array�е�Reverse()���з�ת
            Array.Reverse(_items, index, count);
        }

        public void Sort()
        {
            //������˵����������
            Sort(0, Count, Comparer.Default);
        }
        public void Sort(int index, int count, IComparer comparer)
        {
            if (index < 0)
            {
                throw new Exception("Sort()��index�ض���С��0");
            }
            if (count < 0)
            {
                //��Ȼ0Ҳ�ǲ�̫��ȷ�ģ����������ǿ��е�
                throw new Exception("Sort()��count�ض���С��0");
            }

            if (_size - index < count)
            {
                //ͬReverse()
                throw new Exception("Sort()����������ȷ");
            }

            //ֱ��ʹ��Array�е�Sort()���з�ת
            Array.Sort(_items, index, count, comparer);
        }

        public void TrimToSize()
        {
            //ֻҪ��������Ϊ��ǰԪ�ظ�������ʵ���ǲü�����
            Capacity = _size;
        }
    }

    public static class MArrayListExtension
    {
        public static void Print(this MArrayList list)
        {
            MLog.Print("���: ");

            if (list.Count == 0)
            {
                MLog.Print("��Ԫ��");
                return;
            }

            string outputStr = "";
            foreach (var item in list)
            {
                outputStr += $"{item} ";
            }
            MLog.Print(outputStr);
        }
    }
}