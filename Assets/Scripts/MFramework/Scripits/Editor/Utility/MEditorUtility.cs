using System;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace MFramework
{
    internal static class MEditorUtility
    {
        //��ʾ���ڲ��ȴ�����ѡ������
        internal static int state;
        internal static async Task<int> DisplayDialogAsync(string title, string message, params string[] names)
        {
            state = -100;
            await ShowDialogAsync(title, message, names);
            return state;
        }
        private static async Task ShowDialogAsync(string title, string message, params string[] names)
        {
            MDialog.ShowDialog(title, message, (i) => { state = i; }, names);

            await Task.Run(() =>
            {
                while (state == -100)
                {
                    continue;
                }
            });
        }

        internal static Texture GetIcon(Type type)
        {
            Texture systemIcon = EditorGUIUtility.ObjectContent(null, type).image;
            Texture customIcon = null;
            Texture csScriptIcon = EditorGUIUtility.IconContent("cs Script Icon").image;

            if (type == typeof(TMPro.TMP_InputField))
            {
                customIcon = (Texture2D)EditorGUIUtility.Load("Packages/com.unity.textmeshpro/Editor Resources/Gizmos/TMP - Input Field Icon.psd");
            }
            else if (type == typeof(TMPro.TMP_Dropdown))
            {
                customIcon = (Texture2D)EditorGUIUtility.Load("Packages/com.unity.textmeshpro/Editor Resources/Gizmos/TMP - Dropdown Icon.psd");
            }
            else if (type == typeof(TMPro.TextMeshProUGUI))
            {
                customIcon = (Texture2D)EditorGUIUtility.Load("Packages/com.unity.textmeshpro/Editor Resources/Gizmos/TMP - Text Component Icon.psd");
            }

            return systemIcon ?? customIcon ?? csScriptIcon;
        }

        internal static string ChangePath()
        {
            string guideFolder = Path.GetDirectoryName(Application.dataPath);
            string newPath = EditorUtility.OpenFolderPanel("��ѡ��·��", guideFolder, "");

            if (newPath == "")
            {
                return null;
            }
            return newPath;
        }

        internal static string DisplayGenerateFileDialog(string oldPath, string fileName = "")
        {
            int state = EditorUtility.DisplayDialogComplex("Generating",
                    $"ȷ���ļ���������{oldPath}����", "ȷ��", "ȡ��", "����·��");

            if (state == 0)//ȷ��
            {
                return oldPath;
            }
            else if (state == 1)//ȡ��
            {
                MLog.Print($"��ȡ������{fileName}�ļ�.", MLogType.Warning);
                return null;
            }
            else//����·��
            {
                string newPath = ChangePath();
                newPath = Path.Combine(newPath, fileName);
                return newPath;
            }
        }

        /// <summary>
        /// ��·���ļ���
        /// </summary>
        internal static void OpenFolder(string path)
        {
            if (!MPathUtility.IsFolder(path))
            {
                MLog.Print("·�������ڻ�Ϊ�ļ�������", MLogType.Warning);
                return;
            }

            path = path.Replace("/", "\\");//תΪWindows֧����ʽ
            System.Diagnostics.Process.Start("explorer", path);
            MLog.Print($"·������{path}");
        }
        /// <summary>
        /// ѡ��·���ļ���(ͬEditorUtility.RevealInFinder())
        /// </summary>
        internal static void SelectFolder(string path)
        {
            if (!MPathUtility.IsFolder(path))
            {
                MLog.Print("·�������ڻ�Ϊ�ļ�������", MLogType.Warning);
                return;
            }

            path = path.Replace("/", "\\");//תΪWindows֧����ʽ
            System.Diagnostics.Process.Start("explorer", "/select,\"" + path + "\"");
            MLog.Print($"·������{path}");
        }
        internal static void OpenFile(string path)
        {
            if (!MPathUtility.IsFile(path))
            {
                MLog.Print("·�������ڻ�Ϊ�ļ��У�����", MLogType.Warning);
                return;
            }

            path = path.Replace("/", "\\");//תΪWindows֧����ʽ
            System.Diagnostics.Process.Start("explorer", path);
            MLog.Print($"·������{path}");
        }
        internal static void SelectFile(string path)
        {
            if (!MPathUtility.IsFile(path))
            {
                MLog.Print("·�������ڻ�Ϊ�ļ��У�����", MLogType.Warning);
                return;
            }

            path = path.Replace("/", "\\");//תΪWindows֧����ʽ
            System.Diagnostics.Process.Start("explorer", "/select,\"" + path + "\"");
            MLog.Print($"·������{path}");
        }
    }
}