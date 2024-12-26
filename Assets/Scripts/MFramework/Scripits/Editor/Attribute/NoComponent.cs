using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MFramework
{
    [InitializeOnLoad]
    public static class NoComponent
    {
        static NoComponent()
        {
            // �����������Ĳ���
            ObjectFactory.componentWasAdded += OnComponentAdded;
        }

        private static void OnComponentAdded(Component component)
        {
            var componentType = component.GetType();
            var attrs = componentType.GetCustomAttributes(typeof(NoComponentAttribute), true);
            if (attrs.Length == 0)
            {
                return;//���Ǹ�����
            }

            MLog.Print($"�������{component.GetType().Name}", MLogType.Warning);
            Object.DestroyImmediate(component);
        }
    }
}
