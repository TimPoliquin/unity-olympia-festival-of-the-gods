using System.Collections;
using System.Collections.Generic;
using Azul.Model;
using UnityEngine;

namespace Azul
{
    namespace Controller
    {
        public class TileColorMappingController : MonoBehaviour
        {
            [SerializeField] private List<ColoredValue<string>> godNames;
            [SerializeField] private List<ColoredValue<Color>> fireColors;

            public string GetGodName(TileColor color)
            {
                return this.godNames.Find(godName => godName.GetTileColor() == color).GetValue();
            }

            public int GetGodPoints(TileColor color)
            {
                return System.Instance.GetScoreBoardController().GetCompletionPoints(color);
            }

            public Color GetGodFireColor(TileColor color)
            {
                return this.fireColors.Find(fireColor => fireColor.GetTileColor() == color).GetValue();
            }
        }
    }
}
