using System;
using System.Collections;
using System.Collections.Generic;
using Azul.GameEvents;
using Azul.Model;
using UnityEngine;

namespace Azul
{
    namespace Controller
    {
        public class FirstTimePlayerPanelUIController : MonoBehaviour
        {
            void Start()
            {
                System.Instance.GetGameController().AddOnGameSetupCompleteListener(this.OnSetupComplete);
            }

            private void OnSetupComplete(OnGameSetupCompletePayload payload)
            {
                if (System.Instance.GetPlayerDataController().IsNewPlayer())
                {
                    FirstTimePlayerPanelUI panel = System.Instance.GetPrefabFactory().CreateFirstTimePlayerPanelUI(HelpPanelUIController.HELP_LAYER);
                    panel.AddOnNoListener(() =>
                    {
                        System.Instance.GetUIController().GetPanelManagerController().ShowDefaultLayer();
                        this.OnFinish(panel);
                    });
                    panel.AddOnYesListener(() =>
                    {
                        System.Instance.GetUIController().GetHelpPanelUIController().ShowHelp();
                        this.OnFinish(panel);
                    });
                    System.Instance.GetUIController().GetPanelManagerController().ShowLayer(HelpPanelUIController.HELP_LAYER);
                }
            }

            private void OnFinish(FirstTimePlayerPanelUI panel)
            {
                System.Instance.GetPlayerDataController().SavePlayerData();
                Destroy(panel.gameObject);
                Destroy(this.gameObject);
            }
        }
    }
}
