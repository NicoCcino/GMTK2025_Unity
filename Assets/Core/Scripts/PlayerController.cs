using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Grid Settings")]
    [Tooltip("Reference to the LevelGridManager")]
    public LevelGridManager levelGridManager;
    
    [Tooltip("Color for the cell when mouse is over it")]
    public Color cellColor = Color.blue;
    
    private Camera mainCamera;
    private Vector2Int lastMouseGridPosition;
    private Mouse mouse;
    
    void Start()
    {
        // Get the main camera
        mainCamera = Camera.main;
        
        // Get the mouse device from the new Input System
        mouse = Mouse.current;
        
        // Find LevelGridManager if not assigned
        if (levelGridManager == null)
        {
            levelGridManager = FindFirstObjectByType<LevelGridManager>();
        }
        
        // Initialize last mouse grid position
        lastMouseGridPosition = Vector2Int.zero;
    }
    
    void Update()
    {
        HandleMouseInput();
    }
    
    void HandleMouseInput()
    {
        if (levelGridManager == null || mainCamera == null || mouse == null) return;
        
        // Get mouse position in screen coordinates using the new Input System
        Vector2 mouseScreenPos = mouse.position.ReadValue();
        
        // Convert screen position to viewport coordinates (0-1 range)
        Vector3 viewportPos = mainCamera.ScreenToViewportPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, 0));
        
        // Check if mouse is within the game viewport (0-1 range)
        if (viewportPos.x < 0 || viewportPos.x > 1 || viewportPos.y < 0 || viewportPos.y > 1)
        {
            return;
        }
        
        // For camera facing front (orthogonal to Z axis), we need to cast a ray to the Z=0 plane
        Vector3 mouseWorldPos;
        if (mainCamera.orthographic)
        {
            // For orthographic camera facing front, convert screen position to world position on Z=0 plane
            float height = 2f * mainCamera.orthographicSize;
            float width = height * mainCamera.aspect;
            
            // Convert screen position to normalized viewport coordinates (0-1)
            Vector3 viewportPoint = mainCamera.ScreenToViewportPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, 0));
            
            // Convert to world coordinates on Z=0 plane
            float worldX = (viewportPoint.x - 0.5f) * width;
            float worldY = (viewportPoint.y - 0.5f) * height;
            
            mouseWorldPos = new Vector3(worldX, worldY, 0);
        }
        else
        {
            // For perspective camera facing front, cast a ray to the Z=0 plane
            Ray ray = mainCamera.ScreenPointToRay(new Vector3(mouseScreenPos.x, mouseScreenPos.y, 0));
            
            // Calculate intersection with Z=0 plane
            float distance = -ray.origin.z / ray.direction.z;
            mouseWorldPos = ray.origin + ray.direction * distance;
            mouseWorldPos.z = 0;
        }
        
        // Convert world position to grid position using LevelGridManager's WorldToGrid function
        Vector2Int mouseGridPos = levelGridManager.WorldToGrid(mouseWorldPos);

        Vector2Int PlayerPivotGridPos = levelGridManager.WorldToGrid(levelGridManager.player.transform.position);
        mouseGridPos = new Vector2Int(mouseGridPos.x, PlayerPivotGridPos.y + 5);

        // Only update if the grid position has changed
        if (mouseGridPos != lastMouseGridPosition)
        {
            // Clear the previous cell if it exists
            if (lastMouseGridPosition != Vector2Int.zero)
            {
                levelGridManager.ClearCell(lastMouseGridPosition.x, lastMouseGridPosition.y);
            }
            
            // Set the cell at the new mouse position using LevelGridManager's SetCell function
            levelGridManager.SetCell(mouseGridPos.x, mouseGridPos.y, cellColor);
            
            // Update last position
            lastMouseGridPosition = mouseGridPos;
        }
    }
    
    // Public method to get the current mouse grid position
    public Vector2Int GetMouseGridPosition()
    {
        return lastMouseGridPosition;
    }
    
    // Public method to set a cell at a specific grid position
    public void SetCellAtGridPosition(int x, int y, Color color)
    {
        if (levelGridManager != null)
        {
            levelGridManager.SetCell(x, y, color);
        }
    }
} 
