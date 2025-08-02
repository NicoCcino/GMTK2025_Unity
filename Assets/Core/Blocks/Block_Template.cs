using UnityEngine;

public class Block_Template : Block
{
    public Block_Template() : base("TemplateName", "Prefab_YourPrefab")
    {
        // Pattern (3x3 matrix - visual representation):
        // [0,2] [1,2] [2,2]     O O O
        // [0,1] [1,1] [2,1]  =  O O O  
        // [0,0] [1,0] [2,0]     O O O
        //
        // INSTRUCTIONS:
        // 1. Replace O with X where you want solid cells
        // 2. Set blockMatrix[x,y].isSolid = true for each X
        // 3. Array coordinates: [x,y] where x=column, y=row
        //    - x: 0=left, 1=center, 2=right
        //    - y: 0=bottom, 1=middle, 2=top
        
        if (blockMatrix != null)
        {
            // Example patterns:
            
            // Single center cell:
            // blockMatrix[1, 1].isSolid = true;
            
            // L-shape:
            // blockMatrix[0, 0].isSolid = true;  // Bottom left
            // blockMatrix[0, 1].isSolid = true;  // Middle left 
            // blockMatrix[1, 0].isSolid = true;  // Bottom center
            
            // Line (horizontal):
            // blockMatrix[0, 1].isSolid = true;  // Left
            // blockMatrix[1, 1].isSolid = true;  // Center
            // blockMatrix[2, 1].isSolid = true;  // Right
            
            // Line (vertical):
            // blockMatrix[1, 0].isSolid = true;  // Bottom
            // blockMatrix[1, 1].isSolid = true;  // Middle
            // blockMatrix[1, 2].isSolid = true;  // Top
        }
    }

    public override void Trigger(Vector2Int position, GameObject player)
    {
        base.Trigger(position, player);
        Debug.Log("Template block triggered!");
        // Add your custom block behavior here
    }
}
