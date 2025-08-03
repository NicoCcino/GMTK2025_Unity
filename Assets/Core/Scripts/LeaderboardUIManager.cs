using UnityEngine;
using Dan.Main;
using Dan.Models;
using Dan.Demo;

public class LeaderboardUIManager : MonoBehaviour
{

    public LeaderboardShowcase lb;
    public ProgressionManager progressionManager;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        if (progressionManager == null)
        {
            progressionManager = FindFirstObjectByType<ProgressionManager>();
        }
        lb._playerScore = progressionManager.highScore;
        lb._playerScoreText.text = progressionManager.highScore.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
