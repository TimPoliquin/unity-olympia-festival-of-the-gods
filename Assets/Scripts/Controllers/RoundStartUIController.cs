using System;
using System.Collections;
using System.Collections.Generic;
using Azul.GameEvents;
using Azul.Model;
using Azul.RoundEvents;
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
                RoundStartUI roundStartUI = System.Instance.GetPrefabFactory().CreateRoundStartUI();
                roundStartUI.Show(this.godNames.Find(value => value.GetTileColor() == wild).GetValue(), wild);
                this.StartCoroutine(this.HideAfter(roundStartUI, this.hideAfterSeconds, payload.Next));
            }

            IEnumerator HideAfter(RoundStartUI roundStartUI, float hideAfterSeconds, Action callback)
            {
                yield return new WaitForSeconds(hideAfterSeconds);
                roundStartUI.Hide(callback);
            }

        }
    }
}
