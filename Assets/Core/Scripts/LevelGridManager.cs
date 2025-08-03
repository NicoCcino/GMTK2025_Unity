using UnityEngine;
using UnityEditor;

public class LevelGridManager : MonoBehaviour
{


    [Header("Blocks Settings")]

    public float drawBlockHeightOffset = 0.5f;
    public GameObject simpleBlockPrefab;
    
    [Header("Block Type Prefabs")]
    public GameObject standardBlockPrefab;
    public GameObject jumpPadBlockPrefab;
    public GameObject invinciblePadBlockPrefab;

    // Player reference and grid position
    [Header("Player Settings")]
    [Tooltip("Reference to the player GameObject")]

    public GameObject player;
    public Vector2Int playerGridPosition;
    static int currentPlayerColumn = -1; // Variable pour suivre la colonne actuelle du joueur
    private MoneyManager moneyManager;

    // Gizmo settings for visualizing the grid in the editor
    [Header("Gizmo Settings")]
    [Tooltip("Color of the grid gizmos")]
    public Color gizmoColor = Color.gray;
    public float cellSize = 1f;
    [Tooltip("Origin point for the grid in the world")]
    public Transform gridOrigin;
    public Vector3 gridOriginBackup;

    [Header("Infinite Ground Settings")]
    [Tooltip("Reference to the player GameObject")]

    public GameObject floorPrefab;
    public float floorWidth = 50f;
    public float minDistanceToTeleportChunk = 50f;


