using UnityEngine;

public class Script_Move_World : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Move the object left at a constant speed
        float speed = 5f; // units per second
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }
}
