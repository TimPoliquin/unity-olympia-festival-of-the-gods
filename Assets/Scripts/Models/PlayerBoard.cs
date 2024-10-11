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
using Azul.Event;

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
        [RequireComponent(typeof(MilestoneController))]
        public class PlayerBoard : MonoBehaviour
        {
            [SerializeField] private CircularLayout outerRing;
            [SerializeField] private GameObject center;
            [SerializeField] private DrawnTilesContainer drawnTilesContainer;
            [SerializeField] private RewardController rewardController;
            private int playerNumber;
            private List<Altar> stars = new();

            public void SetupGame(int playerNumber, AltarFactory starController)
            {
                this.playerNumber = playerNumber;
                this.gameObject.name = $"Player Board {this.playerNumber + 1}";
                this.CreateStars(starController);
                this.rewardController.SetupGame(this.playerNumber);
            }

            private void CreateStars(AltarFactory starController)
            {
                TileColor[] colors = TileColorUtils.GetAltarColors();
                List<Altar> stars = new();
                float baseRotation = 360.0f / colors.Length;
                for (int idx = 0; idx < colors.Length; idx++)
                {
                    float rotation = baseRotation * idx + 2 * baseRotation;
                    stars.Add(starController.CreateAltar(colors[idx], rotation));
                }
                Altar wildStar = starController.CreateAltar(TileColor.WILD, 0);
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

            public void AddDrawnTiles(List<Tile> tiles)
            {
                this.drawnTilesContainer.AddTiles(tiles);
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

            public int GetTileCount(TileColor tileColor)
            {
                return this.drawnTilesContainer.GetTileCount(tileColor);
            }

            public int GetTileCount()
            {
                return this.drawnTilesContainer.GetTileCount();
            }

            public List<TileCount> GetTileCounts(bool includeOneTile = false)
            {
                return this.drawnTilesContainer.GetTileCounts(includeOneTile);
            }


            public List<AltarSpace> GetOpenSpaces(TileColor tileColor)
            {
                return this.GetAltar(tileColor).GetOpenSpaces();
            }

            public List<AltarSpace> GetWildOpenSpaces()
            {
                return this.GetWildAltar().GetOpenSpaces();
            }

            public Altar GetAltar(TileColor tileColor)
            {
                return this.stars.Find(star => star.GetColor() == tileColor);
            }

            private Altar GetWildAltar()
            {
                return this.stars.Find(star => star.GetColor() == TileColor.WILD);
            }

            public List<Altar> GetAltars()
            {
                return this.stars;
            }

            public List<TileColor> GetWildTileColors()
            {
                return this.GetWildAltar().GetTileColors();
            }

            public bool IsTileNumberFilledOnAllAltars(int tileNumber)
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

            public Tile DiscardOneTile()
            {
                return this.drawnTilesContainer.DiscardOneTile();
            }

            public void ResizeForScoring()
            {
                //this.drawnTilesContainer.transform.localScale = .66f * Vector3.one;
            }

            public void ResizeForDrawing()
            {
                this.drawnTilesContainer.transform.localScale = Vector3.one;
            }

            public void AddOnPlayerBoardEarnRewardListener(UnityAction<EventTracker<OnPlayerBoardEarnRewardPayload>> listener)
            {
                this.rewardController.AddOnPlayerBoardEarnRewardListener(listener);
            }

            public RewardController GetRewardController()
            {
                return this.rewardController;
            }

            public MilestoneController GetMilestoneController()
            {
                return this.GetComponent<MilestoneController>();
            }
        }
    }
}
