using UnityEngine;

public class CameraShake : MonoBehaviour
{

    public bool isShaking = false; // Flag to check if camera is shaking
    public float shakeDuration = 0.5f; // Duration of the shake
    public float shakeMagnitude = 0.5f; // Magnitude of the shake
    private Vector3 originalPosition; // Original position of the camera

    [ContextMenu("Start Shake")]
    public void StartShake()
    {
        if (!isShaking)
        {
            originalPosition = transform.localPosition; // Store the original position
            isShaking = true; // Set the shaking flag to true
            InvokeRepeating("Shake", 0f, 0.01f); // Start shaking at regular intervals
            Invoke("StopShake", shakeDuration); // Stop shaking after the specified duration
        }
    }

    private void Shake()
    {
        if (isShaking)
        {
            // Generate a random offset for the shake
            float offsetX = Random.Range(-shakeMagnitude, shakeMagnitude);
            float offsetY = Random.Range(-shakeMagnitude, shakeMagnitude);
            // Apply the offset to the camera's position
            transform.localPosition = new Vector3(originalPosition.x + offsetX, originalPosition.y + offsetY, originalPosition.z);
        }
    }
    private void StopShake()
    {
        isShaking = false; // Set the shaking flag to false
        CancelInvoke("Shake"); // Stop invoking the Shake method
        transform.localPosition = originalPosition; // Reset the camera's position to the original position
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
