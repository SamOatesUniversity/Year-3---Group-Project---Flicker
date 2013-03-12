using UnityEngine;
using System.Collections;

//! Different states a player can be in
public enum GruntState {
	Standing,				//!< The player is stood still
	Walking,				//!< The player is walking
	Turning,				//!< The player is turning roud
	Jumping,				//!< The player is jumping
	FallJumping,			//!< The player is falling or has jumped for a long time
	WallJumping,			//!< The player is on a wall
	Attacking,
	LedgeHang,
	LedgeClimb,
	LedgeClimbComplete,
	WallJumpStart,
	FallingFromTower		//!< The player has been pushed of the tower
};

[RequireComponent (typeof (CGruntPhysics))]
[RequireComponent (typeof (CGruntAnimation))]

public class CEntityGrunt : CEntityPlayerBase {
		
	/* -----------------
	    Private Members 
	   ----------------- */

	private float				m_playerPositionAlpha = 0.0f;			//!< How far around the tower are we (in degrees)
	
	private float 				m_lastPlayerPositionAlpha = 0.0f;		//!< 
		
	private GruntState			m_playerState = GruntState.Standing;	//!< The current player state
		
	private CGruntPhysics		m_physics = null;						//!< 
	
	private CGruntAnimation 	m_animation = null;						//!<
	
	private float				m_additionalRadius = 0.0f;
	
	private bool				m_playerDetected = false;
	
	private float 				m_resetTimer = 0.0f;

	private CGruntDebug 		m_debug = null;
	
	private bool				m_onBarrier = false;
	
	
	// dying vars
	
	struct DyingValues {
		public float y;
		public float time;
		public bool didDie;
	};
	
	private DyingValues	m_dead;
	
	private Transform			m_characterMesh = null;

	/* ----------------
	    Public Members 
	   ---------------- */
	
	public float			PlayerPathRadius = 10.0f;		//!< The radius of the circluar path the player should take
	
	public float			InitialAlphaPosition = 0.0f;	//!< The initial point on the circle where the player will start
	
	public float			TimeToReset = 5.0f;
	
	public float 			DetectionRange = 10.0f;
	
	public CCheckPoint		StartCheckPoint = null;			//!< The start point check point
	
	public GameObject 		DeathEffect = null;
	
	public enum LeftRight {
		Left,
		Right
	}
	
	public LeftRight		StartFacing = LeftRight.Left;
	
	public bool				StartMoving = true;
		
	/*
	 * \brief Called when the object is created. At the start.
	 *        Only called once per instaniation.
	*/
	public override void Start () {
		
		base.Start();
		
		if (Application.platform == RuntimePlatform.Android)
			Screen.orientation = ScreenOrientation.Landscape;
		
		//Time.timeScale = 0.75f;
		
		m_playerPositionAlpha = InitialAlphaPosition;
		m_name = "Grunt";
		
		m_physics = GetComponent<CGruntPhysics>();
		m_physics.Create(this, GetComponent<Rigidbody>());
		
        //m_cameraClass = MainCamera.GetComponent<CCamera>();
				
		m_animation = GetComponent<CGruntAnimation>();
		m_animation.OnStart(GetComponentInChildren<Animation>());
			
		m_playerHealth = MaxHealth;
		
		m_characterMesh = this.transform.Find("GruntMesh");
		m_characterMesh.rotation = Quaternion.Euler(new Vector3(0, this.transform.rotation.eulerAngles.y + 90, 0));	
		
		m_physics.MovingDirection = StartFacing == LeftRight.Left ? 1 : -1;
		
		m_playerState = GruntState.Walking;
		
		m_dead.didDie = false;
		
		
		m_debug = GetComponent<CGruntDebug>();
		if (m_debug != null)
		{
			m_debug.SetPlayer(this);	
		}
	}
	
	public int GetCurrentHealth()
	{
		return m_playerHealth;
	}
	
	public int GetMaxHealth()
	{
		return MaxHealth;
	}
	
	public float CurrentPlayerAlpha {
		get {
			return m_playerPositionAlpha;
		}
	}
	
	public CGruntAnimation GetPlayerAnimation()
	{
		return m_animation;	
	}
	
	public string CurrentAnimation()
	{
		return m_animation.CurrentAnimation();	
	}
		
