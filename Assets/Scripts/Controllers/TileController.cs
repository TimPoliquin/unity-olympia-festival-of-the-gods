using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Azul.Model;

namespace Azul
{
    namespace Controller
    {
        public class TileController : MonoBehaviour
        {
            private static int NUM_TILES = 132;
            [SerializeField] private GameObject tilePrefab;
            [SerializeField] private GameObject oneTilePrefab;

            /// <summary>
            /// A special tile. This tile indicates which player goest first in the next steps of the current round,
            /// and which player goes first in the following round.
            /// </summary>
            private Tile oneTile;
            private List<Tile> tiles;

            public void SetupGame()
            {
                this.CreateStandardTiles();
                this.CreateOneTile();
            }

            public void InitializeListeners()
            {
                RoundController roundController = System.Instance.GetRoundController();
                roundController.AddOnRoundPhasePrepareListener(this.OnRoundStart);
            }

            private void CreateStandardTiles()
            {
                this.tiles = new();
                TileColor[] tileColors = TileColorUtils.GetTileColors();
                for (int idx = 0; idx < NUM_TILES; idx++)
                {
                    TileColor tileColor = tileColors[idx % tileColors.Length];
                    tiles.Add(Tile.Create(this.tilePrefab, tileColor));
                }
                tiles.Shuffle();
            }

            private void CreateOneTile()
            {
                GameObject gameObject = Instantiate(this.oneTilePrefab);
                this.oneTile = gameObject.GetComponent<Tile>();
            }


            public List<Tile> GetTiles()
            {
                return this.tiles;
            }

            public Tile GetOneTile()
            {
                return this.oneTile;
            }

            private void OnRoundStart(OnRoundPhasePreparePayload payload)
            {
                TableController tableController = System.Instance.GetTableController();
                tableController.MoveOneTileToCenter(this.oneTile);
            }
        }
    }
}
