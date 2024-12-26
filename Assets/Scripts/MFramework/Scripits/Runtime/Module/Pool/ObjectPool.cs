using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MFramework
{
    /// <summary>
    /// ����أ����д����һ��ObjectPoolContainer<T>
    /// </summary>
    public class ObjectPool<T>
    {
        private Queue<ObjectPoolContainer<T>> unusedQueue;
        private Dictionary<T, ObjectPoolContainer<T>> usedLookup;

        private Func<T> initFunc;

        public int Count
        {
            get { return unusedQueue.Count + usedLookup.Count; }
        }
        public int UsedCount
        {
            get { return usedLookup.Count; }
        }

        public ObjectPool(Func<T> initFunc, int initSize, bool warmObject)
        {
            this.initFunc = initFunc;//ͨ�����캯����ó�ʼ��

            //������ʼlist/lookup
            unusedQueue = new Queue<ObjectPoolContainer<T>>(initSize);
            usedLookup = new Dictionary<T, ObjectPoolContainer<T>>(initSize);
            //������ʼContainer
            if (warmObject) Warm(initSize);
        }

        /// <summary>
        /// ��ȡItem(��ȡNot Used����򴴽�Container)
        /// </summary>
        public T GetItem()
        {
            ObjectPoolContainer<T> container = null;

            if (unusedQueue.Count == 0)//û��Not Used����
            {
                container = CreateContainer();
            }

            container = unusedQueue.Dequeue();//����
            container.Consume();
            usedLookup.Add(container.Item, container);//���

            return container.Item;
        }

        /// <summary>
        /// �ͷ�Item(��������)
        /// </summary>
        public void ReleaseItem()
        {
            ObjectPoolContainer<T> container = null;

            if (usedLookup.Count > 0)
            {
                var k = usedLookup.Keys.First();
                container = usedLookup[k];
                container.Release();
                usedLookup.Remove(k);
                unusedQueue.Enqueue(container);
            }
            else//������ʹ������
            {
                MLog.Print($"{typeof(ObjectPool<T>)}����û�п��ͷ�{container.Item}������", MLogType.Warning);
            }
        }
        /// <summary>
        /// �ͷ�Item(��������)
        /// </summary>
        public void ReleaseItem(T item)
        {
            ObjectPoolContainer<T> container = null;

            if (usedLookup.ContainsKey(item))
            {
                container = usedLookup[item];
                container.Release();
                usedLookup.Remove(item);
                unusedQueue.Enqueue(container);
            }
            else//������ʹ������
            {
                MLog.Print($"{typeof(ObjectPool<T>)}����û�п��ͷ�{container.Item}������", MLogType.Warning);
            }
        }

        /// <summary>
        /// ����Container
        /// </summary>
        private void Warm(int capacity)
        {
            for (int i = 0; i < capacity; i++)
            {
                var container = CreateContainer();
                (container.Item as GameObject).SetActive(false);
            }
        }

        /// <summary>
        /// ��Container�������
        /// </summary>
        private ObjectPoolContainer<T> CreateContainer()
        {
            //Container�Ĵ�������ʵ�������岢������ӽ�Queue
            var container = new ObjectPoolContainer<T>();
            container.Item = initFunc();//��ʵ����ִ��InstantiatePrefab()

            unusedQueue.Enqueue(container);

            return container;
        }
    }
}