using UnityEngine;
using System.Collections;

public enum PlatformState {
	Normal,
	Shaking,
	Falling,
	Down,
	Resetting
}

public class CSceneObjectFallingPlatform : CSceneObject {
		
	/* -----------------
	    Private Members 
	   ----------------- */
	
	private PlatformState m_state;				//!< platform state
	
	public bool automaticReset = true;			//!< flag determining if this platform resets automatically
	
	public float resetTime = 5.0f;				//!< Object position last frame
	
	public float timeToFall = 1.0f;				//!< time before the platform falls
		
	private float m_timeDown;					//!< time the platform fell
	
	private float m_timeTriggered;				//!< time the platform stepped on
	
	// Use this for initialization
	void Start() 
    {
		m_state = PlatformState.Normal;
		m_timeDown = 0.0f;
		m_timeTriggered = 0.0f;
	}
	
	// Update is called once per frame
	void Update() 
    {
		SkinnedMeshRenderer mesh = GameObject.Find("bWallConnect").GetComponent<SkinnedMeshRenderer>();
		Transform parentTransform = mesh.bones[0].transform;
		
		BoxCollider collider = gameObject.GetComponent<BoxCollider>();
		collider.transform.parent = parentTransform;
	}
	
	void FixedUpdate()
	{
		Animation platAnim = gameObject.GetComponent<Animation>();
		if (platAnim == null)
		{
			Debug.Log("No animation!"); 
		}
		if(m_state == PlatformState.Shaking)
		{
			if(platAnim["Take 001"].normalizedTime >= 0.25f)
			{
				platAnim["Take 001"].normalizedTime = 0.10f;
			}
			if(m_timeTriggered+timeToFall < Time.time)
			{
				m_state = PlatformState.Falling;
				platAnim["Take 001"].normalizedTime = 0.30f;
			}
		}
		else if(m_state == PlatformState.Falling)
		{
			if(platAnim["Take 001"].normalizedTime >= 0.5f)
			{
				platAnim.Stop();
				m_state = PlatformState.Down;
				m_timeDown = Time.time;
			}
		}
		else if(m_state == PlatformState.Down)
		{
			if(automaticReset)
			{
				if(m_timeDown+resetTime < Time.time)
				{
					m_state = PlatformState.Resetting;
					platAnim["Take 001"].normalizedTime = 0.71f;
					platAnim.Play();
				}
			}
		}
		else if(m_state == PlatformState.Resetting)
		{
			if(platAnim["Take 001"].normalizedTime >= 0.90f)
			{
				m_state = PlatformState.Normal;
				platAnim.Stop();
			}
		}
	}
	
	void OnTriggerEnter(Collider collider)
	{
		//Debug.Log("Collision entered"); 
		if(m_state == PlatformState.Normal)
		{
			m_state = PlatformState.Shaking;
			m_timeTriggered = Time.time;
			Animation platAnim = gameObject.GetComponent<Animation>();
			platAnim["Take 001"].normalizedTime = 0.10f;
			platAnim.Play();
		}
	}
}
