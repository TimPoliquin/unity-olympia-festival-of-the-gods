using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Azul
{
    namespace Model
    {
        public class PlayerStatsUI : MonoBehaviour
        {
            [SerializeField] private TextMeshProUGUI playerNameText;
            [SerializeField] private GameObject winnerIcons;
            [SerializeField] private AltarStatBoxUI altarStatBoxUI;
            [SerializeField] private ScoreStatBoxUI scoreStatBoxUI;
            [SerializeField] private RitualStatBoxUI ritualStatBoxUI;

            public void ShowAltars(List<TileColor> altarColors)
            {
                if (altarColors.Count == 0)
                {
                    this.altarStatBoxUI.ShowEmptyStat();
                }
                else
                {
                    altarColors.ForEach(color => this.altarStatBoxUI.ShowIcon(color));
                }
            }

            public void ShowRitualNumbers(List<int> ritualNumers)
            {
                if (ritualNumers.Count == 0)
                {
                    this.ritualStatBoxUI.ShowEmptyStat();
                }
                else
                {
                    ritualNumers.ForEach(ritualNumber => this.ritualStatBoxUI.ShowRitual(ritualNumber));
                }
            }

            public void SetScore(int score)
            {
                this.scoreStatBoxUI.SetScore(score);
            }

            public void SetPlayerName(string name)
            {
                this.playerNameText.text = name;
            }

            public void SetWinner(bool winner)
            {
                this.winnerIcons.SetActive(winner);
            }
        }
    }
}
