using UnityEditor;
using UnityEngine;

namespace MFramework
{
    /// <summary>
    /// UICompCollection�Ļ��Ƹ���
    /// </summary>
    [CustomPropertyDrawer(typeof(UICompCollection), true)]
    public class UICompCollectionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty targetSP = property.FindPropertyRelative("target");
            SerializedProperty compListSP = property.FindPropertyRelative("compList");

            //�����ɾ�������
            //�����������ģ�
            //�����Ͽ��ܹ������1/2/3/4��ͬʱ�б�ѡ�������1/2/3/4����ʱ���3��ɾ���ˣ�
            //��ʱ�᣺������������Ϊnull������ʵ����ռ��λ�ã�Ӧ��ɾ������
            if (RemoveNulls(compListSP))
            {
                //ˢ��һ��
                compListSP.serializedObject.ApplyModifiedPropertiesWithoutUndo();
                return;
            }

            EditorGUI.BeginProperty(position, label, property);
            {
                //ע�⣺
                //��EditorGUI.PrefixLabel()���������Rect��
                //Ҫע��������᷵��Rect�����������һ��GUI���Ƶ�λ��

                //����(ǰ׺)
                //id---ͨ��GetControlID()��ȡ���ؼ�ΪFocusType:
                //FocusType.Keyboard---����ѡ�У���Tab������ת
                //FocusType.Passive---����ѡ�У���Tab�������ò���
                position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
                //����Ӧ�Ŀ�ȣ�������⣬target:1/3, select:2/3��
                float itemSpacing = 5;
                float targetWidth = (position.width - itemSpacing) / 3;
                float selectWidth = targetWidth * 2;
                //target��compList��Rect
                Rect targetRect = new Rect(position.x, position.y, targetWidth, position.height);
                Rect selectRect = new Rect(position.x + targetWidth + itemSpacing, position.y, selectWidth, position.height);

                int oldIndent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;//��������
                {
                    //����GameObject
                    EditorGUI.BeginChangeCheck();
                    {
                        GUI.Box(targetRect, GUIContent.none);//���Ƶ׿�(�ᱻѹס�����壬���Բ�����)
                        //GameObject��
                        EditorGUI.PropertyField(targetRect, targetSP, GUIContent.none);
                        //��GameObject�����ı�ʱ
                        if (EditorGUI.EndChangeCheck())
                        {
                            //componentListSP����ʹ�ã�ͬʱ�ı�targetSP
                            compListSP.ClearArray();
                            targetSP.serializedObject.ApplyModifiedProperties();
                            compListSP.serializedObject.ApplyModifiedProperties();
                        }
                    }

                    //�Ҳ�������б�
                    bool isExsistTarget = targetSP.objectReferenceValue != null;
                    //������targetSP�ͽ���componentListSP�ؼ�(���������Ƿ���ö�����Ⱦ�ؼ�)
                    using (new EditorGUI.DisabledScope(!isExsistTarget))
                    {
                        if (isExsistTarget)
                        {
                            GameObject target = targetSP.objectReferenceValue as GameObject;//��ȡ����
                            Component[] existComps = target.GetComponents<Component>();//��ȡ�����ϵ����

                            //��ť(�����·�)
                            GUIContent guiContent = new GUIContent();
                            guiContent.text = (compListSP.arraySize == 0) ? "Default" : string.Empty;//��ť�ı�
                            if (GUI.Button(selectRect, guiContent, EditorStyles.popup))
                            {
                                //���������б�չ��
                                BuildPopupList(existComps, compListSP).DropDown(selectRect);
                            }

                            //ͼ���б�(�������Ϸ�)
                            int posIndex = 0;
                            for (int i = 0; i < existComps.Length; i++)//���������ܹ���֤�����Inspector�ϵ�˳��
                            {
                                Component comp = existComps[i];

                                int savedIndex = GetIndexFromSavedComponentList(compListSP, comp);//ֻ��ʾ�ѱ�������
                                if (savedIndex >= 0)
                                {
                                    Texture icon = MEditorUtility.GetIcon(comp.GetType());
                                    DrawIcon(selectRect, posIndex, icon);
                                    posIndex++;
                                }
                            }
                        }
                        else
                        {
                            //������targetSPʱ�İ�ť
                            if (GUI.Button(selectRect, "Default", EditorStyles.popup)) { };
                        }
                    }
                }
                EditorGUI.indentLevel = oldIndent;//������ԭ
            }
            EditorGUI.EndProperty();
        }

        private GenericMenu BuildPopupList(Component[] existComps, SerializedProperty componentListSP)
        {
            GenericMenu menu = new GenericMenu();

            foreach (Component comp in existComps)
            {
                //��ȡindex
                int savedIndex = GetIndexFromSavedComponentList(componentListSP, comp);

                //���ÿһ��comp
                //on---trueʱΪ��ѡ״̬��falseʱΪ�ǹ�ѡ״̬
                menu.AddItem(new GUIContent(comp.GetType().Name), savedIndex >= 0, (source) =>
                {
                    //���Itemʱ����Item�Ѵ���ִ���Ƴ�������ִ�����
                    //��ʵ���Ƕ�ѡ�б��ѡ����ȡ��
                    if (savedIndex >= 0)
                    {
                        //ע�⣬ɾ��Ԫ��ǰ�����Ƚ�����Ϊnull��
                        //����ֱ�ӵ���DeleteArrayElementAtIndex��ʹԪ�ر�Ϊnull�����Ǵ��б����Ƴ�
                        componentListSP.GetArrayElementAtIndex(savedIndex).objectReferenceValue = null;
                        componentListSP.DeleteArrayElementAtIndex(savedIndex);
                    }
                    else
                    {
                        componentListSP.InsertArrayElementAtIndex(componentListSP.arraySize);
                        componentListSP.GetArrayElementAtIndex(componentListSP.arraySize - 1).objectReferenceValue = source as Component;
                    }
                    componentListSP.serializedObject.ApplyModifiedProperties();
                    EditorApplication.RepaintHierarchyWindow();//��ҪǿˢHierarchy
                }, comp);
            }

            return menu;
        }
        
        /// <summary>
        /// �Ƴ�������
        /// </summary>
        private bool RemoveNulls(SerializedProperty componentListSP)
        {
            bool hasNull = false;
            for (int i = componentListSP.arraySize - 1; i >= 0; i--)
            {
                if (componentListSP.GetArrayElementAtIndex(i).objectReferenceValue == null)
                {
                    componentListSP.DeleteArrayElementAtIndex(i);
                    hasNull = true;
                }
            }
            return hasNull;
        }

        //������ѱ�������б��е�λ��
        private int GetIndexFromSavedComponentList(SerializedProperty componentListSP, Component comp)
        {
            //compListSP---UIOpElement�����
            //comp---GameObject�����
            int index = -1;
            for (int i = 0; i < componentListSP.arraySize; i++)
            {
                Component savedComp = componentListSP.GetArrayElementAtIndex(i).objectReferenceValue as Component;
                if (savedComp.Equals(comp))
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        private void DrawIcon(Rect selectRect, int posIndex, Texture icon)
        {
            float iconWidth = EditorGUIUtility.singleLineHeight * 0.8f;
            float iconHeight = iconWidth;
            float leftPadding = 5;
            float iconSpacing = 5;

            float iconX = selectRect.x + leftPadding + (iconWidth + iconSpacing) * posIndex;//ˮƽ����
            float iconY = selectRect.y + (selectRect.height - iconHeight) / 2;//��ֱ����

            Rect iconRect = new Rect(iconX, iconY, iconWidth, iconHeight);

            GUI.DrawTexture(iconRect, icon);
        }
    }
}