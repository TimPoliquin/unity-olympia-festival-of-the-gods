using System;
using System.Collections;
using System.Collections.Generic;
using Azul.Model;
using Azul.TileAnimation;
using Azul.Util;
using Azul.Utils;
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
            public Action<Tile, int> AfterEach { get; init; }
        }
        public class TileAnimationControllerReadyCondition : ReadyCondition
        {
            public static readonly TileAnimationControllerReadyCondition Instance = new TileAnimationControllerReadyCondition();
            public bool IsReady()
            {
                return !System.Instance.GetTileAnimationController().IsAnimating();
            }
        }
    }
    namespace Controller
    {
        public class TileAnimationController : MonoBehaviour
        {
            [SerializeField] private AudioSource tileMoveSFX;
            private bool isAnimating = false;

            public bool IsAnimating()
            {
                return this.isAnimating;
            }
            public CoroutineStatus MoveTiles(List<Tile> tiles, TilesMoveConfig config)
            {
                CoroutineStatus status = CoroutineStatus.Single();
                this.StartCoroutine(this.MoveMultipleCoroutine(tiles, config, status));
                return status;
            }

            private IEnumerator MoveMultipleCoroutine(List<Tile> tiles, TilesMoveConfig config, CoroutineStatus status)
            {
                status.Start();
                this.isAnimating = true;
                for (int idx = 0; idx < tiles.Count; idx++)
                {
                    Tile tile = tiles[idx];
                    this.tileMoveSFX.Play();
                    yield return this.MoveCoroutine(tile, idx, config);
                    this.isAnimating = true;
                    yield return new WaitForSeconds(config.Delay);
                }
                this.isAnimating = false;
                status.Finish();
            }

            private IEnumerator MoveCoroutine(Tile tile, int idx, TilesMoveConfig tileMoveConfig)
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
                if (tileMoveConfig.AfterEach != null)
                {
                    tileMoveConfig.AfterEach.Invoke(tile, idx);
                }
            }

            public IEnumerator WaitUntilDoneAnimating()
            {
                if (this.isAnimating)
                {
                    yield return new WaitUntil(() => !this.isAnimating);
                }
            }
        }
    }
}
