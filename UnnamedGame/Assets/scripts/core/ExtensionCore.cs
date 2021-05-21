using System.Collections;
using UnityEngine;

public static class ExtensionCore
{
    public static bool ContainsLayer(this int mask, int layer)
    {
        return (mask & (1 << layer)) != 0;
    }

    public static T GetRandom<T>(this T[] array)
    {
        return array[Random.Range(0, array.Length)];
    }
}