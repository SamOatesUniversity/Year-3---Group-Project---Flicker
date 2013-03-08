using UnityEngine;
using System.Collections;

public enum CrowState {
	Resting,				//!< The player is stood still
	Takeoff,				//!< The player is walking
	Flying
};

[RequireComponent (typeof (MathHelpers))]

public class CEntityCrow : MonoBehaviour {
	
	private Vector3 	m_velocity;
	private CrowState	m_state;
	private bool		m_startled;
	private bool		m_startedTakeoff;
	private Animation 	m_animation = null;
	private string 		m_currentAnimation = null;
	private float		m_timeTookoff = 0.0f;
	private bool 		m_doneTakeoffMathCalcs = false;
	
	private Vector3 	m_flightStart;
	private Vector3		m_flightEnd;
	private Vector3		m_flightControl1;
	private Vector3		m_flightControl2;
	
	private Vector3 	m_lastPos;
	
	public float 		Speed = 0.01f;
	public float 		TakeoffTime = 1.0f;
	public float		EndDistance = 5.0f;
	public float		Point1Offset = 2.0f;
	public float		Point2Offset = 2.0f;
	public float		YOffset = 3.0f;
	
	public MathHelpers	MathHelp = null;
	
	// Use this for initialization
	void Start () {
		m_startled = false;
		m_state = CrowState.Resting;
		m_velocity = new Vector3(0.0f, 0.0f, 0.0f);
		m_startedTakeoff = false;
		m_animation = GetComponentInChildren<Animation>();
		MathHelp = GetComponent<MathHelpers>();
		m_lastPos = this.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void FixedUpdate()
	{	
		Debug.Log(m_currentAnimation);
		DoAnimations();
		
		if( m_startled && !m_doneTakeoffMathCalcs )	
		{
			m_doneTakeoffMathCalcs = true;
			m_flightStart = this.transform.position;
			
			Vector3 offsetOrigin = new Vector3(0.0f, this.transform.position.y - YOffset, 0.0f);
			Vector3 originToBird = this.transform.position - offsetOrigin;
			Vector3 playerToBird = this.transform.position - CEntityPlayer.GetInstance().transform.position;
			Vector3 endDirection = originToBird.normalized + playerToBird.normalized;
			endDirection.Normalize();
			m_flightEnd = endDirection * EndDistance;
			m_flightControl1 = this.transform.forward.normalized + endDirection;
			m_flightControl1.Normalize();
			m_flightControl2 = m_flightControl1 + endDirection;
			m_flightControl2.Normalize();
			
			m_flightControl1 *= Point1Offset;
			m_flightControl2 *= Point2Offset;
		}
		if( m_state == CrowState.Flying || m_state == CrowState.Takeoff )
		{
			float t = Time.time - m_timeTookoff;
			t *= Speed;
			this.transform.position = MathHelp.Bezier3(m_flightStart, m_flightControl1, m_flightControl2, m_flightEnd, t);
			
			
			Vector3 basicLookDirection = this.transform.position + ((this.transform.position-m_lastPos) * 20);
			this.transform.LookAt( basicLookDirection + m_flightEnd );
		}
	}
	
	void DoAnimations()
	{
		if( m_state == CrowState.Takeoff && m_timeTookoff + TakeoffTime < Time.time )
		{
			m_state = CrowState.Flying;	
		}
		
		if (m_state == CrowState.Resting)
		{	
			m_currentAnimation = "idle";
			if (!m_animation.IsPlaying("idle"))
			{
				m_animation.CrossFade("idle", 0.2f);
			}
		}
		else if (m_state == CrowState.Takeoff)
		{
			
			m_currentAnimation = "takeoff";
			if (!m_animation.IsPlaying("takeoff") && !m_startedTakeoff)
			{
				m_startedTakeoff = true;
				m_animation["takeoff"].speed = 0.5f;
				m_animation.CrossFade("takeoff", 0.2f);
			}
			else if (!m_animation.IsPlaying("takeoff"))
			{
				m_startedTakeoff = false;
				m_state = CrowState.Flying;
			}
			
			/*
			m_currentAnimation = "takeoff";
			if (!m_animation.IsPlaying("takeoff"))
			{
				m_animation.CrossFade("takeoff", 0.2f);
			}
			*/
		}
		else if (m_state == CrowState.Flying)
		{
			m_currentAnimation = "flight";
			if (!m_animation.IsPlaying("flight"))
			{
				m_animation.CrossFade("flight", 0.2f);
			}
		}
	}
	
	void OnTriggerEnter(Collider collider)
	{
		if(collider.gameObject.name == "Ledge_Grab_Detection")
		{
			//Player should be detected now
			m_startled = true;
			m_state = CrowState.Takeoff;
			m_timeTookoff = Time.time;
			Debug.Log("Crow startled!");
		}
	}
}