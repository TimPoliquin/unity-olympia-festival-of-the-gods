using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azul
{
    namespace Animation
    {
        [RequireComponent(typeof(CanvasGroup))]
        public class Fade : MonoBehaviour
        {
            private CanvasGroup canvasGroup;
            private bool hidden;

            [SerializeField] private float delay;
            [SerializeField] private float time;

            void Awake()
            {
                this.canvasGroup = this.GetComponent<CanvasGroup>();
            }

            public void StartHidden()
            {
                this.canvasGroup.alpha = 0;
            }

            public IEnumerator Show()
            {
                bool startedHidden = this.canvasGroup.alpha == 0;
                this.gameObject.SetActive(true);
                this.hidden = false;
                return this.Transition(startedHidden ? this.delay : 0, this.time, 1, null);
            }

            public IEnumerator Hide()
            {
                this.hidden = true;
                return this.Transition(0, this.time, 0, () =>
                {
                    this.gameObject.SetActive(false);
                });
            }

            private IEnumerator Transition(float delay, float time, float alpha, Action callback)
            {
                bool hidden = this.hidden;
                float alphaDirection = alpha < this.canvasGroup.alpha ? -1 : 1;
                yield return new WaitForSeconds(delay);
                while (hidden == this.hidden && this.canvasGroup.alpha != alpha)
                {
                    this.canvasGroup.alpha = Mathf.Clamp(this.canvasGroup.alpha + Time.deltaTime / time * alphaDirection, 0, 1);
                    yield return null;
                }
                if (hidden == this.hidden && callback != null)
                {
                    callback.Invoke();
                }
            }
        }
    }
}
