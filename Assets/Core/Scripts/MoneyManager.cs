using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MoneyManager : MonoBehaviour
{

    public int money = 0; // Amount of money the player has
    public TextMeshProUGUI moneyText; // Reference to a UI Text component to display the money

    void DisplayMoney()
    {
        // This method would typically update a UI element to show the current amount of money
        Debug.Log("Current Money: " + money);
        moneyText.text = money.ToString();

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        DisplayMoney();
    }

}
