using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Azul.GameStartEvents;
using Azul.Model;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utils;

namespace Azul
{
    namespace GameStartEvents
    {
        public class OnPlayerCountSelectionPayload
        {
            public int PlayerCount { get; init; }
        }

    }
    namespace Model
    {
        public class GameStartUI : MonoBehaviour
        {
            [SerializeField] private List<ColoredValue<IconUI>> icons;
            [SerializeField] private GameStartUIIntroStep introStep;
            [SerializeField] private GameStartUISetupStep setupStep;

            // Start is called before the first frame update
            void Awake()
            {
                this.ShowIntroStep();
            }

            void Start()
            {
                this.icons.ForEach(icon => icon.GetValue().SetTileColor(icon.GetTileColor()));
            }

            public GameStartUIIntroStep GetIntroStep()
            {
                return this.introStep;
            }

            public GameStartUISetupStep GetSetupStep()
            {
                return this.setupStep;
            }

            public void ShowIntroStep()
            {
                this.introStep.gameObject.SetActive(true);
                this.setupStep.gameObject.SetActive(false);
            }

            public void ShowSetupStep()
            {
                this.introStep.gameObject.SetActive(false);
                this.setupStep.gameObject.SetActive(true);
            }
        }
    }
}