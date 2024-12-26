using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using UnityEngine;
using System.IO.Pipes;

namespace MFramework
{
    public static class MSerializationUtility
    {
        //TIP:
        //�����ļ�Ĭ��·����
        //��Ŀ¼���ļ������ļ���---��"��Ŀ��/XmlSettings/..."

        //=====Xml���л�����====
        public static UTF8Encoding UTF8 = new UTF8Encoding(false);

        public static bool SaveToXml<T>(string filePath, T instance)
        {
            string fullPath = GetFullPath(filePath, MSettings.DefaultXMLPath, "xml");

            bool flag = CheckOverwrite(fullPath, SaveMode.Overwrite);
            if (!flag) return false;

            //xmlWriter---XML����д����
            FileStream stream = File.Open(fullPath, FileMode.Create, FileAccess.Write);
            XmlTextWriter writer = new XmlTextWriter(stream, UTF8);
            writer.Formatting = Formatting.None;//����ģʽ

            //namesapces---��Ҫ�������أ�������ڸ��ڵ���������ܳ��������ռ�
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            XmlSerializer serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(writer, instance, ns);

            writer.Close();
            stream.Close();

            return true;
        }
        public static bool SaveToXml<T>(string filePath, T instance, bool isPrettyPrint = false, SaveMode mode = SaveMode.Overwrite)
        {
            string fullPath = GetFullPath(filePath, MSettings.DefaultXMLPath, "xml");

            bool flag = CheckOverwrite(fullPath, mode);
            if (!flag) return false;

            //xmlWriter---XML����д����
            FileStream stream = File.Open(fullPath, FileMode.Create, FileAccess.Write);
            XmlTextWriter writer = new XmlTextWriter(stream, UTF8);
            if (isPrettyPrint) writer.Formatting = Formatting.Indented;//�����ʽ(�ỻ��)
            else writer.Formatting = Formatting.None;//����ģʽ

            //namesapces---��Ҫ�������أ�������ڸ��ڵ���������ܳ��������ռ�
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            XmlSerializer serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(writer, instance, ns);

            writer.Close();
            stream.Close();

            return true;
        }

        public static object ReadFromXml(string filePath, Type type)
        {
            string fullPath = GetFullPath(filePath, MSettings.DefaultXMLPath, "xml");
            //����ļ��Ƿ����
            if (!File.Exists(fullPath))
            {
                MLog.Print($"{typeof(MSerializationUtility)}���ļ�{fullPath}�����ڣ�����", MLogType.Warning);
                return null;
            }

            string fileName = Path.GetFileNameWithoutExtension(filePath);
            FileStream stream = null;
            try
            {
                //xmlReader---XML���ݶ�ȡ��
                stream = File.OpenRead(fullPath);
                XmlReader reader = XmlReader.Create(stream);

                XmlSerializer serializer = new XmlSerializer(type);
                object instance = serializer.Deserialize(reader);

                stream.Close();
                return instance;
            }
            catch
            {
                MLog.Print($"{typeof(MSerializationUtility)}���ļ�{fullPath}���л�ʧ�ܣ�����", MLogType.Warning);
                if (stream != null) stream.Close();
                return null;
            }
        }
        public static T ReadFromXml<T>(string filePath)
        {
            string fullPath = GetFullPath(filePath, MSettings.DefaultXMLPath, "xml");
            //����ļ��Ƿ����
            if (!File.Exists(fullPath))
            {
                MLog.Print($"{typeof(MSerializationUtility)}���ļ�{fullPath}�����ڣ�����", MLogType.Warning);
                return default(T);
            }

            string fileName = Path.GetFileNameWithoutExtension(filePath);
            FileStream stream = null;
            try
            {
                //xmlReader---XML���ݶ�ȡ��
                stream = File.OpenRead(fullPath);
                XmlReader reader = XmlReader.Create(stream);

                XmlSerializer serializer = new XmlSerializer(typeof(T));
                T instance = (T)serializer.Deserialize(reader);

                stream.Close();
                return instance;
            }
            catch
            {
                MLog.Print($"{typeof(MSerializationUtility)}���ļ�{fullPath}���л�ʧ�ܣ�����", MLogType.Warning);
                if (stream != null) stream.Close();
                return default(T);
            }
        }



