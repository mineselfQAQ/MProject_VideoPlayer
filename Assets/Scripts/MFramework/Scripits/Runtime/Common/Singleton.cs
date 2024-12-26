using System.Reflection;
using System;
using UnityEngine;

namespace MFramework
{
    /// <summary>
    /// ��׼Singleton
    /// </summary>
    public abstract class Singleton<T> where T : Singleton<T>
    {
        private static T sm_Instance;

        static Singleton()
        {
            //��ȡT�����й��캯��
            var ctors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
            //�ҵ������б�Ϊ�յ��Ǹ�˽�й��캯��
            var ctor = Array.Find(ctors, c => c.GetParameters().Length == 0);
            //û�ҵ�����
            if (ctor == null) { throw new Exception($"|{typeof(T).ToString()}|���в�����˽���޲ι��캯��"); }
            //����ʵ��
            sm_Instance = ctor.Invoke(null) as T;
        }

        static public T Instance { get { return sm_Instance; } }

        protected Singleton() { }
    }
}
