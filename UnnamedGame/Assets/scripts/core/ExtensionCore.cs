using System.Collections;
using UnityEngine;

public static class ExtensionCore
{
    public static bool ContainsLayer(this int mask, int layer)
    {
        return (mask & (1 << layer)) != 0;
    }
}