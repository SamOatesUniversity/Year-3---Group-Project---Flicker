using UnityEngine;
using System.Collections;

//! Different states a player can be in
public enum PlayerState {
	Standing,				//!< The player is stood still
	Walking,				//!< The player is walking
	Turning,				//!< The player is turning roud
	Jumping,				//!< The player is jumping
	FallJumping,			//!< The player is falling or has jumped for a long time
	WallJumping,			//!< The player is on a wall
	LedgeHang,
	LedgeClimb,
	LedgeClimbComplete,
	WallJumpStart,
	FallingFromTower,		//!< The player has been pushed of the tower
	UpALadder,
	DownALadder
};

[RequireComponent (typeof (CWallJump))]
[RequireComponent (typeof (CPlayerPhysics))]
[RequireComponent (typeof (CPlayerAnimation))]

public class CEntityPlayer : CEntityPlayerBase {
		
	/* -----------------
	    Private Members 
	   ----------------- */

	private float				m_playerPositionAlpha = 0.0f;			//!< How far around the tower are we (in degrees)
	
	private float 				m_lastPlayerPositionAlpha = 0.0f;		//!< 
		
	private PlayerState			m_playerState = PlayerState.Standing;	//!< The current player state
		
    //private CCamera 			m_cameraClass = null;					//!< Todo: Haydn fill these in.
		
	private CPlayerPhysics		m_physics = null;						//!< 
	
	private CPlayerAnimation 	m_animation = null;						//!<
	
	private float				m_additionalRadius = 0.0f;
	
	private CCheckPoint			m_lastCheckpoint = null;	
	
	private CPlayerDebug 		m_debug = null;
	
	private Vector3				m_pelvisOffset;
	
	// dying vars
	
	struct DyingValues {
		public float y;
		public float time;
		public bool didDie;
	};
	
	private DyingValues			m_dead;
	
	private Transform			m_characterMesh = null;
	
	private Transform			m_ledgeGrabBox = null;
	
	private bool 				m_isEscapeDown = false;
	
	/* ----------------
	    Public Members 
	   ---------------- */
	
	public float			PlayerPathRadius = 10.0f;		//!< The radius of the circluar path the player should take
	
	public float			InitialAlphaPosition = 0.0f;	//!< The initial point on the circle where the player will start
	
	//public Camera			MainCamera = null;				//!< The main viewport camera, which will follow the player
	
	public CCheckPoint		StartCheckPoint = null;			//!< The start point check point
	
	public enum LeftRight {
		Left,
		Right
	}
	
	public LeftRight		StartFacing = LeftRight.Left;
		
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
		m_name = "Player";
		
		m_physics = GetComponent<CPlayerPhysics>();
		m_physics.Create(this, GetComponent<Rigidbody>());
		
        //m_cameraClass = MainCamera.GetComponent<CCamera>();
				
		m_animation = GetComponent<CPlayerAnimation>();
		m_animation.OnStart(GetComponentInChildren<Animation>());
			
		m_playerHealth = MaxHealth;

		m_characterMesh = this.transform.Find("Player_Mesh");
		m_characterMesh.rotation = Quaternion.Euler(new Vector3(0, this.transform.rotation.eulerAngles.y + 90, 0));	
		
		m_physics.MovingDirection = StartFacing == LeftRight.Left ? 1 : -1;
		m_ledgeGrabBox = this.transform.Find("Ledge_Grab_Detection");
		
		if (StartCheckPoint != null)
		{
			m_lastCheckpoint = StartCheckPoint;
		}
		
		m_dead.didDie = false;
		
		m_debug = GetComponent<CPlayerDebug>();
		if (m_debug != null)
		{
			m_debug.SetPlayer(this);	
		}
		
		m_pelvisOffset = this.transform.Find("Player_Mesh/Bip001/Bip001 Pelvis").position - this.transform.position;
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
	
	public CPlayerAnimation GetPlayerAnimation()
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
		m_lastPlayerPositionAlpha = m_playerPositionAlpha;
		
		m_physics.OnFixedUpdate(ref m_playerState);
		
		m_playerPositionAlpha -= m_physics.Velocity;
		m_playerPositionAlpha += m_physics.PlatformVelocity;// * 1500;
		// reset platformVelocity
		m_physics.PlatformVelocity = 0.0f;
		
