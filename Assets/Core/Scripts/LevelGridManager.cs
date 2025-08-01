using UnityEngine;
using UnityEditor;

public class LevelGridManager : MonoBehaviour
{

    public GameObject blockPrefab;
    public GameObject floorPrefab;
    public float floorWidth = 50f;

    // Player reference and grid position
    [Header("Player Settings")]
    [Tooltip("Reference to the player GameObject")]

    public GameObject player;
    public Vector2Int playerGridPosition;

    // Gizmo settings for visualizing the grid in the editor
    [Header("Gizmo Settings")]
    [Tooltip("Color of the grid gizmos")]
    public Color gizmoColor = Color.gray;
    public float cellSize = 1f;
    [Tooltip("Origin point for the grid in the world")]
    public Transform gridOrigin;

    [Header("Infinite Ground Settings")]
    [Tooltip("Reference to the player GameObject")]

    public float minDistanceToTeleportChunk = 50f;

    public void SetCell(int x, int y, Color color)
    {
        // Check if the coordinates are within bounds
        if (!LevelGrid.InBounds(x, y)) return;

        if (LevelGrid.grid[x, y] != null)
        {
            Destroy(LevelGrid.grid[x, y].gameObject);
        }

        // Placement du bloc en 3D dans le monde
        Vector3 worldPos = GridToWorld(x, y);
        worldPos += new Vector3(cellSize, cellSize, 0) * 0.5f; // Center the block in the cell
        GameObject newBlockGO = Instantiate(blockPrefab, worldPos, Quaternion.identity, this.transform);
        Block newBlock = new Block(newBlockGO, new Vector2Int(x, y), color);


        // Gestion du parentage du bloc au sol
        foreach (GameObject floor in GameObject.FindGameObjectsWithTag("Floor"))
        {
            if (worldPos.x >= floor.transform.position.x && worldPos.x < floor.transform.position.x + floorWidth)
            { // Si le bloc est sur une position World X entre la position World X de début du floor et celle de fin
                newBlock.gameObject.transform.SetParent(floor.transform); // On attache le bloc au sol qui est en dessous - il se déplacera ainsi avec le sol
                break;
            }
        }

        // Enregistrement du bloc dans la grille
        LevelGrid.grid[x, y] = newBlock;
    }

    public void ClearCell(int x, int y)
    {
        if (!LevelGrid.InBounds(x, y)) return;

        if (LevelGrid.grid[x, y] != null)
        {
            Destroy(LevelGrid.grid[x, y].gameObject);
            LevelGrid.grid[x, y] = null;
        }
    }

    public Vector3 GridToWorld(int x, int y)
    {
        Vector3 worldPosition = new Vector3(x + gridOrigin.position.x, y, 0);         // On ajoute l'offset de la grille
        // Debug.Log($"Grid to World: ({x}, {y}) -> {worldPosition}");
        return worldPosition;

    }

    public Vector2Int WorldToGrid(Vector3 position)
    {

        Vector2Int gridPosition = new Vector2Int(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.y));
        // On force les coordonnées X à être dans l'intervalle de la grille (par exemple entre 0 et 100)
        while (gridPosition.x > LevelGrid.gridWidth -1)
        {
            gridPosition.x -= LevelGrid.gridWidth;
        }
        // Debug.Log($"World to Grid: {position} -> ({gridPosition.x}, {gridPosition.y})");
        return gridPosition;
    }


    // Affichage de la grille en éditeur

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;

        for (int x = 0; x < LevelGrid.gridWidth; x++)
        {
            for (int y = 0; y < LevelGrid.gridHeight ; y++)
            {
                Vector3 pos = GridToWorld(x, y) + new Vector3(cellSize, cellSize, 0) * 0.5f;
                Gizmos.DrawWireCube(pos, new Vector3(cellSize, cellSize, 0.1f));


#if UNITY_EDITOR
                // Affiche 0 ou 1 selon présence d'un bloc
                int cellValue = LevelGrid.grid[x, y] != null ? 1 : 0;
                Handles.Label(pos, cellValue.ToString());
#endif

            }
        }
    }



    private void MoveChunkToFront()
    {
        foreach (GameObject floor in GameObject.FindGameObjectsWithTag("Floor"))
        {
            float distanceToPlayer = player.transform.position.x - floor.transform.position.x;

            // Si le sol est trop loin derrière le joueur
            if (distanceToPlayer > minDistanceToTeleportChunk)
            {
                // Trouve la position du sol le plus à droite
                float maxX = GetMaxFloorXExcept(floor);

                // Le téléporte juste après
                floor.transform.position = new Vector3(maxX + floorWidth, floor.transform.position.y, floor.transform.position.z);
            }
        }
    }

    public Vector2Int GetPlayerGridPosition()
    {
        Vector3 pos = player.transform.position;
        playerGridPosition = WorldToGrid(pos);
        Debug.Log($"Player Grid Position: {playerGridPosition}");
        return playerGridPosition;
    }

    float GetMaxFloorXExcept(GameObject exclude)
    {
        float maxX = -1000f; // Valeur initiale très basse

        foreach (GameObject floor in GameObject.FindGameObjectsWithTag("Floor"))
        {
            if (floor != exclude)
            {
                maxX = Mathf.Max(maxX, floor.transform.position.x);
            }
        }

        return maxX;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("PlayerPivot");
        SetCell(10, 0, Color.red);
        SetCell(52, 1, Color.red);
        SetCell(52, 0, Color.red);

        Debug.Log($"World to Grid: (1,1,0) -> {WorldToGrid(new Vector3(1, 1, 0))}");
        Debug.Log($"World to Grid: (-1,1,0) -> {WorldToGrid(new Vector3(-1, 1, 0))}");
        Debug.Log($"World to Grid: (50,50,0) -> {WorldToGrid(new Vector3(50, 50, 0))}");
        Debug.Log($"World to Grid: (99,1,0) -> {WorldToGrid(new Vector3(99, 1, 0))}");
        Debug.Log($"World to Grid: (150,1,0) -> {WorldToGrid(new Vector3(150, 1, 0))}");


    }

    // Update is called once per frame
    void Update()
    {
        playerGridPosition = WorldToGrid(player.transform.position);
        MoveChunkToFront();
    }
}
