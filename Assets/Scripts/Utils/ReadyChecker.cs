using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azul.Util;
using UnityEngine;

namespace Azul
{
    namespace Utils
    {
        public interface ReadyCondition
        {
            public bool IsReady();
            public WaitUntil WaitUntilReady()
            {
                return new WaitUntil(() => this.IsReady());
            }
        }
        public class ReadyChecker : MonoBehaviour
        {
            public CoroutineStatus CheckReadiness(params ReadyCondition[] conditions)
            {
                CoroutineStatus status = CoroutineStatus.Single();
                if (!conditions.All(condition => condition.IsReady()))
                {
                    this.StartCoroutine(this.WaitUntilReadyCoroutine(status, conditions.ToList()));
                }
                else
                {
                    status.Finish();
                }
                return status;
            }

            public IEnumerator WaitUntilReadyCoroutine(CoroutineStatus status, List<ReadyCondition> conditions)
            {
                status.Start();
                int done = 0;
                while (done < 2)
                {
                    ReadyCondition notReady = conditions.Find(condition => !condition.IsReady());
                    if (null != notReady)
                    {
                        UnityEngine.Debug.Log($"Not ready...{notReady.GetType().Name}");
                        yield return notReady.WaitUntilReady();
                    }
                    else
                    {
                        UnityEngine.Debug.Log($"Ready... double checking {done}");
                        done++;
                    }
                    yield return null;
                }
                status.Finish();
            }
        }
    }
}
