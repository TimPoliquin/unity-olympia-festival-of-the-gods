using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Azul
{
    namespace Controller
    {
        public class AudioController : MonoBehaviour
        {
            [SerializeField] private AudioMixerGroup sfxOutput;
            public void Play(AudioClip audioClip, float volume = 1.0f)
            {
                this.StartCoroutine(this.PlayCoroutine(audioClip, volume));
            }

            private IEnumerator PlayCoroutine(AudioClip audioClip, float volume = 1.0f)
            {
                AudioSource audioSource = this.CreateAudioSource(audioClip.name);
                audioSource.clip = audioClip;
                audioSource.volume = volume;
                audioSource.Play();
                yield return null;
                while (audioSource.isPlaying)
                {
                    yield return null;
                }
                Destroy(audioSource.gameObject);
            }

            private AudioSource CreateAudioSource(string name)
            {
                GameObject go = new GameObject(name);
                go.transform.SetParent(this.transform);
                AudioSource audioSource = go.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.outputAudioMixerGroup = this.sfxOutput;
                return audioSource;
            }

        }
    }
}