        //=====Json���л�����====
        public static bool SaveToJson<T>(string filePath, T instance)
        {
            string fullPath = GetFullPath(filePath, MSettings.DefaultJSONPath, "json");

            bool flag = CheckOverwrite(fullPath, SaveMode.Overwrite);
            if (!flag) return false;

            string text = JsonUtility.ToJson(instance, false);

            File.WriteAllText(fullPath, text);

            return true;
        }
        public static bool SaveToJson<T>(string filePath, T instance, bool isPrettyPrint = false, SaveMode mode = SaveMode.Overwrite)
        {
            string fullPath = GetFullPath(filePath, MSettings.DefaultJSONPath, "json");

            bool flag = CheckOverwrite(fullPath, mode);
            if (!flag) return false;

            string text = JsonUtility.ToJson(instance, isPrettyPrint);

            File.WriteAllText(fullPath, text);

            return true;
        }

        public static object ReadFromJson(string filePath, Type type)
        {
            string fullPath = GetFullPath(filePath, MSettings.DefaultJSONPath, "json");
            //����ļ��Ƿ����
            if (!File.Exists(fullPath))
            {
                MLog.Print($"{typeof(MSerializationUtility)}���ļ�{fullPath}�����ڣ�����", MLogType.Warning);
                return null;
            }

            string fileName = Path.GetFileNameWithoutExtension(filePath);
            using (StreamReader sr = new StreamReader(fullPath))
            {
                string text = sr.ReadToEnd();
                if (text.Length > 0)
                {
                    object result = JsonUtility.FromJson(text, type);
                    return result;
                }
                else
                {
                    MLog.Print($"{typeof(MSerializationUtility)}��{fileName}.json���������ݣ�����", MLogType.Warning);
                    return null;
                }
            }
        }
        public static T ReadFromJson<T>(string filePath)
        {
            string fullPath = GetFullPath(filePath, MSettings.DefaultJSONPath, "json");
            //����ļ��Ƿ����
            if (!File.Exists(fullPath))
            {
                MLog.Print($"{typeof(MSerializationUtility)}���ļ�{fullPath}�����ڣ�����", MLogType.Warning);
                return default(T);
            }

            string fileName = Path.GetFileNameWithoutExtension(filePath);
            
            using (StreamReader sr = new StreamReader(fullPath))
            {
                string text = sr.ReadToEnd();
                if (text.Length > 0)
                {
                    T result = JsonUtility.FromJson<T>(text);
                    return result;
                }
                else
                {
                    MLog.Print($"{typeof(MSerializationUtility)}��{fileName}.json���������ݣ�����", MLogType.Warning);
                    return default(T);
                }
            }
        }



        //=====���������л�����====
        //---�ļ���ϵ---
        public static bool SaveToByte(object instance, string filePath)
        {
            string fullPath = GetFullPath(filePath, MSettings.DefaultBYTEPath, "byte");

            bool flag = CheckOverwrite(fullPath, SaveMode.Overwrite);
            if (!flag) return false;

            using (FileStream fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(fileStream, instance);
            }

            return true;
        }
        public static bool SaveToByte(object instance, string filePath, SaveMode mode = SaveMode.Overwrite)
        {
            string fullPath = GetFullPath(filePath, MSettings.DefaultBYTEPath, "byte");

            bool flag = CheckOverwrite(fullPath, mode);
            if (!flag) return false;

            using (FileStream fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(fileStream, instance);
            }

            return true;
        }

        public static object ReadFromByte(string filePath)
        {
            string fullPath = GetFullPath(filePath, MSettings.DefaultBYTEPath, "byte");
            //����ļ��Ƿ����
            if (!File.Exists(fullPath))
            {
                MLog.Print($"{typeof(MSerializationUtility)}���ļ�{fullPath}�����ڣ�����.", MLogType.Warning);
                return null;
            }

