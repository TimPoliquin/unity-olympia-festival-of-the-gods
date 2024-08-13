using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Azul.Controller;
using UnityEngine;

namespace Azul
{
    namespace Model
    {
        [RequireComponent(typeof(TilePlaceholder))]
        [RequireComponent(typeof(Outline))]
        public class StarSpace : MonoBehaviour
        {
            [SerializeField] private TilePlaceholder tile;
            [SerializeField][Range(1, 6)] private int value;
            private Outline outline;

            void Awake()
            {
                this.tile = this.GetComponent<TilePlaceholder>();
                this.outline = this.GetComponent<Outline>();
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

            public TilePlaceholderPointerEventController GetPointerEventController()
            {
                return this.tile.GetPointerEventController();
            }

            public void PlaceTile(Tile tile)
            {
                this.tile.PlaceTile(tile);
            }
        }

    }
}
