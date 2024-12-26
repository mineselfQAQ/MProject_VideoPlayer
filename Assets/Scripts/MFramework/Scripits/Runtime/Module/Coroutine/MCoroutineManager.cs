using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFramework
{
    //TODO:name��ʽӦ�÷��飬�����޷��򵥵�����Stop����
    [MonoSingletonSetting(HideFlags.NotEditable, "#MCoroutineManager#")]
    public class MCoroutineManager : MonoSingleton<MCoroutineManager>
    {
        private Dictionary<string, Coroutine> dic;//Key---���� Value---Coroutineʵ��

        private int count;
        public int Count
        {
            get
            {
                return count;
            }
        }

        private MCoroutineManager()
        {
            dic = new Dictionary<string, Coroutine>();
        }

        private void OnApplicationQuit()
        {
            foreach (var value in dic.Values)
            {
                StopCoroutine(value);
            }
        }

        #region �޼�¼Я��(������MonoBehaviour�ű�)
        public Coroutine BeginCoroutineNoRecord(IEnumerator enumerator)
        {
            return StartCoroutine(enumerator);
        }
        public void EndCoroutine(Coroutine coroutine)
        {
            StopCoroutine(coroutine);
        }
        #endregion

        #region ���ҹ���Я��
        public Coroutine StartCoroutine(IEnumerator fun, string name, Action onFinish = null)
        {
            return StartCoroutine(StartCoroutinueRoutine(fun, name, onFinish));
        }

        public bool EndCoroutine(string name)
        {
            if (!dic.ContainsKey(name))
            {
                //MLog.Print($"{typeof(MCoroutineManager)}:�ֵ���û����Ϊ{name}��Coroutine������.", MLogType.Warning);
                return false;
            }

            StopCoroutine(dic[name]);
            dic.Remove(name);
            count--;

            return true;
        }

        public void EndAllCoroutines()
        {
            foreach (var value in dic.Values)
            {
                StopCoroutine(value);
            }
            dic.Clear();
            count = 0;
        }

        private IEnumerator StartCoroutinueRoutine(IEnumerator enumerator, string name, Action onFinish)
        {
            if (dic.ContainsKey(name))
            {
                MLog.Print($"{typeof(MCoroutineManager)}��{name}�Ѵ��ڣ�����", MLogType.Warning);
                yield break;
            }

            Coroutine coroutine = StartCoroutine(enumerator);
            dic.Add(name, coroutine);
            count++;

            yield return coroutine;

            OnCoroutineFinishedInternal(name);
            onFinish?.Invoke();

            yield break;
        }

        private void OnCoroutineFinishedInternal(string name)
        {
            if (dic.ContainsKey(name))
            {
                dic.Remove(name);
                count--;
            }
        }
        #endregion

        #region ����Я��
        public Coroutine DelayFrame(Action action, int frame)
        {
            return StartCoroutine(MCoroutineUtility.DelayFrame(action, frame));
        }
        public Coroutine DelayOneFrame(Action action)
        {
            return StartCoroutine(MCoroutineUtility.DelayFrame(action, 1));
        }

        public Coroutine DelayWithTimeScaleNoRecord(Action action, float interval)
        {
            return StartCoroutine(MCoroutineUtility.DelayWithTimeScale(action, interval));
        }

        /// <summary>
        /// �ȴ���ִ��(����¼)
        /// </summary>
        public Coroutine DelayNoRecord(Action action, float interval)
        {
            return StartCoroutine(MCoroutineUtility.Delay(action, interval));
        }
        /// <summary>
        /// �ظ�ִ��(����¼)
        /// </summary>
        public Coroutine RepeatNoRecord(Action action, bool startDo, int count, float interval, Action onFinish = null)
        {
            return StartCoroutine(MCoroutineUtility.Repeat(action, startDo, count, interval, onFinish));
        }
        /// <summary>
        /// �ȴ����ظ�ִ��(����¼)
        /// </summary>
        public Coroutine DelayRepeatNoRecord(Action action, float startInterval, int repeatCount, float repeatInterval, Action onFinish = null)
        {
            return StartCoroutine(MCoroutineUtility.DelayRepeat(action, startInterval, repeatCount, repeatInterval, onFinish));
        }
        /// <summary>
        /// ����ִ�в���(����¼)
        /// </summary>
        public Coroutine LoopNoRecord(Action action, float startInterval, float repeatInterval)
        {
            return StartCoroutine(MCoroutineUtility.Loop(action, startInterval, repeatInterval));
        }

        /// <summary>
        /// �ȴ���ִ��
        /// </summary>
        public void Delay(string name, Action action, float interval)
        {
            StartCoroutine(MCoroutineUtility.Delay(action, interval), name);
        }
        /// <summary>
        /// �ظ�ִ��
        /// </summary>
        public void Repeat(string name, Action action, bool startDo, int count, float interval, Action onFinish = null)
        {
            StartCoroutine(MCoroutineUtility.Repeat(action, startDo, count, interval, onFinish), name);
        }
        /// <summary>
        /// �ȴ����ظ�ִ��
        /// </summary>
        public void DelayRepeat(string name, Action action, float startInterval, int repeatCount, float repeatInterval, Action onFinish = null)
        {
            StartCoroutine(MCoroutineUtility.DelayRepeat(action, startInterval, repeatCount, repeatInterval, onFinish), name);
        }
        /// <summary>
        /// ����ִ�в���
        /// </summary>
        public void Loop(string name, Action action, float startInterval, float repeatInterval)
        {
            StartCoroutine(MCoroutineUtility.Loop(action, startInterval, repeatInterval), name);
        }

        /// <summary>
        /// ���䶯������(����¼)
        /// </summary>
        internal Coroutine UnscaledFixedTweenNoRecord(Action<float> action, MCurve curve, float duration, float startValue, float endValue, Action onFinish)
        {
            return StartCoroutine(UnsacledFixedTweenRoutine(action, curve, duration, startValue, endValue, onFinish));
        }
        /// <summary>
        /// ���䶯������
        /// </summary>
        internal void UnscaledFixedTween(string name, Action<float> action, MCurve curve, float duration, float startValue, float endValue, Action onFinish)
        {
            StartCoroutine(UnsacledFixedTweenRoutine(action, curve, duration, startValue, endValue, onFinish), name);
        }

        /// <summary>
        /// ���䶯������(����¼)
        /// </summary>
        internal Coroutine FixedTweenNoRecord(Action<float> action, MCurve curve, float duration, float startValue, float endValue, Action onFinish)
        {
            return StartCoroutine(FixedTweenRoutine(action, curve, duration, startValue, endValue, onFinish));
        }
        /// <summary>
        /// ���䶯������
        /// </summary>
        internal void FixedTween(string name, Action<float> action, MCurve curve, float duration, float startValue, float endValue, Action onFinish)
        {
            StartCoroutine(FixedTweenRoutine(action, curve, duration, startValue, endValue, onFinish), name);
        }

        /// <summary>
        /// ���䶯������(����¼)
        /// </summary>
        internal Coroutine UnscaledTweenNoRecord(Action<float> action, MCurve curve, float duration, float startValue, float endValue, Action onFinish)
        {
            return StartCoroutine(UnscaledTweenRoutine(action, curve, duration, startValue, endValue, onFinish));
        }
        /// <summary>
        /// ���䶯������
        /// </summary>
        internal void UnsacledTween(string name, Action<float> action, MCurve curve, float duration, float startValue, float endValue, Action onFinish)
        {
            StartCoroutine(UnscaledTweenRoutine(action, curve, duration, startValue, endValue, onFinish), name);
        }


        /// <summary>
        /// ���䶯������(����¼)
        /// </summary>
        internal Coroutine TweenNoRecord(Action<float> action, MCurve curve, float duration, float startValue, float endValue, Action onFinish)
        {
            return StartCoroutine(TweenRoutine(action, curve, duration, startValue, endValue, onFinish));
        }
        /// <summary>
        /// ���䶯������
        /// </summary>
        internal void Tween(string name, Action<float> action, MCurve curve, float duration, float startValue, float endValue, Action onFinish)
        {
            StartCoroutine(TweenRoutine(action, curve, duration, startValue, endValue, onFinish), name);
        }

        //internal static WaitForFixedUpdate waitFixedUpdate = new WaitForFixedUpdate();//��TimeScaleӰ��
        //internal static WaitForSecondsRealtime waitFixedUpdate = new WaitForSecondsRealtime(Time.fixedDeltaTime);

        /// <summary>
        /// ����FixedDeltaTime��Tween����(����timeScaleӰ��)
        /// </summary>
        internal IEnumerator UnsacledFixedTweenRoutine(Action<float> action, MCurve curve, float duration, float startValue, float endValue, Action onFinish)
        {
            float step = duration / Time.fixedUnscaledDeltaTime;//ִ�д���  ����timeScaleӰ��
            float length = endValue - startValue;//���䳤��

            float curValue;
            for (int i = 0; i < step; i++)
            {
                curValue = startValue + MCurveSampler.Sample(curve, i / step) * length;
                action.Invoke(curValue);

                yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
            }
            curValue = curve.curveDir == CurveDir.Increment ? endValue : startValue;
            action.Invoke(curValue);

            onFinish?.Invoke();
        }
        /// <summary>
        /// ����FixedDeltaTime��Tween����(��timeScaleӰ��)
        /// </summary>
        internal IEnumerator FixedTweenRoutine(Action<float> action, MCurve curve, float duration, float startValue, float endValue, Action onFinish)
        {
            float step = duration / Time.fixedUnscaledDeltaTime;//ִ�д���  ����timeScaleӰ��
            float length = endValue - startValue;//���䳤��

            float curValue;
            for (int i = 0; i < step; i++)
            {
                curValue = startValue + MCurveSampler.Sample(curve, i / step) * length;
                action.Invoke(curValue);

                yield return new WaitForSecondsRealtime(Time.deltaTime);
            }
            curValue = curve.curveDir == CurveDir.Increment ? endValue : startValue;
            action.Invoke(curValue);

            onFinish?.Invoke();
        }
        /// <summary>
        /// ����DeltaTime��Tween����(����timeScaleӰ��)
        /// </summary>
        internal IEnumerator UnscaledTweenRoutine(Action<float> action, MCurve curve, float duration, float startValue, float endValue, Action onFinish)
        {
            float elapsed = 0.0f;//����ʱ��
            float length = endValue - startValue;//���䳤��

            float curValue;
            while (elapsed < duration)
            {
                float progress = Mathf.Clamp01(elapsed / duration);//����[0,1]
                curValue = startValue + MCurveSampler.Sample(curve, progress) * length;
                action.Invoke(curValue);

                elapsed += Time.unscaledDeltaTime;//����timeScaleӰ��
                yield return null;
            }
            //���һ֡
            curValue = curve.curveDir == CurveDir.Increment ? endValue : startValue;
            action.Invoke(curValue);

            onFinish?.Invoke();
        }
        /// <summary>
        /// ����DeltaTime��Tween����(��timeScaleӰ��)
        /// </summary>
        internal IEnumerator TweenRoutine(Action<float> action, MCurve curve, float duration, float startValue, float endValue, Action onFinish)
        {
            float elapsed = 0.0f;//����ʱ��
            float length = endValue - startValue;//���䳤��

            float curValue;
            while (elapsed < duration)
            {
                float progress = Mathf.Clamp01(elapsed / duration);//����[0,1]
                curValue = startValue + MCurveSampler.Sample(curve, progress) * length;
                action.Invoke(curValue);

                elapsed += Time.deltaTime;//��timeScaleӰ��
                yield return null;
            }
            //���һ֡
            curValue = curve.curveDir == CurveDir.Increment ? endValue : startValue;
            action.Invoke(curValue);

            onFinish?.Invoke();
        }
        #endregion
    }
}