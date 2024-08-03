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
        public class TileMaterialController : AbstractMaterialController
        {
            protected override Material GetMaterial()
            {
                Tile tile = this.GetComponent<Tile>();
                return System.Instance.GetTileMaterialProvider().GetMaterial(tile.Color, false);
            }
        }
    }
}
