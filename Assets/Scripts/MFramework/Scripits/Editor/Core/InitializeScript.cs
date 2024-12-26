using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MFramework
{
    public class InitializeScript
    {
        #region ��ӭ�����ʼ��
        [InitializeOnLoadMethod]
        public static void InitializeWelcomePage()
        {
            //ֻ�����״ν���ʱ�Զ���WelcomePage
            bool state = EditorPrefs.GetBool(EditorPrefsData.WelcomePageState, true);
            if (state)
            {
                EditorPrefs.SetBool(EditorPrefsData.WelcomePageState, false);
                WelcomePage.Init();
            }
        }
        #endregion

        #region ���MCore�Ƿ���Scene��
        [InitializeOnLoadMethod]
        public static void InitializeSceneOpen()
        {
            EditorSceneManager.sceneOpened += OnSceneOpened;
        }
        private static void OnSceneOpened(Scene scene, OpenSceneMode mode)
        {
            //�򿪳���ʱ�����Ҫ����MCore�����д���
            if (EditorPrefs.GetBool(EditorPrefsData.EnableCheckMCore, true))
            {
                CheckMCoreExist(scene);
            }
        }
        private static void CheckMCoreExist(Scene scene)
        {
            GameObject[] rootGOs = SceneManager.GetActiveScene().GetRootGameObjects();

            //�����������MCore
            foreach (GameObject go in rootGOs)
            {
                if (go.name == MSettings.MCoreName)
                {
                    return;
                }
            }

            //�������Hierarchy������MCore
            foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>())
            {
                if (go.name == MSettings.MCoreName)
                {
                    MLog.Print($"{typeof(InitializeScript)}.{nameof(CheckMCoreExist)}���������MCore�����ڱ�㣬�����Ƿ���", MLogType.Warning);
                    return;
                }
            }

            //���MCore
            GameObject MCore = new GameObject(MSettings.MCoreName);
            MCore.transform.SetAsFirstSibling();
            MCore.AddComponent<MCore>();
            GameObjectUtility.SetParentAndAlign(MCore, null);
            Selection.activeGameObject = MCore;
            EditorUtility.SetDirty(MCore);
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            EditorGUIUtility.PingObject(MCore);
            MLog.Print($"��Ϊ{scene.name}��Ӻ������{MSettings.MCoreName}");
        }
        #endregion

        /// <summary>
        /// ��⵱ǰ�����Ƿ�Ϊһ̨
        /// </summary>
        [InitializeOnLoadMethod]
        public static void CheckComputerUniqueID()
        {
            string infoName = $"{MSettings.CorePath}/Prefs/DeviceInfo";

            bool exist;
            if (!File.Exists(infoName))
            {
                Directory.CreateDirectory(infoName.CD());//��֤�ļ��еĴ���
                exist = false;
            }
            else
            {
                exist = true;
            }

            string curID = SystemInfo.deviceUniqueIdentifier;
            if (!exist)
            {
                MSerializationUtility.SaveToFile(infoName, curID);
            }
            else
            {
                string preID = MSerializationUtility.ReadFromFile(infoName);
                if (curID != preID)
                {
                    MLog.Print($"{typeof(InitializeScript)}.{nameof(CheckComputerUniqueID)}��ע�⣡��ǰ�豸���л������EditorSettingsConfigurator�鿴·���Ƿ�������ȷ", MLogType.Warning);
                }
                MSerializationUtility.SaveToFile(infoName, curID);
            }
        }
    }
}