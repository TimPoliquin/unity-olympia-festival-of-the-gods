using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azul.Layout;
using Azul.Model;
using Azul.PlayerBoardEvents;
using Azul.Utils;
using Azul.Controller;
using UnityEngine;
using UnityEngine.Events;
using Azul.PlayerBoardRewardEvents;
using System;
using Azul.Controller.TableUtilities;

namespace Azul
{
    namespace PlayerBoardEvents
    {
        public class OnPlayerAcquireOneTilePayload
        {
            public int PlayerNumber { get; init; }
            public List<Tile> AcquiredTiles { get; init; }
        }
    }
    namespace Model
    {
        public class PlayerBoard : MonoBehaviour
        {
            [SerializeField] private CircularLayout outerRing;
            [SerializeField] private GameObject center;
            [SerializeField] private DrawnTilesContainer drawnTilesContainer;
            [SerializeField] private Light activePlayerLight;
            [SerializeField] private RewardController rewardController;
            private int playerNumber;
            private List<Altar> stars = new();

            private UnityEvent<OnPlayerAcquireOneTilePayload> onAcquireOneTile = new();

            public void SetupGame(int playerNumber, AltarFactory starController)
            {
                this.playerNumber = playerNumber;
                this.gameObject.name = $"Player Board {this.playerNumber + 1}";
                this.CreateStars(starController);
                this.rewardController.SetupGame(this.playerNumber);
            }

            private void CreateStars(AltarFactory starController)
            {
                TileColor[] colors = TileColorUtils.GetTileColors();
                List<Altar> stars = new();
                for (int idx = 0; idx < colors.Length; idx++)
                {
                    stars.Add(starController.CreateAltar(colors[idx]));
                }
                Altar wildStar = starController.CreateAltar(TileColor.WILD);
                this.AddStars(stars);
                this.AddCenterStar(wildStar);
            }

            public void AddStars(List<Altar> stars)
            {
                this.outerRing.AddChildren(stars.Select(star => star.gameObject).ToList());
                this.stars.AddRange(stars);
            }

            public void AddCenterStar(Altar star)
            {
                star.transform.SetParent(this.center.transform);
                star.transform.localPosition = Vector3.zero;
                this.stars.Add(star);
            }

            public void ActivateLight()
            {
                this.activePlayerLight.gameObject.SetActive(true);
            }

            public void DeactivateLight()
            {
                this.activePlayerLight.gameObject.SetActive(false);
            }

            public void AddDrawnTiles(List<Tile> tiles)
            {
                this.drawnTilesContainer.AddTiles(tiles);
                if (tiles.Any(tile => tile.IsOneTile()))
                {
                    this.onAcquireOneTile.Invoke(new OnPlayerAcquireOneTilePayload { PlayerNumber = this.playerNumber, AcquiredTiles = tiles });
                }
            }

            public List<Tile> UseTiles(TileColor mainColor, int mainCount, TileColor wildColor, int wildCount)
            {
                return this.drawnTilesContainer.UseTiles(mainColor, mainCount, wildColor, wildCount);
            }

            public int GetPlayerNumber()
            {
                return this.playerNumber;
            }

            public void SetPlayerNumber(int playerNumber)
            {
                this.playerNumber = playerNumber;
            }

            public bool HasOneTile()
            {
                return this.drawnTilesContainer.HasOneTile();
            }

            public void AddOnAcquireOneTileListener(UnityAction<OnPlayerAcquireOneTilePayload> listener)
            {
                this.onAcquireOneTile.AddListener(listener);
            }

            public int GetTileCount(TileColor tileColor)
            {
                return this.drawnTilesContainer.GetTileCount(tileColor);
            }

            public int GetTileCount()
            {
                return this.drawnTilesContainer.GetTileCount();
            }

            public List<TileCount> GetTileCounts()
            {
                return this.drawnTilesContainer.GetTileCounts();
            }

            public List<AltarSpace> GetOpenSpaces(TileColor tileColor)
            {
                return this.GetStar(tileColor).GetOpenSpaces();
            }

            public List<AltarSpace> GetWildOpenSpaces()
            {
                return this.GetWildStar().GetOpenSpaces();
            }

            public Altar GetStar(TileColor tileColor)
            {
                return this.stars.Find(star => star.GetColor() == tileColor);
            }

            private Altar GetWildStar()
            {
                return this.stars.Find(star => star.GetColor() == TileColor.WILD);
            }

            public List<TileColor> GetWildTileColors()
            {
                return this.GetWildStar().GetTileColors();
            }

            public bool IsTileNumberFilledOnAllStars(int tileNumber)
            {
                return this.stars.All(star => star.IsSpaceFilled(tileNumber));
            }

            public bool IsSpaceFilled(TileColor color, int tileNumber)
            {
                return this.stars.Find(star => star.GetColor() == color).IsSpaceFilled(tileNumber);
            }

            public void DisableAllHighlights()
            {
                foreach (Altar star in this.stars)
                {
                    star.DisableAllHighlights();
                }
            }

            public void ClearTileEventListeners()
            {
                foreach (Altar star in this.stars)
                {
                    star.ClearTileEventListeners();
                }
            }

            public GameObject GetDrawnTilesContainer(TileColor tileColor)
            {
                return this.drawnTilesContainer.GetTileContainer(tileColor);
            }

            public List<Tile> DiscardRemainingTiles()
            {
                return this.drawnTilesContainer.DiscardRemainingTiles();
            }

            public void ResizeForScoring()
            {
                //this.drawnTilesContainer.transform.localScale = .66f * Vector3.one;
            }

            public void ResizeForDrawing()
            {
                this.drawnTilesContainer.transform.localScale = Vector3.one;
            }

            public void AddOnPlayerBoardEarnRewardListener(UnityAction<OnPlayerBoardEarnRewardPayload> listener)
            {
                this.rewardController.AddOnPlayerBoardEarnRewardListener(listener);
            }
        }
    }
}
