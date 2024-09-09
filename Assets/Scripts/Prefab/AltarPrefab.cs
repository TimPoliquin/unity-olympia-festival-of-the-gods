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
        public struct AltarPrefab
        {
            [SerializeField] private TileColor tileColor;
            [SerializeField] private GameObject altarPrefab;

            public TileColor GetTileColor()
            {
                return this.tileColor;
            }

            public GameObject GetPrefab()
            {
                return this.altarPrefab;
            }
        }

    }
}
