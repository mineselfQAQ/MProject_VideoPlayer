using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace MFramework
{
    internal static class CreateUIAnimatorController
    {
        [MenuItem("Assets/MCreate/UI/UIAnimatorController", false, priority = 1, secondaryPriority = 1.0f)]
        internal static void Create()
        {
            if (CheckAvailability())
            {
                var objName = Selection.objects[0].name;

                var guids = Selection.assetGUIDs;
                string directoryPath = Path.GetDirectoryName(AssetDatabase.GUIDToAssetPath(guids[0]));
                string acPath = Path.Combine(directoryPath, $"{objName}.controller");
                string openClipPath = Path.Combine(directoryPath, $"{objName}_Open.anim");
                string closeClipPath = Path.Combine(directoryPath, $"{objName}_Close.anim");

                CreateAnimatorController(objName, acPath, openClipPath, closeClipPath);
            }
        }

        internal static AnimatorController CreateAnimatorController(string objName, string acPath, string openClipPath, string closeClipPath)
        {
            //����.controller�ļ�
            if (File.Exists(acPath) || File.Exists(openClipPath) || File.Exists(closeClipPath))
            {
                bool state = EditorUtility.DisplayDialog("����",
                    $"{objName}·�����Ѵ���.controller��.anim�ļ����Ƿ���Ҫ���������ļ����´���", "����", "ȡ��");
                if (!state) 
                {
                    MLog.Print("��ȡ������", MLogType.Warning);
                    return AssetDatabase.LoadAssetAtPath<AnimatorController>(acPath);
                }
            }

            AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(acPath);

            //��Ӳ���
            controller.AddParameter("Open", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("Close", AnimatorControllerParameterType.Trigger);

            //���״̬
            var rootStateMachine = controller.layers[0].stateMachine;
            var defaultState = rootStateMachine.AddState("Idle");
            rootStateMachine.defaultState = defaultState;
            var openState = rootStateMachine.AddState("OpenState");
            var closeState = rootStateMachine.AddState("CloseState");

            //���Clip
            AnimationClip openClip = new AnimationClip();
            AnimationUtility.GetAnimationClipSettings(openClip).loopTime = false;
            AssetDatabase.CreateAsset(openClip, openClipPath);
            openState.motion = openClip;

            AnimationClip closeClip = new AnimationClip();
            AnimationUtility.GetAnimationClipSettings(closeClip).loopTime = false;
            AssetDatabase.CreateAsset(closeClip, closeClipPath);
            closeState.motion = closeClip;

            //��ӹ���
            var anyStateToOpen = rootStateMachine.AddAnyStateTransition(openState);
            anyStateToOpen.AddCondition(AnimatorConditionMode.If, 0, "Open");
            anyStateToOpen.duration = 0;

            var anyStateToClose = rootStateMachine.AddAnyStateTransition(closeState);
            anyStateToClose.AddCondition(AnimatorConditionMode.If, 0, "Close");
            anyStateToClose.duration = 0;


            //var defaultToOpen = defaultState.AddTransition(openState);
            //defaultToOpen.AddCondition(AnimatorConditionMode.If, 0, "Open");
            //defaultToOpen.duration = 0;

            //var openToClose = openState.AddTransition(closeState);
            //openToClose.AddCondition(AnimatorConditionMode.If, 0, "Close");
            //openToClose.duration = 0;

            //var closeToOpen = closeState.AddTransition(openState);
            //closeToOpen.AddCondition(AnimatorConditionMode.If, 0, "Open");
            //closeToOpen.duration = 0;

            AssetDatabase.SaveAssets();
            MLog.Print($"������{objName}.controller�ļ�����Clip�ļ�");

            return controller;
        }

        private static bool CheckAvailability()
        {
            var objs = Selection.objects;

            if (objs.Length != 1)
            {
                MLog.Print("�����ѡ��Դ��������", MLogType.Warning);
                return false;
            }

            if (!PrefabUtility.IsPartOfAnyPrefab(objs[0]))
            {
                MLog.Print($"{objs[0].name}����prefab���޷����д���������������", MLogType.Warning);
                return false;
            }

            GameObject prefab = objs[0] as GameObject;
            bool flag = false;
            var comps = prefab.GetComponents<Component>();
            foreach (var comp in comps)
            {
                MonoScript monoScript = MonoScript.FromMonoBehaviour(comp as MonoBehaviour);
                if (monoScript != null && (monoScript.name == "UIPanelBehaviour" || monoScript.name == "UIWidgetBehaviour"))
                {
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                MLog.Print($"{objs[0].name}��û��UIPanelBehaviour��UIWidgetBehaviour�ű����޷����д���������������", MLogType.Warning);
                return false;
            }

            return true;
        }
    }
}