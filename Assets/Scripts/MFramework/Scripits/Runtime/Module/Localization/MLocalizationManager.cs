using MFramework.UI;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MFramework
{
    /// <summary>
    /// Ŀǰ�ܹ���
    /// infoDic---���id�µ�������Ϣ��һ��id�����ж����Ϣ
    /// MLocalizationInfo---������Ϣ��ӵ�ж�������
    /// </summary>
    [MonoSingletonSetting(HideFlags.NotEditable, "#MLocalizationManager#")]
    public class MLocalizationManager : MonoSingleton<MLocalizationManager>
    {
        private SupportLanguage currentLanguage = SupportLanguage.Default;
        public SupportLanguage CurrentLanguage
        {
            internal set
            {
                currentLanguage = value;
            }
            get
            {
                return currentLanguage;
            }
        }
        public List<SupportLanguage> SupportLanguages => asset.supportLanguages;
        public int SupportLanguagesCount => asset.supportLanguages.Count;

        internal MLocalizationAsset asset;//ÿ��ID����Ӧ�Ķ����������б�

        public Dictionary<string, SupportLanguage> StrToSupportLanguageDic = new Dictionary<string, SupportLanguage>()
        {
            { "en", SupportLanguage.ENGLISH },
            { "zh", SupportLanguage.CHINESE }
            //TODO:������������
        };
        public Dictionary<SupportLanguage, string> SupportLanguageToStrDic = new Dictionary<SupportLanguage, string>()
        {
            { SupportLanguage.ENGLISH, "en" },
            { SupportLanguage.CHINESE, "zh" }
            //TODO:������������
        };

        //private void Awake()
        //{
        //    LocalizationTable[] table = LocalizationTable.LoadBytes();
        //    asset = new MLocalizationAsset(table);

        //    InitCurrentLanguage();
        //}
        private void Awake()
        {
            LocalizationTable.LoadBytes((tables) =>
            {
                if (tables == null)
                {
                    //ERROR
                }
                else
                {
                    asset = new MLocalizationAsset(tables);
                    InitCurrentLanguage();
                }
            });
        }

        private void InitCurrentLanguage()
        {
            var settings = MSerializationManager.Instance.coreSettings;
            string language = settings.language;
            currentLanguage = StrToSupportLanguageDic[language];
        }

        public void SetLanguage(SupportLanguage language)
        {
            if (!CheckLanguageValidity(language))
            {
                MLog.Print($"{typeof(MLocalizationManager)}��{language}δ���ã�����ת��Ϊ�����ԣ�����", MLogType.Warning);
                return;
            }
            if (language == currentLanguage) return;

            currentLanguage = language;

            string settingsPath = $"{MSettings.PersistentDataPath}/CoreSettings.json";
            SaveLanguageJson(settingsPath, currentLanguage);

            MText.UpdateAllInfo();
        }
        public void SetLanguage(string language)
        {
            SetLanguage(StrToSupportLanguageDic[language]);
        }

        private void SaveLanguageJson(string settingsPath, SupportLanguage language)
        {
            var settings = MSerializationUtility.ReadFromJson<CoreSettings>(settingsPath);
            settings.language = SupportLanguageToStrDic[language];
            MSerializationUtility.SaveToJson<CoreSettings>(settingsPath, settings);
        }

        private bool CheckLanguageValidity(SupportLanguage language)
        {
            if (!SupportLanguages.Contains(language)) return false;
            return true;
        }
    }
}