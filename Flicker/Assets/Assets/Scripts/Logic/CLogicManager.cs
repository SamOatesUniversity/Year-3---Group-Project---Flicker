using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable] 
public enum LogicOperator {
	AND,
	OR
};

[System.Serializable] 
public class CLogicManager : MonoBehaviour {

	public CLogicExpression 								expression = null;			//! The equation to manage and test
	private CSceneObject									obj = null;
	
	private bool											m_oldState = false;
	private bool											m_currentState = false;
	
	// Use this for initialization
	void Start () {
		obj = GetComponent<CSceneObject>();
	}
	
	// Update is called once per frame
	void Update () {
		
		if (expression.name == "platform_expression_4")
		{
			Debug.Log("m_oldState: " + m_oldState);
			Debug.Log("m_currentState: " + m_currentState);
		}
		
		m_oldState = m_currentState;
		m_currentState = expression.Resolve();
		
	
		if (m_oldState != m_currentState)
		{
			Debug.Log("Sent Pulse to " + obj.name);
			obj.LogicStateChange(m_currentState);
		}
		
	}
}
