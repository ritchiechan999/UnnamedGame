using System.Collections;
using UnityEngine;

public static partial class Core
{
    public static void RemoveDebugLogsOnBuild()
    {
#if UNITY_EDITOR
        Debug.unityLogger.logEnabled = true;
#else
        Debug.unityLogger.logEnabled = false;
#endif
    }

    public static void RemoveDebugLogsOnEditor()
    {
        Debug.unityLogger.logEnabled = false;
    }
}