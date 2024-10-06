using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        public interface CoroutineResult
        {
            public bool IsCompleted();

            public bool IsRunning();

            public void Start();

            public void Finish();

            public static CoroutineResult Single()
            {
                return new SingleCoroutineResult();
            }

            public static CoroutineResult Multi(List<CoroutineResult> results)
            {
                return new MultiCoroutineResult(results);
            }
            public static CoroutineResult Multi(params CoroutineResult[] results)
            {
                return new MultiCoroutineResult(results.ToList());
            }
            public WaitUntil WaitUntilCompleted()
            {
                return new WaitUntil(() => this.IsCompleted());
            }
        }
        public class SingleCoroutineResult : CoroutineResult
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

        public class MultiCoroutineResult : CoroutineResult
        {
            private List<CoroutineResult> coroutineResults;
            public MultiCoroutineResult(List<CoroutineResult> results)
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
        public class TimeBasedCoroutine : MonoBehaviour
        {
            public CoroutineResult Execute(Action<float> action, float time)
            {
                CoroutineResult result = CoroutineResult.Single();
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
                action(time);
                result.Finish();
            }
        }
    }
}
