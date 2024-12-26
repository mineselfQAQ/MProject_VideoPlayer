using Excel;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace MFramework
{
    //TODO��ֻ֧��PC���޷����ڰ�׿��Ios
    public class ExcelGenerator : EditorWindow
    {
        [MenuItem("MFramework/GenerateAllExcel _F8", priority = 202)]
        public static void GenerateAllExcel()
        {
            EditorPrefs.SetBool(EditorPrefsData.ExcelBINGenerationState, true);
            ExcelGenerator window = CreateInstance<ExcelGenerator>();
            window.XLSX2PersistentData();
        }

        [MenuItem("MFramework/ExcelGenerator", priority = 201)]
        public static void Init()
        {
            ExcelGenerator window = GetWindow<ExcelGenerator>(true, "ExcelGenerator", false);
            window.minSize = new Vector2(300, 220);
            window.maxSize = new Vector2(300, 220);
            window.Show();
        }

        private void OnGUI()
        {
            //����Excel�༭��
            MEditorGUIUtility.DrawH1("Excel�༭��");

            DrawGenerateExcelPart();//����Excel����

            EditorGUILayout.LabelField("------------------------------------------", MEditorGUIStyleUtility.BoldStyle);

            DrawGeneratePersistentDataPart();//���ɳ־û����ݲ���

            EditorGUILayout.Space(20);

            DrawCheckPathBtn();
        }

        #region ��������
        //public static void CreateCSBIN(string excelName, string CSName, string BINName, string BINLoadPath)
        //{
        //    CreateSingleBIN(excelName, BINName);

        //    AssetDatabase.Refresh();
        //    EditorUtility.RequestScriptReload();
        //}
        public static bool CreateSingleCS(string excelName, string CSName, string BINLoadPath)
        {
            string fileName = Path.GetFileNameWithoutExtension(excelName);
            string CSPath = CSName;
            string BINPath = BINLoadPath;

            FileStream stream = File.Open(excelName, FileMode.Open, FileAccess.Read);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            DataSet dataset = excelReader.AsDataSet();

            if (!CheckTable(dataset))
            {
                MLog.Print($"{fileName}��������⣬����.", MLogType.Error);
                return false;
            }
            GetDataFromTable(dataset.Tables[0],
                out string[] names, out string[] types, out object[][] data);

            bool isSucceed = CreateCS(CSPath, BINPath, names, types, "MFramework");
            return isSucceed;
        }
        public static bool CreateSingleBIN(string excelName, string BINName)
        {
            string fileName = Path.GetFileNameWithoutExtension(excelName);

            //��ȡdataset
            FileStream stream = File.Open(excelName, FileMode.Open, FileAccess.Read);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            DataSet dataset = excelReader.AsDataSet();

            if (!CheckTable(dataset))
            {
                MLog.Print($"{fileName}��������⣬����.", MLogType.Error);
                return false;
            }
            GetDataFromTable(dataset.Tables[0],
                out string[] names, out string[] types, out object[][] data);

            bool isSucceed = CreateBIN(BINName, fileName, data, "MFramework.Runtime.dll");
            return isSucceed;
        }
        #endregion

        #region Excel����
        private void DrawGenerateExcelPart()
        {
            MEditorGUIUtility.DrawH2("����Excel�ļ�");

            if (GUILayout.Button("����"))
            {
                CreateExcelFile();
            }
        }

        private void CreateExcelFile()
        {
            int state = EditorUtility.DisplayDialogComplex("Generating",
                    $"ȷ���ļ���������{MConfigurableSettings.ExcelPath}����", "ȷ��", "ȡ��", "����·��");
            if (state == 0)//ȷ��
            {
                string path = EditorUtility.SaveFilePanel("����", MConfigurableSettings.ExcelPath, "Sheet", "xlsx");
                path = path.ReplaceSlash(false);//Process.Start()ֻ����\����/

                if (path == "")
                {
                    MLog.Print("��ȡ������Excel�ļ�.", MLogType.Warning);
                    return;
                }

                FileInfo file = new FileInfo(path);
                string fileName = Path.GetFileName(path);
                //����ļ��Ѿ����ڣ���������
                bool isExist = false;
                if (file.Exists)
                {
                    isExist = true;

                    file.Delete();
                    file = new FileInfo(path);
                }

                //����Excel�ļ�
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet1");//������1
                    worksheet.Cells["A1"].LoadFromDataTable(GetDefaultTable(), true);//������ʼ������

                    worksheet.Cells["A1:C6"].AutoFitColumns();//�����п�
                    worksheet.Cells["A1:C6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;//����
                    worksheet.Cells["A1:C3"].Style.Font.Bold = true;//�Ӵ�

                    package.Save();
                }

                System.Diagnostics.Process.Start($"Explorer.exe", $@"/select,{path}");
                if (isExist) MLog.Print($"����������{fileName}.", MLogType.Warning);
                else MLog.Print($"������{fileName}.");
            }
            else if (state == 1)//ȡ��
            {
                MLog.Print("��ȡ������Excel�ļ�.", MLogType.Warning);
            }
            else//����·��
            {
                string pathName = MConfigurableSettingsBase.GetPathName(MConfigurableName.ExcelGenerationPath);
                bool flag = EditorSettingsConfigurator.ChangePath(pathName);
                if (flag) MLog.Print($"�Ѹ���{pathName}·��.");

                AssetDatabase.Refresh();
            }
        }

        private DataTable GetDefaultTable()
        {
            DataTable table = new DataTable();

            //�����ȴ��У����������
            table.Columns.Add("���");
            table.Columns.Add("����");
            table.Columns.Add("����");
            //table.Rows.Add(new object[] { "���", "����", "����" });
            table.Rows.Add(new object[] { "ID", "NAME", "DESC" });
            table.Rows.Add(new object[] { "int", "string", "string[]" });
            table.Rows.Add(new object[] { 1, "ƻ��", "��ɫ#��" });
            table.Rows.Add(new object[] { 2, "�㽶", "��ɫ#��" });
            table.Rows.Add(new object[] { 3, "����", "��ɫ#��" });

            return table;
        }
        #endregion

        #region CS/BIN����
        private void DrawGeneratePersistentDataPart()
        {
            MEditorGUIUtility.DrawH2("���ɳ־û�����");

            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("1.����.cs�ļ�"))
                {
                    string BINFolder = MSettings.ExcelBINPath;
                    string CSFolder = MConfigurableSettings.ExcelCSPath;
                    List<string> fileList = MPathUtility.GetFiles(MConfigurableSettings.ExcelPath, null, ".xlsx");//��ȡ�����ļ���

                    bool haveFile = CheckAllFolder(BINFolder, CSFolder, fileList, CreateMode.CS);
                    if (!haveFile) return;

                    CreateAllCS(BINFolder, CSFolder, fileList);
                    AssetDatabase.Refresh();
                }
                if (GUILayout.Button("2.����.byte�ļ�"))
                {
                    string BINFolder = MSettings.ExcelBINPath;
                    string CSFolder = MConfigurableSettings.ExcelCSPath;
                    List<string> fileList = MPathUtility.GetFiles(MConfigurableSettings.ExcelPath, null, ".xlsx");//��ȡ�����ļ���

                    bool haveFile = CheckAllFolder(BINFolder, CSFolder, fileList, CreateMode.BIN);
                    if (!haveFile) return;

                    CreateAllBIN(BINFolder, fileList);
                    AssetDatabase.Refresh();
                }
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("һ������(F8)"))
            {
                EditorPrefs.SetBool(EditorPrefsData.ExcelBINGenerationState, true);
                XLSX2PersistentData();
            }
        }

        /// <summary>
        /// ���.cs���ɺ����̽���.byte����
        /// </summary>
        private void XLSX2PersistentData()
        {
            string BINFolder = MSettings.ExcelBINPath;//Ĭ��.byte�ļ����λ��---StreamingAssets�ļ����ڲ�
            string CSFolder = MConfigurableSettings.ExcelCSPath;
            List<string> fileList = MPathUtility.GetFiles(MConfigurableSettings.ExcelPath, null, ".xlsx");//��ȡ�����ļ���

            bool haveFile = CheckAllFolder(BINFolder, CSFolder, fileList, CreateMode.CSAndBIN);
            if (!haveFile) return;

            CreateAllCS(BINFolder, CSFolder, fileList);

            //�˴���Ҫ���������¼��أ���ɺ��������BIN�ļ�  Tip:������Refresh()��Reload()�ſ���
            AssetDatabase.Refresh();
            EditorUtility.RequestScriptReload();
            //"�ӳ�ִ��"����ExcelDelayDo��---InitializeAfterAssemblyReload()
            //CreateAllBIN(BINFolder, fileList);
        }

        private bool CreateAllCS(string BINFolder, string CSFolder, List<string> fileList)
        {
            bool flag = true;
            //���������ļ��������ű�
            foreach (string path in fileList)
            {
                string fileName = Path.GetFileNameWithoutExtension(path);
                string CSPath = $"{CSFolder}/{fileName}.cs".ReplaceSlash();
                string BINPath = $"{BINFolder}/{fileName}.byte".ReplaceSlash();

                //��ȡdataset
                FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read);
                IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                DataSet dataset = excelReader.AsDataSet();

                if (!CheckTable(dataset))
                {
                    MLog.Print($"{fileName}��������⣬����.", MLogType.Error);
                    continue;
                }
                GetDataFromTable(dataset.Tables[0],
                    out string[] names, out string[] types, out object[][] data);

                bool isSucceed = CreateCS(CSPath, BINPath, names, types);
                if (!isSucceed) flag = false;
            }
            if (flag)
            {
                //TODO:��֪��Ϊʲô�ڼ��д���.cs�ļ��е�Log�������
                MLog.Print("����������CS�ļ�");
                return true;
            }
            else
            {
                MLog.Print("��CS�ļ�δ�ɹ����ɣ�����", MLogType.Warning);
                return false;
            }
        }
        private static bool CreateCS(string CSPath, string BINPath, string[] names, string[] types, string nameSpace = "")
        {
            string code;
            bool haveNamespace;
            if (nameSpace == "")
            {
                code = CSBASECODE;
                haveNamespace = false;
            }
            else
            {
                code = CSBASECODEWITHNAMESPACE;
                haveNamespace = true;
            }

            code = code.Replace("{NameSpace}", nameSpace);

            string className = Path.GetFileNameWithoutExtension(CSPath);
            string collectionClassName = $"{className}s";

            code = code.Replace("{ClassName}", className);
            code = code.Replace("{CollectionClassName}", collectionClassName);

            string propertiesDefine = GeneratePropertiesDefine(names, types, haveNamespace);
            string constructorDefine = GenerateConstructorDefine(className, names, types, haveNamespace);

            code = code.Replace("{PropertiesDefine}", propertiesDefine);
            code = code.Replace("{ConstructorDefine}", constructorDefine);

            code = code.Replace("{BINPath}", BINPath);

            //д���ļ�
            MSerializationUtility.SaveToFile(CSPath, code);

            return true;
        }

        public static bool CreateAllBIN(string BINFolder, List<string> fileList)
        {
            bool flag = true;
            //���������ļ��������������ļ�
            foreach (string path in fileList)
            {
                string fileName = Path.GetFileNameWithoutExtension(path);
                string BINPath = $"{BINFolder}/{fileName}.byte".ReplaceSlash();

                //��ȡdataset
                FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read);
                IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                DataSet dataset = excelReader.AsDataSet();

                if (!CheckTable(dataset))
                {
                    MLog.Print($"{fileName}��������⣬����.", MLogType.Error);
                    continue;
                }
                GetDataFromTable(dataset.Tables[0],
                    out string[] names, out string[] types, out object[][] data);

                bool isSucceed = CreateBIN(BINPath, fileName, data);
                if (!isSucceed) flag = false;
            }
            if (flag)
            {
                MLog.Print("����������BIN�ļ�");
                return true;
            }
            else
            {
                MLog.Print("��BIN�ļ�δ�ɹ����ɣ�����", MLogType.Warning);
                return false;
            }
        }
        private static bool CreateBIN(string BINPath, string className, object[][] data, string dllName = "Assembly-CSharp.dll")
        {
            //ע�⣡����
            //���dll������ԭ����Assembly-CSharp�ĳ��򼯣������Լ�������dll�ĳ���
            string CSAssemblyPath = $"{Application.dataPath}/../Library/ScriptAssemblies/{dllName}";
            Assembly assembly = Assembly.LoadFile(CSAssemblyPath);

            int rowLength = data.Length;
            int colLength = data[0].Length;
            Array instances = null;
            object resInstance = null;

            Type[] types = assembly.GetTypes();
            foreach (var type in types)//1.��������Сʵ��
            {
                if (type.Name == className)
                {
                    instances = Array.CreateInstance(type, rowLength);

                    var ctors = type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
                    var ctor = ctors[0];//ֻ��˽�й��캯��

                    for (int i = 0; i < rowLength; i++)
                    {
                        object[] parameters = new object[colLength];
                        for (int j = 0; j < colLength; j++)
                        {
                            parameters[j] = data[i][j];
                        }
                        object instance = ctor.Invoke(parameters);
                        instance = Convert.ChangeType(instance, type);
                        instances.SetValue(instance, i);
                    }
                }
            }
            foreach (var type in types)//2.ͨ��Сʵ����ɴ�ʵ��
            {
                if (type.Name == $"{className}s")
                {
                    var ctors = type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
                    var ctor = ctors[0];//ֻ��˽�й��캯��

                    object[] parameter = new object[1] { instances };
                    resInstance = ctor.Invoke(parameter);
                }
            }

            bool flag = MSerializationUtility.SaveToByte(resInstance, BINPath);
            if (!flag) MLog.Print($"{typeof(ExcelGenerator)}�����л�ʧ��", MLogType.Error);

            return true;
        }

        private static string GeneratePropertiesDefine(string[] names, string[] types, bool haveNamespace)
        {
            StringBuilder res = new StringBuilder();

            int n = names.Length;
            for (int i = 0; i < n; i++)
            {
                string name = names[i].ToUpper();

                string tempLine = PROPERTIESBASECODE;
                tempLine = tempLine.Replace("{Type}", types[i]);
                tempLine = tempLine.Replace("{Name}", name);

                if (i != n - 1)
                {
                    if(haveNamespace) res.Append(tempLine + "\n\t\t");
                    else res.Append(tempLine + "\n\t");
                }
                else res.Append(tempLine);
            }

            return res.ToString();
        }
        private static string GenerateConstructorDefine(string className, string[] names, string[] types, bool haveNamespace)
        {
            string constructorBaseCode = haveNamespace ? CONSTRUCTORBASECODEWITHNAMESPACE : CONSTRUCTORBASECODE;
            constructorBaseCode = constructorBaseCode.Replace("{ClassName}", className);
            constructorBaseCode = constructorBaseCode.Replace("{Parameter}", GetParameter(names, types));
            constructorBaseCode = constructorBaseCode.Replace("{AssignmentOperator}", GetAssignmentOperator(names));
            return constructorBaseCode;

            string GetParameter(string[] names, string[] types)
            {
                StringBuilder sb = new StringBuilder();
                int n = names.Length;
                for (int i = 0; i < n - 1; i++)
                {
                    sb.Append($"{types[i]} {names[i].ToLower()}, ");
                }
                sb.Append($"{types[n - 1]} {names[n - 1].ToLower()}");

                return sb.ToString();
            }
            string GetAssignmentOperator(string[] names)
            {
                StringBuilder sb = new StringBuilder();
                int n = names.Length;
                for (int i = 0; i < n - 1; i++)
                {
                    if(haveNamespace) sb.Append($"{names[i].ToUpper()} = {names[i].ToLower()};\n\t\t\t");
                    else sb.Append($"{names[i].ToUpper()} = {names[i].ToLower()};\n\t\t");
                }
                sb.Append($"{names[n - 1].ToUpper()} = {names[n - 1].ToLower()};");

                return sb.ToString();
            }
        }

        public static void GetDataFromTable(DataTable sheet, out string[] names, out string[] types, out object[][] data)
        {
            //���������
            //int actualRowCount = 0;
            //foreach (DataRow row in sheet.Rows)
            //{
            //    if (!row.IsNull(0))
            //    {
            //        actualRowCount++;
            //    }
            //}

            int rowCount = sheet.Rows.Count;
            int colCount = sheet.Columns.Count;

            //����ʵ������(ȥ������Ϊnone����)
            int typeCount = 0;
            for (int i = 0; i < colCount; i++)
            {
                if (sheet.Rows[2][i].ToString() == "none") continue;
                typeCount++;
            }

            //��ʼ������
            names = new string[typeCount];
            types = new string[typeCount];
            data = new object[rowCount - 3][];
            for (int i = 0; i < data.Length; i++) data[i] = new object[typeCount];
            //��ʼ������
            //names/types
            for (int i = 0, col = 0; i < typeCount; i++, col++)
            {
                while (sheet.Rows[2][col].ToString() == "none") col++;

                names[i] = sheet.Rows[1][col].ToString();
                types[i] = sheet.Rows[2][col].ToString();
            }
            //data
            for (int i = 0, col = 0; i < typeCount; i++, col++)
            {
                while (sheet.Rows[2][col].ToString() == "none") col++;
                string colType = sheet.Rows[2][col].ToString();

                for (int row = 0; row < rowCount - 3; row++)
                {
                    if (colType == "byte")
                    {
                        data[row][i] = Convert.ToByte(sheet.Rows[3 + row][col]);
                    }
                    else if (colType == "byte[]")
                    {
                        string originStr = sheet.Rows[3 + row][col].ToString();
                        string[] splitStrs = originStr.Split("#");
                        int n = splitStrs.Length;

                        byte[] resBytes = new byte[n];
                        for (int j = 0; j < n; j++)
                        {
                            resBytes[j] = Convert.ToByte(splitStrs[j]);
                        }

                        data[row][i] = resBytes;
                    }
                    else if (colType == "short")
                    {
                        data[row][i] = Convert.ToInt16(sheet.Rows[3 + row][col]);
                    }
                    else if (colType == "short[]")
                    {
                        string originStr = sheet.Rows[3 + row][col].ToString();
                        string[] splitStrs = originStr.Split("#");
                        int n = splitStrs.Length;

                        short[] resShorts = new short[n];
                        for (int j = 0; j < n; j++)
                        {
                            resShorts[j] = Convert.ToInt16(splitStrs[j]);
                        }

                        data[row][i] = resShorts;
                    }
                    else if (colType == "int")
                    {
                        data[row][i] = Convert.ToInt32(sheet.Rows[3 + row][col]);
                    }
                    else if (colType == "int[]")
                    {
                        string originStr = sheet.Rows[3 + row][col].ToString();
                        string[] splitStrs = originStr.Split("#");
                        int n = splitStrs.Length;

                        int[] resInts = new int[n];
                        for (int j = 0; j < n; j++)
                        {
                            resInts[j] = Convert.ToInt32(splitStrs[j]);
                        }

                        data[row][i] = resInts;
                    }
                    else if (colType == "long")
                    {
                        data[row][i] = Convert.ToUInt64(sheet.Rows[3 + row][col]);
                    }
                    else if (colType == "long[]")
                    {
                        string originStr = sheet.Rows[3 + row][col].ToString();
                        string[] splitStrs = originStr.Split("#");
                        int n = splitStrs.Length;

                        long[] resLongs = new long[n];
                        for (int j = 0; j < n; j++)
                        {
                            resLongs[j] = Convert.ToInt64(splitStrs[j]);
                        }

                        data[row][i] = resLongs;
                    }
                    else if (colType == "float")
                    {
                        data[row][i] = Convert.ToSingle(sheet.Rows[3 + row][col]);
                    }
                    else if (colType == "float[]")
                    {
                        string originStr = sheet.Rows[3 + row][col].ToString();
                        string[] splitStrs = originStr.Split("#");
                        int n = splitStrs.Length;

                        float[] resFloats = new float[n];
                        for (int j = 0; j < n; j++)
                        {
                            resFloats[j] = Convert.ToSingle(splitStrs[j]);
                        }

                        data[row][i] = resFloats;
                    }
                    else if (colType == "double")
                    {
                        data[row][i] = Convert.ToDouble(sheet.Rows[3 + row][col]);
                    }
                    else if (colType == "double[]")
                    {
                        string originStr = sheet.Rows[3 + row][col].ToString();
                        string[] splitStrs = originStr.Split("#");
                        int n = splitStrs.Length;

                        double[] resDoubles = new double[n];
                        for (int j = 0; j < n; j++)
                        {
                            resDoubles[j] = Convert.ToDouble(splitStrs[j]);
                        }

                        data[row][i] = resDoubles;
                    }
                    else if (colType == "bool")
                    {
                        data[row][i] = Convert.ToBoolean(sheet.Rows[3 + row][col]);
                    }
                    else if (colType == "char")
                    {
                        data[row][i] = Convert.ToChar(sheet.Rows[3 + row][col]);
                    }
                    else if (colType == "char[]")
                    {
                        string originStr = sheet.Rows[3 + row][col].ToString();
                        string[] splitStrs = originStr.Split("#");
                        int n = splitStrs.Length;

                        char[] resChars = new char[n];
                        for (int j = 0; j < n; j++)
                        {
                            resChars[j] = Convert.ToChar(splitStrs[j]);
                        }

                        data[row][i] = resChars;
                    }
                    else if (colType == "string")
                    {
                        data[row][i] = sheet.Rows[3 + row][col].ToString();
                    }
                    else if (colType == "string[]")
                    {
                        string originStr = sheet.Rows[3 + row][col].ToString();
                        string[] resStr = originStr.Split("#");
                        data[row][i] = resStr;
                    }
                    else if (colType == "none")
                    {
                        break;
                    }
                    else
                    {
                        MLog.Print("���ݱ��д���δ֪���ͣ�����.", MLogType.Warning);
                    }
                }
            }
        }

        private bool CheckAllFolder(string BINFolder, string CSFolder, List<string> fileList, CreateMode mode)
        {
            //����ļ����Ƿ���ڣ���������ھʹ���
            MPathUtility.CreateFolderIfNotExist(BINFolder);
            MPathUtility.CreateFolderIfNotExist(CSFolder);
            //ȷ���Ƿ��Ѿ����ļ��У�������ھ�ȫ����������
            if (mode == CreateMode.CS)
            {
                MPathUtility.RecreateDirectoryIfFolderNotEmpty(CSFolder);
                MPathUtility.RecreateDirectoryIfFolderNotEmpty(BINFolder);
            }
            else if (mode == CreateMode.BIN)
            {
                MPathUtility.RecreateDirectoryIfFolderNotEmpty(BINFolder);
            }
            else if (mode == CreateMode.CSAndBIN)
            {
                MPathUtility.RecreateDirectoryIfFolderNotEmpty(CSFolder);
                MPathUtility.RecreateDirectoryIfFolderNotEmpty(BINFolder);
            }

            if (fileList.Count == 0)
            {
                MLog.Print($"{MConfigurableSettings.ExcelPath}�з���0��Excel�ļ�������·���Ƿ���ȷ.", MLogType.Warning);
                return false;
            }
            return true;
        }
        public static bool CheckTable(DataSet dataSet)
        {
            if (dataSet.Tables.Count < 1)//�Ƿ���ڱ�
            {
                return false;
            }
            DataTable sheet = dataSet.Tables[0];//ȡ�ױ�
            //�ж����ݱ����Ƿ��������
            if (sheet.Rows.Count < 1)
            {
                return false;
            }
            return true;
        }
        #endregion

        #region ��·�����ð�ť
        private void DrawCheckPathBtn()
        {
            if (GUILayout.Button("���·��������"))
            {
                EditorSettingsConfigurator.Init();
            }
        }
        #endregion


        #region Ԥ���ʼ����
        private const string CSBASECODEWITHNAMESPACE =
      @"using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace {NameSpace}
{
    [Serializable]
    public class {ClassName}
    {
        {PropertiesDefine}

        {ConstructorDefine}

        public static {ClassName}[] LoadBytes()
        {
            string path = $""{BINPath}"";
            if (!File.Exists(path)) return null;

            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                {CollectionClassName} table = binaryFormatter.Deserialize(stream) as {CollectionClassName};
                {ClassName}[] res = table.items;
                return res;
            }
        }
    }

    [Serializable]
    internal class {CollectionClassName}
    {
        public {ClassName}[] items;

        private {CollectionClassName}({ClassName}[] items)
        {
            this.items = items;
        }
    }
}";
        private const string CSBASECODE =
      @"using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class {ClassName}
{
    {PropertiesDefine}

    {ConstructorDefine}

    public static {ClassName}[] LoadBytes()
    {
        string path = $""{BINPath}"";
        if (!File.Exists(path)) return null;

        using (FileStream stream = new FileStream(path, FileMode.Open))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            {CollectionClassName} table = binaryFormatter.Deserialize(stream) as {CollectionClassName};
            {ClassName}[] res = table.items;
            return res;
        }
    }
}

[Serializable]
internal class {CollectionClassName}
{
    public {ClassName}[] items;

    private {CollectionClassName}({ClassName}[] items)
    {
        this.items = items;
    }
}";
        private const string FIELDBASECODE = "private {Type} {Name};";
        private const string PROPERTIESBASECODE = "public {Type} {Name} { get; private set; }";
        private const string CONSTRUCTORBASECODEWITHNAMESPACE = 
@"private {ClassName}({Parameter})
        {
            {AssignmentOperator}
        }"; 
        private const string CONSTRUCTORBASECODE =
@"private {ClassName}({Parameter})
    {
        {AssignmentOperator}
    }";
        #endregion
    }

    public enum CreateMode
    {
        CS,
        BIN,
        CSAndBIN
    }
}
