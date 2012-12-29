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
	
	private float 			m_velocityLockTimer = 0;				//!< The time that velocity was locked
	
	private CSceneObjectPlatform	m_platform = null;				//!< The platform, if any, the player is standing on
	
	private GameObject		m_ledgeGrabBox = null;

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
		
		m_ledgeGrabBox = transform.Find("Ledge_Grab_Detection").gameObject;
		
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
	
	public float PlatformVelocity {
		get {
			return m_platformVelocity;
		}
		set {
			m_platformVelocity = value;	
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
			m_platform = contact.otherCollider.gameObject.GetComponent<CSceneObjectPlatform>();
			if (m_platform != null) {
				m_platform.resetDeltaA();
			}
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
			m_player.SetPlayerState(PlayerState.Standing);
			m_ledgeGrabBox.collider.enabled = true;
		}
	}
	
	/*
	 * \brief Called when the player leaves a collosion
	*/
	public void CallOnCollisionExit(Collision collision)
	{
		m_collisionState = CollisionState.None;
		m_platform = null;
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
					
		if (obj != null && obj.IsLadder && (m_ladderClimb.State != LadderState.JumpOff)) {
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
			Debug.DrawRay(contact.point, contact.normal);
			
			CSceneObject obj = null;
			if (contact.otherCollider)
			{
				obj = contact.otherCollider.gameObject.GetComponent<CSceneObject>();
				if (obj == null && contact.otherCollider.gameObject.transform.parent != null) {
					GameObject parent = contact.otherCollider.gameObject.transform.parent.gameObject;
					if (parent != null) {
						obj = parent.GetComponent<CSceneObject>();
					}
				}
			}
			
			float yContact = contact.point.y - m_body.position.y;
			
			if (contact.thisCollider != null && contact.thisCollider.gameObject.name == "Ledge_Grab_Detection")
			{
				if(CSceneObject.CheckLedgeGrab(collision))
					continue;
			}
			
			if (contact.otherCollider != null && contact.otherCollider.gameObject.name == "Ledge_Grab_Detection")
			{
				if (CSceneObject.CheckLedgeGrab(collision))
					continue;
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
			else if (isNearly(contact.normal.y, 1.0f, 0.8f))
			{
				m_collisionState = CollisionState.OnFloor;
				if (!isNearly(contact.normal.y, 1.0f, 0.15f) && collision.contacts.Length == 1)
				{
					m_velocity = (m_movingDirection * 0.15f);
					m_velocityLockTimer = (Time.time * 1000.0f); 
				}
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

		float velocity = (Input.GetAxis("Horizontal") * MaxSpeed) * m_invert;
		if ((Time.time * 1000.0f) - m_velocityLockTimer < 100)
		{
			velocity = m_velocity;
		}
		
		int direction = isNearly(velocity, 0.0f, 0.1f) ? 0 : velocity > 0 ? 1 : -1;
		
		//platform update
		if(m_platform) {
			m_platformVelocity += m_platform.DeltaA;
			m_platform.resetDeltaA();
		}
		
		// Ledge hanging code start
		if (playerState == PlayerState.LedgeHang || playerState == PlayerState.LedgeClimb || playerState == PlayerState.LedgeClimbComplete) {
			
			m_velocity = 0.0f;
			
			// If we are trying to move in the opposite way of the wall, fall off the wall
			if (direction != 0 && direction != m_movingDirection) {
				m_velocity = velocity * 4.0f;
				playerState = PlayerState.Walking;
				m_jumpState = JumpState.Landed;
				m_body.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
				m_ledgeGrabBox.collider.enabled = true;
				m_movingDirection *= -1;
				m_direction *= -1;
				m_collisionState = CollisionState.None;
				
				Transform t = m_ledgeGrabBox.transform;
				t.localPosition = new Vector3(m_direction > 0 ? -0.18f : 0.18f, t.localPosition.y, t.localPosition.z);
				return;
			} 
			
			// if the player is not already climbing the wall, check keys
			if (playerState != PlayerState.LedgeClimb)
			{
				// if the user pressed up, climb the wall
				if (Input.GetAxis("Vertical") > 0 && playerState != PlayerState.LedgeClimbComplete) {
					playerState = PlayerState.LedgeClimb;
					m_jumpState = JumpState.Landed;
					m_ledgeGrabBox.collider.enabled = true;
				}
				// if the user pressed space, jump off the wall
				else if (m_isJumpDown) {
					m_body.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
					m_velocity = -(m_movingDirection) * 4.0f; // kick it away from the wall
					m_body.AddForce(new Vector3(0, PlayerJumpHeight * 0.25f, 0), ForceMode.Impulse);	
					playerState = PlayerState.Jumping;
					m_collisionState = CollisionState.None;
					m_movingDirection *= -1;
					m_direction *= -1;
					m_ledgeGrabBox.collider.enabled = true;
					
					Transform t = m_ledgeGrabBox.transform;
					t.localPosition = new Vector3(m_direction > 0 ? -0.18f : 0.18f, t.localPosition.y, t.localPosition.z);
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
			} else if (m_isJumpDown && direction != 0 && direction != m_movingDirection) {
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
		m_isJumpDown = Input.GetButton("Jump");
		if (m_isJumpDown && m_jumpState == JumpState.Landed && CanJump(playerState))
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
			if ((Time.time * 1000.0f) - m_jumpTimer > 2000.0f)
			{
				playerState = PlayerState.FallJumping;
			}
		}
		
		if (m_ladderClimb.State != LadderState.None)
		{
			playerState = PlayerState.UpALadder;
			m_ladderClimb.CallOnUpdate(m_collisionState);
			if (m_ladderClimb.State == LadderState.JumpOff && m_jumpState != JumpState.Jumping)
			{
				m_jumpTimer = (Time.time * 1000.0f);
				m_body.AddForce(new Vector3(0, PlayerJumpHeight, 0), ForceMode.Impulse);	
				m_jumpState = JumpState.Jumping;
				playerState = PlayerState.Jumping;
				m_velocity = m_direction * 0.15f;
				m_body.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
				m_velocityLockTimer = (Time.time * 1000.0f); 
			}
		}
				
	}
	
	/*
	 * \brief Works out if a value is almost another value (for floating point accuracy)
	*/
	public static bool isNearly(float x, float amount, float varience) 
	{
		if (x <= amount - varience) return false;
		if (x >= amount + varience) return false;
		return true;
	}

	/*
	 * \brief Work out if a point is in the same direction as the player
	*/
	public static bool isFacingCollision(int playerDirection, Vector3 playerPosition, Vector3 collisionPoint, float alpha)
	{		
		return true; // disabled funtion for now, the maths is a little off
		
		/*
		Vector3 collisionVector = (playerPosition - collisionPoint);
		collisionVector.Normalize();
		float collisionDir = Mathf.Atan2(collisionVector.z, collisionVector.x);
		
		if (collisionDir < 0) collisionDir = -1; else collisionDir = 1;
		
		if (alpha > 180 && alpha < 360)
			return collisionDir == playerDirection;
		
		return collisionDir != playerDirection;
		*/
	}
	
	public void SetLedgeGrabState (CEntityPlayer player, PlayerState state)
	{
		if (state == PlayerState.LedgeHang)
		{
			m_velocity = 0;
			player.SetPlayerState(PlayerState.LedgeHang);
			m_body.constraints = RigidbodyConstraints.FreezeAll;
			m_jumpState = JumpState.Landed;				
		}
	}
	
	private bool CanJump(PlayerState playerState)
	{
		if (playerState == PlayerState.LedgeHang)
			return false;
		
		if (playerState == PlayerState.LedgeClimb)
			return false;
		
		if (playerState == PlayerState.LedgeClimbComplete)
			return false;
		
		return true;
	}
	
}
