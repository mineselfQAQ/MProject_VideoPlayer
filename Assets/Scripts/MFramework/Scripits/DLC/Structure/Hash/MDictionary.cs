using System;
using System.Collections;
using System.Collections.Generic;

namespace MFramework.DLC
{
    public class MDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        //������Hashtable�е�Bucket
        private struct Entry
        {
            public int hashCode;
            public int next;
            public TKey key;
            public TValue value;
        }

        public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
        {
            private MDictionary<TKey, TValue> dictionary;

            private KeyValuePair<TKey, TValue> current;

            private int index;

            private int getEnumeratorRetType;//ģʽ��һ��Ϊ2��Ҳ���Ǽ�ֵ��ģʽ(����)

            internal const int DictEntry = 1;

            internal const int KeyValuePair = 2;

            internal Enumerator(MDictionary<TKey, TValue> dictionary, int getEnumeratorRetType)
            {
                this.dictionary = dictionary;
                index = 0;
                this.getEnumeratorRetType = getEnumeratorRetType;
                current = default(KeyValuePair<TKey, TValue>);
            }

            public KeyValuePair<TKey, TValue> Current
            {
                get
                {
                    return current;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    if (index == 0 || index == dictionary.count + 1)
                    {
                        throw new Exception();
                    }

                    //ģʽ1��Ҳ���ǷǷ���ģʽ
                    if (getEnumeratorRetType == 1)
                    {
                        return new DictionaryEntry(current.Key, current.Value);
                    }

                    //ģʽ2����ֵ��ģʽ��Ҳ���Ƿ���ģʽ
                    return new KeyValuePair<TKey, TValue>(current.Key, current.Value);
                }
            }

            public void Dispose()
            {

            }

            public bool MoveNext()
            {
                while ((uint)index < (uint)dictionary.count)
                {
                    if (dictionary.entries[index].hashCode >= 0)
                    {
                        current = new KeyValuePair<TKey, TValue>(dictionary.entries[index].key, dictionary.entries[index].value);
                        index++;
                        return true;
                    }

                    index++;
                }

                index = dictionary.count + 1;
                current = default(KeyValuePair<TKey, TValue>);
                return false;
            }

            public void Reset()
            {
                index = 0;
                current = default(KeyValuePair<TKey, TValue>);
            }
        }

        public sealed class KeyCollection : IEnumerable<TKey>
        {
            public struct Enumerator : IEnumerator<TKey>
            {
                private MDictionary<TKey, TValue> dictionary;

                private int index;

                private TKey currentKey;

                public TKey Current//IEnumerator<T>��Current
                {
                    get
                    {
                        return currentKey;
                    }
                }

                object IEnumerator.Current//IEnuemrator��Current
                {
                    get
                    {
                        if (index == 0 || index == dictionary.count + 1)
                        {
                            throw new Exception();
                        }

                        return currentKey;
                    }
                }

                internal Enumerator(MDictionary<TKey, TValue> dictionary)
                {
                    this.dictionary = dictionary;
                    index = 0;
                    currentKey = default(TKey);
                }

                public void Dispose(){ }

                public bool MoveNext()
                {
                    while ((uint)index < (uint)dictionary.count)
                    {
                        if (dictionary.entries[index].hashCode >= 0)
                        {
                            currentKey = dictionary.entries[index].key;
                            index++;
                            return true;
                        }

                        index++;
                    }

                    index = dictionary.count + 1;
                    currentKey = default(TKey);
                    return false;
                }

                public void Reset()
                {
                    index = 0;
                    currentKey = default(TKey);
                }
            }

            private MDictionary<TKey, TValue> dictionary;

            public int Count
            {
                get
                {
                    return dictionary.Count;
                }
            }

            public KeyCollection(MDictionary<TKey, TValue> dictionary)
            {
                if (dictionary == null)
                {
                    throw new Exception();
                }

                this.dictionary = dictionary;
            }

            //����������Ҫ���أ�Ŀǰ���������Ȼ���1��Ȼ����3��Ȼ����2
            public Enumerator GetEnumerator()
            {
                return new Enumerator(dictionary);
            }
            IEnumerator IEnumerable.GetEnumerator()//IEnumerable��GetEnumerator()
            {
                return new Enumerator(dictionary);
            }
            IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator()//IEnumerable<T>��GetEnumerator()
            {
                return new Enumerator(dictionary);
            }
        }
        public sealed class ValueCollection : IEnumerable<TValue>
        {
            public struct Enumerator : IEnumerator<TValue>
            {
                private MDictionary<TKey, TValue> dictionary;

