using System.Collections;
using System.Collections.Generic;
using Azul.ClaimRewardEvents;
using Azul.Model;
using Azul.ScoreTileSelectionUIEvent;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Azul
{
    namespace ClaimRewardEvents
    {
        public struct OnClaimTileSelectionConfirmPayload
        {
            public int PlayerNumber { get; init; }
            public List<TileColor> Selections { get; init; }
        }
    }
    namespace Model
    {
        public class GrantRewardTilesUI : MonoBehaviour, ScoreTileSelectionUIContainer
        {
            [SerializeField] private TextMeshProUGUI tilesSelectedText;
            [SerializeField] private TextMeshProUGUI tilesNeededText;
            [SerializeField] private GameObject tileContainer;
            [SerializeField] private Button confirm;

            private int playerNumber;

            private int maxSelectionCount;

            private List<ScoreTileSelectionUI> scoreTileSelectionUIs;

            private UnityEvent<OnClaimTileSelectionConfirmPayload> onClaimTileSelectionConfirm = new();

            void Awake()
            {
                this.confirm.onClick.AddListener(this.OnConfirm);
            }

            public void SetPlayerNumber(int playerNumber)
            {
                this.playerNumber = playerNumber;
            }

            public void SetMaxSelectionCount(int requiredSelectionCount)
            {
                this.maxSelectionCount = requiredSelectionCount;
                this.tilesNeededText.text = $"{requiredSelectionCount}";
                this.CheckSelectionCount();
            }

            public void SetScoreTileSelectionUIs(List<ScoreTileSelectionUI> scoreTileSelectionUIs)
            {
                this.scoreTileSelectionUIs = scoreTileSelectionUIs;
                foreach (ScoreTileSelectionUI scoreTileSelectionUI in this.scoreTileSelectionUIs)
                {
                    scoreTileSelectionUI.transform.SetParent(this.tileContainer.transform);
                    scoreTileSelectionUI.AddOnSelectionCountChangeListener(this.OnSelectionCountChange);
                }
            }

            private void OnSelectionCountChange(OnSelectionCountChangePayload payload)
            {
                this.CheckSelectionCount();
            }

            private void CheckSelectionCount()
            {
                int numSelected = 0;
                int numNotSelected = 0;
                foreach (ScoreTileSelectionUI scoreTileSelectionUI in this.scoreTileSelectionUIs)
                {
                    numSelected += scoreTileSelectionUI.GetSelectedCount();
                    numNotSelected += (scoreTileSelectionUI.GetMaxCount() - scoreTileSelectionUI.GetSelectedCount());
                }
                if (numSelected >= this.maxSelectionCount)
                {
                    this.DisableAddButtons();
                }
                else
                {
                    this.EnableAddButtons();
                }
                this.tilesSelectedText.text = $"{numSelected}";
            }

            private void EnableAddButtons()
            {
                foreach (ScoreTileSelectionUI scoreTileSelectionUI in this.scoreTileSelectionUIs)
                {
                    scoreTileSelectionUI.EnableAddButton();
                }
                this.confirm.interactable = false;
            }

            private void DisableAddButtons()
            {
                foreach (ScoreTileSelectionUI scoreTileSelectionUI in this.scoreTileSelectionUIs)
                {
                    scoreTileSelectionUI.DisableAddButton();
                }
                this.confirm.interactable = true;
            }

            private void OnConfirm()
            {
                List<TileColor> tileSelections = new();
                foreach (ScoreTileSelectionUI scoreTileSelectionUI in this.scoreTileSelectionUIs)
                {
                    for (int idx = 0; idx < scoreTileSelectionUI.GetSelectedCount(); idx++)
                    {
                        tileSelections.Add(scoreTileSelectionUI.GetColor());
                    }
                }
                this.onClaimTileSelectionConfirm.Invoke(
                    new OnClaimTileSelectionConfirmPayload
                    {
                        PlayerNumber = this.playerNumber,
                        Selections = tileSelections
                    }
                );
            }

            public void AddOnConfirmListener(UnityAction<OnClaimTileSelectionConfirmPayload> listener)
            {
                this.onClaimTileSelectionConfirm.AddListener(listener);
            }

            public void AddScoreTileSelectionUI(ScoreTileSelectionUI ui)
            {
                ui.transform.SetParent(this.tileContainer.transform);
            }
        }
    }
}
