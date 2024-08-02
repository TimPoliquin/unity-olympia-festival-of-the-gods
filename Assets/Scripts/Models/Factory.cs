using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azul
{
    namespace Model
    {

        public class Factory : MonoBehaviour
        {
            private List<Tile> tiles = new List<Tile>();
            // Start is called before the first frame update
            void Start()
            {

            }

            // Update is called once per frame
            void Update()
            {

            }

            public void Fill(List<Tile> tiles)
            {
                tiles.ForEach(tile =>
                {
                    tile.transform.SetParent(this.transform);
                    this.tiles.Add(tile);
                });
            }

        }
    }
}
