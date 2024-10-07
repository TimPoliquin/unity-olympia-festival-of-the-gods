using System.Collections;
using System.Collections.Generic;
using Azul.Animation;
using Azul.Util;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Azul
{
    namespace Model
    {
        [RequireComponent(typeof(MoveAndScale))]
        public class MilestoneCompletedPanelUI : MonoBehaviour
        {
            [SerializeField] private string subtitleTextTemplate = "You have honored {0} with your tribute and earned a reward!";
            [SerializeField] private IconUI godIcon;
            [SerializeField] private Image ritualIcon;
            [SerializeField] private TextMeshProUGUI ritualNumber;
            [SerializeField] private GameObject scoreContainer;
            [SerializeField] private Image scoreIcon;
            [SerializeField] private TextMeshProUGUI scoreText;
            [SerializeField] private TextMeshProUGUI subtitleText;
            private Fade fade;
            private MoveAndScale moveAndScale;

            private void Awake()
            {
                this.fade = this.GetComponent<Fade>();
                this.moveAndScale = this.GetComponent<MoveAndScale>();
            }

            public CoroutineResult Show(string godName, TileColor color, int points)
            {
                this.subtitleText.text = string.Format(this.subtitleTextTemplate, godName);
                this.godIcon.SetTileColor(color);
                this.scoreText.text = $"{points}";
                this.ritualIcon.gameObject.SetActive(false);
                this.fade.StartHidden();
                return this.fade.Show();
            }

            public CoroutineResult Show(int ritualNumber, int points)
            {
                this.subtitleText.text = string.Format(this.subtitleTextTemplate, "The Gods");
                this.godIcon.gameObject.SetActive(false);
                this.scoreText.text = $"{points}";
                this.ritualIcon.gameObject.SetActive(true);
                this.ritualNumber.text = $"{ritualNumber}";
                this.fade.StartHidden();
                return this.fade.Show();
            }

            public CoroutineResult AnimateScoreToPoint(Vector3 target, float scale, float time)
            {
                return this.moveAndScale.Animate(this.scoreContainer, target, scale, time);
            }

            public CoroutineResult Hide()
            {
                return this.fade.Hide();
            }
        }
    }
}
