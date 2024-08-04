using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azul.Layout;
using UnityEngine;

namespace Azul
{
    namespace Model
    {
        public class ScoreBoard : MonoBehaviour
        {
            [SerializeField] private List<GameObject> roundMarkers;
            [SerializeField] private GameObject supplyRoot;

            private int currentRound = 0;
            private GameObject roundCounter;
            private CircularLayout supplyLayout;
            private List<TilePlaceholder> supplyPlaceholders;

            void Awake()
            {
                this.InitializeRoundMarkerPositions();
                this.supplyLayout = this.supplyRoot.GetComponent<CircularLayout>();
            }

            public void PlaceCounter(GameObject roundCounter)
            {
                this.roundCounter = roundCounter;
                this.roundCounter.transform.SetParent(this.GetRoundMarker(this.currentRound).transform);
                this.roundCounter.transform.localPosition = Vector3.zero;
            }

            public void StartRound(int round)
            {
                if (this.currentRound == round)
                {
                    // Do nothing
                }
                else if (round == this.currentRound + 1)
                {
                    this.AdvanceRoundMarker();
                }
                else
                {
                    throw new ArgumentOutOfRangeException($"Requested unexpected round start: {round}. Expected {currentRound + 1}", nameof(round));
                }
            }

            public void AdvanceRoundMarker()
            {
                Vector3 currentRoundPosition = this.GetRoundMarker(this.currentRound).transform.position;
                Vector3 nextRoundPosition = this.GetRoundMarker(this.currentRound + 1).transform.position;
                this.currentRound++;
                roundCounter.transform.SetParent(this.GetRoundMarker(this.currentRound).transform);
                this.StartCoroutine(this.MoveRoundMarker(currentRoundPosition, nextRoundPosition));
            }

            public void AddSupplyPlaceholders(List<TilePlaceholder> placeholders)
            {
                this.supplyPlaceholders = placeholders;
                this.supplyLayout.AddChildren(placeholders.Select(placeholder => placeholder.gameObject).ToList());
            }

            private IEnumerator MoveRoundMarker(Vector3 currentPosition, Vector3 nextPosition)
            {
                while (Vector3.Distance(this.roundCounter.transform.position, nextPosition) > .05f)
                {
                    yield return null;
                    this.roundCounter.transform.Translate((nextPosition - currentPosition).normalized * Time.deltaTime * 10.0f);
                }
            }

            private void InitializeRoundMarkerPositions()
            {
                float z = roundMarkers[0].transform.position.z;
                foreach (GameObject roundMarker in this.roundMarkers)
                {
                    Vector3 position = roundMarker.transform.position;
                    roundMarker.transform.position = new Vector3(position.x, position.y, z);
                }
            }

            private GameObject GetRoundMarker(int round)
            {
                return this.roundMarkers[round];
            }
        }
    }
}
