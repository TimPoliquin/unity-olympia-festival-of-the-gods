using System.Collections;
using System.Collections.Generic;
using Azul.Model;
using UnityEngine;

namespace Azul
{
    namespace Controller
    {
        public class MilestoneCompletedPanelUIController : MonoBehaviour
        {
            private MilestoneCompletedPanelUI currentPanel;

            public void Show(TileColor tileColor)
            {
                if (null != this.currentPanel)
                {
                    this.Hide();
                }
                TileColorMappingController tileColorMappingController = System.Instance.GetTileColorMappingController();
                this.currentPanel = System.Instance.GetPrefabFactory().CreateMilestoneCompletedPanelUI();
                this.currentPanel.Show(tileColorMappingController.GetGodName(tileColor), tileColor, tileColorMappingController.GetGodPoints(tileColor));
            }

            public void Hide()
            {
                if (null != this.currentPanel)
                {
                    this.currentPanel.Hide();
                    Destroy(this.currentPanel.gameObject);
                    this.currentPanel = null;
                }
            }
        }
    }
}
