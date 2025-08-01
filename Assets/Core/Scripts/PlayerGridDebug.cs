using UnityEngine;

public class PlayerGridDebug : MonoBehaviour
{

    public GameObject player;
    public LevelGridManager levelGridManager;
    private Transform playerTransform;
    private void OnDrawGizmos()
    {
        if (playerTransform != null)
        {
            Gizmos.color = Color.red;
            Vector3 pos = playerTransform.position;
            Gizmos.DrawSphere(pos, 0.2f);
            Vector2Int playerGridPosition = levelGridManager.WorldToGrid(pos);
            UnityEditor.Handles.Label(pos + Vector3.up * 0.5f, $"Grid: {playerGridPosition}");

            Vector3 playerWorldPosition = levelGridManager.GridToWorld(playerGridPosition.x, playerGridPosition.y);
            UnityEditor.Handles.Label(pos + Vector3.up * 1f, $"World: {Mathf.FloorToInt(playerWorldPosition.x)}, {Mathf.FloorToInt(playerWorldPosition.y)}");

        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerTransform = player.transform;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
