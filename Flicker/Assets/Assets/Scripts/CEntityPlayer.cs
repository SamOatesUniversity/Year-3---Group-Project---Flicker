using UnityEngine;
using System.Collections;

//! Different states a player can be in
public enum PlayerState {
	Standing,				//!< The player is stood still
	Walking,				//!< The player is walking
	Jumping,				//!< The player is jumping
	FallJumping,			//!< The player is falling or has jumped for a long time
	WallJumping,			//!< The player is on a wall
	LedgeHang,
	LedgeClimb,
	LedgeClimbComplete,
	WallJumpStart,
	FallingFromTower		//!< The player has been pushed of the tower
};

[RequireComponent (typeof (CWallJump))]
[RequireComponent (typeof (CPlayerPhysics))]
[RequireComponent (typeof (CPlayerAnimation))]

public class CEntityPlayer : CEntityPlayerBase {
		
	/* -----------------
	    Private Members 
	   ----------------- */

    private CPlayerLight    	m_playerLight = null;

	private float				m_playerPositionAlpha = 0.0f;			//!< How far around the tower are we (in degrees)
	
	private float 				m_lastPlayerPositionAlpha = 0.0f;		//!< 
		
	private PlayerState			m_playerState = PlayerState.Standing;	//!< The current player state
		
    private CCamera 			m_cameraClass = null;					//!< Todo: Haydn fill these in.
		
	private CPlayerPhysics		m_physics = null;						//!< 
	
	private CPlayerAnimation 	m_animation = null;						//!<
	
	private float				m_additionalRadius = 0.0f;
	
	private CCheckPoint			m_lastCheckpoint = null;	
	
	// dying vars
	
	struct DyingValues {
		public float y;
		public float time;
		public bool didDie;
	};
	
	private DyingValues	m_dead;
	
	
	////////////////////////
	
	AudioSource				m_footSteps = null;

	/* ----------------
	    Public Members 
	   ---------------- */
	
	public float			PlayerPathRadius = 10.0f;		//!< The radius of the circluar path the player should take
	
	public float			InitialAlphaPosition = 0.0f;	//!< The initial point on the circle where the player will start
	
	public Camera			MainCamera = null;				//!< The main viewport camera, which will follow the player
	
	public CCheckPoint		StartCheckPoint = null;			//!< The start point check point

		
	/*
	 * \brief Called when the object is created. At the start.
	 *        Only called once per instaniation.
	*/
	public override void Start () {
		
		base.Start();
		
		m_playerPositionAlpha = InitialAlphaPosition;
		m_name = "Player";
		
		m_physics = GetComponent<CPlayerPhysics>();
		m_physics.Create(this, GetComponent<Rigidbody>());
		
        m_cameraClass = MainCamera.GetComponent<CCamera>();
				
		m_animation = GetComponent<CPlayerAnimation>();
		m_animation.OnStart(GetComponentInChildren<Animation>());
			
		m_playerHealth = MaxHealth;
		
		m_footSteps = GetComponent<AudioSource>();
				
		this.transform.GetChild(0).transform.rotation = Quaternion.Euler(new Vector3(0, this.transform.rotation.eulerAngles.y + 90, 0));	
		
		m_lastCheckpoint = StartCheckPoint;
		m_lastCheckpoint.PlayerCheckPointAlpha = m_playerPositionAlpha;
		
		m_dead.didDie = false;
	}
	
	public int GetCurrentHealth()
	{
		return m_playerHealth;
	}
	
	public int GetMaxHealth()
	{
		return MaxHealth;
	}
	
	public CCheckPoint LastCheckPoint {
		set {
			m_lastCheckpoint = value;	
		}
	}
	
	public float CurrentPlayerAlpha {
		get {
			return m_playerPositionAlpha;
		}
	}
		
