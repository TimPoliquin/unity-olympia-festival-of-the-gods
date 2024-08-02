using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azul
{
    namespace Model
    {
        public class Tower : MonoBehaviour
        {
            private List<Tile> tiles = new();
            // Start is called before the first frame update
            void Start()
            {

            }

            // Update is called once per frame
            void Update()
            {

            }

            public void Add(List<Tile> tiles)
            {
                this.tiles.AddRange(tiles);
            }
            public List<Tile> Dump()
            {
                List<Tile> tiles = this.tiles;
                this.tiles = new();
                return tiles;
            }
        }
    }
}
