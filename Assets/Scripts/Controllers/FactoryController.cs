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
                    factory.Fill(bagController.Draw(4));
                }
            }

            GameObject CreateFactory(string name)
            {
                GameObject gameObject = Instantiate(this.factoryPrefab);
                gameObject.name = name;
                this.factories.Add(gameObject.GetComponent<Factory>());
                return gameObject;
            }

            public List<Factory> GetFactories()
            {
                return this.factories;
            }

            private void OnRoundStart(OnRoundPhasePreparePayload payload)
            {
                this.FillFactories(System.Instance.GetBagController());
            }
        }
    }
}
