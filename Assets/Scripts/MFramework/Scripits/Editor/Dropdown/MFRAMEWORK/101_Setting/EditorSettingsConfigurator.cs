using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using static MFramework.MConfigurableSettingsBase;

namespace MFramework
{
    public class EditorSettingsConfigurator : EditorWindow
    {
        private Vector2 scrollPos1;
        private Vector2 scrollPos2;

        [MenuItem("MFramework/EditorSettingsConfigurator", false, 101)]
        public static void Init()
        {
            EditorSettingsConfigurator window = GetWindow<EditorSettingsConfigurator>();
            window.minSize = new Vector2(600, 300);
            window.maxSize = new Vector2(1000, 300);
            window.Show();
        }

        private void OnGUI()
        {
            //==========����==========
            MEditorGUIUtility.DrawH1("�༭��������");

            //==========Excel==========
            MEditorGUIUtility.DrawH2("Excel����");
            //TODO:ʹ�������б�ѡ����Ҫ��ʾ�Ĳ���(Excel����/Json����)������ʾ��Ӧ����(��ʡ�ռ�)
            scrollPos1 = EditorGUILayout.BeginScrollView(scrollPos1);
            {
                //------�ѷ�����ʹ��MSettings�еĹ̶�·��------
                DrawPathWidget("Excel������·����", MConfigurableSettings.ExcelPath,
                    GetPathName(MConfigurableName.ExcelGenerationPath));
                DrawPathWidget("Excel��CS�ļ�����·����", MConfigurableSettings.ExcelCSPath,
                    GetPathName(MConfigurableName.ExcelCSGenerationPath));
            }
            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space(10);

            //==========����==========
            EditorGUILayout.BeginHorizontal();
            {
                DrawResetBtn();
                DrawCheckCSBtn();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            MEditorGUIUtility.DrawH2("Boolֵ");
            DrawEnableCheckMCoreExistBool();

            EditorGUILayout.Space(5);
        }

        private void DrawPathWidget(string title, string path, string originName)
        {
            EditorGUILayout.LabelField(title, MEditorGUIStyleUtility.BoldStyle);
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(path);
                //GUILayout.FlexibleSpace();
                if (GUILayout.Button("�鿴", GUILayout.Width(50)))
                {
                    System.Diagnostics.Process.Start(path);
                }
                if (GUILayout.Button("����", GUILayout.Width(50)))
                {
                    ChangePath(originName);
                    AssetDatabase.Refresh();
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawResetBtn()
        {
            if (GUILayout.Button("����ΪĬ������"))
            {
                EnsureFolderExist();
                RebuildAllEditorSettings();

                AssetDatabase.Refresh();
            }
        }
        private void RebuildAllEditorSettings()
        {
            string editorSettingsFilePath = GetEditorSettingsFilePath();//��ȡEditorSettings·��

            string code = EDITORSETTINGSCODE;

            string settings = GenerateSettings();
            code = code.Replace("{Settings}", settings);

            if (editorSettingsFilePath != null)
            {
                File.WriteAllText(editorSettingsFilePath, code);
            }
            else
            {
                MLog.Print($"δ�ҵ�EditorSettings�ļ���������ѡ���ļ������´���", MLogType.Warning);
                string newDirectoryPath = MEditorUtility.ChangePath();
                string newFilePath = Path.Combine(newDirectoryPath, "MConfigurableSettings.cs");
                File.WriteAllText(newFilePath, code);
            }
        }

        private void EnsureFolderExist()
        {
            foreach (var pair in pathDic)
            {
                MPathUtility.CreateFolderIfNotExist(pair.Value);
            }
        }

        private string GenerateSettings()
        {
            StringBuilder res = new StringBuilder();

            foreach (var pair in pathDic)
            {
                string tempLine = SETTINGSBASECODE;
                tempLine = tempLine.Replace("{ConstantName}", pair.Key);
                tempLine = tempLine.Replace("{Path}", pair.Value);

                res.Append(tempLine + "\n\t");
            }
            string resStr = res.ToString();
            resStr = resStr.TrimEnd('\t', '\n');

            return resStr;
        }

        private void DrawCheckCSBtn()
        {
            if (GUILayout.Button("�鿴EditorSettings�ű�"))
            {
                string editorSettingsFilePath = GetEditorSettingsFilePath();
                if (editorSettingsFilePath == null)
                {
                    MLog.Print("EditorSettings�ű������ڣ�����", MLogType.Warning);
                    return;
                }
                UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(editorSettingsFilePath, 3);
            }
        }

        private void DrawEnableCheckMCoreExistBool()
        {
            EditorGUILayout.BeginHorizontal();
            {
                bool flag = EditorPrefs.GetBool(EditorPrefsData.EnableCheckMCore, true);
                EditorGUILayout.LabelField($"�Ƿ�ǿ�����MCore:  {flag}");
                if (GUILayout.Button("����"))
                {
                    EditorPrefs.SetBool(EditorPrefsData.EnableCheckMCore, !flag);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private bool ResetPath(string originName, string newPath)
        {
            string editorSettingsFilePath = GetEditorSettingsFilePath();//��ȡEditorSettings·��
            if (editorSettingsFilePath != null)
            {
                //��λ��д��
                string str = File.ReadAllText(editorSettingsFilePath);
                string newStr = ReplacePath(str, originName, newPath);
                if (newStr != null) File.WriteAllText(editorSettingsFilePath, newStr);
                else { MLog.Print("δ�滻�ɹ�", MLogType.Error); return false; }
            }
            else
            {
                MLog.Print("δ�ҵ�·��", MLogType.Error);
                return false;
            }
            return true;
        }

        /// <summary>
        /// ͨ���ļ���ѡ�����EditorSettings�еı�����
        /// </summary>
        /// <param playerName="originName">EditorSettings�еı�����</param>
        public static bool ChangePath(string originName)
        {
            string guideFolder = Path.GetDirectoryName(Application.dataPath);
            string newPath = EditorUtility.OpenFolderPanel("��ѡ��Excel�洢·��", guideFolder, "");
            if (newPath == "")
            {
                MLog.Print("ȡ������", MLogType.Warning);
                return false;
            }

            string editorSettingsFilePath = GetEditorSettingsFilePath();//��ȡEditorSettings·��
            if (editorSettingsFilePath != null)
            {
                //��λ��д��
                string str = File.ReadAllText(editorSettingsFilePath);
                string newStr = ReplacePath(str, originName, newPath);
                if (newStr != null) File.WriteAllText(editorSettingsFilePath, newStr);
                else { MLog.Print("δ�滻�ɹ�", MLogType.Error); return false; }

                AssetDatabase.Refresh();
            }
            else
            {
                MLog.Print("δ�ҵ�·��", MLogType.Error);
                return false;
            }
            return true;
        }

        private static string ReplacePath(string str, string originName, string newPath)
        {
            char initials = originName[0];
            string oldPath = "";
            int i = 0;
            bool flag = false;

            //���������ַ���
            while (i < str.Length)
            {
                //������originName���ַ�ƥ�������
                if (str[i] == initials)
                {
                    //�ж��Ƿ���originName��ȫһ��
                    flag = true;
                    for (int j = 1; j < originName.Length; j++)
                    {
                        if (str[j + i] != originName[j])
                        {
                            flag = false;
                            break;
                        }
                    }
                }

                //�ҵ�originName��
                if (flag)
                {
                    i += originName.Length;//��i�����ڱ�������
                    int firstIndex;//����ĸ����

                    while (i < str.Length)
                    {
                        //Ѱ��������
                        if (str[i] == '\"')
                        {
                            firstIndex = i + 1;
                            int count = 0, index = i + 1;
                            while (str[index++] != '\"') count++;//ͳ�������ŵ�������֮�������
                            oldPath = str.Substring(firstIndex, count);//��ȡ·��
                            break;
                        }
                        i++;
                    }
                    //���ǰ׺(��ֹ·��һ�µ���ȫ���滻���)
                    oldPath = $@"{originName} = @""{oldPath}""";
                    newPath = $@"{originName} = @""{newPath}""";
                    return str.Replace(oldPath, newPath);
                }
                i++;
            }
            return null;
        }

        private static string GetEditorSettingsFilePath()
        {
            string[] guids = AssetDatabase.FindAssets("MConfigurableSettings");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(MonoScript));
                //Tip��FindAssets()ֻҪ���ֲ���ƥ�伴��,��"EditorSettings2"Ҳ����ƥ�䣬��Ҫ������֤����
                if (obj && obj.name == "MConfigurableSettings")
                {
                    string resPath = AssetDatabase.GetAssetPath(obj);
                    return resPath;
                }
            }
            return null;
        }

        private const string EDITORSETTINGSCODE =
@"public static class MConfigurableSettings
{
    {Settings}
}";
        private const string SETTINGSBASECODE = "public const string {ConstantName} = @\"{Path}\";";
    }
}