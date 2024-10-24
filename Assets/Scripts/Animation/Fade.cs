using System;
using System.Collections;
using System.Collections.Generic;
using Azul.Util;
using UnityEngine;

namespace Azul
{
    namespace Animation
    {
        [RequireComponent(typeof(CanvasGroup))]
        public class Fade : TimeBasedCoroutine
        {
            private CanvasGroup canvasGroup;
            private bool hidden;
            private CoroutineResult hiddenResult;

            [SerializeField] private float delay;
            [SerializeField] private float time = .25f;

            void Awake()
            {
                this.canvasGroup = this.GetComponent<CanvasGroup>();
            }

            public void StartHidden()
            {
                this.canvasGroup.alpha = 0;
            }

            public CoroutineResult Show()
            {
                CoroutineResult result = CoroutineResult.Single();
                bool startedHidden = this.canvasGroup.alpha == 0;
                this.gameObject.SetActive(true);
                this.hidden = false;
                this.StartCoroutine(this.Transition(startedHidden ? this.delay : 0, this.time, 1, result));
                return result;
            }

            public CoroutineResult Hide()
            {
                if (null != this.hiddenResult)
                {
                    return this.hiddenResult;
                }
                this.hiddenResult = CoroutineResult.Single();
                this.hidden = true;
                this.StartCoroutine(this.Transition(0, this.time, 0, this.hiddenResult, () =>
                {
                    this.hiddenResult = null;
                }));
                return this.hiddenResult;
            }

            private IEnumerator Transition(float delay, float time, float alpha, CoroutineResult result, Action callback = null)
            {
                result.Start();
                bool hidden = this.hidden;
                float alphaDirection = alpha < this.canvasGroup.alpha ? -1 : 1;
                yield return new WaitForSeconds(delay);
                while (hidden == this.hidden && this.canvasGroup.alpha != alpha)
                {
                    this.canvasGroup.alpha = Mathf.Clamp(this.canvasGroup.alpha + Time.deltaTime / time * alphaDirection, 0, 1);
                    yield return null;
                }
                result.Finish();
                callback?.Invoke();
            }
        }
    }
}
