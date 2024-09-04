using System.Collections;
using System.Collections.Generic;
using Azul.Model;
using UnityEngine;

namespace Azul
{
    namespace Controller
    {
        public class PrefabFactory : MonoBehaviour
        {
            [SerializeField]
            private List<TokenPrefab> tokenPrefabs;

            private Dictionary<TileColor, Tile> tilePrefabsByColor;

            void Awake()
            {
                this.tilePrefabsByColor = new Dictionary<TileColor, Tile>();
                foreach (TokenPrefab tokenPrefab in this.tokenPrefabs)
                {
                    this.tilePrefabsByColor[tokenPrefab.GetTileColor()] = tokenPrefab.GetPrefab();
                }
            }

            public Tile CreateTile(TileColor tileColor)
            {
                return Instantiate(this.tilePrefabsByColor[tileColor]);
            }
        }
    }
}
