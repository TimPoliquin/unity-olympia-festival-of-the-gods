using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azul.Layout;
using Azul.Model;
using UnityEngine;

namespace Azul
{
    namespace Controller
    {
        public class PlayerBoardController : MonoBehaviour
        {
            [SerializeField] private GameObject playerBoardPrefab;
            [SerializeField] private float radius = 55.0f;
            [SerializeField] private float starRadius = 11.0f;

            private GameObject root;
            public void SetupGame(int numPlayers, StarController starController)
            {
                this.root = new GameObject("Player Boards");
                CircularLayout layout = this.root.AddComponent<CircularLayout>();
                layout.CreateLayout(numPlayers, this.radius, (input) =>
                {
                    GameObject board = Instantiate(this.playerBoardPrefab);
                    board.name = $"Player Board {input.Index}";
                    this.CreateStars(board, starController);
                    return board;
                }, false);
            }

            public void CreateStars(GameObject board, StarController starController)
            {
                TileColor[] colors = TileColorUtils.GetTileColors();
                GameObject stars = new GameObject("Stars");
                stars.transform.SetParent(board.transform);
                stars.transform.localPosition = Vector3.zero;
                GameObject outerRing = new GameObject("Outer Ring");
                outerRing.transform.SetParent(stars.transform);
                outerRing.transform.localPosition = Vector3.zero;
                outerRing.transform.Rotate(0, 30, 0);
                CircularLayout layout = outerRing.AddComponent<CircularLayout>();
                layout.CreateLayout(colors.Length, this.starRadius, (input) =>
                {
                    Star star = starController.CreateStar(colors[input.Index]);
                    return star.gameObject;
                });
                Star wildStar = starController.CreateStar(TileColor.WILD);
                wildStar.transform.SetParent(stars.transform);
                wildStar.transform.localPosition = Vector3.zero;
            }
        }
    }
}
