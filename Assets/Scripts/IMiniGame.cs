using UnityEngine;

public interface IMiniGame
{
    event System.Action onWin;
    event System.Action onLose;

    string GameInstructions { get; }

    void StartGame(Vector3 basePosition);

    void ResetGame();
}
