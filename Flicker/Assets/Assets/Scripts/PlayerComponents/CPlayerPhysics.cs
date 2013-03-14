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

public enum FootMaterial {
	Stone,
	Wood,
	Metal
}

public class CPlayerPhysics : MonoBehaviour {
	
	/* -----------------
	    Private Members 
	   ----------------- */
	
	private float			m_velocity = 0.0f;						//!< The current speed of the player
	
	private float			m_platformVelocity = 0.0f;				//!< The velocity of a platform the player is standing on, if any
	
	private Rigidbody		m_body = null;							//!< The rigid body component of this entity 
	
	private int				m_direction = 0;						//!< Stores the direction, 0 = not moving, 1 = left, -1 = right
	
	private int				m_movingDirection = 1;					//!< Stores the direction, 0 = not moving, 1 = left, -1 = right

	private CollisionState	m_collisionState = CollisionState.None;	//!< 
	
	private JumpState		m_jumpState = JumpState.Landed;			//!< 
	
	private CEntityPlayer	m_player = null;

	private CWallJump		m_wallJump = null;						//!< 
	
	private bool 			m_isJumpDown = false;					//!< 
	
	private int				m_invert = 1;							//!< 	
	
	private float			m_jumpTimer = 0;						//!< The time the player last jumped
	
	private float 			m_velocityLockTimer = 0;				//!< The time that velocity was locked
	
	private float 			m_turnLockTimer = 0;					//!< The time that turning was locked
	
	private CSceneObjectPlatform	m_platform = null;				//!< The platform, if any, the player is standing on
	
	private GameObject		m_ledgeGrabBox = null;
	
	private FootMaterial	m_footMaterial = FootMaterial.Stone;
	
	private eLedgeType		m_ledgeHangType = eLedgeType.Free;
		
	private bool			m_canJumpFromLedge = false;
	
	private CPlayerLadder	m_ladder = null;
	
	private bool			m_fakeJump = false;
	
	/* ----------------
	    Public Members 
	   ---------------- */		
	
	public float			PlayerJumpHeight = 5.0f;		//!< The amount of force (in the y-axis) jump is represented by
	
	public float			AccelerationRate = 0.05f;		//!< The rate of acceleration
	
	public float			MaxSpeed = 0.5f;				//!< The maximum speed of the player 
	
	public int				JumpDelayMS = 500;				//!< The minimum required time to pass between jumps in miliseconds
	
	public bool				InsideTower = true;
	
	private bool 			m_skipNextJump = false;
	
	/*
	 * \brief Initialise anything we don't know at construct time
	*/
	public void Create(CEntityPlayer player, Rigidbody body)
	{
		m_body = body;
		m_player = player;
		m_wallJump = player.GetComponent<CWallJump>();
		
		m_ladder = new CPlayerLadder();
		
		m_ledgeGrabBox = transform.Find("Ledge_Grab_Detection").gameObject;
		
		m_invert = InsideTower ? -1 : 1;
	}
	
	public CPlayerLadder GetLadder {
		get {
			return m_ladder;	
		}
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
	
	public void SkipNextJump() {
		m_skipNextJump = true;
	}
	
	public eLedgeType GetLedgeGrabType() {
		return m_ledgeHangType;	
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
				
				if (m_player.GetPlayerState() == PlayerState.OnLadder)
				{
					GetLadder.state = LadderState.AtBase;
					m_player.SetPlayerState(PlayerState.Standing);
				}
				
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
				if (contact.otherCollider != null && contact.otherCollider.GetComponent<CSceneObjectPlatform>() != null)
				{
					m_player.PushPlayerFromTower();
				}
			}
			else
			{
				if (m_player.GetPlayerState() != PlayerState.OnLadder)
					m_collisionState = CollisionState.OnWall;
			}
		}
		
