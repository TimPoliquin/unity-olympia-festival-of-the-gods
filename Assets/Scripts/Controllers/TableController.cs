using System;
using System.Collections;
using System.Collections.Generic;
using Azul.Model;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Azul
{
    namespace Controller
    {
        public class TableController : MonoBehaviour
        {
            public class OnTableTilesDrawnPayload
            {
                public List<Tile> Tiles { get; init; }
                public bool IncludesOneTile { get; init; }
            }
            public class OnTableTilesAddedPayload
            {
                public List<Tile> Tiles { get; init; }
            }
            [SerializeField] private GameObject tablePrefab;

            private UnityEvent<OnTableTilesAddedPayload> onTilesAdded = new();
            private UnityEvent<OnTableTilesDrawnPayload> onTilesDrawn = new();
            private UnityEvent onTableEmpty = new();

            private Table table;

            public void SetupGame()
            {
                this.table = Instantiate(this.tablePrefab, Vector3.zero, Quaternion.identity).GetComponent<Table>();
            }

            public void InitializeListeners()
            {
                FactoryController factoryController = System.Instance.GetFactoryController();
                factoryController.AddOnFactoryTilesDiscardedListener(this.OnTilesDiscarded);
                this.table.AddOnTilesDrawnListener(this.OnTilesDrawn);
            }


            public void AddPlayerBoards(List<PlayerBoard> playerBoards)
            {
                this.table.AddPlayerBoards(playerBoards);
            }

            public void AddFactories(List<Factory> factories)
            {
                this.table.AddFactories(factories);
            }

            public void AddScoreBoard(ScoreBoard scoreBoard)
            {
                this.table.AddScoreBoard(scoreBoard);
            }

            public void MoveOneTileToCenter(Tile oneTile)
            {
                this.table.AddToCenter(oneTile);
                this.onTilesAdded.Invoke(new OnTableTilesAddedPayload { Tiles = new() { oneTile } });
            }

            public void AddOnTilesAddedListener(UnityAction<OnTableTilesAddedPayload> listener)
            {
                this.onTilesAdded.AddListener(listener);
            }

            public void AddOnTilesDrawnListener(UnityAction<OnTableTilesDrawnPayload> listener)
            {
                this.onTilesDrawn.AddListener(listener);
            }

            public void AddOnTableEmptyListener(UnityAction listener)
            {
                this.onTableEmpty.AddListener(listener);
            }

            private void OnTilesDiscarded(OnFactoryTilesDiscarded payload)
            {
                this.table.AddToCenter(payload.TilesDiscarded);
                this.onTilesAdded.Invoke(new OnTableTilesAddedPayload { Tiles = payload.TilesDiscarded });
            }

            private void OnTilesDrawn(Table.OnTableDrawTilesPayload payload)
            {
                this.onTilesDrawn.Invoke(new OnTableTilesDrawnPayload
                {
                    Tiles = payload.TilesDrawn,
                    IncludesOneTile = payload.TilesDrawn.Find(tile => tile.Color == TileColor.ONE) != null
                });
                if (this.table.IsEmpty())
                {
                    this.onTableEmpty.Invoke();
                }
            }

        }
    }
}
