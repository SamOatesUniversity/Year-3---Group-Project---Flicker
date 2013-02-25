using UnityEngine;
using System.Collections;

public enum LadderState {
	None,
	AtBase,
	OnBase,
	OnMiddle,
	OnTop,
	JumpingOff
};

public class CPlayerLadder {
	
	public LadderState						state = LadderState.None;
	
	public bool								moving = false;
	public float							offset = 0.0f;
	public float							direction = 0.0f;
	
}
