using System;
using System.Collections;

namespace MFramework.DLC
{
    public struct MDictionaryEntry
    {
        private object _key;

        private object _value;

        public object Key
        {
            get
            {
                return _key;
            }
            set
            {
                _key = value;
            }
        }

        public object Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        public MDictionaryEntry(object key, object value)
        {
            _key = key;
            _value = value;
        }
    }

    public interface IDictionaryEnumerator : IEnumerator
    {
        MDictionaryEntry Entry { get; }
        object Key { get; }
        object Value { get; }
    }

    public class MHashtable : IEnumerable
    {
        private class KeyCollection : IEnumerable
        {
            private MHashtable _hashtable;

            internal KeyCollection(MHashtable hashtable)
            {
                _hashtable = hashtable;
            }

            public IEnumerator GetEnumerator()
            {
                return new MHashtableEnumerator(_hashtable, 1);
            }
        }

        private class ValueCollection : IEnumerable
        {
            private MHashtable _hashtable;

            internal ValueCollection(MHashtable hashtable)
            {
                _hashtable = hashtable;
            }

            public IEnumerator GetEnumerator()
            {
                return new MHashtableEnumerator(_hashtable, 2);
            }
        }

        private class MHashtableEnumerator : IDictionaryEnumerator, IEnumerator
        {
            private MHashtable hashtable;

            private int bucket;//Ͱ����

            private bool current;//��ǰλ���Ƿ�������

            private int getObjectRetType;//���ģʽ

            private object currentKey;

            private object currentValue;

            public object Key
            {
                get
                {
                    if (!current)
                    {
                        throw new Exception();
                    }

                    return currentKey;
                }
            }

            public object Value
            {
                get
                {
                    if (!current)
                    {
                        throw new Exception();
                    }

                    return currentValue;
                }
            }

            public MDictionaryEntry Entry
            {
                get
                {
                    if (!current)
                    {
                        throw new Exception();
                    }

                    return new MDictionaryEntry(currentKey, currentValue);
                }
            }

            public object Current
            {
                get
                {
                    if (!current)
                    {
                        throw new Exception();
                    }

                    //����ѡ���getObjectRetTypeȷ����ȡ�����ݣ�
                    //1---Key
                    //2---Value
                    //3---KeyValuePair
                    if (getObjectRetType == 1)
                    {
                        return currentKey;
                    }
                    if(getObjectRetType == 2) 
                    {
                        return currentValue;
                    }
                    //getObjectRetType == 3
                    return new DictionaryEntry(currentKey, currentValue);
                }
            }

            internal MHashtableEnumerator(MHashtable hashtable, int getObjRetType)
            {
                this.hashtable = hashtable;
                bucket = hashtable.buckets.Length;
                current = false;
                getObjectRetType = getObjRetType;
            }

            public bool MoveNext()
            {
                while (bucket > 0)
                {
                    bucket--;
                    object key = hashtable.buckets[bucket].key;//ȡ��key
                    //�����ֵ����ȡ����������
                    if (key != null && key != hashtable.buckets)
                    {
                        currentKey = key;
                        currentValue = hashtable.buckets[bucket].val;
                        current = true;
                        return true;
                    }
                }

                current = false;
                return false;
            }

            public void Reset()
            {
                current = false;
                bucket = hashtable.buckets.Length;
                currentKey = null;
                currentValue = null;
            }
        }

        private struct bucket
        {
            public object key;

            public object val;

            public int hash_coll;//��ϣ�룬���λ�洢��ϣ��ͻ���
        }

        private bucket[] buckets;

        private int count;

        private int occupancy;//��ϣ��ͻ����

        private int loadsize;//���ؼ���---���������ݵ�

        private float loadFactor;//��������---��������ʱ��


        private IEnumerable keys;

        private IEnumerable values;

        public int Count => count;

        public IEnumerable Keys
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
        public IEnumerable Values
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

        public object this[object key]
        {
            get
            {
                if (key == null)
                {
                    throw new Exception();
                }

                bucket[] array = buckets;
                uint seed;
                uint incr;
                uint num = InitHash(key, array.Length, out seed, out incr);
                int num2 = 0;
                int num3 = (int)(seed % (uint)array.Length);
                bucket bucket;
                do
                {
                    bucket = array[num3];

                    if (bucket.key == null)
                    {
                        return null;
                    }

                    if ((bucket.hash_coll & 0x7FFFFFFF) == num && KeyEquals(bucket.key, key))
                    {
                        return bucket.val;
                    }

                    num3 = (int)((num3 + incr) % (long)(uint)array.Length);
                }
                while (bucket.hash_coll < 0 && ++num2 < array.Length);

                //û�й�ϣ��ͻ���ò���ֵ��˵��û��ֵ������null
                return null;
            }
            set
            {
                Insert(key, value, add: false);
            }
        }

