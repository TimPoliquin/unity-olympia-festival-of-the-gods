using System;
using System.Collections;
using System.Collections.Generic;
using Azul.AudioSettings;
using Azul.Model;
using Azul.Util;
using Azul.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace Azul
{
    namespace AudioSettings
    {
        [Serializable]
        public class AudioOptions
        {
            public float BGMVolume;
            public float SFXVolume;
        }
    }
    namespace Controller
    {
        public class AudioController : MonoBehaviour
        {
            private static readonly string SETTINGS_FILENAME = "audio.dat";

            [SerializeField] private AudioMixerGroup sfxOutput;
            [SerializeField] private AudioMixerGroup bgmOutput;
            private AudioSource bgm;
            private bool isReady = false;

            void Start()
            {
                this.StartCoroutine(this.LoadConfig());
                this.bgm = this.CreateAudioSource("BGM", this.bgmOutput);
            }

            public void PlayBGM(AudioClip audioClip, float volume = .5f, bool loop = true)
            {
                this.StartCoroutine(this.PlayCoroutine(this.bgm, audioClip, volume, loop));
            }
            public void StopBGM()
            {
                this.bgm.Stop();
            }
            public void PlaySFX(AudioClip audioClip, float volume = 1.0f)
            {
                this.StartCoroutine(this.PlayCoroutine(this.CreateAudioSource(audioClip.name, this.sfxOutput), audioClip, volume, false));
            }

            public float GetBGMVolume()
            {
                float volume;
                this.bgmOutput.audioMixer.GetFloat("_BGMVolume", out volume);
                return Mathf.Pow(10, volume / 20f);
            }

            public float GetSFXVolume()
            {
                float volume;
                this.sfxOutput.audioMixer.GetFloat("_SFXVolume", out volume);
                return Mathf.Pow(10, volume / 20f);
            }

            public void SetBGMVolume(float volume)
            {
                if (volume == 0)
                {
                    this.bgmOutput.audioMixer.SetFloat("_BGMVolume", -80f);
                }
                else
                {
                    this.bgmOutput.audioMixer.SetFloat("_BGMVolume", Mathf.Log10(volume) * 20);
                }
            }

            public void SetSFXVolume(float volume)
            {
                if (volume == 0)
                {
                    this.sfxOutput.audioMixer.SetFloat("_SFXVolume", -80f);
                }
                else
                {
                    this.sfxOutput.audioMixer.SetFloat("_SFXVolume", Mathf.Log10(volume) * 20);
                }
            }

            private IEnumerator PlayCoroutine(AudioSource audioSource, AudioClip audioClip, float volume, bool loop)
            {
                yield return this.WaitUntilReady();
                audioSource.clip = audioClip;
                audioSource.volume = volume;
                audioSource.loop = loop;
                audioSource.Play();
                yield return new WaitUntil(() => audioSource.time >= audioClip.length);
                Destroy(audioSource.gameObject);
            }

            private AudioSource CreateAudioSource(string name, AudioMixerGroup mixerGroup)
            {
                GameObject go = new GameObject(name);
                go.transform.SetParent(this.transform);
                AudioSource audioSource = go.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.outputAudioMixerGroup = mixerGroup;
                return audioSource;
            }

            private IEnumerator LoadConfig()
            {
                CoroutineResultValue<AudioOptions> result = System.Instance.GetFileController().ReadFile<AudioOptions>(SETTINGS_FILENAME);
                yield return result.WaitUntilCompleted();
                if (null != result.GetValue())
                {
                    this.SetBGMVolume(result.GetValue().BGMVolume);
                    this.SetSFXVolume(result.GetValue().SFXVolume);
                }
                else
                {
                    this.SetBGMVolume(1f);
                    this.SetSFXVolume(1f);
                }
                this.isReady = true;
            }

            public void SaveAudioConfig()
            {
                System.Instance.GetFileController().WriteFile(SETTINGS_FILENAME, new AudioOptions
                {
                    BGMVolume = this.GetBGMVolume(),
                    SFXVolume = this.GetSFXVolume(),
                });
            }

            private WaitUntil WaitUntilReady()
            {
                return new WaitUntil(() => this.isReady);
            }
        }
    }
}
