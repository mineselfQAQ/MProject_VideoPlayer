using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MFramework
{
    public static class MPathUtility
    {
        public static bool IsFolder(string path)
        {
            return Directory.Exists(path);
        }
        public static bool IsFile(string path)
        {
            return File.Exists(path);
        }

        /// <summary>
        /// ��ȡ�ļ����µ����кϷ��ļ�
        /// </summary>
        public static List<string> GetFiles(string folder, string prefix, params string[] suffixes)
        {
            string[] files = Directory.GetFiles(folder, $"*.*", SearchOption.AllDirectories);
            List<string> result = new List<string>(files.Length);

            for (int i = 0; i < files.Length; ++i)
            {
                string file = files[i].ReplaceSlash();

                if (prefix != null && !file.StartsWith(prefix, StringComparison.InvariantCulture))
                {
                    continue;
                }

                if (suffixes != null && suffixes.Length > 0)
                {
                    bool exist = false;

                    for (int j = 0; j < suffixes.Length; j++)
                    {
                        string suffix = suffixes[j];
                        if (file.EndsWith(suffix, StringComparison.InvariantCulture))
                        {
                            exist = true;
                            break;
                        }
                    }

                    if (!exist) continue;
                }

                result.Add(file);
            }

            return result;
        }

        /// <summary>
        /// ��֤�ļ��еĴ���
        /// </summary>
        public static bool CreateFolderIfNotExist(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                return true;
            }
            return false;
        }

        /// <summary>
        /// ����ļ��Ѵ��ڣ���ɾ���ļ�
        /// </summary>
        public static bool DeleteFileIfExist(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return true;
            }
            return false;
        }

        /// <summary>
        /// ��֤�ļ��е�����״̬(���´���)
        /// </summary>
        /// <param playerName="path"></param>
        /// <returns>���´���ʱΪtrue������Ϊfalse</returns>
        public static bool RecreateDirectoryIfFolderNotEmpty(string path)
        {
            if (Directory.GetFiles(path).Length != 0)
            {
                DeleteFolderFilesSurface(path);
                Directory.CreateDirectory(path);
                return true;
            }
            return false;
        }

        /// <summary>
        /// ɾ���ļ����������ļ�(ֻ����һ�㣬�����ļ���)
        /// </summary>
        public static void DeleteFolderFilesSurface(string folder)
        {
            string[] files = Directory.GetFiles(folder);
            foreach (string file in files)
            {
                File.Delete(file);
            }
            Directory.Delete(folder);
        }

        /// <summary>
        /// ͨ��"Assets\·��"��ȡ����·��
        /// </summary>
        public static string GetFullPathBaseProjectRoot(string secondPath)
        {
            string fullPath = Path.GetFullPath(secondPath);
            return fullPath;
        }

        /// <summary>
        /// ��ȡ·���µ��ļ�����ȷ��������ȷ��׺
        /// </summary>
        /// <returns></returns>
        public static string GetFileNameWithExtension(string path, string suffix)
        {
            string name = Path.GetFileNameWithoutExtension(path);
            if (suffix[0] == '.')
            {
                name += suffix;
            }
            else
            {
                name += $".{suffix}";
            }

            return name;
        }
    }
}