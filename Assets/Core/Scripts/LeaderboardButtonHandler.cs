using UnityEngine;
using UnityEngine.SceneManagement;

public class LeaderboardButtonHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GoToLeaderBoard()
    {
        SceneManager.LoadScene("Scene_Leaderboard");
    }
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("Scene_MainMenu");
    }
}
