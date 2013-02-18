using System;
using UnityEngine;
using System.Collections;

public class CLogicBase : MonoBehaviour {
	
	[Serializable]
	public enum LogicOperator {
		And,
		Or,
		Nand,
		Nor
	};
	
	[Serializable]
	public class LogicInput {
		public GameObject logicObject;
		public bool[] logicOperator = new bool[4];
	}
	
	public LogicInput[]							LogicInputs;
	
	// Use this for initialization
	void Start () {
	
		
			
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
