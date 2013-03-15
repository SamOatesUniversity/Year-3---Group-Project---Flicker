using UnityEngine;
using System.Collections;

public class CGruntPhysics : MonoBehaviour {
	
	/* -----------------
	    Private Members 
	   ----------------- */
	
	private float			m_velocity = 0.0f;						//!< The current speed of the player
	
	private float			m_platformVelocity = 0.0f;				//!< The velocity of a platform the player is standing on, if any
	
	private Rigidbody		m_body = null;							//!< The rigid body component of this entity 
	
	private int				m_direction = 1;						//!< Stores the direction, 0 = not moving, 1 = left, -1 = right
	
	private int				m_movingDirection = 1;					//!< Stores the direction, 0 = not moving, 1 = left, -1 = right

	private CollisionState	m_collisionState = CollisionState.None;	//!< 
	
	private JumpState		m_jumpState = JumpState.Landed;			//!< 
	
	private CEntityGrunt	m_grunt = null;
	
	private bool 			m_isJumpDown = false;					//!< 
	
	private int				m_invert = 1;							//!< 	
	
	private float			m_jumpTimer = 0;						//!< The time the player last jumped
	
	private float 			m_velocityLockTimer = 0;				//!< The time that velocity was locked
	
	private CSceneObjectPlatform	m_platform = null;				//!< The platform, if any, the player is standing on
	
	private FootMaterial	m_footMaterial = FootMaterial.Stone;

	/* ----------------
	    Public Members 
	   ---------------- */		
	
	public float			PlayerJumpHeight = 5.0f;		//!< The amount of force (in the y-axis) jump is represented by
	
	public float			AccelerationRate = 0.05f;		//!< The rate of acceleration
	
	public float			MaxSpeed = 0.5f;				//!< The maximum speed of the player 
	
	public int				JumpDelayMS = 500;				//!< The minimum required time to pass between jumps in miliseconds
	
	public bool				InsideTower = true;
	
	public float 			PeacefulSpeed = 0.2f;
	
	public float			DetectedSpeed = 0.4f;
	
