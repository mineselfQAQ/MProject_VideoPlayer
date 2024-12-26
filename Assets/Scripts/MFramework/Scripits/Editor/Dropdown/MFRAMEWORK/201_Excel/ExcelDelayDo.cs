using System.Collections.Generic;
using UnityEditor;

namespace MFramework
{
    public static class ExcelDelayDo
    {
        [InitializeOnLoadMethod]
        public static void InitializeAfterAssemblyReload()
        {
            //��Ҫ��һ������CS�ļ���BIN�ļ�ʱ������CS�ļ���������������BIN�ļ�����δ�ɹ����أ�
            //��Ҫ**�������غ����BIN�ļ��Ĵ���**
            AssemblyReloadEvents.afterAssemblyReload += GenerateBIN;
        }

        private static void GenerateBIN()
        {
            if (EditorPrefs.GetBool(EditorPrefsData.ExcelBINGenerationState, false))
            {
                EditorPrefs.SetBool(EditorPrefsData.ExcelBINGenerationState, false);

                string BINFolder = MSettings.ExcelBINPath;//Ĭ��.byte�ļ����λ��---StreamingAssets�ļ����ڲ�
                List<string> fileList = MPathUtility.GetFiles(MConfigurableSettings.ExcelPath, null, ".xlsx");//��ȡ�����ļ���

                ExcelGenerator.CreateAllBIN(BINFolder, fileList);
                EditorDelayExecute.Instance.DelayRefresh();//�ӳ�ִ��Refresh(�����޷�ˢ�³ɹ�)
            }
        }

    }
}
