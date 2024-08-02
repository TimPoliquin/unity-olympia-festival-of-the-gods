using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azul
{
    namespace Model
    {
        public class StarSpace : MonoBehaviour
        {
            [SerializeField] private Tile tile;
            [SerializeField][Range(1, 6)] private int value;
            [SerializeField] private bool occupied;

            // Start is called before the first frame update
            void Start()
            {

            }

            // Update is called once per frame
            void Update()
            {

            }
        }

    }
}
