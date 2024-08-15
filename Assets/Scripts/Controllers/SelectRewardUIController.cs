using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azul.Model;
using Azul.PlayerBoardRewardEvents;
using Azul.WildColorSelectionEvents;
using UnityEngine;

namespace Azul
{
    namespace Controller
    {
        public class SelectRewardUIController : MonoBehaviour
        {
            [SerializeField] private GameObject rewardUI;
            [SerializeField] private GameObject prefab;

            private WildColorSelectionUI selectionUI;
            private int playerNumber;
            private int currentCount;
            private int totalCount;

            public void InitializeListeners()
            {
                PlayerBoardController playerBoardController = System.Instance.GetPlayerBoardController();
                playerBoardController.AddOnPlayerBoardEarnRewardListener(this.OnEarnReward);
            }

            public void Activate(int playerNumber, int tileCount)
            {
                UnityEngine.Debug.Log($"Activating select reward {tileCount}");
                this.playerNumber = playerNumber;
                this.currentCount = 0;
                this.totalCount = tileCount;
                this.selectionUI = Instantiate(this.prefab, this.rewardUI.transform).GetComponent<WildColorSelectionUI>();
                this.ActivateUI();
            }

            private void ActivateUI()
            {
                this.selectionUI.SetInstructions(this.GetInstructions());
                this.selectionUI.AddOnColorSelectionListener(this.OnRewardSelection);
                this.selectionUI.Activate(this.rewardUI, TileColorUtils.GetTileColors().ToList(), false);
            }

            private void OnEarnReward(OnPlayerBoardEarnRewardPayload payload)
            {
                this.Activate(playerNumber: payload.PlayerNumber, tileCount: payload.NumberOfTiles);
            }

            private void OnRewardSelection(OnWildColorSelectedPayload payload)
            {
                this.currentCount++;
                this.GrantReward(payload.Color);
                if (this.currentCount < this.totalCount)
                {
                    this.selectionUI.SetInstructions(this.GetInstructions());
                }
                else
                {
                    this.currentCount = 0;
                    this.totalCount = 0;
                    Destroy(this.selectionUI.gameObject);
                    this.selectionUI = null;
                }
            }

            private void GrantReward(TileColor color)
            {
                BagController bagController = System.Instance.GetBagController();
                PlayerBoardController playerBoardController = System.Instance.GetPlayerBoardController();
                playerBoardController.AddDrawnTiles(this.playerNumber, new() { bagController.Draw(color) });
            }

            private string GetInstructions()
            {
                return $"Reward: Choose a tile! {this.currentCount + 1} / {this.totalCount}";
            }
        }

    }
}
