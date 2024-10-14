using System;
using System.Collections;
using System.Collections.Generic;
using Azul.GameEvents;
using Azul.Model;
using Azul.RoundEvents;
using Azul.Util;
using UnityEngine;
using UnityEngine.Events;

namespace Azul
{
    namespace Controller
    {
        public class RoundStartUIController : MonoBehaviour
        {
            [SerializeField] private List<ColoredValue<string>> godNames;
            [SerializeField] private float hideAfterSeconds = 3.0f;
            [SerializeField] private bool autoHide = false;

            private float activeHideDelay;

            void Start()
            {
                System.Instance.GetGameController().AddOnGameSetupCompleteListener(this.OnSetupComplete);
                if (!this.autoHide)
                {
                    this.activeHideDelay = 0;
                }
                else
                {
                    this.activeHideDelay = this.hideAfterSeconds;
                }
            }

            void OnSetupComplete(OnGameSetupCompletePayload payload)
            {
                System.Instance.GetRoundController().AddOnBeforeRoundStartListener(this.OnBeforeRoundStart);
            }

            void OnBeforeRoundStart(OnBeforeRoundStartPayload payload)
            {
                TileColor wild = payload.WildColor;
                string godName = this.godNames.Find(value => value.GetTileColor() == wild).GetValue();
                RoundStartUI roundStartUI = System.Instance.GetPrefabFactory().CreateRoundStartUI();
                roundStartUI.AddOnDismissListener(() => this.OnDismiss(roundStartUI, payload.Next));
                this.StartCoroutine(this.ShowBannerForTime(roundStartUI, godName, wild, this.activeHideDelay, payload.Next));
            }

            private void OnDismiss(RoundStartUI banner, Action callback)
            {
                this.StartCoroutine(this.OnDismissCouroutine(banner, callback));
            }
            private IEnumerator ShowBannerForTime(RoundStartUI banner, string godName, TileColor wildColor, float time, Action callback)
            {
                yield return banner.Show(godName, wildColor).WaitUntilCompleted();
                if (time > 0)
                {
                    yield return new WaitForSeconds(time);
                    yield return this.Hide(banner, callback);
                }
            }

            private IEnumerator OnDismissCouroutine(RoundStartUI banner, Action callback)
            {
                yield return this.Hide(banner, callback);
            }

            private IEnumerator Hide(RoundStartUI banner, Action callback)
            {
                if (!banner.IsDismissed())
                {
                    yield return banner.Hide().WaitUntilCompleted();
                    Destroy(banner.gameObject);
                    if (null != callback)
                    {
                        callback.Invoke();
                    }
                }

            }
        }
    }
}
