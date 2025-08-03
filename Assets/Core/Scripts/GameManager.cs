using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class GameManager : MonoBehaviour
{

    public UIManager uiManager;
    public ProgressionManager progressionManager;
    public MoneyManager moneyManager;
    public AudioSource audioSourceMusic;
    public AudioSource audioSourceSing;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (uiManager == null)
        {
            uiManager = FindFirstObjectByType<UIManager>();
            Debug.Log("Game Manager found and plugged uiManager");
        }
        if (progressionManager == null)
        {
            progressionManager = FindFirstObjectByType<ProgressionManager>();
            Debug.Log("Game Manager found and plugged progressionManager");
        }
        if (moneyManager == null)
        {
            moneyManager = FindFirstObjectByType<MoneyManager>();
            Debug.Log("Game Manager found and plugged progressionManager");
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Script_Move_World.isPlayerDead)
        {
            uiManager.ShowGameOverScreen();

            // Update high score
            if (progressionManager.highScore < moneyManager.money) {
                progressionManager.highScore = moneyManager.money;
            }
            
            // Update money
            progressionManager.metaMoney += moneyManager.money;

            moneyManager.money = 0;
            audioSourceMusic.Stop();
            audioSourceSing.Stop();
        }
    }
}
