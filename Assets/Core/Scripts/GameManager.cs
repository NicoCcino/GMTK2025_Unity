using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class GameManager : MonoBehaviour
{

    public UIManager uiManager;
    public ProgressionManager progressionManager;
    public MoneyManager moneyManager;
    public AudioSource audioSource1;
    public AudioSource audioSource2;

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
            progressionManager.metaMoney += moneyManager.money;
            moneyManager.money = 0;
            audioSource1.Stop();
            audioSource2.Stop();
        }
    }
}
