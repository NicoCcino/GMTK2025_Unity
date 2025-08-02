using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Player Settings")]
    [Tooltip("Reference to the Player GameObject")]
    public GameObject player;

    [Header("Money Manager")]
    [Tooltip("Reference to the Money Manager")]
    public MoneyManager moneyManager;


    [Header("Game Over Screen")]
    [Tooltip("Reference to the Game Over screen UI")]
    public GameObject gameOverScreen;
    public TextMeshProUGUI textScoreSentence; // Reference to a UI Text component to display the score sentence
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (gameOverScreen == null)
        {
            Debug.LogError("Game Over Screen is not assigned in the UIManager.");

            Debug.LogError("Trying to find Game Over screen in the scene, by name Panel_GameOver");
            gameOverScreen = GameObject.Find("Panel_GameOver");
        }

        HideGameOverScreen();

        if (textScoreSentence == null)
        {
            Debug.LogError("Text Score Sentence is not assigned in the UIManager.");

            Debug.LogError("Trying to find Text Score Sentence in the scene, by name Text_ScoreSentence");
            textScoreSentence = GameObject.Find("Text_ScoreSentence").GetComponent<TextMeshProUGUI>();
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Script_Move_World.isPlayerDead)
        {
            ShowGameOverScreen();
        }

    }

    public void ShowGameOverScreen()
    {
        // Logic to show the game over screen
        Debug.Log("Game Over! Showing Game Over Screen.");
        // Here you enable a UI panel and display the score or any other relevant information
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
            string textScoreString = $"You have walked over {moneyManager.score} blocks.";
            textScoreSentence.text = textScoreString;
        }
    }

    public void HideGameOverScreen()
    {
        // Logic to hide the game over screen
        Debug.Log("Hiding Game Over Screen.");
        // Here you would typically disable a UI panel or similar
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(false);
        }
    }

    public Image previewBlockImage1;
    public Image previewBlockImage2;
    public Image previewBlockImage3;

    public void UpdateBlocksQueuePreview(Block[] blocksQueue)
    {
        if (blocksQueue == null || blocksQueue.Length < 3)
        {
            Debug.LogWarning("blockQueue n'est pas initialisée ou trop petite.");
            return;
        }

        // Tableau des images UI
        Image[] previewImages = new Image[] { previewBlockImage1, previewBlockImage2, previewBlockImage3 };

        for (int i = 0; i < 3; i++)
        {
            var block = blocksQueue[i];
            if (block != null && block.previewSprite != null)
            {
                previewImages[i].sprite = block.previewSprite;
                previewImages[i].color = Color.white;  // Assure que l'image est visible
            }
            else
            {
                previewImages[i].sprite = null;        // Pas de sprite => image vide
                previewImages[i].color = new Color(1, 1, 1, 0); // Invisible (alpha 0)
            }
        }
    }
    public void RestartGame()
    {
        // Logic to restart the game
        Debug.Log("Restarting Game.");
        Script_Move_World.isPlayerDead = false; // Reset player state
        moneyManager.money = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        // Logic to restart the game
        Debug.Log("Going to Main Menu.");
        Script_Move_World.isPlayerDead = false; // Reset player state
        moneyManager.money = 0;
        SceneManager.LoadScene("Scene_MainMenu");

    }
}
