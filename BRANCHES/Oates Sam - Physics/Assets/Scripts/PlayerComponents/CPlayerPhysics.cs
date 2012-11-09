using UnityEngine;
using System.Collections;

public enum CollisionState {
	None,
	OnFloor,
	OnWall,
	OnRoof
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
	
	private Rigidbody		m_body = null;							//!< The rigid body component of this entity 
	
	private int				m_direction = 0;						//!< Stores the direction, 0 = not moving, 1 = left, -1 = right
	
	private int				m_movingDirection = 0;					//!< Stores the direction, 0 = not moving, 1 = left, -1 = right

	private CollisionState	m_collisionState = CollisionState.None;	//!< 
	
	private JumpState		m_jumpState = JumpState.Landed;			//!< 

	/* ----------------
	    Public Members 
	   ---------------- */		
	
	public float			PlayerJumpHeight = 5.0f;		//!< The amount of force (in the y-axis) jump is represented by
	
	public float			AccelerationRate = 0.05f;		//!< The rate of acceleration
	
	public float			MaxSpeed = 0.5f;				//!< The maximum speed of the player 
	
	/*
	 * \brief Initialise anything we don't know at construct time
	*/
	public void Create(Rigidbody body)
	{
		m_body = body;
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
	
	/*
	 * \brief Gets the players last known direction
	*/
	public float Direction {
		get {
			return m_direction;
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
	
	/*
	 * \brief Called whilst a collision is taking place
	*/
	public void CallOnCollisionStay(Collision collision, ref PlayerState playerState, float playerAlpha)
	{		
		m_collisionState = CollisionState.None;
		
		foreach (ContactPoint contact in collision)
		{
			float yContact = contact.point.y - m_body.position.y;
			if (yContact >= 0.2 && yContact <= 0.25)
			{
				Debug.DrawRay(contact.point, contact.normal);
				if (isFacingCollision(m_movingDirection, m_body.transform.position, contact.point, playerAlpha))
				{
					playerState = PlayerState.LedgeHang;
					m_body.constraints = RigidbodyConstraints.FreezeAll;
				}
			}
			else if (isNearly(contact.normal.y, 1.0f, 0.2f))
			{
				m_collisionState = CollisionState.OnFloor;
			}
			else if (isNearly(contact.normal.y, -1.0f, 0.2f))
			{
				m_collisionState = CollisionState.OnRoof;
			}
			else
			{
				if (isFacingCollision(m_movingDirection, m_body.transform.position, contact.point, playerAlpha)) {
					m_collisionState = CollisionState.OnWall;
					break;
				}
			}
		}	
		
		if (m_collisionState == CollisionState.OnWall && m_jumpState == JumpState.Jumping)
		{
			m_velocity = -(m_movingDirection * 0.25f);
		}
	}
	
	/*
	 * \brief Called on player update
	*/
	public void OnFixedUpdate(ref PlayerState playerState)
	{
		float velocity = Input.GetAxis("Horizontal") * MaxSpeed;
		int direction = isNearly(velocity, 0.0f, 0.1f) ? 0 : velocity > 0 ? 1 : -1;
		
		// Ledge hanging code start
		if (playerState == PlayerState.LedgeHang || playerState == PlayerState.LedgeClimb || playerState == PlayerState.LedgeClimbComplete) {
			
			m_velocity = 0.0f;
			
			// If we are trying to move in the opposite way of the wall, fall off the wall
			if (direction != 0 && direction != m_movingDirection) {
				m_velocity = velocity;
				playerState = PlayerState.Walking;
				m_body.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
				return;
			} 
			
			// if the player is not already climbing the wall, check keys
			if (playerState != PlayerState.LedgeClimb)
			{
				// if the user pressed up, climb the wall
				if (Input.GetKeyDown(KeyCode.UpArrow)) {
					playerState = PlayerState.LedgeClimb;
				}
				// if the user pressed space, jump off the wall
				else if (Input.GetKeyDown(KeyCode.Space)) {
					m_body.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
					m_velocity = -(m_movingDirection); // kick it away from the wall
					m_body.AddForce(new Vector3(0, PlayerJumpHeight, 0), ForceMode.Impulse);	
					m_jumpState = JumpState.Jumping;
					playerState = PlayerState.Jumping;
					m_collisionState = CollisionState.None;
					m_movingDirection *= -1;
					m_direction *= -1;
				}
				return;
			}

			return;
		}
		// Ledge hanging code end
		
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
		if (Input.GetKeyDown(KeyCode.Space) && m_jumpState == JumpState.Landed && m_collisionState == CollisionState.OnFloor)
		{
			m_body.AddForce(new Vector3(0, PlayerJumpHeight, 0), ForceMode.Impulse);	
			m_jumpState = JumpState.Jumping;
			playerState = PlayerState.Jumping;
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
		Vector3 collisionVector = (playerPosition - collisionPoint);
		collisionVector.Normalize();
		float collisionDir = Mathf.Atan2(collisionVector.z, collisionVector.x);
		
		if (collisionDir < 0) collisionDir = -1; else collisionDir = 1;
		
		if (alpha > 180 && alpha < 360)
			return collisionDir == playerDirection;
		else 
			return collisionDir != playerDirection;
		
		return false;
	}
}
