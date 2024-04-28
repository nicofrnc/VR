using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchObject : InteractiveObject
{
	public GameObject testcube;
	private bool is_on=false;

    void Start()
    {
		myType = ObjectType.Torch;//what type of interactive object this is
		testcube.SetActive(false);
		is_on=false;
    }
    void Update()
    {	
    }
	public void toggle_visibility()
	{
		testcube.SetActive(!testcube.activeSelf);
		is_on=!is_on;
	}
}
