using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

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
    private float blockSpeed;
    private GameObject currentPreviewBlock;

    // Collision state variables shared between functions
    private bool isBlockedLeft;
    private bool isBlockedRight;
    private bool shouldSnapBlock;

    // Timer to prevent early collision detection at startup
    private float initializationDelay = 0.1f; // Small delay to prevent startup collision issues
    private float startTime;

    // Cached player pivot position to avoid repeated calculations
    private Vector2Int cachedPlayerPivotGridPos;
    private bool playerPivotCacheValid = false;

    // Cache previous mouse grid position to avoid unnecessary preview updates
    private Vector2Int previousMouseGridPos;

    // Progression manager
    public ProgressionManager progressionManager;

    void InitManagers()
    {   // Init progression manager if not set

        progressionManager = ProgressionManager.Instance;
        if (progressionManager == null)
        {
            Debug.LogWarning("ProgressionManager not found by PlayerController.");
        }
        // Find LevelGridManager if not assigned
        if (levelGridManager == null)
        {
            levelGridManager = FindFirstObjectByType<LevelGridManager>();
        }

        // Find UIManager if not assigned
        if (uiManager == null)
        {
            uiManager = FindFirstObjectByType<UIManager>();
        }

    }

    void Start()
    {

        InitManagers();

        // Get the main camera
        mainCamera = Camera.main;

        // Get the mouse device from the new Input System
        mouse = Mouse.current;



        // Initialize available blocks
        // InitializeBlocks();

        // SInitialize Queue with 3 blocks
        InitializeQueue();

        // Initialize last mouse grid position
        lastMouseGridPosition = new Vector2Int(5, initialBlocHeight);

        // Initialize previous mouse grid position cache
        previousMouseGridPos = lastMouseGridPosition;

        // Initialize block falling system
        fallTimer = 0f;
        blockSpeed = blockFallSpeed;

        // Ensure position is synchronized with initial height
        UpdateBlockPosition();

        // Record start time for initialization delay
        startTime = Time.time;


    }

    void Update()
    {
        // Invalidate player pivot cache at start of frame (player might have moved)
        playerPivotCacheValid = false;

        // Update block falling
        UpdateBlockFalling();
        // Update mouse position to ensure lastMouseGridPosition uses current height
        HandleMouseInput();
        // Run collision detection last with up-to-date positions
        CollisionUpdate();
    }

    void HandleMouseInput()
    {
        if (levelGridManager == null || mainCamera == null || mouse == null) return;

        // Get mouse position in screen coordinates using the new Input System
        Vector2 mouseScreenPos = mouse.position.ReadValue();

        // Always compute mouse grid position (camera moves even if mouse doesn't)
        // Convert screen position to viewport coordinates (0-1 range) - cache for reuse
        Vector3 viewportPos = mainCamera.ScreenToViewportPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, 0));

        // Check if mouse is within the game viewport (0-1 range)
        // if (viewportPos.x < 0 || viewportPos.x > 1 || viewportPos.y < 0 || viewportPos.y > 1)
        // {
        //     return;
        // }

        // For camera facing front (orthogonal to Z axis), we need to cast a ray to the Z=0 plane
        Vector3 mouseWorldPos;
        if (mainCamera.orthographic)
        {
            // For orthographic camera facing front, convert screen position to world position on Z=0 plane
            float height = 2f * mainCamera.orthographicSize;
            float width = height * mainCamera.aspect;

            // Reuse cached viewport coordinates (avoid duplicate ScreenToViewportPoint call)
            Vector3 viewportPoint = viewportPos;

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
        // We only use the x coordinate of the mouse grid position
        mouseGridPos = new Vector2Int(mouseGridPos.x, lastMouseGridPosition.y);

        // Apply collision constraints based on shared collision state
        if (isBlockedRight && mouseGridPos.x > lastMouseGridPosition.x)
        {
            // If there's a block on the right, prevent moving past it
            mouseGridPos = new Vector2Int(lastMouseGridPosition.x, lastMouseGridPosition.y);
        }

        if (isBlockedLeft && mouseGridPos.x < lastMouseGridPosition.x)
        {
            // If there's a block on the left, prevent moving past it
            mouseGridPos = new Vector2Int(lastMouseGridPosition.x, lastMouseGridPosition.y);
        }


        Vector2Int PlayerPivotGridPos = GetPlayerPivotGridPos();
        mouseGridPos = new Vector2Int(mouseGridPos.x, lastMouseGridPosition.y);
        if (mouseGridPos.x < 0)
        {
            mouseGridPos = new Vector2Int(0, lastMouseGridPosition.y);
        }

        // Only update preview if the grid position has changed (optimization)
        bool mouseGridPosChanged = mouseGridPos != previousMouseGridPos;
        previousMouseGridPos = mouseGridPos;

        if (mouseGridPosChanged)
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
                currentPreviewBlock = levelGridManager.DrawBlock(mouseGridPos.x, mouseGridPos.y, currentBlock.blockPrefab, currentBlock.rotation);
            }
            else if (currentBlock != null)
            {
                Debug.LogWarning($"Block {currentBlock.blockName} has null prefab!");
            }
        }

        // Always update lastMouseGridPosition for collision detection (even if preview didn't change)
        lastMouseGridPosition = mouseGridPos;

        // Handle left mouse click and release (always process clicks, regardless of mouse movement)
        if (mouse.leftButton.wasPressedThisFrame)
        {
            blockSpeed = blockClickFallSpeed;
        }

        if (mouse.leftButton.wasReleasedThisFrame)
        {
            blockSpeed = blockFallSpeed;
        }
        if (mouse.rightButton.wasPressedThisFrame)
        {
            RotateBlock();
        }
    }

    void CollisionUpdate()
    {
        if (levelGridManager == null) return;

        // Prevent collision detection for a brief moment after startup to avoid immediate snapping
        if (Time.time - startTime < initializationDelay) return;

        // Ensure position is current before collision detection
        if (levelGridManager.player != null)
        {
            Vector2Int PlayerPivotGridPos = GetPlayerPivotGridPos();
            Vector2Int expectedPosition = new Vector2Int(lastMouseGridPosition.x, lastMouseGridPosition.y);

            // If position is out of sync with current height, update it
            if (lastMouseGridPosition.y != expectedPosition.y)
            {
                lastMouseGridPosition = expectedPosition;
            }
        }
        bool haveSolidCell = false;
        isBlockedLeft = false;
        isBlockedRight = false;

        for (int i = 0; i < 3; i++) // Pour chaque cellule de la matrice du bloc
        {
            for (int j = 0; j < 3; j++)
            {
                // Convert matrix coordinates to world coordinates (center matrix[1,1] on mouse position)
                Vector2Int cellWorldPos = new Vector2Int(lastMouseGridPosition.x + (i - 1), lastMouseGridPosition.y + (j - 1));
                Cell cell = currentBlock.blockMatrix[i, j];
                if (cell != null && cell.isSolid) // Si la cellule est solide
                {
                    haveSolidCell = true;
                    // We have to check for collision from the matrix center
                    if (!isBlockedLeft)
                    {
                        isBlockedLeft = CellBlockedLeftOf(cellWorldPos.x, cellWorldPos.y);
                    }
                    if (!isBlockedRight)
                    {
                        isBlockedRight = CellBlockedRightOf(cellWorldPos.x, cellWorldPos.y);
                    }
                    if (CellSnapBottomOf(cellWorldPos.x, cellWorldPos.y))
                    {
                        return;
                    }
                }
            }
        }

        if (haveSolidCell)
        {
            return;
        }

        // Update collision state based on current position
        isBlockedLeft = CellBlockedLeftOf(lastMouseGridPosition.x, lastMouseGridPosition.y);
        isBlockedRight = CellBlockedRightOf(lastMouseGridPosition.x, lastMouseGridPosition.y);
        if (CellSnapBottomOf(lastMouseGridPosition.x, lastMouseGridPosition.y))
        {
            return;
        }
    }

    public bool CellBlockedRightOf(int x, int y)
    {
        // Bounds check for Y coordinate
        if (y < 0 || y >= LevelGrid.gridHeight)
        {
            return false; // Out of bounds, no collision
        }

        // Check for cell collision on the right side (with grid wrapping)
        int xChecked = x + 1;

        // Handle grid wrapping for X
        if (xChecked >= LevelGrid.gridWidth)
        {
            xChecked = 0; // Wrap to the left side of the grid
        }

        // Bounds check for wrapped X coordinate
        if (xChecked < 0 || xChecked >= LevelGrid.gridWidth)
        {
            return false; // Safety check
        }



        if (levelGridManager.IsCellSolid(xChecked, y))
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
        // Bounds check for Y coordinate
        if (y < 0 || y >= LevelGrid.gridHeight)
        {
            return false; // Out of bounds, no collision
        }

        // Check for block collision on the left side (with grid wrapping)
        int xChecked = x - 1;

        // Handle grid wrapping for X
        if (xChecked < 0)
        {
            xChecked = LevelGrid.gridWidth - 1; // Wrap to the right side of the grid
        }

        // Bounds check for wrapped X coordinate
        if (xChecked < 0 || xChecked >= LevelGrid.gridWidth)
        {
            return false; // Safety check
        }



        if (levelGridManager.IsCellSolid(xChecked, y))
        {
            return true; // There's a block on the left
        }
        else
        {
            return false;
        }
    }

    public bool CellSnapBottomOf(int x, int y)
    {
        // Bounds check for X coordinate with wrapping
        if (x < 0) x = LevelGrid.gridWidth - 1;
        if (x >= LevelGrid.gridWidth) x = 0;

        // Check if block should snap to ground
        if ((y <= 0) || (y - 1 >= 0 && y - 1 < LevelGrid.gridHeight && levelGridManager.IsCellSolid(x, y - 1)))
        {
            SnapBlock(lastMouseGridPosition);
            return true;
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

    // Get cached player pivot grid position (avoids repeated expensive calculations)
    private Vector2Int GetPlayerPivotGridPos()
    {
        if (!playerPivotCacheValid)
        {
            if (levelGridManager != null && levelGridManager.player != null)
            {
                cachedPlayerPivotGridPos = levelGridManager.WorldToGrid(levelGridManager.player.transform.position);
                playerPivotCacheValid = true;
            }
        }
        return cachedPlayerPivotGridPos;
    }

    // Update block falling over time
    private void UpdateBlockFalling()
    {
        fallTimer += Time.deltaTime;

        // Calculate new height based on fall speed
        if (fallTimer >= blockSpeed)
        {
            lastMouseGridPosition.y = lastMouseGridPosition.y - 1;
            fallTimer = 0f;

            // Immediately update position when height changes to prevent desync
            UpdateBlockPosition();
        }
    }

    private void RotateBlock()
    {
        if (currentBlock != null && currentBlock.blockMatrix != null)
        {
            // Create a new 3x3 matrix for the rotated result
            Cell[,] rotatedMatrix = new Cell[3, 3];

            // Perform 90-degree counterclockwise rotation
            // For each position in the new matrix
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    // Copy the solid state
                    rotatedMatrix[i, j] = currentBlock.blockMatrix[2 - j, i]; ;

                }
            }

            // Replace the original matrix with the rotated one
            currentBlock.blockMatrix = rotatedMatrix;

            // Update rotation tracking: increment by 90° and wrap around at 360°
            currentBlock.rotation = (currentBlock.rotation + 90) % 360;

            // Update the preview block to show the new rotation immediately
            if (currentPreviewBlock != null)
            {
                Destroy(currentPreviewBlock);
                currentPreviewBlock = null;
            }

            // Create new preview with rotated block
            if (currentBlock.blockPrefab != null)
            {
                currentPreviewBlock = levelGridManager.DrawBlock(lastMouseGridPosition.x, lastMouseGridPosition.y, currentBlock.blockPrefab, currentBlock.rotation);
            }

            Debug.Log($"Block {currentBlock.blockName} rotated 90° counterclockwise - Current rotation: {currentBlock.rotation}°");
        }
    }

    // Update the block position based on current height - ensures position stays in sync
    private void UpdateBlockPosition()
    {
        if (levelGridManager != null && levelGridManager.player != null)
        {
            Vector2Int PlayerPivotGridPos = GetPlayerPivotGridPos();
            if (lastMouseGridPosition.y > PlayerPivotGridPos.y + initialBlocHeight)
            {
                lastMouseGridPosition = new Vector2Int(lastMouseGridPosition.x, PlayerPivotGridPos.y + initialBlocHeight);
            }
            lastMouseGridPosition = new Vector2Int(lastMouseGridPosition.x, lastMouseGridPosition.y);
        }
    }

    // Reset block height to initial value
    public void SnapBlock(Vector2Int mouseGridPos)
    {
        Vector2Int PlayerPivotGridPos = GetPlayerPivotGridPos();
        // we reached the bottom of the grid, keep the bloc in position and reset the block height
        if (currentBlock != null && currentBlock.blockPrefab != null)
        {
            // Use SetBlock to place the entire block pattern on the grid
            levelGridManager.SetBlock(mouseGridPos.x, mouseGridPos.y, currentBlock.blockPrefab, currentBlock);
        }
        else
        {
            Debug.LogWarning("Cannot snap block: currentBlock or its prefab is null!");
            return;
        }
        lastMouseGridPosition.y = PlayerPivotGridPos.y + initialBlocHeight;
        fallTimer = 0f;

        // Update position to match new height
        UpdateBlockPosition();

        // Advance in block queue = get next block + generate a new one last in the queue
        AdvanceBlockQueue();

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
    public Block currentBlock; // Bloc actuellement sélectionné

    // La liste progressionManager.availableBlocks désigne les blocs dispos pour le joueur

    public Block[] blockQueue = new Block[3];

    // private void InitializeBlocks()
    // {
    //     availableBlocks = new Block[]
    //     {
    //         new Block_JumpPad_1(),
    //         new Block_T_5(),
    //         new Block_Simple_1()
    //     }

    //     Debug.Log($"Initialized {availableBlocks.Length} blocks");
    //     foreach (Block block in availableBlocks)
    //     {
    //         Debug.Log($"Block initialized: {block.blockName}");
    //     }
    // }

    private void InitializeQueue()
    {
        blockQueue[0] = GetRandomBlock();
        blockQueue[1] = GetRandomBlock();
        blockQueue[2] = GetRandomBlock();
        currentBlock = blockQueue[0];

        // Ensure first block starts with 0 rotation
        if (currentBlock != null)
        {
            currentBlock.rotation = 0;
        }
        Debug.Log("Blocks Queue Initialized with");
        Debug.Log(blockQueue[0].blockName);
        Debug.Log(blockQueue[1].blockName);
        Debug.Log(blockQueue[2].blockName);

        if (IsUIManagerReady())
        {
            uiManager.UpdateBlocksQueuePreview(blockQueue);
        }
        else
        {
            Debug.LogWarning("UIManager not ready (missing or UI components not assigned), cannot update block queue preview");
        }
    }

    public UIManager uiManager;

    // Check if UIManager is properly configured with all required components
    private bool IsUIManagerReady()
    {
        if (uiManager == null) return false;

        // Check if the preview images are assigned (required for UpdateBlocksQueuePreview)
        return uiManager.previewBlockImage1 != null &&
               uiManager.previewBlockImage2 != null &&
               uiManager.previewBlockImage3 != null;
    }

    public void AdvanceBlockQueue()
    {
        Debug.Log("Advancing in Block Queue");
        // Décale tous les blocs vers la gauche
        for (int i = 0; i < blockQueue.Length - 1; i++)
        {
            blockQueue[i] = blockQueue[i + 1];
        }

        // Ajoute un nouveau bloc aléatoire à la fin
        blockQueue[blockQueue.Length - 1] = GetRandomBlock();

        // Met à jour le bloc courant
        currentBlock = blockQueue[0];

        // Ensure current block starts with 0 rotation (should already be 0, but double-check)
        if (currentBlock != null)
        {
            currentBlock.rotation = 0;
        }

        // Met à jour les previews
        if (IsUIManagerReady())
        {
            uiManager.UpdateBlocksQueuePreview(blockQueue);
        }
        else
        {
            Debug.LogWarning("UIManager not ready (missing or UI components not assigned), cannot update block queue preview");
        }
    }

    public Block GetRandomBlock()
    {

        InitManagers();

        // Vérifie que la liste existe et n'est pas vide
        if (progressionManager.availableBlocks != null && progressionManager.availableBlocks.Count > 0)
        {
            int randomIndex = Random.Range(0, progressionManager.availableBlocks.Count);
            Block template = progressionManager.availableBlocks[randomIndex];

            Debug.Log("Random block index: " + randomIndex);


            // Crée une copie fraîche du bloc sélectionné (pour éviter les références partagées)
            Block newBlock = CreateBlockCopy(template);
            newBlock.rotation = 0; // Assure que la rotation commence à 0

            Debug.Log("Random block generated: " + newBlock.blockName + " (rotation: " + newBlock.rotation + "°)");
            return newBlock;
        }
        else
        {
            Debug.LogWarning("No block available to select.");
            // Crée un bloc simple fallback si la liste est vide
            Block fallback = new Block_Simple_1();
            fallback.blockName = "FallbackBlock";
            fallback.rotation = 0;
            return fallback;
        }
    }


    // Create a copy of a block to avoid shared state issues
    private Block CreateBlockCopy(Block template)
    {
        Block copy;

        // Create new instance based on block type
        if (template is Block_T_5)
        {
            copy = new Block_T_5();
        }
        else if (template is Block_Simple_1)
        {
            copy = new Block_Simple_1();
        }
        else if (template is Block_JumpPad_1)
        {
            copy = new Block_JumpPad_1();
        }
        else
        {
            // Generic fallback - create simple block
            copy = new Block_Simple_1();
            copy.blockName = template.blockName + "_Copy";
        }

        return copy;
    }


}
