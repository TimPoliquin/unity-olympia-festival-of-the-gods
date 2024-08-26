using System.Collections;
using System.Collections.Generic;
using Azul.Controller;
using Azul.Model;
using UnityEngine;

namespace Azul
{
    namespace Controller
    {
        [RequireComponent(typeof(Tile))]
        public class EssenceMaterialController : AbstractMaterialController
        {
            [SerializeField] private ParticleSystem particles;
            [SerializeField] private TrailRenderer trail;

            protected override void Start()
            {
                base.Start();
                this.SetColor(this.GetColor());
            }
            protected override Material GetMaterial()
            {
                Tile tile = this.GetComponent<Tile>();
                return System.Instance.GetTileMaterialProvider().GetMaterial(tile.Color, false);
            }
            protected void SetColor(Color color)
            {
                this.trail.startColor = color;
                this.trail.endColor = color;
                ParticleSystem.MainModule main = this.particles.main;
                main.startColor = color;
            }

            protected Color GetColor()
            {
                Tile tile = this.GetComponent<Tile>();
                return System.Instance.GetTileMaterialProvider().GetColor(tile.Color, false);
            }
        }
    }
}
