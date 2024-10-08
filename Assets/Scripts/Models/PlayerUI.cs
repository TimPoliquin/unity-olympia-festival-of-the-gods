using System.Collections;
using System.Collections.Generic;
using Azul.Animation;
using Azul.Util;
using TMPro;
using UnityEngine;

namespace Azul
{
    namespace Model
    {
        [RequireComponent(typeof(ScalePulse))]
        public class PlayerUI : MonoBehaviour
        {
            [SerializeField] private TextMeshProUGUI scoreText;
            [SerializeField] private TextMeshProUGUI playerNameText;
            [SerializeField] private List<ColoredValue<PlayerTileCountUI>> collectedTileCounts;
            [SerializeField] private GameObject status;

            private int score;

            private ScalePulse scalePulse;

            private Dictionary<TileColor, PlayerTileCountUI> tileCountsByColor;
            private int playerNumber;

            void Start()
            {
                this.scalePulse = this.GetComponent<ScalePulse>();
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
                this.playerNumber = player.GetPlayerNumber();
                this.playerNameText.text = player.GetPlayerName();
            }

            public int GetScore()
            {
                return this.score;
            }

            public void SetScore(int score)
            {
                this.score = score;
                this.scoreText.text = $"{score}";
            }

            public CoroutineResult UpdateScore(int score, float time)
            {
                if (this.score != score)
                {
                    this.score = score;
                    this.scoreText.text = $"{score}";
                    return this.scalePulse.Animate(this.scoreText.gameObject, 1.5f, time);
                }
                else
                {
                    CoroutineResult result = CoroutineResult.Single();
                    result.Finish();
                    return result;
                }
            }

            public void SetActive(bool active)
            {
                this.status.SetActive(active);
            }

            public int GetPlayerNumber()
            {
                return this.playerNumber;
            }

            public Vector3 GetTileCountPosition(TileColor tileColor)
            {
                PlayerTileCountUI playerTileCountUI = this.tileCountsByColor[tileColor];
                return System.Instance.GetCameraController().GetMainCamera().ScreenToWorldPoint(playerTileCountUI.transform.position);
            }

            public Vector3 GetScoreWorldPosition()
            {
                return System.Instance.GetCameraController().GetMainCamera().ScreenToWorldPoint(this.scoreText.transform.position);
            }
            public Vector3 GetScoreScreenPosition()
            {
                return this.scoreText.transform.position;
            }
        }
    }
}
