using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azul.Model;
using Azul.PlayerBoardEvents;
using Azul.PlayerBoardRewardEvents;
using Azul.PointerEvents;
using Azul.AltarSpaceEvents;
using UnityEngine;
using UnityEngine.Events;

namespace Azul
{
    namespace PlayerBoardEvents
    {
        public class OnPlayerBoardScoreTileSelectionConfirmPayload
        {
            public Dictionary<TileColor, int> TilesSelected { get; init; }
            public TileColor Color { get; init; }
        }
        public class OnPlayerBoardScoreSpaceSelectionPayload
        {
            public int PlayerNumber { get; init; }
            public TileColor Color { get; init; }
            public int Value { get; init; }
            public PlayerBoard PlayerBoard;
            public UnityAction<OnPlayerBoardScoreTileSelectionConfirmPayload> OnConfirm { get; init; }
        }
        public class OnPlayerBoardWildScoreSpaceSelectionPayload
        {
            public int PlayerNumber { get; init; }
            public int Value { get; init; }
            public AltarSpace Space { get; init; }
            public PlayerBoard PlayerBoard;
            public UnityAction<OnPlayerBoardScoreTileSelectionConfirmPayload> OnConfirm { get; init; }
        }
        public class OnPlayerBoardPlaceStarTilePayload
        {
            public int PlayerNumber { get; init; }
            public int TilePlaced { get; init; }
            public Altar Star { get; init; }
        }
        public class OnPlayerBoardTilesDiscardedPayload
        {
            public int PlayerNumber { get; init; }
            public int NumberOfTilesDiscarded { get; init; }
        }
    }
    namespace Controller
    {
        public class PlayerBoardController : MonoBehaviour
        {
            [SerializeField] private GameObject playerBoardPrefab;
            [SerializeField] private int allowedOverflow = 4;

            private UnityEvent<OnPlayerBoardScoreSpaceSelectionPayload> onScoreSpaceSelection = new();
            private UnityEvent<OnPlayerBoardWildScoreSpaceSelectionPayload> onWildScoreSpaceSelection = new();
            private UnityEvent<OnPlayerBoardPlaceStarTilePayload> onPlaceStarTile = new();
            private UnityEvent<OnPlayerBoardTilesDiscardedPayload> onTilesDiscarded = new();
            private List<PlayerBoard> playerBoards;

            public void SetupGame(int numPlayers, AltarFactory starController)
            {
                this.playerBoards = new();
                for (int idx = 0; idx < numPlayers; idx++)
                {
                    PlayerBoard board = Instantiate(this.playerBoardPrefab).GetComponent<PlayerBoard>();
                    board.SetupGame(idx, starController);
                    this.playerBoards.Add(board);
                }
            }

            public void InitializeListeners()
            {
                RoundController roundController = System.Instance.GetRoundController();
                roundController.AddOnRoundPhaseAcquireListener(this.OnRoundAcquire);
                roundController.AddOnRoundPhasePrepareListener(this.OnRoundPrepare);
                roundController.AddOnRoundPhaseScoreListener(this.OnRoundScore);
                PlayerController playerController = System.Instance.GetPlayerController();
                playerController.AddOnPlayerTurnStartListener(this.OnPlayerTurnStart);
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
                if (System.Instance.GetRoundController().GetCurrentPhase() == Phase.SCORE)
                {
                    this.OnPlayerTurnScoringStart(player);
                }
            }

            public void DiscardTiles(int playerNumber, Dictionary<TileColor, int> tilesToDiscard)
            {
                PlayerBoard playerBoard = this.GetPlayerBoard(playerNumber);
                List<Tile> discarded = new();
                foreach (KeyValuePair<TileColor, int> discard in tilesToDiscard)
                {
                    discarded.AddRange(playerBoard.UseTiles(discard.Key, discard.Value, TileColor.WILD, 0));
                }
                int discardedCount = discarded.Count;
                BagController bagController = System.Instance.GetBagController();
                bagController.Discard(discarded);
                this.onTilesDiscarded.Invoke(new OnPlayerBoardTilesDiscardedPayload
                {
                    PlayerNumber = playerNumber,
                    NumberOfTilesDiscarded = discardedCount
                });
            }

