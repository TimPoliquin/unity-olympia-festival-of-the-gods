using System.Collections;
using System.Collections.Generic;
using Azul.Model;
using UnityEngine;

namespace Azul
{
    namespace Controller
    {
        [RequireComponent(typeof(Factory))]
        public class FactorySelectableTileHolderController : AbstractSelectableTileHolderController
        {
            private Factory factory;
            protected override void Awake()
            {
                base.Awake();
                this.factory = this.GetComponent<Factory>();
                this.factory.AddOnAddTilesListener(this.OnAddTiles);
                this.factory.AddOnTilesDrawnListener(this.OnTilesDrawn);
            }

            protected override Phase[] GetActivePhases()
            {
                return new Phase[] { Phase.ACQUIRE };
            }

            protected override void SelectTiles(List<Tile> selectedTiles)
            {
                this.factory.DrawTiles(selectedTiles);
            }

            private void OnAddTiles(OnFactoryAddTilesPayload payload)
            {
                this.SetTiles(payload.Tiles);
            }

            private void OnTilesDrawn(OnFactoryDrawTilesPayload payload)
            {
                this.Clear();
            }
        }

    }
}
