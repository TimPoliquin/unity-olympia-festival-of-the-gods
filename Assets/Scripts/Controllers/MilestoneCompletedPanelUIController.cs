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

            public CoroutineStatus Show(TileColor tileColor)
            {
                if (null != this.currentPanel)
                {
                    this.Hide();
                }
                TileColorMappingController tileColorMappingController = System.Instance.GetTileColorMappingController();
                this.currentPanel = System.Instance.GetPrefabFactory().CreateMilestoneCompletedPanelUI();
                return this.currentPanel.Show(tileColorMappingController.GetGodName(tileColor), tileColor, tileColorMappingController.GetGodPoints(tileColor));
            }

            public CoroutineStatus Show(int ritualNumber)
            {
                if (null != this.currentPanel)
                {
                    this.Hide();
                }
                this.currentPanel = System.Instance.GetPrefabFactory().CreateMilestoneCompletedPanelUI();
                return this.currentPanel.Show(ritualNumber, System.Instance.GetScoreBoardController().GetCompletionPoints(ritualNumber));

            }

            public CoroutineStatus AnimateScore(int playerNumber, float time)
            {
                return this.currentPanel.AnimateScoreToPoint(
                    System.Instance.GetUIController().GetPlayerUIController().GetScoreScreenPosition(playerNumber),
                    .25f,
                    time
                );
            }

            public CoroutineStatus Hide()
            {
                CoroutineStatus status = CoroutineStatus.Single();
                if (null != this.currentPanel && this.currentPanel.gameObject.activeInHierarchy)
                {
                    this.StartCoroutine(this.HideCoroutine(this.currentPanel, status));
                    this.currentPanel = null;
                }
                else
                {
                    status.Finish();
                }
                return status;
            }

            private IEnumerator HideCoroutine(MilestoneCompletedPanelUI panel, CoroutineStatus status)
            {
                status.Start();
                yield return panel.Hide().WaitUntilCompleted();
                Destroy(panel.gameObject);
                status.Finish();
            }

        }
    }
}
