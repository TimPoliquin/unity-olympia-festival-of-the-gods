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
            private ScoreTileSelectionUIController scoreTileSelectionUIController;
            private StarUIController starUIController;

            public void InitializeListeners()
            {
                this.GetScoreTileSelectionUIController().InitializeListeners();
                this.GetStarUIController().InitializeListeners();
            }

            public ScoreTileSelectionUIController GetScoreTileSelectionUIController()
            {
                if (null == this.scoreTileSelectionUIController)
                {
                    this.scoreTileSelectionUIController = this.GetComponentInChildren<ScoreTileSelectionUIController>();
                }
                return this.scoreTileSelectionUIController;
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
