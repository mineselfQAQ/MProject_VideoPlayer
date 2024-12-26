using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MFramework
{
    public class Resource : ResourceBase
    {
        public override bool keepWaiting => !done;

        internal override void Load()
        {
            if (string.IsNullOrEmpty(url))
            {
                MLog.Print($"{nameof(Resource)}.{nameof(Load)}��urlΪ�գ�����", MLogType.Error);
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

            bundle = MBundleManager.Instance.Load(bundleUrl);//ͬ����ȡBundle
            LoadAsset();
        }

        internal override void LoadAsset()
        {
            if (bundle == null)
            {
                MLog.Print($"{nameof(Resource)}.{nameof(LoadAsset)}��BundleΪ�գ�����", MLogType.Error);
            }

            //�����첽���ص���ԴҪ���ͬ��
            FreshAsyncAsset();

            if (bundle.isStreamedSceneAssetBundle) return;//���ڳ�������ҪLoadAsset()
            asset = bundle.LoadAsset(url, typeof(Object));

            done = true;

            if (finishedCallback != null)
            {
                Action<ResourceBase> tempCallback = finishedCallback;
                finishedCallback = null;
                tempCallback.Invoke(this);
            }
        }
        
        internal override void UnLoad()
        {
            if (bundle == null)
            {
                MLog.Print($"{nameof(Resource)}.{nameof(UnLoad)}��BundleΪ�գ�����", MLogType.Error);
            }
            if (asset != null && !(asset is GameObject))
            {
                Resources.UnloadAsset(asset);
                asset = null;
            }

            MBundleManager.Instance.UnLoad(bundle);

            bundle = null;
            awaiter = null;
            finishedCallback = null;
        }

        public override T GetAsset<T>()
        {
            Object tempAsset = asset;
            Type type = typeof(T);

            if (type == typeof(Sprite))//��ȡSprite��Դ�Ĵ���
            {
                if (asset is Sprite)//�����Դ��Sprite��ôֱ��ȡ������
                {
                    return tempAsset as T;
                }
                else//�����Դ����Sprite����Ҫ���¼���
                {
                    if (tempAsset && !(tempAsset is GameObject))
                    {
                        Resources.UnloadAsset(tempAsset);
                    }

                    asset = bundle.LoadAsset(url, type);
                    return asset as T;
                }
            }
            else//��Spriteֱ��ȡ��
            {
                return tempAsset as T;
            }
        }
    }
}