using UnityEngine;
using System.Collections;

public class CTriggerSteamVentLever : CTriggerBase {
	
	enum eLeverState {
		ReadyForUse,
		InUse,
		Reseting,
		Finished
	};
	
	private bool 							m_triggerEntered = false;
	private Animation						m_animation = null;
	private eLeverState						m_leverState = eLeverState.ReadyForUse;
	private float 							m_timePulled = 0.0f;
	
	public GameObject						ForcedGameObject = null;
	
	// Use this for initialization
	void Start () {
		if (ForcedGameObject != null)
		{
			m_animation = ForcedGameObject.GetComponent<Animation>();
		}
		
		m_animation["Take 001"].speed = 0.0f;
		m_animation.Play("Take 001");
		
		state = false;
	}
	
	// Update is called once per frame
	void Update () {
		
		if (m_leverState == eLeverState.InUse && m_animation["Take 001"].normalizedTime > 0.9f)
		{
			m_leverState = eLeverState.Reseting;
			m_animation["Take 001"].speed = -1.0f;
			
			CEntityPlayer player = CEntityPlayer.GetInstance();
			player.SetPlayerState(PlayerState.Standing);
		}
		
		if (m_leverState == eLeverState.Reseting && m_animation["Take 001"].normalizedTime <= 0.1f)
		{
			m_leverState = eLeverState.ReadyForUse;
			m_animation["Take 001"].speed = 0.0f;
			state = false;
		}
	
		if (m_triggerEntered)
		{
			bool pulled = CheckContextButton();
			if (pulled && m_leverState == eLeverState.ReadyForUse)
			{				
				CEntityPlayer player = CEntityPlayer.GetInstance();
				player.SetPlayerState(PlayerState.PullingWallLeverDown);
				
				state = true;
				m_leverState = eLeverState.InUse;
				m_timePulled = Time.time;
				m_animation["Take 001"].speed = 1.0f;
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