            public void DiscardAllRemainingTiles(int playerNumber)
            {
                PlayerBoard playerBoard = this.GetPlayerBoard(playerNumber);
                List<Tile> discarded = playerBoard.DiscardRemainingTiles();
                BagController bagController = System.Instance.GetBagController();
                bagController.Discard(discarded);
                this.onTilesDiscarded.Invoke(new OnPlayerBoardTilesDiscardedPayload
                {
                    PlayerNumber = playerNumber,
                    NumberOfTilesDiscarded = discarded.Count
                });
            }

            public bool HasExcessiveOverflow(int playerNumber)
            {
                int tileCount = this.GetPlayerBoard(playerNumber).GetTileCount();
                if (System.Instance.GetRoundController().IsLastRound())
                {
                    return tileCount > 0;
                }
                else
                {
                    return tileCount > this.allowedOverflow;
                }
            }

            public int GetAllowedOverflow()
            {
                if (System.Instance.GetRoundController().IsLastRound())
                {
                    return 0;
                }
                else
                {
                    return this.allowedOverflow;
                }
            }

            private void OnPlayerTurnStart(OnPlayerTurnStartPayload payload)
            {
                this.playerBoards.ForEach(playerBoard =>
                {
                    playerBoard.DeactivateLight();
                    playerBoard.DisableAllHighlights();
                });
                if (payload.Phase == Phase.ACQUIRE)
                {
                    this.playerBoards[payload.PlayerNumber].ActivateLight();
                }
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
                        List<AltarSpace> spaces = board.GetOpenSpaces(kvp.Key).FindAll(space => usableCount >= space.GetValue());
                        if (!wildStarUsedColors.Contains(kvp.Key))
                        {
                            spaces.AddRange(board.GetWildOpenSpaces().FindAll(wildSpace => usableCount >= wildSpace.GetValue()));
                        }
                        spaces.Distinct().ToList().ForEach(space =>
                        {
                            space.ActivateHighlight();
                            space.AddOnStarSpaceSelectListener(this.OnPointerSelectSpace);
                        });
                    }
                }
            }

            private void OnPointerSelectSpace(OnStarSpaceSelectPayload payload)
            {
                // TODO - a lot needs to be done here to keep the rails on selection.
                // we should confirm the space is on an active playerboard, for instance.
                if (System.Instance.GetRoundController().GetCurrentPhase() == Phase.SCORE)
                {
                    AltarSpace space = payload.Target;
                    PlayerBoard playerBoard = space.GetComponentInParent<PlayerBoard>();
                    if (space.GetOriginColor() != TileColor.WILD)
                    {
                        this.onScoreSpaceSelection.Invoke(new OnPlayerBoardScoreSpaceSelectionPayload
                        {
                            PlayerBoard = playerBoard,
                            PlayerNumber = playerBoard.GetPlayerNumber(),
                            Color = space.GetOriginColor(),
                            Value = space.GetValue(),
                            OnConfirm = (payload) => this.OnConfirmScoreTileSelection(playerBoard, space, payload)
                        });
                    }
                    else
                    {
                        this.onWildScoreSpaceSelection.Invoke(new OnPlayerBoardWildScoreSpaceSelectionPayload
                        {
                            PlayerBoard = playerBoard,
                            PlayerNumber = playerBoard.GetPlayerNumber(),
                            Space = space,
                            Value = space.GetValue(),
                            OnConfirm = (payload) => this.OnConfirmScoreTileSelection(playerBoard, space, payload)
                        });
                    }
                }
            }

