using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Azul
{
    public class ScoreRowUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI playerName;
        [SerializeField] private TextMeshProUGUI playerScore;

        public void SetValues(string playerName, int playerScore)
        {
            this.playerName.text = playerName;
            this.playerScore.text = $"{playerScore}";
        }
    }
}
