using System;
using System.Collections;
using System.Collections.Generic;
using Azul.GraphicsSettings;
using UnityEngine;

namespace Azul
{
    namespace Model
    {
        [Serializable]
        public class GraphicsOptions
        {
            public GraphicsLevel Level;
            public float RenderScale;
            public bool VSync;
            public AntiAliasingLevel AntiAliasingLevel;
        }
    }
}
