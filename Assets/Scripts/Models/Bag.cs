using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azul
{
    namespace Model
    {
        public class Bag : MonoBehaviour
        {
            private Queue<Tile> tiles = new();
            // Start is called before the first frame update
            void Start()
            {

            }

            // Update is called once per frame
            void Update()
            {

            }

            public List<Tile> Draw(int count)
            {
                List<Tile> drawnTiles = new List<Tile>();
                for (int idx = 0; idx < count && this.tiles.Count > 0; idx++)
                {
                    drawnTiles.Add(this.tiles.Dequeue());
                }
                return drawnTiles;
            }
        }

    }
}
