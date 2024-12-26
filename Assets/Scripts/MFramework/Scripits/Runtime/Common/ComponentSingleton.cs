using UnityEngine;

namespace MFramework
{
    /// <summary>
    /// ��������GameObject�ϵ������MonoBehaviour�ű���
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class ComponentSingleton<T> : MonoBehaviour where T : ComponentSingleton<T>
    {
        protected static T m_instance;

        public static T Instance
        {
            get
            {
                if (m_instance == null)
                {
                    T[] objects = FindObjectsOfType<T>();
                    if (objects.Length > 1)
                    {
                        MLog.Print($"{typeof(ComponentSingleton<T>)}����ǰ����{objects.Length}��{typeof(T)}�ű�������", MLogType.Error);
                    }
                    else if (objects.Length == 0)
                    {
                        MLog.Print($"{typeof(ComponentSingleton<T>)}��δ����ComponentSingleton<{typeof(T)}>�ű�������", MLogType.Error);
                    }

                    m_instance = objects[0];
                }

                return m_instance;
            }
        }

        protected virtual void Awake()
        {
            if (Instance != null && Instance != this)//�����³��������
            {
                DestroyImmediate(gameObject);
                if (typeof(T) == typeof(MCore)) return;//MCore����Ҫ��ʾ

                MLog.Print($"{typeof(ComponentSingleton<T>)}��{typeof(T)}��ΪComponentSingleton�ű��ٴι��أ������Ƿ���ȷ", MLogType.Warning);
            }
        }
    }
}