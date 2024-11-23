using System;
using System.Collections;
using System.Collections.Generic;
using Azul.ScoreTileSelectionUIEvent;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

namespace Azul
{
    namespace Model
    {
        public class TokenCountSelectionStepUI : MonoBehaviour
        {
            [SerializeField] private IconUI icon;
            [SerializeField] private TextMeshProUGUI valueText;
            [SerializeField] private ScoreTileSelectionUI primarySelection;
            [SerializeField] private ScoreTileSelectionUI wildSelection;

            private AltarSpace altarSpace;

            void Awake()
            {
                this.primarySelection.AddOnSelectionCountChangeListener(this.OnSelectionCountChange);
                this.wildSelection.AddOnSelectionCountChangeListener(this.OnSelectionCountChange);
                this.wildSelection.gameObject.SetActive(false);
            }

            public void SetAltarSpace(AltarSpace space)
            {
                this.altarSpace = space;
                this.icon.SetTileColor(space.GetOriginColor());
                this.valueText.text = $"{space.GetValue()}";
            }

            public void ConfigurePrimarySelection(TileColor primaryColor, int min, int max, int total, int defaultValue)
            {
                this.primarySelection.SetColor(primaryColor);
                this.primarySelection.SetCounterRange(min, max, total);
                this.primarySelection.SetDefaultValue(defaultValue);
            }

            public void ConfigureWildSelection(TileColor wildColor, int min, int max, int total, int defaultValue)
            {
                this.wildSelection.gameObject.SetActive(true);
                this.wildSelection.SetColor(wildColor);
                this.wildSelection.SetCounterRange(min, max, total);
                this.wildSelection.SetDefaultValue(defaultValue);
            }

            public void DisableWildSelection()
            {
                this.wildSelection.gameObject.SetActive(false);
            }

            private void OnSelectionCountChange(OnSelectionCountChangePayload payload)
            {
                ScoreTileSelectionUI other;
                if (payload.Color == this.primarySelection.GetColor())
                {
                    other = this.wildSelection;
                }
                else if (payload.Color == this.wildSelection.GetColor())
                {
                    other = this.primarySelection;
                }
                else
                {
                    other = null;
                }
                if (other != null && other.gameObject.activeInHierarchy)
                {
                    other.SetDefaultValue(this.altarSpace.GetValue() - payload.Count);
                }
            }

            public Dictionary<TileColor, int> GetSelectedCounts()
            {
                Dictionary<TileColor, int> selectedCounts = new();
                selectedCounts[this.primarySelection.GetColor()] = this.primarySelection.GetSelectedCount();
                if (this.wildSelection.gameObject.activeInHierarchy)
                {
                    selectedCounts[this.wildSelection.GetColor()] = this.wildSelection.GetSelectedCount();
                }
                return selectedCounts;
            }
        }
    }
}
