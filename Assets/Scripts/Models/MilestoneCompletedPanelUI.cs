using System.Collections;
using System.Collections.Generic;
using Azul.Animation;
using Azul.Util;
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

            public CoroutineResult Show(string godName, TileColor color, int points)
            {
                this.subtitleText.text = string.Format(this.subtitleTextTemplate, godName);
                this.icon.SetTileColor(color);
                this.scoreText.text = $"{points}";
                this.fade.StartHidden();
                return this.fade.Show();
            }

            public CoroutineResult Hide()
            {
                return this.fade.Hide();
            }
        }
    }
}
