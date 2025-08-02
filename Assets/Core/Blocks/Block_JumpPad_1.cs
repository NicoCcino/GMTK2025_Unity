using UnityEngine;

public class Block_JumpPad_1 : Block
{
    public Block_JumpPad_1() : base("JumpPad", "Prefab_Block_T_5")
    {
        // Set specific properties after base initialization
        if (blockMatrix != null)
        {
            blockMatrix[1, 0].isSolid = true;  // Exemple spécifique - center bottom cell is solid
        }
    }

    public override void Trigger(Vector2Int position, GameObject player)
    {
        base.Trigger(position, player);
        Debug.Log("JumpPad activated!");
        
        // Jump pad logic - add force to player
        PlayerJump playerJump = player.GetComponent<PlayerJump>();
        if (playerJump != null)
        {
            // Trigger a jump effect
            //playerJump.Jump();
        }
    }
}
