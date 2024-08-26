using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;

namespace Azul
{
    namespace Model
    {
        public enum TileColor
        {
            RED,
            BLUE,
            YELLOW,
            GREEN,
            PURPLE,
            ORANGE,
            WILD,
            ONE
        }

        public sealed class TileColorUtils
        {
            public static TileColor[] GetTileColors()
            {
                return new TileColor[]{
                    TileColor.RED,
                    TileColor.BLUE,
                    TileColor.YELLOW,
                    TileColor.GREEN,
                    TileColor.PURPLE,
                    TileColor.ORANGE
                };
            }
        }
    }
}
