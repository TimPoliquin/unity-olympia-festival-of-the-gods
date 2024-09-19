using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azul.Model;
using Azul.PointerEvents;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace Azul
{
    namespace Controller
    {
        public abstract class AbstractSelectableTileHolderController : MonoBehaviour
        {
            [SerializeField] private readonly List<Tile> tiles = new();

            private TileColor wildColor;
            private List<Tile> hoveredTiles = null;
            private bool canAcquire = false;

            protected virtual void Awake()
            {
                this.InitializeRoundPhaseHandlers();
            }

            public void AddTiles(List<Tile> tiles)
            {
                this.tiles.AddRange(tiles);
                this.AddTileListeners(tiles);
            }

            public void RemoveTiles(List<Tile> tilesToRemove)
            {
                foreach (Tile tile in tilesToRemove)
                {
                    this.tiles.Remove(tile);
                    this.RemoveTileListeners(tile);
                }
            }


            private void OnTileHoverEnter(OnPointerEnterPayload<Tile> payload)
            {
                if (!this.canAcquire)
                {
                    return;
                }
                if (null != this.hoveredTiles)
                {
                    this.DeselectTiles();
                }
                this.HoverTiles(payload.Target);

            }

            private void OnTileHoverExit(OnPointerExitPayload<Tile> payload)
            {
                if (!this.canAcquire)
                {
                    return;
                }
                if (null != this.hoveredTiles && this.hoveredTiles.Contains(payload.Target))
                {
                    this.DeselectTiles();
                }
            }

            private void OnTileSelect(OnPointerSelectPayload<Tile> payload)
            {
                if (null == this.hoveredTiles || this.hoveredTiles.Count == 0)
                {
                    this.HoverTiles(payload.Target);
                }
                this.SelectHoveredTiles();
            }

            public void HoverTiles(TileColor tileColor)
            {
                if (!this.canAcquire)
                {
                    return;
                }
                if (null != this.hoveredTiles)
                {
                    this.DeselectTiles();
                }
                Tile tile = this.tiles.Find(tile => tile.Color == tileColor);
                if (null != tile)
                {
                    this.HoverTiles(tile);
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(tileColor), $"No tiles with the desired color: {tileColor}");
                }
            }

            public void SelectHoveredTiles()
            {
                if (!this.canAcquire || null == this.hoveredTiles || this.hoveredTiles.Count == 0)
                {
                    return;
                }
                List<Tile> selectedTiles = new(this.hoveredTiles);
                this.DeselectTiles();
                this.SelectTiles(selectedTiles);
            }

            private void HoverTiles(Tile tile)
            {
                this.hoveredTiles = new();
                bool hasHoveredWild = tile.Color == this.wildColor;
                if (tile.IsOneTile())
                {
                    // TODO - we might want to change the cursor so you can't
                    // draw the one tile.
                }
                else if (hasHoveredWild)
                {
                    bool onlyHasWilds = this.tiles.All(tile => tile.Color == wildColor || tile.Color == TileColor.ONE);
                    if (onlyHasWilds)
                    {
                        hoveredTiles.Add(tile);
                        Tile oneTile = this.tiles.Find(tile => tile.IsOneTile());
                        if (oneTile != null)
                        {
                            hoveredTiles.Add(oneTile);
                        }
                    }
                    else
                    {
                        // TODO - we might want to change the cursor so you can't draw
                        // just a wild if there are other tile colors present.
                    }
                }
                else if (null != this.tiles)
                {
                    bool grabbedWild = false;
                    foreach (Tile currentTile in this.tiles)
                    {
                        if (currentTile.Color == tile.Color)
                        {
                            this.hoveredTiles.Add(currentTile);
                        }
                        else if (currentTile.Color == TileColor.ONE)
                        {
                            this.hoveredTiles.Add(currentTile);
                        }
                        else if (currentTile.Color == this.wildColor && !grabbedWild)
                        {
                            grabbedWild = true;
                            this.hoveredTiles.Add(currentTile);
                        }
                    }
                }
                foreach (Tile currentTile in this.hoveredTiles)
                {
                    currentTile.GetOutline().enabled = true;
                }
            }

            private void DeselectTiles()
            {
                if (null != this.hoveredTiles)
                {
                    foreach (Tile tile in this.hoveredTiles)
                    {
                        tile.GetOutline().enabled = false;
                    }
                    this.hoveredTiles = null;
                }
            }

            private void InitializeRoundPhaseHandlers()
            {
                Phase[] activePhases = this.GetActivePhases();
                RoundController roundController = System.Instance.GetRoundController();
                roundController.AddOnRoundPhaseAcquireListener((payload) =>
                {
                    this.canAcquire = activePhases.Contains(payload.Phase);
                    this.wildColor = payload.WildColor;
                });
                roundController.AddOnRoundPhasePrepareListener((payload) =>
                {
                    this.canAcquire = activePhases.Contains(payload.Phase);
                });
                roundController.AddOnRoundPhaseScoreListener((payload) =>
                {
                    this.canAcquire = activePhases.Contains(payload.Phase);
                });
            }

            private void RemoveTileListeners()
            {
                foreach (Tile tile in this.tiles)
                {
                    this.RemoveTileListeners(tile);
                }
            }

            private void RemoveTileListeners(Tile tile)
            {
                TilePointerEventController controller = tile.GetTilePointerController();
                controller.RemoveOnPointerEnterListener(this.OnTileHoverEnter);
                controller.RemoveOnPointerExitListener(this.OnTileHoverExit);
                controller.RemoveOnPointerSelectListener(this.OnTileSelect);
            }

            private void AddTileListeners(List<Tile> tiles)
            {
                foreach (Tile tile in tiles)
                {
                    TilePointerEventController tilePointerController = tile.GetTilePointerController();
                    tilePointerController.AddOnPointerEnterListener(this.OnTileHoverEnter);
                    tilePointerController.AddOnPointerExitListener(this.OnTileHoverExit);
                    tilePointerController.AddOnPointerSelectListener(this.OnTileSelect);
                }
            }

            protected abstract void SelectTiles(List<Tile> selectedTiles);

            protected abstract Phase[] GetActivePhases();
        }

    }
}
