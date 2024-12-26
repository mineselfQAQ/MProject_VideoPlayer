namespace MFramework
{
    public class ObjectPoolContainer<T>
    {
        private T item;//ʵ�ʴ������
        public T Item
        {
            get
            {
                return item;
            }
            set
            {
                item = value;
            }
        }

        public bool Used { get; private set; }

        /// <summary>
        /// ��������Ϊ"����ʹ��״̬(Ҳ����Used)"
        /// </summary>
        public void Consume()
        {
            Used = true;
        }

        /// <summary>
        /// ��������Ϊ"��ʹ��״̬(Ҳ����Not Used)"
        /// </summary>
        public void Release()
        {
            Used = false;
        }
    }
}