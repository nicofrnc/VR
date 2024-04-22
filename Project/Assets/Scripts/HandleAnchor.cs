using UnityEngine;

public class HandleAnchor : ObjectAnchor
{
    public Transform handler;

    public float maxGrabDistance = 2f;


    // This method overrides the detach_from method in the ObjectAnchor class
    public override void detach_from(HandController hand_controller)
    {
        // Make sure that the right hand controller asked for the release
        if (this.hand_controller != hand_controller) return;

        // Detach the hand controller
        this.hand_controller = null;

        // If the initial transform parent is not null, return the object to its parent's position
        if (initial_transform_parent != null)
        {
            transform.SetParent(initial_transform_parent);
            transform.position = handler.transform.position; // Reset local position
            transform.rotation = handler.transform.rotation; // Reset local rotation
            transform.localScale = Vector3.one; // Reset local Scale 

            // Avoid movement door when releasing the handler
            Rigidbody rbhandler = handler.GetComponent<Rigidbody>();
            rbhandler.velocity = Vector3.zero;
            rbhandler.angularVelocity = Vector3.zero; 

        }
    }


    void Update()
    {
        // Check if the object is currently attached to a hand controller
        if (hand_controller != null)
        {
            // Calculate the distance between the hand controller and the handle
            float distanceToHandle = Vector3.Distance(hand_controller.transform.position, handler.position);

            // If the distance exceeds the maximum grab distance, release the handle
            if (distanceToHandle > maxGrabDistance)
            {
                detach_from(hand_controller);
            }
        }
    }
 
}