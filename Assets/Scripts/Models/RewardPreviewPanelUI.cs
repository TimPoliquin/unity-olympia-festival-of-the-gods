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
        public class RewardPreviewPanelUI : MonoBehaviour
        {
            [SerializeField] private string singularRewardTemplate = "{0} Token";
            [SerializeField] private string pluralRewardTemplate = "{0} Tokens";
            [SerializeField] private Image boonIcon;
            [SerializeField] private TextMeshProUGUI boonValueText;
            [SerializeField] private GameObject ritualsContainer;
            [SerializeField] private List<RewardColor> rewardConfiguration;
            [SerializeField] private GameObject requirementsContainer;
            [SerializeField] private GameObject completedContainer;

            private Fade fade;

            private bool completed = true;

            void Awake()
            {
                this.fade = this.GetComponent<Fade>();
            }

            public void SetBoonValue(int value)
            {
                RewardColor rewardColor = this.rewardConfiguration[value - 1];
                this.boonIcon.sprite = rewardColor.Icon;
                this.boonIcon.color = rewardColor.Color;
                if (value == 1)
                {
                    this.boonValueText.text = string.Format(this.singularRewardTemplate, "1");
                }
                else
                {
                    this.boonValueText.text = string.Format(this.pluralRewardTemplate, $"{value}");
                }
            }

            public void AddRequirement(RewardRitualUI requirement)
            {
                requirement.transform.SetParent(this.ritualsContainer.transform);
                this.completed &= requirement.IsCompleted();
                this.completedContainer.SetActive(this.completed);
                this.requirementsContainer.SetActive(!this.completed);
            }

            public CoroutineStatus Show()
            {
                this.fade.StartHidden();
                return this.fade.Show();
            }

            public CoroutineStatus Hide()
            {
                return this.fade.Hide();
            }
        }
    }
}
