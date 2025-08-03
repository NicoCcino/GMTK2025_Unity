using UnityEngine;

public class ShopButtonController : MonoBehaviour
{

    public ProgressionManager progressionManager;
    public UIManagerMainMenu uIManager;

// ************** JumpPad **************
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

// ************** InvinciblePad **************
    public void ButtonBuyInvinciblePad()
    {
        if (progressionManager != null)
        {
            progressionManager.BuyInvinciblePad();
            UpdateInvinciblePadBuyButton();
        }
        else
        {
            Debug.LogWarning("ProgressionManager not found in current scene!");
        }
    }

    public void UpdateInvinciblePadBuyButton()
    {
        if (progressionManager.hasBoughtInvinciblePad)
        {
            uIManager.invinciblePadBuyButton.interactable = false;
            uIManager.invinciblePadBuyButtonText.text = "Bought";
        }
        else
        {
            uIManager.invinciblePadBuyButton.interactable = true;
            uIManager.invinciblePadBuyButtonText.text = "Buy";
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
