using UnityEngine;

namespace MFramework
{
    public static class MABUtility
    {
        public static string GetPlatform()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                    return "WINDOWS";
                case RuntimePlatform.Android:
                    return "ANDROID";
                case RuntimePlatform.IPhonePlayer:
                    return "IOS";
                default:
                    MLog.Print($"δ֧�ֵ�ƽ̨:{Application.platform}", MLogType.Error);
                    return null;
            }
        }
    }
}