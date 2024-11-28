using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Azul
{
    namespace Util
    {
        public enum CoroutineRunStatus
        {
            NOT_STARTED,
            RUNNING,
            COMPLETED
        }
        public interface CoroutineStatus
        {
            public bool IsCompleted();

            public bool IsRunning();

            public void Start();

            public void Finish();

            public static CoroutineStatus Single()
            {
                return new SingleCoroutineResult();
            }

            public static CoroutineStatus Multi(List<CoroutineStatus> results)
            {
                return new MultiCoroutineResult(results.Where(status => status != null).ToList());
            }
            public static CoroutineStatus Multi(params CoroutineStatus[] results)
            {
                return new MultiCoroutineResult(results.Where(status => status != null).ToList());
            }
            public WaitUntil WaitUntilCompleted()
            {
                return new WaitUntil(this.IsCompleted);
            }
        }

        public class SingleCoroutineResult : CoroutineStatus
        {
            private CoroutineRunStatus status = CoroutineRunStatus.NOT_STARTED;

            public bool IsCompleted()
            {
                return this.status == CoroutineRunStatus.COMPLETED;
            }

            public bool IsRunning()
            {
                return this.status == CoroutineRunStatus.RUNNING;
            }

            public void Start()
            {
                this.status = CoroutineRunStatus.RUNNING;
            }

            public void Finish()
            {
                this.status = CoroutineRunStatus.COMPLETED;
            }
        }

        public class MultiCoroutineResult : CoroutineStatus
        {
            private List<CoroutineStatus> coroutineResults;
            public MultiCoroutineResult(List<CoroutineStatus> results)
            {
                this.coroutineResults = results;
            }
            public bool IsCompleted()
            {
                return this.coroutineResults.All(result => result.IsCompleted());
            }
            public bool IsRunning()
            {
                return this.coroutineResults.Any(result => result.IsRunning());
            }
            public void Start()
            {
                this.coroutineResults.ForEach(result => result.Start());
            }
            public void Finish()
            {
                this.coroutineResults.ForEach(result => result.Finish());
            }
        }
        public class CoroutineResultValue<T> : SingleCoroutineResult
        {
            private T value;
            private bool error;
            public T GetValue()
            {
                return this.value;
            }
            public bool IsError()
            {
                return this.error;
            }

            public void Finish(T value)
            {
                this.value = value;
                this.error = false;
                base.Finish();
            }
            public void Error()
            {
                base.Finish();
            }
            public WaitUntil WaitUntilCompleted()
            {
                return new WaitUntil(() => this.IsCompleted());
            }
            public WaitUntil WaitUntilCompleted(float timeout)
            {
                float time = 0;
                return new WaitUntil(() =>
                {
                    time += Time.deltaTime;
                    return this.IsCompleted() || time > timeout;
                });
            }
        }
        public class TimeBasedCoroutine : MonoBehaviour
        {
            public CoroutineStatus Execute(Action<float> action, float time)
            {
                CoroutineStatus status = CoroutineStatus.Single();
                this.StartCoroutine(this.Run(action, time, status));
                return status;
            }

            IEnumerator Run(Action<float> action, float time, CoroutineStatus status)
            {
                status.Start();
                for (float t = 0; t < time; t += Time.deltaTime)
                {
                    action(t);
                    yield return 0;
                }
                action(time);
                status.Finish();
            }
        }
    }
}
