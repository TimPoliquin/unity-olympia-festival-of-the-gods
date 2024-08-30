using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azul
{
    namespace Model
    {
        public class OnPlayerTurnStartPayload
        {
            public int PlayerNumber { get; init; }
            public Player Player { get; init; }
            public Phase Phase { get; init; }
        }

        public class OnPlayerTurnEndPayload
        {
            public int PlayerNumber { get; init; }
            public Phase Phase { get; init; }
        }
    }
}
