

using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

[DefaultExecutionOrder(-100)]
public class ProgressionManager : MonoBehaviour
{
    public static ProgressionManager Instance { get; private set; }

    // Exemple de variables persistantes
    public int currentLevel = 1;
    public int totalScore = 0;
    public bool[] unlockedLevels;

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
        BuyJumpPad();
    }



    public List<Block> allBlocks = new List<Block>(); // Liste des blocs programmés
    public List<Block> availableBlocks = new List<Block>(); // Liste des blocs achetés et donc disponibles pour le joueur


    public void InitializeAllBlocks()
    {
        allBlocks = new List<Block>
        {
            new Block_JumpPad_1(),
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

    public void BuyBlock(Block newBlock)
    {
        availableBlocks.Add(newBlock);
    }

    public void BuyJumpPad()
    {
        Block blockToBuy = new Block_JumpPad_1();
        BuyBlock(blockToBuy);
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
}