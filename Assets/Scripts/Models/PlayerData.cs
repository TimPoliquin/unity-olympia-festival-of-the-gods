using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using Steamworks;
using Azul.OnGameEndEvents;

namespace Azul
{
    namespace Model
    {
        [Serializable]
        public class PlayerData
        {
            public string PlayerName;
            public List<int> Scores;
            [NonSerialized]
            private bool isNewPlayer = false;

            public bool IsNewPlayer()
            {
                return this.isNewPlayer;
            }

            public void AddScore(int score)
            {
                this.Scores.Add(score);
                this.Scores.Sort((a, b) => b.CompareTo(a));
                if (this.Scores.Count > 10)
                {
                    this.Scores.RemoveRange(10, this.Scores.Count - 10);
                }
            }

            public static PlayerData Create(string playerName)
            {
                return new PlayerData
                {
                    PlayerName = playerName,
                    Scores = new(),
                    isNewPlayer = true
                };
            }
        }
        [Serializable]
        public class PlayerDataList
        {
            public List<PlayerData> playerData;
        }
    }
}
