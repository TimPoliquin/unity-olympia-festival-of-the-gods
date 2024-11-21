using System.Collections;
using System.Collections.Generic;
using Azul.AIEvents;
using Azul.Model;
using UnityEngine;

namespace Azul
{
    namespace Controller
    {
        public class UIController : MonoBehaviour
        {
            private BlackScreenUIController blackScreenUIController;
            private GameEndUIController gameEndUIController;
            private GameStartUIController gameStartUIController;
            private HadesTokenPanelUIController hadesTokenPanelUIController;
            private HelpPanelUIController helpPanelUIController;
            private IconUIFactory iconUIFactory;
            private MilestoneCompletedPanelUIController milestoneCompletedPanelUIController;
            private OptionsPanelUIController optionsPanelUIController;
            private OverflowTileSelectionUIController overflowTileSelectionUIController;
            private PanelManagerController panelManagerController;
            private PlayerUIController playerUIController;
            private ScoreTileSelectionUIController scoreTileSelectionUIController;
            private ScoreTilesPreviewPanelUIController scoreTilesPreviewPanelUIController;
            private SelectRewardUIController selectRewardUIController;
            private StarUIController starUIController;

            public void InitializeListeners()
            {
                this.GetGameStartUIController().InitializeListeners();
                this.GetOverflowTileSelectionUIController().InitializeListeners();
                this.GetScoreTileSelectionUIController().InitializeListeners();
                this.GetSelectRewardUIController().InitializeListeners();
                this.GetStarUIController().InitializeListeners();
            }

            public BlackScreenUIController GetBlackScreenUIController()
            {
                if (null == this.blackScreenUIController)
                {
                    this.blackScreenUIController = this.GetComponentInChildren<BlackScreenUIController>();
                }
                return this.blackScreenUIController;
            }

            public GameEndUIController GetGameEndUIController()
            {
                if (null == this.gameEndUIController)
                {
                    this.gameEndUIController = this.GetComponentInChildren<GameEndUIController>();
                }
                return this.gameEndUIController;
            }

            public GameStartUIController GetGameStartUIController()
            {
                if (null == this.gameStartUIController)
                {
                    this.gameStartUIController = this.GetComponentInChildren<GameStartUIController>();
                }
                return this.gameStartUIController;
            }

            public HelpPanelUIController GetHelpPanelUIController()
            {
                if (null == this.helpPanelUIController)
                {
                    this.helpPanelUIController = this.GetComponentInChildren<HelpPanelUIController>();
                }
                return this.helpPanelUIController;
            }

            public HadesTokenPanelUIController GetHadesTokenPanelUIController()
            {
                if (null == this.hadesTokenPanelUIController)
                {
                    this.hadesTokenPanelUIController = this.GetComponentInChildren<HadesTokenPanelUIController>();
                }
                return this.hadesTokenPanelUIController;
            }

            public IconUIFactory GetIconUIFactory()
            {
                if (null == this.iconUIFactory)
                {
                    this.iconUIFactory = this.GetComponentInChildren<IconUIFactory>();
                }
                return this.iconUIFactory;
            }

            public MilestoneCompletedPanelUIController GetMilestoneCompletedPanelUIController()
            {
                if (null == this.milestoneCompletedPanelUIController)
                {
                    this.milestoneCompletedPanelUIController = this.GetComponentInChildren<MilestoneCompletedPanelUIController>();
                }
                return this.milestoneCompletedPanelUIController;
            }

            public OptionsPanelUIController GetOptionsPanelUIController()
            {
                if (null == this.optionsPanelUIController)
                {
                    this.optionsPanelUIController = this.GetComponentInChildren<OptionsPanelUIController>();
                }
                return this.optionsPanelUIController;
            }

            public PlayerBoardPreviewUIController GetPlayerBoardPreviewUIController()
            {
                return this.GetComponentInChildren<PlayerBoardPreviewUIController>();
            }

            public OverflowTileSelectionUIController GetOverflowTileSelectionUIController()
            {
                if (null == this.overflowTileSelectionUIController)
                {
                    this.overflowTileSelectionUIController = this.GetComponentInChildren<OverflowTileSelectionUIController>();
                }
                return this.overflowTileSelectionUIController;
            }

            public PanelManagerController GetPanelManagerController()
            {
                if (null == this.panelManagerController)
                {
                    this.panelManagerController = this.GetComponentInChildren<PanelManagerController>();
                }
                return this.panelManagerController;
            }

            public PlayerUIController GetPlayerUIController()
            {
                if (null == this.playerUIController)
                {
                    this.playerUIController = this.GetComponentInChildren<PlayerUIController>();
                }
                return this.playerUIController;
            }

            public ScoreTileSelectionUIController GetScoreTileSelectionUIController()
            {
                if (null == this.scoreTileSelectionUIController)
                {
                    this.scoreTileSelectionUIController = this.GetComponentInChildren<ScoreTileSelectionUIController>();
                }
                return this.scoreTileSelectionUIController;
            }

            public ScoreTilesPreviewPanelUIController GetScoreTilesPreviewPanelUIController()
            {
                if (null == this.scoreTilesPreviewPanelUIController)
                {
                    this.scoreTilesPreviewPanelUIController = this.GetComponentInChildren<ScoreTilesPreviewPanelUIController>();
                }
                return this.scoreTilesPreviewPanelUIController;
            }

            public SelectRewardUIController GetSelectRewardUIController()
            {
                if (null == this.selectRewardUIController)
                {
                    this.selectRewardUIController = this.GetComponentInChildren<SelectRewardUIController>();
                }
                return this.selectRewardUIController;
            }

            public StarUIController GetStarUIController()
            {
                if (null == this.starUIController)
                {
                    this.starUIController = this.GetComponentInChildren<StarUIController>();
                }
                return this.starUIController;
            }
        }
    }
}
