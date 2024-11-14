using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Azul
{
    namespace Model
    {
        public class GameStartUIIntroStep : MonoBehaviour
        {
            private UnityEvent onPlay = new();
            public void OnPlay()
            {
                this.onPlay.Invoke();
            }

            public void OnOptions()
            {
                System.Instance.GetUIController().GetOptionsPanelUIController().ShowOptions();
            }

            public void OnQuit()
            {
                System.Instance.Quit();
            }

            public void AddOnPlayListener(UnityAction listener)
            {
                this.onPlay.AddListener(listener);
            }
        }
    }
}
