using UnityEngine;
using System.Collections.Generic;

public class CTriggerPressurePlate : CTriggerBase {
		
	void Update () {

	}
	
	public void OnTriggerEnter(Collider other)
    {
        Debug.Log ("Pressure Plate Activate: " + name);
		state = true;	
    }
	
    public void OnTriggerExit(Collider other)
    {
        Debug.Log ("Pressure Plate Deactive: " + name);
		state = false;
    }
}
