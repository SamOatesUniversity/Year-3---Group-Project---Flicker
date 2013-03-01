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
	
	private Animation m_platAnim = null;
	
	
	
	
	
	public bool OutputDebug = false;
	public int yOffset = 0; 
	
	private ArrayList m_information = new ArrayList();
	
	
	
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
		
		m_platAnim = gameObject.GetComponent<Animation>();
		if (m_platAnim == null || m_platAnim["Take 001"] == null)
		{
			Debug.LogError("Falling Platform '" + name + "' is missing required animations!");
		}
	}
	
	// Update is called once per frame
	void Update() 
    {
	}
	
	void FixedUpdate()
	{
		if (m_state == PlatformState.Normal)
		{
			m_platAnim["Take 001"].normalizedTime = 0.10f;
			m_platAnim.Stop();
			return;
		}
				
		if(m_state == PlatformState.Shaking)
		{
			if(m_platAnim["Take 001"].normalizedTime >= 0.39f)
			{
				m_platAnim["Take 001"].normalizedTime = 0.10f;
			}
			
			if(m_timeTriggered+timeToFall < Time.time)
			{
				m_state = PlatformState.Falling;
				m_platAnim["Take 001"].normalizedTime = 0.40f;
				if (m_audio != null)
				{
					m_audio.Play();	
				}
			}
		}
		else if(m_state == PlatformState.Falling)
		{
			if(m_platAnim["Take 001"].normalizedTime >= 0.5f)
			{
				m_platAnim.Stop();
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
					m_platAnim["Take 001"].normalizedTime = 0.70f;
					m_platAnim.Play();
				}
			}
		}
		else if(m_state == PlatformState.Resetting)
		{
			if(m_platAnim["Take 001"].normalizedTime >= 0.97f)
			{
				m_state = PlatformState.Normal;
				m_platAnim["Take 001"].normalizedTime = 0.0f;
			}
			else if (m_timeDown + resetTime + 1 < Time.time)
			{
				m_state = PlatformState.Normal;
				m_platAnim["Take 001"].normalizedTime = 0.10f;
			}
		}
	}
	
	void OnTriggerEnter(Collider collider)
	{
		if(m_state == PlatformState.Normal)
		{
			m_state = PlatformState.Shaking;
			m_timeTriggered = Time.time;			
			m_platAnim["Take 001"].normalizedTime = 0.10f;
			m_platAnim.Play();
		}
	}
	
	void OnGUI ()
	{
		
		if (!OutputDebug)
			return;
		
		m_information.Clear();
		m_information.Add("Platform State: " + m_state);
		m_information.Add("Trigger Time: " + m_timeTriggered);
		m_information.Add("Anim Time: " + m_platAnim["Take 001"].normalizedTime);
		m_information.Add("------------------------------------");
		
		Rect labelPosition = new Rect(Screen.width - 256, 4 + (yOffset * ((m_information.Count + 1) * 20)), 256, 512);
		
		foreach (string info in m_information)
		{
			GUI.Label(labelPosition, info);
			labelPosition.y += 20;
		}
		
	}
}