		float additionalY = 0.0f;
		if (m_playerState == PlayerState.LedgeClimbComplete)
		{
			m_playerPositionAlpha -= m_physics.MovingDirection * 4;	
			m_lastPlayerPositionAlpha = m_playerPositionAlpha;
			//additionalY += 0.65f;
			rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
			m_playerState = PlayerState.Standing;
			//m_physics.CurrentCollisionState = CollisionState.OnFloor;
			CapsuleCollider capCollider = this.GetComponentInChildren<CapsuleCollider>();
			capCollider.enabled = true;
			
			//new stuffs
			Vector3 newPelvisOffset = this.transform.Find("Player_Mesh/Bip001/Bip001 Pelvis").position - this.transform.position;
			this.transform.position = this.transform.position + (newPelvisOffset - m_pelvisOffset) /*+ new Vector3(0, 0.09f, 0)*/;
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
		
		/*
		// Camera Positioning
		{
			//m_cameraClass.TendToMaxOffset(m_physics.Direction);
			
			Vector3 camPostion = Vector3.zero;
			
			float timeStoodStill = (Time.time * 1000.0f) - m_standingStillTime;
			if ((m_playerState == PlayerState.Standing && (timeStoodStill > m_cameraClass.CameraZoomTimerMs)) && m_physics.IsOnPlatform() == false)
			{
				m_cameraClass.DistanceFromPlayer -= 0.01f;
			}
			else
			{
				m_cameraClass.DistanceFromPlayer += 0.1f;
			}
			
			m_cameraClass.DistanceFromPlayer = Mathf.Clamp(m_cameraClass.DistanceFromPlayer, m_cameraClass.MinimumCameraDistance, m_cameraClass.MaximumCameraDistance);
			float cameraDistance = m_cameraClass.DistanceFromPlayer * m_physics.Invert;
			
			if (m_playerState == PlayerState.UpALadder && m_physics.CollisionType == CollisionState.None)
			{
				camPostion = new Vector3(
		            0,
					0,
		            (m_physics.InsideTower) ? cameraDistance : -cameraDistance
				);				
			}
			else
			{
				float movingDirection = Physics.MovingDirection;
				if (movingDirection == 0)
				{
					// this is bad
					movingDirection = 1;
					if (m_characterMesh.rotation.eulerAngles.y > 180.0f) 
					{
						movingDirection = -1;
					}
				}
				
				if (m_playerState == PlayerState.Turning)
					movingDirection *= -1.0f;
				
				camPostion = new Vector3(
		            (movingDirection == -1) ? -cameraDistance : cameraDistance,
					0,
		            0	
				);
			}
			
			if (m_playerState == PlayerState.FallingFromTower)
				camPostion.y = m_dead.y;
			
			Vector3 lookatOffset = transform.FindChild("Player_Mesh/Bip001/Bip001 Pelvis").position;
			
	        m_cameraClass.SetPosition(camPostion);
	        m_cameraClass.SetLookAt(lookatOffset);
		}
		*/
		
		// Animate and position the player model mesh
		{
			if (m_playerState == PlayerState.UpALadder)
			{
				if (m_physics.InsideTower)
					m_characterMesh.rotation = Quaternion.Euler(new Vector3(0, this.transform.rotation.eulerAngles.y - 180, 0));
				else
					m_characterMesh.rotation = Quaternion.Euler(new Vector3(0, this.transform.rotation.eulerAngles.y, 0));
			}
			else if (m_playerState == PlayerState.Turning)
			{
				
			}
			else
			{
				if (m_physics.Direction > 0)
				{
					m_characterMesh.rotation = Quaternion.Euler(new Vector3(0, this.transform.rotation.eulerAngles.y + 90, 0));
					m_ledgeGrabBox.localPosition = new Vector3(0.18f, m_ledgeGrabBox.localPosition.y, m_ledgeGrabBox.localPosition.z);
				}
				else if (m_physics.Direction < 0)
				{
					m_characterMesh.rotation = Quaternion.Euler(new Vector3(0, this.transform.rotation.eulerAngles.y - 90, 0));
					m_ledgeGrabBox.localPosition = new Vector3(-0.18f, m_ledgeGrabBox.localPosition.y, m_ledgeGrabBox.localPosition.z);
				}
				else if (m_physics.MovingDirection > 0)
				{
					m_characterMesh.rotation = Quaternion.Euler(new Vector3(0, this.transform.rotation.eulerAngles.y + 90, 0));
					m_ledgeGrabBox.localPosition = new Vector3(0.18f, m_ledgeGrabBox.localPosition.y, m_ledgeGrabBox.localPosition.z);
				}
				else if (m_physics.MovingDirection < 0)
				{
					m_characterMesh.rotation = Quaternion.Euler(new Vector3(0, this.transform.rotation.eulerAngles.y - 90, 0));
					m_ledgeGrabBox.localPosition = new Vector3(-0.18f, m_ledgeGrabBox.localPosition.y, m_ledgeGrabBox.localPosition.z);
				}
			}
			
			m_animation.OnFixedUpdate(ref m_playerState, m_physics.LadderClimb.State, m_physics.GetFootMaterial(), this);
		}
		
		
		if (m_playerState == PlayerState.FallingFromTower && (Time.time * 1000.0f) - m_dead.time > 3000)
		{
			OnDeath();
		}
		
		//Not very nice - reenables collider if no longer ledge hanging/climbing
		if (
				m_playerState != PlayerState.LedgeHang && 
				m_playerState != PlayerState.LedgeClimb &&
				m_playerState != PlayerState.LedgeClimbComplete
			)
		{
			CapsuleCollider capCollider = this.GetComponentInChildren<CapsuleCollider>();
			capCollider.enabled = true;
		}
		
		base.FixedUpdate();
	}
	
