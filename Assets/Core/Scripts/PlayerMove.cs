using UnityEngine;

public class Script_Move_World : MonoBehaviour
{
    public static bool isPlayerDead = false;
    
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
            float speed = 5f; // units per second
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        }
    }
}
