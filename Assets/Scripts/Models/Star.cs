using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azul.Controller;
using Azul.Layout;
using Unity.VisualScripting;
using UnityEngine;

namespace Azul
{
    namespace Model
    {
        public class Star : MonoBehaviour
        {
            [SerializeField] private GameObject layoutGameObject;
            [SerializeField] private TileColor color;
            [SerializeField] private StarSpace[] spaces;

            private CircularLayout layout;

            void Awake()
            {
                this.layout = this.layoutGameObject.GetComponent<CircularLayout>();
            }

            public TileColor GetColor()
            {
                return this.color;
            }

            public void SetColor(TileColor color)
            {
                this.color = color;
            }
            public void SetSpaces(StarSpace[] spaces)
            {
                this.spaces = spaces;
            }

            public void AddTilePlaceholders(List<TilePlaceholder> placeholders)
            {
                this.layout.AddChildren(placeholders.Select(placeholder => placeholder.gameObject).ToList());
            }

            public List<TileColor> GetTileColors()
            {
                if (this.color == TileColor.WILD)
                {
                    return this.spaces.Select(space => space.IsEmpty() ? this.color : space.GetEffectiveColor()).Distinct().ToList();
                }
                else
                {
                    return new() { this.color };
                }
            }

            public int GetNumberOfSpaces()
            {
                return this.spaces.Length;
            }

            public List<StarSpace> GetOpenSpaces()
            {
                List<StarSpace> openSpaces = new();
                foreach (StarSpace space in this.spaces)
                {
                    if (space.IsEmpty())
                    {
                        openSpaces.Add(space);
                    }
                }
                return openSpaces;
            }

            public List<StarSpace> GetFilledSpaces()
            {
                List<StarSpace> filledSpaces = new();
                foreach (StarSpace space in this.spaces)
                {
                    if (!space.IsEmpty())
                    {
                        filledSpaces.Add(space);
                    }
                }
                return filledSpaces;
            }

            public void DisableAllHighlights()
            {
                foreach (StarSpace space in this.spaces)
                {
                    space.DisableHighlight();
                }
            }
        }
    }
}
