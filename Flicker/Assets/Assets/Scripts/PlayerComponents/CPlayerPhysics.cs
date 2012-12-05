using UnityEngine;
using System.Collections;

public enum CollisionState {
	None,
	OnFloor,
	OnWall,
	OnRoof,
	OnLadder
}

public enum JumpState {
	Landed,
	Jumping
}

public class CPlayerPhysics : MonoBehaviour {
	
	/* -----------------
	    Private Members 
	   ----------------- */
	
	private float			m_velocity = 0.0f;						//!< The current speed of the player
	
	private float			m_platformVelocity = 0.0f;				//!< The velocity of a platform the player is standing on, if any
	
	private Rigidbody		m_body = null;							//!< The rigid body component of this entity 
	
	private int				m_direction = 0;						//!< Stores the direction, 0 = not moving, 1 = left, -1 = right
	
	private int				m_movingDirection = 0;					//!< Stores the direction, 0 = not moving, 1 = left, -1 = right

	private CollisionState	m_collisionState = CollisionState.None;	//!< 
	
	private JumpState		m_jumpState = JumpState.Landed;			//!< 
	
	private CEntityPlayer	m_player = null;
	
	private Vector3			m_ledgeOffset = new Vector3(0.0f, -0.42f, 0.0f);
	
	private CWallJump		m_wallJump = null;						//!< 
	
	private bool 			m_isJumpDown = false;					//!< 
	
	private int				m_invert = 1;							//!< 	
	
	private CLadderClimb	m_ladderClimb = null;					//!< 
	
	private float			m_jumpTimer = 0;						//!< The time the player last jumped

	/* ----------------
	    Public Members 
	   ---------------- */		
	
	public float			PlayerJumpHeight = 5.0f;		//!< The amount of force (in the y-axis) jump is represented by
	
	public float			AccelerationRate = 0.05f;		//!< The rate of acceleration
	
	public float			MaxSpeed = 0.5f;				//!< The maximum speed of the player 
	
	public int				JumpDelayMS = 500;				//!< The minimum required time to pass between jumps in miliseconds
	
	/*
	 * \brief Initialise anything we don't know at construct time
	*/
	public void Create(CEntityPlayer player, Rigidbody body)
	{
		m_body = body;
		m_player = player;
		m_wallJump = player.GetComponent<CWallJump>();
		m_ladderClimb = new CLadderClimb();
		
		m_invert = player.MainCamera.GetComponent<CCamera>().DistanceFromPlayer > 0 ? 1 : -1;
	}
	
	/*
	 * \brief Gets tand sets the players velocity
	*/
	public float Velocity {
		get {
			return m_velocity;
		}
		set {
			m_velocity = value;	
		}
	}
	
	public CollisionState CurrentCollisionState {
		get {
			return m_collisionState;
		}
	}
	
	public CLadderClimb LadderClimb {
		get {
			return m_ladderClimb;	
		}
	}
	
	/*
	 * \brief Gets the players last known direction
	*/
	public int Direction {
		get {
			return m_direction;
		}
		set {
			m_direction = value;
			m_movingDirection = value;
		}
	}
	
	/*
	 * \brief Gets the players last known moving direction
	*/
	public float MovingDirection {
		get {
			return m_movingDirection;
		}
	}
	
	/*
	 * \brief Gets the players rigid body object
	*/
	public Rigidbody Body {
		get {
			return m_body;	
		}
	}
	
	/*
	 * \brief Returns if we are currently in a collision
	*/
	public CollisionState CollisionType {
		get {
			return m_collisionState;
		}
	}
	
	/*
	 * \brief Gets the players last known moving direction
	*/
	public JumpState JumpType {
		get {
			return m_jumpState;
		}
		set {
			m_jumpState = value;
		}
	}
	
	public int Invert {
		get { 
			return m_invert;
		}
	}
	
	/*
	 * \brief Works out if a value is almost another value (for floating point accuracy)
	*/
	public void CallOnCollisionEnter(Collision collision)
	{
		m_collisionState = CollisionState.None;
		
		foreach (ContactPoint contact in collision)
		{			
			if (isNearly(contact.normal.y, 1.0f, 0.2f))
			{
				m_collisionState = CollisionState.OnFloor;
			}
			else if (isNearly(contact.normal.y, -1.0f, 0.2f))
			{
				m_collisionState = CollisionState.OnRoof;
			}
			else
			{
				m_collisionState = CollisionState.OnWall;
			}
		}
		
		if (m_collisionState == CollisionState.OnFloor)
		{
			m_jumpState = JumpState.Landed;	
		}
	}
	
	/*
	 * \brief Called when the player leaves a collosion
	*/
	public void CallOnCollisionExit(Collision collision)
	{
		m_collisionState = CollisionState.None;
	}
	
