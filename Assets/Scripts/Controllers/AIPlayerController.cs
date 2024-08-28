using System.Collections;
using System.Collections.Generic;
using Azul.AI;
using Azul.Model;
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
                UnityEngine.Debug.Log($"AIPlayerController {this.playerNumber}: Placing Tiles");
                this.scoringStrategy.Score();
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