    public GameObject DrawBlock(int x, int y, Block block)
    {
        // Create parent GameObject and set its position
        GameObject newBlockGO = new GameObject($"Block_{block.blockName}");
        Vector3 parentWorldPos = GridToWorld(x, y);
        newBlockGO.transform.position = parentWorldPos;
        Debug.Log($"Created parent block '{newBlockGO.name}' at position {parentWorldPos} for grid ({x}, {y})");

        int gridWidth = LevelGrid.grid.GetLength(0);
        //Debug.Log("gridWidth: " + gridWidth);
        int gridHeight = LevelGrid.grid.GetLength(1);
        for (int i = 0; i < 3; i++) // Pour chaque cellule de la matrice du bloc
        {
            for (int j = 0; j < 3; j++)
            {
                int worldGridPosX = x + i - 1;
                int worldGridPosY = y + j -1;
                Debug.Log($"Trying to set cell at ({i}, {j}) in matrix of {block.blockName} at position ({worldGridPosX}, {worldGridPosY})");

                // Vérifie si les coordonnées sont dans les limites de la grille
                if (worldGridPosX >= 0 && worldGridPosX < gridWidth &&
                    worldGridPosY >= 0 && worldGridPosY < gridHeight)
                {
                    Vector3 cellWorldPos = GridToWorld(worldGridPosX, worldGridPosY);
                    cellWorldPos += new Vector3(0.5f, drawBlockHeightOffset, 0f); // Offset
                    Cell cell = block.blockMatrix[i, j];
                    
                    // Skip if cell is null
                    if (cell == null)
                    {
                        Debug.Log($"Skipping null cell at matrix position ({i}, {j})");
                        continue;
                    }
                    
                    // Debug cell properties
                    Debug.Log($"Cell ({i}, {j}): isSolid={cell.isSolid}, blockType={cell.blockType}");
                    
                    GameObject childBlockGO = null;
                    
                    // For each cell, we create a GameObject at the cell position depending on the type of block
                    // None block don't create a GameObject
                    // Standard block is a Prefab_Block_Simple_1
                    // JumpPad block is a Prefab_JumpPad_1
                    // InvinciblePad block is a Prefab_InvinciblePad_1
                    // Each of those block are child of the GameObject newBlockGO
                    switch (cell.blockType)
                    {
                        case BlockType.NoBlock:
                            // None block: don't create a GameObject
                            Debug.Log($"Cell ({i}, {j}) is NoBlock, skipping");
                            continue;
                            
                        case BlockType.Standard:
                            // Standard block: use standardBlockPrefab
                            Debug.Log($"Cell ({i}, {j}) is Standard block, standardBlockPrefab null? {standardBlockPrefab == null}");
                            if (standardBlockPrefab != null)
                                childBlockGO = Instantiate(standardBlockPrefab, cellWorldPos, Quaternion.identity);
                            else
                                Debug.LogError("standardBlockPrefab is NULL! Assign it in the Inspector.");
                            break;
                            
                        case BlockType.JumpPad:
                            // JumpPad block: use jumpPadBlockPrefab
                            Debug.Log($"Cell ({i}, {j}) is JumpPad block, jumpPadBlockPrefab null? {jumpPadBlockPrefab == null}");
                            if (jumpPadBlockPrefab != null)
                                childBlockGO = Instantiate(jumpPadBlockPrefab, cellWorldPos, Quaternion.identity);
                            else
                                Debug.LogError("jumpPadBlockPrefab is NULL! Assign it in the Inspector.");
                            break;
                            
                        case BlockType.InvinciblePad:
                            // InvinciblePad block: use invinciblePadBlockPrefab
                            Debug.Log($"Cell ({i}, {j}) is InvinciblePad block, invinciblePadBlockPrefab null? {invinciblePadBlockPrefab == null}");
                            if (invinciblePadBlockPrefab != null)
                                childBlockGO = Instantiate(invinciblePadBlockPrefab, cellWorldPos, Quaternion.identity);
                            else
                                Debug.LogError("invinciblePadBlockPrefab is NULL! Assign it in the Inspector.");
                            break;
                            
                        default:
                            Debug.LogWarning($"Unknown block type: {cell.blockType} at cell ({i}, {j})");
                            break;
                    }
                    
                    // Set the created GameObject as child of the parent block
                    if (childBlockGO != null)
                    {
                        childBlockGO.transform.SetParent(newBlockGO.transform, true); // Keep world position
                        Debug.Log($"Created {cell.blockType} block at world pos {cellWorldPos} (grid: {worldGridPosX}, {worldGridPosY})");
                    }
                }
            }
        }

        // Check if any children were created
        if (newBlockGO.transform.childCount == 0)
        {
            Debug.LogWarning($"No child blocks were created for {block.blockName}! Check block matrix and blockType assignments.");
        }
        else
        {
            Debug.Log($"Successfully created {newBlockGO.transform.childCount} child blocks for {block.blockName}");
        }

        // Gestion du parentage du bloc au sol
        foreach (GameObject floor in GameObject.FindGameObjectsWithTag("Floor"))
        {
            if (parentWorldPos.x >= floor.transform.position.x && parentWorldPos.x < floor.transform.position.x + floorWidth)
            { // Si le bloc est sur une position World X entre la position World X de début du floor et celle de fin
                newBlockGO.transform.SetParent(floor.transform); // On attache le bloc au sol qui est en dessous - il se déplacera ainsi avec le sol
                break;
            }
        }

        return newBlockGO; // Donne l'instance du bloc 3D crée en sortie
    }

