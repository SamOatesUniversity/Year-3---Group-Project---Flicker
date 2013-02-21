using UnityEngine;
using System.Collections.Generic;

public class CMovingPlatformTrigger : CTriggerBase {
		
	private bool 							m_triggerEntered = false;
	
	// Update is called once per frame
	void Update () {
		if (m_triggerEntered)
        {
			if (CheckContextButton())
            {
				Debug.Log ("Pulled Lever: " + name);
				state = true;							
                Animation lever = GetComponentInChildren<Animation>();
                if (lever != null && lever["pull"] != null)
				{
                    lever.Play("pull");
				}
			}
        }
	}
	
	public void OnTriggerEnter(Collider other)
    {
        m_triggerEntered = true;
    }
	
    public void OnTriggerExit(Collider other)
    {
        m_triggerEntered = false;
    }
}
