using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    public Rigidbody rb;
    public int jumpPower;
    public float fallMultiplier;
    Vector2 vecGravity;

    [Header("Grid Settings")]
    [Tooltip("Reference to the LevelGridManager")]
    public LevelGridManager levelGridManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        vecGravity = new Vector2(0, -Physics2D.gravity.y);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2Int playerGridPosition;
        playerGridPosition = levelGridManager.GetPlayerGridPosition();
        if (LevelGrid.grid[playerGridPosition.x + 1, playerGridPosition.y] != null)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
        }

        
    }
}