		if (m_collisionState == CollisionState.OnFloor)
		{
			if (m_jumpState != JumpState.Landed)
				m_player.GetPlayerAnimation().PlayFootstepAudio(m_footMaterial);
			
			m_jumpState = JumpState.Landed;	
			
			if (GetLadder.state == LadderState.JumpingOff)
				GetLadder.state = LadderState.None;
			
			if (m_player.GetPlayerState() != PlayerState.Turning && m_player.GetPlayerState() != PlayerState.OnLadder && !m_player.PullingLever(false)) 
			{
				m_player.SetPlayerState(PlayerState.Standing);
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
	public void CallOnCollisionStay(Collision collision, ref PlayerState playerState, ref float playerAlpha)
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
				if (m_player.GetPlayerState() != PlayerState.OnLadder)
					m_collisionState = CollisionState.OnFloor;
				
				if (!isNearly(contact.normal.y, 1.0f, 0.15f) && collision.contacts.Length == 1)
				{
					m_velocity = (m_movingDirection * 0.15f);
					m_velocityLockTimer = (Time.time * 1000.0f); 
				}
				
				if (contact.otherCollider != null)
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
				if (m_player.GetPlayerState() != PlayerState.OnLadder)
				{				
					if (isFacingCollision(m_movingDirection, m_body.transform.position, contact.point, playerAlpha)) {
						m_collisionState = CollisionState.OnWall;
						break;
					}
				}
			}
		}	
		
		if (m_collisionState == CollisionState.OnWall && m_jumpState == JumpState.Jumping && playerState != PlayerState.WallJumpStart)
		{
			m_velocity = -(m_movingDirection * 0.15f);
		}
		
		if (m_collisionState == CollisionState.OnFloor && ((Time.time * 1000.0f) - m_jumpTimer > 200.0f))
		{
			m_jumpState = JumpState.Landed;	
						
			if (m_player.GetPlayerState() != PlayerState.Turning)
			{
				if (m_player.GetPlayerState() != PlayerState.OnLadder && !m_player.PullingLever(false))
				{
					m_player.SetPlayerState(PlayerState.Standing);
				}
			}
		}
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
					
