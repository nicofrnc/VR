using UnityEngine;

public class PuzzleController : MonoBehaviour {

	public enum HandType : int { LeftHand, RightHand };
	[Header( "Hand Properties" )]
	public HandType handType;
	[Header( "Player Controller" )]
	public MainPlayerController playerController;
	
	static protected PuzzleObject[] anchors_in_the_scene;
	protected bool is_hand_closed_previous_frame = false;
	protected PuzzleObject object_grasped = null;
	
	void Start () 
	{
		if ( anchors_in_the_scene == null ) anchors_in_the_scene = GameObject.FindObjectsOfType<PuzzleObject>();//find and store puzzle objects
	}
	
	void Update () {
		bool hand_closed = is_hand_closed();
		if ( hand_closed == is_hand_closed_previous_frame ) return;
		is_hand_closed_previous_frame = hand_closed;
		
		if ( hand_closed )//if hand gets closed, try to grab object
			{
			int best_object_id = -1;
			float best_object_distance = float.MaxValue;
			float oject_distance;
			
			for ( int i = 0; i < anchors_in_the_scene.Length; i++ ) //determine best object to grab
			{
				if ( !anchors_in_the_scene[i].is_available() ) continue;
				oject_distance = Vector3.Distance( this.transform.position, anchors_in_the_scene[i].transform.position );
				if ( oject_distance < best_object_distance && oject_distance <= anchors_in_the_scene[i].get_grasping_radius() ) {
					best_object_id = i;
					best_object_distance = oject_distance;
				}
			}
			
			if ( best_object_id != -1 ) //if found then grab object
			{
				object_grasped = anchors_in_the_scene[best_object_id];
				object_grasped.attach_to( this );
			}
		} 
		
		else if ( object_grasped != null ) {object_grasped.detach_from( this );}//if hand gets opened, try to drop object
	}

	protected bool is_hand_closed () {
		if ( handType == HandType.LeftHand ) return
			OVRInput.Get( OVRInput.Button.Three )                           
			&& OVRInput.Get( OVRInput.Button.Four )                         
			&& OVRInput.Get( OVRInput.Axis1D.PrimaryHandTrigger ) > 0.5     
			&& OVRInput.Get( OVRInput.Axis1D.PrimaryIndexTrigger ) > 0.5;   
		else return
			OVRInput.Get( OVRInput.Button.One )                             
			&& OVRInput.Get( OVRInput.Button.Two )                          
			&& OVRInput.Get( OVRInput.Axis1D.SecondaryHandTrigger ) > 0.5   
			&& OVRInput.Get( OVRInput.Axis1D.SecondaryIndexTrigger ) > 0.5; 
	}

	
}