            string fileName = Path.GetFileNameWithoutExtension(filePath);
            using (FileStream fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                object instance = binaryFormatter.Deserialize(fileStream);
                return instance;
            }
        }
        public static T ReadFromByte<T>(string filePath)
        {
            string fullPath = GetFullPath(filePath, MSettings.DefaultBYTEPath, "byte");
            //����ļ��Ƿ����
            if (!File.Exists(fullPath))
            {
                MLog.Print($"{typeof(MSerializationUtility)}���ļ�{fullPath}�����ڣ�����", MLogType.Warning);
                return default(T);
            }

            string fileName = Path.GetFileNameWithoutExtension(filePath);
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                T instance = (T)binaryFormatter.Deserialize(fileStream);
                return instance;
            }
        }

        //---�ڴ���ϵ---
        public static byte[] SaveToByte<T>(T instance)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, instance);
                return memoryStream.ToArray();
            }
        }
        public static object ReadFromByte(byte[] data)
        {
            using (MemoryStream memoryStream = new MemoryStream(data))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                return binaryFormatter.Deserialize(memoryStream);
            }
        }
        public static T ReadFromByte<T>(byte[] data)
        {
            using (MemoryStream memoryStream = new MemoryStream(data))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                return (T)binaryFormatter.Deserialize(memoryStream);
            }
        }


        /// <summary>
        /// ��ȡ����·����������ṩ����·����������Ŀ��Ŀ¼�´����ļ�
        /// </summary>
        private static string GetFullPath(string filePath, string defaultPath, string suffix)
        {
            string fileName = Path.GetFileName(filePath);
            //�ļ���Ҫô��".��׺"��ʽ��Ҫô�ǲ�����׺��ʽ
            if (!fileName.Contains($".{suffix}") && fileName.Contains('.'))
            {
                MLog.Print($"{typeof(MSerializationUtility)}���ļ���{fileName}������Ҫ������", MLogType.Warning);
                return null;
            }
            fileName = $"{Path.GetFileNameWithoutExtension(filePath)}.{suffix}";
            string directoryPath = filePath.CD();

            string fullPath = null;
            if (Path.IsPathRooted(filePath))//����·����ʽ
            {
                MPathUtility.CreateFolderIfNotExist(directoryPath);
                fullPath = $"{directoryPath}/{fileName}";
            }
            else//���·����ʽ
            {
                string fullDirectoryPath = $"{defaultPath}/{directoryPath}";
                MPathUtility.CreateFolderIfNotExist(fullDirectoryPath);
                fullPath = $"{fullDirectoryPath}/{fileName}";
            }

            return fullPath;
        }

        private static bool CheckOverwrite(string filePath, SaveMode mode)
        {
            if (mode == SaveMode.Overwrite)
            {
                MPathUtility.DeleteFileIfExist(filePath);
                return true;//����ɾ��ɾ����˵�����Խ�����һ��
            }
            else if (mode == SaveMode.DontOverwrite)
            {
                //���븲�ǣ����Ƿ����ļ��Ѵ���
                if (File.Exists(filePath))
                {
                    MLog.Print($"{typeof(MSerializationUtility)}��{filePath}�Ѵ��ڵ���ֹ���ǣ�����", MLogType.Warning);
                    return false;
                }
            }
            return false;
        }

        //---�ļ�ϵ---
        public static void SaveToFile(string filePath, string code)
        {
            string directoryPath = Path.GetDirectoryName(filePath);
            Directory.CreateDirectory(directoryPath);

            using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                using (TextWriter textWriter = new StreamWriter(fileStream, Encoding.UTF8))
                {
                    textWriter.Write(code);
                }
            }
        }

        public static string ReadFromFile(string filePath)
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (TextReader textReader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    return textReader.ReadToEnd();
                }
            }
        }
    }

    public enum SaveMode
    {
        Overwrite,
        DontOverwrite
    }
}