using System;
using System.Collections;
using System.Collections.Generic;
using Azul.Model;
using UnityEngine;

namespace Azul
{
    namespace Controller
    {
        public class AIController : MonoBehaviour
        {
            private List<AIPlayerController> aIPlayerControllers;
            public void SetupGame(List<Player> players)
            {
                this.aIPlayerControllers = new();
                foreach (Player player in players)
                {
                    if (player.IsAI())
                    {
                        AIPlayerController aIPlayerController = AIPlayerController.Create(player);
                        aIPlayerController.transform.SetParent(this.transform);
                        this.aIPlayerControllers.Add(aIPlayerController);
                    }
                }
            }

            public void InitializeListeners()
            {
                PlayerController playerController = System.Instance.GetPlayerController();
                playerController.AddOnPlayerTurnStartListener(this.OnPlayerTurnStart);
            }

            private void OnPlayerTurnStart(OnPlayerTurnStartPayload payload)
            {
                AIPlayerController aiPlayerController = this.GetAIPlayerController(payload.PlayerNumber);
                RoundController roundController = System.Instance.GetRoundController();
                if (null != aiPlayerController && !roundController.IsAfterLastRound())
                {
                    switch (payload.Phase)
                    {
                        case Phase.ACQUIRE:
                            aiPlayerController.OnAcquireTurn();
                            break;
                        case Phase.SCORE:
                            aiPlayerController.OnScoreTurn();
                            break;
                    }
                }
            }

            private AIPlayerController GetAIPlayerController(int playerNumber)
            {
                return this.aIPlayerControllers.Find(controller => controller.GetPlayerNumber() == playerNumber);
            }
        }
    }
}
