using System;
using System.Collections;
using System.Collections.Generic;
using Azul.AI;
using Azul.AIEvents;
using Azul.Event;
using Azul.GameEvents;
using Azul.Model;
using Azul.PlayerBoardRewardEvents;
using Azul.PlayerEvents;
using Azul.Util;
using Unity.VisualScripting;
using UnityEngine;

namespace Azul
{
    namespace Controller
    {
        public class AIPlayerController : MonoBehaviour
        {
            [SerializeField] private int playerNumber;
            private AcquireStrategy acquireStrategy;
            private ScoringStrategy scoringStrategy;
            private bool scoring = false;

            void Awake()
            {
                this.acquireStrategy = this.gameObject.AddComponent<AcquireStrategy>();
                this.scoringStrategy = this.gameObject.AddComponent<ScoringStrategy>();
                System.Instance.GetGameController().AddOnGameSetupCompleteListener(this.OnGameSetupComplete);
            }

            private void OnGameSetupComplete(OnGameSetupCompletePayload payload)
            {
                this.InitializeListeners();
            }

            private void InitializeListeners()
            {
                PlayerController playerController = System.Instance.GetPlayerController();
                PlayerBoardController playerBoardController = System.Instance.GetPlayerBoardController();
                playerController.AddOnPlayerBoardExceedsOverflowListener(this.OnOverflow);
                playerBoardController.AddOnPlayerBoardEarnRewardListener(this.OnEarnReward);
            }

            public void OnAcquireTurn()
            {
                this.StartCoroutine(this.AcquireTurnCoroutine());
            }

            private IEnumerator AcquireTurnCoroutine()
            {
                // give the players a bit of time to breath
                yield return new WaitForSeconds(1.0f);
                // wait for animations to play, if any are left
                yield return System.Instance.GetTileAnimationController().WaitUntilDoneAnimating();
                if (System.Instance.GetRoundController().IsCurrentPhaseAcquire() && System.Instance.GetPlayerController().GetCurrentPlayer().GetPlayerNumber() == this.playerNumber)
                {
                    UnityEngine.Debug.Log($"AIPlayerController {this.playerNumber}: Evaluating acquiring strategy");
                    this.acquireStrategy.EvaluateGoals();
                    UnityEngine.Debug.Log($"AIPlayerController {this.playerNumber}: Acquiring");
                    this.acquireStrategy.Acquire();
                }
                else
                {
                    UnityEngine.Debug.Log($"No longer acquire phase!!!");
                }
            }

            public CoroutineResult OnScoreTurn()
            {
                CoroutineResult result = CoroutineResult.Single();
                this.StartCoroutine(this.ScoreTurnCoroutine(result));
                return result;
            }

            private IEnumerator ScoreTurnCoroutine(CoroutineResult result)
            {
                if (this.scoring)
                {
                    UnityEngine.Debug.Log($"AIPlayerController: Player {this.playerNumber} is already scoring!");
                    result.Finish();
                }
                else
                {
                    this.scoring = true;
                    result.Start();
                    yield return new WaitUntil(() => !System.Instance.GetTileAnimationController().IsAnimating());
                    yield return new WaitUntil(() => !System.Instance.GetPlayerBoardController().IsPlacingTiles());
                    UnityEngine.Debug.Log($"AIPlayerController {this.playerNumber}: Evaluating Scoring Goals");
                    int count = 0;
                    while (count < 2 && this.IsMyTurn())
                    {
                        yield return this.scoringStrategy.EvaluateGoals(this.acquireStrategy.GetGoals()).WaitUntilCompleted();
                        if (this.scoringStrategy.CanScore())
                        {
                            count = 0;
                            UnityEngine.Debug.Log($"AIPlayerController {this.playerNumber}: Placing Tiles");
                            yield return this.scoringStrategy.Score(this.playerNumber).WaitUntilCompleted();
                            yield return new WaitForSeconds(.5f);
                        }
                        else
                        {
                            count++;
                            yield return new WaitForSeconds(.5f);
                        }
                    }
                    PlayerBoardController playerBoardController = System.Instance.GetPlayerBoardController();
                    if (playerBoardController.HasExcessiveOverflow(this.playerNumber))
                    {
                        this.HandleOverflow(this.playerNumber, playerBoardController.GetAllowedOverflow());
                    }
                    // TODO - also see if there are pending rewards somehow.
                    this.acquireStrategy.GetGoals().ForEach(goal => goal.EndScoring());
                    PlayerController playerController = System.Instance.GetPlayerController();
                    playerController.EndPlayerScoringTurn();
                    result.Finish();
                    this.scoring = false;
                }
            }

            private bool IsMyTurn()
            {
                return !System.Instance.GetRoundController().IsAfterLastRound() && System.Instance.GetPlayerController().GetCurrentPlayer().GetPlayerNumber() == this.playerNumber;
            }

            private void OnOverflow(OnPlayerBoardExceedsOverflowPayload payload)
            {
                if (this.HandleOverflow(payload.PlayerNumber, payload.TilesAllowed))
                {
                    PlayerController playerController = System.Instance.GetPlayerController();
                    playerController.EndPlayerScoringTurn();
                }
            }

            private bool HandleOverflow(int playerNumber, int allowedOverflow)
            {
                if (playerNumber == this.playerNumber)
                {
                    Dictionary<TileColor, int> discard = this.scoringStrategy.HandleOverflowDiscard(this.playerNumber, allowedOverflow);
                    PlayerBoardController playerBoardController = System.Instance.GetPlayerBoardController();
                    UnityEngine.Debug.Log($"Player {this.playerNumber} is discarding the following tiles:");
                    int totalDiscarded = 0;
                    foreach (KeyValuePair<TileColor, int> discardNum in discard)
                    {
                        UnityEngine.Debug.Log($"- {discardNum.Key}: {discardNum.Value}");
                        totalDiscarded += discardNum.Value;
                    }
                    if (totalDiscarded == 0)
                    {
                        throw new OverflowException("Failed to discard any tiles!");
                    }
                    playerBoardController.DiscardTiles(this.playerNumber, discard);
                    return true;
                }
                return false;
            }

            private void OnEarnReward(EventTracker<OnPlayerBoardEarnRewardPayload> payload)
            {
                if (payload.Payload.PlayerNumber == this.playerNumber)
                {
                    this.StartCoroutine(this.EarnRewardsCoroutine(payload.Payload.PlayerNumber, payload.Done));
                }
                else
                {
                    payload.Done();
                }
            }

            private IEnumerator EarnRewardsCoroutine(int numberOfTiles, Action Done)
            {
                PlayerBoardController playerBoardController = System.Instance.GetPlayerBoardController();
                for (int idx = 0; idx < numberOfTiles; idx++)
                {
                    TileColor tileColor = this.scoringStrategy.ChooseReward();
                    yield return playerBoardController.GrantRewardAndWait(this.playerNumber, tileColor).WaitUntilCompleted();
                }
                Done();
            }

            public int GetPlayerNumber()
            {
                return this.playerNumber;
            }

            public static AIPlayerController Create(Player player)
            {
                GameObject aiControllerGO = new GameObject($"{player.GetPlayerName()} AI Controller");
                AIPlayerController aiController = aiControllerGO.AddComponent<AIPlayerController>();
                aiController.playerNumber = player.GetPlayerNumber();
                aiController.acquireStrategy.CreateGoals(player.GetPlayerNumber());
                return aiController;
            }
        }
    }
}
