using System;
using System.Collections;
using System.Collections.Generic;
using Azul.Model;
using Azul.OptionsEvents;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Azul
{
    namespace Controller
    {
        public class OptionsPanelUIController : MonoBehaviour
        {
            private OptionsPanelUI optionsPanelUI;
            void Update()
            {
                if (Keyboard.current.escapeKey.wasPressedThisFrame)
                {
                    if (null == this.optionsPanelUI)
                    {
                        this.ShowOptions();
                    }
                    else
                    {
                        this.HideOptions();
                    }
                }
            }

            private void ShowOptions()
            {
                System.Instance.Pause();
                GraphicsSettingsController graphicsSettingsController = System.Instance.GetGraphicsSettingsController();
                this.optionsPanelUI = System.Instance.GetPrefabFactory().CreateOptionsPanelUI();
                this.optionsPanelUI.AddOnGraphicsOptionChangeListener(this.OnGraphicsOptionChange);
                this.optionsPanelUI.AddOnGraphicsAntiAliasingChangeListener(this.OnAntiAliasingChange);
                this.optionsPanelUI.AddOnGraphicsRenderScaleChangeListener(this.OnRenderScaleChange);
                this.optionsPanelUI.AddOnGraphicsVSyncChangeListener(this.OnVsyncChange);
                this.optionsPanelUI.AddOnMusicVolumeChangeListener(this.OnMusicVolumeChange);
                this.optionsPanelUI.AddOnSoundVolumeChangeListener(this.OnSoundVolumeChange);
                this.optionsPanelUI.AddOnReturnToGameListener(this.HideOptions);
                this.optionsPanelUI.AddOnQuitListener(this.OnQuit);
                this.optionsPanelUI.SetGraphicsLevel(System.Instance.GetGraphicsSettingsController().GetQualityLevel());
                this.optionsPanelUI.SetAntiAliasing(graphicsSettingsController.GetAntiAliasingLevel());
                this.optionsPanelUI.SetVSync(graphicsSettingsController.GetVSync());
                this.optionsPanelUI.SetRenderScale(graphicsSettingsController.GetRenderScale());
                this.optionsPanelUI.SetBGMVolume(System.Instance.GetAudioController().GetBGMVolume());
                this.optionsPanelUI.SetSFXVolume(System.Instance.GetAudioController().GetSFXVolume());
            }

            private void OnVsyncChange(OnGraphicsVSyncChangePayload payload)
            {
                System.Instance.GetGraphicsSettingsController().SetVsync(payload.VSync);
            }

            private void OnRenderScaleChange(OnGraphicsRenderScaleChangePayload payload)
            {
                System.Instance.GetGraphicsSettingsController().SetRenderScale(payload.RenderScale);
            }

            private void OnAntiAliasingChange(OnGraphicsAntiAliasingChangePayload payload)
            {
                System.Instance.GetGraphicsSettingsController().SetAntiAliasing(payload.AntiAliasing);
            }

            private void HideOptions()
            {
                System.Instance.GetGraphicsSettingsController().SaveGraphicsConfig();
                System.Instance.GetAudioController().SaveAudioConfig();
                Destroy(this.optionsPanelUI.gameObject);
                this.optionsPanelUI = null;
                System.Instance.Resume();
            }

            private void OnGraphicsOptionChange(OnGraphicsOptionChangePayload payload)
            {
                GraphicsSettingsController graphicsSettingsController = System.Instance.GetGraphicsSettingsController();
                graphicsSettingsController.SetQualityLevel(payload.Level);
                this.optionsPanelUI.SetGraphicsLevel(payload.Level);
            }


            private void OnSoundVolumeChange(OnSoundVolumeChangePayload payload)
            {
                System.Instance.GetAudioController().SetSFXVolume(payload.SoundVolume);
            }

            private void OnMusicVolumeChange(OnMusicVolumeChangePayload payload)
            {
                System.Instance.GetAudioController().SetBGMVolume(payload.MusicVolume);
            }

            private void OnQuit()
            {
#if UNITY_EDITOR
                EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }
        }
    }
}
