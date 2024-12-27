using System;
using System.Threading;

namespace MFramework
{
    public static class MainThreadUtility
    {
        private static SynchronizationContext _mainThread;

        //ע�⣺���������߳�����һ��(�����Monobehaviour)
        internal static void SetMainThread()
        {
            _mainThread = SynchronizationContext.Current;
        }

        /// <summary>
        /// ֪ͨ���̻߳ص�
        /// </summary>
        public static void Post(Action action)
        {
            _mainThread.Post(new SendOrPostCallback((o) =>
            {
                Action e = (Action)o.GetType().GetProperty("action").GetValue(o);
                if (e != null) e();
            }), new { action = action });
        }
        public static void Post<T>(Action<T> action, T arg1)
        {
            _mainThread.Post(new SendOrPostCallback((o) =>
            {
                Action<T> e = (Action<T>)o.GetType().GetProperty("action").GetValue(o);
                T t1 = (T)o.GetType().GetProperty("arg1").GetValue(o);
                if (e != null) e(t1);
            }), new { action = action, arg1 = arg1 });
        }
        public static void Post<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2)
        {
            _mainThread.Post(new SendOrPostCallback((o) =>
            {
                Action<T1, T2> e = (Action<T1, T2>)o.GetType().GetProperty("action").GetValue(o);
                T1 t1 = (T1)o.GetType().GetProperty("arg1").GetValue(o);
                T2 t2 = (T2)o.GetType().GetProperty("arg2").GetValue(o);
                if (e != null) e(t1, t2);
            }), new { action = action, arg1 = arg1, arg2 = arg2 });
        }
    }
}
