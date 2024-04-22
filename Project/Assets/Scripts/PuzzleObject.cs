using UnityEngine;
using System.Collections;
public class PuzzleObject : MonoBehaviour {

	[Header( "Grasping Properties" )]
	public float graspingRadius = 0.1f;
	
	public enum DirectionOfMovement : int { x, y,z };
	[Header( "direction of movement Properties" )]
	public DirectionOfMovement directionOfMovement;
	
	// Store initial transform parent
	protected Transform initial_transform_parent;
	void Start () {
		initial_transform_parent = transform.parent;
	}


	// Store the hand controller this object will be attached to
	protected PuzzleController hand_controller = null;

	public void attach_to ( PuzzleController hand_controller ) {
		// Store the hand controller in memory
		this.hand_controller = hand_controller;
		Transform newTransform=new GameObject().transform;
		newTransform.position = hand_controller.transform.position;
		newTransform.rotation=Quaternion.identity;
		transform.SetParent(newTransform);
		
		StartCoroutine(UpdatePosition(newTransform.transform));
		//Collider collider = newTransform.gameObject.AddComponent<BoxCollider>();
		//Rigidbody rigidbody = newTransform.gameObject.AddComponent<Rigidbody>();
		//rigidbody.isKinematic = true; // Make the object kinematic to prevent it from being affected by physics
		//transform.rotation = Quaternion.Inverse(hand_controller.transform.rotation);
	}
	
	private IEnumerator UpdatePosition(Transform newTransform) {
		while (true) {
			
			Vector3 targetPosition = newTransform.position;
			Vector3 originalPosition=newTransform.position;
			
			
			RaycastHit hit;
			
			if (directionOfMovement == DirectionOfMovement.x){targetPosition.x = hand_controller.transform.position.x;}
			if (directionOfMovement == DirectionOfMovement.y){targetPosition.y = hand_controller.transform.position.y;}
			if (directionOfMovement == DirectionOfMovement.z){targetPosition.z = hand_controller.transform.position.z;}
			
			Vector3 direction = targetPosition - originalPosition;
			
			if (Physics.Raycast(originalPosition, direction.normalized, out hit, direction.magnitude)) {
            // If a collision is detected, adjust the target position to the point of collision
            targetPosition = hit.point;
			}
			
			newTransform.position = targetPosition;
			yield return null; // Wait for next frame
		}
	}
	public void detach_from ( PuzzleController hand_controller ) {
		// Make sure that the right hand controller ask for the release
		if ( this.hand_controller != hand_controller ) return;

		// Detach the hand controller
		this.hand_controller = null;

		// Set the object to be placed in the original transform parent
		transform.SetParent( initial_transform_parent );
	}

	public bool is_available () { return hand_controller == null; }

	public float get_grasping_radius () { return graspingRadius; }
}
