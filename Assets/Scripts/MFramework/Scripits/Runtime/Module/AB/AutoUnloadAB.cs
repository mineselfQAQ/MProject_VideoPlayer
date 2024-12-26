using UnityEngine;

namespace MFramework
{
    /// <summary>
    /// �Զ�ж��AB�ű�
    /// </summary>
    public class AutoUnloadAB : MonoBehaviour
    {
        public IResource resource { get; set; }

        private void OnDestroy()
        {
            if (resource == null) return;

            MResourceManager.Instance.Unload(resource);//ж��
            resource = null;
        }
    }
}