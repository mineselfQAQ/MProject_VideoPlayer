using UnityEngine;

namespace MFramework
{
    /// <summary>
    /// Resource�����ӿ�
    /// </summary>
    public interface IResource
    {
        string url { get; }

        Object GetAsset();
        T GetAsset<T>() where T : Object;

        GameObject Instantiate(bool autoUnload = false);
        GameObject Instantiate(Vector3 position, Quaternion rotation, bool autoUnload = false);
        GameObject Instantiate(Transform parent, bool instantiateInWorldSpace, bool autoUnload = false);
    }
}