using System.Collections;
using System.Collections.Generic;
using Azul.GameStartEvents;
using Azul.Model;
using UnityEngine;

namespace Azul
{
    namespace Controller
    {
        public class GameStartUIController : MonoBehaviour
        {
            [SerializeField] private GameStartUI gameStartUI;

            void Awake()
            {
                this.gameStartUI.AddOnPlayerCountSelectionListener(this.OnPlayerCountSelection);
                this.gameStartUI.AddOnGameStartListener(this.OnGameStart);
                this.gameStartUI.gameObject.SetActive(true);
            }

            public void InitializeListeners()
            {
                // nothing to do here... yet
            }

            public void OnPlayerCountSelection(OnPlayerCountSelectionPayload payload)
            {
                this.gameStartUI.ShowPlayerConfigs(payload.PlayerCount);
            }

            public void OnGameStart(OnGameStartPayload payload)
            {
                System.Instance.GetPlayerController().SetPlayers(payload.PlayerConfigs);
                // TODO - add a real mechanism to decide who goes first.
                System.Instance.GetPlayerController().SetStartingPlayer(Random.Range(0, payload.PlayerConfigs.Count));
                System.Instance.GetGameController().StartGame();
                this.gameStartUI.gameObject.SetActive(false);
            }
        }
    }
}
