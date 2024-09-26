using System.Collections;
using System.Collections.Generic;
using Azul.GameEvents;
using Azul.GameStartEvents;
using Azul.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Azul
{
    namespace Controller
    {
        public class ScoringPanelUIController : MonoBehaviour
        {
            [SerializeField] private Button scoringButton;
            [SerializeField] private ScoringPanelUI scoringPanelUI;


            void Start()
            {
                this.scoringPanelUI.gameObject.SetActive(false);
                this.scoringButton.onClick.AddListener(this.ToggleScoringPanel);
                System.Instance.GetGameController().AddOnGameSetupCompleteListener(this.OnGameStart);
            }

            private void OnGameStart(OnGameSetupCompletePayload payload)
            {
                ScoreBoardController scoreBoardController = System.Instance.GetScoreBoardController();
                foreach (StarCompletedMilestone starCompletedMilestone in scoreBoardController.GetStarCompletedMilestones())
                {
                    this.scoringPanelUI.AddGodScoreUI(this.CreateGodScoreUI(starCompletedMilestone));
                }
                foreach (NumberCompletedMilestone numberCompletedMilestone in scoreBoardController.GetNumberCompletedMilestones())
                {
                    if (numberCompletedMilestone.GetPoints() > 0)
                    {
                        RitualScoreUI ritualScoreUI = this.CreateRitualScoreUI(numberCompletedMilestone);
                        this.scoringPanelUI.AddRitualScoreUI(ritualScoreUI);
                    }
                }
            }

            private void ToggleScoringPanel()
            {
                this.scoringPanelUI.Toggle();
            }

            private GodScoreUI CreateGodScoreUI(StarCompletedMilestone starCompletedMilestone)
            {
                GodScoreUI godScoreUI = System.Instance.GetPrefabFactory().CreateGodScoreUI();
                godScoreUI.Setup(starCompletedMilestone.GetColor(), starCompletedMilestone.GetPoints());
                return godScoreUI;
            }

            private RitualScoreUI CreateRitualScoreUI(NumberCompletedMilestone numberCompletedMilestone)
            {
                RitualScoreUI ritualScoreUI = System.Instance.GetPrefabFactory().CreateRitualScoreUI();
                ritualScoreUI.Setup(numberCompletedMilestone.GetNumber(), numberCompletedMilestone.GetPoints());
                return ritualScoreUI;
            }

        }
    }
}
