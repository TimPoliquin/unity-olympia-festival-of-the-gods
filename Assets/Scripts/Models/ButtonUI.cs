using System.Collections;
using System.Collections.Generic;
using Azul.Cursor;
using TMPro;
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
            [SerializeField] private TextMeshProUGUI labelText;

            private CursorChange cursorChange;

            void Awake()
            {
                this.cursorChange = this.GetComponentInChildren<CursorChange>();
            }

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

            public void RemoveAllListeners()
            {
                this.button.onClick.RemoveAllListeners();
            }

            public void SetInteractable(bool interactable)
            {
                this.cursorChange.enabled = interactable;
                this.button.interactable = interactable;
            }

            public void SetActive(bool active)
            {
                this.gameObject.SetActive(active);
            }

            public void SetText(string text)
            {
                this.labelText.text = text;
            }
        }
    }
}
