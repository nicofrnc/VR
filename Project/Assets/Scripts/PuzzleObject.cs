using UnityEngine;
using System.Collections;
public class PuzzleObject : InteractiveObject {

	[Header( "Grasping Properties" )]
	public float graspingRadius = 0.1f;
	
	public enum DirectionOfMovement : int { x, y,z };
	[Header( "direction of movement Properties" )]
	public DirectionOfMovement directionOfMovement;
	
	protected Transform initial_transform_parent;
	protected PuzzleController controller = null;
	protected WandController wand_controller=null;

	
	public bool is_available () { return (controller == null) && (wand_controller == null); }
	public float get_grasping_radius () { return graspingRadius; }
	
	void Start () {
		myType = ObjectType.Puzzle;//what type of interactive object this is
		initial_transform_parent = transform.parent;
	}
	
	public void attach_to ( PuzzleController controller ) {
		this.controller = controller;
		Transform newTransform=new GameObject().transform;
		newTransform.position = controller.transform.position;
		newTransform.rotation=Quaternion.identity;
		transform.SetParent(newTransform);
		
		StartCoroutine(UpdatePosition(newTransform.transform));
	}
	
	public void detach_from ( PuzzleController controller ) {
		if ( this.controller != controller ) return;
		this.controller = null;
		transform.SetParent( initial_transform_parent );
	}

	public void attach_to_wand ( WandController controller ) {
		this.wand_controller = controller;
		Transform newTransform=new GameObject().transform;
		newTransform.position = controller.transform.position;
		newTransform.rotation= controller.transform.rotation;
		transform.SetParent(newTransform);
		StartCoroutine(UpdatePosition2(newTransform));
	}
	public void detach_from_wand ( WandController controller) {
		if ( this.wand_controller != controller ) return;
		this.wand_controller = null;
		transform.SetParent( initial_transform_parent );
	}

	private IEnumerator UpdatePosition(Transform newTransform) {
		while (true) {
			
			Vector3 targetPosition = newTransform.position;
			Vector3 originalPosition=newTransform.position;
			
			
			RaycastHit hit;
			
			if (directionOfMovement == DirectionOfMovement.x){targetPosition.x = controller.transform.position.x;}
			if (directionOfMovement == DirectionOfMovement.y){targetPosition.y = controller.transform.position.y;}
			if (directionOfMovement == DirectionOfMovement.z){targetPosition.z = controller.transform.position.z;}
			
			Vector3 direction = targetPosition - originalPosition;
			
			if (Physics.Raycast(originalPosition, direction.normalized, out hit, direction.magnitude)) {
            // If a collision is detected, adjust the target position to the point of collision
            targetPosition = hit.point;
			}
			
			newTransform.position = targetPosition;
			yield return null; // Wait for next frame
		}
	}
	
	private IEnumerator UpdatePosition2(Transform newTransform) {
		while (true) {
			
			
			Vector3 targetPosition = newTransform.position;
			Vector3 originalPosition=newTransform.position;
			float distance=0;
			
			if (directionOfMovement == DirectionOfMovement.x)
			{
				distance=Mathf.Abs(this.transform.position.z-wand_controller.transform.position.z);
			}
			if (directionOfMovement == DirectionOfMovement.y)
			{
				distance=Mathf.Max(Mathf.Abs(this.transform.position.x-wand_controller.transform.position.x),Mathf.Abs(this.transform.position.z-wand_controller.transform.position.z));
			}
			if (directionOfMovement == DirectionOfMovement.z)
			{
				distance=Mathf.Abs(this.transform.position.x-wand_controller.transform.position.x);
			}
			distance+=1f;
			
			Vector3 forwardDirection = wand_controller.transform.forward;
			Vector3 newPosition = wand_controller.transform.position + forwardDirection * distance;
			
			Transform temp=new GameObject().transform;
			temp.position = newPosition;
			temp.rotation= wand_controller.transform.rotation;
			
			RaycastHit hit;
			
			if (directionOfMovement == DirectionOfMovement.x)
			{
				targetPosition.x = temp.position.x;
			}
			if (directionOfMovement == DirectionOfMovement.y)
			{
				targetPosition.y = temp.position.y;
			}
			if (directionOfMovement == DirectionOfMovement.z)
			{
				targetPosition.z = temp.position.z;
			}
			
			Vector3 direction = targetPosition - originalPosition;
			
			if (Physics.Raycast(originalPosition, direction.normalized, out hit, direction.magnitude)) {
            // If a collision is detected, adjust the target position to the point of collision
            targetPosition = hit.point;
			}
			if (direction.magnitude<0.1f)
			{newTransform.position = targetPosition;}
			else
			{newTransform.position=originalPosition+0.1f*direction.normalized;}
			//this.transform.position=targetPosition
			yield return null; // Wait for next frame
		}
	}



}
