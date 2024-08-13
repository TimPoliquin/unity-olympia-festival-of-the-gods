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
        public class TilePointerEventController : PointerEventController<Tile>
        {
        }

    }
}
