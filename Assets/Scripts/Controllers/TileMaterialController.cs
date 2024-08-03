using System.Collections;
using System.Collections.Generic;
using Azul.Model;
using Azul.Provider;
using UnityEngine;

namespace Azul
{
    namespace Controller
    {
        [RequireComponent(typeof(Tile))]
        public class TileMaterialController : MonoBehaviour
        {
            [SerializeField] private GameObject mesh;
            void Start()
            {
                Tile tile = this.GetComponent<Tile>();
                TileMaterialProvider materialProvider = System.Instance.GetTileMaterialProvider();
                this.SetMaterial(materialProvider.GetMaterial(this.GetTileColor(), tile.IsPlaceholder));
                this.enabled = false;
            }

            private void SetMaterial(Material material)
            {
                this.mesh.GetComponent<Renderer>().material = material;
            }

            private TileColor GetTileColor()
            {
                return this.GetComponent<Tile>().Color;
            }
        }
    }
}
