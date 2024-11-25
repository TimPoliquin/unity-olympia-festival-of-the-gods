using System.Collections.Generic;
using Azul.Animation;
using Azul.Model;
using Azul.MtOlympusSelectionEvents;
using Azul.ScoringSelectionWizardEvents;
using Azul.Util;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Azul
{
    namespace ScoringSelectionWizardEvents
    {
        public struct OnMtOlympusColorSelectionStepInitializePayload
        {
            public int PlayerNumber { get; init; }
            public MtOlympusColorSelectionStepUI Step { get; init; }
            public AltarSpace Space { get; init; }
        }
        public struct OnTokenCountSelectionStepInitializePayload
        {
            public TileColor Color { get; init; }
            public int PlayerNumber { get; init; }
            public TokenCountSelectionStepUI Step { get; init; }
            public AltarSpace Space { get; init; }
        }
        public struct OnTokenCountSelectionConfirmPayload
        {
            public TileColor Color { get; init; }
            public Dictionary<TileColor, int> SelectedCounts { get; init; }
        }
    }
    namespace Model
    {
        [RequireComponent(typeof(Fade))]
        public class ScoringSelectionWizardPanelUI : MonoBehaviour
        {
            private enum Step
            {
                MT_OLYMPUS,
                TOKEN_COUNT
            }
            [SerializeField] private MtOlympusColorSelectionStepUI mtOlympusColorSelectionStepUI;
            [SerializeField] private TokenCountSelectionStepUI tokenCountSelectionStepUI;
            [SerializeField] private ButtonUI backButton;
            [SerializeField] private ButtonUI nextButton;
            [SerializeField] private Image nextIcon;
            [SerializeField] private ButtonUI cancelButton;
            private Fade fade;

            private List<Step> steps;
            private int currentStep = -1;
            private int playerNumber;
            private AltarSpace altarSpace;
            private UnityEvent onCancel = new();

            private UnityEvent<OnMtOlympusColorSelectionStepInitializePayload> onMtOlympusSelectionStepInitialize = new();
            private UnityEvent<OnTokenCountSelectionStepInitializePayload> onTokenCountSelectionStepInitialize = new();
            private UnityEvent<OnTokenCountSelectionConfirmPayload> onConfirm = new();

            void Awake()
            {
                this.fade = this.GetComponent<Fade>();
                this.mtOlympusColorSelectionStepUI.AddOnSelectionListener(this.OnMtOlympusColorSelection);
                this.backButton.AddOnClickListener(this.OnBack);
                this.nextButton.AddOnClickListener(this.OnNext);
                this.cancelButton.AddOnClickListener(this.OnCancel);
            }

            public CoroutineStatus Show()
            {
                this.fade.StartHidden();
                this.ActivateStep(0);
                return this.fade.Show();
            }

            public CoroutineStatus Hide()
            {
                return this.fade.Hide();
            }

            public AltarSpace GetAltarSpace()
            {
                return this.altarSpace;
            }

            public void SetAltarSpace(AltarSpace altarSpace)
            {
                this.altarSpace = altarSpace;
                if (this.altarSpace.IsMtOlympus())
                {
                    this.steps = new() { Step.MT_OLYMPUS, Step.TOKEN_COUNT };
                }
                else
                {
                    this.steps = new() { Step.TOKEN_COUNT };
                }
            }

            public void SetPlayerNumber(int playerNumber)
            {
                this.playerNumber = playerNumber;
            }

            private bool IsMtOlympus()
            {
                return this.altarSpace.IsMtOlympus();
            }

            public MtOlympusColorSelectionStepUI GetMtOlympusColorSelectionStep()
            {
                return this.mtOlympusColorSelectionStepUI;
            }

            public TokenCountSelectionStepUI GetTokenCountSelectionStep()
            {
                return this.tokenCountSelectionStepUI;
            }

            public void InitializeButtonStates()
            {
                if (this.currentStep >= 0)
                {
                    switch (this.steps[currentStep])
                    {
                        case Step.MT_OLYMPUS:
                            this.backButton.SetActive(false);
                            this.nextButton.SetActive(true);
                            this.nextButton.SetInteractable(this.mtOlympusColorSelectionStepUI.HasSelection());
                            this.nextButton.SetText("Next");
                            this.nextIcon.gameObject.SetActive(true);
                            break;
                        case Step.TOKEN_COUNT:
                            this.backButton.SetActive(this.currentStep > 0);
                            this.nextButton.SetActive(true);
                            this.nextButton.SetInteractable(true);
                            this.nextButton.SetText("Confirm");
                            this.nextIcon.gameObject.SetActive(false);
                            break;
                    }
                }
            }

            public void ActivateStep(int step)
            {
                this.currentStep = step;
                switch (this.steps[this.currentStep])
                {
                    case Step.MT_OLYMPUS:
                        this.tokenCountSelectionStepUI.gameObject.SetActive(false);
                        this.mtOlympusColorSelectionStepUI.gameObject.SetActive(true);
                        this.onMtOlympusSelectionStepInitialize.Invoke(new OnMtOlympusColorSelectionStepInitializePayload
                        {
                            PlayerNumber = this.playerNumber,
                            Step = this.mtOlympusColorSelectionStepUI,
                            Space = this.altarSpace
                        });
                        break;
                    case Step.TOKEN_COUNT:
                        this.mtOlympusColorSelectionStepUI.gameObject.SetActive(false);
                        this.tokenCountSelectionStepUI.gameObject.SetActive(true);
                        this.onTokenCountSelectionStepInitialize.Invoke(new OnTokenCountSelectionStepInitializePayload
                        {
                            Color = this.IsMtOlympus() ? this.mtOlympusColorSelectionStepUI.GetSelection() : this.altarSpace.GetOriginColor(),
                            PlayerNumber = this.playerNumber,
                            Step = this.tokenCountSelectionStepUI,
                            Space = this.altarSpace
                        });
                        break;
                }
                this.InitializeButtonStates();
            }


            private void OnBack()
            {
                if (this.currentStep > 0)
                {
                    this.ActivateStep(this.currentStep - 1);
                }
            }

            private void OnNext()
            {
                switch (this.steps[this.currentStep])
                {
                    case Step.MT_OLYMPUS:
                        this.ActivateStep(this.currentStep + 1);
                        break;
                    case Step.TOKEN_COUNT:
                        this.OnConfirm();
                        break;
                }
            }

            private void OnCancel()
            {
                this.onCancel.Invoke();
            }

            private void OnConfirm()
            {
                this.onConfirm.Invoke(new OnTokenCountSelectionConfirmPayload
                {
                    Color = this.IsMtOlympus() ? this.mtOlympusColorSelectionStepUI.GetSelection() : this.altarSpace.GetOriginColor(),
                    SelectedCounts = this.tokenCountSelectionStepUI.GetSelectedCounts()
                });
            }


            private void OnMtOlympusColorSelection(OnMtOlympusColorSelectedPayload payload)
            {
                this.InitializeButtonStates();
            }
            public void AddOnCancel(UnityAction listener)
            {
                this.onCancel.AddListener(listener);
            }

            public void AddOnConfirmListener(UnityAction<OnTokenCountSelectionConfirmPayload> listener)
            {
                this.onConfirm.AddListener(listener);
            }

            public void AddOnMtOlympusColorSelectionStepInitializeListener(UnityAction<OnMtOlympusColorSelectionStepInitializePayload> listener)
            {
                this.onMtOlympusSelectionStepInitialize.AddListener(listener);
            }

            public void AddOnTokenCountSelectionStepInitializeListener(UnityAction<OnTokenCountSelectionStepInitializePayload> listener)
            {
                this.onTokenCountSelectionStepInitialize.AddListener(listener);
            }
        }
    }
}
