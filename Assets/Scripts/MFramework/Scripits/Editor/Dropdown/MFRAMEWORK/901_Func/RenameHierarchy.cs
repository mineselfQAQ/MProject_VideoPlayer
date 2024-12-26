using System.Linq;
using UnityEditor;
using UnityEngine;

public class RenameHierarchy : EditorWindow
{
    public string _NewName;
    public int _StartValue = 0;

    //����---������
    //�У�
    //NewName---���º������
    //StartValue---��׺��ʼֵ
    [MenuItem("MFramework/Rename Hierarchy GameObject", false, 902)]
    public static void Init()
    {
        RenameHierarchy window = GetWindow<RenameHierarchy>(true, "RenameTool");
        window.minSize = new Vector2(320, 150);
        window.maxSize = new Vector2(320, 1000);
        window.Show();
    }


    private void OnGUI()
    {
        //***����NewName��StartValue***
        _NewName = EditorGUILayout.TextField("NewName:", _NewName);
        _StartValue = EditorGUILayout.IntField("StartValue", _StartValue);



        //===========================================================
        EditorGUILayout.Space(20);
        //===========================================================



        //***��ʾ����ǰ����ĺ����ֵı仯***
        var selectObject = Selection.gameObjects.OrderBy(obj => obj.transform.GetSiblingIndex());

        bool hasObject = Selection.objects.Length > 0;
        if (hasObject)
        {
            EditorGUILayout.LabelField("���ĺ�:");
        }

        int i = 0;
        foreach (var obj in selectObject)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"{obj.name}--->{_NewName}_{_StartValue + i}");
            EditorGUILayout.EndHorizontal();
            i++;
        }



        //===========================================================
        EditorGUILayout.Space(20);
        //===========================================================



        //***ִ�в���***
        //ǿ����ǰ״̬��
        //��ɫ---����ִ��
        //��ɫ---����ִ�У���Ϊû��ѡ�����壬���Ե���ȥҲû��Ӧ
        GUI.enabled = hasObject;//����״̬����ɫ��ʱ���ǻҵ�---���ɵ��
        if (hasObject)
        {
            GUI.color = Color.green;
        }
        else
        {
            GUI.color = Color.red;
        }

        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Execute"))//һ��ִ��
            {
                i = 0;
                foreach (var obj in selectObject)
                {
                    obj.name = $"{_NewName}_{_StartValue + i}";
                    i++;
                }
            }
            if (GUILayout.Button("Execute(NoSuffix)"))//������׺ִ��
            {
                i = 0;
                foreach (var obj in selectObject)
                {
                    obj.name = $"{_NewName}";
                    i++;
                }
            }
            if (GUILayout.Button("Execute(OnlySuffix)"))//ֻ�����׺ִ�У��磺0 1 2 3
            {
                i = 0;
                foreach (var obj in selectObject)
                {
                    obj.name = $"{_StartValue + i}";
                    i++;
                }
            }
        }
        EditorGUILayout.EndHorizontal();
    }
}