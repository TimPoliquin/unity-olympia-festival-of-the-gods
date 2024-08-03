using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azul
{
    namespace Model
    {
        public enum PlayerColor
        {
            BAIGE,
            BLACK,
            GREY,
            WHITE,
        }
        public class Player : MonoBehaviour
        {
            [SerializeField] private string playerName;
            [SerializeField] private PlayerColor color;
        }
    }
}
