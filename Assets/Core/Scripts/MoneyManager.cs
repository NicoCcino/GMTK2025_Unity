using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MoneyManager : MonoBehaviour
{

    public int money = 0; // Amount of money the player has
    public int score = 0; // Player's score, can be used for other purposes
    public TextMeshProUGUI moneyText; // Reference to a UI Text component to display the money

    void DisplayMoney()
    {
        // This method would typically update a UI element to show the current amount of money
        Debug.Log("Current Money: " + money);
        moneyText.text = money.ToString();

    }

    public void AddMoney(int amount)
    {
        // Method to add money to the player's total
        money += amount;
        score += amount; // Le score s'incrémente en même temps que l'argent (mais lui ne diminue jamais)
        Debug.Log("Money added: " + amount + ". Total Money: " + money);
        DisplayMoney();
    }

    void RemoveMoney(int amount)
    {
        // Method to remove money from the player's total
        if (money >= amount)
        {
            money -= amount;
            Debug.Log("Money removed: " + amount + ". Total Money: " + money);

            DisplayMoney();
        }
        else
        {
            Debug.LogWarning("Not enough money to remove: " + amount + ". Current Money: " + money);
        }
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