    public void SetBlock(int x, int y, GameObject blockPrefab, Block block)
    {
        int gridWidth = LevelGrid.grid.GetLength(0);
        //Debug.Log("gridWidth: " + gridWidth);
        int gridHeight = LevelGrid.grid.GetLength(1);
        //Debug.Log("gridHeight: " + gridHeight);

        // Create an actual instance of the block at this position
        GameObject blockInstance = DrawBlock(x, y, block);

        for (int i = 0; i < 3; i++) // Pour chaque cellule de la matrice du bloc
        {
            for (int j = 0; j < 3; j++)
            {
                int worldGridPosX = x + i - 1;
                int worldGridPosY = y + j -1;
                Debug.Log($"Trying to set cell at ({i}, {j}) in matrix of {block.blockName} at position ({worldGridPosX}, {worldGridPosY})");

                // Vérifie si les coordonnées sont dans les limites de la grille
                if (worldGridPosX >= 0 && worldGridPosX < gridWidth &&
                    worldGridPosY >= 0 && worldGridPosY < gridHeight)
                {
                    Cell cell = block.blockMatrix[i, j];
                    if ((cell != null && cell.isSolid) || (cell != null && cell.blockType != BlockType.NoBlock)) // Si la cellule est solide
                    {
                        // Si la case de grille est vide ou contient une cellule non solide
                        if (LevelGrid.grid[worldGridPosX, worldGridPosY] == null || !LevelGrid.grid[worldGridPosX, worldGridPosY].isSolid)
                        {
                                    // If there's already a cell at this position, destroy it
                            if (LevelGrid.grid[worldGridPosX, worldGridPosY] != null && LevelGrid.grid[worldGridPosX, worldGridPosY].blockGO != null)
                            {
                                // Only destroy if it's an instantiated GameObject (has a scene), not a prefab asset
                                if (LevelGrid.grid[worldGridPosX, worldGridPosY].blockGO.scene.IsValid())
                                {
                                    Destroy(LevelGrid.grid[worldGridPosX, worldGridPosY].blockGO);
                                }
                            }
                            // On place la cellule dans la grille en définissant les coordonnées à partir du centre x,y
                            LevelGrid.grid[worldGridPosX, worldGridPosY] = cell;
                            Debug.Log($"Cell at ({worldGridPosX}, {worldGridPosY}) set with cell {cell} which is isSolid: {cell.isSolid}");
                        }

                    }
                }
                else
                {
                    Debug.LogWarning($"Tentative de placement hors grille ignorée : ({worldGridPosX}, {worldGridPosY})");
                }

            }
        }
    }

    public bool IsCellSolid(int x, int y) // is there a solid cell at this position?
    {
        if (!LevelGrid.InBounds(x, y)) return false; // Out of bounds, no solid cell
        if (LevelGrid.grid[x, y] == null) return false; // No cell at this position
        return LevelGrid.grid[x, y].isSolid; // Return if the cell is solid
    }

    public bool IsCellABlock(int x, int y) // is there a solid cell at this position?
    {
        if (!LevelGrid.InBounds(x, y)) return false; // Out of bounds, no solid cell
        if (LevelGrid.grid[x, y] == null) return false; // No cell at this position
        return LevelGrid.grid[x, y].blockType != BlockType.NoBlock; // Return if the cell is solid
    }

    public BlockType GetBlockType(int x, int y) // get the block type of the cell
    {
        if (!LevelGrid.InBounds(x, y)) return BlockType.NoBlock; // Out of bounds, no block
        if (LevelGrid.grid[x, y] == null) return BlockType.NoBlock; // No cell at this position
        return LevelGrid.grid[x, y].blockType; // Return the block type
    }

    public void ClearCell(int x, int y)
    {
        if (!LevelGrid.InBounds(x, y)) return;

        if (LevelGrid.grid[x, y] != null)
        {
            // Only destroy if it's an instantiated GameObject (has a scene), not a prefab asset
            if (LevelGrid.grid[x, y].blockGO != null && LevelGrid.grid[x, y].blockGO.scene.IsValid())
            {
                Destroy(LevelGrid.grid[x, y].blockGO); // Destroy the GameObject if it exists
            }
            LevelGrid.grid[x, y] = null;
        }
    }

    public Vector3 GridToWorld(int x, int y)
    {
        // On envoie chier si les coordonnées sont hors de la grille
        if (x > LevelGrid.gridWidth - 1 || y > LevelGrid.gridHeight - 1)
        {
            Debug.LogError($"GridToWorld: Coordinates ({x}, {y}) are out of bounds for the grid size ({LevelGrid.gridWidth}, {LevelGrid.gridHeight})");
        }
        if (x < 0 || y < 0)
        {
            Debug.LogError($"GridToWorld: Coordinates ({x}, {y}) cannot be negative.");
        }

        // Si la case est en dessous de 50, j'utilise l'origine de la grille
        if (x < 50)
        {
            return new Vector3(x, y, 0) + gridOrigin.position;
        }
        else
        {
            return new Vector3(x, y, 0) + gridOriginBackup;
        }
    }

