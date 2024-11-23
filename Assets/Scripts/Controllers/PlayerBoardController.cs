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
using Azul.TileAnimation;
using Azul.MilestoneEvents;
using Azul.Util;
using Azul.Event;
using Azul.PlayerEvents;

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
            public AltarSpace Space { get; init; }
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
        public struct OnPlayerBoardTilesCollectedPayload
        {
            public int PlayerNumber { get; init; }
            public List<ColoredValue<int>> TileCounts { get; init; }

        }
        public struct OnPlayerBoardDiscardOneTilePayload
        {
            public int PlayerNumber { get; init; }
        }
    }
    namespace Controller
    {
        public class PlayerBoardController : MonoBehaviour
        {
            [SerializeField] private GameObject playerBoardPrefab;
            [SerializeField] private int allowedOverflow = 4;
            [SerializeField] private List<AudioSource> tilePlacementSFX;

            private UnityEvent<OnPlayerBoardScoreSpaceSelectionPayload> onScoreSpaceSelection = new();
            private UnityEvent<OnPlayerBoardWildScoreSpaceSelectionPayload> onWildScoreSpaceSelection = new();
            private AzulEvent<OnPlayerBoardPlaceStarTilePayload> onPlaceStarTile = new();
            private UnityEvent<OnPlayerBoardTilesDiscardedPayload> onTilesDiscarded = new();
            private UnityEvent<OnPlayerBoardDiscardOneTilePayload> onDiscardOneTile = new();
            private UnityEvent<OnPlayerBoardTilesCollectedPayload> onTilesCollected = new();
            private UnityEvent<OnPlayerAcquireOneTilePayload> onAcquireOneTile = new();

            private List<PlayerBoard> playerBoards;
            private bool isPlacingTiles = false;

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

            public CoroutineStatus AddDrawnTiles(int player, List<Tile> tiles)
            {
                CoroutineStatus status = CoroutineStatus.Single();
                this.StartCoroutine(this.AddDrawnTilesCoroutine(this.playerBoards[player], tiles, status));
                return status;
            }

            private IEnumerator AddDrawnTilesCoroutine(PlayerBoard playerBoard, List<Tile> tiles, CoroutineStatus status)
            {
                status.Start();
                PlayerController playerController = System.Instance.GetPlayerController();
                RoundController roundController = System.Instance.GetRoundController();
                TileAnimationController tileAnimationController = System.Instance.GetTileAnimationController();
                PlayerUIController playerUIController = System.Instance.GetUIController().GetPlayerUIController();
                foreach (Tile tile in tiles)
                {
                    if (roundController.GetCurrentPhase() == Phase.ACQUIRE)
                    {
                        yield return tileAnimationController.MoveTiles(new() { tile }, new TilesMoveConfig()
                        {
                            Position = playerUIController.GetTileCountPosition(playerBoard.GetPlayerNumber(), tile.Color),
                            Time = .25f,
                            Delay = 0f,
                            AfterEach = (tile, idx) =>
                            {
                                tile.gameObject.SetActive(false);
                            }
                        }).WaitUntilCompleted();
                    }
                    playerBoard.AddDrawnTiles(new() { tile });
                    this.onTilesCollected.Invoke(new OnPlayerBoardTilesCollectedPayload
                    {
                        PlayerNumber = playerBoard.GetPlayerNumber(),
                        TileCounts = playerBoard.GetTileCounts(true).Select(tileCount => tileCount.ToColoredValue()).ToList()
                    });
                }
                if (tiles.Any(tile => tile.IsHadesToken()))
                {
                    this.onAcquireOneTile.Invoke(new OnPlayerAcquireOneTilePayload { PlayerNumber = playerBoard.GetPlayerNumber(), AcquiredTiles = tiles });
                }
                if (roundController.GetCurrentPhase() == Phase.SCORE && playerController.GetCurrentPlayer().GetPlayerNumber() == playerBoard.GetPlayerNumber())
                {
                    this.OnPlayerTurnScoringStart(playerBoard.GetPlayerNumber());
                }
                status.Finish();
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
                    playerBoard.DisableAllHighlights();
                });
                if (payload.Phase == Phase.SCORE)
                {
                    this.OnPlayerTurnScoringStart(payload.PlayerNumber);
                }
            }

            public void AddOnPlayerAcquiresOneTileListener(UnityAction<OnPlayerAcquireOneTilePayload> listener)
            {
                this.onAcquireOneTile.AddListener(listener);
            }

            private void OnPlayerTurnScoringStart(int playerNumber)
            {
                UnityEngine.Debug.Log($"PlayerTurnScoringStart {playerNumber}");
                Dictionary<TileColor, int> tileColorCounts = new();
                PlayerBoard board = this.playerBoards[playerNumber];
                TileColor wildColor = System.Instance.GetRoundController().GetCurrentRound().GetWildColor();
                board.DisableAllHighlights();
                board.ClearTileEventListeners();
                foreach (TileColor tileColor in TileColorUtils.GetTileColors())
                {
                    tileColorCounts[tileColor] = board.GetTileCount(tileColor);
                }
                List<TileColor> wildStarUsedColors = board.GetWildTileColors();
                List<AltarSpace> spaces = new();
                foreach (KeyValuePair<TileColor, int> kvp in tileColorCounts)
                {
                    if (kvp.Value > 0)
                    {
                        int usableCount = kvp.Value + (kvp.Key == wildColor ? 0 : tileColorCounts[wildColor]);
                        spaces.AddRange(board.GetOpenSpaces(kvp.Key).FindAll(space => usableCount >= space.GetValue()));
                        if (!wildStarUsedColors.Contains(kvp.Key))
                        {
                            spaces.AddRange(board.GetWildOpenSpaces().FindAll(wildSpace => usableCount >= wildSpace.GetValue()));
                        }
                    }
                }
                spaces.Distinct().ToList().ForEach(space =>
                {
                    space.ActivateHighlight();
                    space.AddOnStarSpaceSelectListener(this.OnPointerSelectSpace);
                });
            }

            public void RestoreScoringUI(int playerNumber)
            {
                this.OnPlayerTurnScoringStart(playerNumber);
            }

            public void HideScoringUI(int playerNumber)
            {
                PlayerBoard playerBoard = this.playerBoards[playerNumber];
                playerBoard.DisableAllHighlights();
                playerBoard.ClearTileEventListeners();
            }

            private void OnPointerSelectSpace(OnAltarSpaceSelectPayload payload)
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
                            Space = space,
                            Color = space.GetOriginColor(),
                            Value = space.GetValue(),
                            OnConfirm = (payload) => this.PlaceTiles(playerBoard.GetPlayerNumber(), space, payload.Color, payload.TilesSelected)
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
                            OnConfirm = (payload) => this.PlaceTiles(playerBoard.GetPlayerNumber(), space, payload.Color, payload.TilesSelected)
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
                this.playerBoards.ForEach(playerBoard =>
                {
                    playerBoard.ResizeForDrawing();
                    playerBoard.DisableAllHighlights();
                });
            }

            private void OnRoundPrepare(OnRoundPhasePreparePayload payload)
            {
                this.playerBoards.ForEach(playerBoard =>
                {
                    playerBoard.ResizeForDrawing();
                    Tile oneTile = playerBoard.DiscardOneTile();
                    if (oneTile != null)
                    {
                        System.Instance.GetTableController().MoveOneTileToCenter(oneTile);
                        this.onDiscardOneTile.Invoke(new OnPlayerBoardDiscardOneTilePayload
                        {
                            PlayerNumber = playerBoard.GetPlayerNumber()
                        });
                    }
                });
            }

            private void OnRoundScore(OnRoundPhaseScorePayload payload)
            {
                this.playerBoards.ForEach(playerBoard => playerBoard.ResizeForScoring());
            }

            public CoroutineStatus PlaceTiles(int playerNumber, AltarSpace space, TileColor spaceColor, Dictionary<TileColor, int> tilesSelected)
            {
                CoroutineStatus status = CoroutineStatus.Single();
                RoundController roundController = System.Instance.GetRoundController();
                TileColor wildColor = roundController.GetCurrentRound().GetWildColor();
                PlayerBoard playerBoard = this.GetPlayerBoard(playerNumber);
                List<Tile> tiles;
                if (spaceColor != wildColor)
                {
                    int wildCount = tilesSelected.ContainsKey(wildColor) ? tilesSelected[wildColor] : 0;
                    tiles = playerBoard.UseTiles(spaceColor, tilesSelected[spaceColor], wildColor, wildCount);
                }
                else
                {
                    tiles = playerBoard.UseTiles(wildColor, tilesSelected[wildColor], wildColor, 0);
                }
                this.StartCoroutine(this.TilePlacementCoroutine(playerBoard, tiles, space, spaceColor, status));
                return status;
            }

            private IEnumerator TilePlacementCoroutine(PlayerBoard playerBoard, List<Tile> tiles, AltarSpace space, TileColor effectiveColor, CoroutineStatus status)
            {
                status.Start();
                this.isPlacingTiles = true;
                yield return System.Instance.GetTileAnimationController().MoveTiles(tiles, new TilesMoveConfig
                {
                    Position = space.transform.position,
                    Time = .25f,
                    AfterEach = (tile, idx) =>
                    {
                        this.tilePlacementSFX[idx].Play();
                        ExplosionEffect explosionEffect = System.Instance.GetPrefabFactory().CreateExplosionEffect(space.transform);
                        explosionEffect.Play(tile.Color);
                        tile.gameObject.SetActive(false);
                    },
                }).WaitUntilCompleted();
                space.PlaceTile(effectiveColor);
                System.Instance.GetBagController().Discard(tiles);
                yield return playerBoard.GetMilestoneController().OnStarTilePlaced(playerBoard, playerBoard.GetAltar(space.GetOriginColor()), space).WaitUntilCompleted();
                yield return this.onPlaceStarTile.Invoke(new OnPlayerBoardPlaceStarTilePayload
                {
                    PlayerNumber = playerBoard.GetPlayerNumber(),
                    TilePlaced = space.GetValue(),
                    Star = playerBoard.GetAltar(space.GetOriginColor())
                }).WaitUntilCompleted();
                this.OnPlayerTurnScoringStart(playerBoard.GetPlayerNumber());
                this.isPlacingTiles = false;
                status.Finish();
            }

            public void AddOnPlaceStarTileListener(UnityAction<EventTracker<OnPlayerBoardPlaceStarTilePayload>> listener)
            {
                this.onPlaceStarTile.AddListener(listener);
            }

            public void AddOnPlayerBoardEarnRewardListener(UnityAction<EventTracker<OnPlayerBoardEarnRewardPayload>> listener)
            {
                this.playerBoards.ForEach(playerBoard => playerBoard.AddOnPlayerBoardEarnRewardListener(listener));
            }

            public void AddOnPointerEnterRewardListener(UnityAction<OnPointerEnterPayload<RewardIndicator>> listener)
            {
                this.playerBoards.ForEach(playerBoard => playerBoard.GetRewardController().AddOnPointerEnterRewardListener(listener));
            }

            public void AddOnPointerExitRewardListener(UnityAction<OnPointerExitPayload<RewardIndicator>> listener)
            {
                this.playerBoards.ForEach(playerBoard => playerBoard.GetRewardController().AddOnPointerExitRewardListener(listener));
            }
            public void AddOnPlayerBoardTilesDiscardedListener(UnityAction<OnPlayerBoardTilesDiscardedPayload> listener)
            {
                this.onTilesDiscarded.AddListener(listener);
            }

            public void AddOnPlayerBoardDiscardOneTileListener(UnityAction<OnPlayerBoardDiscardOneTilePayload> listener)
            {
                this.onDiscardOneTile.AddListener(listener);
            }

            public void AddOnPlayerBoardTilesCollectedListener(UnityAction<OnPlayerBoardTilesCollectedPayload> listener)
            {
                this.onTilesCollected.AddListener(listener);
            }

            public void AddOnAltarMilestoneCompleteListener(UnityAction<OnAltarMilestoneCompletedPayload> listener)
            {
                foreach (PlayerBoard playerBoard in this.playerBoards)
                {
                    playerBoard.GetMilestoneController().AddOnAltarMilestoneCompleteListener(listener);
                }
            }
            public void AddOnNumberMilestoneCompleteListener(UnityAction<OnNumberMilestoneCompletedPayload> listener)
            {
                foreach (PlayerBoard playerBoard in this.playerBoards)
                {
                    playerBoard.GetMilestoneController().AddOnNumberMilestoneCompleteListener(listener);
                }
            }

            public Tile ClaimReward(int playerNumber, TileColor tileColor)
            {
                BagController bagController = System.Instance.GetBagController();
                Tile claimedTile = bagController.Draw(tileColor);
                this.AddDrawnTiles(playerNumber, new() { claimedTile });
                return claimedTile;
            }

            public CoroutineResultValue<Tile> ClaimRewardAndWait(int playerNumber, TileColor tileColor)
            {
                CoroutineResultValue<Tile> result = new CoroutineResultValue<Tile>();
                this.StartCoroutine(this.ClaimRewardCoroutine(playerNumber, tileColor, result)); ;
                return result;
            }

            private IEnumerator ClaimRewardCoroutine(int playerNumber, TileColor tileColor, CoroutineResultValue<Tile> result)
            {
                BagController bagController = System.Instance.GetBagController();
                Tile claimedTile = bagController.Draw(tileColor);
                yield return this.AddDrawnTiles(playerNumber, new() { claimedTile }).WaitUntilCompleted();
                result.Finish(claimedTile);
            }

            public bool IsPlacingTiles()
            {
                return this.isPlacingTiles;
            }

            public List<TileColor> GetCompletedAltarColors(int playerNumber)
            {
                PlayerBoard playerBoard = this.GetPlayerBoard(playerNumber);
                return playerBoard.GetAltars()
                    .Where(altar => altar.IsFilled())
                    .Select(altar => altar.GetColor())
                    .ToList();
            }

            public List<int> GetCompletedRitualNumbers(int playerNumber)
            {
                List<int> ritualNumbers = new() { 1, 2, 3, 4 };
                PlayerBoard playerBoard = this.GetPlayerBoard(playerNumber);
                List<Altar> altars = playerBoard.GetAltars();
                return ritualNumbers
                    .Where(ritualNumber => altars.All(altar => altar.IsSpaceFilled(ritualNumber)))
                    .ToList();
            }
        }
    }
}
