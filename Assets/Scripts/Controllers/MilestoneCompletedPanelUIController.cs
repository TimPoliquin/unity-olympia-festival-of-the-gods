using System.Collections;
using System.Collections.Generic;
using Azul.Model;
using Azul.Util;
using UnityEngine;

namespace Azul
{
    namespace Controller
    {
        public class MilestoneCompletedPanelUIController : MonoBehaviour
        {
            private MilestoneCompletedPanelUI currentPanel;

            public CoroutineResult Show(TileColor tileColor)
            {
                if (null != this.currentPanel)
                {
                    this.Hide();
                }
                TileColorMappingController tileColorMappingController = System.Instance.GetTileColorMappingController();
                this.currentPanel = System.Instance.GetPrefabFactory().CreateMilestoneCompletedPanelUI();
                return this.currentPanel.Show(tileColorMappingController.GetGodName(tileColor), tileColor, tileColorMappingController.GetGodPoints(tileColor));
            }

            public CoroutineResult Hide()
            {
                CoroutineResult result = CoroutineResult.Single();
                if (null != this.currentPanel && this.currentPanel.gameObject.activeInHierarchy)
                {
                    this.StartCoroutine(this.HideCoroutine(this.currentPanel, result));
                    this.currentPanel = null;
                }
                else
                {
                    result.Finish();
                }
                return result;
            }

            private IEnumerator HideCoroutine(MilestoneCompletedPanelUI panel, CoroutineResult result)
            {
                result.Start();
                yield return panel.Hide().WaitUntilCompleted();
                Destroy(panel.gameObject);
                result.Finish();
            }

        }
    }
}
