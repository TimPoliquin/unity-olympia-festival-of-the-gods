using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azul.Layout;
using Azul.Model;
using Azul.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace Azul
{
    namespace Model
    {
        public class Table : MonoBehaviour
        {
            public class OnTableAddTilesPayload
            {
                public List<Tile> Tiles { get; init; }
            }
            public class OnTableDrawTilesPayload
            {
                public List<Tile> TilesDrawn { get; init; }
            }
            [SerializeField] private GameObject playerBoards;
            [SerializeField] private GameObject factories;
            [SerializeField] private GameObject scoreBoard;
            [SerializeField] private GameObject center;
            private List<Tile> tiles = new();
            private UnityEvent<OnTableAddTilesPayload> onAddTiles = new();
            private UnityEvent<OnTableDrawTilesPayload> onDrawTiles = new();

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
                tile.transform.localPosition = VectorUtils.CreateRandomVector3(10, 5);
                this.tiles.Add(tile);
                this.onAddTiles.Invoke(new OnTableAddTilesPayload { Tiles = new() { tile } });
            }

            public void AddToCenter(List<Tile> tiles)
            {
                foreach (Tile tile in tiles)
                {
                    tile.transform.SetParent(this.center.transform);
                    // TODO - some kind of animation is probably warranted here.
                    // for now, we'll just drop it?
                    tile.transform.localPosition = VectorUtils.CreateRandomVector3(10, 5);
                }
                this.tiles.AddRange(tiles);
                this.onAddTiles.Invoke(new OnTableAddTilesPayload { Tiles = tiles });
            }

            public void DrawTiles(List<Tile> drawnTiles)
            {
                foreach (Tile tile in drawnTiles)
                {
                    this.tiles.Remove(tile);
                    tile.transform.SetParent(null);
                }
                this.onDrawTiles.Invoke(new OnTableDrawTilesPayload { TilesDrawn = drawnTiles }); ;
            }

            public void AddOnTilesAddedListener(UnityAction<OnTableAddTilesPayload> listener)
            {
                this.onAddTiles.AddListener(listener);
            }

            public void AddOnTilesDrawnListener(UnityAction<OnTableDrawTilesPayload> listener)
            {
                this.onDrawTiles.AddListener(listener);
            }

            public bool IsEmpty()
            {
                return this.tiles.Count == 0;
            }
        }
    }
}
