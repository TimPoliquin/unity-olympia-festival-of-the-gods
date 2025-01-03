using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azul.GameEvents;
using Azul.OnGameEndEvents;
using Azul.ScoreBoardEvents;
using UnityEngine;
using UnityEngine.Events;

namespace Azul
{
    namespace GameEvents
    {
        public struct OnGameSetupCompletePayload
        {
            public int NumberOfPlayers { get; init; }
        }
    }
    namespace Controller
    {
        public class GameController : MonoBehaviour
        {
            [SerializeField] private UnityEvent<OnGameSetupCompletePayload> onGameSetupComplete = new();
            [SerializeField] private AudioClip startBGM;

            private bool isGameSetupComplete;

            void Start()
            {
                System.Instance.GetAudioController().PlayBGM(this.startBGM, .5f);
            }

            public void StartGame()
            {
                System.Instance.GetAudioController().StopBGM();
                AIController aiController = System.Instance.GetAIController();
                BagController bagController = System.Instance.GetBagController();
                CameraController cameraController = System.Instance.GetCameraController();
                FactoryController factoryController = System.Instance.GetFactoryController();
                PlayerController playerController = System.Instance.GetPlayerController();
                PlayerBoardController playerBoardController = System.Instance.GetPlayerBoardController();
                RoundController roundController = System.Instance.GetRoundController();
                ScoreBoardController scoreBoardController = System.Instance.GetScoreBoardController();
                AltarFactory starController = System.Instance.GetStarController();
                TableController tableController = System.Instance.GetTableController();
                TileController tileController = System.Instance.GetTileController();
                UIController uIController = System.Instance.GetUIController();
                // TODO - this should be triggered by the UI/Player Ready
                aiController.SetupGame(playerController.GetPlayers());
                tableController.SetupGame();
                playerBoardController.SetupGame(playerController.GetNumberOfPlayers(), starController);
                scoreBoardController.SetupGame(playerController.GetNumberOfPlayers());
                factoryController.SetupGame(playerController.GetNumberOfPlayers());
                tileController.SetupGame();
                bagController.SetupGame(tileController.GetTiles());
                // DEVNOTE - we will not fill the supply for now, in favor of allowing the player to select a tile of their choosing from the bag.
                // scoreBoardController.FillSupply(bagController);
                roundController.SetupGame();
                // dispatch game setup complete
                this.SetSetupComplete();
                // initialize event listeners
                aiController.InitializeListeners();
                cameraController.InitializeListeners();
                factoryController.InitializeListeners();
                playerController.InitializeListeners();
                playerBoardController.InitializeListeners();
                roundController.InitializeListeners();
                scoreBoardController.InitializeListeners();
                tableController.InitializeListeners();
                uIController.InitializeListeners();
                // populate the table
                tableController.AddPlayerBoards(playerBoardController.GetPlayerBoards());
                tableController.AddFactories(factoryController.GetFactories());
                // Start the first round!   
                roundController.StartRound();
            }

            public void AddOnGameSetupCompleteListener(UnityAction<OnGameSetupCompletePayload> listener)
            {
                if (this.isGameSetupComplete)
                {
                    listener.Invoke(this.CreatGameSetupCompletePayload());
                }
                else
                {
                    this.onGameSetupComplete.AddListener(listener);
                }
            }

            private void SetSetupComplete()
            {
                this.isGameSetupComplete = true;
                this.onGameSetupComplete.Invoke(this.CreatGameSetupCompletePayload());
                this.onGameSetupComplete.RemoveAllListeners();
            }

            private OnGameSetupCompletePayload CreatGameSetupCompletePayload()
            {
                return new OnGameSetupCompletePayload
                {
                    NumberOfPlayers = System.Instance.GetPlayerController().GetNumberOfPlayers()
                };
            }
        }
    }
}
