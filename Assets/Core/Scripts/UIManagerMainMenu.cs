using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManagerMainMenu : MonoBehaviour
{

    public ProgressionManager progressionManager;

    [Header("Shop UI")]
    [Tooltip("Shop UI references")]
    public TextMeshProUGUI metaMoneyText;
    public TextMeshProUGUI jumpPadPriceText;
    public Button jumpPadBuyButton;     // Le bouton d'achat dans l'UI
    public Button invinciblePadBuyButton;     // Le bouton d'achat dans l'UI
    public TextMeshProUGUI jumpPadBuyButtonText;   // Le texte affiché sur le bouton
    public TextMeshProUGUI invinciblePadBuyButtonText;   // Le texte affiché sur le bouton



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetReferences();
        InitializeMainMenuUI();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetReferences()
    {
        if (progressionManager == null)
        {
            progressionManager = FindFirstObjectByType<ProgressionManager>();
            Debug.Log("UI Manager Main Menu has found and plugged progressionManager");
        }
        progressionManager.CheckRefUIManagerMainMenu();
    }


    public void InitializeMainMenuUI()
    {
        UpdateMetaMoneyShopUI();
    }

    public void UpdateMetaMoneyShopUI()
    {
        metaMoneyText.text = progressionManager.metaMoney.ToString();
        int jumpPadPrice = 10; // En dur pour l'instant, idéalement devrait être lié au Block
        jumpPadPriceText.text = jumpPadPrice.ToString(); // To string 
    }


}
