using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azul.Model;
using Azul.PlayerBoardRewardEvents;
using Azul.RewardUIEvents;
using Azul.WildColorSelectionEvents;
using UnityEngine;
using UnityEngine.Events;

namespace Azul
{
    namespace RewardUIEvents
    {
        public class OnGrantRewardPayload
        {
            public int PlayerNumber { get; init; }
            public Tile Tile { get; init; }
        }
    }
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
            private UnityEvent<OnGrantRewardPayload> onGrantReward = new();

            public void InitializeListeners()
            {
                PlayerBoardController playerBoardController = System.Instance.GetPlayerBoardController();
                playerBoardController.AddOnPlayerBoardEarnRewardListener(this.OnEarnReward);
            }

            public void Activate(int playerNumber, int tileCount)
            {
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
                Tile grantedTile = bagController.Draw(color);
                PlayerBoardController playerBoardController = System.Instance.GetPlayerBoardController();
                playerBoardController.AddDrawnTiles(this.playerNumber, new() { grantedTile });
                this.onGrantReward.Invoke(new OnGrantRewardPayload
                {
                    PlayerNumber = this.playerNumber,
                    Tile = grantedTile
                });
            }

            private string GetInstructions()
            {
                return $"Reward: Choose a tile! {this.currentCount + 1} / {this.totalCount}";
            }

            public void AddOnGrantRewardListener(UnityAction<OnGrantRewardPayload> listener)
            {
                this.onGrantReward.AddListener(listener);
            }
        }

    }
}
