using System;
using System.Collections;
using UnityEngine;

namespace MFramework
{
    public static class MTimeUtility
    {
        /// <summary>
        /// ��ȡ��ǰʱ���(1970.1.1---��)
        /// </summary>
        public static long GetNowTime()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds);
        }
    }
}