        public MHashtable() : this(0, 1.0f) { }
        public MHashtable(int capacity) : this(capacity, 1.0f) { }
        public MHashtable(int capacity, float loadFactor)
        {
            //capacity---������ָ������Ҫ���ٵ�Ͱ�ռ�
            //loadFactor---�������ӣ�ָ���ǿռ����������
            //num---capacity/loadFactor��ָ���Ǹ��ݸ������ӻ�ȡ��Ͱ�ռ�
            //num2---����num��ȡ��һ��������ָ�����������ٵ�Ͱ�ռ�
            //��ô��˼���ǣ�
            //���ǿ��ܴ���100������ζ��������Ҫ����100��Ԫ�صĿռ䣬������ʵ�ϻῪ�ٸ���Ŀռ䣬����Ҫȡ���ڸ�������

            //����������С��0
            if (capacity < 0)
            {
                throw new Exception();
            }

            //��������ȡֵ��ΧӦ����[0,1]֮�䣬[0,0.1)̫С��Ҳ�ų�
            if (loadFactor < 0.1f || loadFactor > 1.0f)
            {
                throw new Exception();
            }

            this.loadFactor = 0.72f * loadFactor;

            double num = (float)capacity / this.loadFactor;
            //�����и������ӵĴ��ڣ�������һ��������
            if (num > 2147483647.0)
            {
                throw new Exception();
            }

            //����������Ͱ����(����Ϊ3(��С����)������Ϊ��num���һ���涨����)
            int num2 = ((num > 3.0) ? MHashHelpers.GetPrime((int)num) : 3);
            buckets = new bucket[num2];//����Ͱ(ʹ�����������ڼ���index�Ĺ�ϣ��ͻ)
            loadsize = (int)(this.loadFactor * (float)num2);//���㸺�ؼ���---���������ݵ�
        }

        public void Add(object key, object value)
        {
            Insert(key, value, add: true);
        }

        public void Remove(object key)
        {
            //�Ƴ��������Ƴ�null
            if (key == null)
            {
                throw new Exception();
            }

            uint seed;//hashcode1
            uint incr;//indexIncr��û�ҵ������Դ�ֵ���в���
            uint num = InitHash(key, buckets.Length, out seed, out incr);//hashcode1
            int num2 = 0;//��������
            int num3 = (int)(seed % (uint)buckets.Length);//index1
            bucket bucket;

            //ѭ����ֱ������û�й�ϣ��ͻ��bucket(ѭ���л�����Ƴ�����)
            do
            {
                bucket = buckets[num3];//�ҵ������bucket
                //bucket�д洢��hashcode��hashcode1һ����bucket�д洢��key��key��ͬ
                //Ҳ����bucketȷʵ����Ҫ�Ƴ����Ǹ�
                //0x7FFFFFFF---0111...1������&����Ҳ�����ų����λ
                if ((bucket.hash_coll & 0x7FFFFFFF) == num && KeyEquals(bucket.key, key))
                {
                    buckets[num3].hash_coll &= int.MinValue;//ȡ����ϣ��ͻ��ʶ(���λΪ1/0������λ��Ϊ0)
                    //�����ڹ�ϣ��ͻʱ
                    if (buckets[num3].hash_coll != 0)
                    {
                        buckets[num3].key = buckets;//��key��Ϊbuckets��������ʶ
                    }
                    else//������˵���key����
                    {
                        buckets[num3].key = null;
                    }

                    buckets[num3].val = null;
                    count--;
                    break;
                }

                num3 = (int)((num3 + incr) % (long)(uint)buckets.Length);//���������index
            }
            while (bucket.hash_coll < 0 && ++num2 < buckets.Length);//����ҵĴ�������������Ͱ�ĳ��ȣ��Ͳ�����
        }

