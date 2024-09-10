using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Azul
{
    namespace Model
    {
        public class PlayerUI : MonoBehaviour
        {
            [SerializeField] private TextMeshProUGUI scoreText;
            [SerializeField] private TextMeshProUGUI playerNameText;
            [SerializeField] private List<ColoredValue<PlayerTileCountUI>> collectedTileCounts;

            private Dictionary<TileColor, PlayerTileCountUI> tileCountsByColor;
            void Start()
            {
                this.tileCountsByColor = new();
                foreach (ColoredValue<PlayerTileCountUI> tileCount in this.collectedTileCounts)
                {
                    tileCount.GetValue().SetTileCount(0);
                    this.tileCountsByColor[tileCount.GetTileColor()] = tileCount.GetValue();
                }
            }

            public void UpdateTileCount(List<ColoredValue<int>> tileCounts)
            {
                foreach (ColoredValue<int> tileCount in tileCounts)
                {
                    this.tileCountsByColor[tileCount.GetTileColor()].SetTileCount(tileCount.GetValue());
                }
            }

            public void SetPlayer(Player player)
            {
                this.playerNameText.text = player.GetPlayerName();
            }

            public void SetScore(int score)
            {
                this.scoreText.text = $"{score}";
            }
        }
    }
}
