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
	
	private int 			m_playerHealth = 0;
	
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
		
		m_playerHealth = Health;


        if (Bulb)
        {
            GameObject obj = (GameObject)Instantiate( Bulb, this.m_position, Quaternion.identity);
            m_playerLight = obj.GetComponent<CPlayerLight>();
            m_playerLight.transform.parent = this.transform;
        }

	}
	
	/*
	 * \brief Called once per frame
	*/
	public override void Update () {
					
		m_physics.OnUpdate(ref m_playerState);
				
		m_playerPositionAlpha -= m_physics.Velocity;
					
		m_position = new Vector3(
			Mathf.Sin(m_playerPositionAlpha * Mathf.Deg2Rad) * PlayerPathRadius,
			transform.position.y,
			Mathf.Cos(m_playerPositionAlpha * Mathf.Deg2Rad) * PlayerPathRadius
		);
		
		// Handle wall jumping. TODO: get a class that stores 'physics' memebers and pass that around like a dirty whore
		m_wallJump.onUpdate(ref m_physics, ref m_playerState);
				
        //position the lookat
		Vector3 lookat = new Vector3(0.0f, transform.position.y, 0.0f);
		
		m_cameraClass.TendToMaxOffset(m_physics.Direction);
		
		float cameraAlpha = (m_playerPositionAlpha - m_cameraClass.CameraOffset) * Mathf.Deg2Rad;
		
		// position the camera
	    Vector3 camPostion = new Vector3(
            Mathf.Sin(cameraAlpha) * (m_cameraClass.DistanceFromPlayer + PlayerPathRadius),
			transform.position.y,
            Mathf.Cos(cameraAlpha) * (m_cameraClass.DistanceFromPlayer + PlayerPathRadius)	
		);

        m_cameraClass.SetPosition(camPostion);
        m_cameraClass.SetLookAt(lookat);
		
		base.Update();

		if (m_physics.Direction > 0)
			this.transform.GetChild(0).transform.rotation = Quaternion.Euler(new Vector3(0, this.transform.rotation.eulerAngles.y + 90, 0));
		else if (m_physics.Direction < 0)
			this.transform.GetChild(0).transform.rotation = Quaternion.Euler(new Vector3(0, this.transform.rotation.eulerAngles.y - 90, 0));
	
		if (m_physics.Velocity != 0) {
			if (!m_animation.IsPlaying("walk"))
				m_animation.Play("walk");
			m_animation["walk"].speed = Mathf.Abs(m_physics.Velocity) * 2.0f;
		} else {
			m_animation.Stop();	
		}
		
		
		if(m_playerHealth <= 0)
		{
			OnDeath();
		}	
	}
		
	/*
	 * \called to deal damage to the player
	*/
	public void DoDamage(int damage) {
		m_playerHealth -= damage;
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
		m_playerPositionAlpha = InitialAlphaPosition;
		m_playerHealth = Health;
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
		
		m_wallJump.CallOnCollisionEnter(collision, m_playerState);	
		
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
