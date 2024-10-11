using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azul
{
    namespace Model
    {
        public interface TileProvider
        {
            public bool HasHadesToken();
            public bool IsTable();
            public bool IsFactory();
        }
    }
}
