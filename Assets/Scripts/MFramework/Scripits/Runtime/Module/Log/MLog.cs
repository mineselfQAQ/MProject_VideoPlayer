using System.IO;
using UnityEngine;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MFramework
{
    public class MLog : INeedInit, INeedQuit
    {
        private static FileStream fs;
        private static string path = $@"{Application.dataPath}/../LogCallBack.txt";

        public void Init()
        {
#if !UNITY_EDITOR
            if (MCore.Instance.LogState)
            {
                Application.logMessageReceived += OnLogCallBack;
            }
#endif
        }
        public void Quit()
        {
#if !UNITY_EDITOR
            if (MCore.Instance.LogState)
            {
                Application.logMessageReceived -= OnLogCallBack;
            }
#endif
        }

        public static void Blank()
        {
#if UNITY_EDITOR
            Debug.Log("");
#endif
        }

        public static void Print(object message, MLogType type = MLogType.Log, Object context = null)
        {
            switch (type) 
            {
#if UNITY_EDITOR
                case MLogType.Log:
                    Debug.Log($"<b>Log:</b> {message}", context);
                    break;
                case MLogType.Warning:
                    Debug.LogWarning($"<b><color=#CC9A06FF>Warning:</color></b> {message}", context);
                    break;
                case MLogType.Error:
                    Debug.LogError($"<b><color=#FF6E40FF>Error:</color></b> {message}", context);
                    throw new System.Exception("����������������ִ��");
#else
                case MLogType.Log:
                    if (MCore.Instance.LogState) Debug.Log($"Log: {message}", context);
                    break;
                case MLogType.Warning:
                    if (MCore.Instance.LogState) Debug.LogWarning($"Warning: {message}", context);
                    break;
                case MLogType.Error:
                    if (MCore.Instance.LogState) Debug.LogError($"Error: {message}", context);
                    throw new System.Exception("����������������ִ��");
#endif
            }
        }

        public static string BoldWord(object message)
        {
            string resultStr = message.ToString();
#if UNITY_EDITOR
            resultStr = $"<b>{message}</b>";

            return resultStr;
#else
            return resultStr;
#endif
        }

        public static string ItalicWord(object message)
        {
            string resultStr = message.ToString();
#if UNITY_EDITOR
            resultStr = $"<i>{message}</i>";

            return resultStr;
#else
            return resultStr;
#endif
        }

        public static string ColorWord(object message, Color color)
        {
#if UNITY_EDITOR
            return AddColor(message, color);
#else
            return message.ToString();
#endif
        }
        public static string ColorWord(object message, Color color, bool isBold, bool isItalic)
        {
#if UNITY_EDITOR
            string resultStr = AddColor(message, color);

            if (isBold)
            {
                resultStr = $"<b>{resultStr}</b>";
            }
            if (isItalic)
            {
                resultStr = $"<i>{resultStr}</i>";
            }
            
            return resultStr;
#else
            return message.ToString();
#endif
        }

        private static void OnLogCallBack(string logString, string stackTrace, LogType type)
        {
            //�������
            fs = new FileStream(path, FileMode.OpenOrCreate);
            //����"ָ��"
            fs.Position = fs.Length;

            //�������
            string str = null;
            str = $"Time: {GetCurrTime()}\n" +
                    $"{logString}\n\n\n";

            //���벢д��
            byte[] bytes = System.Text.Encoding.Default.GetBytes(str);
            fs.Write(bytes, 0, bytes.Length);
            //�ر�
            fs.Close();
        }



        private static string GetCurrTime()
        {
            return System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff");
        }
        private static string AddColor(object message, Color color)
        {
            string htmlColor = ConvertColorToHtml(color);
            string resultStr = $"<color=#{htmlColor}>{message}</color>";

            return resultStr;
        }
        private static string ConvertColorToHtml(Color color)
        {
            return ColorUtility.ToHtmlStringRGBA(color);
        }



#if UNITY_EDITOR
        //����(0)������ӣ������ȼ��ĺ��壬����������ʱ���ᴥ��
        [UnityEditor.Callbacks.OnOpenAsset(0)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            //������Ҫ�õ�Console������˫�����������Ϣ
            string stackTrace = GetStackTrace();

            //����õ�������"MLog.cs"��˵�����ȷʵ����ת����(��ջ�����Ļ����ҵ�MLog���µ�ĳ������)
            if (!string.IsNullOrEmpty(stackTrace) && stackTrace.Contains("MLog.cs"))
            {
                //Ѱ�����ݣ�
                //(at xxx)������ʵ����׷�ٺ�����ʱ��λ���Ľű���Ϣ
                //������(at Assets/Utils/MLog.cs:53)
                var matches = Regex.Match(stackTrace, @"\(at (.+)\)", RegexOptions.IgnoreCase);
                string pathLine = "";
                //ѭ�����ң��Ȼ�ƥ���һ���ҵ��ģ�Ȼ��ʹ��NextMatch()�ҵ��ڶ������Դ�����
                while (matches.Success)
                {
                    //��[1]��ָ���ľ���(.+)���������������˵������Assets/Utils/MLog.cs:53
                    pathLine = matches.Groups[1].Value;

                    //���ݶ�ջ�����е����ݣ����ǿ���֪����
                    //������Ҫ���ǵ�һ������MLog.cs������(������Ϊ��װ��������һ�㣬����MLog.cs�ŵ��µ�����)
                    if (!pathLine.Contains("MLog.cs"))
                    {
                        //��֣����·�����к�
                        int splitIndex = pathLine.LastIndexOf(":");
                        string path = pathLine.Substring(0, splitIndex);
                        line = System.Convert.ToInt32(pathLine.Substring(splitIndex + 1));
                        //ƴ�ӵõ��ű�������·��
                        string fullPath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("Assets"));
                        fullPath = fullPath + path;
                        //��ת������Ӧ����ת����λ��
                        UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(fullPath.Replace('/', '\\'), line);
                        break;
                    }
                    matches = matches.NextMatch();
                }
                return true;
            }
            return false;
        }

        private static string GetStackTrace()
        {
            System.Type ConsoleWindowType = typeof(UnityEditor.EditorWindow).Assembly.GetType("UnityEditor.ConsoleWindow");
            // ��ȡConsole����ʵ��
            FieldInfo fieldInfo = ConsoleWindowType.GetField
                ("ms_ConsoleWindow", BindingFlags.Static | BindingFlags.NonPublic);
            object consoleInstance = fieldInfo.GetValue(null);
            if (consoleInstance != null)
            {
                //�жϵ�ǰ�۽��Ĵ����Ƿ�ΪConsole���
                if ((object)UnityEditor.EditorWindow.focusedWindow == consoleInstance)
                {
                    // ��ȡm_ActiveText��Ա
                    fieldInfo = ConsoleWindowType.GetField
                        ("m_ActiveText", BindingFlags.Instance | BindingFlags.NonPublic);
                    // ��ȡm_ActiveText��ֵ
                    string activeText = fieldInfo.GetValue(consoleInstance).ToString();

                    return activeText;
                }
            }
            return null;
        }
#endif
    }
}
