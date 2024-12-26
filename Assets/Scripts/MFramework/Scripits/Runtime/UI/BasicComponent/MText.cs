using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MFramework.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MLocalization))]
    [AddComponentMenu("MFramework/MText")]
    public class MText : TextMeshProUGUI
    {
        public static List<MText> textList = new List<MText>();//���е�MText�б�

        public MLocalization mLocal;
        public MTextAnimator mAnimator;

        internal Dictionary<SupportLanguage, ParsedText> parsedTextDic;//������ʽ�����ַ���(�������Ի�ȡ)
        //�����ж�����
        internal bool enableLocalization = false;
        internal bool enableTextAnimatorInline = false;

        protected override void Awake()
        {
            base.Awake();

            //����ʹ����[ExecuteAlways]����Ҫ��ֹ�༭���µ��ã���MLocalizationManager.Instance.asset.tableDic���޷��ڱ༭��ģʽ�»�ȡ
            if (Application.isPlaying)
            {
                //�ӳ�һ֡
                //MCoroutineManager.Instance.DelayOneFrame(() => Init());
                Init();
            }
        }
        private void Init()
        {
            textList.Add(this);

            mLocal = GetComponent<MLocalization>();//�ض�����
            mAnimator = GetComponent<MTextAnimator>();//���п���
            mAnimator?.Init();

            if (mLocal.LocalMode == LocalizationMode.On && mLocal.LocalID != -1)
            {
                //�п��ܴ��ڱ��ػ�����(��һ�����ǿ�ѡ����)
                enableLocalization = true;
            }
            if (mAnimator != null && mAnimator.inlineEffectsSwitch == InlineEffectSwitch.On)
            {
                //�п��ܴ��ڶ�������(��һ����ֻ�������˶���)
                enableTextAnimatorInline = true;
            }

            if (enableLocalization || enableTextAnimatorInline)
            {
                //Tip:
                //�������ػ�---����key-���� value-�����ַ������ֵ�(��������)
                //���������ػ�---����key-���� value-�����ַ������ֵ�(1������-Default)
                //�Ƿ����ļ�������������ParsedText֮��
                parsedTextDic = GenerateParsedTextDic();
                UpdateView();
            }
        }
        internal Dictionary<SupportLanguage, ParsedText> GenerateParsedTextDic()
        {
            Dictionary<SupportLanguage, ParsedText> dic = null;
            if (enableLocalization)//�������ػ�
            {
                var tableDic = MLocalizationManager.Instance.asset.tableDic;
                int id = mLocal.LocalID;
                if (tableDic.ContainsKey(id))
                {
                    var table = tableDic[id];
                    dic = new Dictionary<SupportLanguage, ParsedText>();
                    TableToParsedTextDic(table, dic);
                }
            }
            else//δ�������ػ�
            {
                //��ʱֻӦ����һ�����ԣ���ôֻ����SupportLanguage.Default
                //ͬʱ������û�п������ػ�����ôһ��ֻ�����������˲Ż����
                dic = new Dictionary<SupportLanguage, ParsedText>();
                ParsedText parsedText = new ParsedText(text, SupportLanguage.Default);//û�б��ػ�ʱֻ����ʹ��MText�и�ֵ��text
                dic.Add(SupportLanguage.Default, parsedText);
            }
            return dic;
        }
        private void TableToParsedTextDic(LocalizationTable table, Dictionary<SupportLanguage, ParsedText> dic)
        {
            int n = MLocalizationManager.Instance.SupportLanguagesCount;
            for (int i = 0; i < n; i++)
            {
                SupportLanguage lanaguage = MLocalizationManager.Instance.SupportLanguages[i];
                Type type = typeof(LocalizationTable);
                var info = type.GetProperty(lanaguage.ToString());//��ȡ��Ҫ������(PropertyInfo)
                string text = (string)info.GetValue(table);//ͨ��ʵ����ȡ�洢������

                dic.Add(lanaguage, new ParsedText(text, lanaguage));
            }
        }

        protected override void Start()
        {
            base.Start();

            if (Application.isPlaying)
            {
                PlayAnimator();
            }
        }
        private void PlayAnimator()
        {
            ParsedText pText = GetCurParsedText();
            mAnimator?.PlayInit(pText);
        }
        internal ParsedText GetCurParsedText()
        {
            if (parsedTextDic == null) return null;

            if (parsedTextDic.ContainsKey(SupportLanguage.Default))//Default���
            {
                return parsedTextDic[SupportLanguage.Default];
            }
            else
            {
                return parsedTextDic[MLocalizationManager.Instance.CurrentLanguage];
            }
        }

        #region ���ֶ���
        public void PlayText()
        {
            if (mAnimator == null)
            {
                MLog.Print($"{typeof(MText)}��{name}��û�����MTextAnimator���������Ӻ��Ե��ô��ֻ�Ч��", MLogType.Warning);
                return;
            }

            mAnimator.PlayText();
        }

        public void PlayTextFastly()
        {
            if (mAnimator == null)
            {
                MLog.Print($"{typeof(MText)}��{name}��û�����MTextAnimator���������Ӻ��ٳ���", MLogType.Warning);
                return;
            }

            mAnimator.PlayTextFastly();
        }

        public void FinishTextImmediately()
        {
            if (mAnimator == null)
            {
                MLog.Print($"{typeof(MText)}��{name}��û�����MTextAnimator���������Ӻ��ٳ���", MLogType.Warning);
                return;
            }

            mAnimator.FinishTextImmediately();
        }
        #endregion

        #region ���ݸ���
        public static void UpdateAllInfo()
        {
            foreach (var text in textList)
            {
                text.UpdateInfo();
            }
        }

        internal void UpdateInfo()
        {
            if (parsedTextDic == null) return;

            //�������������˵���п�����Ҫ����
            //bool haveAnimatorInline = false, haveLocalInline = false;
            //foreach (var parsedText in parsedTextDic.Values)
            //{
            //    haveAnimatorInline = parsedText.haveAnimatorInline;
            //    haveLocalInline = parsedText.haveLocalInline;
            //    break;
            //}

            //�������ػ����п�����Ҫ����
            if (enableLocalization)
            {
                //�����е�ParsedText���и���(��Ϊ���ػ���������ѡ��)
                foreach (var parsedText in parsedTextDic.Values)
                {
                    parsedText.UpdateData();
                }
                UpdateView();//������ͼ
            }
        }

        internal void UpdateView()
        {
            //2�ָ��µĿ��ܣ�
            //1.�����л�
            //2.���ػ���������ѡ��

            SupportLanguage curLanguage = MLocalizationManager.Instance.CurrentLanguage;
            if (parsedTextDic.ContainsKey(SupportLanguage.Default))
            {
                text = parsedTextDic[SupportLanguage.Default].ToString();
            }
            else if (parsedTextDic.ContainsKey(curLanguage))
            {
                text = parsedTextDic[curLanguage].ToString();
            }
        }
        #endregion

        public void UpdateParsedTextDic(int id)
        {
            mLocal.LocalID = id;
            parsedTextDic = GenerateParsedTextDic();
            UpdateView();
        }
    }

    public static class MTextExtension
    {
        public static void ChangeState(this MText text, int pos, int state)
        {
            foreach (var parsedText in text.parsedTextDic.Values)
            {
                parsedText.ChangeLocalState(pos, state);
            }

            text.UpdateInfo();
        }

        public static int GetCurState(this MText text, int pos)
        {
            foreach (var parsedText in text.parsedTextDic.Values)
            {
                return parsedText.GetCurState(pos);
            }
            return -1;
        }

        /// <summary>
        /// ���ݴ���Alphaֵ����
        /// </summary>
        public static void SetAlpha(this TMP_Text text, float alpha)
        {
            alpha = Mathf.Clamp01(alpha);

            Color32 c = text.color.ToColor32();
            c.a = (byte)(alpha * 255);//����alpha
            SetColor(text, c);
        }
        public static void SetAlpha(this TMP_Text text, float alpha, int startIndex, int endIndex)
        {
            alpha = Mathf.Clamp01(alpha);

            Color32 c = text.color.ToColor32();
            c.a = (byte)(alpha * 255);//����alpha
            SetColor(text, c, startIndex, endIndex);
        }
        
        /// <summary>
        /// ���ݴ���Color32ֵ����
        /// </summary>
        public static void SetColor(this TMP_Text text, Color32 color)
        {
            for (int i = 0; i < text.textInfo.meshInfo[0].colors32.Length; i++)
            {
                text.textInfo.meshInfo[0].colors32[i] = color;
            }
            text.UpdateVertexData();//ˢ��
        }
        public static void SetColor(this TMP_Text text, Color32 color, int startIndex, int endIndex)
        {
            //0-0 1-4 2-8 3-12
            //0-3 1-7 2-11 3-15
            for (int i = startIndex * 4; i <= endIndex * 4 + 3; i++)
            {
                text.textInfo.meshInfo[0].colors32[i] = color;
            }
            text.UpdateVertexData();//ˢ��
        }
    
        public static void ApplyEffects(this MText text)
        {
            text.GetCurParsedText().ApplyEffects(text.mAnimator.mAnim);
        }

        public static void ApplyEffect(this MText text, int index)
        {
            ParsedText pText = text.GetCurParsedText();
            pText.ApplyEffect(text.mAnimator.mAnim, pText.animatorInfoList[index]);
        }

        public static void SetMText(this MText text, string content)
        {
            //����SetAlpha()�����ܼ�ʹ��Ч�����������أ��ڵ���֮ǰ��ʾ����
            text.alpha = 0;

            text.text = content;
            if (text.enableLocalization || text.enableTextAnimatorInline)
            {
                text.parsedTextDic = text.GenerateParsedTextDic();
                text.UpdateView();
            }
            text.mAnimator.PlayNewText();
        }
        /// <summary>
        /// ͨ��localID��������
        /// </summary>
        public static void SetMText(this MText text, int localID)
        {
            //Tip������ʹ�ñ��ػ���������Ҫ��localID����Ϊ��-1

            //����SetAlpha()�����ܼ�ʹ��Ч�����������أ��ڵ���֮ǰ��ʾ����
            text.alpha = 0;

            text.UpdateParsedTextDic(localID);
            text.mAnimator.PlayNewText();
        }
    }
}