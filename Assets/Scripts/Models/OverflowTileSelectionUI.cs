using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Azul.Model;
using Azul.OverflowTileSelectionEvents;
using Azul.ScoreTileSelectionUIEvent;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Azul
{
    namespace OverflowTileSelectionEvents
    {
        public class OnOverflowTileSelectionConfirmPayload
        {
            public int PlayerNumber { get; init; }
            public Dictionary<TileColor, int> TileSelections { get; init; }
        }
    }
    namespace Model
    {
        public class OverflowTileSelectionUI : MonoBehaviour
        {
            [SerializeField] private string titleTemplate = "Select up to 4 tokens to keep for the next round";
            [SerializeField] private string finalRoundTitleTemplate = "You must discard all tokens after the final round.";
            [SerializeField] private TextMeshProUGUI titleText;
            [SerializeField] private TextMeshProUGUI tilesSelectedText;
            [SerializeField] private TextMeshProUGUI tilesNeededText;
            [SerializeField] private TextMeshProUGUI pointsLostText;
            [SerializeField] private GameObject tileContainer;
            [SerializeField] private Button confirm;
            [SerializeField] private Button cancel;

            private int playerNumber;

            private int maxSelectionCount;

            private List<ScoreTileSelectionUI> scoreTileSelectionUIs;

            private UnityEvent<OnOverflowTileSelectionConfirmPayload> onOverflowTileSelectionConfirm = new();
            private UnityEvent onCancel = new();

            void Awake()
            {
                this.confirm.onClick.AddListener(this.OnConfirm);
                this.cancel.onClick.AddListener(this.OnCancel);
            }

            public void SetPlayerNumber(int playerNumber)
            {
                this.playerNumber = playerNumber;
            }

            public void SetMaxSelectionCount(int requiredSelectionCount)
            {
                this.maxSelectionCount = requiredSelectionCount;
                this.tilesNeededText.text = $"{requiredSelectionCount}";
                if (requiredSelectionCount > 0)
                {
                    this.titleText.text = this.titleTemplate;
                }
                else
                {
                    this.titleText.text = this.finalRoundTitleTemplate;
                }
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
                this.pointsLostText.text = $"{numNotSelected}";
            }

            private void EnableAddButtons()
            {
                foreach (ScoreTileSelectionUI scoreTileSelectionUI in this.scoreTileSelectionUIs)
                {
                    scoreTileSelectionUI.EnableAddButton();
                }
            }

            private void DisableAddButtons()
            {
                foreach (ScoreTileSelectionUI scoreTileSelectionUI in this.scoreTileSelectionUIs)
                {
                    scoreTileSelectionUI.DisableAddButton();
                }
            }

            private void OnConfirm()
            {
                Dictionary<TileColor, int> tileSelections = new();
                foreach (ScoreTileSelectionUI scoreTileSelectionUI in this.scoreTileSelectionUIs)
                {
                    if (scoreTileSelectionUI.GetSelectedCount() > 0)
                    {
                        tileSelections[scoreTileSelectionUI.GetColor()] = scoreTileSelectionUI.GetSelectedCount();
                    }
                }
                this.onOverflowTileSelectionConfirm.Invoke(
                    new OnOverflowTileSelectionConfirmPayload
                    {
                        PlayerNumber = this.playerNumber,
                        TileSelections = tileSelections
                    }
                );
            }

            private void OnCancel()
            {
                this.onCancel.Invoke();
            }

            public void AddOnConfirmListener(UnityAction<OnOverflowTileSelectionConfirmPayload> listener)
            {
                this.onOverflowTileSelectionConfirm.AddListener(listener);
            }

            public void AddOnCancelListener(UnityAction listener)
            {
                this.onCancel.AddListener(listener);
            }
        }

    }
}
