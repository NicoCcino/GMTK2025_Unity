using UnityEngine;

public class Cell
{
    public GameObject gameObject;  // L'objet visuel du bloc
    public Vector2Int gridPosition; // Position dans la grille
    public bool isSolid; // Indique si la cellule est solide ou non
    public Color color; // Optionnel : couleur du bloc
    public int value; // Valeur du bloc

    public Cell(GameObject obj, Vector2Int pos, Color col)
    {
        gameObject = obj;
        gridPosition = pos;
        color = col;
        value = 1; // Valeur par défaut, peut être modifiée plus tard
        isSolid = false; // Par défaut, la cellule n'est pas solide
    }
    
}