	/*
	 * \brief Called once per frame
	*/
	public override void FixedUpdate () 
	{
		m_lastPlayerPositionAlpha = m_playerPositionAlpha;
		
		m_physics.OnFixedUpdate(ref m_playerState);
		
		m_playerPositionAlpha -= m_physics.Velocity;
		
		float additionalY = 0.0f;
		if (m_playerState == PlayerState.LedgeClimbComplete)
		{
			m_playerPositionAlpha -= m_physics.MovingDirection * 4;	
			m_lastPlayerPositionAlpha = m_playerPositionAlpha;
			additionalY += 0.6f;
			rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
			m_playerState = PlayerState.Standing;
		}
		
		if (m_physics.LadderClimb.State != LadderState.None) {
			additionalY += m_physics.LadderClimb.Offset;
		}
		
		if (m_playerState == PlayerState.FallingFromTower) {		
			m_additionalRadius += (0.025f * m_physics.Invert);
			m_additionalRadius = Mathf.Clamp(m_additionalRadius, -3.0f, 3.0f);
			m_playerPositionAlpha = m_lastPlayerPositionAlpha;			
		}
				
		float yPosition = transform.position.y + additionalY;
		if (m_dead.didDie) {
			yPosition = m_lastCheckpoint.transform.position.y;
			m_dead.didDie = false;
		}
		
		m_position = new Vector3(
			Mathf.Sin(m_playerPositionAlpha * Mathf.Deg2Rad) * (PlayerPathRadius + m_additionalRadius),
			yPosition,
			Mathf.Cos(m_playerPositionAlpha * Mathf.Deg2Rad) * (PlayerPathRadius + m_additionalRadius)
		);
		
		// Camera Positioning
		{
			float cameraAlpha = (m_playerPositionAlpha - m_cameraClass.CameraOffset) * Mathf.Deg2Rad;
			
		    Vector3 camPostion = new Vector3(
	            Mathf.Sin(cameraAlpha) * (m_cameraClass.DistanceFromPlayer + PlayerPathRadius),
				m_position.y,
	            Mathf.Cos(cameraAlpha) * (m_cameraClass.DistanceFromPlayer + PlayerPathRadius)	
			);
			
			if (m_playerState == PlayerState.FallingFromTower)
				camPostion.y = m_dead.y;
			
	        m_cameraClass.SetPosition(camPostion);
	        m_cameraClass.SetLookAt(transform.position);
		}
		
		// Animate and position the player model mesh
		{
			if (m_physics.Direction > 0)
				this.transform.GetChild(0).transform.rotation = Quaternion.Euler(new Vector3(0, this.transform.rotation.eulerAngles.y + 90, 0));
			else if (m_physics.Direction < 0)
				this.transform.GetChild(0).transform.rotation = Quaternion.Euler(new Vector3(0, this.transform.rotation.eulerAngles.y - 90, 0));
			
			m_animation.OnFixedUpdate(ref m_playerState);
		}
		
		
		if (m_playerState == PlayerState.FallingFromTower && (Time.time * 1000.0f) - m_dead.time > 3000)
		{
			OnDeath();
		}
		
		base.FixedUpdate();
	}
	
	public override void Update()
	{
		if (Input.GetButton("Reset"))
		{
			OnDeath();
			return;
		}		
		
		if (m_playerState == PlayerState.FallingFromTower)
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
	public void SetPlayerState(PlayerState newState)
	{
		m_playerState = newState;
	}
	
	/*
	 * \brief Gets the players physcis object
	*/
	public CPlayerPhysics Physics {
		get {
			return m_physics;	
		}
	}
	
	/*
	 * \called when player health drops to zero
	*/
	void OnDeath() 
	{
		m_lastPlayerPositionAlpha = m_playerPositionAlpha = m_lastCheckpoint.PlayerCheckPointAlpha;
		m_physics.Direction = m_lastCheckpoint.Direction;
		m_playerHealth = MaxHealth;
		m_playerState = PlayerState.Standing;
		m_additionalRadius = 0.0f;
		m_dead.didDie = true;
		rigidbody.velocity = Vector3.zero;
	}
		
	/*
	 * \brief Called when this first collides with something
	*/
	void OnCollisionEnter(Collision collision)
	{
		if (m_playerState == PlayerState.FallingFromTower)
			return;
		
		m_physics.CallOnCollisionEnter(collision);
	}
	
	/*
	 * \brief Called when this leaves a collosion
	*/
	void OnCollisionExit(Collision collision)
	{

	}
	
	/*
	 * \brief Called whilst a collision is taking place
	*/
	void OnCollisionStay(Collision collision)
	{		
		if (m_playerState == PlayerState.FallingFromTower)
			return;
		
		m_physics.CallOnCollisionStay(collision, ref m_playerState, ref m_playerPositionAlpha);
		if (m_physics.CollisionType == CollisionState.OnWall)
		{
			m_playerPositionAlpha = m_lastPlayerPositionAlpha;
		}
	}
	
	void OnTriggerStay(Collider collision) {
		
		if (m_playerState == PlayerState.FallingFromTower)
			return;
						
		if (collision.gameObject.name == "VentCollision") {
			GameObject parent = collision.gameObject.transform.parent.gameObject;	
			if (parent != null) {
				CSteamVent vent = parent.GetComponent<CSteamVent>();
				if (vent != null && vent.StreamOn) {
					m_playerState = PlayerState.FallingFromTower;
					m_dead.y = transform.position.y;
					m_dead.time = Time.time * 1000.0f;
					return;
				}
			}
		}
		
		m_physics.CallOnTriggerStay(collision, ref m_playerState);
    }
	
	void OnTriggerExit(Collider collision) {
		
		if (m_playerState == PlayerState.FallingFromTower)
			return;
		
		m_physics.CallOnTriggerExit(collision, ref m_playerState);
	}
}
