using System;
using System.Collections;
using System.Collections.Generic;
using Azul.Model;
using Azul.PlayerBoardEvents;
using UnityEngine;
using UnityEngine.Events;

namespace Azul
{
    namespace Controller
    {
        public class PlayerBoardController : MonoBehaviour
        {
            [SerializeField] private GameObject playerBoardPrefab;

            private List<PlayerBoard> playerBoards;

            public void SetupGame(int numPlayers, StarController starController)
            {
                this.playerBoards = new();
                for (int idx = 0; idx < numPlayers; idx++)
                {
                    PlayerBoard board = Instantiate(this.playerBoardPrefab).GetComponent<PlayerBoard>();
                    board.gameObject.name = $"Player Board {idx + 1}";
                    board.SetPlayerNumber(idx);
                    this.CreateStars(board, starController);
                    this.playerBoards.Add(board);
                }
            }

            public void InitializeListeners()
            {
                PlayerController playerController = System.Instance.GetPlayerController();
                playerController.AddOnPlayerTurnStartListener(this.OnPlayerTurnStart);
            }

            public void CreateStars(PlayerBoard board, StarController starController)
            {
                TileColor[] colors = TileColorUtils.GetTileColors();
                List<Star> stars = new();
                for (int idx = 0; idx < colors.Length; idx++)
                {
                    stars.Add(starController.CreateStar(colors[idx]));
                }
                Star wildStar = starController.CreateStar(TileColor.WILD);
                board.AddStars(stars);
                board.AddCenterStar(wildStar);
            }

            public List<PlayerBoard> GetPlayerBoards()
            {
                return this.playerBoards;
            }

            public PlayerBoard GetPlayerBoard(int playerNumber)
            {
                return this.playerBoards[playerNumber];
            }

            public void AddDrawnTiles(int player, List<Tile> tiles)
            {
                this.playerBoards[player].AddDrawnTiles(tiles);
            }

            private void OnPlayerTurnStart(OnPlayerTurnStartPayload payload)
            {
                this.playerBoards.ForEach(playerBoard =>
                {
                    playerBoard.DeactivateLight();
                    playerBoard.DisableAllHighlights();
                });
                this.playerBoards[payload.PlayerNumber].ActivateLight();
                if (payload.Phase == Phase.SCORE)
                {
                    this.OnPlayerTurnScoringStart(payload.PlayerNumber);
                }
            }

            public int GetPlayerWithOneTile()
            {
                return this.playerBoards.Find(playerBoard => playerBoard.HasOneTile()).GetPlayerNumber();
            }

            public void AddOnPlayerAcquiresOneTileListener(UnityAction<OnPlayerAcquireOneTilePayload> listener)
            {
                this.playerBoards.ForEach(playerBoard => playerBoard.AddOnAcquireOneTileListener(listener));
            }

            private void OnPlayerTurnScoringStart(int playerNumber)
            {
                Dictionary<TileColor, int> tileColorCounts = new();
                PlayerBoard board = this.playerBoards[playerNumber];
                TileColor wildColor = System.Instance.GetRoundController().GetCurrentRound().GetWildColor();
                foreach (TileColor tileColor in TileColorUtils.GetTileColors())
                {
                    tileColorCounts[tileColor] = board.GetTileCount(tileColor);
                }
                List<TileColor> wildStarUsedColors = board.GetWildTileColors();
                foreach (KeyValuePair<TileColor, int> kvp in tileColorCounts)
                {
                    if (kvp.Value > 0)
                    {
                        int usableCount = kvp.Value + (kvp.Key == wildColor ? 0 : tileColorCounts[wildColor]);
                        List<StarSpace> spaces = board.GetOpenSpaces(kvp.Key).FindAll(space => usableCount >= space.GetValue());
                        if (!wildStarUsedColors.Contains(kvp.Key))
                        {
                            spaces.AddRange(board.GetWildOpenSpaces().FindAll(wildSpace => usableCount >= wildSpace.GetValue()));
                        }
                        spaces.ForEach(space => space.ActivateHighlight());
                    }
                }
            }
        }
    }
}
