using System.Collections;
using System.Collections.Generic;
using Azul.Debug;
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
            void OnDrawGizmosSelected()
            {
                if (this.root)
                {
                    DebugUtils.DrawCircle(this.root.transform.position, this.radius, 50, Color.white);
                }
            }

            public void SetupGame(int numPlayers)
            {
                this.root = new GameObject("Factories");
                this.factories = new();
                int numFactories = (numPlayers * 2) + 1;
                float arcSize = 360.0f / numFactories;
                for (int idx = 0; idx < numFactories / 2; idx++)
                {
                    float x = this.radius * Mathf.Cos(idx * arcSize);
                    float z = this.radius * Mathf.Sin(idx * arcSize);
                    this.CreateFactory($"Factory {idx + 1}", x, z);
                    this.CreateFactory($"Factory {idx + 1 + numFactories / 2}", -x, -z);
                }
            }

            public void FillFactories(BagController bagController)
            {
                foreach (Factory factory in this.factories)
                {
                    factory.Fill(bagController.Draw(4));
                }
            }



            void CreateFactory(string name, float x, float z)
            {
                Vector3 position = new Vector3(x, 0, z);
                GameObject gameObject = Instantiate(this.factoryPrefab, this.root.transform);
                gameObject.name = name;
                gameObject.transform.position = position;
                this.factories.Add(gameObject.GetComponent<Factory>());
            }
        }
    }
}
