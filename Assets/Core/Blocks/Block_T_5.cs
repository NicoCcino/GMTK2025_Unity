using UnityEngine;

public class Block_T_5 : Block
{
    public Block_T_5() : base("Block_T_5", "Prefab_Block_T_5", "Sprite_Block_T_5") // Setup name, Prefab path, Preview Sprite path
    {
        // Pattern (3x3 matrix - visual representation):
        // [0,2] [1,2] [2,2]     X X X
        // [0,1] [1,1] [2,1]  =  O X O  
        // [0,0] [1,0] [2,0]     O X O
        
        if (blockMatrix != null)
        {
            // Top row (y=2)
            blockMatrix[0, 2].isSolid = true;   // Left top
            blockMatrix[1, 2].isSolid = true;   // Center top  
            blockMatrix[2, 2].isSolid = true;   // Right top
            
            // Middle row (y=1)
            blockMatrix[1, 1].isSolid = true;   // Center middle
            
            // Bottom row (y=0)
            blockMatrix[1, 0].isSolid = true;   // Center bottom
        }
    }

    public override void Trigger(Vector2Int position, GameObject player)
    {

    }
}
