using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    public float jumpHeight = 3f;
    public float jumpDuration = 0.5f;

    private bool isJumping = false;
    private float jumpTime = 0f;
    private float startY;

    private float lastHeightOffset = 0;


    [Header("Grid Settings")]
    [Tooltip("Reference to the LevelGridManager")]
    public LevelGridManager levelGridManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector2Int playerGridPosition;
        playerGridPosition = levelGridManager.GetPlayerGridPosition();
        Debug.Log("Player Grid Position: " + playerGridPosition);
        if ((LevelGrid.grid[playerGridPosition.x + 1, playerGridPosition.y] != null) && !isJumping)
        {
            Debug.Log("Player can jump");
            StartJump();
        }

        if (isJumping)
        {
            UpdateJump();
        }

    }

    void StartJump()
    {
        isJumping = true;
        jumpTime = 0f;
        startY = transform.position.y;
    }

    void UpdateJump()
    {
        jumpTime += Time.deltaTime;
        float t = jumpTime / jumpDuration;
        Vector2Int playerGridPosition;
        playerGridPosition = levelGridManager.GetPlayerGridPosition();

        // Courbe parabolique simple (0 → 1 → 0)
        float heightOffset = 4 * jumpHeight * t * (1 - t);
        Vector3 pos = transform.position;
        pos.y = startY + heightOffset;
        transform.position = pos;
        lastHeightOffset = heightOffset;

        // Check if player has landed
        if ((heightOffset < lastHeightOffset) && lastHeightOffset > 0)
        {
            CheckStopJump();
            Debug.Log("Check Stop Jump");
        }
    }

    void CheckStopJump()
    {
        Vector2Int playerGridPosition = levelGridManager.GetPlayerGridPosition();
        
        // Check if player has landed on ground or on a block
        if ((playerGridPosition.y == 0) || (LevelGrid.grid[playerGridPosition.x, playerGridPosition.y - 1] != null))
        {
            Debug.Log("Player has landed at " + playerGridPosition.y);
            
            // End the jump
            isJumping = false;
            
            // Adjust position to be above the grid
            Vector3 pos = transform.position;
            Debug.Log("Player has landed at grid Y: " + playerGridPosition.y);
            
            // Debug: Check grid origin and world conversion
            Vector3 worldPos = levelGridManager.GridToWorld(playerGridPosition.x, playerGridPosition.y);
            Debug.Log("Grid origin: " + levelGridManager.gridOrigin.position);
            Debug.Log("Grid to world conversion: (" + playerGridPosition.x + "," + playerGridPosition.y + ") -> " + worldPos);
            Debug.Log("Current player world position: " + transform.position);
            
            pos.y = worldPos.y + 0.5f;
            Debug.Log("Final landing position Y: " + pos.y);
            transform.position = pos;
        }
    }
}
