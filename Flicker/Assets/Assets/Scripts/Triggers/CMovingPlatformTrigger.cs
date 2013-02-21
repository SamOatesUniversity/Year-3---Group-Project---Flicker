using UnityEngine;
using System.Collections.Generic;

public class CMovingPlatformTrigger : CTriggerBase {
		
	private bool 							m_triggerEntered = false;
	
	// Update is called once per frame
	void FixedUpdate () {
		if (m_triggerEntered)
        {
			if (CheckContextButton())
            {
				SetState(true);								
                Animation lever = GetComponentInChildren<Animation>();
                if (lever != null)
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
