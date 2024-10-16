using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azul.Controller;
using Azul.Layout;
using Azul.Util;
using UnityEngine;

namespace Azul
{
    namespace Model
    {
        public class Altar : TimeBasedCoroutine
        {
            [SerializeField] private GameObject center;
            [SerializeField] private CircularLayout layout;
            [SerializeField] private TileColor color;
            [SerializeField] private AltarSpace[] spaces;
            [SerializeField] private Light milestoneCompletionLight;


            private readonly int alphaVar = Shader.PropertyToID("_Alpha");


            public TileColor GetColor()
            {
                return this.color;
            }

            public void SetColor(TileColor color)
            {
                this.color = color;
            }

            public void SetAltarModel(GameObject altar)
            {
                altar.transform.SetParent(this.center.transform);
                altar.transform.localPosition = Vector3.zero;
            }

            public void AddAltarSpaces(List<AltarSpace> spaces, float rotate = 0)
            {
                this.layout.SetRotateAfterLayout(rotate);
                this.layout.AddChildren(spaces.Select(placeholder => placeholder.gameObject).ToList());
                this.spaces = spaces.ToArray();
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

            public CoroutineResult Fade(float time, float target)
            {
                Material[] materials = this.GetComponentsInChildren<MeshRenderer>().Select(renderer => renderer.material).ToArray();
                Dictionary<Material, Transition<float>> materialColors = new();
                if (target == 0)
                {
                    foreach (AltarSpace space in this.spaces)
                    {
                        space.DisableFire();
                    }
                }
                else
                {
                    foreach (AltarSpace space in this.spaces)
                    {
                        space.EnableFire();
                    }
                }
                foreach (Material material in materials)
                {
                    materialColors[material] = new()
                    {
                        Original = material.GetFloat(this.alphaVar),
                        Target = target,
                    };
                }
                return this.Execute(t =>
                {
                    foreach (Material material in materials)
                    {
                        float alpha = Mathf.Lerp(materialColors[material].Original, materialColors[material].Target, t / time);
                        material.SetFloat(this.alphaVar, alpha);
                    }
                }, time);
            }

            public CoroutineResult TurnOnMilestoneCompletionLight(Color color, float intensity, float time)
            {
                this.milestoneCompletionLight.gameObject.SetActive(true);
                this.milestoneCompletionLight.color = color;
                return this.Execute(t =>
                {
                    this.milestoneCompletionLight.intensity = Mathf.Lerp(0, intensity, t / time);
                }, time);
            }

            public static Altar Create(TileColor tileColor)
            {
                Altar altar = new GameObject($"Altar: {tileColor}").AddComponent<Altar>();
                altar.layout = new GameObject("Ring").AddComponent<CircularLayout>();
                altar.center = new GameObject("Center");
                return altar;
            }
        }
    }
}
