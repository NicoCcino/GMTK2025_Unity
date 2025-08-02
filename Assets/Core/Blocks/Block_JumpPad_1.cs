using UnityEngine;

public class Block_JumpPad_1 : BlockData
{
    public Block_JumpPad_1()
    {
        blockName = "JumpPad";

        // Assignation du prefab
        blockPrefab = Resources.Load<GameObject>("Prefab_Block_T_5");

        // blockMatrix = new Cell[3, 3];
        
        InitializeMatrix();


    }

    protected override void InitializeMatrix()
    {
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                blockMatrix[x, y] = new Cell(blockPrefab, this, new Vector2Int(x, y));
            }

        }
        blockMatrix[1, 0].isSolid = true;  // Exemple spécifique
    }

    public override void Trigger(Vector2Int position, GameObject player)
    {
        base.Trigger(position, player);
        Debug.Log("JumpPad activated!");
        // Logique du jump pad (ex: ajouter force au joueur)
    }
}
