using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

namespace MFramework
{
    public class MResourceManager : Singleton<MResourceManager>
    {
        /// <summary>
        /// ������Դ��Ӧ��bundle(key---��Դ value---����bundle)
        /// </summary>
        internal Dictionary<string, string> ResourceBunldeDic = new Dictionary<string, string>();
        /// <summary>
        /// ������Դ��������ϵ(key---������������Դ value---����������Դ)
        /// </summary>
        internal Dictionary<string, List<string>> ResourceDependencyDic = new Dictionary<string, List<string>>();

        //��ǰ���ڵ�resource
        private Dictionary<string, ResourceBase> resourceDic = new Dictionary<string, ResourceBase>();
        //�첽�б�(���ڼ��ص�resource)
        private List<ResourceBaseAsync> asyncList = new List<ResourceBaseAsync>();
        //ж���б�(����ж�ص�resource)
        private LinkedList<ResourceBase> unloadList = new LinkedList<ResourceBase>();

        private MResourceManager() { }

        private const string MANIFEST_NAME = "manifest.ab";
        private static readonly string RESOURCEASSET_NAME = MSettings.ResourceAssetName;
        private static readonly string BUNDLEASSET_NAME = MSettings.BundleAssetName;
        private static readonly string DEPENDENCYASSET_NAME = MSettings.DependencyAssetName;

        public void Initialize(string platform, Func<string, string> getFileCallback, ulong offset)
        {
            //��ȡBundleManager.assetBundleManifest��Ϣ
            MBundleManager.Instance.Initialize(platform, getFileCallback, offset);

            //��ȡmanifest.ab�����س�AssetBundle
            string manifestBunldeFile = getFileCallback.Invoke(MANIFEST_NAME);
            AssetBundle manifestAssetBundle = AssetBundle.LoadFromFile(manifestBunldeFile, 0, offset);
            //ͨ��manifest.ab��ȡ�ڲ���Ϣ(���ʱ��¼��������Ϣ)
            TextAsset resourceTextAsset = manifestAssetBundle.LoadAsset(RESOURCEASSET_NAME) as TextAsset;
            TextAsset bundleTextAsset = manifestAssetBundle.LoadAsset(BUNDLEASSET_NAME) as TextAsset;
            TextAsset dependencyTextAsset = manifestAssetBundle.LoadAsset(DEPENDENCYASSET_NAME) as TextAsset;
            byte[] resourceBytes = resourceTextAsset.bytes;
            byte[] bundleBytes = bundleTextAsset.bytes;
            byte[] dependencyBytes = dependencyTextAsset.bytes;
            //��ȡ��Ϻ�ж��
            manifestAssetBundle.Unload(true);
            manifestAssetBundle = null;

            //-----��ȡ��Ϣ-----
            Dictionary<ushort, string> assetUrlDic = new Dictionary<ushort, string>();
            #region ��ȡ��Դ��Ϣ
            {
                MemoryStream resourceMemoryStream = new MemoryStream(resourceBytes);
                BinaryReader resourceBinaryReader = new BinaryReader(resourceMemoryStream);
                //��ȡ��Դ����
                ushort resourceCount = resourceBinaryReader.ReadUInt16();
                for (ushort i = 0; i < resourceCount; i++)
                {
                    string assetUrl = resourceBinaryReader.ReadString();
                    assetUrlDic.Add(i, assetUrl);
                }
            }
            #endregion
            #region ��ȡbundle��Ϣ
            {
                ResourceBunldeDic.Clear();
                MemoryStream bundleMemoryStream = new MemoryStream(bundleBytes);
                BinaryReader bundleBinaryReader = new BinaryReader(bundleMemoryStream);
                //��ȡbundle����
                ushort bundleCount = bundleBinaryReader.ReadUInt16();
                for (int i = 0; i < bundleCount; i++)
                {
                    string bundleUrl = bundleBinaryReader.ReadString();
                    string bundleFileUrl = bundleUrl;
                    //��ȡbundle�ڵ���Դ����
                    ushort resourceCount = bundleBinaryReader.ReadUInt16();
                    for (int j = 0; j < resourceCount; j++)
                    {
                        ushort assetId = bundleBinaryReader.ReadUInt16();
                        string assetUrl = assetUrlDic[assetId];
                        ResourceBunldeDic.Add(assetUrl, bundleFileUrl);
                    }
                }
            }
            #endregion
            #region ��ȡ��Դ������Ϣ
            {
                ResourceDependencyDic.Clear();
                MemoryStream dependencyMemoryStream = new MemoryStream(dependencyBytes);
                BinaryReader dependencyBinaryReader = new BinaryReader(dependencyMemoryStream);
                //��ȡ����������
                ushort dependencyCount = dependencyBinaryReader.ReadUInt16();
                for (int i = 0; i < dependencyCount; i++)
                {
                    //��ȡ��Դ����
                    ushort resourceCount = dependencyBinaryReader.ReadUInt16();
                    ushort assetId = dependencyBinaryReader.ReadUInt16();
                    string assetUrl = assetUrlDic[assetId];
                    List<string> dependencyList = new List<string>(resourceCount);
                    for (int j = 1; j < resourceCount; j++)//�������Լ�
                    {
                        ushort dependencyAssetId = dependencyBinaryReader.ReadUInt16();
                        string dependencyUrl = assetUrlDic[dependencyAssetId];
                        dependencyList.Add(dependencyUrl);
                    }

                    ResourceDependencyDic.Add(assetUrl, dependencyList);
                }
            }
            #endregion
        }

