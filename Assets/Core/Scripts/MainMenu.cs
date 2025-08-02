using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    // Method to start the game by loading the first scene
    public void StartGame()
    {
        SceneManager.LoadScene("Scene_Freepouille");
    }

    // Method to quit the application
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game is quitting");
    }
}
