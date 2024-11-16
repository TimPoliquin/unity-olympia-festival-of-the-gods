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

            public CoroutineResult FadeToBlack(float time)
            {
                CoroutineResult result = CoroutineResult.Single();
                this.image.color = new Color(0, 0, 0, 0);
                this.StartCoroutine(this.Fade(1, time, result));
                return result;
            }

            public CoroutineResult FadeIn(float time)
            {
                CoroutineResult result = CoroutineResult.Single();
                this.StartCoroutine(this.Fade(0, time, result));
                return result;
            }

            private IEnumerator Fade(float alpha, float time, CoroutineResult result)
            {
                float progress = 0;
                float originalAlpha = this.image.color.a;
                result.Start();
                while (progress < time)
                {
                    progress += Time.deltaTime;
                    this.image.color = new Color(0, 0, 0, Mathf.Lerp(originalAlpha, alpha, progress / time));
                    yield return null;
                }
                this.image.color = new Color(0, 0, 0, alpha);
                result.Finish();
            }
        }
    }
}
