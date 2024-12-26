using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MFramework
{
    [InitializeOnLoad]
    public static class DisallowComponent
    {
        static DisallowComponent()
        {
            // �����������Ĳ���
            ObjectFactory.componentWasAdded += OnComponentAdded;
        }

        private static void OnComponentAdded(Component component)
        {
            var componentType = component.GetType();

            var attrs = componentType.GetCustomAttributes(typeof(DisallowComponentAttribute), true);
            if (attrs.Length == 0)
            {
                return;//���Ǹ�����
            }

            //===���죺���Խű�����ӣ������ű��ж��Ƿ������Խű���ͻ����ͻ����Ӹýű�===
            var allComponents = component.gameObject.GetComponents<MonoBehaviour>();
            foreach (var comp in allComponents)
            {
                var attributes = comp.GetType().GetCustomAttributes(typeof(DisallowComponentAttribute), true)
                                       .Cast<DisallowComponentAttribute>();

                foreach (var attr in attributes)
                {
                    if (attr.disallowedTypes.Contains(componentType))
                    {
                        MLog.Print($"{comp.GetType().Name}���������{componentType.Name}", MLogType.Warning);
                        Object.DestroyImmediate(component);
                        return;
                    }
                }
            }

            //===�Լ죺������Խű����ж��Ƿ�����֮��ͻ�ű�����ͻ����ʾ(ɾ��)��ͻ�ű�===
            //Tip��ɾ����̫�ã���ֹ�޷����أ����û�����
            var attributes2 = componentType.GetCustomAttributes(typeof(DisallowComponentAttribute), true);
            foreach (DisallowComponentAttribute attr in attributes2)
            {
                foreach (var disallowedType in attr.disallowedTypes)
                {
                    if (component.gameObject.GetComponent(disallowedType) != null)
                    {
                        MLog.Print($"{componentType.Name}���������{componentType.Name}�������{disallowedType.Name}������ɾ��", MLogType.Warning);
                        //Object.DestroyImmediate(component.gameObject.GetComponent(disallowedType));
                        Object.DestroyImmediate(component);
                        return;
                    }
                }
            }
        }
    }
}