	public override void Update()
	{
		if (Input.GetButton("Reset") && !m_isEscapeDown)
		{
			m_isEscapeDown = true;
			
			// if we are on android, kill the game
			if (Application.platform == RuntimePlatform.Android)
				Application.Quit();				
				
			if (m_lastCheckpoint != null && m_lastCheckpoint.NextCheckPoint != null)
			{
				m_lastCheckpoint = m_lastCheckpoint.NextCheckPoint;	
			}
			OnDeath();
			return;
		}
		else if (m_isEscapeDown && !Input.GetButton("Reset"))
		{
			m_isEscapeDown = false;
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
	 * \brief External access to get a players state
	*/
	public PlayerState GetPlayerState()
	{
		return m_playerState;
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
		if (m_lastCheckpoint != null)
		{
			m_lastPlayerPositionAlpha = m_playerPositionAlpha = m_lastCheckpoint.PlayerCheckPointAlpha;
			m_physics.Direction = m_lastCheckpoint.Direction;	
		}
		else
		{
			m_physics.Direction = 1;
			m_physics.MovingDirection = 1;
			m_lastPlayerPositionAlpha = m_playerPositionAlpha = InitialAlphaPosition;
		}
		
		m_playerState = PlayerState.Standing;
		m_physics.JumpType = JumpState.Landed;
		m_physics.LadderClimb.State = LadderState.None;
		m_playerHealth = MaxHealth;
		m_additionalRadius = 0.0f;
		m_dead.didDie = true;
		rigidbody.velocity = Vector3.zero;
		
		m_characterMesh.rotation = Quaternion.Euler(new Vector3(0, this.transform.rotation.eulerAngles.y + 90, 0));
		m_ledgeGrabBox.localPosition = new Vector3(0.18f, m_ledgeGrabBox.localPosition.y, m_ledgeGrabBox.localPosition.z);
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
		m_physics.CallOnCollisionExit(collision);
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
	
	void OnTriggerEnter(Collider collider)
	{
		//Disabled ledge grab stuff
		/*
		if(collider.gameObject != null && collider.gameObject.name == "LedgeHang" && this.collider.name == "LedgeGrabTrigger")
		{
			
			//Debug.Log("Triggered!");
			if (
				m_playerState != PlayerState.LedgeHang && 
				m_playerState != PlayerState.LedgeClimb &&
				m_playerState != PlayerState.LedgeClimbComplete
			)
			{
				m_physics.SetLedgeGrabState(this, PlayerState.LedgeHang);
				CapsuleCollider capCollider = this.GetComponentInChildren<CapsuleCollider>();
				capCollider.enabled = false;
			}
		}
		*/
		
	}
	
	void OnTriggerStay(Collider collision) {
		
		if (m_playerState == PlayerState.FallingFromTower)
			return;
						
		if (collision.gameObject.name == "VentCollision") {
			GameObject parent = collision.gameObject.transform.parent.gameObject;	
			if (parent != null) {
				CSteamVent vent = parent.GetComponent<CSteamVent>();
				if (vent != null && vent.StreamOn) {
					PushPlayerFromTower();
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
	
	public void PushPlayerFromTower()
	{
		if (m_playerState == PlayerState.FallingFromTower)
			return;
		
		m_playerState = PlayerState.FallingFromTower;
		m_dead.y = transform.position.y;
		m_dead.time = Time.time * 1000.0f;
	}
}
