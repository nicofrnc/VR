using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HandController : MonoBehaviour
{

	// Store the hand type to know which button should be pressed
	public enum HandType : int { LeftHand, RightHand };
	[Header("Hand Properties")]
	public HandType handType;


	// Store the player controller to forward it to the object
	[Header("Player Controller")]
	public MainPlayerController playerController;

	List<Vector3> trackingPos = new List<Vector3>();



	// Store all gameobjects containing an Anchor
	// N.B. This list is static as it is the same list for all hands controller
	// thus there is no need to duplicate it for each instance
	static protected ObjectAnchor[] anchors_in_the_scene;
	void Start()
	{
		// Prevent multiple fetch
		if (anchors_in_the_scene == null) anchors_in_the_scene = GameObject.FindObjectsOfType<ObjectAnchor>();
	}


	// This method checks that the hand is closed depending on the hand side
	protected bool is_hand_closed()
	{
		// Case of a left hand
		if (handType == HandType.LeftHand) return
		  //OVRInput.Get(OVRInput.Button.Three)                           // Check that the A button is pressed
		  //&& OVRInput.Get(OVRInput.Button.Four)                         // Check that the B button is pressed
		  //&&
		  OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger) > 0.5     // Check that the middle finger is pressing
		  && OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) > 0.5;   // Check that the index finger is pressing


		// Case of a right hand
		else return
			//OVRInput.Get(OVRInput.Button.One)                             // Check that the A button is pressed
			//&& OVRInput.Get(OVRInput.Button.Two)                          // Check that the B button is pressed
			//&&
			OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) > 0.5   // Check that the middle finger is pressing
			&& OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0.5; // Check that the index finger is pressing
	}


	// NEWWWWWWStockez les positions précédentes de la manette sur plusieurs frames
	private Queue<Vector3> previousControllerPositions = new Queue<Vector3>();
	private int numPreviousPositionsToTrack = 15; // Nombre de positions précédentes à garder en mémoire
	private Vector3 previousControllerPosition;
	private float throwForce;

	// Automatically called at each frame
	void Update() {

		//Keep track of the position
		if (object_grasped != null)
		{
			if (trackingPos.Count > 40)
			{
				trackingPos.RemoveAt(0);
			}
		}

		trackingPos.Add(transform.position);

		//NEWWWWWW
		// Ajoutez la position actuelle de la manette à la queue
		previousControllerPositions.Enqueue(transform.position);

		// Limitez le nombre de positions dans la queue pour éviter une utilisation excessive de la mémoire
		while (previousControllerPositions.Count > numPreviousPositionsToTrack)
		{
			previousControllerPositions.Dequeue();
		}

		// Calculez le déplacement moyen de la manette sur les frames précédentes
		Vector3 averageControllerDisplacement = Vector3.zero;
		foreach (Vector3 position in previousControllerPositions)
		{
			averageControllerDisplacement += position - previousControllerPosition;
		}
		averageControllerDisplacement /= previousControllerPositions.Count;

		// Calculez la vitesse instantanée moyenne de la manette
		float controllerSpeed = averageControllerDisplacement.magnitude / Time.deltaTime;

		// Stockez la position actuelle de la manette pour une utilisation dans la prochaine frame
		previousControllerPosition = transform.position;

		// Utilisez la vitesse de la manette pour ajuster la force du lancer
		throwForce = (float)(controllerSpeed * 0.1);

		handle_controller_behavior();
		handle_throw_behavior();
	}


	// Store the previous state of triggers to detect edges
	protected bool is_hand_closed_previous_frame = false;

	// Store the object atached to this hand
	// N.B. This can be extended by using a list to attach several objects at the same time
	protected ObjectAnchor object_grasped = null;

	public UnityEvent objetPrisEvent;

	/// <summary>
	/// This method handles the linking of object anchors to this hand controller
	/// </summary>
	protected void handle_controller_behavior()
	{

		// Check if there is a change in the grasping state (i.e. an edge) otherwise do nothing
		bool hand_closed = is_hand_closed();
		if (hand_closed == is_hand_closed_previous_frame) return;
		is_hand_closed_previous_frame = hand_closed;


		//==============================================//
		// Define the behavior when the hand get closed //
		//==============================================//
		if (hand_closed)
		{

			// Log hand action detection
			Debug.LogWarningFormat("{0} get closed", this.transform.parent.name);

			// Determine which object available is the closest from the left hand
			int best_object_id = -1;
			float best_object_distance = float.MaxValue;
			float oject_distance;

			// Iterate over objects to determine if we can interact with it
			for (int i = 0; i < anchors_in_the_scene.Length; i++)
			{

				// Skip object not available
				if (!anchors_in_the_scene[i].is_available()) continue;

				// Compute the distance to the object
				oject_distance = Vector3.Distance(this.transform.position, anchors_in_the_scene[i].transform.position);

				// Keep in memory the closest object
				// N.B. We can extend this selection using priorities
				if (oject_distance < best_object_distance && oject_distance <= anchors_in_the_scene[i].get_grasping_radius())
				{
					best_object_id = i;
					best_object_distance = oject_distance;
				}
			}

			// If the best object is in range grab it
			if (best_object_id != -1)
			{

				// Store in memory the object grasped
				object_grasped = anchors_in_the_scene[best_object_id];

				// Log the grasp
				Debug.LogWarningFormat("{0} grasped {1}", this.transform.parent.name, object_grasped.name);

				// Grab this object
				object_grasped.attach_to(this);

				TriggerHapticFeedback();

				objetPrisEvent.Invoke();
			}


			//==============================================//
			// Define the behavior when the hand get opened //
			//==============================================//
		}
		else if (object_grasped != null)
		{
			// Log the release
			Debug.LogWarningFormat("{0} released {1}", this.transform.parent.name, object_grasped.name);

			// Release the object
			object_grasped.detach_from(this);
		}
	}

	
	void TriggerHapticFeedback()
	{
		
		OVRInput.Controller controllerType = (handType == HandType.LeftHand) ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch;

		float vibrationStrength = 0.5f; 

		OVRInput.SetControllerVibration(vibrationStrength, vibrationStrength, controllerType);

		Invoke("StopHapticFeedback", 0.5f); 
	}

	void StopHapticFeedback()
	{
		OVRInput.Controller controllerType = (handType == HandType.LeftHand) ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch;
		OVRInput.SetControllerVibration(0, 0, controllerType);
	}


	protected void handle_throw_behavior()
	{
        bool throwButtonPressed_ = OVRInput.Get(OVRInput.Button.Two);

        if (throwButtonPressed_ && object_grasped != null)
		{
			Vector3 throwDirection = Camera.main.transform.forward.normalized;

			object_grasped.detach_from(this);

			Rigidbody rb = object_grasped.GetComponent<Rigidbody>();
			if (rb != null)
			{
				rb.isKinematic = false;
				rb.AddForce(throwDirection * throwForce, ForceMode.Impulse);
			}
		}
	}
}