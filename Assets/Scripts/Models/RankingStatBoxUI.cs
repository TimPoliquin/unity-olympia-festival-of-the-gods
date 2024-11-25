using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Azul
{
    namespace Model
    {
        public class RankingStatBoxUI : MonoBehaviour
        {
            [SerializeField] private TextMeshProUGUI rankingText;
            [SerializeField] private TextMeshProUGUI emptyText;

            public void SetRanking(int ranking)
            {
                this.rankingText.text = $"{ranking}";
                this.emptyText.gameObject.SetActive(false);
            }

            public void SetNoRanking()
            {
                this.rankingText.gameObject.SetActive(false);
                this.emptyText.gameObject.SetActive(true);
            }
        }

    }
}
