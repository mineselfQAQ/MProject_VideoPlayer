using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

namespace MFramework
{
    public enum AudioSourceMode
    {
        Music,
        SFX,
        Custom
    }

    /// <summary>
    /// AudioSource��չ��ʹ�ð�װ��ʵ��
    /// </summary>
    [DisallowComponent(typeof(AudioSource))]
    public class MAudioSource : MonoBehaviour
    {
        [SerializeField]
        public UnityEvent OnStart;

        [SerializeField]
        public AudioSourceMode mode = AudioSourceMode.SFX;

        [SerializeField]
        public AudioClip audioClip;
        [SerializeField]
        public AudioMixerGroup audioMixerGroup;

        [Header("Initial Setup")]
        [SerializeField] public bool mute;
        [SerializeField] public bool playOnAwake;
        [SerializeField] public bool loop;
        [SerializeField] public bool fadeInOut;
        [SerializeField] public float fadeInTime = 5.0f;
        [SerializeField] public float fadeOutTime = 5.0f;

        [Space(10)]

        [SerializeField][Range(0, 256)]
        public int priority = 128;
        [SerializeField][Range(0, 1)]
        public float volume = 1;
        [SerializeField][Range(0, 3)]
        public float pitch = 1;

        /// <summary>
        /// ����Output����
        /// </summary>
        public static Func<string, AudioMixerGroup> OnSetOutput;

        protected AudioSource audioSource;

        protected float maxVolume;

        protected bool trigger = true;
        protected bool hasFadeIn => fadeInTime != 0;
        protected bool hasFadeOut => fadeOutTime != 0;

        protected virtual void Awake()
        {
#if UNITY_EDITOR
            hideFlags = HideFlags.NotEditable;
#endif
            maxVolume = volume;

            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = audioClip;
            audioSource.outputAudioMixerGroup = audioMixerGroup;
            audioSource.mute = mute;
            audioSource.playOnAwake = false;
            //audioSource.loop = loop;//�ֶ�loop����ֹ���뽥������
            audioSource.priority = priority;
            audioSource.volume = volume;
            audioSource.pitch = pitch;

            if (fadeInOut && fadeInTime + fadeOutTime >= audioClip.length) 
            {
                MLog.Print($"{typeof(MAudioSource)}��{name}��AudioClipʱ������{fadeInTime + fadeOutTime}�룬�޷����뽥��", MLogType.Warning);
                fadeInOut = false;
            }

            if (playOnAwake && !loop)//loop������������Ӱ���߼�
            {
                audioSource.Play();
            }

            OnStart?.Invoke();
            if (audioMixerGroup)
            {
                var group = OnSetOutput?.Invoke(audioMixerGroup.name);
                if (group) audioSource.outputAudioMixerGroup = group;
            }
        }

        protected virtual void Update()
        {
            LoopPlay();
        }

        public void PlayOneShot(AudioClip clip)
        {
            if (audioSource)
            {
                audioSource.PlayOneShot(clip);
            }
        }

        private void LoopPlay()
        {
            if (loop)
            {
                if (fadeInOut)//ѭ��+���뵭�����
                {
                    if (!audioSource.isPlaying && trigger)
                    {
                        audioSource.volume = 0;
                        audioSource.Play();

                        if (hasFadeIn)//����
                        {
                            MTween.UnscaledDoTween01NoRecord((f) =>
                            {
                                audioSource.volume = f * maxVolume;
                            }, MCurve.Linear, fadeInTime);
                        }
                        if (hasFadeOut)//����
                        {
                            MCoroutineManager.Instance.DelayNoRecord(() =>
                            {
                                MTween.UnscaledDoTween01NoRecord((f) =>
                                {
                                    audioSource.volume = 1 - (f * maxVolume);
                                    trigger = false;
                                }, MCurve.Linear, fadeOutTime, () => { trigger = true; });
                            }, audioClip.length - fadeOutTime);
                        }
                    }
                }
                else//ѭ�����
                {
                    if (!audioSource.isPlaying)
                    {
                        audioSource.Play();
                    }
                }
            }
        }
    }
}
