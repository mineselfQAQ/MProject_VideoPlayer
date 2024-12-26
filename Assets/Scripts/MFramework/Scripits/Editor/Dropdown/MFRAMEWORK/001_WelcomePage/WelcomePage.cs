using MFramework;
using UnityEditor;
using UnityEngine;

public class WelcomePage : EditorWindow
{
    private Texture2D LOGOTex;

    [MenuItem("MFramework/WelcomePage", false, 1)]
    public static void Init()
    {
        WelcomePage window = GetWindow<WelcomePage>(true, "MFramework", false);
        window.minSize = new Vector2(425, 300);
        window.maxSize = new Vector2(425, 300);
        window.Show();
    }

    private void OnEnable()
    {
        LOGOTex = AssetDatabase.LoadAssetAtPath<Texture2D>(EditorResourcesPath.LOGOPath);
    }

    private void OnGUI()
    {
        GUILayout.Space(5);

        MEditorGUIUtility.DrawTexture(LOGOTex, MEditorGUIStyleUtility.CenterStyle);
        MEditorGUIUtility.DrawH1("��ӭ");

        EditorGUILayout.LabelField("��ӭʹ��MFramework��Ŀǰ�ÿ��ֻ�����Ǹ������ܵļ��ϣ������");
        EditorGUILayout.LabelField("�����²��֣�");
        EditorGUILayout.LabelField("1.MFramework---���Ŀ��");
        EditorGUILayout.LabelField("2.MFramework_Demo---ʹ�ÿ�ܱ�д��Demo");
        EditorGUILayout.LabelField("3.MFramework_Test---����Լ�ĳЩ֪ʶ��ļ򵥲���");
        EditorGUILayout.LabelField("");
        EditorGUILayout.LabelField("��л���ʹ��~~~");

        GUILayout.Space(8);
    }
}
