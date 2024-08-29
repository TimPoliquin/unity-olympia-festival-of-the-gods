using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Azul.Controller;
using Azul.Model;
using Azul.StarSpaceEvents;
using UnityEngine;
using UnityEngine.Events;

namespace Azul
{
    namespace StarSpaceEvents
    {
        public struct OnStarSpaceSelectPayload
        {
            public StarSpace Target { get; init; }
        }
    }
    namespace Model
    {
        [RequireComponent(typeof(TilePlaceholder))]
        [RequireComponent(typeof(Outline))]
        public class StarSpace : MonoBehaviour
        {
            [SerializeField] private TilePlaceholder tile;
            [SerializeField][Range(1, 6)] private int value;

            private Outline outline;
            private UnityEvent<OnStarSpaceSelectPayload> onStarSpaceSelect = new();

            void Awake()
            {
                this.tile = this.GetComponent<TilePlaceholder>();
                this.outline = this.GetComponent<Outline>();
                this.GetPointerEventController().AddOnPointerSelectListener(payload =>
                {
                    this.Select();
                });
            }

            void Start()
            {
                System.Instance.GetUIController().GetStarUIController().CreateStarSpaceUI(this);
            }

            public int GetValue()
            {
                return this.value;
            }

            public void SetValue(int value)
            {
                this.value = value;
            }

            public bool IsEmpty()
            {
                return this.tile.IsEmpty();
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
                return this.tile.GetColor();
            }

            public TileColor GetEffectiveColor()
            {
                return this.tile.GetEffectiveColor();
            }

            public bool IsWild()
            {
                return this.tile.GetColor() == TileColor.WILD;
            }

            public TilePlaceholderPointerEventController GetPointerEventController()
            {
                return this.tile.GetPointerEventController();
            }

            public void PlaceTile(Tile tile)
            {
                this.tile.PlaceTile(tile);
            }

            public void Select()
            {
                this.onStarSpaceSelect.Invoke(new OnStarSpaceSelectPayload { Target = this });
            }

            public void AddOnStarSpaceSelectListener(UnityAction<OnStarSpaceSelectPayload> listener)
            {
                this.onStarSpaceSelect.AddListener(listener);
            }

            public void RemoveOnStarSpaceSelectListener(UnityAction<OnStarSpaceSelectPayload> listener)
            {
                this.onStarSpaceSelect.RemoveListener(listener);
            }

            public void ClearStarSpaceSelectListeners()
            {
                this.onStarSpaceSelect.RemoveAllListeners();
            }
        }

    }
}