                private int index;

                private TValue currentValue;

                public TValue Current//IEnumerator<T>��Current
                {
                    get
                    {
                        return currentValue;
                    }
                }

                object IEnumerator.Current//IEnuemrator��Current
                {
                    get
                    {
                        if (index == 0 || index == dictionary.count + 1)
                        {
                            throw new Exception();
                        }

                        return currentValue;
                    }
                }

                internal Enumerator(MDictionary<TKey, TValue> dictionary)
                {
                    this.dictionary = dictionary;
                    index = 0;
                    currentValue = default(TValue);
                }

                public void Dispose() { }

                public bool MoveNext()
                {
                    while ((uint)index < (uint)dictionary.count)
                    {
                        if (dictionary.entries[index].hashCode >= 0)
                        {
                            currentValue = dictionary.entries[index].value;
                            index++;
                            return true;
                        }

                        index++;
                    }

                    index = dictionary.count + 1;
                    currentValue = default(TValue);
                    return false;
                }

                public void Reset()
                {
                    index = 0;
                    currentValue = default(TValue);
                }
            }

            private MDictionary<TKey, TValue> dictionary;

            public int Count
            {
                get
                {
                    return dictionary.Count;
                }
            }

            public ValueCollection(MDictionary<TKey, TValue> dictionary)
            {
                if (dictionary == null)
                {
                    throw new Exception();
                }

                this.dictionary = dictionary;
            }

            //����������Ҫ���أ�Ŀǰ���������Ȼ���1��Ȼ����3��Ȼ����2
            public Enumerator GetEnumerator()
            {
                return new Enumerator(dictionary);
            }
            IEnumerator IEnumerable.GetEnumerator()//IEnumerable��GetEnumerator()
            {
                return new Enumerator(dictionary);
            }
            IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()//IEnumerable<T>��GetEnumerator()
            {
                return new Enumerator(dictionary);
            }
        }



        private int[] buckets;//entries����

        private Entry[] entries;//ʵ�ʴ洢Ԫ�ص�����

        private int count;//��ǰ���ڵ�entries��λ��  Tip��������Ϊ��Ԫ���������������Remove()�������ᷢ���仯

        private int freeList;

        private int freeCount;

        private IEqualityComparer<TKey> comparer;//���ڽ���Equals()��GetHashCode()����

        private KeyCollection keys;

        private ValueCollection values;

        //׼ȷ��Ԫ������
        public int Count
        {
            get
            {
                return count - freeCount;
            }
        }

        public MDictionary() : this(0, null) { }//Ĭ������Ϊ0������ζ�Ų�����Ҳ��Initialize()
        public MDictionary(int capacity) : this(capacity, null){ }
        public MDictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            if (capacity < 0)
            {
                throw new Exception();
            }

            if (capacity > 0)
            {
                Initialize(capacity);
            }

