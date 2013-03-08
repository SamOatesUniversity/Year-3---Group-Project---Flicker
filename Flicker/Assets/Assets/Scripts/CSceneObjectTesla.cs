using UnityEngine;
using System.Collections;

public class CSceneObjectTesla : CSceneObject {

	public bool m_enabled = false;
	private GameObject m_electricObject = null;
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
