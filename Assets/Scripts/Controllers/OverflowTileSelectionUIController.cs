using System.Collections;
using System.Collections.Generic;
using Azul.Controller.TableUtilities;
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
                IconUIFactory iconUIFactory = System.Instance.GetUIController().GetIconUIFactory();
                foreach (KeyValuePair<TileColor, int> tileCount in tileCounts)
                {
                    TileColor tileColor = tileCount.Key;
                    int count = tileCount.Value;
                    ScoreTileSelectionUI scoreTileSelectionUI = Instantiate(this.scoreTileSelectionUIPrefab, this.overflowTileSelectionUI.transform).GetComponent<ScoreTileSelectionUI>();
                    scoreTileSelectionUI.SetColor(tileColor, iconUIFactory.GetIcon(tileColor), iconUIFactory.GetBackgroundColor(tileColor));
                    scoreTileSelectionUI.SetCounterRange(0, count, count);
                    scoreTileSelectionUI.SetDefaultValue(0);
                    scoreTileSelectionUIs.Add(scoreTileSelectionUI);
                }
                this.overflowTileSelectionUI.SetScoreTileSelectionUIs(scoreTileSelectionUIs);
                this.overflowTileSelectionUI.SetMaxSelectionCount(playerBoardController.GetAllowedOverflow());
                this.overflowTileSelectionUI.SetPlayerNumber(payload.PlayerNumber);
                this.overflowTileSelectionUI.AddOnConfirmListener(this.OnOverflowKeepSelection);
                this.overflowTileSelectionUI.AddOnCancelListener(this.OnCancel);
            }

            private void OnCancel()
            {
                Destroy(this.overflowTileSelectionUI.gameObject);
                this.overflowTileSelectionUI = null;
                this.onCancel.Invoke();
            }

            private void OnOverflowKeepSelection(OnOverflowTileSelectionConfirmPayload payload)
            {
                PlayerBoardController playerBoardController = System.Instance.GetPlayerBoardController();
                PlayerBoard playerBoard = playerBoardController.GetPlayerBoard(payload.PlayerNumber);
                Dictionary<TileColor, int> tilesToDiscard = new();
                foreach (TileCount tileCount in playerBoard.GetTileCounts())
                {
                    if (payload.TileSelections.ContainsKey(tileCount.TileColor))
                    {
                        tilesToDiscard[tileCount.TileColor] = tileCount.Count - payload.TileSelections[tileCount.TileColor];
                    }
                    else
                    {
                        tilesToDiscard[tileCount.TileColor] = tileCount.Count;
                    }
                }
                playerBoardController.DiscardTiles(payload.PlayerNumber, tilesToDiscard);
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
