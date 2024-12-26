using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MFramework
{
    public static class UIPanelUtility
    {
#if UNITY_EDITOR
        public static string GetPrefabPath(UnityEngine.Object target)
        {
            //���¾��ǣ�
            //�ڲ�ͬ״̬�µ��Inspector��Export��ť��ʹ��ͬһ�����������ܻ�ȡ��·��
            //1---����ͨ�ģ����ǵ��Project����µ�Prefab����ʱ��������AssetDatabase.GetAssetPath()
            //2---���Hierarchy�µ�Prefab����ʱֻ��ͨ��PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot()���ܻ�ȡ
            //3---��Ԥ��������е����prefabStage.assetPath���Ի�ȡ

            PrefabAssetType singlePrefabType = PrefabUtility.GetPrefabAssetType(target);
            PrefabInstanceStatus singleInstanceStatus = PrefabUtility.GetPrefabInstanceStatus(target);
            string targetAssetPath = AssetDatabase.GetAssetPath(target);
            string prefabAssetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(target);
            UnityEditor.SceneManagement.PrefabStage prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();

            //��Ҫ���ǲ���ȷ�ж����������
            string finalPrefabPath = null;
            if (singlePrefabType == PrefabAssetType.Regular && !string.IsNullOrEmpty(targetAssetPath))
            {
                finalPrefabPath = targetAssetPath;//���Ԥ��ʱ
            }
            else if (singlePrefabType == PrefabAssetType.Regular && !string.IsNullOrEmpty(prefabAssetPath))
            {
                finalPrefabPath = prefabAssetPath;//Ԥ������Hierarchy��ѡ��ʱ
            }
            else if (prefabStage != null)
            {
                finalPrefabPath = prefabStage.assetPath;//˫��Ԥ�貢��Hierarchy��ѡ��ʱ
            }

            return finalPrefabPath;
        }
#endif

        internal static void ResetOrder(UIRoot root)
        {
            List<UIPanel> panels = FilterSortedPanel(root);
            if (panels.Count > 0) panels[0].SetSortingOrder(root.startOrder);
            for (int i = 1; i < panels.Count; i++)
            {
                int order = panels[i - 1].sortingOrder + panels[i - 1].panelBehaviour.Thickness;
                if (order > root.endOrder)
                {
                    MLog.Print($"{typeof(UIPanelUtility)}.{nameof(ResetOrder)}��RootID-<{root.rootID}>�ѳ��ݣ������ݻ����Thickness", MLogType.Error);
                    break;
                }
                panels[i].SetSortingOrder(order);
            }
        }

        /// <summary>
        /// ɸѡPanel�������ɸѡ�ͻ��ȡ���е�Panel
        /// </summary>
        public static List<UIPanel> FilterPanels(UIRoot root, Func<UIPanel, bool> filterFunc = null)
        {
            List<UIPanel> panels = new List<UIPanel>();

            foreach (KeyValuePair<string, UIPanel> kvPair in root.panelDic)
            {
                if (filterFunc == null || filterFunc(kvPair.Value))
                {
                    panels.Add(kvPair.Value);
                }
            }
            return panels;
        }
        /// <summary>
        /// ɸѡPanel������
        /// </summary>
        /// <param playerName="filterFunc"></param>
        /// <returns></returns>
        public static List<UIPanel> FilterSortedPanel(UIRoot root, Func<UIPanel, bool> filterFunc = null)
        {
            List<UIPanel> panels = FilterPanels(root, filterFunc);

            panels.Sort((a, b) => { return a.canvas.sortingOrder - b.canvas.sortingOrder; });

            return panels;
        }
        /// <summary>
        /// ɸѡPanel����ȡ���ϲ�Panel
        /// </summary>
        public static UIPanel FilterTopestPanel(UIRoot root, Func<UIPanel, bool> filterFunc = null)
        {
            List<UIPanel> panels = FilterPanels(root, filterFunc);

            panels.Sort((a, b) => { return a.canvas.sortingOrder - b.canvas.sortingOrder; });

            return panels.Count > 0 ? panels[panels.Count - 1] : null;
        }
        /// <summary>
        /// ɸѡPanel����ȡ���²�Panel
        /// </summary>
        public static UIPanel FilterBottommostPanel(UIRoot root, Func<UIPanel, bool> filterFunc = null)
        {
            List<UIPanel> panels = FilterPanels(root, filterFunc);

            panels.Sort((a, b) => { return a.canvas.sortingOrder - b.canvas.sortingOrder; });

            return panels.Count > 0 ? panels[0] : null;
        }

        public static bool SetCanvasGroupActive(CanvasGroup canvasGroup, bool active)
        {
            if (canvasGroup == null) return false;

            canvasGroup.alpha = active ? 1 : 0;
            canvasGroup.interactable = active;
            canvasGroup.blocksRaycasts = active;
            return true;
        }
    }
}