using MFramework.UI;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace MFramework
{
    public class ParsedText
    {
        private string originText;//ԭ����---����ı�
        private string localText;//�����˱��ػ�����ַ���---����ı�
        private string localedText;//�����ػ�Ӧ�ã�����û�д��������ַ���---state�������ı�
        private string parsedText;//��ǰ�Ľ����ַ���(����б��ػ��ͻ�����滻)---��localedText���

        private SupportLanguage language;

        internal bool haveLocalInline;//�������ػ������������磺{#��|��|��}����
        internal bool haveAnimatorInline;//�������������������磺<wave>�������</>

        private List<LocalInfo> localInfoList;//�洢���ĸ�ʽ��{index}���е���Ϣ
        internal List<AnimatorInfo> animatorInfoList;//������Ϣ

        /// <summary>
        /// ���ģ���Ϊ4���׶ε�����
        /// 1.originText---ֱ���ռ�����ԭʼ���ݣ���ʱ���ػ�����{}�붯������<></>û��ȥ��
        /// 2.localText---���б��ػ���������ݣ���ʱ�Ὣ{����}ת��Ϊ{0}{1}����ʽ
        /// 3.localedText---Ӧ�õ�ǰ���ػ���������ݣ�ÿ����({0})�����ж��ѡ���ʱ��ѡ������ѡ��
        /// 4.parsedText---��Ӧ�ö��������ݣ���ʱ��Ϊ��������
        /// </summary>
        public ParsedText(string text, SupportLanguage language)
        {
            originText = text;
            this.language = language;

            ParseText();
        }
        private void ParseText()
        {
            localText = GetLocalText(originText);
            if (localText != originText)//���б��ػ�����
            {
                haveLocalInline = true;
                localInfoList = GetLocalInfo(originText);
                localedText = GetLocaledText(localText);

                DealAnimator(localedText);
            }
            else//�����б��ػ�����
            {
                haveLocalInline = false;
                localText = null;

                DealAnimator(originText);
            }
        }

        public override string ToString()
        {
            if (!haveLocalInline && !haveAnimatorInline) return originText;

            return parsedText;
        }

        #region ��������
        /// <summary>
        /// ��������(���ػ���������ѡ��)
        /// </summary>
        public void UpdateData()
        {
            if (!haveLocalInline) return;//û�б��ػ���������û����Ҫ���µ�����

            //�����������ʹ����ChangeLocalState()������ĳ��{}�е����ֵ�������ı仯
            localedText = GetLocaledText(localText);//����localedText
            DealAnimator(localedText);//ͬ������parsedText
        }

        /// <summary>
        /// ���ı��ػ���������
        /// </summary>
        internal void ChangeLocalState(int pos, int state)
        {
            if (pos < 0 || pos >= localInfoList.Count) return;

            localInfoList[pos].curStr = localInfoList[pos].texts[state];
            localInfoList[pos].curState = state;
        }

        internal int GetCurState(int pos)
        {
            if (pos < 0 || pos >= localInfoList.Count) return -1;

            return localInfoList[pos].curState;
        }
        #endregion

        #region �����ı���Ϣ
        /// <summary>
        /// �����ػ�(��{xx|xx}����Ϊ{index})
        /// </summary>
        private string GetLocalText(string originStr)
        {
            string pattern = @"\{[^}]*\}";//{�����ַ�}
            int count = 0;
            string res = Regex.Replace(originStr, pattern, m => $"{{{count++}}}");//������{�����ַ�}�滻Ϊ{��ǰ����}

            return res;
        }
        private List<LocalInfo> GetLocalInfo(string originStr)
        {
            string pattern = @"\{([^}]*)\}";//{�����ַ�}�������ַ������Ϊ��
            MatchCollection matches = Regex.Matches(originStr, pattern);
            List<LocalInfo> res = new List<LocalInfo>();
            foreach (Match match in matches)
            {
                string innerStr = match.Groups[1].Value;
                res.Add(new LocalInfo(innerStr));
            }

            return res;
        }
        private string GetLocaledText(string localText)
        {
            if (string.IsNullOrEmpty(localText)) return null;

            localedText = localText;
            for (int i = 0; i < localInfoList.Count; i++)
            {
                localedText = localedText.Replace($"{{{i}}}", localInfoList[i].ToString());
            }
            return localedText;
        }
        private void DealAnimator(string str)
        {
            string pattern = @"<([^>]+)>(.*?)</>";

            //�磺<wave(1,2)>ABC</>
            MatchCollection matches = Regex.Matches(str, pattern);

            if (matches.Count > 0)
            {
                haveAnimatorInline = true;
                animatorInfoList = new List<AnimatorInfo>();
            }
            else 
            {
                haveAnimatorInline = false;
                parsedText = str;
                return;
            } 

            foreach (Match match in matches)
            {
                string content = match.Groups[2].Value;//��ABC
                //match.Index---����������(ָ��ǰ<>ǰ���Ԫ�ظ���)
                //match.Value.IndexOf(content)---ָǰ<>�е�Ԫ�ظ���
                //before---ֱ��<wave>ǰ������(����<wave>)
                int beforeLength = match.Index + match.Value.IndexOf(content);
                string beforeStr = str.Substring(0, beforeLength);
                //��ȥ����<wave>֮ǰ��tags(����<wave>��������֮���һ��</>)
                int startIndex = beforeLength - CountTagsLength(beforeStr) - CountSpacesLength(beforeStr);
                int endIndex = startIndex + content.Length - 1 - CountSpacesLength(content);
                string funcStr = match.Groups[1].Value;//��wave(1,2)
                string typeStr, argStr;
                GetTypeStrAndArgStr(funcStr, out typeStr, out argStr);
                animatorInfoList.Add(new AnimatorInfo(content, startIndex, endIndex, typeStr, argStr));
            }

            //ȥ��<>�õ������ַ���
            parsedText = Regex.Replace(str, pattern, m => m.Groups[2].Value);
        }


        /// <summary>
        /// ��������<>�ĳ���
        /// </summary>
        /// <param playerName="str"></param>
        /// <returns></returns>
        private int CountTagsLength(string str)
        {
            int length = 0;
            string pattern = @"<[^>]+>";

            MatchCollection matches = Regex.Matches(str, pattern);
            foreach (Match match in matches)
            {
                length += match.Length;
            }

            return length;
        }
        private int CountSpacesLength(string str)
        {
            int spaceCount = 0;
            foreach (char c in str)
            {
                if (char.IsWhiteSpace(c))
                {
                    spaceCount++;
                }
            }
            return spaceCount;
        }
        private void GetTypeStrAndArgStr(string funcStr, out string typeStr, out string argStr)
        {
            int n = funcStr.Length;
            int splitIndex = funcStr.IndexOf('(');
            if (splitIndex == -1)//û������β����
            {
                typeStr = funcStr;
                argStr = null;
                return;
            }
            typeStr = funcStr.Substring(0, splitIndex);
            argStr = funcStr.Substring(splitIndex + 1, n - splitIndex - 2);
        }
        #endregion

        /// <summary>
        /// Ӧ����������Ч��
        /// </summary>
        internal void ApplyEffects(MTextAnimation anim)
        {
            if (animatorInfoList == null) return;

            foreach (var info in animatorInfoList)
            {
                ApplyEffect(anim, info);
            }
        }
        internal void ApplyEffect(MTextAnimation anim, AnimatorInfo info)
        {
            if (info == null) return;

            int count = info.GetCount();//�β�����

            //��ɫColor
            //color(speed,r,g,b)---�ٶ�Ĭ��Ϊ1
            //color(r,g,b)
            if (info.type == AnimatorType.Color)
            {
                if (!CheckArgsNum(count, 3, 4)) return;

                if (count == 3)
                {
                    anim.Color(1, new Color32((byte)info.args[0], (byte)info.args[1], (byte)info.args[2], 255), info.start, info.end);
                }
                else if (count == 4)
                {
                    anim.Color(info.args[0], new Color32((byte)info.args[1], (byte)info.args[2], (byte)info.args[3], 255), info.start, info.end);
                }
            }
            //����Scale
            //scale(scaleFactor, speed)---�ٶ�Ĭ��Ϊ1
            //scale(scaleFactor)
            else if (info.type == AnimatorType.Scale)
            {
                if (!CheckArgsNum(count, 1, 2)) return;

                if (count == 1)
                {
                    anim.Scale(1, info.args[0], info.start, info.end);
                }
                else if (count == 2)
                {
                    anim.Scale(info.args[1], info.args[0], info.start, info.end);
                }
            }
            //��תRotate  ע��degree>0Ϊ˳ʱ��
            //rotate(degree, speed)---�ٶ�Ĭ��Ϊ1
            //rotate(degree)
            else if (info.type == AnimatorType.Rotate)
            {
                if (!CheckArgsNum(count, 1, 2)) return;

                if (count == 1)
                {
                    anim.Rotate(1, info.args[0], info.start, info.end);
                }
                else if (count == 2)
                {
                    anim.Rotate(info.args[1], info.args[0], info.start, info.end);
                }
            }
            //����FadeIn
            //fadein(speed)---�ٶ�Ĭ��Ϊ1
            //fadein()
            else if (info.type == AnimatorType.FadeIn)
            {
                if (!CheckArgsNum(count, 0, 1)) return;

                if (count == 0)
                {
                    anim.FadeIn(1, info.start, info.end);
                }
                else if (count == 1)
                {
                    anim.FadeIn(1 * info.args[0], info.start, info.end);
                }
            }
            //����FadeOut
            //fadeout(speed)---�ٶ�Ĭ��Ϊ1
            //fadeout()
            else if (info.type == AnimatorType.FadeOut)
            {
                if (!CheckArgsNum(count, 0, 1)) return;

                if (count == 0)
                {
                    anim.FadeOut(1, info.start, info.end);
                }
                else if (count == 1)
                {
                    anim.FadeOut(1 * info.args[0], info.start, info.end);
                }
            }
            //ѭ��������ɫColorFade
            //colorfade(speed,r,g,b)---�ٶ�Ĭ��Ϊ1
            //colorfade(r,g,b)
            else if (info.type == AnimatorType.ColorFade)
            {
                if (!CheckArgsNum(count, 3, 4)) return;

                else if (count == 3)
                {
                    anim.ColorFade(1, new Color32((byte)info.args[0], (byte)info.args[1], (byte)info.args[2], 255), info.start, info.end);
                }
                else if (count == 4)
                {
                    anim.ColorFade(info.args[0], new Color32((byte)info.args[1], (byte)info.args[2], (byte)info.args[3], 255), info.start, info.end);
                }
            }
            //��Shake
            //��ʱ
            //shake(duration,amplitude,frequency) amplitude---ǿ��(Ĭ��1) frequency---Ƶ��(Ĭ��1)
            //shake(duration)
            //����
            //shake(amplitude,frequency)
            //shake()
            else if (info.type == AnimatorType.Shake)
            {
                if (!CheckArgsNum(count, 0, 1, 2, 3)) return;

                if (count == 0)
                {
                    anim.Shake(info.start, info.end);
                }
                else if (count == 1)
                {
                    anim.Shake(info.start, info.end, info.args[0]);
                }
                else if (count == 2)
                {
                    anim.Shake(info.start, info.end, -1, info.args[0], info.args[1]);
                }
                else if (count == 3)
                {
                    anim.Shake(info.start, info.end, info.args[0], info.args[1], info.args[2]);
                }
            }
            //��бSlant
            //slant(speed, degree)---�ٶ�Ĭ��Ϊ1 �Ƕ�Ĭ��Ϊ15��
            //slant(speed)
            //slant()
            else if (info.type == AnimatorType.Slant)
            {
                if (!CheckArgsNum(count, 0, 1, 2)) return;

                if (count == 0)
                {
                    anim.Slant(1, info.start, info.end);
                }
                else if (count == 1)
                {
                    anim.Slant(info.args[0], info.start, info.end);
                }
                else if (count == 2)
                {
                    anim.Slant(info.args[0], info.start, info.end, info.args[1]);
                }
            }
            //��˸Twinkle
            //twinkle(speed)
            //twinkle()---�ٶ�Ĭ��Ϊ1
            else if (info.type == AnimatorType.Twinkle)
            {
                if (!CheckArgsNum(count, 0, 1)) return;

                if (count == 0)
                {
                    anim.Twinkle(1, info.start, info.end);
                }
                else if (count == 1)
                {
                    anim.Twinkle(info.args[0], info.start, info.end);
                }
            }
            //���Exclaim(��+�Ӵ�)
            //��ʱ
            //exclaim(speed,duration,scaleFactor,r,g,b)
            //exclaim(speed,duration)
            //����
            //exclaim(speed,sacleFactor,r,g,b)
            //exclaim(speed)
            //exclaim()
            else if (info.type == AnimatorType.Exclaim)
            {
                if (!CheckArgsNum(count, 0, 1, 2, 5, 6)) return;

                if (count == 0)
                {
                    anim.Exclaim(1, info.start, info.end);
                }
                else if (count == 1)
                {
                    anim.Exclaim(info.args[0], info.start, info.end);
                }
                else if (count == 2)
                {
                    anim.ExclaimTimed(info.args[0], info.args[1], info.start, info.end);
                }
                else if (count == 5)
                {
                    anim.ExclaimColor(info.args[0], new Color32((byte)info.args[2], (byte)info.args[3], (byte)info.args[4], 255), info.start, info.end, info.args[1]);
                }
                else if (count == 6)
                {
                    anim.ExclaimColorTimed(info.args[0], info.args[1], new Color32((byte)info.args[3], (byte)info.args[4], (byte)info.args[5], 255), info.start, info.end, info.args[2]);
                }
            }
            //���޲���WaveLoop
            //wave(speed, amplitude, loopIntervalFactor) amplitude---ǿ��(Ĭ��1) loopIntervalFactor---ÿ��ѭ�����ʱ��(Ĭ��Ϊ1������Ϊһ�ֽ���ֱ�ӿ�ʼ��һ��)
            //wave(speed)
            //wave()
            else if (info.type == AnimatorType.WaveLoop)
            {
                if (!CheckArgsNum(count, 0, 1)) return;

                if (count == 0)
                {
                    anim.Wave(1, info.start, info.end);
                }
                else if (count == 1)
                {
                    anim.Wave(info.args[0], info.start, info.end);
                }
                else if (count == 3)
                {
                    anim.Wave(info.args[0], info.start, info.end, info.args[1], info.args[2]);
                }
            }
        }
        private bool CheckArgsNum(int count, params int[] validNums)
        {
            if (!validNums.Contains(count))
            {
                MLog.Print($"{typeof(ParsedText)}��{ToString()}���������β�����������Ҫ������", MLogType.Warning);
                return false;
            }
            return true;
        }
    }

    public class LocalInfo
    {
        internal string curStr;

        internal int curState = 0;//��ǰѡ�Ĭ��ʹ�õ�һ��ѡ��
        internal string[] texts;//ѡ��ľ�������

        public LocalInfo(string str)
        {
            string[] strs = str.Split('|');

            for (int i = 0; i < strs.Length; i++)
            {
                if (strs[i].StartsWith('#'))
                {
                    strs[i] = strs[i].Substring(1);
                    curState = i;//Ĭ��ѡ��
                }
            }

            texts = strs;
            curStr = texts[curState];
        }

        public override string ToString()
        {
            return curStr;
        }
    }

    public class AnimatorInfo
    {
        internal string str;

        internal int start;
        internal int end;

        internal AnimatorType type;//��������
        internal List<float> args;//�����б�

        public AnimatorInfo(string str, int start, int end, string typeStr, string argStr)
        {
            this.str = str;
            this.start = start;
            this.end = end;

            GetType(typeStr);
            GetArgs(argStr);
        }

        public int GetCount()
        {
            if (args == null) return 0;

            return args.Count;
        }

        private void GetType(string typeStr)
        {
            typeStr = typeStr.ToLower();

            if (typeStr == MTextAnimatorName.COLORFADE)
            {
                type = AnimatorType.ColorFade;
            }
            else if (typeStr == MTextAnimatorName.COLOR)
            {
                type = AnimatorType.Color;
            }
            else if (typeStr == MTextAnimatorName.SLANT)
            {
                type = AnimatorType.Slant;
            }
            else if (typeStr == MTextAnimatorName.EXCLAIM)
            {
                type = AnimatorType.Exclaim;
            }
            else if (typeStr == MTextAnimatorName.FADEIN)
            {
                type = AnimatorType.FadeIn;
            }
            else if (typeStr == MTextAnimatorName.FADEOUT)
            {
                type = AnimatorType.FadeOut;
            }
            else if (typeStr == MTextAnimatorName.WAVELOOP)
            {
                type = AnimatorType.WaveLoop;
            }
            else if (typeStr == MTextAnimatorName.ROTATE)
            {
                type = AnimatorType.Rotate;
            }
            else if (typeStr == MTextAnimatorName.SCALE)
            {
                type = AnimatorType.Scale;
            }
            else if (typeStr == MTextAnimatorName.SHAKE)
            {
                type = AnimatorType.Shake;
            }
            else if (typeStr == MTextAnimatorName.TWINKLE)
            {
                type = AnimatorType.Twinkle;
            }
            else
            {
                MLog.Print($"{typeof(MText)}��δ֪����������{typeStr}������", MLogType.Warning);
                type = AnimatorType.None;
            }
        }

        private void GetArgs(string argStr)
        {
            if (string.IsNullOrEmpty(argStr)) return;

            args = new List<float>();

            string[] strs = argStr.Split(',');
            strs = strs.Select(s => s.Trim()).ToArray();//ȥ������ո�

            foreach (var str in strs)
            {
                args.Add(float.Parse(str));
            }
        }
    }

    public enum AnimatorType
    {
        None,

        ColorFade,
        FadeIn,
        FadeOut,
        Slant,
        Shake,
        Exclaim,
        WaveLoop,
        Scale,
        Rotate,
        Color,
        Twinkle
    }
}