using System;
using UnityEngine;
using System.Collections;

[System.Serializable]
public class CLogicObject : MonoBehaviour {
		
	public bool										state;				//!< The current state of the logic object
	
	// Use this for initialization
	void Start () {
		state = false;					
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