            this.comparer = comparer ?? EqualityComparer<TKey>.Default;
        }

        public TValue this[TKey key]
        {
            get
            {
                int num = FindEntry(key);//Ѱ��Entry
                if (num >= 0)//�������Entry�ͷ���
                {
                    return entries[num].value;
                }

                throw new Exception();
            }
            set
            {
                Insert(key, value, add: false);//��ֵ
            }
        }

        public KeyCollection Keys
        {
            get
            {
                if (keys == null)
                {
                    keys = new KeyCollection(this);
                }
                return keys;
            }
        }
        public ValueCollection Values
        {
            get
            {
                if (values == null)
                {
                    values = new ValueCollection(this);
                }
                return values;
            }
        }

        public void Add(TKey key, TValue value)
        {
            Insert(key, value, add: true);
        }

        public bool Remove(TKey key)
        {
            if (key == null)
            {
                throw new Exception();
            }

            if (buckets != null)
            {
                int num = comparer.GetHashCode(key) & 0x7FFFFFFF;//hashcode
                int num2 = num % buckets.Length;//index
                int num3 = -1;
                //ͬInsert()
                for (int num4 = buckets[num2]; num4 >= 0; num4 = entries[num4].next)
                {
                    //�ҵ���Ҫɾ����Entry
                    if (entries[num4].hashCode == num && comparer.Equals(entries[num4].key, key))
                    {
                        //Tip��ֻ����һ�ֿ��ܣ�
                        //���;�з���entries[num4].next < 0�Ļ�����һ��ѭ���ͻ��˳��ˣ�Ҳ��û�л���õ�num3 < 0��
                        if (num3 < 0)//��һ�������ˣ�������ֱ�Ӹ���(ͷɾ)
                        {
                            buckets[num2] = entries[num4].next;
                        }
                        else//һ�����������ɾ����ʽ(A->B->C��ΪA->C)
                        {
                            entries[num3].next = entries[num4].next;//Tip:��ʱ��num3������һ��Ԫ��
                        }

                        entries[num4].hashCode = -1;
                        entries[num4].next = freeList;//����1
                        entries[num4].key = default(TKey);
                        entries[num4].value = default(TValue);
                        freeList = num4;//����2
                        freeCount++;
                        return true;
                    }

                    num3 = num4;//num3�������һ��index
                }
            }

            return false;
        }

        public bool ContainsKey(TKey key)
        {
            return FindEntry(key) >= 0;
        }

        public bool ContainsValue(TValue value)
        {
            if (value == null)
            {
                for (int i = 0; i < count; i++)
                {
                    if (entries[i].hashCode >= 0 && entries[i].value == null)
                    {
                        return true;
                    }
                }
            }
            else
            {
                EqualityComparer<TValue> @default = EqualityComparer<TValue>.Default;
                for (int j = 0; j < count; j++)
                {
                    if (entries[j].hashCode >= 0 && @default.Equals(entries[j].value, value))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void Clear()
        {
            if (count > 0)
            {
                for (int i = 0; i < buckets.Length; i++)
                {
                    buckets[i] = -1;
                }

                Array.Clear(entries, 0, count);
                count = 0;
                freeList = -1;
                freeCount = 0;
            }
        }

        /// <summary>
        /// ���캯���еĳ�ʼ������
        /// </summary>
        private void Initialize(int capacity)
        {
            //��С---������������ȡ��С����
            int prime = MHashHelpers.GetPrime(capacity);
            buckets = new int[prime];
            //��ʼ��Ϊ-1
            for (int i = 0; i < buckets.Length; i++)
            {
                buckets[i] = -1;
            }
            entries = new Entry[prime];
            freeList = -1;
        }

        /// <summary>
        /// �����ֵ�Ե��ڲ�ʵ��
        /// </summary>
        private void Insert(TKey key, TValue value, bool add)
        {
            if (key == null)
            {
                throw new Exception();
            }

            //Ͱ��δ��ʼ������ҪInitialize()����Ӧ�޲ι��캯�����
            if (buckets == null)
            {
                Initialize(0);//��ζ������С������Ϊ3
            }

            //֮����ȡ��31λ������Ϊ��Ҫ��֤����������������������������ȡ�����
            int num = comparer.GetHashCode(key) & 0x7FFFFFFF;//ȡ��31λhashcode
            int num2 = num % buckets.Length;//index(��hashcodeӳ����Ͱ��Χ)
            int num3 = 0;//����Hashtable�е�count��������Insert()�м�ʱ�����

            //=====�������=====
            //���̣�
            //��buckets�м��index����intֵ������ô��ѱ���ֵ��˵����ָ�򣬿��Խ���ѭ��
            //ÿ�ζ�����entries��num4����key��hashcode��
            //�����ƥ�䣬��˵���ҵ���Ӧλ��
            //�������ƥ�䣬��˵��û���Ҷ�λ�ã���Ҫͨ����Entry�е�next�ҵ���һλ��
            for (int num4 = buckets[num2]; num4 >= 0; num4 = entries[num4].next)
            {
                //�����е�hashcode/key�뵱ǰ�������ͬ---�ҵ�����ͬ��key
                if (entries[num4].hashCode == num && comparer.Equals(entries[num4].key, key))
                {
                    if (add)//��ʱ���Ԫ���ǲ���ȷ�ģ�ֻ�ܸ���
                    {
                        throw new Exception();
                    }
                    //����Ԫ��
                    entries[num4].value = value;
                    return;
                }

                num3++;//ͳ����һ������洢Ԫ������
            }

            //=====��ȡ�����Ԫ�ص�entries����=====
            int num5;
            if (freeCount > 0)//����п���λ��
            {
                num5 = freeList;//ȡ������һ��λ��
                freeList = entries[num5].next;//������һ������λ��
                freeCount--;
            }
            else//���û�п���λ�ã���ôֻ���Լ�ȥ����
            {
                if (count == entries.Length)//������������Ҫ����
                {
                    Resize();//����
                    num2 = num % buckets.Length;//buckets���������仯�����¼���index
                }

                num5 = count;//���Ԫ����entries�����е�λ��
                count++;//��ǰ������һλ��
            }

            //=====������=====
            entries[num5].hashCode = num;
            //���û�з�����ײ��nextֵ��ΪĬ��ֵ-1
            //���������ײ�������ָ���磺
            //��һ��indexΪ4����ô��entries[0]�����Ÿ����ݣ���ʱbuckets[4]=0
            //�ڶ���index��Ϊ4����ô�����˹�ϣ��ײ����entries[1]����������ݣ���������next�ᱻ��Ϊ0��ͬʱbuckets[4]Ҳ������Ϊ1
            //��ô������ʵ��һ�������ϵ��
            //buckets[4]--->entries[1]--->entries[0]
            entries[num5].next = buckets[num2];
            entries[num5].key = key;
            entries[num5].value = value;
            buckets[num2] = num5;//ʹbuckets[nums2]��ָ��entreis��nums5��

            //num3������ζ�ų����˼������Ĺ�ϣ��ͻ(ÿ�ζ��ҵ�ͬһ��index)
            if (num3 > 100 /*&& MHashHelpers.IsWellKnownEqualityComparer(comparer)*/)
            {
                //Tip��ԭ����Բ��������comparer�������´���
                //comparer = (IEqualityComparer<TKey>)HashHelpers.GetRandomizedEqualityComparer(comparer);
                Resize(entries.Length, forceNewHashCodes: true);
            }
        }

        private void Resize()
        {
            Resize(MHashHelpers.ExpandPrime(count), forceNewHashCodes: false);
        }
        private void Resize(int newSize, bool forceNewHashCodes)
        {
            //��ʼ����buckets
            int[] array = new int[newSize];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = -1;
            }
            //��ʼ����entries
            Entry[] array2 = new Entry[newSize];
            Array.Copy(entries, 0, array2, 0, count);

            //ǿ�Ƹ���Hashcode���
            if (forceNewHashCodes)
            {
                for (int j = 0; j < count; j++)
                {
                    if (array2[j].hashCode != -1)
                    {
                        array2[j].hashCode = comparer.GetHashCode(array2[j].key) & 0x7FFFFFFF;//ȥ�����λ
                    }
                }
            }

            //�������ӣ��������̣�
            //��ʱ���ǵ�buckets�Ѿ�����ˣ�������Ҫ�����е�entries����buckets
            //��Ԫ������ĵ�һ����ʼ��ֻҪ������hashCode����˵���ô��д���Ԫ��
            //Ȼ�����ν�entry����ͷ�巨���Ӽ���
            for (int k = 0; k < count; k++)
            {
                if (array2[k].hashCode >= 0)//���λΪ0ʱ
                {
                    //���¾��ǣ���Ԫ��ָ����һ��Ԫ�أ�buckets[newIndex]ָ����Ԫ��
                    int num = array2[k].hashCode % newSize;//newIndex
                    array2[k].next = array[num];
                    array[num] = k;
                }
            }

            buckets = array;
            entries = array2;
        }

        private int FindEntry(TKey key)
        {
            if (key == null)
            {
                throw new Exception();
            }

            if (buckets != null)
            {
                int num = comparer.GetHashCode(key) & 0x7FFFFFFF;//hashcode
                //���β�ѯͰ�����е�ÿһ��Entry
                for (int num2 = buckets[num % buckets.Length]; num2 >= 0; num2 = entries[num2].next)
                {
                    //�ҵ�ƥ��Entry�ͷ���
                    if (entries[num2].hashCode == num && comparer.Equals(entries[num2].key, key))
                    {
                        return num2;
                    }
                }
            }

            return -1;
        }

        //����������Ҫ���أ����Ȼ���1
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this, 2);
        }
        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return new Enumerator(this, 2);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this, 2);
        }
    }
}