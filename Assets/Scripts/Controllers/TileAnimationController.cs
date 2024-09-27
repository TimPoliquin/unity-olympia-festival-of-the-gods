using System;
using System.Collections;
using System.Collections.Generic;
using Azul.Model;
using Azul.TileAnimation;
using UnityEngine;

namespace Azul
{
    namespace TileAnimation
    {
        public class TileMoveConfig
        {
            public Vector3 Position { get; init; }
            public float Time { get; init; }
            public Action OnComplete { get; init; }
        }

        public class TilesMoveConfig : TileMoveConfig
        {
            public float Delay { get; init; }
            public Action<Tile> AfterEach { get; init; }
        }
    }
    namespace Controller
    {
        public class TileAnimationController : MonoBehaviour
        {
            public void MoveTile(Tile tile, TileMoveConfig config)
            {
                this.StartCoroutine(this.MoveCoroutine(tile, config));
            }

            public void MoveTiles(List<Tile> tiles, TilesMoveConfig config)
            {
                this.StartCoroutine(this.MoveMultipleCoroutine(tiles, config));
            }

            private IEnumerator MoveMultipleCoroutine(List<Tile> tiles, TilesMoveConfig config)
            {
                foreach (Tile tile in tiles)
                {
                    yield return this.MoveCoroutine(tile, new TileMoveConfig
                    {
                        Position = config.Position,
                        Time = config.Time,
                        OnComplete = () => config.AfterEach(tile)
                    });
                    yield return new WaitForSeconds(config.Delay);
                }
                config.OnComplete.Invoke();
            }

            private IEnumerator MoveCoroutine(Tile tile, TileMoveConfig tileMoveConfig)
            {
                tile.GetComponentInChildren<Collider>().enabled = false;
                tile.GetComponentInChildren<Rigidbody>().useGravity = false;
                Vector3 startingPosition = tile.transform.position;
                Vector3 direction = (tileMoveConfig.Position - tile.transform.position).normalized;
                float time = 0;
                while (Vector3.Distance(tile.transform.position, tileMoveConfig.Position) > .5f)
                {
                    time += Time.deltaTime / tileMoveConfig.Time;
                    tile.transform.position = Vector3.Lerp(startingPosition, tileMoveConfig.Position, time);
                    yield return null;
                }
                tile.GetComponentInChildren<Collider>().enabled = true;
                tile.GetComponentInChildren<Rigidbody>().useGravity = true;
                tileMoveConfig.OnComplete.Invoke();
            }
        }
    }
}
