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
		if (m_canWallJump && Input.GetKeyDown(KeyCode.Space) && m_lastWallJumpObject != null)
		{
			physics.Body.AddForce(new Vector3(0.0f, PlayerWallJumpHeight, 0.0f), ForceMode.Impulse);	
			physics.Velocity = (-physics.Direction);
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
	public void CallOnCollisionEnter(CEntityPlayer player, Collision collision, ref PlayerState playerState)
	{
		if (playerState != PlayerState.Jumping)
			return;
		
		CSceneObject sceneObject = collision.gameObject.GetComponent<CSceneObject>();
		if (sceneObject && (m_lastWallJumpObject == null || m_lastWallJumpObject != sceneObject) && sceneObject.CanWallJump)
		{
			m_canWallJump = true;
			m_lastWallJumpObject = sceneObject;
			m_wallJumpPoint = player.transform.position;
			playerState = PlayerState.WallJumping;
		}
		else
		{
			m_canWallJump = false;	
		}
	}
	
	/*
	 * \brief Called when the player leaves a collosion
	*/
	public void CallOnCollisionExit(Collision collision, ref PlayerState playerState)
	{
		m_canWallJump = false;	
		m_startWallTime = Time.time;
		m_lastWallJumpObject = null;
	}
	
	/*
	 * \brief Called whilst a collision is taking place
	*/
	public void CallOnCollisionStay(Collision collision, ref CPlayerPhysics physics, ref PlayerState playerState)
	{
		// push the user off the wall, cos they didnt jump in time
		float ms = (Time.time - m_startWallTime) * 1000.0f;
		if (physics.IsColliding && ms >= WallHangTime)
		{
			physics.Velocity = (-physics.Direction) * 0.1f;
			m_startWallTime = Time.time;	
			playerState = PlayerState.Standing;
		}
	}
}
