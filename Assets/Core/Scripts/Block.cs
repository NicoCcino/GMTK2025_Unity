using UnityEngine;

public class Block
{
    public GameObject gameObject;  // L'objet visuel du bloc
    public Vector2Int gridPosition; // Position dans la grille
    public Color color; // Optionnel : couleur du bloc
    public int value; // Valeur du bloc

    public Block(GameObject obj, Vector2Int pos, Color col)
    {
        gameObject = obj;
        gridPosition = pos;
        color = col;
        value = 1; // Valeur par défaut, peut être modifiée plus tard
    }
    
}