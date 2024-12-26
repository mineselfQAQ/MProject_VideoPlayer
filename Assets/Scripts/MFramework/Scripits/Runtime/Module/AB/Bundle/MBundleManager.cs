using System;
using System.Collections.Generic;
using UnityEngine;

namespace MFramework
{
    public class MBundleManager : Singleton<MBundleManager>
    {
        private Func<string, string> getFileCallback;
        private AssetBundleManifest assetBundleManifest;
        internal ulong offset { get; private set; }

        //��ǰ���ڵ�bundle
        private Dictionary<string, BundleBase> bundleDic = new Dictionary<string, BundleBase>();
        //�첽�б�(���ڼ��ص�bundle)
        private List<BundleBaseAsync> asyncList = new List<BundleBaseAsync>();
        //ж���б�(����ж�ص�bundle)
        private LinkedList<BundleBase> unloadList = new LinkedList<BundleBase>();

        private MBundleManager() { }

        internal void Initialize(string platform, Func<string, string> getFileCallback, ulong offset)
        {
            this.getFileCallback = getFileCallback;
            this.offset = offset;

            //ƽ̨���ļ�(��WINDOWS(�޺�׺))
            string assetBundleManifestFile = getFileCallback.Invoke(platform);

            //��ȡAssetBundleManifest
            AssetBundle manifestAssetBundle = AssetBundle.LoadFromFile(assetBundleManifestFile);
            if (manifestAssetBundle == null)
            {
                MLog.Print($"{nameof(MBundleManager)}.{nameof(Initialize)}��AssetBundleManifest����ʧ�ܣ�����getFileCallback()��ȡ·���Ƿ���ȷ", MLogType.Error);
            }
            UnityEngine.Object[] objs = manifestAssetBundle.LoadAllAssets();
            if (objs.Length == 0)
            {
                MLog.Print($"{nameof(MBundleManager)}.{nameof(Initialize)}��AssetBundleManifest�������ݣ�����", MLogType.Error);
            }
            assetBundleManifest = objs[0] as AssetBundleManifest;
        }

        public void Update()
        {
            for (int i = 0; i < asyncList.Count; i++)
            {
                if (asyncList[i].Update())//�ȴ��첽��Դ�������
                {
                    asyncList.RemoveAt(i);
                    i--;
                }
            }
        }

        public void LateUpdate()
        {
            //������Ҫ�ͷŵ���Դ
            while (unloadList.Count > 0)//�������
            {
                BundleBase bundle = unloadList.First.Value;
                unloadList.RemoveFirst();
                if (bundle == null) continue;

                bundleDic.Remove(bundle.url);

                //��û�������ж��
                if (!bundle.done && bundle is BundleAsync)
                {
                    BundleAsync bundleAsync = bundle as BundleAsync;
                    if (asyncList.Contains(bundleAsync))
                        asyncList.Remove(bundleAsync);
                }
                //һ��ж��
                bundle.UnLoad();

                //��������
                if (bundle.dependencies != null)
                {
                    for (int i = 0; i < bundle.dependencies.Length; i++)
                    {
                        BundleBase temp = bundle.dependencies[i];
                        UnLoad(temp);//ж��(reference--�����û�����þͼ���unloadList׼��ж��)
                    }
                }
            }
        }

        /// <summary>
        /// ͬ������
        /// </summary>
        internal BundleBase Load(string url)
        {
            return LoadInternal(url, false);
        }
        /// <summary>
        /// �첽����
        /// </summary>
        internal BundleBase LoadAsync(string url)
        {
            return LoadInternal(url, true);
        }

        private BundleBase LoadInternal(string url, bool async)
        {
            //���Ի�ȡBundle
            BundleBase bundle = null;
            if (bundleDic.TryGetValue(url, out bundle))
            {
                //�Ѿ�׼��ж�أ�������������Ҫʹ�ã�ֱ��ȡ��
                if (bundle.reference == 0)
                {
                    unloadList.Remove(bundle);
                }
                bundle.AddReference();

                return bundle;
            }

            //����Bundle
            if (async)
            {
                bundle = new BundleAsync();
                bundle.url = url;
                asyncList.Add(bundle as BundleBaseAsync);
            }
            else
            {
                bundle = new Bundle();
                bundle.url = url;
            }
            bundleDic.Add(url, bundle);

            //��������
            string[] dependencies = assetBundleManifest.GetDirectDependencies(url);
            if (dependencies.Length > 0)//�����url�������ͽ������
            {
                bundle.dependencies = new BundleBase[dependencies.Length];
                for (int i = 0; i < dependencies.Length; i++)
                {
                    string dependencyUrl = dependencies[i];
                    BundleBase dependencyBundle = LoadInternal(dependencyUrl, async);
                    bundle.dependencies[i] = dependencyBundle;
                }
            }

            bundle.AddReference();
            bundle.Load();//**ʵ��bundle��Load()**

            return bundle;
        }

        internal void UnLoad(BundleBase bundle)
        {
            if (bundle == null)
            {
                MLog.Print($"{nameof(MBundleManager)}.{nameof(UnLoad)}��Bundle�����ڣ�����", MLogType.Error);
            }

            //��Bundle��reference--�����Ϊ0(û������)������׼��ж��
            bundle.ReduceReference();
            if (bundle.reference == 0)
            {
                WillUnload(bundle);
            }
        }

        private void WillUnload(BundleBase bundle)
        {
            unloadList.AddLast(bundle);
        }

        internal string GetFileUrl(string url)
        {
            if (getFileCallback == null)
            {
                MLog.Print($"{nameof(MBundleManager)}.{nameof(GetFileUrl)}����ȡ·���ص�Ϊ�գ�����", MLogType.Error);
            }

            //�����ⲿ����
            return getFileCallback.Invoke(url);
        }
    }
}
