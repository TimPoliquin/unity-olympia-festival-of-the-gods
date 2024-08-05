using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azul.Model;
using Unity.VisualScripting;
using UnityEngine;

namespace Azul
{
    namespace Controller
    {
        public abstract class AbstractSelectableTileHolderController : MonoBehaviour
        {
            [SerializeField] private List<Tile> tiles;

            private TileColor wildColor;
            private List<Tile> hoveredTiles = null;
            private bool canAcquire = false;

            protected virtual void Awake()
            {
                this.InitializeRoundPhaseHandlers();
            }

            public void SetTiles(List<Tile> tiles)
            {
                this.Clear();
                this.tiles = tiles;
                foreach (Tile tile in tiles)
                {
                    TilePointerController tilePointerController = tile.GetTilePointerController();
                    tilePointerController.AddOnTileHoverEnterListener(this.OnTileHoverEnter);
                    tilePointerController.AddOnTileHoverExitListener(this.OnTileHoverExit);
                    tilePointerController.AddOnTileSelectListener(this.OnTileSelect);
                }
            }


            private void OnTileHoverEnter(OnTileHoverEnterPayload payload)
            {
                if (!this.canAcquire)
                {
                    return;
                }
                if (null != this.hoveredTiles)
                {
                    this.DeselectTiles();
                }
                this.HoverTiles(payload.Tile);

            }

            private void OnTileHoverExit(OnTileHoverExitPayload payload)
            {
                if (!this.canAcquire)
                {
                    return;
                }
                if (null != this.hoveredTiles && this.hoveredTiles.Contains(payload.Tile))
                {
                    this.DeselectTiles();
                }
            }

            private void OnTileSelect(OnTileSelectPayload payload)
            {
                if (!this.canAcquire)
                {
                    return;
                }
                List<Tile> selectedTiles = new(this.hoveredTiles);
                this.RemoveTileListeners();
                this.DeselectTiles();
                this.SelectTiles(selectedTiles);
            }

            private void HoverTiles(Tile tile)
            {
                this.hoveredTiles = new();
                bool hasHoveredWild = tile.Color == this.wildColor;
                if (hasHoveredWild)
                {
                    hoveredTiles.Add(tile);
                }
                else if (null != this.tiles)
                {
                    foreach (Tile currentTile in this.tiles)
                    {
                        if (currentTile.Color == tile.Color)
                        {
                            this.hoveredTiles.Add(currentTile);
                        }
                        else if (currentTile.Color == this.wildColor && !hasHoveredWild)
                        {
                            hasHoveredWild = true;
                            this.hoveredTiles.Add(currentTile);
                        }
                    }
                }
                foreach (Tile currentTile in this.hoveredTiles)
                {
                    currentTile.GetComponent<Outline>().enabled = true;
                }
            }

            private void DeselectTiles()
            {
                if (null != this.hoveredTiles)
                {
                    foreach (Tile tile in this.hoveredTiles)
                    {
                        tile.GetComponent<Outline>().enabled = false;
                    }
                    this.hoveredTiles = null;
                }
            }

            private void InitializeRoundPhaseHandlers()
            {
                Phase[] activePhases = this.GetActivePhases();
                System.Instance.GetRoundController().AddOnRoundPhaseAcquireListener((payload) =>
                {
                    this.canAcquire = activePhases.Contains(payload.Phase);
                    this.wildColor = payload.WildColor;
                });
                System.Instance.GetRoundController().AddOnRoundPhasePrepareListener((payload) =>
                {
                    this.canAcquire = activePhases.Contains(payload.Phase);
                });
                System.Instance.GetRoundController().AddOnRoundPhaseScoreListener((payload) =>
                {
                    this.canAcquire = activePhases.Contains(payload.Phase);
                });
            }

            protected void Clear()
            {
                if (null != this.tiles)
                {
                    this.RemoveTileListeners();
                }
                this.tiles = null;
                this.hoveredTiles = null;
            }

            private void RemoveTileListeners()
            {
                foreach (Tile tile in this.tiles)
                {
                    TilePointerController controller = tile.GetTilePointerController();
                    controller.RemoveOnTileHoverEnterListener(this.OnTileHoverEnter);
                    controller.RemoveOnTileHoverExitListener(this.OnTileHoverExit);
                    controller.RemoveOnTileSelectListener(this.OnTileSelect);
                }
            }

            protected abstract void SelectTiles(List<Tile> selectedTiles);

            protected abstract Phase[] GetActivePhases();
        }

    }
}
