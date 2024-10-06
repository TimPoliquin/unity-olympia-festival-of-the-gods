using System.Collections;
using System.Collections.Generic;
using Azul.Animation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Azul
{
    namespace Model
    {
        public class MilestoneCompletedPanelUI : MonoBehaviour
        {
            [SerializeField] private string subtitleTextTemplate = "You have honored {0} with your tribute and earned a reward!";
            [SerializeField] private IconUI icon;
            [SerializeField] private Image scoreIcon;
            [SerializeField] private TextMeshProUGUI scoreText;
            [SerializeField] private TextMeshProUGUI subtitleText;
            private Fade fade;

            private void Awake()
            {
                this.fade = this.GetComponent<Fade>();
            }
            public void Show(string godName, TileColor color, int points)
            {
                this.subtitleText.text = string.Format(this.subtitleTextTemplate, godName);
                this.icon.SetTileColor(color);
                this.scoreText.text = $"{points}";
                this.fade.StartHidden();
                this.StartCoroutine(this.fade.Show());
            }

            public void Hide()
            {
                this.StartCoroutine(this.fade.Hide());
            }
        }
    }
}
