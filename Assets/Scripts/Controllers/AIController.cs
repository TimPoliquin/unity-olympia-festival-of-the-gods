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
                if (null != aiPlayerController)
                {
                    switch (payload.Phase)
                    {
                        case Phase.ACQUIRE:
                            this.StartCoroutine(this.PerformAIPlayerAcquire(aiPlayerController));
                            break;
                        case Phase.SCORE:
                            this.StartCoroutine(this.PerformAIPlayerScore(aiPlayerController));
                            break;
                    }
                }
            }

            private AIPlayerController GetAIPlayerController(int playerNumber)
            {
                return this.aIPlayerControllers.Find(controller => controller.GetPlayerNumber() == playerNumber);
            }

            private IEnumerator PerformAIPlayerAcquire(AIPlayerController aiPlayerController)
            {
                // TODO - this is presently a hack to wait for other controllers to execute before the AI takes immediate action
                // TODO -- an extra acquire phase event dispatches when the last person draws the last tile.
                // we should stop that from happening.
                // in the meantime, this prevents the AI from trying to draw when there are no tiles left.
                yield return new WaitForSeconds(1.0f);
                if (System.Instance.GetRoundController().GetCurrentPhase() == Phase.ACQUIRE)
                {
                    aiPlayerController.OnAcquireTurn();
                }
            }
            private IEnumerator PerformAIPlayerScore(AIPlayerController aiPlayerController)
            {
                // TODO - this is presently a hack to wait for other controllers to execute before the AI takes immediate action
                // TODO -- an extra acquire phase event dispatches when the last person draws the last tile.
                // we should stop that from happening.
                // in the meantime, this prevents the AI from trying to draw when there are no tiles left.
                yield return new WaitForSeconds(1.0f);
                if (System.Instance.GetRoundController().GetCurrentPhase() == Phase.SCORE)
                {
                    aiPlayerController.OnScoreTurn();
                }
            }
        }
    }
}
