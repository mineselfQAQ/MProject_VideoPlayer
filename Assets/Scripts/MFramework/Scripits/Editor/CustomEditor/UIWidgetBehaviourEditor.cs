using UnityEditor;
using UnityEngine;

namespace MFramework
{
    [CustomEditor(typeof(UIWidgetBehaviour))]
    public class UIWidgetBehaviourEditor : UIViewBehaviourEditor
    {
        private UIWidgetBehaviour behaviour;

        private SerializedProperty widgetModeSP;
        private SerializedProperty animSwitchSP;
        private SerializedProperty openAnimModeSP;
        private SerializedProperty closeAnimModeSP;

        private static GUIContent widgetModeLabel = new GUIContent("Widget Mode", "���ģʽ");
        private static GUIContent animationSwitchLabel = new GUIContent("Animation Switch", "��������");
        private static GUIContent openAnimationModeLabel = new GUIContent("Open Animation Mode", "��������ģʽ");
        private static GUIContent closeAnimationModeLabel = new GUIContent("Close Animation Mode", "�رն���ģʽ");
        
        protected void OnEnable()
        {
            behaviour = (UIWidgetBehaviour)target;

            widgetModeSP = serializedObject.FindProperty("widgetMode");
            animSwitchSP = serializedObject.FindProperty("animSwitch");
            openAnimModeSP = serializedObject.FindProperty("openAnimMode");
            closeAnimModeSP = serializedObject.FindProperty("closeAnimMode");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawUIPanelSettings();
            DrawCompCollections();
            DrawExportBtn();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawUIPanelSettings()
        {
            var widgetMode = MEditorControlUtility.DrawPopup<UIWidgetMode>(widgetModeSP, widgetModeLabel);
            if (widgetMode == UIWidgetMode.Simple) return;//�������߼�ģʽ
            else//�߼�ģʽ
            {
                EditorGUI.indentLevel++;
                {
                    int lastAnimSwitch = animSwitchSP.enumValueIndex;

                    var animSwitch = MEditorControlUtility.DrawPopup<UIAnimSwitch>(animSwitchSP, animationSwitchLabel);
                    if (animSwitch ==  UIAnimSwitch.Off) return;//����������
                    else//��������
                    {
                        //���û��Animator��������Զ����������������
                        Animator checkAnimator = behaviour.GetComponent<Animator>();
                        if (checkAnimator == null)
                        {
                            bool state = EditorUtility.DisplayDialog("��ʾ",
                            "���ö���������ҪAnimator��֧�֣��Ƿ���Ҫ����", "��Animator", "�����ö���");
                            if (!state)//ȡ��
                            {
                                animSwitchSP.enumValueIndex = 0;
                                return;
                            }

                            Animator animator = behaviour.gameObject.AddComponent<Animator>();
                            //������ڸ��ӣ������Ҽ�Prefab�����ļ���������ȽϺ���
                            //if (state == 2)//ȫ������
                            //{
                            //    string prefabPath = UIPanelUtility.GetPrefabPath(target);
                            //    string objName = Path.GetFileNameWithoutExtension(prefabPath);
                            //    string directoryPath = Path.GetDirectoryName(prefabPath);
                            //    string acPath = Path.Combine(directoryPath, $"{objName}.controller");
                            //    string openClipPath = Path.Combine(directoryPath, $"{objName}_Open.anim");
                            //    string closeClipPath = Path.Combine(directoryPath, $"{objName}_Close.anim");

                            //    //ע�⣺���ڻ���ֵڶ�����������ᵼ��������⣬��Ҫ�ӳ�ִ��
                            //    EditorDelayExecute.Instance.DelayDo(DelayNoRecord());

                            //    IEnumerator DelayNoRecord()
                            //    {
                            //        yield return null;

                            //        var controller = CreateUIAnimatorController.CreateAnimatorController(objName, acPath, openClipPath, closeClipPath);
                            //        animator.runtimeAnimatorController = controller;
                            //    }
                            //}
                            MLog.Print("�Ѵ���Animator���뼰ʱ����AnimatorController����ʹ��MCreate--->UIAnimatorController�Զ�����", MLogType.Warning);
                            return;
                        }
                        if (checkAnimator.runtimeAnimatorController == null)
                        {
                            //��Off�л���On����һ֡
                            if (lastAnimSwitch == 0 && animSwitchSP.enumValueIndex == 1)
                            {
                                MLog.Print("��Prefab�ϵ�Animator��û��Controller���޷�ʹ�ö���ϵͳ�������й���", MLogType.Warning);
                            }
                        }

                        EditorGUI.indentLevel++;
                        {
                            MEditorControlUtility.DrawPopup<UIOpenAnimMode>(openAnimModeSP, openAnimationModeLabel);
                            MEditorControlUtility.DrawPopup<UICloseAnimMode>(closeAnimModeSP, closeAnimationModeLabel);
                        }
                        EditorGUI.indentLevel--;
                    }
                }
                EditorGUI.indentLevel--;
            }
        }
    }
}