            public void AddOnPlayerBoardScoreSpaceSelectionListener(UnityAction<OnPlayerBoardScoreSpaceSelectionPayload> listener)
            {
                this.onScoreSpaceSelection.AddListener(listener);
            }
            public void RemoveOnPlayerBoardScoreSpaceSelectionListener(UnityAction<OnPlayerBoardScoreSpaceSelectionPayload> listener)
            {
                this.onScoreSpaceSelection.RemoveListener(listener);
            }

            public void AddOnPlayerBoardWildScoreSpaceSelectionListener(UnityAction<OnPlayerBoardWildScoreSpaceSelectionPayload> listener)
            {
                this.onWildScoreSpaceSelection.AddListener(listener);
            }
            public void RemoveOnPlayerBoardWildScoreSpaceSelectionListener(UnityAction<OnPlayerBoardWildScoreSpaceSelectionPayload> listener)
            {
                this.onWildScoreSpaceSelection.RemoveListener(listener);
            }

            private void OnRoundAcquire(OnRoundPhaseAcquirePayload payload)
            {
                this.playerBoards.ForEach(playerBoard => playerBoard.ResizeForDrawing());
            }

            private void OnRoundPrepare(OnRoundPhasePreparePayload payload)
            {
                this.playerBoards.ForEach(playerBoard => playerBoard.ResizeForDrawing());
            }

            private void OnRoundScore(OnRoundPhaseScorePayload payload)
            {
                this.playerBoards.ForEach(playerBoard => playerBoard.ResizeForScoring());
            }

            private void OnConfirmScoreTileSelection(PlayerBoard playerBoard, AltarSpace space, OnPlayerBoardScoreTileSelectionConfirmPayload payload)
            {
                RoundController roundController = System.Instance.GetRoundController();
                TileColor wildColor = roundController.GetCurrentRound().GetWildColor();
                TileColor spaceColor = payload.Color;
                int spaceCount = payload.TilesSelected[spaceColor];
                List<Tile> tiles;
                if (spaceColor != wildColor)
                {
                    int wildCount = payload.TilesSelected.ContainsKey(wildColor) ? payload.TilesSelected[wildColor] : 0;
                    tiles = playerBoard.UseTiles(spaceColor, payload.TilesSelected[spaceColor], wildColor, wildCount);
                }
                else
                {
                    tiles = playerBoard.UseTiles(wildColor, payload.TilesSelected[wildColor], wildColor, 0);
                }
                Tile tileToPlace = tiles.Find(tile => tile.Color == spaceColor);
                space.PlaceTile(tileToPlace);
                tiles.Remove(tileToPlace);
                System.Instance.GetBagController().Discard(tiles);
                playerBoard.DisableAllHighlights();
                playerBoard.ClearTileEventListeners();
                this.onPlaceStarTile.Invoke(new OnPlayerBoardPlaceStarTilePayload
                {
                    PlayerNumber = playerBoard.GetPlayerNumber(),
                    TilePlaced = space.GetValue(),
                    Star = playerBoard.GetStar(space.GetOriginColor())
                });
                this.OnPlayerTurnScoringStart(playerBoard.GetPlayerNumber());
            }

            public void AddOnPlaceStarTileListener(UnityAction<OnPlayerBoardPlaceStarTilePayload> listener)
            {
                this.onPlaceStarTile.AddListener(listener);
            }

            public void AddOnPlayerBoardEarnRewardListener(UnityAction<OnPlayerBoardEarnRewardPayload> listener)
            {
                this.playerBoards.ForEach(playerBoard => playerBoard.AddOnPlayerBoardEarnRewardListener(listener));
            }

            public void AddOnPlayerBoardTilesDiscardedListener(UnityAction<OnPlayerBoardTilesDiscardedPayload> listener)
            {
                this.onTilesDiscarded.AddListener(listener);
            }

            public Tile GrantReward(int playerNumber, TileColor tileColor)
            {
                BagController bagController = System.Instance.GetBagController();
                Tile grantedTile = bagController.Draw(tileColor);
                this.AddDrawnTiles(playerNumber, new() { grantedTile });
                return grantedTile;
            }

        }
    }
}