	/*
	 * \brief Called once per frame
	*/
	public override void FixedUpdate () 
	{
		if( m_playerDetected )
		{
			if(m_resetTimer + TimeToReset < Time.time)
			{
				//reset detection
				m_playerDetected = false;
				m_playerState = GruntState.Turning;
				if( m_physics.MovingDirection == 1)
				{
					m_physics.MovingDirection = -1;
					//m_physics.MovingDirection = -1;
				}
				else if( m_physics.MovingDirection == -1 )
				{
					m_physics.MovingDirection = 1;
					//m_direction = 1;
				}
			}
		}

		if( m_playerPositionAlpha > 360.0f )
		{
			m_playerPositionAlpha -= 360.0f;	
		}
		else if( m_playerPositionAlpha <= -360.0f )
		{
			m_playerPositionAlpha += 360.0f;	
		}
		
		
		m_lastPlayerPositionAlpha = m_playerPositionAlpha;
		
		m_physics.OnFixedUpdate(ref m_playerState, m_playerDetected);

		
		m_playerPositionAlpha -= m_physics.Velocity;
		m_playerPositionAlpha += m_physics.PlatformVelocity;

		m_physics.PlatformVelocity = 0.0f;
		
		float additionalY = 0.0f;
		
		if (m_playerState == GruntState.FallingFromTower) {		
			m_additionalRadius += (0.025f * m_physics.Invert);
			m_additionalRadius = Mathf.Clamp(m_additionalRadius, -3.0f, 3.0f);
			m_playerPositionAlpha = m_lastPlayerPositionAlpha;
		}

		float yPosition = transform.position.y + additionalY;

		m_position = new Vector3(
			Mathf.Sin(m_playerPositionAlpha * Mathf.Deg2Rad) * (PlayerPathRadius + m_additionalRadius),
			yPosition,
			Mathf.Cos(m_playerPositionAlpha * Mathf.Deg2Rad) * (PlayerPathRadius + m_additionalRadius)
		);
		
		// Animate and position the player model mesh
		{
			
			if (m_playerState == GruntState.Turning)
			{
				//Debug.Log("NOT IN ROTATION CODE");
			}
			else
			{
				//Debug.Log("rOTATION");
				if (m_physics.Direction > 0)
				{
					m_characterMesh.rotation = Quaternion.Euler(new Vector3(0, this.transform.rotation.eulerAngles.y + 90, 0));
				}
				else if (m_physics.Direction < 0)
				{
					m_characterMesh.rotation = Quaternion.Euler(new Vector3(0, this.transform.rotation.eulerAngles.y - 90, 0));
				}
				else if (m_physics.MovingDirection > 0)
				{
					m_characterMesh.rotation = Quaternion.Euler(new Vector3(0, this.transform.rotation.eulerAngles.y + 90, 0));
				}
				else if (m_physics.MovingDirection < 0)
				{
					m_characterMesh.rotation = Quaternion.Euler(new Vector3(0, this.transform.rotation.eulerAngles.y - 90, 0));
				}
			}
			
			int mDirection = m_physics.MovingDirection;
			m_animation.OnFixedUpdate(ref m_playerState, ref mDirection, m_physics, m_physics.GetFootMaterial(), m_playerDetected);
			m_physics.MovingDirection = mDirection;
		}
		
		
		if (m_playerState == GruntState.FallingFromTower && (Time.time * 1000.0f) - m_dead.time > 3000)
		{
			OnDeath();
		}

		base.FixedUpdate();
	}
	
	public override void Update()
	{		
		if (m_playerState == GruntState.FallingFromTower)
			return;
		
		m_physics.OnUpdate(ref m_playerState);
	}
		
	/*
	 * \called to deal damage to the player
	*/
	public void DoDamage(int damage) {
		m_playerHealth -= damage;
		if (m_playerHealth <= 0)
			OnDeath();
	}
	
	/*
	 * \brief External access to set a players state
	*/
	public void SetGruntState(GruntState newState)
	{
		m_playerState = newState;
	}
	
	/*
	 * \brief External access to get a players state
	*/
	public GruntState GetGruntState()
	{
		return m_playerState;
	}
	
	/*
	 * \brief External access to set playerDetected
	*/
	public void SetGruntPlayerDetected(bool detected)
	{
		m_playerDetected = detected;
	}
	
	/*
	 * \brief External access to get a players state
	*/
	public bool GetGruntPlayerDetected()
	{
		return m_playerDetected;
	}
	
