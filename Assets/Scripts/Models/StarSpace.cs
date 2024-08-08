using System.Collections;
using System.Collections.Generic;
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

            public TileColor GetEffectiveColor()
            {
                return this.tile.GetEffectiveColor();
            }
        }

    }
}
