using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveObject : MonoBehaviour
{
    public enum ObjectType { Torch, Puzzle }
    protected ObjectType myType;
    private bool isSelected = false;
    private Renderer objectRenderer;
	public ObjectType get_my_type(){return myType;}
	public void toggle_visibility(){}
	
    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
    }
	
    public void SelectObject()
    {
        isSelected = true;
        ApplyGlowEffect();
    }
    public void DeselectObject()
    {
        isSelected = false;
        RemoveGlowEffect();
    }
    private void ApplyGlowEffect()
    {
        if (objectRenderer != null)
        {
            objectRenderer.material.SetColor("_EmissionColor", Color.white * 0.5f);
        }
    }
    private void RemoveGlowEffect()
    {
        if (objectRenderer != null)
        {
            objectRenderer.material.SetColor("_EmissionColor", Color.black);
        }
    }
}
