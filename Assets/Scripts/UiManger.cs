using UnityEngine;
using TMPro;




public class UiManger : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] GameObject messagePanel;
    [SerializeField] TextMeshProUGUI textMessage;
    [SerializeField] TextMeshProUGUI instructionsText;



    void OnEnable()
    {
        gameManager.onGamestateChanged += HandleStateChanged;
        HandleStateChanged(gameManager.CurrentGamestate);
    }

    void OnDisable()
    {
        gameManager.onGamestateChanged -= HandleStateChanged;

    }

    void HandleStateChanged(GameManager.GameState gameState)
    {
        switch (gameState)
        {
            case GameManager.GameState.Scanning:
                ShowInstructions("Find and scan the image to start game");
                break;
            case GameManager.GameState.Placing:
                ShowInstructions("Image scanned, now find a location to play and tap ypur screen to begin");
                break;
            case GameManager.GameState.Playing:
                ShowInstructions(gameManager.ActiveMiniGame.GameInstructions);
                    break;

            case GameManager.GameState.Win:
                ShowMessage("You are a Winner!!");
                break;

            case GameManager.GameState.Lose:
                ShowMessage("Failed, better luck next time");
                break;

            default:
                messagePanel.SetActive(false); 
                break;

        }


    }

    void ShowMessage(string message)
    {
        instructionsText.gameObject.SetActive(false);
        textMessage.text = message;
        messagePanel.SetActive(true);

    }

    void ShowInstructions(string instructions)
    {
        messagePanel.SetActive(false);
        instructionsText.text = instructions;
        instructionsText.gameObject.SetActive(true);
    }
}
