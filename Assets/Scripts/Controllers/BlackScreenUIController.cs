using System;
using System.Collections;
using System.Collections.Generic;
using Azul.Model;
using Azul.Util;
using NSubstitute.Core;
using UnityEngine;

namespace Azul
{
    namespace Controller
    {
        public class BlackScreenUIController : MonoBehaviour
        {
            private BlackScreenUI blackScreenUI;


            public CoroutineResult FadeToBlack(float time)
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

            public CoroutineResult FadeIn(float time)
            {
                if (null != blackScreenUI)
                {
                    CoroutineResult result = this.blackScreenUI.FadeIn(time);
                    this.StartCoroutine(this.DestroyOnComplete(this.blackScreenUI, result));
                    this.blackScreenUI = null;
                    return result;
                }
                else
                {
                    throw new Exception("Cannot fade in when no fade out has been performed");
                }
            }

            private IEnumerator DestroyOnComplete(BlackScreenUI blackScreenUI, CoroutineResult result)
            {
                yield return result.WaitUntilCompleted();
                Destroy(blackScreenUI.gameObject);
            }
        }

    }
}
