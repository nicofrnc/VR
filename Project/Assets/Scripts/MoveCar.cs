using UnityEngine;

public class MoveCar : MonoBehaviour
{
    public float speed = 5f; // Speed of the car
    public Transform cameraTransform; // Camera's transform to determine the forward direction
    public AudioSource audioSource; // AudioSource component for playing sounds
    private bool isPlayerAttached = false; // Flag to check if the player is attached to the car

    public void SetPlayerAttached(bool attached)
    {
        isPlayerAttached = attached; // Set whether the player is attached to the car
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
        if (leftPressure > rightPressure)
            direction = -cameraForward; // Move backwards if the left trigger is more pressed
        else if (rightPressure > leftPressure)
            direction = cameraForward; // Move forwards if the right trigger is more pressed

        // Move the car in the direction the camera is facing
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