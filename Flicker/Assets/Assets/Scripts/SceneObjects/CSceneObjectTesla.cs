using UnityEngine;
using System.Collections;

public class CSceneObjectTesla : CSceneObject {
	
	public bool IsTimerDriven			= false;
	public float TimeOn					= 2.0f;
	public float TimeOff				= 1.5f;
	public bool m_enabled 				= false;
	private GameObject m_electricObject = null;
	private float m_activityTimer		= 0.0f;
	// Use this for initialization
	void Start ()
	{
		m_electricObject = this.transform.FindChild("Electricity").gameObject;
		m_electricObject.active = m_enabled;
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
	
	//fixed update
	void FixedUpdate()
	{
		if( IsTimerDriven )
		{
			m_activityTimer += Time.deltaTime;
			if( m_enabled )
			{
				if( m_activityTimer >= TimeOn )
				{
					m_enabled = false;
					m_electricObject.active = m_enabled;
					m_activityTimer = 0.0f;
				}
			}
			else
			{
				if( m_activityTimer >= TimeOff )
				{
					m_enabled = true;
					m_electricObject.active = m_enabled;
					m_activityTimer = 0.0f;
				}
			}
		}
	}
	
	public override void LogicStateChange(bool newState)
	{
		m_enabled = newState;
		if( m_electricObject )
		{
			m_electricObject.active = m_enabled;
		}	
	}
	
}
