using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MFramework
{
    public class ResourceAsync : ResourceBaseAsync
    {
        public override bool keepWaiting => !done;

        private AssetBundleRequest assetBundleRequest;

        public override Object asset
        {
            get
            {
                if (done) return base.asset;

                //�����첽���ص���ԴҪ���ͬ��
                FreshAsyncAsset();

                return base.asset;
            }

            protected set
            {
                base.asset = value;
            }
        }

        public override bool Update()
        {
            if (done) return true;

            //�������ǰ�������������
            if (dependencies != null)
            {
                for (int i = 0; i < dependencies.Length; i++)
                {
                    if (!dependencies[i].done)
                    {
                        return false;
                    }
                }
            }

            //��Ҫ�ȴ�Bundle���
            if (!bundle.done) return false;

            //���ڳ��������ֱ�ӽ��㼴��
            if (bundle.assetBundle.isStreamedSceneAssetBundle)
            {
                LoadAsset();
                return true;
            }

            if (assetBundleRequest == null)
            {
                LoadAssetAsync();//�õ�assetBundleRequest(�첽��ȡ��Դ)
            }
            //�ȴ��������
            if (assetBundleRequest != null && !assetBundleRequest.isDone) return false;

            //������ɺ����ռ�����Դ
            LoadAsset();//�õ�asset

            return true;
        }

        internal override void Load()
        {
            if (string.IsNullOrEmpty(url))
            {
                MLog.Print($"{nameof(Resource)}.{nameof(Load)}��{url}Ϊ�գ�����", MLogType.Error);
            }
            if (bundle != null)
            {
                MLog.Print($"{nameof(Resource)}.{nameof(Load)}��Bundle�Ѽ��أ�����", MLogType.Error);
            }

            string bundleUrl = null;
            if (!MResourceManager.Instance.ResourceBunldeDic.TryGetValue(url, out bundleUrl))
            {
                MLog.Print($"{nameof(Resource)}.{nameof(Load)}��{bundleUrl}Ϊ�գ�����", MLogType.Error);
            }
            bundle = MBundleManager.Instance.LoadAsync(bundleUrl);//�첽����Bundle
        }

        internal override void LoadAsset()
        {
            if (bundle == null)
            {
                MLog.Print($"{nameof(ResourceAsync)}.{nameof(LoadAsset)}��BundleΪ�գ�����", MLogType.Error);
            }

            if (!bundle.isStreamedSceneAssetBundle)
            {
                if (assetBundleRequest != null)//�첽����
                {
                    asset = assetBundleRequest.asset;
                }
                else//ͬ������
                {
                    asset = bundle.LoadAsset(url, typeof(UnityEngine.Object));
                }
            }

            done = true;

            if (finishedCallback != null)
            {
                Action<ResourceBase> tempCallback = finishedCallback;
                finishedCallback = null;
                tempCallback.Invoke(this);
            }
        }

        internal override void LoadAssetAsync()
        {
            if (bundle == null)
            {
                MLog.Print($"{nameof(ResourceAsync)}.{nameof(LoadAssetAsync)}��BundleΪ�գ�����", MLogType.Error);
            }

            assetBundleRequest = bundle.LoadAssetAsync(url, typeof(UnityEngine.Object));
        }

        internal override void UnLoad()
        {
            if (bundle == null)
            {
                MLog.Print($"{nameof(Resource)}.{nameof(UnLoad)}��BundleΪ�գ�����", MLogType.Error);

            }
            if (base.asset != null && !(base.asset is GameObject))
            {
                Resources.UnloadAsset(base.asset);
                asset = null;
            }

            assetBundleRequest = null;
            MBundleManager.Instance.UnLoad(bundle);
            bundle = null;
            awaiter = null;
            finishedCallback = null;
        }

        public override T GetAsset<T>()
        {
            Object tempAsset = asset;
            Type type = typeof(T);
            if (type == typeof(Sprite))
            {
                if (asset is Sprite)
                {
                    return tempAsset as T;
                }
                else
                {
                    if (tempAsset && !(tempAsset is GameObject))
                    {
                        Resources.UnloadAsset(tempAsset);
                    }

                    asset = bundle.LoadAsset(url, type);
                    return asset as T;
                }
            }
            else
            {
                return tempAsset as T;
            }
        }
    }
}