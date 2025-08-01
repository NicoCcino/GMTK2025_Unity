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
        if (LevelGrid.grid[playerGridPosition.x + 1, playerGridPosition.y] != null)
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

        // Vérifie si le joueur a atterri
        if (heightOffset <= lastHeightOffset && lastHeightOffset > 0)
        {

            if ((playerGridPosition.y == 0) || (LevelGrid.grid[playerGridPosition.x, playerGridPosition.y - 1] != null))
            {
                Debug.Log("Player has landed" + playerGridPosition.y);
                // Fin du saut
                t = 1f;
                isJumping = false;
                pos.y = levelGridManager.GetPlayerGridPosition().y + 0.5f; // Ajuste la position pour qu'elle soit au-dessus de la grille
            }
        }
    }

}
