using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azul
{
    namespace Model
    {
        public class ScoreBoard : MonoBehaviour
        {
            [SerializeField] private List<GameObject> roundMarkers;
            private int currentRound = 0;
            private GameObject roundCounter;

            void Awake()
            {
                this.InitializeRoundMarkerPositions();
            }

            public void PlaceCounter(GameObject roundCounter)
            {
                this.roundCounter = roundCounter;
                this.roundCounter.transform.SetParent(this.roundMarkers[this.currentRound].transform);
                this.roundCounter.transform.localPosition = Vector3.zero;
            }

            public void AdvanceRoundMarker()
            {
                Vector3 currentRoundPosition = this.roundMarkers[this.currentRound].transform.position;
                Vector3 nextRoundPosition = this.roundMarkers[this.currentRound + 1].transform.position;
                this.currentRound++;
                roundCounter.transform.SetParent(this.roundMarkers[this.currentRound].transform);
                this.StartCoroutine(this.MoveRoundMarker(currentRoundPosition, nextRoundPosition));
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
        }
    }
}
