using System.Collections;
using System.Collections.Generic;
using Azul.Model;
using Azul.OverflowTileSelectionEvents;
using Azul.PlayerEvents;
using UnityEngine;
using UnityEngine.Events;

namespace Azul
{
    namespace Controller
    {
        public class OverflowTileSelectionUIController : MonoBehaviour
        {
            [SerializeField] private GameObject container;
            [SerializeField] private GameObject prefab;
            [SerializeField] private GameObject scoreTileSelectionUIPrefab;


            private OverflowTileSelectionUI overflowTileSelectionUI;
            private UnityEvent onCancel = new();

            public void InitializeListeners()
            {
                PlayerController playerController = System.Instance.GetPlayerController();
                playerController.AddOnPlayerBoardExceedsOverflowListener(this.OnOverflow);
            }

            private void OnOverflow(OnPlayerBoardExceedsOverflowPayload payload)
            {
                if (System.Instance.GetPlayerController().GetPlayer(payload.PlayerNumber).IsAI())
                {
                    return;
                }
                RoundController roundController = System.Instance.GetRoundController();
                PlayerBoardController playerBoardController = System.Instance.GetPlayerBoardController();
                PlayerBoard playerBoard = playerBoardController.GetPlayerBoard(payload.PlayerNumber);
                Dictionary<TileColor, int> tileCounts = new();
                foreach (TileColor tileColor in TileColorUtils.GetTileColors())
                {
                    int count = playerBoard.GetTileCount(tileColor);
                    if (count > 0)
                    {
                        tileCounts[tileColor] = count;
                    }
                }
                this.overflowTileSelectionUI = Instantiate(this.prefab, this.container.transform).GetComponent<OverflowTileSelectionUI>();
                List<ScoreTileSelectionUI> scoreTileSelectionUIs = new();
                foreach (KeyValuePair<TileColor, int> tileCount in tileCounts)
                {
                    TileColor tileColor = tileCount.Key;
                    int count = tileCount.Value;
                    ScoreTileSelectionUI scoreTileSelectionUI = Instantiate(this.scoreTileSelectionUIPrefab, this.overflowTileSelectionUI.transform).GetComponent<ScoreTileSelectionUI>();
                    scoreTileSelectionUI.SetColor(tileColor);
                    scoreTileSelectionUI.SetAnchor(playerBoard.GetDrawnTilesContainer(tileColor));
                    if (roundController.IsLastRound())
                    {
                        // all tiles must be discarded after the last round
                        scoreTileSelectionUI.SetCounterRange(count, count);
                        scoreTileSelectionUI.SetDefaultValue(count);
                    }
                    else
                    {
                        scoreTileSelectionUI.SetCounterRange(0, count);
                        scoreTileSelectionUI.SetDefaultValue(0);
                    }
                    scoreTileSelectionUIs.Add(scoreTileSelectionUI);
                }
                this.overflowTileSelectionUI.SetScoreTileSelectionUIs(scoreTileSelectionUIs);
                if (roundController.IsLastRound())
                {
                    this.overflowTileSelectionUI.SetRequiredSelectionCount(playerBoard.GetTileCount());
                }
                else
                {
                    this.overflowTileSelectionUI.SetRequiredSelectionCount(playerBoard.GetTileCount() - playerBoardController.GetAllowedOverflow());
                }
                this.overflowTileSelectionUI.SetPlayerNumber(payload.PlayerNumber);
                this.overflowTileSelectionUI.AddOnConfirmListener(this.OnOverflowDiscardSelection);
                this.overflowTileSelectionUI.AddOnCancelListener(this.OnCancel);
            }

            private void OnCancel()
            {
                Destroy(this.overflowTileSelectionUI.gameObject);
                this.overflowTileSelectionUI = null;
                this.onCancel.Invoke();
            }

            private void OnOverflowDiscardSelection(OnOverflowTileSelectionConfirmPayload payload)
            {
                PlayerBoardController playerBoardController = System.Instance.GetPlayerBoardController();
                playerBoardController.DiscardTiles(payload.PlayerNumber, payload.TileSelections);
                Destroy(this.overflowTileSelectionUI.gameObject);
                this.overflowTileSelectionUI = null;
                PlayerController playerController = System.Instance.GetPlayerController();
                playerController.EndPlayerScoringTurn();
            }

            public void AddOnCancelListener(UnityAction listener)
            {
                this.onCancel.AddListener(listener);
            }
        }

    }
}
