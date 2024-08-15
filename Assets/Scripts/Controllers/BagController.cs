using System.Collections;
using System.Collections.Generic;
using Azul.Model;
using UnityEngine;

namespace Azul
{
    public class BagController : MonoBehaviour
    {

        [SerializeField] private Bag bag;
        [SerializeField] private Tower tower;

        public void SetupGame(List<Tile> tiles)
        {
            this.bag = new GameObject("Bag").AddComponent<Bag>();
            this.tower = new GameObject("Tower").AddComponent<Tower>();
            this.bag.Fill(tiles);
        }

        public Tile Draw()
        {
            return this.Draw(1)[0];
        }

        public List<Tile> Draw(int count)
        {
            List<Tile> tiles = this.bag.Draw(count);
            if (tiles.Count < count)
            {
                bag.Fill(tower.Dump());
            }
            tiles.AddRange(this.bag.Draw(count - tiles.Count));
            return tiles;
        }

        public Tile Draw(TileColor tileColor)
        {
            Tile tile = this.bag.Draw(tileColor);
            if (null == tile)
            {
                bag.Fill(tower.Dump());
                tile = this.bag.Draw(tileColor);
            }
            return tile;
        }

        public void Discard(List<Tile> tiles)
        {
            this.tower.Add(tiles);
        }
    }
}
