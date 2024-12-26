using System;
using UnityEditor;
using UnityEngine;

namespace MFramework
{
    public static class MEditorControlUtility
    {
        #region �����б�
        public static T DrawPopup<T>(SerializedProperty SP, GUIContent content) where T : Enum
        {
            Enum enumValue = EditorGUILayout.EnumPopup(content, (T)(object)SP.enumValueIndex);
            T TEnumValue = (T)enumValue;
            SP.enumValueIndex = Convert.ToInt32(TEnumValue);

            return TEnumValue;
        }
        public static T DrawPopup<T>(SerializedProperty SP, string label) where T : Enum
        {
            Enum enumValue = EditorGUILayout.EnumPopup(label, (T)(object)SP.enumValueIndex);
            T TEnumValue = (T)enumValue;
            SP.enumValueIndex = Convert.ToInt32(TEnumValue);

            return TEnumValue;
        }
        #endregion

        #region ��ť
        public static bool DrawToggle(SerializedProperty SP, GUIContent content)
        {
            bool boolValue = EditorGUILayout.Toggle(content, SP.boolValue);
            SP.boolValue = boolValue;

            return boolValue;
        }
        public static bool DrawToggle(SerializedProperty SP, string label)
        {
            bool boolValue = EditorGUILayout.Toggle(label, SP.boolValue);
            SP.boolValue = boolValue;

            return boolValue;
        }
        public static void SetToggle(bool newValue, SerializedProperty SP, GUIContent content)
        {
            GUI.enabled = false;
            {
                bool boolValue = EditorGUILayout.Toggle(content, SP.boolValue);
                SP.boolValue = newValue;
            }
            GUI.enabled = true;
        }
        public static void SetToggle(bool newValue, SerializedProperty SP, string label)
        {
            GUI.enabled = false;
            {
                bool boolValue = EditorGUILayout.Toggle(label, SP.boolValue);
                SP.boolValue = newValue;
            }
            GUI.enabled = true;
        }
        #endregion

        #region Int/Float
        public static int DrawInt(SerializedProperty SP, GUIContent content)
        {
            int intValue = EditorGUILayout.IntField(content, SP.intValue);
            SP.intValue = intValue;

            return intValue;
        }
        public static int DrawInt(SerializedProperty SP, string label)
        {
            int intValue = EditorGUILayout.IntField(label, SP.intValue);
            SP.intValue = intValue;

            return intValue;
        }

        public static float DrawFloat(SerializedProperty SP, GUIContent content)
        {
            float floatValue = EditorGUILayout.FloatField(content, SP.floatValue);
            SP.floatValue = floatValue;

            return floatValue;
        }
        public static float DrawFloat(SerializedProperty SP, string label)
        {
            float floatValue = EditorGUILayout.FloatField(label, SP.floatValue);
            SP.floatValue = floatValue;

            return floatValue;
        }
        #endregion

        #region ����
        public static float DrawSlider(SerializedProperty SP, float min, float max, GUIContent content)
        {
            float floatValue = EditorGUILayout.Slider(content, SP.floatValue, min, max);
            SP.floatValue = floatValue;

            return floatValue;
        }
        public static float DrawSlider(SerializedProperty SP, float min, float max, string label)
        {
            float floatValue = EditorGUILayout.Slider(label, SP.floatValue, min, max);
            SP.floatValue = floatValue;

            return floatValue;
        }

        public static float DrawIntSlider(SerializedProperty SP, int min, int max, GUIContent content)
        {
            int intValue = EditorGUILayout.IntSlider(content, SP.intValue, min, max);
            SP.intValue = intValue;

            return intValue;
        }
        public static float DrawIntSlider(SerializedProperty SP, int min, int max, string label)
        {
            int intValue = EditorGUILayout.IntSlider(label, SP.intValue, min, max);
            SP.intValue = intValue;

            return intValue;
        }
        #endregion

        #region Property(ԭ������)
        public static void DrawProperty(SerializedProperty SP, GUIContent content = null)
        {
            EditorGUILayout.PropertyField(SP, content);
        }
        #endregion
    }
}
