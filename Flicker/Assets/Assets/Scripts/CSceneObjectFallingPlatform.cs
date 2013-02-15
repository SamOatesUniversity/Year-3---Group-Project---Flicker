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
	
	private AudioSource m_audio;				//!< The audio to play on fall
	
	// Use this for initialization
	void Start() 
    {
		m_state = PlatformState.Normal;
		m_timeDown = 0.0f;
		m_timeTriggered = 0.0f;
		
		m_audio = this.GetComponentInChildren<AudioSource>();
		if (m_audio == null)
		{
			Debug.LogWarning("Falling platform missing audio source!");	
		}
	}
	
	// Update is called once per frame
	void Update() 
    {
	}
	
	void FixedUpdate()
	{
		Animation platAnim = gameObject.GetComponent<Animation>();
		if (platAnim == null || platAnim["Take 001"] == null)
		{
			//Debug.Log("No animation on falling platform!"); 
			return;
		}
		if(m_state == PlatformState.Shaking)
		{
			if(platAnim["Take 001"].normalizedTime >= 0.39f)
			{
				platAnim["Take 001"].normalizedTime = 0.10f;
			}
			if(m_timeTriggered+timeToFall < Time.time)
			{
				m_state = PlatformState.Falling;
				platAnim["Take 001"].normalizedTime = 0.40f;
				if (m_audio != null)
				{
					m_audio.Play();	
				}
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
					platAnim["Take 001"].normalizedTime = 0.70f;
					platAnim.Play();
				}
			}
		}
		else if(m_state == PlatformState.Resetting)
		{
			if(platAnim["Take 001"].normalizedTime >= 0.97f)
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
			if (platAnim == null || platAnim["Take 001"] == null)
				return;
			
			platAnim["Take 001"].normalizedTime = 0.10f;
			platAnim.Play();
		}
	}
}
