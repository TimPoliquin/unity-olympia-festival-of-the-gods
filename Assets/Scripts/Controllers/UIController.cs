using System.Collections;
using System.Collections.Generic;
using Azul.Model;
using UnityEngine;

namespace Azul
{
    namespace Controller
    {
        public class UIController : MonoBehaviour
        {
            private OverflowTileSelectionUIController overflowTileSelectionUIController;
            private ScoreTileSelectionUIController scoreTileSelectionUIController;
            private SelectRewardUIController selectRewardUIController;
            private StarUIController starUIController;

            public void InitializeListeners()
            {
                this.GetOverflowTileSelectionUIController().InitializeListeners();
                this.GetScoreTileSelectionUIController().InitializeListeners();
                this.GetSelectRewardUIController().InitializeListeners();
                this.GetStarUIController().InitializeListeners();
            }

            public OverflowTileSelectionUIController GetOverflowTileSelectionUIController()
            {
                if (null == this.overflowTileSelectionUIController)
                {
                    this.overflowTileSelectionUIController = this.GetComponentInChildren<OverflowTileSelectionUIController>();
                }
                return this.overflowTileSelectionUIController;
            }


            public ScoreTileSelectionUIController GetScoreTileSelectionUIController()
            {
                if (null == this.scoreTileSelectionUIController)
                {
                    this.scoreTileSelectionUIController = this.GetComponentInChildren<ScoreTileSelectionUIController>();
                }
                return this.scoreTileSelectionUIController;
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
