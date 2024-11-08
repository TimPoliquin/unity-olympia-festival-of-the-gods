using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Azul
{
    namespace Model
    {
        public class EndTurnPanelUI : MonoBehaviour
        {
            [SerializeField] TextMeshProUGUI carryOverText;
            [SerializeField] string carryOverTemplate = "You may carry over up to {0} tokens for the next round";
            [SerializeField] string lastRoundTemplate = "All tokens must be used or discarded in the final round";
            [SerializeField] Button endTurnButton;

            private UnityEvent onEndTurn = new();

            private void Start()
            {
                this.endTurnButton.onClick.AddListener(this.OnEndTurn);
            }

            public void Show()
            {
                if (System.Instance.GetRoundController().IsAfterLastRound())
                {
                    return;
                }
                this.gameObject.SetActive(true);
                if (System.Instance.GetRoundController().IsLastRound())
                {
                    this.carryOverText.text = this.lastRoundTemplate;
                }
                else
                {
                    this.carryOverText.text = string.Format(this.carryOverTemplate, System.Instance.GetPlayerBoardController().GetAllowedOverflow());
                }
            }

            public void Hide()
            {
                this.gameObject.SetActive(false);
            }

            public void AddOnEndTurnListener(UnityAction listener)
            {
                this.onEndTurn.AddListener(listener);
            }

            private void OnEndTurn()
            {
                this.onEndTurn.Invoke();
            }
        }
    }
}
