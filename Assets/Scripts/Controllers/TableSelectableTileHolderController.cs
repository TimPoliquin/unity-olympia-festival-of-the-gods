using System;
using System.Collections;
using System.Collections.Generic;
using Azul.Controller;
using Azul.Model;
using UnityEngine;

namespace Azul
{
    namespace Controller
    {
        [RequireComponent(typeof(Table))]
        public class TableSelectableTileHolderController : AbstractSelectableTileHolderController
        {
            private Table table;

            protected override void Awake()
            {
                base.Awake();
                this.table = this.GetComponent<Table>();
                this.table.AddOnTilesAddedListener(this.OnTilesAdded);
                this.table.AddOnTilesDrawnListener(this.OnTilesDrawn);
            }

            protected override Phase[] GetActivePhases()
            {
                return new Phase[] { Phase.ACQUIRE };
            }

            protected override void SelectTiles(List<Tile> selectedTiles)
            {
                this.table.DrawTiles(selectedTiles);
            }

            private void OnTilesAdded(Table.OnTableAddTilesPayload payload)
            {
                this.AddTiles(payload.Tiles);
            }

            private void OnTilesDrawn(Table.OnTableDrawTilesPayload payload)
            {
                this.RemoveTiles(payload.TilesDrawn);
            }
        }
    }
}
