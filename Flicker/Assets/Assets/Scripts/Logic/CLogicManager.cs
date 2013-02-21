using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum LogicOperator {
	AND,
	OR
};

public class CLogicManager : MonoBehaviour {

	public CLogicExpression 								expression = null;			//! The equation to manage and test
	
	private CSceneObject									m_object = null;
	
	// Use this for initialization
	void Start () {
		m_object = GetComponent<CSceneObject>();
	}
	
	// Update is called once per frame
	void Update () {
	
		if (GetOuput() == true)
		{
			m_object.LogicSuccess();
		}
		
	}
	
	/*
	*	\brief Returns the result of the expressions
	*/
	public bool GetOuput() {		
		return expression.Resolve();
	}	
}
