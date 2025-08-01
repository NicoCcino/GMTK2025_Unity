using UnityEngine;

public static class LevelGrid
{
    public static int gridHeight = 50; // Hauteur de la grille
    public static int gridWidth = 100;  // Largeur de la grille
    public static Cell[,] grid = new Cell[gridWidth, gridHeight];
    
public static bool InBounds(int x, int y)
    {
        return x >= 0 && x < gridWidth && y >= 0 && y < gridHeight;
    }

}
