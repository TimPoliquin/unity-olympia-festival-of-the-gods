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
            return this.bag.Draw(1)[0];
        }

        public List<Tile> Draw(int count)
        {
            return this.bag.Draw(count);
        }

        public void Discard(List<Tile> tiles)
        {
            this.tower.Add(tiles);
        }
    }
}
