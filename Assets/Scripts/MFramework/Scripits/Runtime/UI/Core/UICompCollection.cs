using System;
using System.Collections.Generic;
using UnityEngine;

namespace MFramework
{
    /// <summary>
    /// Behaviour���ռ�����GameObject��Component����
    /// </summary>
    [Serializable]
    public class UICompCollection
    {
        [SerializeField]
        private GameObject target;//�ռ�Ŀ��
        [SerializeField]
        private List<Component> compList;//�ռ�����target�ϵ����

        public GameObject Target
        {
            get
            {
                return target;
            }
            set
            {
                target = value;
            }
        }
        public List<Component> CompList
        {
            get
            {
                return compList;
            }
        }

        public UICompCollection()
        {
            target = null;
            compList = new List<Component>();
        }

        public Component GetComp(int index) => compList[index];
    }
}