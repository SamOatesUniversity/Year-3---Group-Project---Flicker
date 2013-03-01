using UnityEngine;
using System.Collections.Generic;

public class CMovingPlatformTrigger : CTriggerBase {
	
	public enum eLevelType {
		SingleUse,
		InstantReset,
		Toggle
	};
	
	enum eLevelState {
		ReadyForUse,
		InUse,
		Reseting,
		Finished
	};
	
	private bool 							m_triggerEntered = false;
	private Animation						m_animation = null;
	private eLevelState						m_leverState = eLevelState.ReadyForUse;	
	
	public eLevelType						LeverType = eLevelType.SingleUse;		
	
	new void Start() {
	
		m_animation = GetComponent<Animation>();
		if (m_animation == null || m_animation["Take 001"] == null)
		{
			Debug.LogError("The lever '" + name + "' has no animation!");	
		}
		
		m_animation["Take 001"].speed = 0.0f;
		m_animation.Play("Take 001");
		
	}
	
	// Update is called once per frame
	new void Update () {

		if (m_triggerEntered)
        {
			bool pulled = CheckContextButton();
			if (pulled && m_leverState == eLevelState.ReadyForUse)
            {
				Debug.Log("PULLING LEVER " + name);
				
				state = true;
				m_animation["Take 001"].speed = 1.0f;                
				m_leverState = eLevelState.InUse;
			}
			else if (LeverType == eLevelType.Toggle && pulled && m_leverState == eLevelState.Finished)
			{
				state = false;
				m_animation["Take 001"].speed = -1.0f;	
				m_leverState = eLevelState.Reseting;
			}
        }
		
		if (m_leverState == eLevelState.InUse && m_animation["Take 001"].normalizedTime > 0.9f)
		{			
			if (LeverType == eLevelType.InstantReset)
			{
				m_animation["Take 001"].speed = -1.0f;
				m_leverState = eLevelState.Reseting;
			}
			else if (LeverType == eLevelType.Toggle)
			{
				m_animation["Take 001"].speed = 0.0f;
				m_leverState = eLevelState.Finished;	
			}
		}
		
		if (m_leverState == eLevelState.Reseting && m_animation["Take 001"].normalizedTime < 0.1f)
		{
			m_animation["Take 001"].speed = 0.0f;
			m_leverState = eLevelState.ReadyForUse;
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
