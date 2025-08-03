using UnityEngine;

public class ShopButtonController : MonoBehaviour
{

    public ProgressionManager progressionManager;
    public UIManagerMainMenu uIManager;


    public void ButtonBuyJumpPad()
    {

        if (progressionManager != null)
        {
            progressionManager.BuyJumpPad();
            UpdateJumpPadBuyButton();
        }
        else
        {
            Debug.LogWarning("ProgressionManager not found in current scene!");
        }
    }

    public void UpdateJumpPadBuyButton()
{
    if (progressionManager.hasBoughtJumpPad)
    {
        uIManager.jumpPadBuyButton.interactable = false;
        uIManager.jumpPadBuyButtonText.text = "Bought";
    }
    else
    {
        uIManager.jumpPadBuyButton.interactable = true;
        uIManager.jumpPadBuyButtonText.text = "Buy";
    }
}

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        progressionManager = FindFirstObjectByType<ProgressionManager>();
        uIManager = FindFirstObjectByType<UIManagerMainMenu>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
