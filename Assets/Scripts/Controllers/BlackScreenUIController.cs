using System;
using System.Collections;
using Azul.Model;
using Azul.Util;
using UnityEngine;

namespace Azul
{
    namespace Controller
    {
        public class BlackScreenUIController : MonoBehaviour
        {
            private BlackScreenUI blackScreenUI;


            public CoroutineStatus FadeToBlack(float time)
            {
                if (null == this.blackScreenUI)
                {
                    this.blackScreenUI = System.Instance.GetPrefabFactory().CreateBlackScreenUI();
                    return this.blackScreenUI.FadeToBlack(time);
                }
                else
                {
                    throw new Exception("Cannot invoke a fade while another fade is occurring");
                }
            }

            public CoroutineStatus FadeIn(float time)
            {
                if (null != blackScreenUI)
                {
                    CoroutineStatus status = this.blackScreenUI.FadeIn(time);
                    this.StartCoroutine(this.DestroyOnComplete(this.blackScreenUI, status));
                    this.blackScreenUI = null;
                    return status;
                }
                else
                {
                    throw new Exception("Cannot fade in when no fade out has been performed");
                }
            }

            private IEnumerator DestroyOnComplete(BlackScreenUI blackScreenUI, CoroutineStatus status)
            {
                yield return status.WaitUntilCompleted();
                Destroy(blackScreenUI.gameObject);
            }
        }

    }
}
