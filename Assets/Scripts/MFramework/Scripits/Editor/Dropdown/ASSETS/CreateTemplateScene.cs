using System.IO;
using UnityEditor;
using UnityEditor.SceneTemplate;
using UnityEngine;

namespace MFramework
{
    public class CreateTemplateScene
    {
        [MenuItem("Assets/MCreate/TemplateScene/UITemplateScene", priority = 1, secondaryPriority = 1.0f)]
        public static void CreateUITemplateScene()
        {
            if (CheckAvailability())
            {
                SceneTemplateAsset sceneTemplate = AssetDatabase.LoadAssetAtPath<SceneTemplateAsset>(EditorResourcesPath.SceneFilePath);

                if (sceneTemplate == null)
                {
                    MLog.Print($"·��{EditorResourcesPath.SceneFilePath}δ�ҵ�UI����ģ�壬�޷�����", MLogType.Warning);
                    return;
                }

                string path = GetFilePath("UITemplateScene.unity");
                if (File.Exists(path))
                {
                    bool flag = EditorUtility.DisplayDialog("����", 
                        $"{path}���Ѵ���ͬ���������Ƿ���Ҫ����", "����", "ȡ��");
                    if (!flag) return;
                }
                SceneTemplateService.Instantiate(sceneTemplate, false, path);
            }
        }

        private static string GetFilePath(string fileName)
        {
            Object obj = Selection.objects[0];
            string path = AssetDatabase.GetAssetPath(obj);
            string directoryPath;
            if (File.Exists(path))//ѡ������ļ�
            {
                directoryPath = Path.GetDirectoryName(path);
            }
            else//ѡ������ļ���
            {
                directoryPath = path;
            }

            return $"{directoryPath}/{fileName}";
        }

        private static bool CheckAvailability()
        {
            var objs = Selection.objects;

            if (objs.Length != 1)
            {
                MLog.Print($"��ѡ��һ������", MLogType.Warning);
                return false;
            }

            return true;
        }
    }
}