    public Vector2Int WorldToGrid(Vector3 position)
    {

        Vector2Int gridPosition = new Vector2Int(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.y));
        // On force les coordonnées X à être dans l'intervalle de la grille (par exemple entre 0 et 100)
        while (gridPosition.x > LevelGrid.gridWidth - 1)
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
            for (int y = 0; y < LevelGrid.gridHeight; y++)
            {
                Vector3 pos = GridToWorld(x, y) + new Vector3(cellSize, cellSize, 0) * 0.5f;
                Gizmos.DrawWireCube(pos, new Vector3(cellSize, cellSize, 0.1f));


#if UNITY_EDITOR
                int cellValue = 0;
                // Affiche 0 ou 1 selon présence d'un bloc
                if (LevelGrid.grid[x, y] != null)
                {
                    cellValue = (int)LevelGrid.grid[x, y].blockType; // Affiche 1 si la cellule est solide
                }
                // Affiche la valeur de la cellule dans l'éditeur
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

                // Met à jour le backup quand le floor 2 qui ne contient pas la grille est déplacé après le floor 1 qui contient la grille
                if (floor.name == "FloorPivot2")
                {
                    gridOriginBackup = gridOrigin.position;
                }
            }

        }
    }

    public Vector2Int GetPlayerGridPosition()
    {
        Vector3 pos = player.transform.position;
        playerGridPosition = WorldToGrid(pos);
        // Debug.Log($"Player Grid Position: {playerGridPosition}");
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

    int CountValueInColumn(int column)
    {
        int count = 0;
        for (int y = 0; y < LevelGrid.gridHeight; y++)
        {
            Cell block = LevelGrid.grid[column, y];
            if (block != null)
            {
                count += block.value; // On additionne la valeur du bloc
            }
        }
        return count;
    }

    void PlayerEntersNewColumn(int newCol)
    {
        // Logique pour gérer l'entrée du joueur dans une nouvelle colonne
        //Debug.Log("Player entered a new column at: " + playerGridPosition);
        // Ici, vous pouvez ajouter des actions spécifiques à effectuer lorsque le joueur entre dans une nouvelle colonne


        currentPlayerColumn = newCol;
        int blocksInColumn = CountValueInColumn(newCol);
        moneyManager.AddMoney(blocksInColumn);
        //Debug.Log($"Nouvelle colonne {newCol} : +{blocksInColumn} argent. Total: {moneyManager.money}");
    }

    void EmptyGrid()
    {
        // Vide la grille de tous les blocs 
        for (int x = 0; x < LevelGrid.gridWidth; x++)
        {
            for (int y = 0; y < LevelGrid.gridHeight; y++)
            {
                if (LevelGrid.grid[x, y] != null)
                {
                    // Only destroy if it's an instantiated GameObject (has a scene), not a prefab asset
                    if (LevelGrid.grid[x, y].blockGO != null && LevelGrid.grid[x, y].blockGO.scene.IsValid())
                    {
                        Destroy(LevelGrid.grid[x, y].blockGO); // On détruit le GameObject du bloc
                    }
                    LevelGrid.grid[x, y] = null; // On vide la cellule de la grille
                }
            }
        }
        Debug.Log("Grid emptied.");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EmptyGrid();
        player = GameObject.Find("PlayerPivot");
        moneyManager = GameObject.Find("MoneyManager").GetComponent<MoneyManager>();

        //Debug.Log($"World to Grid: (1,1,0) -> {WorldToGrid(new Vector3(1, 1, 0))}");
        //Debug.Log($"World to Grid: (-1,1,0) -> {WorldToGrid(new Vector3(-1, 1, 0))}");
        //Debug.Log($"World to Grid: (50,50,0) -> {WorldToGrid(new Vector3(50, 50, 0))}");
        //Debug.Log($"World to Grid: (99,1,0) -> {WorldToGrid(new Vector3(99, 1, 0))}");
        //Debug.Log($"World to Grid: (150,1,0) -> {WorldToGrid(new Vector3(150, 1, 0))}");

        gridOriginBackup = gridOrigin.position;

    }

    // Update is called once per frame
    void Update()
    {
        playerGridPosition = WorldToGrid(player.transform.position);
        MoveChunkToFront();


        int newCol = playerGridPosition.x;
        if (newCol != currentPlayerColumn)
        {
            PlayerEntersNewColumn(newCol);
        }
    }
}
