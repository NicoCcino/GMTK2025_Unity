using UnityEngine;
using UnityEditor;

public class LevelGridManager : MonoBehaviour
{


    [Header("Blocks Settings")]

    public float blockHeightOffset = -1;

    public GameObject floorPrefab;
    public float floorWidth = 50f;

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

    public float minDistanceToTeleportChunk = 50f;

    public void BlockHit(int gridHitX, int gridHitY, BlockData blockData, int blockHitX, int blockHitY, int blockRotation = 0)
    {
        // gridHit représente les coordonnées de la grille où le bloc a été touché

        // blockHit représente les coordonnées de la cellule dans le bloc (qui a touché)
        // Par exemple pour un bloc T:
        //

        // On peut ajouter ici la logique de ce qui se passe quand le bloc est touché
        Debug.Log($"Block hit ground at ({gridHitX}, {gridHitY}) with block data: {blockData.blockName}");

        // Placement de l'objet 3D

        // Placement du centre du bloc
        int pivotX = gridHitX - blockHitX;
        int pivotY = gridHitY - blockHitY;

        GameObject newBlockGO = DrawBlock(gridHitX, gridHitY, blockData.blockPrefab, blockRotation);


        // Update des cellules dans la grille


        for (int i = 0; i < blockData.shape.Length; i++)
        {
            bool cellIsSolid = blockData.shape[i];
            if (!cellIsSolid) continue;

            int localX = i % 3;     // Colonne (0 à 2)
            int localY = i / 3;     // Ligne   (0 à 2)

            int rotX = localX;
            int rotY = localY;

            // Appliquer la rotation
            switch (blockRotation % 360)
            {
                case 90:
                    rotX = 2 - localY;
                    rotY = localX;
                    break;
                case 180:
                    rotX = 2 - localX;
                    rotY = 2 - localY;
                    break;
                case 270:
                    rotX = localY;
                    rotY = 2 - localX;
                    break;
                    // 0 ou valeur par défaut : aucune rotation
            }

            int gridX = pivotX + rotX;
            int gridY = pivotY + rotY;

            // Mise à jour de la grille de cellules:
            LevelGrid.grid[gridX, gridY] = new Cell(newBlockGO, new Vector2Int(gridX, gridY), blockData.color);
            Debug.Log($"→ Cellule placée en ({gridX}, {gridY})");
        }
    }

    public GameObject DrawBlock(int x, int y, GameObject blockPrefab, int blockRotation = 0)
    {
        // Placement du bloc en 3D dans le monde
        Vector3 worldPos = GridToWorld(x, y);
        worldPos += new Vector3(cellSize, cellSize + blockHeightOffset, 0) * 0.5f; // Center the block in the cell


        // Applique une rotation selon Z (Unity fonctionne avec Z vers l'écran en 2D vue de dessus)
        Quaternion rotation = Quaternion.Euler(0f, 0f, blockRotation);

        // Place le bloc
        GameObject newBlockGO = Instantiate(blockPrefab, worldPos, rotation, this.transform);

        // Gestion du parentage du bloc au sol
        foreach (GameObject floor in GameObject.FindGameObjectsWithTag("Floor"))
        {
            if (worldPos.x >= floor.transform.position.x && worldPos.x < floor.transform.position.x + floorWidth)
            { // Si le bloc est sur une position World X entre la position World X de début du floor et celle de fin
                newBlockGO.transform.SetParent(floor.transform); // On attache le bloc au sol qui est en dessous - il se déplacera ainsi avec le sol
                break;
            }
        }

        return newBlockGO; // Donne l'instance du bloc 3D crée en sortie
    }

    public void MoveDrawBlock(int currentPosX, int currentPosY, int newPosX, int newPosY)
    {
        //Récupère le bloc à déplacer
        GameObject blockPrefab = LevelGrid.grid[currentPosX, currentPosY].gameObject;
        // Déplace le bloc sur sa nouvelle position
        blockPrefab.transform.position = GridToWorld(newPosX, newPosY) + new Vector3(cellSize, cellSize + blockHeightOffset, 0) * 0.5f; // Center the block in the cell


        // Revérifie le parentage si on a déplacé le bloc sur le côté
        if (newPosX != currentPosX)
        {
            foreach (GameObject floor in GameObject.FindGameObjectsWithTag("Floor"))
            {
                if (blockPrefab.transform.position.x >= floor.transform.position.x && blockPrefab.transform.position.x < floor.transform.position.x + floorWidth)
                { // Si le bloc est sur une position World X entre la position World X de début du floor et celle de fin
                    blockPrefab.transform.SetParent(floor.transform); // On attache le bloc au sol qui est en dessous - il se déplacera ainsi avec le sol
                    break;
                }
            }
        }
    }

    public void SetCell(int x, int y, GameObject blockPrefab, bool isSolid, Color color)
    {
        // Check if the coordinates are within bounds
        if (!LevelGrid.InBounds(x, y)) return;

        // If there's already a drawn block at this position, destroy it
        if (LevelGrid.grid[x, y] != null)
        {
            Destroy(LevelGrid.grid[x, y].gameObject);
        }

        // GameObject newBlockGO = DrawBlock(x, y, currentBlockData.blockPrefab);

        // Gestion des cellules dans grille
        Cell newCell = new Cell(blockPrefab, new Vector2Int(x, y), color);

        // Enregistrement du bloc dans la grille
        newCell.isSolid = isSolid; // On marque la cellule comme solide ou non
        LevelGrid.grid[x, y] = newCell;
    }

    public bool IsCellSolid(int x, int y) // is there a solid cell at this position?
    {
        if (!LevelGrid.InBounds(x, y)) return false; // Out of bounds, no solid cell
        if (LevelGrid.grid[x, y] == null) return false; // No cell at this position
        return LevelGrid.grid[x, y].isSolid; // Return if the cell is solid
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
        Debug.Log("Player entered a new column at: " + playerGridPosition);
        // Ici, vous pouvez ajouter des actions spécifiques à effectuer lorsque le joueur entre dans une nouvelle colonne


        currentPlayerColumn = newCol;
        int blocksInColumn = CountValueInColumn(newCol);
        moneyManager.AddMoney(blocksInColumn);
        Debug.Log($"Nouvelle colonne {newCol} : +{blocksInColumn} argent. Total: {moneyManager.money}");
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
                    Destroy(LevelGrid.grid[x, y].gameObject);
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
        //SetCell(10, 0, Color.red);
        //SetCell(52, 1, Color.red);
        //SetCell(52, 0, Color.red);

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
