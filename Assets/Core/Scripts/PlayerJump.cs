using UnityEngine;

public enum PlayerState
{
    Stable,
    Ascending,
    Descending
}

public class PlayerJump : MonoBehaviour
{
    public float jumpHeight = 3f;
    public float jumpDuration = 0.5f;
    public float gravity = 20f; // Gravity strength for realistic parabolic motion

    private PlayerState currentState = PlayerState.Stable;
    private float jumpTime = 0f;
    private float startY;
    private float initialVelocity = 0f;
    private float currentVelocity = 0f;

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
        Vector2Int playerGridPosition = levelGridManager.GetPlayerGridPosition();
        Vector3 pos = transform.position;
        
        // Check if player have to jump obstacle
        Vector2Int forwardPlayerGridPosition = levelGridManager.WorldToGrid(new Vector3(pos.x + 1f, pos.y, 0));
        if (((LevelGrid.grid[(forwardPlayerGridPosition.x+1)%(LevelGrid.gridWidth-1), forwardPlayerGridPosition.y] != null)
        || (LevelGrid.grid[(forwardPlayerGridPosition.x+1)%(LevelGrid.gridWidth-1), forwardPlayerGridPosition.y+1] != null)
        || (LevelGrid.grid[(playerGridPosition.x+1)%(LevelGrid.gridWidth-1), playerGridPosition.y] != null))
        && currentState == PlayerState.Stable)
        {
            Debug.Log("Player jump obstacle");
            StartJump();
        }
        // Check if player have to jump hole
        if (playerGridPosition.y > 0)
        {
            if (((LevelGrid.grid[(playerGridPosition.x + 2)%(LevelGrid.gridWidth-1), playerGridPosition.y-1] == null) 
            && (playerGridPosition.y-1 >= 0))&& currentState == PlayerState.Stable)
            {
                Debug.Log("Player jump hole");
                StartJump();
            }
        }

        // Call appropriate updater based on state
        switch (currentState)
        {
            case PlayerState.Ascending:
                UpdateAscending();
                break;
            case PlayerState.Descending:
                UpdateDescending();
                break;
            case PlayerState.Stable:
                UpdateStable();
                break;
        }
    }

    void CheckForFalling(Vector2Int playerGridPosition)
    {
        // Check if there's no block underneath and not at ground level
        bool hasSupport = (playerGridPosition.y == 0) || (LevelGrid.grid[playerGridPosition.x, playerGridPosition.y - 1] != null);
        
        if (!hasSupport)
        {
            Debug.Log("Player is falling - no support underneath");
            StartDescending();
        }
    }

    void StartDescending()
    {
        currentState = PlayerState.Descending;
        startY = transform.position.y;
        currentVelocity = 0f; // Start falling from rest
    }

    void StartJump()
    {
        currentState = PlayerState.Ascending;
        jumpTime = 0f;
        startY = transform.position.y;
        
        // Calculate initial velocity for parabolic jump
        // Using physics formula: v = sqrt(2 * g * h) where h is jump height
        initialVelocity = Mathf.Sqrt(2f * gravity * jumpHeight);
        currentVelocity = initialVelocity;
    }

    void UpdateStable()
    {
        // Player is stable on ground or block, no movement needed
        Vector2Int playerGridPosition = levelGridManager.GetPlayerGridPosition();
        CheckForFalling(playerGridPosition);
    }

    void UpdateAscending()
    {
        jumpTime += Time.deltaTime;
        
        // Apply gravity to create parabolic motion
        currentVelocity -= gravity * Time.deltaTime;
        
        Vector3 pos = transform.position;
        pos.y += currentVelocity * Time.deltaTime;
        transform.position = pos;
        
        // Check if we've reached the peak (velocity becomes negative)
        if (currentVelocity <= 0)
        {
            Debug.Log("Reached jump peak, transitioning to descending");
            currentState = PlayerState.Descending;
        }
        
        lastHeightOffset = pos.y - startY;
    }

    void UpdateDescending()
    {
        Vector3 pos = transform.position;
        
        // Check if player has landed
        // Check the player position with a margin of 0.5f to have the bottom of the player touching the ground
        if (pos.y <= 0.5f)
        {
            Debug.Log($"Player has landed on the ground at position: {pos}");
            StopDescending();
            return;
        }
        
        Vector2Int playerGridPosition = levelGridManager.WorldToGrid(new Vector3(pos.x, pos.y - 0.5f, 0));
        if (LevelGrid.grid[playerGridPosition.x, playerGridPosition.y] != null)
        {
            Debug.Log($"Player has landed on a block at position: {pos}, grid position: {playerGridPosition}");
            StopDescending();
            return;
        }
        
        // Realistic falling with gravity
        currentVelocity += gravity * Time.deltaTime;
        pos.y -= currentVelocity * Time.deltaTime;
        transform.position = pos;
    }

    void StopDescending()
    {
        currentState = PlayerState.Stable;
        currentVelocity = 0f;
        
        // Adjust position to be above the grid
        Vector2Int playerGridPosition = levelGridManager.GetPlayerGridPosition();
        Vector3 pos = transform.position;
        
        Debug.Log($"StopDescending - Current pos: {pos}, playerGridPosition: {playerGridPosition}");
        Debug.Log($"StopDescending - Setting pos.y to: {playerGridPosition.y + 0.5f}");
        
        pos.y = playerGridPosition.y + 0.5f;
        transform.position = pos;
        
        Debug.Log($"StopDescending - Final pos: {transform.position}");
    }
}
