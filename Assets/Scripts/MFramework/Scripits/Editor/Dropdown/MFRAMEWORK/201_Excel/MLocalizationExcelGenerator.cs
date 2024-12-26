using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using MFramework.UI;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace MFramework
{
    public class MLocalizationExcelGenerator : EditorWindow
    {
        private bool FirstOpen = true;
        private List<LocalizationTableInfo> infos;

        private int pos;
        private Vector2 scrollPos1;

        private Scene curScene;

        [MenuItem("MFramework/MLocalizationExcelGenerator", priority = 203)]
        public static void Init()
        {
            EditorWindow window = GetWindow<MLocalizationExcelGenerator>(true, "MLocalizationExcelGenerator", false);
            window.minSize = new Vector2(400, 400);
            window.Show();
        }

        private void OnGUI()
        {
            MEditorGUIUtility.DrawH1("���ػ�������");

            DrawCheckBtn();
            DrawCreateBtn();
            DrawCSBtn();
            DrawBINBtn();

            MEditorGUIUtility.DrawH2("��ѯ���������е�MLocalization");
            EditorGUILayout.LabelField("Tip:Prefab���ֵĸ���Ϊ�ֲ����ģ�����ȫ�ָ������Ԥ���������޸�", MEditorGUIStyleUtility.ColorStyle(Color.red));

            EditorGUILayout.BeginHorizontal();
            {
                DrawResetSortBtn();
                //DrawAutoSortBtn();//TODO
            }
            EditorGUILayout.EndHorizontal();

            DrawJumpToPosBtn();
            DrawMLocalizationChecker();
        }

        private void OnDestroy()
        {
            FirstOpen = true;
        }

        private void DrawCheckBtn()
        {
            if (GUILayout.Button("�鿴Excel�ļ�"))
            {
                string fileName = MSettings.LocalizationTableName;
                if (!File.Exists(fileName)) 
                {
                    MLog.Print($"{typeof(MLocalizationExcelGenerator)}�����ȴ���Excel��鿴");
                    return;
                }

                MLog.Print($"��ַ��<{fileName}>");
                System.Diagnostics.Process.Start(fileName);
            }
        }
        private void DrawCreateBtn()
        {
            if (GUILayout.Button("����Excel�ļ�"))
            {
                string fileName = MSettings.LocalizationTableName;
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));

                FileInfo file = new FileInfo(fileName);
                if (file.Exists)
                {
                    int flag = EditorUtility.DisplayDialogComplex("Generating",
                        "���ػ�Excel�ļ��Ѵ��ڣ��Ƿ���Ҫ���к��ֲ���", "����", "ȡ��", "����");
                    if (flag == 1) return;//ȡ��
                    else if (flag == 2)//����
                    {
                        //TODO:δ���
                        MLog.Print("TODO", MLogType.Warning);
                        return;
                    }
                    else if (flag == 0)//����---ɾ���ش�
                    {
                        file.Delete();
                        file = new FileInfo(fileName);
                    }
                }

                //Ѱ������MLocalization�ű�
                List<LocalizationTableInfo> infos = GetMLocalizationTabelInfo(true, false);
                Scene curScene = EditorSceneManager.GetActiveScene();

                //����Excel�ļ�
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet1");//������1
                    worksheet.Cells["A1"].LoadFromDataTable(GetLocalizationTable(GetValidInfos(infos)), true);//������ʼ������

                    int row = infos.Count + 3;
                    worksheet.Cells[$"A1:G{row}"].AutoFitColumns();//�����п�
                    worksheet.Cells[$"A1:G{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;//����
                    worksheet.Cells["A1:G3"].Style.Font.Bold = true;//�Ӵ�

                    package.Save();
                }

                System.Diagnostics.Process.Start(fileName);
                EditorSceneManager.OpenScene(curScene.path, OpenSceneMode.Single);

                return;
            }
        }
        private void DrawCSBtn()
        {
            if (GUILayout.Button("����CS"))
            {
                bool flag = ExcelGenerator.CreateSingleCS(MSettings.LocalizationTableName, MSettings.LocalizationCSName, MSettings.LocalizationLoadBINName);
                if (!flag) return;

                MLog.Print("�������");
                AssetDatabase.Refresh();
            }
        }
        private void DrawBINBtn()
        {
            if (GUILayout.Button("����BIN"))
            {
                bool flag = ExcelGenerator.CreateSingleBIN(MSettings.LocalizationTableName, MSettings.LocalizationBYTEName);
                if (!flag) return;

                MLog.Print("�������");
                AssetDatabase.Refresh();
            }
        }
        private void DrawResetSortBtn()
        {
            if (GUILayout.Button("��������"))
            {
                infos = GetMLocalizationTabelInfo(true);
                GUI.FocusControl(null);//ȡ���۽�
                return;
            }
        }
        private void DrawJumpToPosBtn()
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("��ת��ID:", GUILayout.Width(60));
                pos = EditorGUILayout.IntField(pos, GUILayout.Width(50));
                EditorGUILayout.LabelField("��", GUILayout.Width(50));
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("��ת", GUILayout.Width(100)))
                {
                    GUI.FocusControl(null);//ȡ���۽�
                    int index = FindPosIndex(pos);
                    int y = index * 50;
                    scrollPos1.y = y;
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        private int FindPosIndex(int pos)
        {
            for(int i = 0; i < infos.Count; i++)
            {
                if (infos[i].id == pos)
                {
                    return i;
                }
            }
            MLog.Print($"{typeof(MLocalizationExcelGenerator)}��δ�ҵ�idΪ{pos}�����壬����", MLogType.Warning);
            return -1;
        }
        private void DrawAutoSortBtn()
        {
            if (GUILayout.Button("һ������"))
            {
                //TODO:�ռ����г����Լ�����Prefab�е�MLocalization�����г�ʼ������
            }
        }

        private List<LocalizationTableInfo> GetValidInfos(List<LocalizationTableInfo> infos)
        {
            //��ȡ���о���ID������(�ų�LocalizationMode.Off�����LocalID=-1���)
            List<LocalizationTableInfo> validList = new List<LocalizationTableInfo>();
            foreach (var info in infos)
            {
                if (info.mLocal.LocalMode == LocalizationMode.Off || info.mLocal.LocalID == -1)
                {
                    continue;
                }
                validList.Add(info);
            }

            //�ų��ظ�ID
            Dictionary<int, LocalizationTableInfo> resDic = new Dictionary<int, LocalizationTableInfo>();
            foreach (var info in validList)
            {
                if (!resDic.ContainsKey(info.id))
                {
                    resDic.Add(info.id, info);
                }
                else
                {
                    //��Ǹ�id���ж������
                    //resDic[info.id].go = null;//���Ϊnull
                    resDic[info.id].isMulti = true;
                    //�����ڶೡ��
                    if (!resDic[info.id].sceneNames.Contains(info.sceneName))
                    {
                        resDic[info.id].sceneNames.Add(info.sceneName);
                    }
                    //�����ڶ�Prefab
                    if (!resDic[info.id].prefabNames.Contains(info.prefabParent.name))
                    {
                        resDic[info.id].prefabNames.Add(info.prefabParent.name);
                    }
                }
            }

            return new List<LocalizationTableInfo>(resDic.Values);
        }
        private DataTable GetLocalizationTable(List<LocalizationTableInfo> infos)
        {
            DataTable table = new DataTable();

            //�����ȴ��У����������
            table.Columns.Add("���");
            table.Columns.Add("������");
            table.Columns.Add("Ԥ������");
            table.Columns.Add("������");
            table.Columns.Add("����");
            table.Columns.Add("����");
            table.Columns.Add("Ӣ��");
            table.Rows.Add(new object[] { "ID", "SceneName", "PrefabName","GOName", "Desc", "Chinese", "English" });
            table.Rows.Add(new object[] { "int", "none", "none", "none", "none", "string", "string" });
            foreach (var info in infos)
            {
                //���г�����
                string sceneStr = "";
                for (int i = 0; i < info.sceneNames.Count - 1; i++)
                {
                    sceneStr += $"{info.sceneNames[i]}|";
                }
                sceneStr += info.sceneNames[info.sceneNames.Count - 1];
                //����Ԥ������
                string prefabStr = "";
                for (int i = 0; i < info.prefabNames.Count - 1; i++)
                {
                    prefabStr += $"{info.prefabNames[i]}|";
                }
                prefabStr += info.prefabNames[info.prefabNames.Count - 1];
                //������(��ע�Ƿ���ж������)
                string name = info.isMulti ? $"{info.go.name}(Multi)" : info.go.name;

                table.Rows.Add(new object[] { info.id, sceneStr, prefabStr, name, info.text, "", "" });
            }

            return table;
        }

        private void DrawMLocalizationChecker()
        {
            //�������ڼ��ֿ��ܣ�
            //1.���������---�������嶼��ǰ����Hierarchy�У���ô��ʱȫ����ȡ���ļ���
            //2.Ԥ���������������Ԥ���壬���Ա�Ȼ�ڳ�����û�����壬��ô��ʱ�޷�����
            //2.1.�����з���---Ԥ�����Ȼ��������ʱ����������Ԥ�����ʱ�Ѿ��������ɣ��޷���������
            //2.2.���з���---��Ԥ����������һ�����볡������в��������Ǵ�ʱ��Ҫ����|����ļ���״̬|Ԥ����ı���|
            //2.3.���з���---�����Ԥ���壬ֱ�ӽ���Ԥ����������

            List<MLocalization> mLocalList = MLocalizationUtility.FindAllLoclizations();

            //����Ҫ��
            //1.�״ν��� 2.infos��ʧ 3.�����л�
            if (FirstOpen || infos == null || curScene != EditorSceneManager.GetActiveScene())
            {
                infos = GetMLocalizationTabelInfo(true);
                curScene = EditorSceneManager.GetActiveScene();
                FirstOpen = false;
            }

            scrollPos1 = EditorGUILayout.BeginScrollView(scrollPos1);
            {
                foreach (var info in infos)
                {
                    EditorGUILayout.BeginVertical();
                    {
                        EditorGUILayout.LabelField($"���֣�{info.text}");

                        EditorGUILayout.BeginHorizontal();
                        {
                            GUI.enabled = false;
                            EditorGUILayout.ObjectField(info.go, typeof(GameObject), true, GUILayout.Width(175));
                            GUI.enabled = true;

                            if (info.prefabParent != null)
                            {
                                if (GUILayout.Button("��ת"))
                                {
                                    Selection.activeGameObject = info.prefabParent;
                                }
                                if (GUILayout.Button("����Prefab"))
                                {
                                    string prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(info.prefabParent);
                                    PrefabStageUtility.OpenPrefab(prefabPath);

                                    var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
                                    GameObject prefabRoot = prefabStage.prefabContentsRoot;
                                    GameObject targetObject = prefabRoot.FindChildByName(info.go.name);
                                    if (targetObject != null)
                                    {
                                        Selection.activeGameObject = targetObject;
                                    }
                                }
                            }

                            GUILayout.FlexibleSpace();
                            if (info.id == -1)
                            {
                                if (GUILayout.Button("�Ƴ�MLocalization"))
                                {
                                    info.go.GetComponent<MLocalization>().LocalMode = LocalizationMode.Off;
                                    //infos.Remove(info);

                                    infos = GetMLocalizationTabelInfo(true);
                                    GUI.FocusControl(null);//ȡ���۽�
                                }
                            }

                            PrefabInstanceStatus status = PrefabUtility.GetPrefabInstanceStatus(info.go);
                            int oldID = info.id;
                            int newID = -1;
                            newID = EditorGUILayout.IntField(oldID, GUILayout.Width(50));

                            if (newID != oldID)
                            {
                                info.id = newID;
                                info.mLocal.LocalID = newID;
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.Space(10);
                }
            }
            EditorGUILayout.EndScrollView();

            SaveModify();
        }

        private void SaveModify()
        {

        }

        /// <summary>
        /// ��ȡ����ɸѡ���MLocalization��Ϣ
        /// </summary>
        private List<LocalizationTableInfo> GetMLocalizationTabelInfo(bool isSort, bool onlyCurScene = true)
        {
            List<LocalizationTableInfo> infos = new List<LocalizationTableInfo>();

            List<MLocalization> mLocalList = new List<MLocalization>();
            List<string> sceneNameList = new List<string>();
            if (onlyCurScene)//ֻ���ǵ�ǰ����
            {
                mLocalList = MLocalizationUtility.FindAllLoclizations(); 
                for (int i = 0; i < mLocalList.Count; i++) sceneNameList.Add("");
            }
            else//ѡ��BuildingSettings�п��������г���
            {
                EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
                foreach (var scene in scenes)
                {
                    if (scene.enabled)
                    {
                        Scene loadedScene = EditorSceneManager.OpenScene(scene.path, OpenSceneMode.Additive);
                        string sceneName = loadedScene.name;
                        GameObject[] rootObjects = loadedScene.GetRootGameObjects();
                        foreach (GameObject rootObject in rootObjects)
                        {
                            var list = MLocalizationUtility.FindLoclizations(rootObject);
                            mLocalList.AddRange(list);
                            for(int i = 0; i < list.Count; i++) sceneNameList.Add(sceneName);
                        }
                    }
                }
            }

            int j = 0;
            foreach (var mLocal in mLocalList)
            {
                if (mLocal.LocalMode == LocalizationMode.Off) continue;//localIDΪ-1Ҳ��Ҫ���У���Ϊ�����ǻ�û��

                GameObject parent = GetPrefabParent(mLocal);
                LocalizationTableInfo info = new LocalizationTableInfo(mLocal, mLocal.LocalID, mLocal.gameObject, mLocal.GetComponent<MText>().text, parent, sceneNameList[j], new List<string>() { sceneNameList[j] }, new List<string>() { parent.name });
                infos.Add(info);

                j++;
            }

            if(isSort) infos = infos.OrderBy(info => info.id).ToList();//����

            return infos;
        }
        private GameObject GetPrefabParent(MLocalization mLocal)
        {
            GameObject go = mLocal.gameObject;

            GameObject root = PrefabUtility.GetNearestPrefabInstanceRoot(go);
            if (root != null) return root;

            return null;
        }
        //private GameObject GetPrefabParent(MLocalization mLocal)
        //{
        //    GameObject go = mLocal.gameObject;

        //    GameObject curRoot = PrefabUtility.GetNearestPrefabInstanceRoot(go);
        //    while (curRoot != null)
        //    {
        //        Transform rootParent = curRoot.transform.parent;
        //        GameObject parentRoot = null;
        //        if (rootParent != null)
        //        {
        //            parentRoot = PrefabUtility.GetNearestPrefabInstanceRoot(rootParent.gameObject);
        //        }

        //        if (parentRoot == null) return curRoot;
        //        curRoot = parentRoot;
        //    }

        //    return null;
        //}

        private class LocalizationTableInfo
        {
            public MLocalization mLocal;
            public int id;
            public GameObject go;
            public string text;
            public GameObject prefabParent;
            public string sceneName;

            //Excel�����������
            public bool isMulti;//�Ƿ���ڶ������
            public List<string> sceneNames;//���ڳ���
            public List<string> prefabNames;//����Prefab

            public LocalizationTableInfo(MLocalization mLocal, int id, GameObject go, string text, GameObject prefabParent, string sceneName, List<string> sceneNames = null, List<string> prefabNames = null)
            {
                this.mLocal = mLocal;
                this.id = id;
                this.go = go;
                this.text = text;
                this.prefabParent = prefabParent;
                this.sceneName = sceneName;

                this.sceneNames = sceneNames;
                this.prefabNames = prefabNames;

                isMulti = false;//Ĭ��Ϊfalse
            }
        }
    }
}
