using System;
using System.Collections;
using System.Collections.Generic;
using Azul.Model;
using Unity.VisualScripting;
using UnityEngine;

namespace Azul
{
    namespace Prefab
    {

        [Serializable]
        public struct TokenPrefab
        {
            [SerializeField] private TileColor Color;
            [SerializeField] private Tile Prefab;

            public TileColor GetTileColor()
            {
                return this.Color;
            }

            public Tile GetPrefab()
            {
                return this.Prefab;
            }
        }
    }
}
