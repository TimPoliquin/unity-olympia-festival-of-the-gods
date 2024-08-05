using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Azul
{
    public class InputController : MonoBehaviour
    {
        public void OnSelect(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                this.OnSelectStart(context);
            }
            else if (context.canceled)
            {
                this.OnSelectRelease(context);
            }
        }

        private void OnSelectStart(InputAction.CallbackContext context)
        {
            // UnityEngine.Debug.Log("Pressed");
        }

        private void OnSelectRelease(InputAction.CallbackContext callbackContext)
        {
            // UnityEngine.Debug.Log("Released");
        }
    }
}
