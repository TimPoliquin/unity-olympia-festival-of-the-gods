using System.Collections;
using System.Collections.Generic;
using Azul.Util;
using UnityEngine;

namespace Azul
{
    namespace Animation
    {
        public class MoveAndScale : TimeBasedCoroutine
        {

            public CoroutineResult Animate(GameObject subject, Vector3 targetPosition, float scale, float totalTime)
            {
                Vector3 originalPosition = subject.transform.position;
                Vector3 originalScale = subject.transform.localScale;
                Vector3 targetScale = Vector3.one * scale;
                return this.Execute((t) =>
                {
                    subject.transform.position = Vector3.Lerp(originalPosition, targetPosition, t / totalTime);
                    subject.transform.localScale = Vector3.Lerp(originalScale, targetScale, t / totalTime);
                }, totalTime);
            }
        }

    }
}
