using UnityEngine;
using System.Collections;

public class CSceneObject : MonoBehaviour {
	
	/* ----------------
	    Public Members 
	   ---------------- */
	
	public bool CanWallJump 			= true;			//!< Can a player wall jump on this object?
	
	public bool IsLadder				= false; 			//!< Is the object a ladder?
	
	public float ExtraSlide				= 1.0f; 			//!< Can the player wall jump upon this object?
	
	public bool CanLedgeGrab			= true;			//!< Can the player ledge grab on the object?

	// Use this for initialization
	void Start() 
    {
	
	}
	
	// Update is called once per frame
	void Update() 
    {
	
	}

}
