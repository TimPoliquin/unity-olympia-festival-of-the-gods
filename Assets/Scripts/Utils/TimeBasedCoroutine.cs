using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azul
{
    namespace Util
    {
        public enum CoroutineStatus
        {
            NOT_STARTED,
            RUNNING,
            COMPLETED
        }
        public class CoroutineResult
        {
            private CoroutineStatus status = CoroutineStatus.NOT_STARTED;

            public bool IsCompleted()
            {
                return this.status == CoroutineStatus.COMPLETED;
            }

            public bool IsRunning()
            {
                return this.status == CoroutineStatus.RUNNING;
            }

            public void Start()
            {
                this.status = CoroutineStatus.RUNNING;
            }

            public void Finish()
            {
                this.status = CoroutineStatus.COMPLETED;
            }


        }
        public class TimeBasedCoroutine : MonoBehaviour
        {
            public CoroutineResult Execute(Action<float> action, float time)
            {
                CoroutineResult result = new();
                this.StartCoroutine(this.Run(action, time, result));
                return result;
            }

            IEnumerator Run(Action<float> action, float time, CoroutineResult result)
            {
                result.Start();
                for (float t = 0; t < time; t += Time.deltaTime)
                {
                    action(t);
                    yield return 0;
                }
                result.Finish();
            }
        }
    }
}
