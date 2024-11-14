using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Azul
{
    namespace Model
    {
        public class ButtonUI : MonoBehaviour
        {
            [SerializeField] private Button button;
            [SerializeField] private AudioClip buttonDown;
            [SerializeField] private AudioClip buttonUp;

            public void OnPointerDown()
            {
                if (this.button.interactable)
                {
                    System.Instance.GetAudioController().PlaySFX(this.buttonDown, .5f);
                }
            }

            public void OnPointerUp()
            {
                if (this.button.interactable)
                {
                    System.Instance.GetAudioController().PlaySFX(this.buttonUp, .5f);
                }
            }

            public void AddOnClickListener(UnityAction listener)
            {
                this.button.onClick.AddListener(listener);
            }

            public void SetInteractable(bool interactable)
            {
                this.button.interactable = interactable;
            }
        }
    }
}
