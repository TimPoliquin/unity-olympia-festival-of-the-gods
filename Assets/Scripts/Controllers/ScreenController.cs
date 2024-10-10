using System.Collections;
using System.Collections.Generic;
using Azul.ScreenEvents;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Azul
{
    namespace ScreenEvents
    {
        public struct OnScreenSizeChangePayload
        {
            public int Width { get; init; }
            public int Height { get; init; }
            public float AspectRatio { get; init; }
        }
    }
    namespace Controller
    {
        public class ScreenController : MonoBehaviour
        {
            [SerializeField] private float tick = 1f / 30f;
            private UnityEvent<OnScreenSizeChangePayload> onScreenSizeChange = new();
            private bool running = false;
            void Start()
            {

                this.StartCoroutine(this.MonitorScreenSize());
            }

            public float GetAspectRatio()
            {
                return (float)Screen.width / (float)Screen.height;
            }

            private IEnumerator MonitorScreenSize()
            {
                this.running = true;
                int lastWidth = Screen.width;
                int lastHeight = Screen.height;
                while (this.running)
                {
                    if (lastWidth != Screen.width || lastHeight != Screen.height)
                    {
                        lastWidth = Screen.width;
                        lastHeight = Screen.height;
                        this.onScreenSizeChange.Invoke(new OnScreenSizeChangePayload()
                        {
                            Height = lastHeight,
                            Width = lastWidth,
                            AspectRatio = (float)lastWidth / (float)lastHeight
                        });
                    }
                    yield return new WaitForSeconds(this.tick);
                }
            }

            void OnDestroy()
            {
                this.running = false;
            }

            public void AddOnScreenSizeChangeListener(UnityAction<OnScreenSizeChangePayload> listener)
            {
                this.onScreenSizeChange.AddListener(listener);
            }
        }
    }
}