        public void Update()
        {
            MBundleManager.Instance.Update();//ͬResourceManager.Update()����������Bundle

            for (int i = 0; i < asyncList.Count; i++)
            {
                ResourceBaseAsync resourceAsync = asyncList[i];
                if (resourceAsync.Update())//�ȴ��첽��Դ�������
                {
                    asyncList.RemoveAt(i);
                    i--;

                    if (resourceAsync.awaiter != null)
                    {
                        resourceAsync.awaiter.SetResult(resourceAsync as IResource);
                    }
                }
            }
        }

        public void LateUpdate()
        {
            //������Ҫ�ͷŵ���Դ
            while (unloadList.Count > 0)//�������
            {
                ResourceBase resource = unloadList.First.Value;
                unloadList.RemoveFirst();
                if (resource == null) continue;

                resourceDic.Remove(resource.url);
                resource.UnLoad();//ж����Դ(��ҪΪ��bundle����unloadList)

                //��������
                if (resource.dependencies != null)
                {
                    for (int i = 0; i < resource.dependencies.Length; i++)
                    {
                        ResourceBase temp = resource.dependencies[i];
                        Unload(temp);//ж��(reference--�����û�����þͼ���unloadList׼��ж��)
                    }
                }
            }

            MBundleManager.Instance.LateUpdate();//ͬResourceManager.LateUpdate()��ΪBundle��ж��
        }

        /// <summary>
        /// ������Դ�������첽����Ҫʹ��Я��
        /// </summary>
        public IResource Load(string url, bool async)
        {
            return LoadInternal(url, async);
        }
        /// <summary>
        /// ������Դ(���лص�)
        /// </summary>
        public void LoadWithCallback(string url, bool async, Action<IResource> callback)
        {
            ResourceBase resource = LoadInternal(url, async);

            if (resource.done)//ͬ�����
            {
                callback?.Invoke(resource);
            }
            else//�첽������ȴ�������ִ�лص�
            {
                resource.finishedCallback += callback;
            }
        }
        /// <summary>
        /// ������Դ(ʹ��Awaiter)
        /// </summary>
        public ResourceAwaiter LoadWithAwaiter(string url)
        {
            ResourceBase resource = LoadInternal(url, true);

            //��Դ������ɣ��ɷ���
            if (resource.done)
            {
                if (resource.awaiter == null)
                {
                    resource.awaiter = new ResourceAwaiter();
                    resource.awaiter.SetResult(resource as IResource);
                }

                return resource.awaiter;
            }

            //��һ�μ��أ�����ResoureAwaiter
            if (resource.awaiter == null)
            {
                resource.awaiter = new ResourceAwaiter();
            }

            return resource.awaiter;
        }

        private ResourceBase LoadInternal(string url, bool async)
        {
            //���Ի�ȡResource
            ResourceBase resource = null;
            if (resourceDic.TryGetValue(url, out resource))
            {
                //�Ѿ�׼��ж�أ�������������Ҫʹ�ã�ֱ��ȡ��
                if (resource.reference == 0)
                {
                    unloadList.Remove(resource);
                }
                resource.AddReference();

                return resource;
            }

            //����Resource
            if (async)
            {
                ResourceAsync resourceAsync = new ResourceAsync();//�첽���ط�ʽ
                asyncList.Add(resourceAsync);
                resource = resourceAsync;
            }
            else
            {
                resource = new Resource();//ͬ�����ط�ʽ
            }
            resource.url = url;
            resourceDic.Add(url, resource);

            //��������
            List<string> dependencies = null;
            ResourceDependencyDic.TryGetValue(url, out dependencies);
            if (dependencies != null && dependencies.Count > 0)//�����url�������ͽ������
            {
                resource.dependencies = new ResourceBase[dependencies.Count];
                for (int i = 0; i < dependencies.Count; i++)
                {
                    string dependencyUrl = dependencies[i];
                    ResourceBase dependencyResource = LoadInternal(dependencyUrl, async);
                    resource.dependencies[i] = dependencyResource;
                }
            }

            resource.AddReference();
            resource.Load();//**ʵ��resource��Load()**

            return resource;
        }

        public void Unload(string assetUrl)
        {
            if (string.IsNullOrEmpty(assetUrl))
            {
                MLog.Print($"{nameof(MResourceManager)}.{nameof(Unload)}����Դ·��Ϊ�գ�����", MLogType.Error);
            }

            ResourceBase resource;
            if (!resourceDic.TryGetValue(assetUrl, out resource))
            {
                MLog.Print($"{nameof(MResourceManager)}.{nameof(Unload)}��<{assetUrl}>����Ӧ��Դ�����ڣ�����", MLogType.Error);
            }
            Unload(resource);
        }
        public void Unload(IResource resource)
        {
            if (resource == null)
            {
                MLog.Print($"{nameof(MResourceManager)}.{nameof(Unload)}����ԴΪ�գ�����", MLogType.Error);
            }

            //��Resource��reference--�����Ϊ0(û������)������׼��ж��
            ResourceBase resourceBase = resource as ResourceBase;
            resourceBase.ReduceReference();
            if (resourceBase.reference == 0)
            {
                WillUnload(resourceBase);
            }
        }

        private void WillUnload(ResourceBase resource)
        {
            unloadList.AddLast(resource);
        }
    }
}