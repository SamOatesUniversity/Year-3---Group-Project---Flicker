using UnityEngine;
using System.Collections;

public class CSceneObjectPlatform : CSceneObjectBase {
		
	/* -----------------
	    Private Members 
	   ----------------- */
	
	private float m_deltaA;			//!< Rotation delta
	
	private float m_lastRotY;
	
	public bool m_enabled = false;
	
	private Animation m_platAnim = null;
	
	public bool isLevel1_3 = false;
	private bool m_reversePlatform = false;
	
	public bool OnIsOn = false;
	
	// Use this for initialization
	void Start() 
    {
		m_platAnim = gameObject.GetComponent<Animation>();
		m_lastRotY = gameObject.transform.rotation.eulerAngles.y;
		if (!m_enabled && m_platAnim != null)
		{
			if (m_platAnim.clip == null || m_platAnim[m_platAnim.clip.name] == null) {
				return;
			}
			
			m_platAnim.Play();
			m_platAnim[m_platAnim.clip.name].speed = 0.0f;
		}
		
	}
	
	// Update is called once per frame
	void Update() 
    {
		
	}
		
	public void ToggleTrigger()
	{
		m_enabled = !m_enabled;
	}
	
	void FixedUpdate()
	{		
		if (m_platAnim == null || m_platAnim.clip == null || m_platAnim[m_platAnim.clip.name] == null)
			return;
		
		if (m_enabled == true)
		{
			float rotY = m_platAnim.transform.rotation.eulerAngles.y;
			m_deltaA += rotY - m_lastRotY;
			m_lastRotY = rotY;
		}
		
		if (isLevel1_3 && m_platAnim[m_platAnim.clip.name].speed != 0.0f)
		{
			if (m_platAnim[m_platAnim.clip.name].normalizedTime >= 0.97f) 
			{
				m_enabled = false;
				m_reversePlatform = true;
			}
			else if (m_platAnim[m_platAnim.clip.name].normalizedTime == 0.0f)
			{
				m_enabled = false;
				m_reversePlatform = false;
			}
		}
		
		if (m_platAnim[m_platAnim.clip.name].speed != 0.0f && !m_enabled)
		{
			m_platAnim[m_platAnim.clip.name].speed = 0.0f;
			
		}	
		else if (m_platAnim[m_platAnim.clip.name].speed == 0.0f && m_enabled)
		{
			m_platAnim[m_platAnim.clip.name].speed = m_reversePlatform ? -1.0f : 1.0f;
		}
	}
	
	public float DeltaA {
		get {
			return m_deltaA;
		}
	}
		
	public void resetDeltaA() {
		m_deltaA = 0.0f;	
	}
	
	public override void LogicStateChange(bool newState) {
		m_enabled = newState;
		if (OnIsOn)
			m_enabled = true;
	}
}