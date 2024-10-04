using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azul.Controller;
using Azul.Layout;
using Azul.Util;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;

namespace Azul
{
    namespace Model
    {
        public class Altar : TimeBasedCoroutine
        {
            [SerializeField] private GameObject center;
            [SerializeField] private GameObject layoutGameObject;
            [SerializeField] private TileColor color;
            [SerializeField] private AltarSpace[] spaces;

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
            public void SetSpaces(AltarSpace[] spaces)
            {
                this.spaces = spaces;
            }

            public void SetAltar(GameObject altar)
            {
                altar.transform.SetParent(this.center.transform);
                altar.transform.localPosition = Vector3.zero;
            }

            public void AddTilePlaceholders(List<AltarSpace> spaces, float rotate = 0)
            {
                this.layout.SetRotateAfterLayout(rotate);
                this.layout.AddChildren(spaces.Select(placeholder => placeholder.gameObject).ToList());
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

            public List<AltarSpace> GetOpenSpaces()
            {
                List<AltarSpace> openSpaces = new();
                foreach (AltarSpace space in this.spaces)
                {
                    if (space.IsEmpty())
                    {
                        openSpaces.Add(space);
                    }
                }
                return openSpaces;
            }

            public List<AltarSpace> GetFilledSpaces()
            {
                List<AltarSpace> filledSpaces = new();
                foreach (AltarSpace space in this.spaces)
                {
                    if (!space.IsEmpty())
                    {
                        filledSpaces.Add(space);
                    }
                }
                return filledSpaces;
            }

            public List<AltarSpace> GetSpaces()
            {
                return this.spaces.ToList();
            }

            public bool IsSpaceFilled(int tileNumber)
            {
                return !this.spaces.ToList().Find(space => space.GetValue() == tileNumber).IsEmpty();
            }

            public bool IsFilled()
            {
                return this.spaces.All(space => !space.IsEmpty());
            }

            public void DisableAllHighlights()
            {
                foreach (AltarSpace space in this.spaces)
                {
                    space.DisableHighlight();
                }
            }

            public void ClearTileEventListeners()
            {
                foreach (AltarSpace space in this.spaces)
                {
                    space.ClearStarSpaceSelectListeners();
                }
            }

            public CoroutineResult FadeOut(float time)
            {
                Material[] materials = this.GetComponentsInChildren<MeshRenderer>().Select(renderer => renderer.material).ToArray();
                Dictionary<Material, Transition<Color>> materialColors = new();
                foreach (Material material in materials)
                {
                    materialColors[material] = new()
                    {
                        Original = material.color,
                        Target = new Color(material.color.r, material.color.g, material.color.b, 0.0f),
                    };
                }
                return this.Execute(t =>
                {
                    foreach (Material material in materials)
                    {
                        material.color = Color.Lerp(materialColors[material].Original, materialColors[material].Target, t / time);
                    }
                }, time);
            }
        }
    }
}
