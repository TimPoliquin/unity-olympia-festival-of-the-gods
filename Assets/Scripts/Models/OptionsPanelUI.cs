using System;
using System.Collections.Generic;
using Azul.GraphicsSettings;
using Azul.OptionsEvents;
using log4net.Core;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Azul
{
    namespace OptionsEvents
    {
        public struct OnGraphicsOptionChangePayload
        {
            public GraphicsLevel Level { get; init; }
        }
        public struct OnGraphicsAntiAliasingChangePayload
        {
            public AntiAliasingLevel AntiAliasing { get; init; }
        }
        public struct OnGraphicsVSyncChangePayload
        {
            public bool VSync { get; init; }
        }
        public struct OnGraphicsRenderScaleChangePayload
        {
            public float RenderScale { get; init; }
        }
    }
    namespace Model
    {
        [Serializable]
        public struct GraphicsOptionButton
        {
            public GraphicsLevel Level;
            public Button Button;

        }
        public class OptionsPanelUI : MonoBehaviour
        {
            [SerializeField] private List<GraphicsOptionButton> graphicsOptionButtons;
            [SerializeField] private Slider renderScaleSlider;
            [SerializeField] private Toggle vsyncToggle;
            [SerializeField] private TMP_Dropdown antiAliasingDropdown;
            [SerializeField] private Button okButton;
            [SerializeField] private Button quitButton;
            private UnityEvent<OnGraphicsOptionChangePayload> onGraphicsOptionChange = new();
            private UnityEvent<OnGraphicsAntiAliasingChangePayload> onGraphicsAntiAliasingChange = new();
            private UnityEvent<OnGraphicsRenderScaleChangePayload> onGraphicsRenderScaleChange = new();
            private UnityEvent<OnGraphicsVSyncChangePayload> onGraphicsVSyncChange = new();
            private UnityEvent onOk = new();
            private UnityEvent onQuit = new();

            void Awake()
            {
                this.Awake_GraphicsOptions();
                this.Awake_AntiAliasingOptions();
                this.vsyncToggle.onValueChanged.AddListener(this.OnVSyncChange);
                this.renderScaleSlider.onValueChanged.AddListener(this.OnRenderScaleChange);
                this.okButton.onClick.AddListener(this.OnOk);
                this.quitButton.onClick.AddListener(this.OnQuit);
            }

            void Awake_GraphicsOptions()
            {
                this.graphicsOptionButtons.ForEach(optionButton =>
                {
                    optionButton.Button.onClick.AddListener(() => this.OnGraphicsOptionClick(optionButton.Level));
                });
            }

            void Awake_AntiAliasingOptions()
            {
                this.antiAliasingDropdown.onValueChanged.AddListener(this.OnAntiAliasingChange);
            }

            public void SetVSync(bool vsync)
            {
                this.vsyncToggle.isOn = vsync;
            }

            public void SetRenderScale(float renderScale)
            {
                this.renderScaleSlider.value = renderScale;
            }

            public void SetAntiAliasing(AntiAliasingLevel level)
            {
                this.antiAliasingDropdown.value = (int)level;
            }

            public void SetGraphicsLevel(GraphicsLevel level)
            {
                this.graphicsOptionButtons.ForEach(option =>
                {
                    option.Button.interactable = level != option.Level;
                });

            }

            private void OnGraphicsOptionClick(GraphicsLevel level)
            {
                this.onGraphicsOptionChange.Invoke(new OnGraphicsOptionChangePayload
                {
                    Level = level
                });
            }

            public void AddOnGraphicsOptionChangeListener(UnityAction<OnGraphicsOptionChangePayload> listener)
            {
                this.onGraphicsOptionChange.AddListener(listener);
            }

            public void AddOnReturnToGameListener(UnityAction listener)
            {
                this.onOk.AddListener(listener);
            }

            public void AddOnQuitListener(UnityAction listener)
            {
                this.onQuit.AddListener(listener);
            }

            public void AddOnGraphicsRenderScaleChangeListener(UnityAction<OnGraphicsRenderScaleChangePayload> listener)
            {
                this.onGraphicsRenderScaleChange.AddListener(listener);
            }

            public void AddOnGraphicsVSyncChangeListener(UnityAction<OnGraphicsVSyncChangePayload> listener)
            {
                this.onGraphicsVSyncChange.AddListener(listener);
            }

            public void AddOnGraphicsAntiAliasingChangeListener(UnityAction<OnGraphicsAntiAliasingChangePayload> listener)
            {
                this.onGraphicsAntiAliasingChange.AddListener(listener);
            }

            private void OnOk()
            {
                this.onOk.Invoke();
            }

            private void OnQuit()
            {
                this.onQuit.Invoke();
            }

            private void OnAntiAliasingChange(int antiAliasing)
            {
                this.onGraphicsAntiAliasingChange.Invoke(new()
                {
                    AntiAliasing = (AntiAliasingLevel)antiAliasing
                });
            }
            private void OnVSyncChange(bool vsync)
            {
                this.onGraphicsVSyncChange.Invoke(new()
                {
                    VSync = vsync
                });
            }

            private void OnRenderScaleChange(float renderScale)
            {
                this.onGraphicsRenderScaleChange.Invoke(new()
                {
                    RenderScale = renderScale
                });
            }
        }
    }
}
