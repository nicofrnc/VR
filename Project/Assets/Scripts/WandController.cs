using UnityEngine;

public class WandController : MonoBehaviour {

	public enum HandType : int { LeftHand, RightHand };
	[Header( "Hand Properties" )]
	public HandType handType;
	[Header( "Player Controller" )]
	public MainPlayerController playerController;
	static protected WandObject[] anchors_in_the_scene;
	
	protected WandObject wand_grasped = null;// wand that is currently in hand
	protected Transform proxy_controller_transform=null;
	protected PuzzleObject object_grasped = null;
	public WandObject get_wand(){return wand_grasped;}
	//public GameObject testcube;
	//public GameObject testcube2;
	//public GameObject testcube3;
	
	void Start () 
	{
		if ( anchors_in_the_scene == null ) anchors_in_the_scene = GameObject.FindObjectsOfType<WandObject>();	
		//testcube.SetActive(false);		
	}

	void Update () 
	{
		
		if (try_to_grab_wand() && wand_grasped == null)//grab wand
		{
			int best_object_id = -1;
			float best_object_distance = float.MaxValue;
			float oject_distance;
			for ( int i = 0; i < anchors_in_the_scene.Length; i++ ) {
				if ( !anchors_in_the_scene[i].is_available() ) continue;
				oject_distance = Vector3.Distance( this.transform.position, anchors_in_the_scene[i].transform.position );
				if ( oject_distance < best_object_distance && oject_distance <= anchors_in_the_scene[i].get_grasping_radius() ) {
					best_object_id = i;
					best_object_distance = oject_distance;
				}
			}
			if ( best_object_id != -1 ) {
				wand_grasped = anchors_in_the_scene[best_object_id];
				Debug.LogWarningFormat( "{0} grasped {1}", this.transform.parent.name, wand_grasped.name );
				wand_grasped.attach_to( this );
			}			
		}
		
		if (try_to_release_wand() && wand_grasped != null)//release wand
		{
			wand_grasped.detach_from( this );
			wand_grasped=null;
		}
		
		if(wand_grasped != null){wand_grasped.set_visibleRay( visible_ray());}//set ray visible
		
		if(try_to_light_torch() && wand_grasped != null)//light torch
		{
			InteractiveObject interactiveObject=wand_grasped.get_interactiveObject ();//object pointed at by wand
			if(interactiveObject!=null && interactiveObject.get_my_type()==InteractiveObject.ObjectType.Torch)
			{
				TorchObject torchObject =(TorchObject) interactiveObject;
				torchObject.toggle_visibility();
			}	
		}
		
		if(object_grasped!=null && try_to_release_puzzle())//release puuzle
		{
			//testcube2.SetActive(!testcube2.activeSelf);
			object_grasped.detach_from_wand(this);
			object_grasped=null;
		}
		if(try_to_grab_puzzle() && wand_grasped != null && object_grasped==null)//grab puzzle
		{
			
			InteractiveObject interactiveObject=wand_grasped.get_interactiveObject ();//consider object pointed at by wand
			if(interactiveObject!=null && interactiveObject.get_my_type()==InteractiveObject.ObjectType.Puzzle)//if it's a puzzle then proceed
			{
				//testcube2.SetActive(!testcube2.activeSelf);//debugging
				//testcube.SetActive(!testcube.activeSelf);
				object_grasped=(PuzzleObject) interactiveObject;
				object_grasped.attach_to_wand( this );
				
				
			}					
		}

		//if(object_grasped==null){testcube3.SetActive(false);}
		//else{testcube3.SetActive(true);}
		
	}
	
/////////////////////////////commands

	protected bool try_to_grab_wand () //grabbing wand
	{
		if ( handType == HandType.LeftHand ) return
			OVRInput.Get( OVRInput.Button.Three );                 
		else return
			OVRInput.Get( OVRInput.Button.One );                   
	}
	protected bool try_to_release_wand ()//releasing wand
	{
		if ( handType == HandType.LeftHand ) return
			OVRInput.Get( OVRInput.Button.Four );                  
		else return
			OVRInput.Get( OVRInput.Button.Two );                   
	}
	protected bool try_to_grab_puzzle()//move puzzle piece
	{
		bool condition1=OVRInput.GetDown( OVRInput.Button.SecondaryIndexTrigger)&& OVRInput.Get( OVRInput.Button.PrimaryIndexTrigger);
		bool condition2=OVRInput.Get( OVRInput.Button.SecondaryIndexTrigger)&& OVRInput.GetDown( OVRInput.Button.PrimaryIndexTrigger);
		return condition1 || condition2;
	}	
	protected bool try_to_release_puzzle()//move puzzle piece
	{
		return !(OVRInput.Get( OVRInput.Button.PrimaryIndexTrigger)&& OVRInput.Get( OVRInput.Button.SecondaryIndexTrigger));

	}
	protected bool try_to_light_torch()//light torch
	{
		bool condition1=OVRInput.GetDown( OVRInput.Button.SecondaryIndexTrigger)&& OVRInput.Get( OVRInput.Button.PrimaryIndexTrigger);
		bool condition2=OVRInput.Get( OVRInput.Button.SecondaryIndexTrigger)&& OVRInput.GetDown( OVRInput.Button.PrimaryIndexTrigger);
		return condition1 || condition2;
	}
	protected bool visible_ray () //make ray visible
	{
		if ( handType == HandType.LeftHand ) return
			OVRInput.Get( OVRInput.Button.PrimaryIndexTrigger);                 
		else return
			OVRInput.Get( OVRInput.Button.SecondaryIndexTrigger );                   
	}
}	
        // foreach (OVRInput.Button button in desiredSequence)
        // {
            // if (OVRInput.GetDown(button))
            // {
                // if (button == desiredSequence[currentStep])
                // {
                    // if (Time.time - lastPressTime <= timeBetweenPresses)
                    // {
                        // currentStep++;
                        // if (currentStep == desiredSequence.Length)
                        // {
                            // Debug.Log("Sequence completed successfully!");
							// if(wand_grasped!=null){
								// wand_grasped.light_torch();
							// }
                            // ResetSequence();
                        // }
                    // }
                    // else
                    // {
                        // ResetSequence();
                    // }
                    // lastPressTime = Time.time;
                // }
                // else
                // {
                    // ResetSequence();
                // }
            // }
        // }
    

    // void ResetSequence()
    // {
        // currentStep = 0;
        // lastPressTime = 0.0f;
    // }	
	// private OVRInput.Button[] desiredSequence = { OVRInput.Button.PrimaryIndexTrigger};//, OVRInput.Button.SecondaryIndexTrigger, OVRInput.Button.PrimaryIndexTrigger, OVRInput.Button.SecondaryIndexTrigger };
	// private int currentStep = 0;
	// public float timeBetweenPresses = 1.0f;
	// private float lastPressTime = 0.0f;
	
///////////////////////////////////////////	


