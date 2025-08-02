using UnityEngine;

public class BlockData : MonoBehaviour
{
    public string blockName = "BaseBlock";
    public GameObject blockPrefab;

    public Cell[,] blockMatrix;

    public BlockData()
    {
        blockMatrix = new Cell[3, 3];
        //     InitializeMatrix();
    }

    protected virtual void InitializeMatrix()
    {
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                blockMatrix[x, y] = new Cell(blockPrefab, this, new Vector2Int(x, y));
            }

        }

    }

    // Méthode virtuelle que les classes filles peuvent override
    public virtual void Trigger(Vector2Int position, GameObject player)
    {
        Debug.Log($"Block {blockName} triggered at {position}");
    }
}
