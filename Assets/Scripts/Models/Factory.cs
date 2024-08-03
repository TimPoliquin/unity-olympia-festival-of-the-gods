using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azul
{
    namespace Model
    {

        public class Factory : MonoBehaviour
        {
            [SerializeField] private List<GameObject> tileHolder;
            private List<Tile> tiles = new List<Tile>();

            public void Fill(List<Tile> tiles)
            {
                if (tiles.Count > this.tileHolder.Count)
                {
                    UnityEngine.Debug.Log($"Unexpected number of tiles added to Factory: {tiles.Count}");
                    return;
                }
                this.tiles.AddRange(tiles);
                for (int idx = 0; idx < tiles.Count; idx++)
                {
                    tiles[idx].transform.SetParent(this.tileHolder[idx].transform);
                    tiles[idx].transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                }
            }

        }
    }
}
