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
	public CSceneObject										obj = null;
	
	// Use this for initialization
	void Start () {
		obj = GetComponent<CSceneObject>();
	}
	
	// Update is called once per frame
	void Update () {
	
		if (GetOuput() == true)
		{
			obj.LogicSuccess();
		}
		
	}
	
	/*
	*	\brief Returns the result of the expressions
	*/
	public bool GetOuput() {		
		return expression.Resolve();
	}	
}
