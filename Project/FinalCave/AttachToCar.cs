using UnityEngine;

public class AttachToCar : MonoBehaviour
{
    // Anchor point on the car 
    public Transform cameraAnchor;
    // Variable of character controller
    private CharacterController characterController;
    // Variables to know if the player is attached to the car and is near the car 
    private bool isAttached = false;
    private bool isNearCar = false;

    // Variable to access MoveCar class
    private MoveCar moveCarScript;


    private void Start()
    {
        // Retrieve the character controller
        characterController = GetComponent<CharacterController>();
        // Find the MoveCar script attached to the car using the cameraAnchor
        moveCarScript = cameraAnchor.GetComponentInParent<MoveCar>();
    }

    // Check inputs for attaching or detaching the car
    private void Update()
    {
        // Attach the player if near the car and button pressed
        if (isNearCar && !isAttached && OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch))
        {
            AttachCar();
        }
        // Detach the player if already attached and button pressed
        else if (isAttached && OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.LTouch))
        {
            DetachFromCar();
        }


    }

    // Set flag when entering the car's trigger collider
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            Debug.Log("Collision detected");
            // Set variable to close to Car 
            isNearCar = true;
        }
    }

    // Reset flag when exiting the car's trigger collider
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            // Set variable to non close to Car 
            isNearCar = false;
            // Detach Player of the Car 
            if (isAttached)
            {
                DetachFromCar();
            }
        }
    }
    

    // Method to attach player to the car
    private void AttachCar()
    {
        // Player set as a child of the car 
        this.transform.SetParent(cameraAnchor);
        // Object will be positioned exactly at the location of its parent
        this.transform.localPosition = Vector3.zero;
        // Object aligns exactly with the orientation of its parent
        this.transform.localRotation = Quaternion.identity;
        // Set variable to 'attached' mode 
        isAttached = true;

        // Disable character Controller and enable movement of the car 
        if (characterController != null && moveCarScript != null)
        {
            characterController.enabled = false; // prevents the player from being affected by physics forces :/
            moveCarScript.SetPlayerAttached(true);
        }
    }

    // Method to detach player from the car
    private void DetachFromCar()
    {
        // Player is no longer a child of the car 
        this.transform.SetParent(null);
        // Set variable to 'non attached' mode 
        isAttached = false;

        if (characterController != null)
        {
            // Enable character Controller 
            characterController.enabled = true;
            // Use the current position and rotation of the cameraAnchor to set the player's position upon detachment
            characterController.transform.position = cameraAnchor.position;
            characterController.transform.rotation = cameraAnchor.rotation;
        }

        // Player no longer attaches to the car so car won't move 
        if (moveCarScript != null)
        {
            moveCarScript.SetPlayerAttached(false);
        }
    }
}