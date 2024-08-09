using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azul
{
    namespace Controller
    {
        public class UIController : MonoBehaviour
        {
            private StarUIController starUIController;

            public void InitializeListeners()
            {
                this.GetStarUIController().InitializeListeners();
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
