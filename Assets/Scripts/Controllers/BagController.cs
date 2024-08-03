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
            this.bag.Fill(tiles);
        }

        public List<Tile> Draw(int count)
        {
            return this.bag.Draw(count);
        }
    }
}
