using UnityEngine;
using System.Collections;

public class CWallJump : MonoBehaviour {
	
	/* -----------------
	    Private Members 
	   ----------------- */

	private CSceneObject	m_lastWallJumpObject = null;			//!< The last scene object a player wall jumped from
	
	private float			m_startWallTime = 0;					//!< The start time that the player initiated a wall jump
	
	private bool 			m_canWallJump = false;					//!< States wheter a player can wall jump
	
	private Vector3			m_wallJumpPoint = Vector3.zero;			//!< 
	
	/* ----------------
	    Public Members 
	   ---------------- */	
	
	public float 			PlayerWallJumpHeight = 200.0f;			//!< The y-axis force applied when jumpin off a wall
	
	public int				WallHangTime = 3000;					//!< The number of milliseconds a player will hang onto a wall before being pushed off
		
	/*
	 * \brief Gets if the player can preform a wall jump
	*/		
	public bool GetCanWallJump() {
		return m_canWallJump;	
	}
	
	public Vector3 GetWallJumpPoint() {
		return m_wallJumpPoint;
	}
	
	/*
	 * \brief called by the players update method
	*/	
	public void onUpdate (ref CPlayerPhysics physics, ref PlayerState playerState)
	{

	}
		
	/*
	 * \brief Resets all wall jump memebers to there inital state
	*/
	public void Reset() 
	{

	}
	
	/*
	 * \brief Called when the player enters a collosion
	*/
	public void CallOnCollisionEnter(CEntityPlayer player, Collision collision, ref PlayerState playerState)
	{

	}
	
	/*
	 * \brief Called when the player leaves a collosion
	*/
	public void CallOnCollisionExit(Collision collision, ref PlayerState playerState)
	{

	}
	
	/*
	 * \brief Called whilst a collision is taking place
	*/
	public void CallOnCollisionStay(Collision collision, ref CPlayerPhysics physics, ref PlayerState playerState)
	{

	}
}
