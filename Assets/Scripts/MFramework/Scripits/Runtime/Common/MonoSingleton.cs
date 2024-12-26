using System;
using UnityEngine;

namespace MFramework
{
    /// <summary>
    /// Manager�ű���
    /// </summary>
    [NoComponent]
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private const string k_RootName = "#MonoSingletons#";//���������е�MonoSingleton�ű������ڸ�GameObject����
        private static T sm_Instance;

        //ע��:��̬���캯���ĵ���ʱ��Ϊ:
        //�ڵ��þ�̬��Ա֮ǰ�����ڴ���ʵ��֮ǰ
        //����ζ�Ŷ��ڼ̳�MonoSingleton�������ͨ������Instance����������
        static MonoSingleton()
        {
            //���Ի�ȡ������#MonoSingleton#
            GameObject rootObj = GameObject.Find(k_RootName);
            //���ڵ�һ��MonoSingleton�ű�����ʱ��û�д�������Ҫ����
            if (rootObj == null)
            {
                rootObj = new GameObject(k_RootName);
                DontDestroyOnLoad(rootObj);
            }

            Transform rootTransform = rootObj.transform;
            Type type = typeof(T);
            string name = type.Name;//Ĭ������
            HideFlags hideFlags = HideFlags.None;//Ĭ��HideFlags
#if UNITY_EDITOR
            //��Editor�У�����ͨ��[MonoSingletonSetting]���Ի�ȡ���ĵĲ���(HideFlags/Name)
            var attributes = type.GetCustomAttributes(true);
            for (int i = 0; i < attributes.Length; i++)
            {
                if (attributes[i] is MonoSingletonSetting mss)
                {
                    hideFlags = mss.hideFlags;
                    name = !string.IsNullOrEmpty(mss.nameInHierarchy) ? mss.nameInHierarchy : name;
                    break;
                }
            }
#endif
            //��������
            sm_Instance = new GameObject(name).AddComponent<T>();//����Ψһʵ������Ϊname��GameObject��
            sm_Instance.transform.SetParent(rootTransform, false);
            sm_Instance.hideFlags = hideFlags;
        }

        public static T Instance
        {
            get
            {
                return sm_Instance;
            }
        }

        protected virtual void OnDestroy()
        {
            sm_Instance = null;
        }
    }

    /// <summary>
    /// �������ڸ����������ã�HideFlags������
    /// ���Ի�ͨ�������ȡ����������
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class MonoSingletonSetting : Attribute
    {
        private HideFlags m_HideFlags;
        private string m_NameInHierarchy;//null��""ʱĬ��ʹ������

        public MonoSingletonSetting(HideFlags hideFlags, string nameInHierarchy = null)
        {
            m_HideFlags = hideFlags;
            m_NameInHierarchy = nameInHierarchy;
        }

        public string nameInHierarchy
        {
            get { return m_NameInHierarchy; }
        }

        public HideFlags hideFlags
        {
            get { return m_HideFlags; }
        }
    }
}