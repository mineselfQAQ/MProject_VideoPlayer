using System;
using UnityEditor;
using UnityEngine;

namespace MFramework
{
    public class MDialog : EditorWindow
    {
        private string[] btnsName;

        private string message;

        private bool isClickCustomBtn = false; 
        
        private Action<int> onBtnClick;

        public static void ShowDialog(string title, string message, Action<int> onButtonClick, params string[] names)
        {
            MDialog window = ScriptableObject.CreateInstance<MDialog>();
            window.minSize = new Vector2(300, 50);
            window.maxSize = new Vector2(300, 50);
            window.ShowUtility();
            window.titleContent = new GUIContent(title);
            window.btnsName = names;
            window.message = message;
            window.onBtnClick = onButtonClick;
            window.Show();
        }

        private void OnDestroy()
        {
            //�߼���������Զ��尴ť�����ڹر�ǰ��ǰ��Ϊtrue���رպ���Ϊfalse����ôֻ�����Ϻ��ᴥ��
            if (!isClickCustomBtn)//�����津���ر�
            {
                onBtnClick(-1);
                MLog.Print("��ȡ��");
            }
        }

        private void OnGUI()
        {
            GUILayout.Label(message);

            EditorGUILayout.BeginHorizontal();
            {
                for (int i = 0; i < btnsName.Length; i++)
                {
                    if (GUILayout.Button(btnsName[i]))
                    {
                        isClickCustomBtn = true;
                        onBtnClick(i);
                        this.Close();
                        isClickCustomBtn = false;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}