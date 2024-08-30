using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

static class ListExtensions
{
    public static void Shuffle<T>(this IList<T> list)
    {
        System.Random rng = new();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}

namespace Utils
{
    public sealed class ListUtils
    {
        public static T GetRandomElement<T>(List<T> list)
        {
            return list[UnityEngine.Random.Range(0, list.Count)];
        }

        public static T GetRandomElement<T>(T[] list)
        {
            return list[UnityEngine.Random.Range(0, list.Length)];
        }
    }
}
