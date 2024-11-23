using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Azul.Controller;
using Azul.Model;
using Azul.Cursor;
using Azul.AltarSpaceEvents;
using UnityEngine;
using UnityEngine.Events;

namespace Azul
{
    namespace AltarSpaceEvents
    {
        public struct OnAltarSpaceSelectPayload
        {
            public AltarSpace Target { get; init; }
        }

        public struct OnAltarSpaceHoverEnterPayload
        {
            public AltarSpace Target { get; init; }
        }

        public struct OnAltarSpaceHoverExitPayload
        {
            public AltarSpace Target { get; init; }
        }

    }
    namespace Model
    {
        [RequireComponent(typeof(Outline))]
        public class AltarSpace : MonoBehaviour
        {
            [SerializeField] private TileColor originalColor;
            [SerializeField] private TileColor effectiveColor;
            [SerializeField][Range(1, 6)] private int value;
            [SerializeField] private bool isFilled = false;
            [SerializeField] private Fire fire;
            private StarSpaceUI starSpaceUI;

            private Outline outline;
            private CursorChange changeCursor;
            private UnityEvent<OnAltarSpaceHoverEnterPayload> onAltarSpaceHoverEnter = new();
            private UnityEvent<OnAltarSpaceHoverExitPayload> onAltarSpaceHoverExit = new();
            private UnityEvent<OnAltarSpaceSelectPayload> onAltarSpaceSelect = new();

            void Start()
            {
                this.outline = this.GetComponent<Outline>();
                this.changeCursor = this.GetComponent<CursorChange>();
                this.starSpaceUI = System.Instance.GetUIController().GetStarUIController().CreateStarSpaceUI(this);
                this.GetPointerEventController().AddOnPointerSelectListener(payload =>
                {
                    this.Select();
                });
                this.GetPointerEventController().AddOnPointerEnterListener(payload =>
                {
                    this.HoverEnter();
                });
                this.GetPointerEventController().AddOnPointerExitListener(payload =>
                {
                    this.HoverExit();
                });
                if (null != this.fire)
                {
                    this.fire.Disable();
                }

            }

            public int GetValue()
            {
                return this.value;
            }

            public void SetValue(int value)
            {
                this.value = value;
            }

            public void SetOriginalColor(TileColor tileColor)
            {
                this.originalColor = tileColor;
                this.effectiveColor = tileColor;
                if (null != this.fire)
                {
                    this.fire.SetColor(this.originalColor);
                }
            }

            public bool IsEmpty()
            {
                return !this.isFilled;
            }

            public void ActivateHighlight()
            {
                this.outline.enabled = true;
                this.changeCursor.enabled = true;
                this.GetPointerEventController().SetInteractable(true);
                this.starSpaceUI.ActivateHighlight();
            }

            public void DisableHighlight()
            {
                this.changeCursor.enabled = false;
                this.outline.enabled = false;
                this.GetPointerEventController().SetInteractable(false);
                this.starSpaceUI.DeactivateHighlight();
            }

            public TileColor GetOriginColor()
            {
                return this.originalColor;
            }

            public TileColor GetEffectiveColor()
            {
                if (this.isFilled)
                {
                    return this.effectiveColor;
                }
                else
                {
                    return this.originalColor;
                }
            }

            public bool IsMtOlympus()
            {
                return this.originalColor == TileColor.WILD;
            }

            public RitualPointerEventController GetPointerEventController()
            {
                return this.GetComponent<RitualPointerEventController>();
            }

            public void PlaceTile(TileColor color)
            {
                this.isFilled = true;
                this.effectiveColor = color;
                if (null != this.fire)
                {
                    this.fire.SetColor(color);
                    this.fire.Enable();
                }
            }

            public void EnableFire()
            {
                if (this.isFilled)
                {
                    this.fire.Enable(false);
                }
            }

            public void DisableFire()
            {
                if (this.isFilled)
                {
                    this.fire.Disable();
                }
            }

            public void Select()
            {
                this.changeCursor.OnHoverExit();
                this.onAltarSpaceSelect.Invoke(new OnAltarSpaceSelectPayload { Target = this });
            }

            private void HoverEnter()
            {
                this.changeCursor.OnHoverEnter();
                this.onAltarSpaceHoverEnter.Invoke(new OnAltarSpaceHoverEnterPayload
                {
                    Target = this
                });
            }

            private void HoverExit()
            {
                this.changeCursor.OnHoverExit();
                this.onAltarSpaceHoverExit.Invoke(new OnAltarSpaceHoverExitPayload
                {
                    Target = this
                });
            }

            public void AddOnStarSpaceHoverEnterListener(UnityAction<OnAltarSpaceHoverEnterPayload> listener)
            {
                this.onAltarSpaceHoverEnter.AddListener(listener);
            }
            public void AddOnStarSpaceHoverExitListener(UnityAction<OnAltarSpaceHoverExitPayload> listener)
            {
                this.onAltarSpaceHoverExit.AddListener(listener);
            }

            public void AddOnStarSpaceSelectListener(UnityAction<OnAltarSpaceSelectPayload> listener)
            {
                this.onAltarSpaceSelect.AddListener(listener);
            }

            public void RemoveOnStarSpaceSelectListener(UnityAction<OnAltarSpaceSelectPayload> listener)
            {
                this.onAltarSpaceSelect.RemoveListener(listener);
            }

            public void ClearStarSpaceSelectListeners()
            {
                this.onAltarSpaceSelect.RemoveAllListeners();
            }

            public static AltarSpace Create(TileColor tileColor, int value)
            {
                AltarSpace space = new GameObject($"{tileColor}:{value}").AddComponent<AltarSpace>();
                space.SetOriginalColor(tileColor);
                space.SetValue(value);
                return space;
            }
        }

    }
}