	public void CallOnTriggerStay(Collider collider, ref PlayerState playerState)
	{
		CSceneObject obj = collider.gameObject.GetComponent<CSceneObject>();
		if (obj == null && collider.gameObject != null && collider.gameObject.transform.parent != null) {
			GameObject parent = collider.gameObject.transform.parent.gameObject;
			if (parent != null) {
				obj = parent.GetComponent<CSceneObject>();
			}
		}
					
		if (obj != null && obj.IsLadder) {
			m_ladderClimb.CallOnTriggerStay(collider, ref playerState);
			if (m_ladderClimb.State != LadderState.None) {
				m_body.constraints = RigidbodyConstraints.FreezeAll;
			}
			else
			{
				m_collisionState = CollisionState.OnFloor;	
			}
			return;
		}
	}
	
	public void CallOnTriggerExit(Collider collider, ref PlayerState playerState)
	{
		if (m_ladderClimb.State != LadderState.None) {
			m_ladderClimb.CallOnTriggerExit(collider, ref playerState);
			m_body.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
		}
	}
	
	/*
	 * \brief Called whilst a collision is taking place
	*/
	public void CallOnCollisionStay(Collision collision, ref PlayerState playerState, ref float playerAlpha)
	{		
		m_collisionState = CollisionState.None;
				
		foreach (ContactPoint contact in collision)
		{
			CSceneObject obj = contact.otherCollider.gameObject.GetComponent<CSceneObject>();
			if (obj == null) {
				GameObject parent = contact.otherCollider.gameObject.transform.parent.gameObject;
				if (parent != null) {
					obj = parent.GetComponent<CSceneObject>();
				}
			}
			
			float yContact = contact.point.y - m_body.position.y;
			
			// ledge grabbing
			if (yContact >= 0.05f && yContact <= 0.3f && playerState != PlayerState.LedgeHang && contact.normal.y > -0.5f && (obj != null && obj.CanLedgeGrab))
			{			
				if (isFacingCollision(m_movingDirection, m_body.transform.position, contact.point, playerAlpha))
				{
					m_velocity = 0;
					playerState = PlayerState.LedgeHang;
					m_body.constraints = RigidbodyConstraints.FreezeAll;
					m_jumpState = JumpState.Landed;						
					m_ledgeOffset.x = m_movingDirection > 0 ? 0.1f : -0.1f;
					transform.Find("Player").localPosition = m_ledgeOffset;
					continue;
				}
			}
			
			// wall jumping
			if (obj != null && obj.CanWallJump == true && m_jumpState != JumpState.Landed && !isNearly(contact.normal.y, 1.0f, 0.2f) && !isNearly(contact.normal.y, -1.0f, 0.1f))
			{
				m_collisionState = CollisionState.OnWall;
				playerState = PlayerState.WallJumpStart;
				m_jumpTimer = (Time.time * 1000.0f); 
				m_body.constraints = RigidbodyConstraints.FreezeAll;
				m_velocity = 0.0f;
				m_wallJump.StartHangTime = Time.time * 1000.0f;
			}
			// floor check
			else if (isNearly(contact.normal.y, 1.0f, 0.2f))
			{
				m_collisionState = CollisionState.OnFloor;
			}
			// head check
			else if (isNearly(contact.normal.y, -1.0f, 0.1f))
			{
				m_collisionState = CollisionState.OnRoof;
			}
			// wall check
			else
			{
				if (isFacingCollision(m_movingDirection, m_body.transform.position, contact.point, playerAlpha)) {
					m_collisionState = CollisionState.OnWall;
					break;
				}
			}
			
			CSceneObjectPlatform platform = contact.otherCollider.gameObject.GetComponent<CSceneObjectPlatform>();
			if (platform != null) {
				m_platformVelocity += platform.DeltaA;
				//print( "m_platformVelocity: " + m_platformVelocity );
			}
		}	
		
		if (m_collisionState == CollisionState.OnWall && m_jumpState == JumpState.Jumping && playerState != PlayerState.WallJumpStart)
		{
			m_velocity = -(m_movingDirection * 0.15f);
		}
		
		//print ( "m_velocity is: " + m_velocity );
	}
	
