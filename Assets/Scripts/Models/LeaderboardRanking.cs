using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azul
{
    namespace Model
    {
        [Serializable]
        public class LeaderboardRanking
        {
            public int Ranking { get; init; }
            public int Score { get; init; }

            public override string ToString()
            {
                return $"Ranking: {this.Ranking}|Score: {this.Score}";
            }
        }
    }
}
