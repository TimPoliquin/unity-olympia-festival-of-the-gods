using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Azul.Model;

namespace Azul
{
    namespace Provider
    {
        [Serializable]
        public class TileMaterialProvider : MonoBehaviour
        {
            [Serializable]
            private struct TileMaterial
            {
                public Azul.Model.TileColor color;
                public Material material;
            }

            [SerializeField] private List<TileMaterial> tileMaterials;
            [SerializeField] private List<TileMaterial> placeholderMaterials;

            public Material GetMaterial(Model.TileColor color, bool placeholder = false)
            {
                List<TileMaterial> tileMaterials = placeholder ? this.placeholderMaterials : this.tileMaterials;
                return tileMaterials.Find(m => m.color == color).material;
            }
        }
    }
}
