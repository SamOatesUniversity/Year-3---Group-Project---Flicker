using UnityEngine;
using System.Collections.Generic;

public class CTriggerPressurePlate : CTriggerBase {
	
	public void OnTriggerEnter(Collider other)
    {
        Debug.Log ("Pressure Plate Activate: " + name);
		state = true;	
		
		Animation anim = GetComponent<Animation>();
		if (anim != null)
		{
			anim.Play();	
		}
    }
	
    public void OnTriggerExit(Collider other)
    {
        Debug.Log ("Pressure Plate Deactive: " + name);
		state = false;
    }
}
