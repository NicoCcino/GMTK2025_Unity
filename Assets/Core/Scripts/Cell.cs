using UnityEngine;

public class Cell
{
    public GameObject blockGO;    // L'objet visuel du bloc
    public BlockData blockData;   // Référence aux données du bloc
    public Vector2Int positionInBlockDataMatrix; // Position dans la matrice de données du bloc
    public bool isSolid;          // Indique si la cellule est solide ou non
    public int value;             // Valeur du bloc

    public Cell(GameObject blockGO, BlockData blockData, Vector2Int posInBlockDataMatrix)
    {
        this.blockGO = blockGO;           // Assignation correcte du GameObject
        this.value = 1;                   // Valeur par défaut, modifiable après
        this.positionInBlockDataMatrix = posInBlockDataMatrix; // Position dans la matrice de données du bloc
        this.isSolid = blockData.blockMatrix[posInBlockDataMatrix.x, posInBlockDataMatrix.y].isSolid; // Détermine si la cellule est solide
        this.blockData = blockData; // Référence aux données du bloc
    }
}