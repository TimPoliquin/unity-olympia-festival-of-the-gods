using System.Collections;
using System.Collections.Generic;
using Azul.Model;
using Azul.OverflowTileSelectionEvents;
using Azul.PlayerEvents;
using UnityEngine;

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

            public void InitializeListeners()
            {
                PlayerController playerController = System.Instance.GetPlayerController();
                playerController.AddOnPlayerBoardExceedsOverflowListener(this.OnOverflow);
            }

            private void OnOverflow(OnPlayerBoardExceedsOverflowPayload payload)
            {
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
                    scoreTileSelectionUI.SetCounterRange(0, count);
                    scoreTileSelectionUI.SetDefaultValue(0);
                    scoreTileSelectionUIs.Add(scoreTileSelectionUI);
                }
                this.overflowTileSelectionUI.SetScoreTileSelectionUIs(scoreTileSelectionUIs);
                this.overflowTileSelectionUI.SetRequiredSelectionCount(playerBoard.GetTileCount() - playerBoardController.GetAllowedOverflow());
                this.overflowTileSelectionUI.SetPlayerNumber(payload.PlayerNumber);
                this.overflowTileSelectionUI.AddOnConfirmListener(this.OnOverflowDiscardSelection);
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
        }

    }
}
