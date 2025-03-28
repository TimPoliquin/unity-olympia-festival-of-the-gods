using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azul.Controller;
using Azul.Controller.TableUtilities;
using Azul.Utils;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace Azul
{
    namespace Model
    {
        public class OnFactoryAddTilesPayload
        {
            public List<Tile> Tiles { get; init; }
        }
        public class OnFactoryDrawTilesPayload
        {
            public List<Tile> TilesDrawn { get; init; }
            public List<Tile> TilesDiscarded { get; init; }
        }
        public class Factory : MonoBehaviour, TileProvider
        {
            [SerializeField] private GameObject ivyMesh;
            [SerializeField] private List<GameObject> tileHolder;
            [SerializeField] private Light pointLight;
            private List<Tile> tiles = new List<Tile>();

            private UnityEvent<OnFactoryAddTilesPayload> onAddTiles = new();
            private UnityEvent<OnFactoryDrawTilesPayload> onTileDraw = new();

            void Start()
            {
                this.ivyMesh.transform.Rotate(Vector3.forward * UnityEngine.Random.Range(135f, 225f));
            }


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
                this.pointLight.enabled = true;
                this.onAddTiles.Invoke(new OnFactoryAddTilesPayload { Tiles = this.tiles });
            }

            public void DrawTiles(List<Tile> drawnTiles)
            {
                if (this.tiles.Count == 0)
                {
                    throw new Exception("Attempted to draw from an empty factory. Event listeners are probably not being removed correctly");
                }
                OnFactoryDrawTilesPayload payload = new()
                {
                    TilesDrawn = new(),
                    TilesDiscarded = new()
                };
                foreach (Tile tile in this.tiles)
                {
                    if (drawnTiles.Contains(tile))
                    {
                        payload.TilesDrawn.Add(tile);
                    }
                    else
                    {
                        payload.TilesDiscarded.Add(tile);
                    }
                    tile.transform.SetParent(null);
                }
                this.pointLight.enabled = false;
                this.tiles.Clear();
                this.onTileDraw.Invoke(payload);
            }

            public void AddOnAddTilesListener(UnityAction<OnFactoryAddTilesPayload> listener)
            {
                this.onAddTiles.AddListener(listener);
            }

            public void AddOnTilesDrawnListener(UnityAction<OnFactoryDrawTilesPayload> listener)
            {
                this.onTileDraw.AddListener(listener);
            }

            public bool IsEmpty()
            {
                return this.tiles.Count == 0;
            }

            public int GetTileCount(TileColor desiredColor)
            {
                return this.tiles.FindAll(tile => tile.Color == desiredColor).Count;
            }

            public Dictionary<TileColor, int> GetTileCounts()
            {
                Dictionary<TileColor, int> tileCounts = new();
                foreach (Tile tile in this.tiles)
                {
                    if (!tileCounts.ContainsKey(tile.Color))
                    {
                        tileCounts[tile.Color] = 1;
                    }
                    else
                    {
                        tileCounts[tile.Color] += 1;
                    }
                }
                return tileCounts;
            }

            public bool HasTileOfColor(TileColor tileColor)
            {
                return this.tiles.Any(tile => tile.Color == tileColor);
            }

            public FactorySelectableTileHolderController GetSelectableTileHolderController()
            {
                return this.GetComponent<FactorySelectableTileHolderController>();
            }

            public bool HasHadesToken()
            {
                return false;
            }

            public bool IsFactory()
            {
                return true;
            }

            public bool IsTable()
            {
                return false;
            }
        }
    }
}