	/*
	 * \brief Gets the players physcis object
	*/
	public CGruntPhysics Physics {
		get {
			return m_physics;	
		}
	}
	
	/*
	 * \called when player health drops to zero
	*/
	void OnDeath() 
	{
	}
		
	/*
	 * \brief Called when this first collides with something
	*/
	void OnCollisionEnter(Collision collision)
	{
		if (m_playerState == GruntState.FallingFromTower)
			return;
		
		foreach (ContactPoint contact in collision)
		{
			//Debug.Log("This Collider: " + contact.thisCollider.gameObject.name);
			//Debug.Log("Other Collider: " + contact.otherCollider.gameObject.name);
			if (contact.otherCollider != null && contact.otherCollider.gameObject != null)
			{
				if(contact.otherCollider.gameObject.name == "Player Spawn")
				{
					//Debug.Log("Collided with player");
					m_playerState = GruntState.Attacking;
				}
			}
		}
		
		m_physics.CallOnCollisionEnter(collision, m_playerDetected);
		
		if( collision.collider.gameObject.name=="GruntBarrier" )
		{
			m_onBarrier = true;
		}
		if( collision.collider.gameObject.name=="GruntSpawn" && !m_playerDetected )
		{
			m_playerState = GruntState.Standing;
			m_physics.MovingDirection = -1;
		}
	}
	
	/*
	 * \brief Called when this leaves a collosion
	*/
	void OnCollisionExit(Collision collision)
	{
		m_physics.CallOnCollisionExit(collision);
		if( collision.collider.gameObject.name=="GruntBarrier" )
		{
			m_onBarrier = false;
		}
	}
	
	/*
	 * \brief Called whilst a collision is taking place
	*/
	void OnCollisionStay(Collision collision)
	{		
		if (m_playerState == GruntState.FallingFromTower)
			return;
		
		m_physics.CallOnCollisionStay(collision, ref m_playerState, ref m_playerPositionAlpha);
		if (m_physics.CollisionType == CollisionState.OnWall)
		{
			m_playerPositionAlpha = m_lastPlayerPositionAlpha;
		}
		
		foreach (ContactPoint contact in collision)
		{
			//Debug.Log("This Collider: " + contact.thisCollider.gameObject.name);
			//Debug.Log("Other Collider: " + contact.otherCollider.gameObject.name);
			if (contact.otherCollider != null && contact.otherCollider.gameObject != null)
			{
				if(contact.thisCollider.gameObject.name == "Bip001 L Hand001" && contact.otherCollider.gameObject.name == "Player Spawn")
				{
					Debug.Log("Hit player");
					CEntityPlayer.GetInstance().PushPlayerFromTower();
					m_playerDetected = false;
				}
			}
		}
	}
	
	void OnTriggerEnter(Collider collider)
	{
		//Debug.Log (collider.gameObject.name);
		if(collider.gameObject.name == "Ledge_Grab_Detection")
		{
			//Player should be detected now
			m_playerDetected = true;
			m_resetTimer = Time.time;
			
			if( !m_onBarrier )
			{
				m_playerState = GruntState.Walking;
			}
		}
	
		if(collider.gameObject.name == "Electricity")
		{
			//Kill grunt
			Object deathBlast = Instantiate(DeathEffect, transform.position, transform.rotation);
			Destroy(deathBlast, 2);
			Debug.Log(this);
			this.transform.gameObject.SetActiveRecursively(false);
		}
	}
	
	void OnTriggerStay(Collider collider) {
		
		if (m_playerState == GruntState.FallingFromTower)
			return;
		
		if(collider.gameObject.name == "Ledge_Grab_Detection")
		{
			m_resetTimer = Time.time;
		}
		
		m_physics.CallOnTriggerStay(collider, ref m_playerState);
    }
	
	void OnTriggerExit(Collider collider) {
		
		if (m_playerState == GruntState.FallingFromTower)
			return;
		if(collider.gameObject.name == "Ledge_Grab_Detection")
		{
			//Player should be detected now
			m_resetTimer = Time.time;
			
		}
		m_physics.CallOnTriggerExit(collider, ref m_playerState);
	}
	
	public void PushPlayerFromTower()
	{
		if (m_playerState == GruntState.FallingFromTower)
			return;
		
		m_playerState = GruntState.FallingFromTower;
		m_dead.y = transform.position.y;
		m_dead.time = Time.time * 1000.0f;
	}
}
