using System;
using System.Collections;
using System.Collections.Generic;
using Azul.Debug;
using Azul.Layout;
using Azul.Model;
using Unity.VisualScripting;
using UnityEngine;

namespace Azul
{
    namespace Controller
    {
        public class FactoryController : MonoBehaviour
        {
            [SerializeField] private GameObject factoryPrefab;
            private List<Factory> factories;

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
                    tiles.ForEach(tile =>
                    {
                        // TODO - this doesn't feel like it belongs in the FACTORY controller.
                        TilePointerController tilePointerController = tile.GetComponent<TilePointerController>();
                        tilePointerController.AddOnTileHoverEnterListener(this.OnTileHoverEnter);
                        tilePointerController.AddOnTileHoverExitListener(this.OnTileHoverExit);
                        tilePointerController.AddOnTileSelectListener(this.OnTileSelect);
                    });
                    factory.Fill(tiles);
                }
            }

            public List<Factory> GetFactories()
            {
                return this.factories;
            }

            private GameObject CreateFactory(string name)
            {
                GameObject gameObject = Instantiate(this.factoryPrefab);
                gameObject.name = name;
                this.factories.Add(gameObject.GetComponent<Factory>());
                return gameObject;
            }

            private void OnRoundStart(OnRoundPhasePreparePayload payload)
            {
                this.FillFactories(System.Instance.GetBagController());
            }

            private void OnTileHoverEnter(OnTileHoverEnterPayload payload)
            {
                UnityEngine.Debug.Log("Hovering tile");
            }

            private void OnTileHoverExit(OnTileHoverExitPayload payload)
            {
                UnityEngine.Debug.Log("Exiting tile");

            }

            private void OnTileSelect(OnTileSelectPayload payload)
            {
                UnityEngine.Debug.Log("Selecing tile");
            }
        }
    }
}
