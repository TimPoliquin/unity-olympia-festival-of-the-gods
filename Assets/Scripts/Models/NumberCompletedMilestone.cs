using System;
using System.Collections;
using System.Collections.Generic;
using Azul.Model;
using UnityEngine;

namespace Azul
{
    [Serializable]
    public class NumberCompletedMilestone
    {
        [SerializeField] private int Number;
        [SerializeField] private int Points;

        public int GetNumber()
        {
            return this.Number;
        }

        public bool IsMilestoneComplete(int playerNumber, int tileNumber)
        {
            PlayerBoard board = System.Instance.GetPlayerBoardController().GetPlayerBoard(playerNumber);
            return board.IsTileNumberFilledOnAllAltars(tileNumber);
        }

        public int GetPoints()
        {
            return this.Points;
        }
    }
}
