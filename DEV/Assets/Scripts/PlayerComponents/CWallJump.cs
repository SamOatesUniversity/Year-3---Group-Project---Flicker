using UnityEngine;
using System.Collections;

public class CWallJump : MonoBehaviour {
	
	/* -----------------
	    Private Members 
	   ----------------- */

	private CSceneObject	m_lastWallJumpObject = null;			//!< The last scene object a player wall jumped from
	
	private float			m_startWallTime = 0;					//!< The start time that the player initiated a wall jump
	
	private bool 			m_canWallJump = false;					//!< States wheter a player can wall jump
	
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
	
	/*
	 * \brief called by the players update method
	*/	
	public void onUpdate (ref CPlayerPhysics physics, ref PlayerState playerState)
	{
		if (m_canWallJump && Input.GetKeyDown(KeyCode.Space))
		{
			physics.Body.AddForce(new Vector3(0.0f, PlayerWallJumpHeight, 0.0f));	
			physics.Velocity = (-physics.Direction) * 0.5f;
			playerState = PlayerState.Jumping;
		}
	}
		
	/*
	 * \brief Resets all wall jump memebers to there inital state
	*/
	public void Reset() 
	{
		m_canWallJump = false;
		m_startWallTime = Time.time;
		m_lastWallJumpObject = null;
	}
	
	/*
	 * \brief Called when the player enters a collosion
	*/
	public void CallOnCollisionEnter(Collision collision)
	{
		CSceneObject sceneObject = collision.gameObject.GetComponent<CSceneObject>();
		if (sceneObject && (m_lastWallJumpObject == null || m_lastWallJumpObject != sceneObject) && sceneObject.CanWallJump)
		{
			m_canWallJump = true;
			m_lastWallJumpObject = sceneObject;
		}
	}
	
	/*
	 * \brief Called when the player leaves a collosion
	*/
	public void CallOnCollisionExit(Collision collision)
	{
		m_canWallJump = false;	
	}
	
	/*
	 * \brief Called whilst a collision is taking place
	*/
	public void CallOnCollisionStay(Collision collision, ref CPlayerPhysics physics)
	{
		// push the user off the wall, cos they didnt jump in time
		float ms = (Time.time - m_startWallTime) * 1000.0f;
		if (physics.IsColliding && ms >= WallHangTime)
		{
			physics.Velocity = (-physics.Direction) * 0.1f;
			m_startWallTime = Time.time;	
		}
	}
}
