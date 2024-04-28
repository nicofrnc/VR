using UnityEngine;

public class WandObject : MonoBehaviour {

	[Header( "Grasping Properties" )]
	public float graspingRadius = 0.1f;
	

	
	protected Transform initial_transform_parent;
	protected WandController wandController = null;	
	
	private GameObject hitObject=null;//object being pointed at
	
	private GameObject previousHitObject=null;
	private InteractiveObject interactiveObject=null; //object being interacted with
	private float max_distance_to_target=100;
	private bool visibleRay=false;
	private LineRenderer lineRenderer;
	
	public bool is_available () { return wandController == null; }//utility methods
	public float get_grasping_radius () { return graspingRadius; }
	public GameObject get_hitObject () { return hitObject; }
	public InteractiveObject get_interactiveObject () { return interactiveObject; }
	public void set_visibleRay(bool visible_ray) { visibleRay=visible_ray; }

	void Start () {
		initial_transform_parent = transform.parent;
		lineRenderer ??= gameObject.AddComponent<LineRenderer>();
		lineRenderer.startWidth = 0.05f;
		lineRenderer.endWidth = 0.05f;
		lineRenderer.enabled = false;
	}
	
	void Update()
	{
		if(visibleRay)//renders ray and updates hitObject
		{
			DisplayRay();
			lineRenderer.enabled = true;
		}		
		else
		{
			hitObject=null;//no hit object
			lineRenderer.enabled = false;//no line
		}
		
		if (hitObject != previousHitObject)//if  hitObject changed, then deselect current interactiveObject and select new one
		{
			if (interactiveObject!=null){interactiveObject.DeselectObject();}
			interactiveObject=hitObject.GetComponent<InteractiveObject>();
			if (interactiveObject!=null){interactiveObject.SelectObject();}
		}
		Material material = lineRenderer.material;
		if(interactiveObject!=null){material.SetColor("_Color", Color.blue);}
		else{material.SetColor("_Color", Color.red);}
		
		
		previousHitObject = hitObject;//updates previousHitObject
	}
	
	public void attach_to ( WandController wandController ) {
		this.wandController = wandController;
		transform.position=wandController.transform.position;
		transform.rotation=Quaternion.LookRotation(wandController.transform.forward,wandController.transform.up);
		transform.rotation *= Quaternion.Euler(90f, 0f, 0f);
		transform.SetParent( wandController.transform );
	}

	public void detach_from ( WandController wandController ) {
		if ( this.wandController != wandController ) return;
		this.wandController = null;
		transform.SetParent( initial_transform_parent );
	}
	
	public void DisplayRay() 
	{	
		Vector3 rayOrigin = transform.position;
		Vector3 rayDirection = wandController.transform.forward;
		float distance_to_target=100;
		RaycastHit hit;
		
		if (Physics.Raycast(rayOrigin, rayDirection, out hit))//determine hitObject if any
		{
			distance_to_target=Vector3.Distance(hit.point, rayOrigin);
			if(distance_to_target<max_distance_to_target){hitObject = hit.collider.gameObject;}
			else{hitObject=null;}
		}
		else{hitObject=null;}
		
        Vector3[] positions = { rayOrigin, rayOrigin + rayDirection.normalized * distance_to_target };//renders line
        lineRenderer.positionCount = 2;
        lineRenderer.SetPositions(positions);
    }
}

