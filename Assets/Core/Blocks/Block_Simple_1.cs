using UnityEngine;

public class Block_Simple_1 : Block
{
    public Block_Simple_1() : base("Simple Block", "Prefab_Block_Simple_1","Sprite_Block_Simple_1")
    {
        // Pattern (3x3 matrix - visual representation):
        // [0,2] [1,2] [2,2]     O O O
        // [0,1] [1,1] [2,1]  =  O X O  
        // [0,0] [1,0] [2,0]     O O O
        
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
