using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

            void Awake()
            {
            }

            public void OnPointerDown()
            {
                if (this.button.interactable)
                {
                    System.Instance.GetAudioController().Play(this.buttonDown, .5f);
                }
            }

            public void OnPointerUp()
            {
                if (this.button.interactable)
                {
                    System.Instance.GetAudioController().Play(this.buttonUp, .5f);
                }
            }
        }
    }
}
