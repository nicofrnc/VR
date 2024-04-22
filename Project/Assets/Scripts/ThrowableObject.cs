
using UnityEngine;

public class ThrowableObject : ObjectAnchor
{

	public override void Start()
	{
		initial_transform_parent = transform.parent;

		// NEW Add Rigidbody component if not already attached
		if (!GetComponent<Rigidbody>())
		{
			gameObject.AddComponent<Rigidbody>();
		}
	}

	public override void attach_to(HandController hand_controller)
	{
		// Store the hand controller in memory
		this.hand_controller = hand_controller;

		// Set the object to be placed in the hand controller referential
		transform.SetParent(hand_controller.transform);

		// Activate or deactivate the Rigidbody based on the needs
		Rigidbody rb = GetComponent<Rigidbody>();
		if (rb != null)
		{
			rb.isKinematic = true; // Ensure Rigidbody is kinematic when attached to hand
		}
	}


	// This method overrides the detach_from method in the ObjectAnchor class
	public override void detach_from(HandController hand_controller)
    {
		// Make sure that the right hand controller ask for the release
		if (this.hand_controller != hand_controller) return;

		// Detach the hand controller
		this.hand_controller = null;

		// Set the object to be placed in the original transform parent
		transform.SetParent(initial_transform_parent);

		// Activate or deactivate the Rigidbody based on the needs
		Rigidbody rb = GetComponent<Rigidbody>();
		if (rb != null)
		{
			rb.isKinematic = false; // Re-enable Rigidbody when detached from hand
		}
	}


}
