using System;
using UnityEngine;
using System.Collections;

public class CLogicObject : MonoBehaviour {
		
	protected bool						m_state;				//!< The current state of the logic object
	
	// Use this for initialization
	void Start () {
		m_state = false;					
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	/*
	*	\brief Get the state of the logic object 
	*/
	public bool GetState() {
		return m_state;	
	}
	
	/*
	*	\brief Set the state of the logic object
	*/
	public void SetState(
			bool state
		)
	{
		m_state = state;
		
		Debug.Log (this.name + " Logic State Set To " + state);
		
	}
}
