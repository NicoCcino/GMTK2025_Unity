using UnityEngine;

public class Block
{

    public GameObject gameObject;  // L'objet visuel du bloc
    public Vector2Int gridPosition; // Position du centre (pivot) sur la grille
    public Cell[,] blockSize = new Cell[3, 3]; // Taille du bloc (largeur, hauteur)
    public Color color; // Optionnel : couleur du bloc


    public Block(GameObject obj, Vector2Int pos, Color col)
    {
        gameObject = obj;
        gridPosition = pos;
        color = col;
    }

}
