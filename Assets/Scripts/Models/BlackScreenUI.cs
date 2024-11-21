using System;
using System.Collections;
using System.Collections.Generic;
using Azul.Util;
using UnityEngine;
using UnityEngine.UI;

namespace Azul
{
    namespace Model
    {
        public class BlackScreenUI : MonoBehaviour
        {
            [SerializeField] private Image image;

            public CoroutineStatus FadeToBlack(float time)
            {
                CoroutineStatus status = CoroutineStatus.Single();
                this.image.color = new Color(0, 0, 0, 0);
                this.StartCoroutine(this.Fade(1, time, status));
                return status;
            }

            public CoroutineStatus FadeIn(float time)
            {
                CoroutineStatus status = CoroutineStatus.Single();
                this.StartCoroutine(this.Fade(0, time, status));
                return status;
            }

            private IEnumerator Fade(float alpha, float time, CoroutineStatus status)
            {
                float progress = 0;
                float originalAlpha = this.image.color.a;
                status.Start();
                while (progress < time)
                {
                    progress += Time.deltaTime;
                    this.image.color = new Color(0, 0, 0, Mathf.Lerp(originalAlpha, alpha, progress / time));
                    yield return null;
                }
                this.image.color = new Color(0, 0, 0, alpha);
                status.Finish();
            }
        }
    }
}
