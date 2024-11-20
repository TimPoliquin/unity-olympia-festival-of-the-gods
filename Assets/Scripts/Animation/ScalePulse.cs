using System.Collections;
using System.Collections.Generic;
using Azul.Util;
using UnityEngine;

namespace Azul
{
    namespace Animation
    {
        public class ScalePulse : TimeBasedCoroutine
        {
            public CoroutineStatus Animate(GameObject subject, float scaleTo, float totalTime)
            {
                Vector3 originalScale = Vector3.one;
                Vector3 targetScale = Vector3.one * scaleTo;
                CoroutineStatus status = CoroutineStatus.Single();
                this.StartCoroutine(this.Animate(subject, originalScale, targetScale, totalTime, status));
                return status;
            }

            IEnumerator Animate(GameObject subject, Vector3 startScale, Vector3 targetScale, float totalTime, CoroutineStatus status)
            {
                status.Start();
                yield return this.Scale(subject, startScale, targetScale, totalTime / 2.0f).WaitUntilCompleted();
                yield return this.Scale(subject, targetScale, startScale, totalTime / 2.0f).WaitUntilCompleted();
                status.Finish();
            }


            CoroutineStatus Scale(GameObject subject, Vector3 startScale, Vector3 targetScale, float time)
            {
                return this.Execute((t) =>
                {
                    subject.transform.localScale = Vector3.Lerp(startScale, targetScale, t / time);
                }, time);
            }

        }
    }
}
