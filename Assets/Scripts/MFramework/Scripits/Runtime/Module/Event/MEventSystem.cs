using System;
using System.Collections.Generic;

namespace MFramework
{
    public static class MEventSystem
    {
        private static Dictionary<int, Action> eventDict = new Dictionary<int, Action>();
        //builtInEventDict---һ���¼���
        //builtInEventDict2---OnApplicationFocus��OnApplicationPause�ã�����bool�β�
        private static Dictionary<int, Action> builtInEventDict = new Dictionary<int, Action>();
        private static Dictionary<int, Action<bool>> builtInEventDict2 = new Dictionary<int, Action<bool>>();

        public static void Dispatch(int id)
        {
            if (!eventDict.ContainsKey(id))//û�е��ù�AddListener()���
            {
                MLog.Print($"{typeof(MEventSystem)}��id-<{id}>��û��Action���ַܷ������ȵ���AddListener()��ʹ��", MLogType.Warning);
                return;
            }
            if (eventDict[id] == null)//���ù�AddListener()����Action����ȫ�Ƴ�
            {
                MLog.Print($"{typeof(MEventSystem)}��id-<{id}>��û��Action���ַܷ������ȵ���AddListener()��ʹ��", MLogType.Warning);
                return;
            }

            eventDict[id].Invoke();
        }
        internal static void DispatchBuiltIn(BuiltInEvent e)
        {
            int id = (int)e;

            if (!builtInEventDict.ContainsKey(id))//û�е��ù�AddListener()���
            {
                return;
            }
            if (builtInEventDict[id] == null)//û��Action��ִ��
            {
                return;
            }

            builtInEventDict[id].Invoke();
        }
        internal static void DispatchBuiltIn(BuiltInEvent e, bool b)
        {
            int id = (int)e;

            if (!builtInEventDict2.ContainsKey(id))//û�е��ù�AddListener()���
            {
                return;
            }
            if (builtInEventDict2[id] == null)//û��Action��ִ��
            {
                return;
            }

            builtInEventDict2[id].Invoke(b);
        }

        public static void AddListener(int id, Action action)
        {
            if (!eventDict.ContainsKey(id))
            {
                eventDict.Add(id, action);
                return;
            }

            eventDict[id] += action;
        }
        public static void AddListener(BuiltInEvent e, Action action)
        {
            if (e == BuiltInEvent.ONAPPLICATIONFOCUS || e == BuiltInEvent.ONAPPLICATIONPAUSE)
            {
                MLog.Print($"{typeof(MEventSystem)}��{typeof(BuiltInEvent).Name}.{e}��ʹ��Action<bool>����Action", MLogType.Warning);
                return;
            }

            int id = (int)e;

            if (!builtInEventDict.ContainsKey(id))
            {
                builtInEventDict.Add(id, action);
                return;
            }

            builtInEventDict[id] += action;
        }
        public static void AddListener(BuiltInEvent e, Action<bool> action)
        {
            if (e != BuiltInEvent.ONAPPLICATIONFOCUS && e != BuiltInEvent.ONAPPLICATIONPAUSE)
            {
                MLog.Print($"{typeof(MEventSystem)}��{typeof(BuiltInEvent).Name}.{e}��ʹ��Action����Action<bool>", MLogType.Warning);
                return;
            }

            int id = (int)e;

            if (!builtInEventDict.ContainsKey(id))
            {
                builtInEventDict2.Add(id, action);
                return;
            }

            builtInEventDict2[id] += action;
        }

        public static void RemoveListener(int id, Action action)
        {
            if (!eventDict.ContainsKey(id))//û�е��ù�AddListener()���
            {
                MLog.Print($"{typeof(MEventSystem)}��id-<{id}>��û��Action�����Ƴ������ȵ���AddListener()��ʹ��", MLogType.Warning);
                return;
            }
            if(eventDict[id] == null)//���ù�AddListener()����Action����ȫ�Ƴ�
            {
                MLog.Print($"{typeof(MEventSystem)}��id-<{id}>��û��Action�����Ƴ������ȵ���AddListener()��ʹ��", MLogType.Warning);
                return;
            }

            eventDict[id] -= action;
        }
        public static void RemoveListener(BuiltInEvent e, Action action)
        {
            if (e == BuiltInEvent.ONAPPLICATIONFOCUS || e == BuiltInEvent.ONAPPLICATIONPAUSE)
            {
                MLog.Print($"{typeof(MEventSystem)}��{typeof(BuiltInEvent).Name}.{e}��ʹ��Action<bool>����Action", MLogType.Warning);
                return;
            }

            int id = (int)e;

            if (!builtInEventDict.ContainsKey(id))//û�е��ù�AddListener()���
            {
                MLog.Print($"{typeof(MEventSystem)}��id-<{id}>��û��Action�����Ƴ������ȵ���AddListener()��ʹ��", MLogType.Warning);
                return;
            }
            if (builtInEventDict[id] == null)//���ù�AddListener()����Action����ȫ�Ƴ�
            {
                MLog.Print($"{typeof(MEventSystem)}��id-<{id}>��û��Action�����Ƴ������ȵ���AddListener()��ʹ��", MLogType.Warning);
                return;
            }

            builtInEventDict[id] -= action;
        }
        public static void RemoveListener(BuiltInEvent e, Action<bool> action)
        {
            if (e != BuiltInEvent.ONAPPLICATIONFOCUS && e != BuiltInEvent.ONAPPLICATIONPAUSE)
            {
                MLog.Print($"{typeof(MEventSystem)}��{typeof(BuiltInEvent).Name}.{e}��ʹ��Action����Action<bool>", MLogType.Warning);
                return;
            }

            int id = (int)e;

            if (!builtInEventDict2.ContainsKey(id))//û�е��ù�AddListener()���
            {
                MLog.Print($"{typeof(MEventSystem)}��id-<{id}>��û��Action�����Ƴ������ȵ���AddListener()��ʹ��", MLogType.Warning);
                return;
            }
            if (builtInEventDict2[id] == null)//���ù�AddListener()����Action����ȫ�Ƴ�
            {
                MLog.Print($"{typeof(MEventSystem)}��id-<{id}>��û��Action�����Ƴ������ȵ���AddListener()��ʹ��", MLogType.Warning);
                return;
            }

            builtInEventDict2[id] -= action;
        }

        public static void RemoveAllListener(int id)
        {
            if (!eventDict.ContainsKey(id))//û�е��ù�AddListener()��Ҳ��û��Action
            {
                return;
            }

            eventDict[id] = null;
        }
        public static void RemoveAllListener(BuiltInEvent e)
        {
            int id = (int)e;
            if (e != BuiltInEvent.ONAPPLICATIONFOCUS && e != BuiltInEvent.ONAPPLICATIONPAUSE)
            {
                if (!builtInEventDict.ContainsKey(id))//û�е��ù�AddListener()��Ҳ��û��Action
                {
                    return;
                }

                builtInEventDict[id] = null;
            }
            else
            {
                if (!builtInEventDict2.ContainsKey(id))//û�е��ù�AddListener()��Ҳ��û��Action
                {
                    return;
                }

                builtInEventDict2[id] = null;
            }
        }
    }
}