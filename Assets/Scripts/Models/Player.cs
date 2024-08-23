using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azul
{
    namespace Model
    {
        public enum PlayerColor
        {
            BAIGE,
            BLACK,
            GREY,
            WHITE,
        }
        public class Player : MonoBehaviour
        {
            [SerializeField] private int playerNumber;
            [SerializeField] private string playerName;
            [SerializeField] private PlayerColor color;

            public int GetPlayerNumber()
            {
                return this.playerNumber;
            }

            public string GetPlayerName()
            {
                return this.playerName;
            }

            public PlayerColor GetPlayerColor()
            {
                return this.color;
            }

            public void Initialize(int playerNumber, string playerName)
            {
                this.playerName = playerName;
                this.playerNumber = playerNumber;
            }
        }
    }
}
