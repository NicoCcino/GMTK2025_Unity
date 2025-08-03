using UnityEngine;

public class Block_L_4 : Block
{
    public Block_L_4() : base("Block_T_5", "Prefab_Block_T_5", "Sprite_Block_L_4") // Setup name, Prefab path, Preview Sprite path
    {
        // Pattern (3x3 matrix - visual representation):
        // [0,2] [1,2] [2,2]     X O O
        // [0,1] [1,1] [2,1]  =  X 0 O  
        // [0,0] [1,0] [2,0]     X X O
        
        if (blockMatrix != null)
        {
            // Top row (y=2)
            blockMatrix[0, 2].isSolid = true;   // Left top
            blockMatrix[0, 2].blockType = BlockType.Standard;
            
            // Middle row (y=1)
            blockMatrix[0, 1].isSolid = true;   // Center middle
            blockMatrix[0, 1].blockType = BlockType.Standard;
            // Bottom row (y=0)
            blockMatrix[1, 0].isSolid = true;   // Center bottom
            blockMatrix[1, 0].blockType = BlockType.Standard;
            blockMatrix[0, 0].isSolid = true;   // Center bottom
            blockMatrix[0, 0].blockType = BlockType.Standard;
        }
    }
}
