using MFramework.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace MFramework
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MText))]
    public class MTextAnimator : MonoBehaviour
    {
        [Header("Typewriter")]
        public TypeWriterSwitch typeWriterSwitch = TypeWriterSwitch.Off;
        public MTextTypewriterType typewriterType;
        public bool typewriterStartDo = false;
        public float typeSpeed = 1.0f;
        public UnityEvent onTypeWriterFinished;

        [Header("Inline Effects")]
        public InlineEffectSwitch inlineEffectsSwitch = InlineEffectSwitch.Off;
        public bool inlineEffectsAutoDo = false;

        public bool typewriterPlaying { get; internal set; } = false;

        internal MTextAnimation mAnim;
        private TMP_Text text;

        internal void Init()
        {
            TMP_Text t = GetComponent<TMP_Text>();

            mAnim = new MTextAnimation(t, this);
            text = t;
        }

        internal void PlayInit(ParsedText pText = null)
        {
            //ע�⣺�����ӳٽ��У�����textData�޷������ռ�
            MCoroutineManager.Instance.DelayNoRecord((() =>
            {
                mAnim.UpdateTextInfo();//mAnim�ռ���Ϣ(textData)

                if (typewriterStartDo)
                {
                    PlayTypeWriterInternal();
                }

                //���������ֻ�Ч���Ż�ֱ�ӽ�������Ч��
                if (inlineEffectsAutoDo && typeWriterSwitch == TypeWriterSwitch.Off && pText != null && pText.haveAnimatorInline)
                {
                    pText.ApplyEffects(mAnim);
                }
            }), 0.1f);
        }

        //Tip��Play()����ֱ�����ڿ�ʼ����Ҫ�ӳٵȴ����ݼ�����ϲ���ִ��
        //�ṩ��StartDo���ܣ��������StartDo����ô��Ȼ������һ��ʱ��
        //����˵�����ǣ���Ҫ��Awake()/Start()��ʹ�ü���
        public void PlayText()
        {
            if (typeWriterSwitch == TypeWriterSwitch.Off) return;

            MTextAnimatorCoroutine.StopAllCoroutines(text);
            MCoroutineManager.Instance.DelayNoRecord((() =>
            {
                //��������
                mAnim.UpdateInfo();
                mAnim.UpdateTextInfo();

                PlayTypeWriterInternal();//Tip:ִ����Ϻ�������������������Զ�ִ��
            }), 0.05f);
        }
        public void PlayNewText()
        {
            if (typeWriterSwitch == TypeWriterSwitch.Off) return;

            MTextAnimatorCoroutine.StopAllCoroutines(text);
            mAnim = new MTextAnimation(text, this);
            MCoroutineManager.Instance.DelayNoRecord((() =>
            {
                text.alpha = 1;

                mAnim.UpdateTextInfo();//mAnim�ռ���Ϣ(textData)

                PlayTypeWriterInternal();
            }), 0.06f);

        }

        public void PlayTextFastly()
        {
            if (typeWriterSwitch == TypeWriterSwitch.Off) return;

            MTextAnimatorCoroutine.StopAllCoroutines(text);
            MCoroutineManager.Instance.DelayNoRecord((() =>
            {
                //��������
                mAnim.UpdateInfo();
                mAnim.UpdateTextInfo();

                PlayTypeWriterInternalFastly();//Tip:ִ����Ϻ�������������������Զ�ִ��
            }), 0.05f);
        }

        public void FinishTextImmediately()
        {
            if (typeWriterSwitch == TypeWriterSwitch.Off) return;

            //if (mAnim.curTypeWriterFinishIndex == -1) return;
            if (mAnim.curTypeWriterStartIndex == -1) return;//��ǰ��û�н��д��ֻ�Ч������Ӧ�ý���
            if (mAnim.fastFinish) return;//���ڼ��٣������ٴμ���

            //TODO:��ʱֻ��Show��Fade��������
            if (typewriterType != MTextTypewriterType.Show && typewriterType != MTextTypewriterType.Fade) return;

            mAnim.fastFinish = true;
            MCoroutineManager.Instance.DelayNoRecord((() =>
            {
                FinishTypeWriterInternalFastly();
            }), 0.1f / typeSpeed);//TODO:0.1f��һ������ͳ��ʱ�䣬��׼ȷ��ʱ���ǣ��������һ��TweenĿǰִ�е��Ĳ�ֵ�������ֵ�ʱ��
        }

        private void PlayTypeWriterInternal()
        {
            if (typeWriterSwitch == TypeWriterSwitch.Off) return;

            typewriterPlaying = true;

            //����Inspectorѡȡ��ģʽ���ж���
            switch (typewriterType)
            {
                case MTextTypewriterType.Show:
                    mAnim.TypeWriterShow(typeSpeed);
                    break;
                case MTextTypewriterType.Fade:
                    mAnim.TypeWriterFade(typeSpeed);
                    break;
                case MTextTypewriterType.Scale:
                    mAnim.TypeWriterScale(typeSpeed);
                    break;
                case MTextTypewriterType.Translation:
                    mAnim.TypeWriterTranslation(typeSpeed);
                    break;
                case MTextTypewriterType.Rotation:
                    mAnim.TypeWriterRotation(typeSpeed);
                    break;
                case MTextTypewriterType.Wave:
                    mAnim.TypeWriterWave(typeSpeed);
                    break;
                case MTextTypewriterType.Shake:
                    mAnim.TypeWriterShake(typeSpeed);
                    break;
            }
        }

        private void PlayTypeWriterInternalFastly()
        {
            if (typeWriterSwitch == TypeWriterSwitch.Off) return;

            typewriterPlaying = true;
            float speed = 16777216;

            mAnim.TypeWriterFade(speed);//�ٶȹ����޷�����ϸ�ڣ�ֱ�ӽ���Ч�����
        }

        private void FinishTypeWriterInternalFastly()
        {
            typewriterPlaying = true;

            float speed = 16777216;
            mAnim.TypeWriterFadePart(mAnim.curTypeWriterStartIndex + 1, mAnim.textData.charData.Length - 1, speed);//�ٶȹ����޷�����ϸ�ڣ�ֱ�ӽ���Ч�����
        }
    }

    public enum TypeWriterSwitch
    {
        Off,
        On
    }
    public enum InlineEffectSwitch
    {
        Off,
        On
    }

    public enum MTextTypewriterType
    {
        Show,
        Fade,
        Scale,
        Translation,
        Rotation,
        Wave,
        Shake
    }
}