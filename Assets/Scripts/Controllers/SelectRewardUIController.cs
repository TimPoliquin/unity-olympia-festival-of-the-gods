using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azul.GrantRewardEvents;
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
            private GrantRewardTilesUI selectionUI;
            private UnityEvent<OnGrantRewardPayload> onGrantReward = new();

            public void InitializeListeners()
            {
                PlayerBoardController playerBoardController = System.Instance.GetPlayerBoardController();
                playerBoardController.AddOnPlayerBoardEarnRewardListener(this.OnEarnReward);
            }

            public void Show(int playerNumber, int tileCount)
            {
                this.selectionUI = System.Instance.GetPrefabFactory().CreateGrantRewardTilesUI();
                this.selectionUI.SetScoreTileSelectionUIs(this.CreateTileSelectionUIs(tileCount));
                this.selectionUI.SetPlayerNumber(playerNumber);
                this.selectionUI.SetMaxSelectionCount(tileCount);
                this.selectionUI.AddOnConfirmListener(this.OnConfirm);
            }

            private List<ScoreTileSelectionUI> CreateTileSelectionUIs(int count)
            {
                PrefabFactory prefabFactory = System.Instance.GetPrefabFactory();
                IconUIFactory iconUIFactory = System.Instance.GetUIController().GetIconUIFactory();
                return TileColorUtils.GetTileColors().Select(tileColor =>
                {
                    ScoreTileSelectionUI scoreTileSelectionUI = prefabFactory.CreateScoreTileSelectionUI(this.selectionUI);
                    scoreTileSelectionUI.SetColor(tileColor, iconUIFactory.GetIcon(tileColor), iconUIFactory.GetBackgroundColor(tileColor));
                    scoreTileSelectionUI.SetCounterRange(0, count, count);
                    scoreTileSelectionUI.SetDefaultValue(0);
                    return scoreTileSelectionUI;
                }).ToList();
            }

            private void Hide()
            {
                Destroy(this.selectionUI.gameObject);
                this.selectionUI = null;
            }


            private void OnEarnReward(OnPlayerBoardEarnRewardPayload payload)
            {
                if (System.Instance.GetPlayerController().GetPlayer(payload.PlayerNumber).IsHuman())
                {
                    this.Show(playerNumber: payload.PlayerNumber, tileCount: payload.NumberOfTiles);
                }
            }

            private void OnConfirm(OnGrantTileSelectionConfirmPayload payload)
            {
                foreach (TileColor rewardColor in payload.Selections)
                {
                    this.GrantReward(payload.PlayerNumber, rewardColor);
                }
                this.Hide();
            }

            private void GrantReward(int playerNumber, TileColor color)
            {
                Tile grantedTile = System.Instance.GetPlayerBoardController().GrantReward(playerNumber, color);
                this.onGrantReward.Invoke(new OnGrantRewardPayload
                {
                    PlayerNumber = playerNumber,
                    Tile = grantedTile
                });
            }

            public void AddOnGrantRewardListener(UnityAction<OnGrantRewardPayload> listener)
            {
                this.onGrantReward.AddListener(listener);
            }
        }

    }
}
