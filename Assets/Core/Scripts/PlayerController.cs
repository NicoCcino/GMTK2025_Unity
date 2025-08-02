using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Grid Settings")]
    [Tooltip("Reference to the LevelGridManager")]
    public LevelGridManager levelGridManager;

    [Tooltip("Color for the cell when mouse is over it")]
    public Color cellColor = Color.blue;

    [Tooltip("Height offset above player for block placement")]
    public int initialBlocHeight = 5;

    [Tooltip("Speed at which the block falls in cells per second")]
    public float blockFallSpeed = 1f;

    [Tooltip("Speed at which the block falls when clicking in cells per second")]
    public float blockClickFallSpeed = 0.01f;

    private Camera mainCamera;
    private Vector2Int lastMouseGridPosition;
    private Mouse mouse;
    private float fallTimer;
    private int currentBlockHeight;
    private float blockSpeed;
    private GameObject currentPreviewBlock;
    
    // Collision state variables shared between functions
    private bool isBlockedLeft;
    private bool isBlockedRight;
    private bool shouldSnapBlock;
    private Vector2Int snapMouseGridPos;
    private Vector2Int snapPlayerPivotGridPos;

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

        // Initialize available blocks
        InitializeBlocks();
        
        // Select a random block to start with
        SelectRandomBlock();

        // Initialize last mouse grid position
        lastMouseGridPosition = new Vector2Int(0, currentBlockHeight);

        // Initialize block falling system
        currentBlockHeight = initialBlocHeight;
        fallTimer = 0f;
        blockSpeed = blockFallSpeed;


    }

    void Update()
    {
        CollisionUpdate();
        HandleMouseInput();
        UpdateBlockFalling();
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

        // Apply collision constraints based on shared collision state
        if (isBlockedRight && mouseGridPos.x > lastMouseGridPosition.x)
        {
            // If there's a block on the right, prevent moving past it
            mouseGridPos = new Vector2Int(lastMouseGridPosition.x, mouseGridPos.y);
        }
        
        if (isBlockedLeft && mouseGridPos.x < lastMouseGridPosition.x)
        {
            // If there's a block on the left, prevent moving past it
            mouseGridPos = new Vector2Int(lastMouseGridPosition.x, mouseGridPos.y);
        }


        Vector2Int PlayerPivotGridPos = levelGridManager.WorldToGrid(levelGridManager.player.transform.position);
        mouseGridPos = new Vector2Int(mouseGridPos.x, PlayerPivotGridPos.y + currentBlockHeight);
        if (mouseGridPos.x < 0)
        {
            mouseGridPos = new Vector2Int(0, mouseGridPos.y);
        }
        if (mouseGridPos.y < 0)
        {
            mouseGridPos = new Vector2Int(mouseGridPos.x, 0);
        }

        // Only update if the grid position has changed
        if (mouseGridPos != lastMouseGridPosition)
        {
            // Destroy the previous preview block if it exists
            if (currentPreviewBlock != null)
            {
                Destroy(currentPreviewBlock);
                currentPreviewBlock = null;
            }

            // Create a new preview block at the new mouse position (visual only, no grid registration)
            if (currentBlock != null && currentBlock.blockPrefab != null)
            {
                currentPreviewBlock = levelGridManager.DrawBlock(mouseGridPos.x, mouseGridPos.y, currentBlock.blockPrefab);
            }
            else if (currentBlock != null)
            {
                Debug.LogWarning($"Block {currentBlock.blockName} has null prefab!");
            }

            // Update last position
            lastMouseGridPosition = mouseGridPos;
        }

        // Handle left mouse click and release
        if (mouse.leftButton.wasPressedThisFrame)
        {
            blockSpeed = blockClickFallSpeed;
        }

        if (mouse.leftButton.wasReleasedThisFrame)
        {
            blockSpeed = blockFallSpeed;
        }
    }

    void CollisionUpdate()
    {
        if (levelGridManager == null) return;
        
        // Update collision state based on current position
        isBlockedLeft = CellBlockedLeftOf(lastMouseGridPosition.x, lastMouseGridPosition.y);
        isBlockedRight = CellBlockedRightOf(lastMouseGridPosition.x, lastMouseGridPosition.y);
        
        // Check for ground collision (block hitting bottom or another block below)
        Vector2Int PlayerPivotGridPos = levelGridManager.WorldToGrid(levelGridManager.player.transform.position);
        Vector2Int currentBlockPos = new Vector2Int(lastMouseGridPosition.x, PlayerPivotGridPos.y + currentBlockHeight);
        
        // Store positions for potential snapping
        snapMouseGridPos = lastMouseGridPosition;
        snapPlayerPivotGridPos = PlayerPivotGridPos;
        
        // Check if block should snap to ground
        if((PlayerPivotGridPos.y + currentBlockHeight <= 0) || 
        (currentBlockPos.y - 1 >= 0 && LevelGrid.grid[currentBlockPos.x, currentBlockPos.y - 1] != null))
        {
            SnapBlock(snapMouseGridPos, snapPlayerPivotGridPos);
            return;
        }
    }

    public bool CellBlockedRightOf(int x, int y)
    {
        // Check for cell collision on the right side (with grid wrapping)
        int xChecked = x + 1;
        if (xChecked > LevelGrid.gridWidth - 1)
        {
            xChecked = 0; // Wrap to the left side of the grid
        }
        if (LevelGrid.grid[xChecked, y] != null)
        {
            return true; // There's a block on the right
        }
        else
        {
            return false; // No block on the right
        }
    }
    public bool CellBlockedLeftOf(int x, int y)
    {
        // Check for block collision on the left side (with grid wrapping)
        int xChecked = lastMouseGridPosition.x - 1;
        if (xChecked < 0)
        {
            xChecked = LevelGrid.gridWidth - 1; // Wrap to the right side of the grid
        }
        if (LevelGrid.grid[xChecked, y] != null)
        {

            return true; // There's a block on the left
        }
        else
        {
            return false;
        }
    }

    // Public method to get the current mouse grid position
    public Vector2Int GetMouseGridPosition()
    {
        return lastMouseGridPosition;
    }

    // Public method to set a block at a specific grid position
    public void SetBlockAtGridPosition(int x, int y, Color color)
    {
        if (levelGridManager != null && currentBlock != null && currentBlock.blockPrefab != null)
        {
            // Use SetBlock instead of SetCell to place the entire block pattern
            levelGridManager.SetBlock(x, y, currentBlock.blockPrefab, currentBlock);
        }
    }

    // Update block falling over time
    private void UpdateBlockFalling()
    {
        fallTimer += Time.deltaTime;

        // Calculate new height based on fall speed
        if (fallTimer >= blockSpeed)
        {
            currentBlockHeight = currentBlockHeight - 1;
            fallTimer = 0f;
        }
    }

    // Reset block height to initial value
    public void SnapBlock(Vector2Int mouseGridPos, Vector2Int PlayerPivotGridPos)
    {
        // we reached the bottom of the grid, keep the bloc in position and reset the block height
        if (currentBlock != null && currentBlock.blockPrefab != null)
        {
            // Use SetBlock to place the entire block pattern on the grid
            levelGridManager.SetBlock(mouseGridPos.x, PlayerPivotGridPos.y + currentBlockHeight, currentBlock.blockPrefab, currentBlock);
        }
        else
        {
            Debug.LogWarning("Cannot snap block: currentBlock or its prefab is null!");
            return;
        }
        currentBlockHeight = initialBlocHeight;
        fallTimer = 0f;
        
        // Clean up any existing preview block when starting a new block
        if (currentPreviewBlock != null)
        {
            Destroy(currentPreviewBlock);
            currentPreviewBlock = null;
        }

        // Destroy preview block and create permanent block
        if (currentPreviewBlock != null)
        {
            Destroy(currentPreviewBlock);
            currentPreviewBlock = null;
        }
        lastMouseGridPosition = new Vector2Int(mouseGridPos.x, PlayerPivotGridPos.y + currentBlockHeight);

        // Trigger a cam shake effect when resetting the block height
        CameraShake cameraShake = FindFirstObjectByType<CameraShake>();
        if (blockSpeed == blockClickFallSpeed)
        {
            cameraShake.shakeMagnitude = 0.2f;
            cameraShake.shakeDuration = 0.3f;
        }
        else
        {
            cameraShake.shakeMagnitude = 0.05f;
            cameraShake.shakeDuration = 0.2f;
        }
        cameraShake.StartShake();

        blockSpeed = blockFallSpeed;
    }


    // GESTION DES DIFFERENTS TYPES DE BLOC / SELECTION / RANDOMISATION
    
    public Block[] availableBlocks; // Liste des blocs disponibles
    public Block currentBlock; // Bloc actuellement sélectionné

    private void InitializeBlocks()
    {
        availableBlocks = new Block[]
        {
            new Block_JumpPad_1(),
            new Block_T_5(),
            new Block_Simple_1()
        };
        
        Debug.Log($"Initialized {availableBlocks.Length} blocks");
        foreach (Block block in availableBlocks)
        {
            Debug.Log($"Block initialized: {block.blockName}");
        }
    }

    public void SelectRandomBlock()
    {
        // Select a random block from the array
        if (availableBlocks != null && availableBlocks.Length > 0)
        {
            int randomIndex = Random.Range(0, availableBlocks.Length);
            currentBlock = availableBlocks[randomIndex];
            
            Debug.Log("Selected Block: " + currentBlock.blockName);
        }
        else
        {
            Debug.LogWarning("No block available to select.");
        }
    }

} 
