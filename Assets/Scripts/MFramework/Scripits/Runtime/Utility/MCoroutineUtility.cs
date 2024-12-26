using System;
using System.Collections;
using UnityEngine;

namespace MFramework
{
    internal class MCoroutineUtility
    {
        internal static IEnumerator DelayFrame(Action action, int frame)
        {
            for(int i = 0; i < frame; i++) yield return null;

            action();
        }

        internal static IEnumerator DelayWithTimeScale(Action action, float interval)
        {
            yield return new WaitForSeconds(interval);
            action();
        }

        /// <summary>     
        /// �ӳ�'interval'���ִ�в���
        /// </summary>
        internal static IEnumerator Delay(Action action, float interval)
        {
            yield return new WaitForSecondsRealtime(interval);
            action();
        }

        /// <summary>
        /// ÿ��'interval'���ִ�в�������ִ��count��(startDo����ú�ֱ��ִ��һ��)
        /// </summary>
        internal static IEnumerator Repeat(Action action, bool startDo, int repeatCount, float repeatInterval, Action onFinish)
        {
            //��Ȼ��ΪRepeat()������repeatCount=0Ҳ�ǿ��Խ��ܵ�(��һ��ѭ������ʱ�ظ�10�Σ���ʱִֻ��1��)

            if (startDo)
            {
                action();
                repeatCount--;
            }

            for (int i = 0; i < repeatCount; i++)
            {
                yield return new WaitForSecondsRealtime(repeatInterval);
                action();
            }

            onFinish?.Invoke();
        }

        /// <summary>        
        /// ����'startInterval'���ִ�е�һ�β�����
        /// Ȼ��ÿ��'interval'���ִ�в�������ִ��count-1��
        /// </summary>
        internal static IEnumerator DelayRepeat(Action action, float startInterval, int repeatCount, float repeatInterval, Action onFinish)
        {
            //��Ȼ��ΪDelayRepeat()������repeatCount=0Ҳ�ǿ��Խ��ܵ�(��һ��ѭ������ʱ�ظ�10�Σ���ʱִֻ��1��)

            yield return new WaitForSecondsRealtime(startInterval);
            action();

            for (int i = 1; i < repeatCount; i++)
            {
                yield return new WaitForSecondsRealtime(repeatInterval);
                action();
            }

            onFinish?.Invoke();
        }

        /// <summary>
        /// ����'startInterval'���ִ�е�һ�β�����
        /// Ȼ��ÿ��'interval'���ִ�в���������ִ��
        /// </summary>
        internal static IEnumerator Loop(Action action, float startInterval, float repeatInterval)
        {
            yield return new WaitForSecondsRealtime(startInterval);

            while (true)
            {
                action();

                yield return new WaitForSecondsRealtime(repeatInterval);
            }
        }
    }
}

