using UnityEngine;
using System.Collections;

public class CSceneObjectPlatform : CSceneObject {
		
	/* -----------------
	    Private Members 
	   ----------------- */
	
	private float m_deltaA;			//!< Rotation delta
	
	private float m_lastRotY;
	
	public bool m_enabled = false;
	
	private Animation m_platAnim = null;
	
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
		
		if (m_platAnim[m_platAnim.clip.name].speed == 1.0f && !m_enabled)
		{
			Debug.Log ("STOP PLAT");
			m_platAnim[m_platAnim.clip.name].speed = 0.0f;
			
		}	
		else if (m_platAnim[m_platAnim.clip.name].speed == 0.0f && m_enabled)
		{
			Debug.Log ("start plat");
			m_platAnim[m_platAnim.clip.name].speed = 1.0f;
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
	}
}