using System;
using System.Collections;
using System.Collections.Generic;
using Azul.Model;
using UnityEngine;
using UnityEngine.Events;

namespace Azul
{
    namespace Controller
    {
        public class PlayerController : MonoBehaviour
        {
            [SerializeField] private List<Player> players;
            [SerializeField][Range(0, 3)] private int currentPlayer = 0;
            [SerializeField] private int turnStartsWith = 0;
            private int playerWithOneTile = -1;

            private UnityEvent<OnPlayerTurnStartPayload> onPlayerTurnStart = new UnityEvent<OnPlayerTurnStartPayload>();
            private UnityEvent<OnPlayerTurnStartPayload> onPlayerTurnEnd = new UnityEvent<OnPlayerTurnStartPayload>();

            public void InitializeListeners()
            {
                RoundController roundController = System.Instance.GetRoundController();
                roundController.AddOnRoundPhaseAcquireListener(this.OnRoundStart);
            }

            public int GetNumberOfPlayers()
            {
                return this.players.Count;
            }

            public Player GetCurrentPlayer()
            {
                return this.players[this.currentPlayer];
            }

            public void SetCurrentPlayer(int player)
            {
                if (player < this.players.Count)
                {
                    this.currentPlayer = player;
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(player), "Player number is out of bounds");
                }
            }

            public void StartTurn()
            {
                this.onPlayerTurnStart.Invoke(new OnPlayerTurnStartPayload
                {
                    PlayerNumber = this.currentPlayer,
                });
            }

            public void NextTurn()
            {
                this.currentPlayer = (this.currentPlayer + 1) % this.players.Count;
                if (this.currentPlayer == this.turnStartsWith)
                {

                }
            }

            public void AddOnPlayerTurnStartListener(UnityAction<OnPlayerTurnStartPayload> listener)
            {
                this.onPlayerTurnStart.AddListener(listener);
            }

            private void OnRoundStart(OnRoundPhaseAcquirePayload payload)
            {
                if (this.playerWithOneTile >= 0)
                {
                    this.turnStartsWith = this.playerWithOneTile;
                }
                else
                {
                    this.turnStartsWith = 0;
                }
                this.currentPlayer = this.turnStartsWith;
                this.StartTurn();
            }

        }
    }
}
