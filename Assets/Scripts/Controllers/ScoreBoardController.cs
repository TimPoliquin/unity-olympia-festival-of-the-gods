using System;
using System.Collections;
using System.Collections.Generic;
using Azul.Layout;
using Azul.Model;
using Unity.VisualScripting;
using UnityEngine;

namespace Azul
{
    namespace Controller
    {
        public class ScoreBoardController : MonoBehaviour
        {
            [SerializeField] private Vector3 position = new Vector3(100, 1, -55);
            [SerializeField][Range(1, 10)] private int supplySize = 10;
            [SerializeField] private GameObject scoreBoardPrefab;
            [SerializeField] private GameObject tilePrefab;
            [SerializeField] private GameObject roundCounterPrefab;

            private ScoreBoard scoreBoard;

            private List<TilePlaceholder> placeholderTiles;
            public void SetupGame()
            {
                this.CreateScoreBoard();
                this.CreateSupplyStar();
                this.PlaceRoundCounter();
            }

            public void InitializeListeners()
            {
                RoundController roundController = System.Instance.GetRoundController();
                roundController.AddOnRoundPhaseAcquireListener(this.OnRoundStart);
            }

            private void CreateScoreBoard()
            {
                this.scoreBoard = Instantiate(this.scoreBoardPrefab, this.position, Quaternion.identity).GetComponent<ScoreBoard>();
                this.scoreBoard.transform.position = this.position;
            }

            private void CreateSupplyStar()
            {
                this.placeholderTiles = new();
                for (int idx = 0; idx < this.supplySize; idx++)
                {
                    TilePlaceholder tile = TilePlaceholder.Create(this.tilePrefab, TileColor.WILD);
                    this.placeholderTiles.Add(tile);
                }
                this.scoreBoard.AddSupplyPlaceholders(this.placeholderTiles);
            }

            public void FillSupply(BagController bagController)
            {
                this.placeholderTiles.ForEach(placeholder =>
                {
                    if (placeholder.IsEmpty())
                    {
                        placeholder.PlaceTile(bagController.Draw());
                    }
                });
            }

            private void PlaceRoundCounter()
            {
                GameObject roundCounter = Instantiate(this.roundCounterPrefab);
                this.scoreBoard.PlaceCounter(roundCounter);
            }

            public ScoreBoard GetScoreBoard()
            {
                return this.scoreBoard;
            }

            public void DeductPoints(int player, int points)
            {
                // TODO - score tracking!
                UnityEngine.Debug.Log($"Deducting points: {player} loses {points} points");
            }

            private void OnRoundStart(OnRoundPhaseAcquirePayload payload)
            {
                this.scoreBoard.StartRound(payload.RoundNumber);
            }
        }
    }
}
