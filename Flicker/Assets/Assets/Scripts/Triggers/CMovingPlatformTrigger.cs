using UnityEngine;
using System.Collections.Generic;

public class CMovingPlatformTrigger : CTriggerBase {
	
	public enum eLevelType {
		SingleUse,
		InstantReset,
		Toggle,
		MultiUse
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
	
	private float 							m_timePulled = 0.0f;
	public float							TimeDelaySecs = 0.1f;
	public float 							ResetTime = 4.0f;
	
	new void Start() {
	
		m_animation = GetComponent<Animation>();
		if (m_animation == null || m_animation["Take 001"] == null)
		{
			m_animation = transform.parent.GetComponent<Animation>();
			if (m_animation == null || m_animation["Take 001"] == null)
			{
				Debug.LogError("The lever '" + name + "' has no animation!");	
				return;
			}
		}
		
		m_animation["Take 001"].speed = 0.0f;
		m_animation.Play("Take 001");
		
	}
	
	// Update is called once per frame
	new void Update () {
		
		if (m_leverState == eLevelState.InUse && LeverType == eLevelType.MultiUse && (Time.time - m_timePulled > ResetTime))
		{
			m_leverState = eLevelState.ReadyForUse;	
			state = false;
		}
		
		if (m_triggerEntered)
        {						
			if (m_animation != null && (Time.time - m_timePulled > TimeDelaySecs && m_leverState == eLevelState.InUse))
			{
				m_animation["Take 001"].speed = 1.0f;
			}
			
			if (m_animation != null && m_leverState == eLevelState.Reseting)
			{
				m_animation["Take 001"].speed = -1.0f;
			}
			
			bool pulled = CheckContextButton();
			if (pulled && m_leverState == eLevelState.ReadyForUse)
            {
				CEntityPlayer player = CEntityPlayer.GetInstance();
				player.SetPlayerState(PlayerState.PullingWallLeverDown);
				
				state = true;
				m_leverState = eLevelState.InUse;
				m_timePulled = Time.time;
			}
			else if (LeverType == eLevelType.Toggle && pulled && m_leverState == eLevelState.Finished)
			{
				CEntityPlayer player = CEntityPlayer.GetInstance();
				player.SetPlayerState(PlayerState.PullingWallLeverUp);
				
				state = false;
				m_leverState = eLevelState.Reseting;
				m_timePulled = Time.time;
			}
        }
		
		if (m_animation == null) {
			return;
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
			else if (LeverType == eLevelType.MultiUse)
			{
				m_leverState = eLevelState.ReadyForUse;	
				state = false;
			}
			
			CEntityPlayer player = CEntityPlayer.GetInstance();
			player.SetPlayerState(PlayerState.Standing);
		}
		
		if (m_leverState == eLevelState.Reseting && m_animation["Take 001"].normalizedTime < 0.1f)
		{
			m_animation["Take 001"].speed = 0.0f;
			m_leverState = eLevelState.ReadyForUse;
			
			CEntityPlayer player = CEntityPlayer.GetInstance();
			player.SetPlayerState(PlayerState.Standing);
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
