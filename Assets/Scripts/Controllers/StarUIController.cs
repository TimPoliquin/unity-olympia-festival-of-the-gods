using System;
using System.Collections;
using System.Collections.Generic;
using Azul.Model;
using Azul.PlayerBoardEvents;
using UnityEngine;

namespace Azul
{
    namespace Controller
    {
        public class StarUIController : MonoBehaviour
        {
            [SerializeField] private GameObject container;
            [SerializeField] private GameObject spaceValueUIPrefab;

            private List<StarSpaceUI> starSpaceUIs = new();

            public void CreateStarSpaceUI(AltarSpace starSpace)
            {
                StarSpaceUI ui = Instantiate(this.spaceValueUIPrefab, this.container.transform).GetComponent<StarSpaceUI>();
                ui.SetStarSpace(starSpace);
                ui.ScaleToTableView();
                this.starSpaceUIs.Add(ui);
            }

            public void InitializeListeners()
            {
                CameraController cameraController = System.Instance.GetCameraController();
                cameraController.AddOnFocusOnTableListener(this.OnFocusOnTable);
                cameraController.AddOnFocusOnPlayerBoardListener(this.OnFocusOnPlayerBoard);
                RoundController roundController = System.Instance.GetRoundController();
                roundController.AddOnAllRoundsCompleteListener(this.OnAllRoundsComplete);
            }

            private void OnFocusOnTable()
            {
                foreach (StarSpaceUI starSpaceUI in this.starSpaceUIs)
                {
                    starSpaceUI.ScaleToTableView();
                }
            }

            private void OnFocusOnPlayerBoard()
            {
                foreach (StarSpaceUI starSpaceUI in this.starSpaceUIs)
                {
                    starSpaceUI.ScaleToPlayerBoardView();
                }
            }

            private void OnAllRoundsComplete(OnAllRoundsCompletePayload payload)
            {
                this.container.SetActive(false);
            }
        }
    }
}
