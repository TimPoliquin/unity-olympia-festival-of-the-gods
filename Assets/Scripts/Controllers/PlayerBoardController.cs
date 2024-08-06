using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azul.Layout;
using Azul.Model;
using UnityEngine;

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

            public void AddDrawnTiles(int player, List<Tile> tiles)
            {
                this.playerBoards[player].AddDrawnTiles(tiles);
            }

            private void OnPlayerTurnStart(OnPlayerTurnStartPayload payload)
            {
                this.playerBoards.ForEach(playerBoard =>
                {
                    playerBoard.DeactivateLight();
                });
                this.playerBoards[payload.PlayerNumber].ActivateLight();
            }

        }
    }
}
