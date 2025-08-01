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
    public Transform origin;

    public void SetCell(int x, int y, Color color)
    {
        if (!LevelGrid.InBounds(x, y)) return;

        if (LevelGrid.grid[x, y] != null)
        {
            Destroy(LevelGrid.grid[x, y].gameObject);
        }


        Vector3 worldPos = GridToWorld(x, y);
        worldPos += new Vector3(cellSize, cellSize, 0) * 0.5f; // Center the block in the cell
        GameObject newBlockGO = Instantiate(blockPrefab, worldPos, Quaternion.identity, this.transform);
        Block newBlock = new Block(newBlockGO, new Vector2Int(x, y), color);


        // Gestion du parentage du bloc au sol
        foreach (GameObject floor in GameObject.FindGameObjectsWithTag("Floor")){
            if (worldPos.x >= floor.transform.position.x && worldPos.x < floor.transform.position.x + floorWidth)
            { // Si le bloc est sur une position World X entre la position World X de début du floor et celle de fin
                newBlock.gameObject.transform.SetParent(floor.transform); // On attache le bloc au sol qui est en dessous - il se déplacera ainsi avec le sol
                break;
            }
        }
        


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
        return new Vector3(x, y, 0);
    }

    public Vector2Int WorldToGrid(Vector3 position)
    {
        return new Vector2Int(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.y));
    }


// Affichage de la grille en éditeur

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;

        for (int x = 0; x < LevelGrid.gridWidth - 1; x++)
        {
            for (int y = 0; y < LevelGrid.gridHeight - 1; y++)
            {
                Vector3 pos = GridToWorld(x, y) + new Vector3(cellSize, cellSize, 0) * 0.5f;
                // Add offset based on the grid position in the world
                pos += origin.position;
                Gizmos.DrawWireCube(pos, new Vector3(cellSize, cellSize, 0.1f));


#if UNITY_EDITOR
                // Affiche 0 ou 1 selon présence d'un bloc
                int cellValue = LevelGrid.grid[x, y] != null ? 1 : 0;
                Handles.Label(pos, cellValue.ToString());
#endif

            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("PlayerPivot");
        SetCell (1,1, Color.red);
    }

    // Update is called once per frame
    void Update()
    {
        playerGridPosition = WorldToGrid(player.transform.position);
    }
}
