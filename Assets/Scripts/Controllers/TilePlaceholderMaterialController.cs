using System.Collections;
using System.Collections.Generic;
using Azul.Controller;
using Azul.Model;
using UnityEngine;

namespace Azul
{
    namespace Controller
    {
        [RequireComponent(typeof(TilePlaceholder))]
        public class TilePlaceholderMaterialController : AbstractMaterialController
        {
            protected override Material GetMaterial()
            {
                TilePlaceholder tile = this.GetComponent<TilePlaceholder>();
                return System.Instance.GetTileMaterialProvider().GetMaterial(tile.GetColor(), true);
            }
        }
    }
}
