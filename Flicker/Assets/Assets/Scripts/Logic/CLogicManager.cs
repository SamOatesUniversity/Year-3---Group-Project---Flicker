using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum LogicOperator {
	AND,
	OR
};

public class CLogicManager : MonoBehaviour {

	public CLogicEquation 								equation = new CLogicEquation();			//! The equation to manage and test

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	/*
	*	\brief Returns the result of the expressions
	*/
	public bool GetOuput() {		
		return equation.Resolve();
	}
	
	/*
	*	\brief Get the equation we are managing
	*/	
	public CLogicEquation GetEquation() {
		return equation;	
	}
	
}
