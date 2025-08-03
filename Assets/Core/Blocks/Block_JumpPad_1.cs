using UnityEngine;

public class Block_JumpPad_1 : Block
{
    public Block_JumpPad_1() : base("JumpPad", "Prefab_JumpPad_1", "Sprite_JumpPad_1")
    {
        // Pattern (3x3 matrix - visual representation):
        // [0,2] [1,2] [2,2]     O O O
        // [0,1] [1,1] [2,1]  =  O x O  
        // [0,0] [1,0] [2,0]     O 0 O

        if (blockMatrix != null)
        {
            blockMatrix[1, 1].isSolid = false;  // Center bottom cell is solid
            blockMatrix[1, 1].blockType = BlockType.JumpPad;
        }

        unlockPrice = 100;
    }
}
