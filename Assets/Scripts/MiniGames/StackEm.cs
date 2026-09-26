using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;
using System.Collections.Generic;


public class StackEm : MonoBehaviour, IMiniGame
{
    [SerializeField] ObjectSpawner objectSpawner;
    [SerializeField] Transform spawnPoint;
    [SerializeField] float spawnOffset = 0.5f;
    [SerializeField] int ScoreToWin = 3;
    [SerializeField] Transform fallZone;
    [SerializeField] float fallZoneOffset;

    List<GameObject> spawnedObjects = new List<GameObject>();
    bool isGameRunning;

    int score;
    Vector3 basePosition;
    GameObject previousStackedObject;

    public event System.Action onWin;
    public event System.Action onLose;

    [SerializeField, TextArea] string gameInstructions = "Stack {0} boxex on eachother to win." +
        " Tip: hold your phone level with the box to keep it straight when picking it up.";
    public string GameInstructions => string.Format(gameInstructions, ScoreToWin);
  

    void HandleObjectSpawned(GameObject obj)
    {
        var stackable = obj.GetComponent<StackableObject>();
        spawnedObjects.Add(obj);
        if (stackable == null)
        {
            Debug.LogWarning("prefab is missing stackable obj");
            return;

        }
        stackable.SetRequiredSupport(previousStackedObject);
        stackable.onFailed += HandleFailed;
        stackable.onStable += HandleStable;
    }

    void SpawnNextObject()
    {
        spawnPoint.position = basePosition + Vector3.right * spawnOffset + Vector3.up * 0.1f;
        objectSpawner.TrySpawnObject(spawnPoint.position, Vector3.up);

    }

    void HandleFailed(StackableObject stackObject)
    {
        if(!isGameRunning) { return; }
        EndGame();
        onLose?.Invoke();

    }

    void HandleStable(StackableObject stackObject)
    {

        previousStackedObject = stackObject.gameObject;
        score++;
        if (score >= ScoreToWin)
        {
            EndGame();
            onWin?.Invoke();
        }
        else
        {
            SpawnNextObject();
        }

    }

    public void StartGame(Vector3 BasePosition)
    {
        ResetGame();
        isGameRunning = true;
        
        basePosition = BasePosition;
        fallZone.position = BasePosition + Vector3.down * fallZoneOffset; 
        previousStackedObject = null;
        objectSpawner.objectSpawned += HandleObjectSpawned;
        SpawnNextObject();

    }

    void EndGame()
    {
        isGameRunning = false;
        objectSpawner.objectSpawned -= HandleObjectSpawned;

    }

    public void ResetGame()
    {
        EndGame();
        score = 0;
        foreach (GameObject obj in spawnedObjects)
        {
            if(obj != null) Destroy(obj);

        }
                   

    }






}
