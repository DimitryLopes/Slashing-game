using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions
{
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static T Last<T>(this IList<T> list)
    {
        int lastIndex = list.Count - 1;
        return list[lastIndex];
    }

    public static T GetRandom<T>(this IList<T> list)
    {
        int randomIndex = Random.Range(0, list.Count - 1);
        return list[randomIndex];
    }
}