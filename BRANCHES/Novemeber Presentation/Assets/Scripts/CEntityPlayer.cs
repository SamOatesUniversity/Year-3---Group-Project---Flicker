using UnityEngine;
using System.Collections;

//! Different states a player can be in
public enum PlayerState {
	Standing,				//!< The player is stood still
	Walking,				//!< The player is walking
	Jumping					//!< The player is jumping
};

[RequireComponent (typeof (CWallJump))]
[RequireComponent (typeof (CPlayerPhysics))]

public class CEntityPlayer : CEntityPlayerBase {
		
	/* -----------------
	    Private Members 
	   ----------------- */

    private CPlayerLight    m_playerLight;

	private float			m_playerPositionAlpha = 0.0f;			//!< How far around the tower are we (in degrees)
		
	private PlayerState		m_playerState = PlayerState.Standing;	//!< The current player state
		
    private CCamera 		m_cameraClass = null;					//!< Todo: Haydn fill these in.
		
	private CPlayerPhysics	m_physics = null;						//!< 
	
	private CWallJump		m_wallJump = null;						//!< 
	
	private Animation		m_animation = null;	
	
	//////////////////////////
	// Move to animation class
	
	string[] 				m_idleAnimations = new string[4]; 
	
	int						m_idleAnimID = 0;
	
	////////////////////////
	
	AudioSource				m_footSteps = null;

	/* ----------------
	    Public Members 
	   ---------------- */
	
	public float			PlayerPathRadius = 10.0f;		//!< The radius of the circluar path the player should take
	
	public float			InitialAlphaPosition = 0.0f;	//!< The initial point on the circle where the player will start
	
	public Camera			MainCamera = null;				//!< The main viewport camera, which will follow the player

    public GameObject       Bulb = null;
		
	/*
	 * \brief Called when the object is created. At the start.
	 *        Only called once per instaniation.
	*/
	public override void Start () {
		
		base.Start();
		
		m_playerPositionAlpha = InitialAlphaPosition;
		m_name = "Player";
		
		m_physics = GetComponent<CPlayerPhysics>();
		m_physics.Create(GetComponent<Rigidbody>());
		
        m_cameraClass = MainCamera.GetComponent<CCamera>();
		
		m_wallJump = GetComponent<CWallJump>();
		
		m_animation = GetComponentInChildren<Animation>();
		
		m_playerHealth = MaxHealth;

        if (Bulb)
        {
            GameObject obj = (GameObject)Instantiate( Bulb, this.m_position, Quaternion.identity);
            m_playerLight = obj.GetComponent<CPlayerLight>();
            m_playerLight.transform.parent = this.transform;
        }
		
		m_idleAnimations[0] = "idle";
		m_idleAnimations[1] = "idle1";
		m_idleAnimations[2] = "idle2";
		m_idleAnimations[3] = "idle3";
		
		m_footSteps = GetComponent<AudioSource>();
	}
	
	public int GetCurrentHealth()
	{
		return m_playerHealth;
	}
	
	public int GetMaxHealth()
	{
		return MaxHealth;
	}
	
	
	/*
	 * \brief Called once per frame
	*/
	public override void Update () {
					
		m_physics.OnUpdate(ref m_playerState);
				
		m_playerPositionAlpha -= m_physics.Velocity;
					
		float yPos = transform.position.y;
		if (m_wallJump.GetCanWallJump()) {
			yPos = m_wallJump.GetWallJumpPoint().y;
			transform.transform.position = new Vector3(transform.transform.position.x, yPos, transform.transform.position.z);	
		}
		
		m_position = new Vector3(
			Mathf.Sin(m_playerPositionAlpha * Mathf.Deg2Rad) * PlayerPathRadius,
			yPos,
			Mathf.Cos(m_playerPositionAlpha * Mathf.Deg2Rad) * PlayerPathRadius
		);
		
		// Handle wall jumping.
		m_wallJump.onUpdate(ref m_physics, ref m_playerState);
				
        //position the lookat
		Vector3 lookat = new Vector3(0.0f, yPos, 0.0f);
		
		m_cameraClass.TendToMaxOffset(m_physics.Direction);
		
		float cameraAlpha = (m_playerPositionAlpha - m_cameraClass.CameraOffset) * Mathf.Deg2Rad;
		
		// position the camera
	    Vector3 camPostion = new Vector3(
            Mathf.Sin(cameraAlpha) * (m_cameraClass.DistanceFromPlayer + PlayerPathRadius),
			yPos,
            Mathf.Cos(cameraAlpha) * (m_cameraClass.DistanceFromPlayer + PlayerPathRadius)	
		);

        m_cameraClass.SetPosition(camPostion);
        m_cameraClass.SetLookAt(lookat);
	
		base.Update();

		if (m_physics.Direction >= 0)
			this.transform.GetChild(0).transform.rotation = Quaternion.Euler(new Vector3(0, this.transform.rotation.eulerAngles.y + 90, 0));
		else if (m_physics.Direction < 0)
			this.transform.GetChild(0).transform.rotation = Quaternion.Euler(new Vector3(0, this.transform.rotation.eulerAngles.y - 90, 0));
	
		// move animation stuff to a class
		if (m_playerState == PlayerState.Walking) 
		{	
			if (!m_animation.IsPlaying("walk"))
				m_animation.CrossFade("walk", 0.25f);
			m_animation["walk"].speed = Mathf.Abs(m_physics.Velocity) * 2.0f;
			
			if (!m_footSteps.isPlaying)
				m_footSteps.Play();			
		}
		else if (m_playerState == PlayerState.Standing)
		{
			if (!m_animation.IsPlaying(m_idleAnimations[m_idleAnimID])) {
				m_animation.CrossFade(m_idleAnimations[m_idleAnimID]);
				m_idleAnimID = Random.Range(0, 4);
			}
			
			if (!m_footSteps.isPlaying)
				m_footSteps.Stop();
		}
		else if (m_playerState == PlayerState.Jumping)
		{
			if (!m_animation.IsPlaying("jump"))
				m_animation.CrossFade("jump");
			
			if (!m_footSteps.isPlaying)
				m_footSteps.Stop();
		}
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
	void OnDeath() {
		//TODO: Position variables are pulled from a spawn point - one for each scene
		m_playerPositionAlpha = InitialAlphaPosition;
		m_playerHealth = MaxHealth;
		transform.position = new Vector3(0.0f, 1.0f, 0.0f);
	}
		
	/*
	 * \brief Called when this first collides with something
	*/
	void OnCollisionEnter(Collision collision) {
		// spin through all the points of contact
        foreach (ContactPoint contact in collision.contacts) {
			
			m_physics.CallOnCollisionContactEnter(contact);
						
			if (contact.normal.y >= 0.2) {
				m_playerState = PlayerState.Standing;
				m_wallJump.Reset();
			}			
        }
		
		m_wallJump.CallOnCollisionEnter(this, collision, m_playerState);	
		
		m_physics.CanJump = true;
	}
	
	/*
	 * \brief Called when this leaves a collosion
	*/
	void OnCollisionExit(Collision collision) {
		m_wallJump.CallOnCollisionExit(collision);
		m_physics.CallOnCollisionExit(collision);
	}
	
	/*
	 * \brief Called whilst a collision is taking place
	*/
	void OnCollisionStay(Collision collision) {			
		m_wallJump.CallOnCollisionStay(collision, ref m_physics);
		m_physics.CallOnCollisionStay(collision, ref m_wallJump);		
	}
}
