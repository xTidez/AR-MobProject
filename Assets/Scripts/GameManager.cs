using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.Interaction.Toolkit.Samples.ARStarterAssets;


public class GameManager : MonoBehaviour
{
    public enum GameState 
    { 
        Scanning,
        Placing,
        Playing,
        Win,
        Lose
    }

    public GameState CurrentGamestate { get; private set; } = GameState.Scanning;
    public IMiniGame ActiveMiniGame => activeMiniGame;


    [System.Serializable] public class MiniGameEntry
    {
        public string imageName;
        public MonoBehaviour gameSource;
    }

    [SerializeField] ARPlaneManager planeManager;
    [SerializeField] ARInteractorSpawnTrigger PlacementTrigger;
    [SerializeField] List<MiniGameEntry> miniGames;

    IMiniGame activeMiniGame;
    public event System.Action<GameState> onGamestateChanged;

    private void Awake()
    {
        planeManager.enabled = false;
        PlacementTrigger.enabled = false; 
        PlacementTrigger.objectSpawnTriggered.AddListener(OnPlacementConfirmed);
        

    }

    private void OnDestroy()
    {

        PlacementTrigger.objectSpawnTriggered.RemoveListener(OnPlacementConfirmed);
        if (activeMiniGame != null)
        {
            activeMiniGame.onWin -= HandleGameWon;
            activeMiniGame.onLose -= HandleGameLost;
        }
    }

    public void PlaceGame(string imageName)
    {
        if (CurrentGamestate != GameState.Scanning) return;

        MiniGameEntry entryGame = miniGames.Find(game => game.imageName == imageName);

        if (entryGame == null)
        {
            Debug.LogError($"Invalid image name: {imageName}");
            return;
        }
        activeMiniGame = entryGame.gameSource as IMiniGame;
        if (activeMiniGame == null)
        {
            Debug.LogWarning($"{entryGame.gameSource} does not imp Iminigame");
            return;
        }

        activeMiniGame.onWin += HandleGameWon;
        activeMiniGame.onLose += HandleGameLost;

        SetGameState(GameState.Placing);
        planeManager.enabled = true;
        PlacementTrigger.enabled = true;

        Debug.Log("Loc scnned, now placing gameboard");

    }

    private void OnPlacementConfirmed(Vector3 target, Vector3 normal)
    {
        if (CurrentGamestate != GameState.Placing) { return; }

        SetGameState(GameState.Playing);

        planeManager.enabled = false;
        PlacementTrigger.enabled = false;

        activeMiniGame.StartGame(target);
        Debug.Log($"Stack base placed at {target}");

    }

    void SetGameState(GameState newState)
    {
        CurrentGamestate = newState;
        onGamestateChanged?.Invoke(newState);

    }

    void HandleGameWon()
    {
        SetGameState(GameState.Win);

    }

    async void HandleGameLost()
    {
        SetGameState(GameState.Lose);
        


    }
    

    public void RestartGame()
    {
        if (CurrentGamestate != GameState.Win && CurrentGamestate != GameState.Lose) { return; }

        activeMiniGame.ResetGame();
        SetGameState(GameState.Placing);
        planeManager.enabled = true;
        PlacementTrigger.enabled = true;

    }
}
