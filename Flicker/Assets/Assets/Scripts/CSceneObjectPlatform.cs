using UnityEngine;
using System.Collections;

public class CSceneObjectPlatform : CSceneObject {
		
	/* -----------------
	    Private Members 
	   ----------------- */
	
	private float m_deltaA;			//!< Rotation delta
	
	private float m_lastRotY;
	
	public bool m_enabled = false;
	Animation platAnim = null;
	// Use this for initialization
	void Start() 
    {
		platAnim = gameObject.GetComponent<Animation>();
		m_lastRotY = gameObject.transform.rotation.eulerAngles.y;
		if (!m_enabled && platAnim != null)
		{
			platAnim.Stop();
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
		if (platAnim == null)
			return;

		float rotY = platAnim.transform.rotation.eulerAngles.y;
		m_deltaA += rotY - m_lastRotY;
		m_lastRotY = rotY;
		
		if (!m_enabled && platAnim.isPlaying)
		{
			platAnim.Stop();
		}	
		else if (!platAnim.isPlaying && m_enabled)
		{
			platAnim.Play();
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
	
	public override void LogicSuccess() {
		m_enabled = true;
	}
}