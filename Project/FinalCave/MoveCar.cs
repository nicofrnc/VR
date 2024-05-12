using UnityEngine;

public class MoveCar : MonoBehaviour
{
    public float speed = 5f; // Speed of the car
    public Transform cameraTransform;

    public GameObject startPoint;
    public GameObject endPoint;

    public AudioSource audioSource; // AudioSource component for playing sounds

    private bool isPlayerAttached = false;

    private bool start = false;
    private bool end = false;

    public void SetPlayerAttached(bool attached)
    {
        isPlayerAttached = attached; // Set whether the player is attached to the car
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Respawn"))
        {
            start = true;
        }
        else if (other.CompareTag("Finish"))
        {
            end = true;
        }
    }

    void Update()
    {
        // If the player is not attached, stop any ongoing audio and return
        if (!isPlayerAttached)
        {
            if (audioSource.isPlaying)
                audioSource.Stop();
            return;
        }

        // Get the pressure values from both triggers
        float leftPressure = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger);
        float rightPressure = OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger);

        // Calculate the acceleration based on the maximum trigger pressure
        float acc = speed * Mathf.Max(leftPressure, rightPressure);

        // Calculate the forward vector of the attached camera, neutralizing the vertical component
        Vector3 cameraForward = cameraTransform.forward;
        cameraForward.y = 0; // Neutralize the vertical component to ensure horizontal movement
        cameraForward.Normalize(); // Normalize the vector to maintain consistent speed

        // Determine the movement direction based on which trigger is pressed more
        Vector3 direction = Vector3.zero;

        if (leftPressure > rightPressure && !start)
            direction = -cameraForward; // Move backwards if the left trigger is more pressed or the player is at the starting point
        else if (rightPressure > leftPressure && !end)
            direction = cameraForward; // Move forwards if the right trigger is more pressed or the player is at the ending point

        transform.position += direction * acc * Time.deltaTime;

        // Play sound when the car starts moving and stop when it's not moving
        if (direction != Vector3.zero)
        {
            if (!audioSource.isPlaying)
                audioSource.Play(); // Play sound only if it is not already playing
        }
        else if (audioSource.isPlaying) // Stop sound if there is no movement
        {
            audioSource.Stop();
        }
    }
}
