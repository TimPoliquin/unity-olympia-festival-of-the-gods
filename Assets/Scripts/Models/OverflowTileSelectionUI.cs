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
            [SerializeField] private TextMeshProUGUI tilesSelectedText;
            [SerializeField] private TextMeshProUGUI tilesNeededText;
            [SerializeField] private TextMeshProUGUI instructions;
            [SerializeField] private GameObject tileContainer;
            [SerializeField] private Button confirm;
            [SerializeField] private Button cancel;

            private int playerNumber;

            private int requiredSelectionCount;

            private List<ScoreTileSelectionUI> scoreTileSelectionUIs;

            private UnityEvent<OnOverflowTileSelectionConfirmPayload> onOverflowTileSelectionConfirm = new();
            private UnityEvent onCancel = new();

            void Awake()
            {
                this.confirm.onClick.AddListener(this.OnConfirm);
                this.cancel.onClick.AddListener(this.OnCancel);
                this.DisableButton();
            }

            public void SetPlayerNumber(int playerNumber)
            {
                this.playerNumber = playerNumber;
            }

            public void SetRequiredSelectionCount(int requiredSelectionCount)
            {
                this.requiredSelectionCount = requiredSelectionCount;
                this.tilesNeededText.text = $"{requiredSelectionCount}";
                this.CheckRequiredSelectionCount();
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
                this.CheckRequiredSelectionCount();
            }

            private void CheckRequiredSelectionCount()
            {
                int numSelected = 0;
                foreach (ScoreTileSelectionUI scoreTileSelectionUI in this.scoreTileSelectionUIs)
                {
                    numSelected += scoreTileSelectionUI.GetSelectedCount();
                }
                if (numSelected >= this.requiredSelectionCount)
                {
                    this.EnableButton();
                }
                else
                {
                    this.DisableButton();
                }
                this.tilesSelectedText.text = $"{numSelected}";
            }

            private void EnableButton()
            {
                this.confirm.interactable = true;
            }

            private void DisableButton()
            {
                this.confirm.interactable = false;
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
