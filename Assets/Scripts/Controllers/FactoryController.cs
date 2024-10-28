using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azul.Debug;
using Azul.Event;
using Azul.Layout;
using Azul.Model;
using Azul.TileHolderEvents;
using Azul.Util;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Azul
{
    namespace Controller
    {
        public struct OnFactoryTilesDrawnPayload
        {
            public List<Tile> TilesDrawn { get; init; }
            public Action Done { get; init; }
        }
        public struct OnFactoryTilesDiscardedPayload
        {
            public List<Tile> TilesDiscarded { get; init; }
        }
        public class FactoryController : MonoBehaviour
        {
            [SerializeField] private GameObject factoryPrefab;
            private List<Factory> factories;

            private AzulEvent<OnFactoryTilesDrawnPayload> onTilesDrawn = new();
            private AzulEvent<OnFactoryTilesDiscardedPayload> onTilesDiscarded = new();
            private UnityEvent onAllFactoriesEmpty = new();
            private UnityEvent onFactoryDrawComplete = new();

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

            public void AddOnFactoryTilesDrawnListener(UnityAction<EventTracker<OnFactoryTilesDrawnPayload>> listener)
            {
                this.onTilesDrawn.AddListener(listener);
            }

            public void AddOnFactoryTilesDiscardedListener(UnityAction<EventTracker<OnFactoryTilesDiscardedPayload>> listener)
            {
                this.onTilesDiscarded.AddListener(listener);
            }

            public void AddOnFactoryDrawCompleteListener(UnityAction listener)
            {
                this.onFactoryDrawComplete.AddListener(listener);
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
                yield return this.onTilesDrawn.Invoke(new OnFactoryTilesDrawnPayload { TilesDrawn = tilesDrawn }).WaitUntilCompleted();
                yield return this.onTilesDiscarded.Invoke(new OnFactoryTilesDiscardedPayload { TilesDiscarded = tilesDiscarded }).WaitUntilCompleted();
                // check for empty factories
                if (this.factories.All(factory => factory.IsEmpty()))
                {
                    this.onAllFactoriesEmpty.Invoke();
                }
                this.onFactoryDrawComplete.Invoke();
            }

            public void AddOnAllFactoriesEmptyListener(UnityAction listener)
            {
                this.onAllFactoriesEmpty.AddListener(listener);
            }

            public void AddOnTokenHoverExitListener(UnityAction<OnTileHoverExitPayload> listener)
            {
                foreach (Factory factory in this.factories)
                {
                    factory.GetSelectableTileHolderController().AddOnTileHoverExitListener(listener);
                }
            }

            public void AddOnUnavailableTokenHoverEnterListener(UnityAction<OnUnavailableTokenHoverEnter> listener)
            {
                foreach (Factory factory in this.factories)
                {
                    factory.GetSelectableTileHolderController().AddOnUnavailableTokenHoverEnterListener(listener);
                }
            }
        }
    }
}
