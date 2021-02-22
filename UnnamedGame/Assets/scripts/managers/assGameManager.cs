using System.Collections;
using UnityEngine;

public class assGameManager : SimpleSingleton<assGameManager>
{
    public bool NoDebugsOnEditor;

    private void Start()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        Core.RemoveDebugLogsOnBuild();

        if (NoDebugsOnEditor) {
            Core.RemoveDebugLogsOnEditor();
        }
    }
}