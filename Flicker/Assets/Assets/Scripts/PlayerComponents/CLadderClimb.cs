using UnityEngine;
using System.Collections;

public enum LadderState {
	None,
	AtBase,
	AtMiddle,
	AtTop
}

public class CLadderClimb : MonoBehaviour {

	private LadderState		m_ladderState = LadderState.None;		//!< 
	
	public float 			Offset = 0.0f;
	
	public LadderState State {
		get {
			return m_ladderState;	
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
			Offset = (climb * 0.05f); 
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
