using System.Collections;
using System.Collections.Generic;
using Azul.OnGameEndEvents;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Azul
{

    namespace Model
    {
        public class GameEndUI : MonoBehaviour
        {
            [SerializeField] private TextMeshProUGUI winnerNameText;
            [SerializeField] private GameObject scoresContainer;
            [SerializeField] private Button playAgain;
            [SerializeField] private Button quit;

            void Awake()
            {
                this.playAgain.onClick.AddListener(this.OnPlayAgain);
                this.quit.onClick.AddListener(this.OnQuit);
            }

            public void SetWinnerName(string winnerName)
            {
                this.winnerNameText.text = winnerName;
            }

            public void AddScoreRow(ScoreRowUI prefab, string playerName, int score)
            {
                ScoreRowUI scoreRowUI = Instantiate(prefab, this.scoresContainer.transform);
                scoreRowUI.SetValues(playerName, score);
            }

            private void OnPlayAgain()
            {
                // UH OH!
            }

            private void OnQuit()
            {
#if UNITY_EDITOR
                EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }
        }
    }
}
