using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Azul.Controller;
using Azul.Model;
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

            private Outline outline;
            private UnityEvent<OnAltarSpaceHoverEnterPayload> onAltarSpaceHoverEnter = new();
            private UnityEvent<OnAltarSpaceHoverExitPayload> onAltarSpaceHoverExit = new();
            private UnityEvent<OnAltarSpaceSelectPayload> onAltarSpaceSelect = new();

            void Awake()
            {
                this.outline = this.GetComponent<Outline>();
            }

            void Start()
            {
                System.Instance.GetUIController().GetStarUIController().CreateStarSpaceUI(this);
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
                this.fire.SetColor(this.originalColor);
            }

            public bool IsEmpty()
            {
                return !this.isFilled;
            }

            public void ActivateHighlight()
            {
                this.outline.enabled = true;
            }

            public void DisableHighlight()
            {
                this.outline.enabled = false;
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

            public bool IsWild()
            {
                return this.originalColor == TileColor.WILD;
            }

            public TilePlaceholderPointerEventController GetPointerEventController()
            {
                return this.GetComponent<TilePlaceholderPointerEventController>();
            }

            public void PlaceTile(TileColor color)
            {
                this.isFilled = true;
                this.effectiveColor = color;
                this.fire.SetColor(color);
                this.fire.Enable();
            }

            public void Select()
            {
                this.onAltarSpaceSelect.Invoke(new OnAltarSpaceSelectPayload { Target = this });
            }

            private void HoverEnter()
            {
                this.onAltarSpaceHoverEnter.Invoke(new OnAltarSpaceHoverEnterPayload
                {
                    Target = this
                });
            }

            private void HoverExit()
            {
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
        }

    }
}
