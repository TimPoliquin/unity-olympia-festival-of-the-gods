using System.Collections;
using System.Collections.Generic;
using Azul.AI;
using Azul.AIEvents;
using Azul.GameEvents;
using Azul.Model;
using Azul.PlayerBoardRewardEvents;
using Azul.PlayerEvents;
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

            void Awake()
            {
                this.acquireStrategy = this.AddComponent<AcquireStrategy>();
                this.scoringStrategy = this.AddComponent<ScoringStrategy>();
                this.scoringStrategy.AddOnScoreCompleteListener(this.OnScoreComplete);
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
                UnityEngine.Debug.Log($"AIPlayerController {this.playerNumber}: Evaluating strategy");
                this.acquireStrategy.EvaluateGoals();
                UnityEngine.Debug.Log($"AIPlayerController {this.playerNumber}: Acting");
                this.acquireStrategy.Acquire();
            }

            public void OnScoreTurn()
            {
                UnityEngine.Debug.Log($"AIPlayerController {this.playerNumber}: Evaluating Scoring Goals");
                this.scoringStrategy.EvaluateGoals(this.acquireStrategy.GetGoals());
                if (this.scoringStrategy.CanScore())
                {
                    UnityEngine.Debug.Log($"AIPlayerController {this.playerNumber}: Placing Tiles");
                    this.scoringStrategy.Score();
                }
                else
                {
                    PlayerController playerController = System.Instance.GetPlayerController();
                    playerController.EndPlayerScoringTurn();
                }
            }

            public void OnOverflow(OnPlayerBoardExceedsOverflowPayload payload)
            {
                Dictionary<TileColor, int> discard = this.scoringStrategy.HandleOverflowDiscard(this.playerNumber, payload.TilesAllowed);
                PlayerBoardController playerBoardController = System.Instance.GetPlayerBoardController();
                playerBoardController.DiscardTiles(this.playerNumber, discard);
            }

            public void OnEarnReward(OnPlayerBoardEarnRewardPayload payload)
            {
                PlayerBoardController playerBoardController = System.Instance.GetPlayerBoardController();
                for (int idx = 0; idx < payload.NumberOfTiles; idx++)
                {
                    TileColor tileColor = this.scoringStrategy.ChooseReward();
                    playerBoardController.GrantReward(this.playerNumber, tileColor);
                }
            }

            public void OnScoreComplete(OnAIScorePayload payload)
            {
                this.OnScoreTurn();
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
