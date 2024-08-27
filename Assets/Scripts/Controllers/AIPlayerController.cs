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

            void Awake()
            {
                this.acquireStrategy = this.AddComponent<AcquireStrategy>();
                this.acquireStrategy.CreateGoals();
            }

            public void OnAcquireTurn()
            {
                UnityEngine.Debug.Log($"AIPlayerController {this.playerNumber}: Evaluating strategy");
                this.acquireStrategy.EvaluateGoalsForRound();
                UnityEngine.Debug.Log($"AIPlayerController {this.playerNumber}: Acting");
                this.acquireStrategy.Act();
            }

            public void OnScoreTurn()
            {

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
                return aiController;
            }
        }
    }
}
