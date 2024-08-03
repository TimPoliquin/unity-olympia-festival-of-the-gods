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
            [SerializeField] private float radius = 15.0f;
            private GameObject root;
            private List<Factory> factories;

            public void SetupGame(int numPlayers)
            {
                this.factories = new();
                this.root = new GameObject("Factories");
                int numFactories = (numPlayers * 2) + 1;
                CircularLayout layout = this.root.AddComponent<CircularLayout>();
                layout.CreateLayout(numFactories, this.radius, (input) =>
                {
                    return this.CreateFactory($"Factory {input.Index}", input.Position);
                });
            }

            public void FillFactories(BagController bagController)
            {
                foreach (Factory factory in this.factories)
                {
                    factory.Fill(bagController.Draw(4));
                }
            }

            GameObject CreateFactory(string name, Vector3 position)
            {
                GameObject gameObject = Instantiate(this.factoryPrefab, this.root.transform);
                gameObject.name = name;
                gameObject.transform.position = position;
                this.factories.Add(gameObject.GetComponent<Factory>());
                return gameObject;
            }
        }
    }
}
