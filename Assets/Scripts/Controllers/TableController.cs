using System.Collections;
using System.Collections.Generic;
using Azul.Model;
using UnityEngine;

namespace Azul
{
    namespace Controller
    {
        public class TableController : MonoBehaviour
        {
            [SerializeField] private GameObject tablePrefab;

            private Table table;

            public void SetupGame()
            {
                this.table = Instantiate(this.tablePrefab, Vector3.zero, Quaternion.identity).GetComponent<Table>();
            }

            public void InitializeListeners()
            {
                FactoryController factoryController = System.Instance.GetFactoryController();
                factoryController.AddOnFactoryTilesDiscardedListener(this.OnTilesDiscarded);
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
            }

            private void OnTilesDiscarded(OnFactoryTilesDiscarded payload)
            {
                foreach (Tile tile in payload.TilesDiscarded)
                {
                    this.table.AddToCenter(tile);
                }
            }
        }
    }
}
