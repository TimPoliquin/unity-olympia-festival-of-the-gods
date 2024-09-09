using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azul
{
    namespace Model
    {
        [Serializable]
        public class StarCompletedMilestone
        {
            [SerializeField] private TileColor Color;
            [SerializeField] private int Points;

            public TileColor GetColor()
            {
                return this.Color;
            }

            public bool IsMilestoneComplete(Altar star)
            {
                return star.IsFilled();
            }

            public int GetPoints()
            {
                return this.Points;
            }
        }

    }
}
