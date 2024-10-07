using System;
using System.Collections;
using System.Collections.Generic;
using Azul.Model;
using Azul.TileAnimation;
using Azul.Util;
using UnityEngine;

namespace Azul
{
    namespace TileAnimation
    {
        public struct TilesMoveConfig
        {
            public Vector3 Position { get; init; }
            public float Time { get; init; }
            public float Delay { get; init; }
            public Action<Tile> AfterEach { get; init; }
        }
    }
    namespace Controller
    {
        public class TileAnimationController : MonoBehaviour
        {
            private bool isAnimating = false;

            public bool IsAnimating()
            {
                return this.isAnimating;
            }
            public CoroutineResult MoveTiles(List<Tile> tiles, TilesMoveConfig config)
            {
                CoroutineResult result = CoroutineResult.Single();
                this.StartCoroutine(this.MoveMultipleCoroutine(tiles, config, result));
                return result;
            }

            private IEnumerator MoveMultipleCoroutine(List<Tile> tiles, TilesMoveConfig config, CoroutineResult result)
            {
                result.Start();
                this.isAnimating = true;
                foreach (Tile tile in tiles)
                {
                    yield return this.MoveCoroutine(tile, config);
                    this.isAnimating = true;
                    yield return new WaitForSeconds(config.Delay);
                }
                this.isAnimating = false;
                result.Finish();
            }

            private IEnumerator MoveCoroutine(Tile tile, TilesMoveConfig tileMoveConfig)
            {
                this.isAnimating = true;
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
                this.isAnimating = false;
                if (tileMoveConfig.AfterEach != null)
                {
                    tileMoveConfig.AfterEach.Invoke(tile);
                }
            }
        }
    }
}
