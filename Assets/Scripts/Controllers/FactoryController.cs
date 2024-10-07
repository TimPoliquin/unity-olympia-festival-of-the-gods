using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azul.Debug;
using Azul.Event;
using Azul.Layout;
using Azul.Model;
using Azul.Util;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Azul
{
    namespace Controller
    {
        public class OnFactoryTilesDrawn
        {
            public List<Tile> TilesDrawn { get; init; }
            public Action Done { get; init; }
        }
        public class OnFactoryTilesDiscarded
        {
            public List<Tile> TilesDiscarded { get; init; }
        }
        public class FactoryController : MonoBehaviour
        {
            [SerializeField] private GameObject factoryPrefab;
            private List<Factory> factories;

            private AzulEvent<OnFactoryTilesDrawn> onTilesDrawn = new();
            private AzulEvent<OnFactoryTilesDiscarded> onTilesDiscarded = new();
            private UnityEvent onAllFactoriesEmpty = new();

            public void SetupGame(int numPlayers)
            {
                this.factories = new();
                int numFactories = (numPlayers * 2) + 1;
                for (int idx = 0; idx < numFactories; idx++)
                {
                    this.CreateFactory($"Factory {idx}");
                }
            }

            public void InitializeListeners()
            {
                RoundController roundController = System.Instance.GetRoundController();
                roundController.AddOnRoundPhasePrepareListener(this.OnRoundStart);
            }

            public void FillFactories(BagController bagController)
            {
                foreach (Factory factory in this.factories)
                {
                    List<Tile> tiles = bagController.Draw(4);
                    factory.Fill(tiles);
                }
            }

            public List<Factory> GetFactories()
            {
                return this.factories;
            }

            public void AddOnFactoryTilesDrawnListener(UnityAction<EventTracker<OnFactoryTilesDrawn>> listener)
            {
                this.onTilesDrawn.AddListener(listener);
            }

            public void AddOnFactoryTilesDiscardedListener(UnityAction<EventTracker<OnFactoryTilesDiscarded>> listener)
            {
                this.onTilesDiscarded.AddListener(listener);
            }

            private GameObject CreateFactory(string name)
            {
                Factory factory = Instantiate(this.factoryPrefab).GetComponent<Factory>();
                factory.gameObject.name = name;
                factory.AddOnTilesDrawnListener(this.OnFactoryDrawTiles);
                this.factories.Add(factory);
                return gameObject;
            }

            private void OnRoundStart(OnRoundPhasePreparePayload payload)
            {
                this.FillFactories(System.Instance.GetBagController());
            }

            private void OnFactoryDrawTiles(OnFactoryDrawTilesPayload payload)
            {
                this.StartCoroutine(this.FactoryDrawTilesCoroutine(payload.TilesDrawn, payload.TilesDiscarded));
            }

            private IEnumerator FactoryDrawTilesCoroutine(List<Tile> tilesDrawn, List<Tile> tilesDiscarded)
            {
                yield return this.onTilesDrawn.Invoke(new OnFactoryTilesDrawn { TilesDrawn = tilesDrawn }).WaitUntilCompleted();
                yield return this.onTilesDiscarded.Invoke(new OnFactoryTilesDiscarded { TilesDiscarded = tilesDiscarded }).WaitUntilCompleted();
                // check for empty factories
                if (this.factories.All(factory => factory.IsEmpty()))
                {
                    this.onAllFactoriesEmpty.Invoke();
                }
            }

            public void AddOnAllFactoriesEmptyListener(UnityAction listener)
            {
                this.onAllFactoriesEmpty.AddListener(listener);
            }
        }
    }
}