	/*
	 * \brief Called on player update
	*/
	public void OnFixedUpdate(ref PlayerState playerState)
	{		
		if (playerState == PlayerState.FallingFromTower)
			return;
			
		float velocity = ((Input.GetAxis("Horizontal") * MaxSpeed) * m_invert) + m_platformVelocity;
		int direction = isNearly(velocity, 0.0f, 0.1f) ? 0 : velocity > 0 ? 1 : -1;
		
		// reset platformVelocity
		m_platformVelocity = 0.0f;
		
		// Ledge hanging code start
		if (playerState == PlayerState.LedgeHang || playerState == PlayerState.LedgeClimb || playerState == PlayerState.LedgeClimbComplete) {
			
			m_velocity = 0.0f;
			
			// If we are trying to move in the opposite way of the wall, fall off the wall
			if (direction != 0 && direction != m_movingDirection) {
				m_velocity = velocity;
				playerState = PlayerState.Walking;
				m_jumpState = JumpState.Landed;
				m_body.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
				transform.Find("Player").localPosition = new Vector3(0.0f, -0.28f, 0.0f);
				return;
			} 
			
			// if the player is not already climbing the wall, check keys
			if (playerState != PlayerState.LedgeClimb)
			{
				// if the user pressed up, climb the wall
				if (Input.GetAxis("Vertical")>0) {
					playerState = PlayerState.LedgeClimb;
					m_jumpState = JumpState.Landed;
					transform.Find("Player").localPosition = new Vector3(0.0f, -0.28f, 0.0f);
				}
				// if the user pressed space, jump off the wall
				else if (m_isJumpDown) {
					m_body.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
					m_velocity = -(m_movingDirection); // kick it away from the wall
					m_body.AddForce(new Vector3(0, PlayerJumpHeight, 0), ForceMode.Impulse);	
					playerState = PlayerState.Jumping;
					m_collisionState = CollisionState.None;
					m_movingDirection *= -1;
					m_direction *= -1;
					transform.Find("Player").localPosition = new Vector3(0.0f, -0.28f, 0.0f);
				}
				return;
			}

			return;
		}
		// Ledge hanging code end
		
		if (playerState == PlayerState.WallJumpStart)
		{
			if ((Time.time * 1000.0f) - m_wallJump.StartHangTime > m_wallJump.WallHangTime) {
				m_body.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
				m_velocity = -m_movingDirection;
				playerState = PlayerState.Walking;
				m_jumpState = JumpState.Landed;
			} else if (Input.GetButton("Jump") && direction != 0 && direction != m_movingDirection) {
				m_body.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
				m_velocity = -(m_movingDirection); // kick it away from the wall
				m_body.AddForce(new Vector3(0, PlayerJumpHeight * 1.1f, 0), ForceMode.Impulse);	
				playerState = PlayerState.Jumping;
				m_jumpTimer = (Time.time * 1000.0f); 
				m_collisionState = CollisionState.None;
				m_movingDirection *= -1;
				m_direction *= -1;	
			}
			return;
		}
						
		if (!(m_collisionState == CollisionState.OnWall && m_jumpState == JumpState.Jumping))
			m_velocity = velocity;	
		
		m_direction = direction;
		
		if (m_collisionState != CollisionState.None && m_jumpState != JumpState.Jumping)
		{
			if (m_direction == 0)
			{
				playerState = PlayerState.Standing;
			}
			else
			{
				m_movingDirection = m_direction;
				playerState = PlayerState.Walking;	
			}
		}
	}
	
	public void OnUpdate(ref PlayerState playerState)
	{	
		if (Input.GetButton("Jump") && m_jumpState == JumpState.Landed && m_collisionState == CollisionState.OnFloor)
		{
			if ((Time.time * 1000.0f) - m_jumpTimer > JumpDelayMS)
			{
				m_jumpTimer = (Time.time * 1000.0f);
				m_body.AddForce(new Vector3(0, PlayerJumpHeight, 0), ForceMode.Impulse);	
				m_jumpState = JumpState.Jumping;
				playerState = PlayerState.Jumping;
			}
		}		
		
		if (m_jumpState == JumpState.Jumping && playerState == PlayerState.Jumping)
		{
			if ((Time.time * 1000.0f) - m_jumpTimer > 500.0f)
			{
				playerState = PlayerState.FallJumping;
			}
		}
		
		m_ladderClimb.CallOnUpdate(m_collisionState);
		if (m_ladderClimb.State == LadderState.JumpOff && m_jumpState != JumpState.Jumping)
		{
			m_jumpTimer = (Time.time * 1000.0f);
			m_body.AddForce(new Vector3(0, PlayerJumpHeight, 0), ForceMode.Impulse);	
			m_jumpState = JumpState.Jumping;
			playerState = PlayerState.Jumping;
			m_ladderClimb.State = LadderState.None;
			m_velocity = m_direction;
			m_body.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
			Debug.Log("Jump Twat");
		}
				
	}
	
	/*
	 * \brief Works out if a value is almost another value (for floating point accuracy)
	*/
	public static bool isNearly(float x, float amount, float varience) 
	{
		if (x < amount - varience) return false;
		if (x > amount + varience) return false;
		return true;
	}

	/*
	 * \brief Work out if a point is in the same direction as the player
	*/
	public static bool isFacingCollision(int playerDirection, Vector3 playerPosition, Vector3 collisionPoint, float alpha)
	{		
		return true; // disabled funtion for now, the maths is a little off
		
		Vector3 collisionVector = (playerPosition - collisionPoint);
		collisionVector.Normalize();
		float collisionDir = Mathf.Atan2(collisionVector.z, collisionVector.x);
		
		if (collisionDir < 0) collisionDir = -1; else collisionDir = 1;
		
		if (alpha > 180 && alpha < 360)
			return collisionDir == playerDirection;
		
		return collisionDir != playerDirection;
	}
}
