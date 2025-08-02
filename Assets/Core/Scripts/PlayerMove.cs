using UnityEngine;

public class Script_Move_World : MonoBehaviour
{
    public static bool isPlayerDead = false;
    public float playerRunSpeed = 5f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Only move if player is not dead
        if (!isPlayerDead)
        {
            // Move the object left at a constant speed
            transform.Translate(Vector3.right * playerRunSpeed * Time.deltaTime);
        }
    }
}
