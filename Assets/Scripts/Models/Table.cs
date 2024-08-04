using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azul.Layout;
using Azul.Model;
using UnityEngine;

namespace Azul
{
    namespace Model
    {
        public class Table : MonoBehaviour
        {
            [SerializeField] private GameObject playerBoards;
            [SerializeField] private GameObject factories;
            [SerializeField] private GameObject scoreBoard;
            [SerializeField] private GameObject center;

            public void AddPlayerBoards(List<PlayerBoard> playerBoards)
            {
                Layout.Layout layout = this.playerBoards.GetComponent<CircularLayout>();
                layout.AddChildren(playerBoards.Select(playerBoard => playerBoard.gameObject).ToList());
            }

            public void AddFactories(List<Factory> factories)
            {
                Layout.Layout layout = this.factories.GetComponent<CircularLayout>();
                layout.AddChildren(factories.Select(factory => factory.gameObject).ToList());
            }

            public void AddScoreBoard(ScoreBoard scoreBoard)
            {
                scoreBoard.transform.SetParent(this.scoreBoard.transform);
                scoreBoard.transform.localPosition = Vector3.zero;
            }

            public void AddToCenter(Tile tile)
            {
                tile.transform.SetParent(this.center.transform);
                // TODO - some kind of animation is probably warranted here.
                // for now, we'll just drop it?
                tile.transform.localPosition = Vector3.up * 5;
            }
        }
    }
}
