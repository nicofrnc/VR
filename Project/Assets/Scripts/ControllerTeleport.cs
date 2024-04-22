using UnityEngine;

public class ControllerTeleport : MonoBehaviour
{
    // Controller Anchors
    [Header("Controller Anchors")]
    public Transform leftHandAnchor; 
    public Transform rightHandAnchor; 

    // Maximum distance for teleportation 
    [Header("Maximum Distance")]
    [Range(2f, 30f)]
    public float maximumTeleportationDistance = 15f;

    // Store the refence to the marker prefab used to highlight the targeted point
    [Header("Marker")]
    public GameObject markerPrefab; 
    private GameObject markerPrefabInstance;

    // Retrieve the character controller used later to move the player in the environment
    private CharacterController characterController;

    // Indicates if the player is currently aiming
    private bool aiming = false; 

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Check if the index triggers are pressed on the controllers
        bool leftTriggerPressed = OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.LTouch) ;
        bool rightTriggerPressed = OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.RTouch) ;


        // Aim with the right hand by pressing its index trigger
        if (rightTriggerPressed && !aiming)
        {
            Vector3 targetPoint;

            // Aiming has started, show marker at Target point
            if (AimWith(rightHandAnchor, out targetPoint))
            {
                ShowMarkerAt(targetPoint);
                aiming = true; 
            }
        }

        // Execute teleportation if both triggers are pressed after aiming
        if (leftTriggerPressed && rightTriggerPressed && aiming)
        {
            // Teleport to the marker's position
            characterController.Move(markerPrefabInstance.transform.position - this.transform.position);
            aiming = false; // Reset aiming state
            RemoveMarker(); // Clean up the marker
        }
        else if (!rightTriggerPressed)
        {
            RemoveMarker(); // Remove the marker if the right hand trigger is released
            aiming = false; // Reset aiming state
        }
    }

    protected bool AimWith(Transform handTransform, out Vector3 targetPoint)
    {
        // Default the "output" target point to the null vector
        targetPoint = new Vector3();

        // Launch the ray cast and leave if it doesn't hit anything
        RaycastHit hit;
        if (!Physics.Raycast(handTransform.position, handTransform.forward, out hit, maximumTeleportationDistance)) return false;

        // If the aimed point is out of range (i.e. the raycast distance is above the maximum distance) then prevent the teleportation
        if (hit.distance > maximumTeleportationDistance) return false;

        // A valid target point was found
        targetPoint = hit.point;
        return true; 
         
    }

    private void ShowMarkerAt(Vector3 position)
    {
        if (markerPrefabInstance == null)
        {
            markerPrefabInstance = Instantiate(markerPrefab, position, Quaternion.identity);
        }
        else
        {
            markerPrefabInstance.transform.position = position;
        }
    }

    private void RemoveMarker()
    {
        if (markerPrefabInstance != null)
        {
            Destroy(markerPrefabInstance);
            markerPrefabInstance = null;
        }
    }
}

