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
            [SerializeField] private bool isAI;

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

            public bool IsAI()
            {
                return this.isAI;
            }

            public bool IsHuman()
            {
                return !this.isAI;
            }

            public void Initialize(int playerNumber, string playerName, bool isAI)
            {
                this.playerNumber = playerNumber;
                this.playerName = playerName;
                this.isAI = isAI;
            }
        }
    }
}
