using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace MFramework
{
    public enum MBuildTarget
    {
        WINDOWS,
        ANDROID,
        IOS
    }

    public class ABGenerator : EditorWindow
    {
        [MenuItem("MFramework/BuildAB _F9", priority = 222)]
        public static async void Build()
        {
            bool flag = await ABBuilder.SwitchPlatform();
            if (!flag) return;

            ABBuilder.BuildInternal();
        }

        [MenuItem("MFramework/ABGenerator", priority = 221)]
        public static void Init()
        {
            ABGenerator window = GetWindow<ABGenerator>(true, "ABGenerator", false);
            window.minSize = new Vector2(200, 400);
            window.maxSize = new Vector2(200, 400);
            window.Show();
        }

        private void OnGUI()
        {
            //Tip:XML�ļ�·��---"��Ŀ��/XmlSettings/CORE/XMLBuildSetting.xml"

            MEditorGUIUtility.DrawH2("���׹���");
            DrawCheckXMLBtn();
            DrawCheckABBtn();

            EditorGUILayout.Space(30);

            MEditorGUIUtility.DrawH2("XML����");
            DrawXMLGenerator();

            EditorGUILayout.Space(10);

            MEditorGUIUtility.DrawH2("����AB��");
            DrawABGenerator();
        }

        private void DrawXMLGenerator()
        {
            if (GUILayout.Button("����Ĭ��XML"))
            {
                DrawDefaultGenerator();
                MLog.Print("�������");
                AssetDatabase.Refresh();
            }
            EditorGUILayout.LabelField("�Զ������ɣ�");
            if (GUILayout.Button("����"))
            {
                //TODO:�������ô��·��/��ʽ...
                //DrawCustomGenerator();
                MLog.Print("TODO", MLogType.Warning);
                AssetDatabase.Refresh();
            }
        }

        private void DrawABGenerator()
        {
            BuildTarget activeBuildTarget = EditorUserBuildSettings.activeBuildTarget;
            EditorGUILayout.LabelField($"��ǰƽ̨��{activeBuildTarget}");
            MBuildTarget buildTarget =
                activeBuildTarget == BuildTarget.StandaloneWindows64 ? MBuildTarget.WINDOWS :
                activeBuildTarget == BuildTarget.Android ? MBuildTarget.ANDROID : MBuildTarget.IOS;

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("�л�ƽ̨:", GUILayout.Width(70));

                MBuildTarget newBuildTarget = (MBuildTarget)EditorGUILayout.EnumPopup(buildTarget, GUILayout.Width(120));
                if (buildTarget != newBuildTarget)
                {
                    if (newBuildTarget == MBuildTarget.WINDOWS)
                        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
                    else if (newBuildTarget == MBuildTarget.ANDROID)
                        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
                    else if (newBuildTarget == MBuildTarget.IOS)
                        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);

                    return;
                }

                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("����"))
            {
                ABBuilder.BuildInternal();
                AssetDatabase.Refresh();
            }
        }

        private void DrawCheckXMLBtn()
        {
            if (GUILayout.Button("�鿴XML"))
            {
                string fileName = MSettings.ABBuildSettingName;
                if (!File.Exists(fileName))
                {
                    MLog.Print("ABBuildSetting.xmlδ���������ȴ������ٲ鿴", MLogType.Warning);
                    return;
                }

                MEditorUtility.OpenFile(fileName);
            }
        }
        private void DrawCheckABBtn()
        {
            if (GUILayout.Button("�鿴AB��"))
            {
                string settingPath = MSettings.ABBuildSettingName;
                if (!File.Exists(settingPath))
                {
                    MLog.Print("���ȴ���ABBuildSetting.xml������AB�����ٲ鿴", MLogType.Warning);
                    return;
                }

                var buildSetting = MSerializationUtility.ReadFromXml<BuildSetting>(settingPath);
                if (buildSetting == null)
                {
                    MLog.Print($"ABBuildSetting.xml��ȡʧ�ܣ�����", MLogType.Warning);
                    return;
                }

                string resPath = buildSetting.buildRoot;
                resPath = Path.GetFullPath(resPath).ReplaceSlash();
                //���⴦��{ProjectName}
                resPath = resPath.Replace("{ProjectName}", Application.productName);
                if (!Directory.Exists(resPath))
                {
                    MLog.Print($"����ABBuildSetting.xml�е�BuildRoot��õ�·��<{resPath}>����ȷ������");
                    return;
                }
                MEditorUtility.OpenFolder(resPath);
            }
        }

        private void DrawDefaultGenerator()
        {
            string defaultSavePath = Application.dataPath.CD();

            string abPath = $"{defaultSavePath}/Assets/AssetBundle";
            CreateABDirectoryIfNotExist(abPath);

            string projectPath = Application.dataPath;
            projectPath = projectPath.Substring(0, projectPath.Length - "Assets".Length);//"Assets"֮ǰ��·��
            abPath = abPath.Replace(projectPath, "");//��"Assets"��ͷ��abPath
            CreateDefaultXML(abPath);
        }
        private void CreateABDirectoryIfNotExist(string rootPath)
        {
            Directory.CreateDirectory(rootPath);

            Directory.CreateDirectory($"{rootPath}/Common");
            Directory.CreateDirectory($"{rootPath}/Atlas");
            Directory.CreateDirectory($"{rootPath}/Background");
            Directory.CreateDirectory($"{rootPath}/Icon");
            Directory.CreateDirectory($"{rootPath}/Model");
            Directory.CreateDirectory($"{rootPath}/Shader");
            Directory.CreateDirectory($"{rootPath}/UI");
        }
        private void CreateDefaultXML(string abPath)
        {
            string filePath = MSettings.ABBuildSettingName;

            string code = ABXMLCODE;

            string productName = Application.productName;
            code = code.Replace("{PROJECTNAME}", productName);
            code = code.Replace("{BUILDROOT}", $"../{productName}_AssetBundle");

            string buildItemsCode = GenerateBuildItemsCode(abPath);
            code = code.Replace("{BUILDITEM}", buildItemsCode);

            MSerializationUtility.SaveToFile(filePath, code);
            EditorUtility.RevealInFinder(filePath);
        }
        private string GenerateBuildItemsCode(string abPath)
        {
            StringBuilder res = new StringBuilder();

            AppendBuildItem(res, $"{abPath}/Common/",     "Direct",  "File",       ".renderTexture");
            AppendBuildItem(res, $"{abPath}/Atlas/",      "Direct",  "Directory",  ".png|.spriteatlas");
            AppendBuildItem(res, $"{abPath}/Background/", "Direct",  "File",       ".png");
            AppendBuildItem(res, $"{abPath}/Icon/",       "Direct",  "Directory",  ".png");
            AppendBuildItem(res, $"{abPath}/Model/",      "Direct",  "Directory",  ".prefab");
            AppendBuildItem(res, $"{abPath}/Shader/",     "Direct",  "Directory",  ".shader");
            AppendBuildItem(res, $"{abPath}/UI/",         "Direct",  "File",       ".prefab", false);

            return res.ToString();
        }
        private void AppendBuildItem(StringBuilder sb, string assetPath, string resourceType, string bundleType, string suffix, bool newLine = true)
        {
            string buildItemCode = BUILDITEMCODE;
            buildItemCode = buildItemCode.Replace("{ASSETPATH}", assetPath);
            buildItemCode = buildItemCode.Replace("{RESOURCETYPE}", resourceType);
            buildItemCode = buildItemCode.Replace("{BUNDLETYPE}", bundleType);
            buildItemCode = buildItemCode.Replace("{SUFFIX}", suffix);
            if (newLine) buildItemCode += "\n\t";

            sb.Append(buildItemCode);
        }

        private const string ABXMLCODE =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<BuildSetting ProjectName=""{PROJECTNAME}"" BuildRoot=""{BUILDROOT}"">
    {BUILDITEM}
</BuildSetting>";

        private const string BUILDITEMCODE =
@"<BuildItem AssetPath=""{ASSETPATH}"" ResourceType=""{RESOURCETYPE}"" BundleType=""{BUNDLETYPE}"" Suffix=""{SUFFIX}"" />";
    }
}