		if (obj != null && obj.KillPlayerOnTouch)
		{
			m_player.PushPlayerFromTower();
		}
	}
	
	public void CallOnTriggerExit(Collider collider, ref PlayerState playerState)
	{

	}
		
	/*
	 * \brief Called on player update
	*/
	public void OnFixedUpdate(ref PlayerState playerState)
	{		
		if (playerState == PlayerState.FallingFromTower)
			return;
		
		float velocity = (Input.GetAxis("Horizontal") * MaxSpeed) * m_invert;
		if (Application.platform == RuntimePlatform.Android)
			velocity = Input.acceleration.y;
		
		if (playerState == PlayerState.OnLadder || playerState == PlayerState.LedgeClimb || playerState == PlayerState.LedgeClimbComplete)
			velocity = 0.0f;	
		
		if (m_player.PullingLever(false))
			velocity = 0.0f;	

		if ((Time.time * 1000.0f) - m_velocityLockTimer < 350)
		{
			velocity = m_velocity;
		}
		
		if (velocity != 0.0f && (m_ladder.state != LadderState.AtBase && m_ladder.state != LadderState.JumpingOff))
		{
			m_ladder.state = LadderState.None;	
		}
							
		int direction = isNearly(velocity, 0.0f, 0.1f) ? 0 : velocity > 0 ? 1 : -1;
		
		//platform update
		if (m_platform && m_collisionState == CollisionState.OnFloor) {
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
				}
				// if the user pressed space, jump off the wall
				else if (m_isJumpDown && m_canJumpFromLedge) {
					m_body.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
					m_velocity = -(m_movingDirection) * 4.0f; // kick it away from the wall
					m_body.AddForce(new Vector3(0, PlayerJumpHeight * 0.25f, 0), ForceMode.Impulse);	
					playerState = PlayerState.Jumping;
					m_collisionState = CollisionState.None;
					m_movingDirection *= -1;
					m_direction *= -1;
					
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
				m_collisionState = CollisionState.None;
			} else if (m_isJumpDown && direction != 0 && direction != m_movingDirection) {
				m_body.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
				m_velocity = -(m_movingDirection * 0.5f); // kick it away from the wall
				m_velocityLockTimer = (Time.time * 1000.0f) + 50; 
				m_body.AddForce(new Vector3(0, PlayerJumpHeight * 1.1f, 0), ForceMode.Impulse);	
				playerState = PlayerState.Jumping;
				m_jumpState = JumpState.Jumping;
				m_jumpTimer = (Time.time * 1000.0f); 
				m_collisionState = CollisionState.None;
				m_movingDirection *= -1;
				m_direction *= -1;	
			}
			return;
		}
						
		if (!(m_collisionState == CollisionState.OnWall && m_jumpState == JumpState.Jumping))
			m_velocity = velocity;	
		
		int lastDirection = m_direction;
		int lastMovingDirection = m_movingDirection;
		
		if (playerState != PlayerState.Turning)
		{
			m_direction = direction;
			if (m_direction != 0) m_movingDirection = m_direction;
		}
			
		if (m_collisionState != CollisionState.None && m_jumpState != JumpState.Jumping)
		{
			if (m_direction == 0 && playerState != PlayerState.Turning)
			{
				if (playerState != PlayerState.OnLadder && !m_player.PullingLever(false))
				{
					playerState = PlayerState.Standing;
				}
			}
			else
			{	
				if (playerState == PlayerState.Turning)
				{
					m_velocity = 0.0f;
					return;
				}
				
				// are we tuning round?
				if (lastDirection != direction && ((Time.time * 1000.0f) - m_turnLockTimer > 1000.0f))
				{
					playerState = PlayerState.Walking;
					if (m_velocity != 0.0f && lastMovingDirection != m_direction)
					{
						playerState = PlayerState.Turning;
						m_velocity = 0.0f;
						m_turnLockTimer = Time.time * 1000.0f;
					}
				} 
				else
					playerState = PlayerState.Walking;	
			}
		}
	}
	
	public void OnUpdate(ref PlayerState playerState)
	{			
		if (playerState == PlayerState.Standing)
		{
			m_body.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
			m_fakeJump = false;
			
			if (GetLadder.state != LadderState.AtBase)
			{
				// standing, we can't be on a ladder
				GetLadder.state = LadderState.None;
				GetLadder.offset = 0.0f;
				GetLadder.moving = false;
				GetLadder.direction = 0.0f;
			}
		}
		
		bool wasJump = m_isJumpDown;
		m_isJumpDown = Input.GetButton("Jump");
		if (Application.platform == RuntimePlatform.Android)
			m_isJumpDown = Input.touchCount != 0;
		
		if (m_isJumpDown && !wasJump)
		{
			m_canJumpFromLedge = true;	
		}
		else
		{
			m_canJumpFromLedge = false;
		}
		
		if (m_isJumpDown && m_jumpState == JumpState.Landed && CanJump(playerState))
		{
			if ((Time.time * 1000.0f) - m_jumpTimer > JumpDelayMS)
			{
				m_jumpTimer = (Time.time * 1000.0f);
				m_body.AddForce(new Vector3(0, PlayerJumpHeight, 0), ForceMode.Impulse);	
				m_jumpState = JumpState.Jumping;
				playerState = PlayerState.Jumping;
				m_collisionState = CollisionState.None;
			}
		}		
		
		if (m_jumpState == JumpState.Jumping && playerState == PlayerState.Jumping)
		{
			if ((Time.time * 1000.0f) - m_jumpTimer > 2000.0f)
			{
				playerState = PlayerState.FallJumping;
				if (!m_fakeJump && ((Time.time * 1000.0f) - m_jumpTimer > 3000.0f))
				{
					m_player.PushPlayerFromTower();	
				}
			}
		}	
		
		if (m_isJumpDown == true && (GetLadder.state == LadderState.OnMiddle || GetLadder.state == LadderState.OnTop))		
		{
			GetLadder.state = LadderState.JumpingOff;
			m_jumpTimer = (Time.time * 1000.0f);
			m_body.velocity = Vector3.zero;
			m_body.AddForce(new Vector3(0, PlayerJumpHeight, 0), ForceMode.Impulse);	
			m_jumpState = JumpState.Jumping;
			playerState = PlayerState.Jumping;
			m_collisionState = CollisionState.None;	
			rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
		}
		
		// LADDER CODE
		
		float updown = (Input.GetAxis("Vertical") * 0.001f);
		if (updown > 0.0f)
		{
			if (GetLadder.state == LadderState.AtBase)
			{
				if (m_jumpState == JumpState.Jumping) {
					GetLadder.state = LadderState.OnMiddle;
				} else {
					GetLadder.state = LadderState.OnBase;
				}
				playerState = PlayerState.OnLadder;
				m_collisionState = CollisionState.None;
				m_jumpState = JumpState.Landed;
			}
			else if (GetLadder.state == LadderState.OnBase || GetLadder.state == LadderState.OnMiddle)
			{
				GetLadder.offset = (updown * 10.0f);
				GetLadder.moving = true;
				GetLadder.direction = 1.0f;
			}
			else if (GetLadder.state == LadderState.OnTop)
			{
				GetLadder.moving = false;
				GetLadder.direction = 0.0f;
				GetLadder.offset = 0.0f;
			}
		}
		else if (updown < 0.0f)
		{
			if (GetLadder.state == LadderState.OnBase)
			{
				GetLadder.state = LadderState.AtBase;
				playerState = PlayerState.Standing;
				rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
				rigidbody.AddForce(0, 0.1f, 0);
			}	
			else if (GetLadder.state == LadderState.OnBase || GetLadder.state == LadderState.OnMiddle || GetLadder.state == LadderState.OnTop)
			{
				GetLadder.offset = (updown * 10.0f);
				GetLadder.moving = true;
				GetLadder.direction = -1.0f;
			}
		}
		
		if (updown == 0.0f && GetLadder.state != LadderState.None)
		{
			GetLadder.moving = false;
			GetLadder.direction = 0.0f;
			GetLadder.offset = 0.0f;
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
	
	public void SetLedgeGrabState (CEntityPlayer player, PlayerState state, eLedgeType ledgeHangType)
	{
		if (state == PlayerState.LedgeHang)
		{
			m_velocity = 0;
			player.SetPlayerState(PlayerState.LedgeHang);
			m_body.constraints = RigidbodyConstraints.FreezeAll;
			m_jumpState = JumpState.Landed;	
			m_ledgeHangType = ledgeHangType;
		}
	}
	
	private bool CanJump(PlayerState playerState)
	{
		if (m_skipNextJump) {
			m_skipNextJump = false;
			m_jumpTimer = Time.time * 1000.0f;
			return false;
		}
		
		if (playerState == PlayerState.LedgeHang)
			return false;
		
		if (playerState == PlayerState.LedgeClimb)
			return false;
		
		if (playerState == PlayerState.LedgeClimbComplete)
			return false;

		if (m_collisionState == CollisionState.OnRoof)
			return false;
		
		if (m_collisionState == CollisionState.None)
			return false;
		
		if (playerState == PlayerState.OnLadder)
			return false;
		
		if (m_player.PullingLever(false))
			return false;
		
		return true;
	}
	
	public void MakeJump() 
	{
		m_fakeJump = true;
		m_jumpTimer = ((Time.time) * 1000.0f);
		m_jumpState = JumpState.Jumping;
		CEntityPlayer.GetInstance().SetPlayerState(PlayerState.Jumping);
		m_collisionState = CollisionState.None;	
	}
	
}
