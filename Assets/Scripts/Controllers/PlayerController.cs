using System.Collections;
using System.Collections.Generic;
using Azul.Model;
using UnityEngine;

namespace Azul
{
    namespace Controller
    {
        public class PlayerController : MonoBehaviour
        {
            [SerializeField] private List<Player> players;
            [SerializeField][Range(0, 3)] private int currentPlayer = 0;

            public int GetNumberOfPlayers()
            {
                return this.players.Count;
            }

            public Player GetCurrentPlayer()
            {
                return this.players[this.currentPlayer];
            }
        }
    }
}
