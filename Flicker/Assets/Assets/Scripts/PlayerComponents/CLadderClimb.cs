using UnityEngine;
using System.Collections;

public enum LadderState {
	None,
	AtBase,
	AtMiddle,
	AtTop,
	JumpOff
}

public class CLadderClimb : MonoBehaviour {

	private LadderState		m_ladderState = LadderState.None;		//!< 
	
	public float 			Offset = 0.0f;
	
	public LadderState State {
		get {
			return m_ladderState;	
		}
		set {
			m_ladderState = value;	
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void CallOnUpdate(CollisionState collisionState) 
	{
		if (m_ladderState != LadderState.None) 
		{
			float climb = Input.GetAxis("Vertical");
			Offset = (climb * 0.025f); 
		}
		else {
			Offset = 0.0f;	
		}
		
		if (m_ladderState == LadderState.AtTop && Offset > 0.0f) {
			Offset = 0.0f;	
		}
		
		if (collisionState == CollisionState.OnFloor && Offset < 0.0f) {
			Offset = 0.0f;	
		}
		
		if (Input.GetButton("Jump") && m_ladderState != LadderState.None) {
			m_ladderState = LadderState.JumpOff;
			Offset = 0.0f;	
		}
	}
	
	public void CallOnTriggerStay(Collider collider, ref PlayerState playerState)
	{
		string state = collider.gameObject.name;
				
		if (state == "LadderBASE") {
			m_ladderState = LadderState.AtBase;
		} else if (state == "LadderMID") {
			m_ladderState = LadderState.AtMiddle;
		} else if (state == "LadderTOP") {
			m_ladderState = LadderState.AtTop;
		}
	}
	
	public void CallOnTriggerExit(Collider collider, ref PlayerState playerState)
	{
		if (Offset == 0.0f) {
			m_ladderState = LadderState.None;	
		}
	}
}
