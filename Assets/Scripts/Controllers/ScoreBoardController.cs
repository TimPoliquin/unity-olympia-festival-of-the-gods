using System.Collections;
using System.Collections.Generic;
using Azul.Layout;
using Azul.Model;
using Unity.VisualScripting;
using UnityEngine;

namespace Azul
{
    namespace Controller
    {
        public class ScoreBoardController : MonoBehaviour
        {
            [SerializeField] private Vector3 position = new Vector3(100, 0, 0);
            [SerializeField][Range(1, 10)] private int supplySize = 10;
            [SerializeField] private GameObject scoreBoardPrefab;
            [SerializeField] private GameObject tilePrefab;

            private GameObject scoreBoard;

            private List<TilePlaceholder> placeholderTiles;
            public void SetupGame()
            {
                this.CreateScoreBoard();
                this.CreateSupplyStar();
            }

            private void CreateScoreBoard()
            {
                this.scoreBoard = Instantiate(this.scoreBoardPrefab, this.position, Quaternion.identity);
                this.scoreBoard.transform.position = this.position;
            }

            private void CreateSupplyStar()
            {
                this.placeholderTiles = new();
                GameObject gameObject = new GameObject("Supply");
                gameObject.transform.SetParent(this.scoreBoard.transform);
                gameObject.transform.localPosition = Vector3.zero;
                CircularLayout layout = gameObject.AddComponent<CircularLayout>();
                layout.CreateLayout(this.supplySize, 5.0f, (input) =>
                {
                    TilePlaceholder tile = TilePlaceholder.Create(this.tilePrefab, TileColor.WILD);
                    this.placeholderTiles.Add(tile);
                    return tile.gameObject;
                });
            }

            public void FillSupply(BagController bagController)
            {
                this.placeholderTiles.ForEach(placeholder =>
                {
                    if (placeholder.IsEmpty())
                    {
                        placeholder.PlaceTile(bagController.Draw());
                    }
                });
            }
        }
    }
}
