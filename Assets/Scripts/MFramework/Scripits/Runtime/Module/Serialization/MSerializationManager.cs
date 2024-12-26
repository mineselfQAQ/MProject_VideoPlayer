using System;
using System.IO;
using UnityEngine;

namespace MFramework
{
    [MonoSingletonSetting(HideFlags.NotEditable, "#MSerializationManager#")]
    public class MSerializationManager : MonoSingleton<MSerializationManager>
    {
        public string settingsPath;
        public CoreSettings coreSettings => MSerializationUtility.ReadFromJson<CoreSettings>(settingsPath);

        private void Awake()
        {
            //�������ȡCoreSettings
            settingsPath = $"{MSettings.PersistentDataPath}/CoreSettings.json";
            if (!File.Exists(settingsPath))
            {
                MSerializationUtility.SaveToJson<CoreSettings>(settingsPath, new CoreSettings(), true);
                MLog.Print($"{typeof(MLocalizationManager)}���ѳ�ʼ��CoreSettings�ļ���·��:<{settingsPath}>");
            }
        }
    }
}
