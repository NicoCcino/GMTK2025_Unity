

using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[DefaultExecutionOrder(-100)]
public class ProgressionManager : MonoBehaviour
{
    public static ProgressionManager Instance { get; private set; }

    public int metaMoney = 0;
    public bool[] unlockedLevels;

    public UIManagerMainMenu uiManagerMainMenu;

    public bool hasBoughtJumpPad = false;
    public bool hasBoughtInvinciblePad = false;

    private void Awake()
    {
        // Singleton : garantir qu'il n'y ait qu'une seule instance
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Détruire les doublons
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Persister entre les scènes

        // Exemple d'initialisation
        unlockedLevels = new bool[10];
        unlockedLevels[0] = true; // Débloquer le premier niveau par défaut

        InitializeAllBlocks();
        InitializeAvailableBlocks();
    }

    public void Start()
    {
        //CheckRefUIManagerMainMenu();
        //BuyJumpPad();
    }



    public List<Block> allBlocks = new List<Block>(); // Liste des blocs programmés
    public List<Block> availableBlocks = new List<Block>(); // Liste des blocs achetés et donc disponibles pour le joueur


    public void InitializeAllBlocks()
    {
        allBlocks = new List<Block>
        {
            new Block_JumpPad_1(),
            new Block_InvinciblePad_1(),
            new Block_T_5(),
            new Block_Simple_1()
        };

        Debug.Log($"Initialized {allBlocks.Count} blocks");
        foreach (Block block in allBlocks)
        {
            Debug.Log($"Block initialized: {block.blockName}");
        }
    }

    public void InitializeAvailableBlocks()
    {
        availableBlocks = new List<Block>
        {
            // new Block_JumpPad_1(),
            new Block_T_5(),
            new Block_Simple_1()
        };

        Debug.Log($"Initialized {availableBlocks.Count} blocks");
        foreach (Block block in availableBlocks)
        {
            Debug.Log($"Block initialized: {block.blockName}");
        }
    }

    public bool BuyBlock(Block newBlock, int price = 0)
    {
        Debug.Log($"Trying to buy block {newBlock.blockName}");
        price = newBlock.unlockPrice; // Je récupère le coût d'upgrade du bloc
        Debug.Log($"Price is {price}");
        if (metaMoney >= price) // Vérifie que l'argent du joueur est suffisant
        {
            Debug.Log("Player has enough money and is about to buy");
            metaMoney -= price; // Update argent
            uiManagerMainMenu.UpdateMetaMoneyShopUI();         // Update UI argent

            availableBlocks.Add(newBlock); // Ajout du bloc au pool
            Debug.Log("Block added to available blocks pool from BuyBlock function");
            return true;
        }
        else
        {
            Debug.LogWarning("Not enough metaMoney to purchase block");
            return false;
        }
    }

    public void BuyJumpPad()
    {
        Debug.Log("BuyJumpPad function started");
        Block blockToBuy = new Block_JumpPad_1();
        if (BuyBlock(blockToBuy) == true)
        {
            hasBoughtJumpPad = true; 
            Debug.Log("Keeping in memory: player has bought jumpPad");
        }
    }

    public void BuyInvinciblePad()
    {
        Debug.Log("BuyInvinciblePad function started");
        Block blockToBuy = new Block_InvinciblePad_1();
        if (BuyBlock(blockToBuy) == true)
        {
            hasBoughtInvinciblePad = true;
            Debug.Log("Keeping in memory: player has bought invinciblePad");
        }
    }

    public void UnlockLevel(int level)
    {
        if (level >= 0 && level < unlockedLevels.Length)
            unlockedLevels[level] = true;
    }

    public bool IsLevelUnlocked(int level)
    {
        return level >= 0 && level < unlockedLevels.Length && unlockedLevels[level];
    }

    public void CheckRefUIManagerMainMenu()
    {
        if (uiManagerMainMenu == null)
        {
            uiManagerMainMenu = FindFirstObjectByType<UIManagerMainMenu>();
            Debug.Log("Progression Manager found and plugged uiManagerMainMenu");
        }
    }
}
