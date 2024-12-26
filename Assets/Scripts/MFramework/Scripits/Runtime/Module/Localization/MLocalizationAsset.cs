using MFramework;
using System;
using System.Collections.Generic;
using System.Reflection;

public class MLocalizationAsset
{
    /// <summary>
    /// Excel����
    /// </summary>
    internal LocalizationTable[] items;

    /// <summary>
    /// ֧�����������б�
    /// </summary>
    internal List<SupportLanguage> supportLanguages;

    /// <summary>
    /// Excel�����(ÿ��idһ����Ϣ(������))
    /// </summary>
    internal Dictionary<int, LocalizationTable> tableDic;

    //��������ѡ��
    Dictionary<string, SupportLanguage> supportLanguageDic = new Dictionary<string, SupportLanguage>()
    {
        { "CHINESE" , SupportLanguage.CHINESE  },
        { "ENGLISH" , SupportLanguage.ENGLISH  },
        { "JAPENESE", SupportLanguage.JAPANESE }
    };

    internal MLocalizationAsset(LocalizationTable[] table)
    {
        if (table == null || table.Length == 0)
        {
            MLog.Print($"{typeof(MLocalizationAsset)}��δ��ȡ��xlsx���ڻ�xlsx���������ݣ�����", MLogType.Error);
            return;
        }

        items = table;
        //����tableDic
        tableDic = new Dictionary<int, LocalizationTable>();
        foreach (var item in items)
        {
            if (!tableDic.ContainsKey(item.ID))//ȷ��key���ظ�
            {
                tableDic.Add(item.ID, item);
            }
        }

        //��ȡ��������
        Type type = typeof(LocalizationTable);
        PropertyInfo[] properties = type.GetProperties();
        supportLanguages = new List<SupportLanguage>();
        foreach (var property in properties)
        {
            if (supportLanguageDic.ContainsKey(property.Name))//�������е�һ��
            {
                supportLanguages.Add(supportLanguageDic[property.Name]);
            }
        }
    }
}

public enum SupportLanguage
{
    CHINESE = 0,
    ENGLISH = 1,
    JAPANESE = 2,

    Default = 1000
}