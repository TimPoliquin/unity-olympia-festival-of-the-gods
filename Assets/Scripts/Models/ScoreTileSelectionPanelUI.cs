using System.Collections;
using System.Collections.Generic;
using Azul.Model;
using Azul.ScoreTileSelectionUIEvent;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Azul
{
    namespace ScoreTileSelectionUIEvent
    {
        public struct OnScoreTileSelectionCancelPayload
        {
            public ScoreTileSelectionPanelUI Panel { get; init; }
        }

        public struct OnScoreTileSelectionConfirmPayload
        {
            public ScoreTileSelectionPanelUI Panel { get; init; }
            public Dictionary<TileColor, int> SelectedCounts { get; init; }
        }
    }
    namespace Model
    {
        public class ScoreTileSelectionPanelUI : MonoBehaviour
        {
            [SerializeField] private PanelUI panel;
            [SerializeField] private Image backgroundColorContainer;
            [SerializeField] private Image icon;
            [SerializeField] private TextMeshProUGUI selectedCountText;
            [SerializeField] private GameObject scoreTileSelectionUIContainer;
            [SerializeField] private Button confirmButton;
            [SerializeField] private Button cancelButton;

            private UnityEvent<OnScoreTileSelectionCancelPayload> onCancel = new();
            private UnityEvent<OnScoreTileSelectionConfirmPayload> onConfirm = new();

            private int countNeeded;

            private List<ScoreTileSelectionUI> tileSelectionUIs = new();

            void Start()
            {
                this.confirmButton.onClick.AddListener(this.OnConfirm);
                this.cancelButton.onClick.AddListener(this.OnCancel);
            }

            public void Show(int value, Sprite icon, Color backgroundColor)
            {
                this.panel.Show();
                this.icon.sprite = icon;
                this.backgroundColorContainer.color = backgroundColor;
                this.selectedCountText.text = $"{value}";
                this.countNeeded = value;
            }

            public void Hide()
            {
                this.panel.Hide();
                foreach (ScoreTileSelectionUI scoreTileSelectionUI in this.tileSelectionUIs)
                {
                    Destroy(scoreTileSelectionUI.gameObject);
                }
                this.tileSelectionUIs.Clear();
                Destroy(this.gameObject);
            }

            public void AddScoreTileSelectionUI(ScoreTileSelectionUI scoreTileSelectionUI)
            {
                this.tileSelectionUIs.Add(scoreTileSelectionUI);
                scoreTileSelectionUI.transform.SetParent(this.scoreTileSelectionUIContainer.transform);
                scoreTileSelectionUI.AddOnSelectionCountChangeListener(this.OnSelectionCountChange);
            }

            public void AddOnConfirmListener(UnityAction<OnScoreTileSelectionConfirmPayload> listener)
            {
                this.onConfirm.AddListener(listener);
            }

            public void AddOnCancelListener(UnityAction<OnScoreTileSelectionCancelPayload> listener)
            {
                this.onCancel.AddListener(listener);
            }

            public Dictionary<TileColor, int> GetSelectedCounts()
            {
                Dictionary<TileColor, int> tileSelections = new();
                foreach (ScoreTileSelectionUI scoreTileSelectionUI in this.tileSelectionUIs)
                {
                    tileSelections[scoreTileSelectionUI.GetColor()] = scoreTileSelectionUI.GetSelectedCount();
                }
                return tileSelections;
            }

            private void OnSelectionCountChange(OnSelectionCountChangePayload payload)
            {
                foreach (ScoreTileSelectionUI ui in this.tileSelectionUIs)
                {
                    if (payload.Color != ui.GetColor())
                    {
                        ui.SetDefaultValue(this.countNeeded - payload.Count);
                    }
                }
            }

            private void OnConfirm()
            {
                this.onConfirm.Invoke(new OnScoreTileSelectionConfirmPayload
                {
                    Panel = this,
                    SelectedCounts = this.GetSelectedCounts()
                });
            }

            private void OnCancel()
            {
                this.onCancel.Invoke(new OnScoreTileSelectionCancelPayload
                {
                    Panel = this
                });
            }
        }

    }
}
