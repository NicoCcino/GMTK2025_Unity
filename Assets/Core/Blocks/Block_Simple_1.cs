using UnityEngine;

public class Block_Simple_1 : Block
{
    public Block_Simple_1() : base("Simple Block", "Prefab_Block_Simple_1")
    {
        // Set all cells to solid for a simple 1x1 block
        if (blockMatrix != null)
        {
            blockMatrix[1, 1].isSolid = true;  // Center cell is solid
        }
    }

    public override void Trigger(Vector2Int position, GameObject player)
    {
        base.Trigger(position, player);
        Debug.Log("Simple block triggered - nothing special happens!");
    }
}
