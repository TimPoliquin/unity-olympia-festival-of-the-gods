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

            void Start()
            {
                System.Instance.GetGameController().AddOnGameSetupCompleteListener(this.OnSetupComplete);
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
                this.StartCoroutine(this.ShowBannerForTime(roundStartUI, godName, wild, this.hideAfterSeconds, payload.Next));
            }
            private IEnumerator ShowBannerForTime(RoundStartUI banner, string godName, TileColor wildColor, float time, Action callback)
            {
                yield return banner.Show(godName, wildColor).WaitUntilCompleted();
                yield return new WaitForSeconds(time);
                yield return banner.Hide().WaitUntilCompleted();
                Destroy(banner.gameObject);
                callback.Invoke();
            }
        }
    }
}
