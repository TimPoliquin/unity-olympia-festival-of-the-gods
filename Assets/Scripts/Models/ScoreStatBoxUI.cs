using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Azul
{
    namespace Model
    {
        public class ScoreStatBoxUI : MonoBehaviour
        {
            [SerializeField] private ScoringIconUI scoringIconUI;

            public void SetScore(int score)
            {
                this.scoringIconUI.SetScore(score);
            }
        }

    }
}
