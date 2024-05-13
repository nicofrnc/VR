using UnityEngine;

public class CanvasController : MonoBehaviour
{
    public Transform player; // Assign this to the player's transform in the inspector
    public float visibilityThreshold = 5f; // Distance at which the canvas will be visible

    private Canvas canvas; // The canvas component

    void Start()
    {
        // Get the Canvas component on this GameObject
        canvas = GetComponent<Canvas>();
    }

    void Update()
    {
        if (player == null) return;

        // Calculate the distance from the player to this canvas
        float distance = Vector3.Distance(player.position, transform.position);

        // Toggle the visibility based on the distance
        if (distance <= visibilityThreshold)
        {
            canvas.enabled = true; // Show the canvas
        }
        else
        {
            canvas.enabled = false; // Hide the canvas
        }
    }
}