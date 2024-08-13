using System.Collections;
using System.Collections.Generic;
using Azul.Controller;
using UnityEngine;

namespace Azul
{
    namespace Controller
    {
        [RequireComponent(typeof(TilePlaceholder))]
        public class TilePlaceholderPointerEventController : PointerEventController<TilePlaceholder>
        {
        }
    }
}
