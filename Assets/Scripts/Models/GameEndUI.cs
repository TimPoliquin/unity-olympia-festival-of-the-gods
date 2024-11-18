using System.Collections;
using System.Collections.Generic;
using Azul.Animation;
using Azul.OnGameEndEvents;
using Azul.Util;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Azul
{

    namespace Model
    {
        [RequireComponent(typeof(Fade))]
        public class GameEndUI : MonoBehaviour
        {
            [SerializeField] private GameObject playerStatsContainer;
            [SerializeField] private RectTransform panelContainer;
            [SerializeField] private float defaultPanelHeight = 350f;
            [SerializeField] private float rowHeight = 64f;
            [SerializeField] private Fade playerStatsFade;
            [SerializeField] private Fade buttonsFade;
            [SerializeField] private ButtonUI playAgain;
            [SerializeField] private ButtonUI quit;

            private Fade fade;

            void Awake()
            {
                this.playAgain.AddOnClickListener(this.OnPlayAgain);
                this.quit.AddOnClickListener(this.OnQuit);
                this.fade = this.GetComponent<Fade>();
                this.fade.StartHidden();
                this.playerStatsFade.StartHidden();
                this.buttonsFade.StartHidden();
            }

            public GameObject GetPlayerStatsContainer()
            {
                return this.playerStatsContainer;
            }

            public void SetPlayerCount(int playerCount)
            {
                float verticalScale = this.panelContainer.sizeDelta.y / 350f;
                float scaledRowHeight = this.rowHeight * verticalScale;
                float reductionFactor = 4 - playerCount;
                this.panelContainer.sizeDelta = new Vector2(this.panelContainer.sizeDelta.x, this.defaultPanelHeight * verticalScale - reductionFactor * scaledRowHeight);
            }

            private void OnPlayAgain()
            {
                System.Instance.PlayAgain();
            }

            private void OnQuit()
            {
                System.Instance.Quit();
            }

            public CoroutineResult Show()
            {
                return this.fade.Show();
            }

            public CoroutineResult ShowPlayerStats()
            {
                return this.playerStatsFade.Show();
            }

            public CoroutineResult ShowButtons()
            {
                return this.buttonsFade.Show();
            }
        }
    }
}
