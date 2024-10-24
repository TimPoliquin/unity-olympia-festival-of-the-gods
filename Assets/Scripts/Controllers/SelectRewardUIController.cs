using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azul.Event;
using Azul.ClaimRewardEvents;
using Azul.Model;
using Azul.PlayerBoardRewardEvents;
using Azul.RewardUIEvents;
using UnityEngine;
using UnityEngine.Events;

namespace Azul
{
    namespace RewardUIEvents
    {
        public class OnClaimRewardPayload
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
            private UnityEvent<OnClaimRewardPayload> onClaimReward = new();

            public void InitializeListeners()
            {
                PlayerBoardController playerBoardController = System.Instance.GetPlayerBoardController();
                playerBoardController.AddOnPlayerBoardEarnRewardListener(this.OnEarnReward);
            }

            public bool IsRewardPanelOpen()
            {
                return this.selectionUI != null;
            }

            public void Show(int playerNumber, int tileCount, Action callback)
            {
                System.Instance.GetPlayerBoardController().HideScoringUI(playerNumber);
                this.selectionUI = System.Instance.GetPrefabFactory().CreateGrantRewardTilesUI();
                this.selectionUI.SetScoreTileSelectionUIs(this.CreateTileSelectionUIs(tileCount));
                this.selectionUI.SetPlayerNumber(playerNumber);
                this.selectionUI.SetMaxSelectionCount(tileCount);
                this.selectionUI.AddOnConfirmListener((payload) => this.OnConfirm(payload, callback));
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
                if (null != this.selectionUI)
                {
                    Destroy(this.selectionUI.gameObject);
                    this.selectionUI = null;
                }
            }


            private void OnEarnReward(EventTracker<OnPlayerBoardEarnRewardPayload> payload)
            {
                if (System.Instance.GetPlayerController().GetPlayer(payload.Payload.PlayerNumber).IsHuman())
                {
                    this.Show(playerNumber: payload.Payload.PlayerNumber, tileCount: payload.Payload.NumberOfTiles, payload.Done);
                }
                else
                {
                    payload.Done();
                }
            }

            private void OnConfirm(OnClaimTileSelectionConfirmPayload payload, Action callback)
            {
                foreach (TileColor rewardColor in payload.Selections)
                {
                    this.ClaimReward(payload.PlayerNumber, rewardColor);
                }
                this.Hide();
                callback?.Invoke();
            }

            private void ClaimReward(int playerNumber, TileColor color)
            {
                Tile claimedTile = System.Instance.GetPlayerBoardController().ClaimReward(playerNumber, color);
                this.onClaimReward.Invoke(new OnClaimRewardPayload
                {
                    PlayerNumber = playerNumber,
                    Tile = claimedTile
                });
            }

            public void AddOnClaimRewardListener(UnityAction<OnClaimRewardPayload> listener)
            {
                this.onClaimReward.AddListener(listener);
            }
        }

    }
}
