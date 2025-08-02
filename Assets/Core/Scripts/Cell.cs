using UnityEngine;

public class Cell
{
    public GameObject blockGO;    // L'objet visuel du bloc
    public Block block;           // Référence au bloc
    public Vector2Int positionInBlockMatrix; // Position dans la matrice du bloc
    public bool isSolid;          // Indique si la cellule est solide ou non
    public int value;             // Valeur du bloc

    public Cell(GameObject blockGO, Block block, Vector2Int posInBlockMatrix)
    {
        this.blockGO = blockGO;           // Assignation correcte du GameObject
        this.value = 1;                   // Valeur par défaut, modifiable après
        this.positionInBlockMatrix = posInBlockMatrix; // Position dans la matrice du bloc
        this.block = block;               // Référence au bloc
    }
    
}
