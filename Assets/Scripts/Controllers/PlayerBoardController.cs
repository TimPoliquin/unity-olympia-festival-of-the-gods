using System;
using System.Collections;
using System.Collections.Generic;
using Azul.Model;
using Azul.PlayerBoardEvents;
using Azul.PointerEvents;
using UnityEngine;
using UnityEngine.Events;

namespace Azul
{
    namespace PlayerBoardEvents
    {
        public class OnPlayerBoardScoreTileSelectionConfirmPayload
        {
            public Dictionary<TileColor, int> TilesSelected { get; init; }
        }
        public class OnPlayerBoardScoreSpaceSelectionPayload
        {
            public int PlayerNumber { get; init; }
            public TileColor Color { get; init; }
            public int Value { get; init; }
            public PlayerBoard PlayerBoard;
            public UnityAction<OnPlayerBoardScoreTileSelectionConfirmPayload> OnConfirm { get; init; }
        }
    }
    namespace Controller
    {
        public class PlayerBoardController : MonoBehaviour
        {
            [SerializeField] private GameObject playerBoardPrefab;

            private UnityEvent<OnPlayerBoardScoreSpaceSelectionPayload> onScoreSpaceSelection = new();
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
                RoundController roundController = System.Instance.GetRoundController();
                roundController.AddOnRoundPhaseAcquireListener(this.OnRoundAcquire);
                roundController.AddOnRoundPhasePrepareListener(this.OnRoundPrepare);
                roundController.AddOnRoundPhaseScoreListener(this.OnRoundScore);
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
                        spaces.ForEach(space =>
                        {
                            space.ActivateHighlight();
                            space.GetPointerEventController().AddOnPointerSelectListener(this.OnPointerSelectSpace);
                        });
                    }
                }
            }

            private void OnPointerSelectSpace(OnPointerSelectPayload<TilePlaceholder> payload)
            {
                // TODO - a lot needs to be done here to keep the rails on selection.
                // we should confirm the space is on an active playerboard, for instance.
                if (System.Instance.GetRoundController().GetCurrentPhase() == Phase.SCORE)
                {
                    StarSpace space = payload.Target.GetComponent<StarSpace>();
                    PlayerBoard playerBoard = space.GetComponentInParent<PlayerBoard>();
                    this.onScoreSpaceSelection.Invoke(new OnPlayerBoardScoreSpaceSelectionPayload
                    {
                        PlayerBoard = playerBoard,
                        PlayerNumber = playerBoard.GetPlayerNumber(),
                        Color = space.GetEffectiveColor(),
                        Value = space.GetValue(),
                        OnConfirm = (payload) => this.OnConfirmScoreTileSelection(playerBoard, space, payload)
                    });
                }
            }

            public void AddOnPlayerBoardScoreSpaceSelectionListener(UnityAction<OnPlayerBoardScoreSpaceSelectionPayload> listener)
            {
                this.onScoreSpaceSelection.AddListener(listener);
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

            private void OnConfirmScoreTileSelection(PlayerBoard playerBoard, StarSpace space, OnPlayerBoardScoreTileSelectionConfirmPayload payload)
            {
                RoundController roundController = System.Instance.GetRoundController();
                TileColor wildColor = roundController.GetCurrentRound().GetWildColor();
                TileColor spaceColor = space.GetEffectiveColor();
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
                this.OnPlayerTurnScoringStart(playerBoard.GetPlayerNumber());
            }
        }
    }
}