	/*
	 * \brief Initialise anything we don't know at construct time
	*/
	public void Create(CEntityGrunt grunt, Rigidbody body)
	{
		m_body = body;
		m_grunt = grunt;
		m_invert = InsideTower ? -1 : 1;
		m_direction = 1;
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
		set {
			m_collisionState = value;	
		}
	}

	
	public FootMaterial GetFootMaterial() {
		return m_footMaterial;	
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
		}
	}
	
	/*
	 * \brief Gets the players last known moving direction
	*/
	public int MovingDirection {
		get {
			return m_movingDirection;
		}
		set {
			m_movingDirection = value;	
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
	
	public bool IsOnPlatform()
	{
		return m_platform != null;	
	}
	
	/*
	 * \brief Works out if a value is almost another value (for floating point accuracy)
	*/
	public void CallOnCollisionEnter(Collision collision, bool playerDetected)
	{
		m_collisionState = CollisionState.None;
		
		foreach (ContactPoint contact in collision)
		{			
			m_platform = contact.otherCollider.gameObject.GetComponent<CSceneObjectPlatform>();
			if (m_platform != null) {
				m_platform.resetDeltaA();
			}
			else if (isNearly(contact.normal.y, 1.0f, 0.2f))
			{
				m_collisionState = CollisionState.OnFloor;
				
				// are we on a special material?
				m_footMaterial = FootMaterial.Stone;
				if (contact.otherCollider.tag == "Wood Object")
					m_footMaterial = FootMaterial.Wood;
				else if (contact.otherCollider.tag == "Metal Object")
					m_footMaterial = FootMaterial.Metal;
				
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
			if (m_jumpState != JumpState.Landed)
				m_grunt.GetPlayerAnimation().PlayFootstepAudio(m_footMaterial);
			
			m_jumpState = JumpState.Landed;	
		}
		if ( (m_collisionState == CollisionState.OnWall || collision.collider.gameObject.name=="GruntBarrier") &&  m_grunt.GetGruntState() != GruntState.Turning && m_grunt.GetGruntState() != GruntState.Attacking)
		{
			if( !playerDetected )
			{
				m_grunt.SetGruntState( GruntState.Turning );
				if( m_movingDirection == 1)
				{
					m_movingDirection = -1;
					m_direction = -1;
				}
				else if( m_movingDirection == -1 )
				{
					m_movingDirection = 1;
					m_direction = 1;
				}
			}
			else
			{
				m_grunt.SetGruntState( GruntState.Standing );
			}
		}
	}
	
	/*
	 * \brief Called when the player leaves a collosion
	*/
	public void CallOnCollisionExit(Collision collision)
	{
		//m_collisionState = CollisionState.None;
		m_platform = null;
	}
	
/*
	 * \brief Called whilst a collision is taking place
	*/
	public void CallOnCollisionStay(Collision collision, ref GruntState playerState, ref float playerAlpha)
	{		
		m_collisionState = CollisionState.None;
							
		foreach (ContactPoint contact in collision)
		{
			Debug.DrawRay(contact.point, contact.normal);
			//
			if (contact.otherCollider != null && contact.otherCollider.gameObject != null)
			{
				CSceneObjectPlatform platform = contact.otherCollider.gameObject.GetComponent<CSceneObjectPlatform>();
				if (platform != null && m_platform == null) {
					m_platform = platform;
					m_platform.resetDeltaA();
				}
			}
			
			//
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

			// floor check
			else if (isNearly(contact.normal.y, 1.0f, 0.8f))
			{
				m_collisionState = CollisionState.OnFloor;
				if (!isNearly(contact.normal.y, 1.0f, 0.15f) && collision.contacts.Length == 1)
				{
					if(m_grunt.GetGruntState() == GruntState.Walking)
					{
						if(m_grunt.GetGruntPlayerDetected())
						{
							m_velocity = (m_movingDirection * 0.6f);
						}
						else
						{
							m_velocity = (m_movingDirection * 0.15f);
						}
						m_velocityLockTimer = (Time.time * 1000.0f);	
					}
					 
				}
				if(contact.otherCollider)
				{
					// are we on a special material?
					m_footMaterial = FootMaterial.Stone;
					if (contact.otherCollider.tag == "Wood Object")
						m_footMaterial = FootMaterial.Wood;
					else if (contact.otherCollider.tag == "Metal Object")
						m_footMaterial = FootMaterial.Metal;
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
		
		if (m_collisionState == CollisionState.OnWall && m_jumpState == JumpState.Jumping && playerState != GruntState.WallJumpStart)
		{
			m_velocity = -(m_movingDirection * 0.15f);
		}
		
		if (m_collisionState == CollisionState.OnFloor && ((Time.time * 1000.0f) - m_jumpTimer > 200.0f))
		{
			m_jumpState = JumpState.Landed;	
			
			if (m_grunt.GetGruntState() != GruntState.Turning) m_grunt.SetGruntState(GruntState.Standing);
		}
	}
	
	public void CallOnTriggerStay(Collider collider, ref GruntState playerState)
	{
		CSceneObject obj = collider.gameObject.GetComponent<CSceneObject>();
		if (obj == null && collider.gameObject != null && collider.gameObject.transform.parent != null) {
			GameObject parent = collider.gameObject.transform.parent.gameObject;
			if (parent != null) {
				obj = parent.GetComponent<CSceneObject>();
			}
		}
		
		if (obj != null && obj.KillPlayerOnTouch)
		{
			//m_grunt.PushPlayerFromTower();
		}
	}
	
	public void CallOnTriggerExit(Collider collider, ref GruntState playerState)
	{
	}
		
	/*
	 * \brief Called on player update
	*/
	public void OnFixedUpdate(ref GruntState playerState, bool playerDetected)
	{		
		if (playerState == GruntState.FallingFromTower)
			return;
		
		GameObject player = GameObject.Find("Player Spawn");
		float playerAlpha = player.GetComponent<CEntityPlayer>().CurrentPlayerAlpha;
		float gruntAlpha = m_grunt.CurrentPlayerAlpha;
		
		if( playerDetected )
		{
			if( playerAlpha > gruntAlpha + 180 )
			{
				gruntAlpha += 360;
			}
			else if( playerAlpha + 180 < gruntAlpha )
			{
				gruntAlpha -= 360;
			}
			if( playerAlpha > gruntAlpha )
			{
				m_movingDirection = -1;
				m_direction = -1;
			} 
			else if ( gruntAlpha > playerAlpha )
			{
				m_movingDirection = 1;
				m_direction = 1;
			}
		}
		
	//	float velocity = (Input.GetAxis("Horizontal") * MaxSpeed) * m_invert;
	//	if (Application.platform == RuntimePlatform.Android)
	//		velocity = Input.acceleration.y;
		float velocity = 0.0f;
		if( m_movingDirection != 0 )
		{
			if( playerState != GruntState.Turning && playerState != GruntState.Attacking && playerState != GruntState.Standing )
			{
				if(m_grunt.GetGruntPlayerDetected())
				{
					velocity = (m_movingDirection * DetectedSpeed);
				}
				else
				{
					velocity = (m_movingDirection * PeacefulSpeed);
				}
			}
		}
			
		if ((Time.time * 1000.0f) - m_velocityLockTimer < 100)
		{
			velocity = m_velocity;
		}
					
		int direction = isNearly(velocity, 0.0f, 0.1f) ? 0 : velocity > 0 ? 1 : -1;
		
		//platform update
		if (m_platform && m_collisionState == CollisionState.OnFloor) {
			m_platformVelocity += m_platform.DeltaA;
			m_platform.resetDeltaA();
		}
		
						
		if (!(m_collisionState == CollisionState.OnWall && m_jumpState == JumpState.Jumping))
			m_velocity = velocity;	
				
		if (playerState != GruntState.Turning)
		{
			m_direction = direction;
			if (m_direction != 0) m_movingDirection = m_direction;
		}
			
		if (m_collisionState != CollisionState.None && m_jumpState != JumpState.Jumping)
		{
			if (m_movingDirection == 0 && playerState != GruntState.Turning)
			{
				//playerState = GruntState.Standing;
			}
			else
			{	
				if (playerState == GruntState.Turning)
				{
					m_velocity = 0.0f;
					return;
				}
			}
			
		}
	}
	
	public void OnUpdate(ref GruntState playerState)
	{	
		if (playerState == GruntState.Standing)
			m_body.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
		
		
		m_isJumpDown = false; //= Input.GetButton("Jump");
		//if (Application.platform == RuntimePlatform.Android)
		//	m_isJumpDown = Input.touchCount != 0;
		
		if (m_isJumpDown && m_jumpState == JumpState.Landed && CanJump(playerState))
		{
			if ((Time.time * 1000.0f) - m_jumpTimer > JumpDelayMS)
			{
				m_jumpTimer = (Time.time * 1000.0f);
				m_body.AddForce(new Vector3(0, PlayerJumpHeight, 0), ForceMode.Impulse);	
				m_jumpState = JumpState.Jumping;
				playerState = GruntState.Jumping;
				m_collisionState = CollisionState.None;
			}
		}		
		
		if (m_jumpState == JumpState.Jumping && playerState == GruntState.Jumping)
		{
			if ((Time.time * 1000.0f) - m_jumpTimer > 2000.0f)
			{
				playerState = GruntState.FallJumping;
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
	
	
	private bool CanJump(GruntState playerState)
	{
		if (playerState == GruntState.LedgeHang)
			return false;
		
		if (playerState == GruntState.LedgeClimb)
			return false;
		
		if (playerState == GruntState.LedgeClimbComplete)
			return false;
		
		if (m_collisionState != CollisionState.OnFloor)
			return false;
		
		return true;
	}
	
}