        public bool ContainsKey(object key)
        {
            //��key��key�ز�����Ϊnull
            if (key == null)
            {
                throw new Exception();
            }

            //����index�㷨
            bucket[] array = buckets;
            uint seed;
            uint incr;
            uint num = InitHash(key, array.Length, out seed, out incr);
            int num2 = 0;
            int num3 = (int)(seed % (uint)array.Length);
            bucket bucket;

            do
            {
                bucket = array[num3];//ȡ��bucket

                //��λ�õ�key�ز�����Ϊnull
                if (bucket.key == null)
                {
                    return false;
                }

                //��λ�þ���key����Ӧ��λ��
                if ((bucket.hash_coll & 0x7FFFFFFF) == num && KeyEquals(bucket.key, key))
                {
                    return true;
                }

                num3 = (int)((num3 + incr) % (long)(uint)array.Length);
            }
            while (bucket.hash_coll < 0 && ++num2 < array.Length);
            //����1˵���ˣ�
            //һ��λ�ô���key(���1������)����key�봫���key�����(���2������)��
            //����ζ�ŷ����˳�ͻ����û�г�ͻ(����1)�����ǲ����ܵģ�����false

            return false;
        }

        public bool ContainsValue(object value)
        {
            if (value == null)//Ѱ��ĳ��valueΪnull��λ��
            {
                int num = buckets.Length;
                while (--num >= 0)
                {
                    //��λ�ñ��������valueΪnull
                    if (buckets[num].key != null && buckets[num].key != buckets && buckets[num].val == null)
                    {
                        return true;
                    }
                }
            }
            else//һ��Ѱ��
            {
                int num = buckets.Length;
                while (--num >= 0)
                {
                    object val = buckets[num].val;
                    //�ж�val�Ƿ����value(����2)�����val�Ѿ���null���Ǳز����ܵ�(����1)
                    if (val != null && val.Equals(value))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void Insert(object key, object nvalue, bool add)
        {
            //��Ҫ��ӵĻ�key������Ϊnull
            if (key == null)
            {
                throw new Exception();
            }

            //�����������������޶�
            if (count >= loadsize)
            {
                expand();
            }
            //����������ͻ����������100(�����Ƿ���������ͻ)
            //ע�⣺occupancyһ��λ��ֻ������һ�Σ�����ζ��ͳ�Ƶ����ж���λ�÷�������ͻ
            else if (occupancy > loadsize && count > 100)
            {
                rehash();
            }

            uint seed;//hashcode1
            uint incr;//hashcode2
            uint num = InitHash(key, buckets.Length, out seed, out incr);//hashcode1
            int num2 = 0;//��������
            int num3 = -1;//emptySlotNumber
            int num4 = (int)(seed % (uint)buckets.Length);//index

            do
            {
                //����ô���ǰ��������ϣ��ײ(����2)����ĿǰҲ���ڹ�ϣ��ײ(����3)
                //��û�б���λ�õ�������ҵ���һ������(��������ϣ��ײ)�Ŀ�λ
                if (num3 == -1 && buckets[num4].key == buckets && buckets[num4].hash_coll < 0)
                {
                    num3 = num4;//���num3��û�����ù�(����1)����ô�ͽ���index�ݴ���num3��
                }

                //�����λ��û��ʹ�ù�(���1)
                if (buckets[num4].key == null || (buckets[num4].key == buckets && (buckets[num4].hash_coll & 0x80000000u) == 0L))
                {
                    //������2---��һ����̫��(��������ϣ��ײ����rehash()������)�Ŀ�λ
                    //����Ѿ�������һ������index
                    if (num3 != -1)
                    {
                        num4 = num3;//ʹ������ݴ�index
                    }

                    buckets[num4].val = nvalue;
                    buckets[num4].key = key;
                    buckets[num4].hash_coll |= (int)num;
                    count++;

                    return;
                }

                //bucket�д洢��hashcode��hashcode1һ����bucket�д洢��key��key��ͬ
                //˵�����λ���Ѿ����ڣ���Ҫ����set����
                if ((buckets[num4].hash_coll & 0x7FFFFFFF) == num && KeyEquals(buckets[num4].key, key))
                {
                    //�˴�Ϊ���Ĳ���������Ӳ�����addΪtrue�Ļ��ǲ���ȷ��
                    if (add)
                    {
                        throw new Exception();
                    }

                    buckets[num4].val = nvalue;

                    return;
                }

                //���ˣ���λ��һ��������ͻ��
                //��Ϊ��Ȼkey����ֵ�������е�����һ���Ѿ������ֵ���ǲ�ƥ��ģ��ⲻ����Ҫ�Ķ��Ƿ�����ͻ(�йظ����)

                //����Ϊ������ײ�����ǲ�û������(hash_coll���λΪ0)
                if (num3 == -1 && buckets[num4].hash_coll >= 0)
                {
                    //���ù�ϣ��ͻ״̬
                    buckets[num4].hash_coll |= int.MinValue;
                    occupancy++;//������һ��λ��ֻ������һ��
                }

                num4 = (int)((num4 + incr) % (long)(uint)buckets.Length);//���������index
            }
            while (++num2 < buckets.Length);//�����������Ͱ�ĳ��ȴλ�û�ҵ����Ͳ�����

            //������ϣ�ֻ�ܴ洢��Ԥ��λ����
            if (num3 != -1)
            {
                buckets[num3].val = nvalue;
                buckets[num3].key = key;
                buckets[num3].hash_coll |= (int)num;
                count++;

                //�Ѿ�ȫ���ҹ�һ���ˣ���û��һ�����ʵ�λ�ã�����ζ��buckets��״̬�ǳ�����
                //�����������Ͱ��ͦ���ģ���100�����ϣ�����ʱ���Ƿ����ˣ�˵������Ĳ���(����˵��3��λ����ô�����ǳ�����)
                if (buckets.Length > 100)
                {
                    rehash(buckets.Length, forceNewHashCode: true);
                }

                return;
            }

            //����ִ�н����˶�û�д洢�ɹ������Ǵ����
            throw new Exception();
        }

        private uint InitHash(object key, int hashsize, out uint seed, out uint incr)
        {
            //result = seed = ��ϣ��ͻ��ʶΪ0��hashcode
            uint result = (seed = (uint)key.GetHashCode() & 0x7FFFFFFFu);
            incr = 1 + seed * 101 % (uint)(hashsize - 1);//indexIncr��Ӧ�ÿ��Գ�Ϊ��Ϲ�ϣ
            return result;
        }

        private void expand()
        {
            int newsize = MHashHelpers.ExpandPrime(buckets.Length);
            rehash(newsize, forceNewHashCode: false);
        }

        private void rehash()
        {
            rehash(buckets.Length, forceNewHashCode: false);
        }
        private void rehash(int newsize, bool forceNewHashCode)
        {
            occupancy = 0;
            bucket[] newBuckets = new bucket[newsize];

            for (int i = 0; i < buckets.Length; i++)
            {
                bucket bucket = buckets[i];
                //����key�Ҹ�λ��û�з������Ƴ�ʱ���й�ϣ��ͻ(����2������˵�����Ѿ��Ƴ����ǿյ���)
                if (bucket.key != null && bucket.key != buckets)
                {
                    //forceNewHashCode=1---���»�ȡ��ϣ��
                    //forceNewHashCode=0---�����ϣ��ͻ���
                    //��Ϊ������ת����Ͱ������ԭ��ͻ����������
                    int hashcode = (forceNewHashCode ? bucket.key.GetHashCode() : bucket.hash_coll) & 0x7FFFFFFF;
                    putEntry(newBuckets, bucket.key, bucket.val, hashcode);
                }
            }

            buckets = newBuckets;
            loadsize = (int)(loadFactor * (float)newsize);
        }

        private void putEntry(bucket[] newBuckets, object key, object nvalue, int hashcode)
        {
            uint num = 1 + (uint)(hashcode * 101) % (uint)(newBuckets.Length - 1);//indexIncr��Ϊindex����
            int num2 = (int)((uint)hashcode % (uint)newBuckets.Length);//index

            //�ж�index���Ƿ���key��ռ�þ�һֱ�ҵ�û��ռ�õ�key
            while (newBuckets[num2].key != null && newBuckets[num2].key != buckets)
            {
                //��ϣ��ͻ���Ϊ0ʱ(û�й�ϣ��ͻ)
                if (newBuckets[num2].hash_coll >= 0)
                {
                    newBuckets[num2].hash_coll |= int.MinValue;//��ϣ��ͻ�����Ϊ1(��ʵ�����˹�ϣ��ͻ)
                    occupancy++;
                }

                //�����ҵ�һ���µ�index
                num2 = (int)((num2 + num) % (long)(uint)newBuckets.Length);
            }

            //��û��ռ�õ�key������
            newBuckets[num2].val = nvalue;
            newBuckets[num2].key = key;
            newBuckets[num2].hash_coll |= hashcode;
        }

        private bool KeyEquals(object item, object key)
        {
            //�ô�Ϊ�ձ�Ȼ��������key���
            if (buckets == item)
            {
                return false;
            }

            //==�Ƚ�
            if (item == key)
            {
                return true;
            }

            //Equals()�Ƚ�
            return item?.Equals(key) ?? false;
        }

        //�������أ�ʹ��foreach���ʱ��Ӱ��
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new MHashtableEnumerator(this, 3);
        }
    }
}