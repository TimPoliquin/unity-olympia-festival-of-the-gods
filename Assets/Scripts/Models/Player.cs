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
        public enum PlayerType
        {
            HUMAN,
            AI_EASY,
            AI_DIFFICULT
        }
        public sealed class PlayerTypeUtils
        {
            public static PlayerType[] GetPlayerTypes()
            {
                return new PlayerType[] { PlayerType.HUMAN, PlayerType.AI_EASY, PlayerType.AI_DIFFICULT };
            }
            public static string GetPlayerTypeName(PlayerType playerType)
            {
                switch (playerType)
                {
                    case PlayerType.HUMAN:
                        return "Human";
                    case PlayerType.AI_EASY:
                        return "AI - Easy";
                    case PlayerType.AI_DIFFICULT:
                        return "AI - Difficult";
                    default:
                        return "";
                }
            }
        }
        public class Player : MonoBehaviour
        {
            [SerializeField] private int playerNumber;
            [SerializeField] private string playerName;
            [SerializeField] private PlayerColor color;
            [SerializeField] private PlayerType playerType;
            private string userName;

            public int GetPlayerNumber()
            {
                return this.playerNumber;
            }

            public string GetPlayerName()
            {
                return this.playerName;
            }

            public string GetUsername()
            {
                return this.userName;
            }

            public PlayerColor GetPlayerColor()
            {
                return this.color;
            }

            public bool IsAI()
            {
                return this.playerType == PlayerType.AI_EASY || this.playerType == PlayerType.AI_DIFFICULT;
            }

            public bool IsHuman()
            {
                return this.playerType == PlayerType.HUMAN;
            }

            public void Initialize(int playerNumber, string playerName, PlayerType playerType, string userName)
            {
                this.playerNumber = playerNumber;
                this.playerName = playerName;
                this.playerType = playerType;
                this.userName = userName;
            }

            public PlayerType GetPlayerType()
            {
                return this.playerType;
            }
        }
    }
}
