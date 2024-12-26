using UnityEditor;
using UnityEngine;

namespace MFramework
{
    [CustomEditor(typeof(MAudioSource))]
    public class MAudioSourceEditor : Editor
    {
        private SerializedProperty OnStartSP;
        private SerializedProperty modeSP;
        private SerializedProperty audioClipSP;
        private SerializedProperty audioMixerGroupSP;
        private SerializedProperty muteSP;
        private SerializedProperty playOnAwakeSP;
        private SerializedProperty loopSP;
        private SerializedProperty fadeInOutSP;
        private SerializedProperty fadeInTimeSP;
        private SerializedProperty fadeOutTimeSP;

        private SerializedProperty prioritySP;
        private SerializedProperty volumeSP;
        private SerializedProperty pitchSP;

        private static GUIContent modeLabel = new GUIContent("Mode", "AudioSource����");

        private static GUIContent muteLabel = new GUIContent("Mute", "�Ƿ���");
        private static GUIContent playOnAwakeLabel = new GUIContent("Play On Awake", "����ʱ�Զ�����");
        private static GUIContent loopLabel = new GUIContent("Loop", "�Ƿ�ѭ������");
        private static GUIContent fadeInoutLabel = new GUIContent("Enable Fade", "�������뽥��");
        private static GUIContent fadeInTimeLabel = new GUIContent("FadeIn Time", "����ʱ��");
        private static GUIContent fadeOutTimeLabel = new GUIContent("FadeOut Time", "����ʱ��");

        private static GUIContent priorityLabel = new GUIContent("Priority", "��ʼ˳λ");
        private static GUIContent volumeLabel = new GUIContent("Volume", "��ʼ����");
        private static GUIContent pitchLabel = new GUIContent("Pitch", "��ʼ����");

        protected void OnEnable()
        {
            OnStartSP = serializedObject.FindProperty("OnStart");
            modeSP = serializedObject.FindProperty("mode");
            audioClipSP = serializedObject.FindProperty("audioClip");
            audioMixerGroupSP = serializedObject.FindProperty("audioMixerGroup");
            muteSP = serializedObject.FindProperty("mute");
            playOnAwakeSP = serializedObject.FindProperty("playOnAwake");
            loopSP = serializedObject.FindProperty("loop");
            fadeInOutSP = serializedObject.FindProperty("fadeInOut");
            fadeInTimeSP = serializedObject.FindProperty("fadeInTime");
            fadeOutTimeSP = serializedObject.FindProperty("fadeOutTime");
            prioritySP = serializedObject.FindProperty("priority");
            volumeSP = serializedObject.FindProperty("volume");
            pitchSP = serializedObject.FindProperty("pitch");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            Draw();

            serializedObject.ApplyModifiedProperties();
        }

        private void Draw()
        {
            MEditorGUIUtility.DrawLeftH2("��������");

            MEditorControlUtility.DrawProperty(OnStartSP);
            MEditorControlUtility.DrawProperty(audioClipSP);
            MEditorControlUtility.DrawProperty(audioMixerGroupSP);

            EditorGUILayout.Space(5);

            MEditorControlUtility.DrawIntSlider(prioritySP, 0, 256, priorityLabel);
            MEditorControlUtility.DrawSlider(volumeSP, 0, 1, volumeLabel);
            MEditorControlUtility.DrawSlider(pitchSP, 0, 3, pitchLabel);

            EditorGUILayout.Space(5);
            MEditorGUIUtility.DrawLeftH2("ר������");

            var mode = MEditorControlUtility.DrawPopup<AudioSourceMode>(modeSP, modeLabel);

            EditorGUI.indentLevel++;
            {
                if (mode == AudioSourceMode.Music)
                {
                    MEditorControlUtility.SetToggle(false, muteSP, muteLabel);
                    MEditorControlUtility.SetToggle(true, playOnAwakeSP, playOnAwakeLabel);
                    MEditorControlUtility.SetToggle(true, loopSP, loopLabel);

                    EditorGUILayout.Space(5);

                    MEditorControlUtility.SetToggle(true, fadeInOutSP, fadeInoutLabel);
                    EditorGUI.indentLevel++;
                    {
                        MEditorControlUtility.DrawFloat(fadeInTimeSP, fadeInTimeLabel);
                        MEditorControlUtility.DrawFloat(fadeOutTimeSP, fadeOutTimeLabel);
                    }
                    EditorGUI.indentLevel--;
                }
                else if (mode == AudioSourceMode.SFX)
                {
                    MEditorControlUtility.SetToggle(false, muteSP, muteLabel);
                    MEditorControlUtility.SetToggle(false, playOnAwakeSP, playOnAwakeLabel);
                    MEditorControlUtility.SetToggle(false, loopSP, loopLabel);

                    EditorGUILayout.Space(5);

                    MEditorControlUtility.SetToggle(false, fadeInOutSP, fadeInoutLabel);
                }
                else if (mode == AudioSourceMode.Custom)
                {
                    MEditorControlUtility.DrawToggle(muteSP, muteLabel);
                    MEditorControlUtility.DrawToggle(playOnAwakeSP, playOnAwakeLabel);
                    MEditorControlUtility.DrawToggle(loopSP, loopLabel);

                    EditorGUILayout.Space(5);

                    bool fadeState = MEditorControlUtility.DrawToggle(fadeInOutSP, fadeInoutLabel);

                    if (fadeState)
                    {
                        EditorGUI.indentLevel++;
                        {
                            MEditorControlUtility.DrawFloat(fadeInTimeSP, fadeInTimeLabel);
                            MEditorControlUtility.DrawFloat(fadeOutTimeSP, fadeOutTimeLabel);
                        }
                        EditorGUI.indentLevel--;
                    }
                }
            }
            EditorGUI.indentLevel--;
        }
    }
}
