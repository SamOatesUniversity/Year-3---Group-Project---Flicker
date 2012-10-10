using UnityEngine;
using System.Collections;

public class CEntityPlayer : CEntityPlayerBase {
	
	private enum PlayerState {
		Standing,
		Walking,
		Jumping
	};
	
	/* -----------------
	    Private Members 
	   ----------------- */
		
	private float			m_playerPositionAlpha = 0.0f;			//!< How far around the tower are we (in degrees)
	
	private float			m_volocity = 0.0f;						//!< The current speed of the player
	
	private Rigidbody		m_body = null;							//!< The rigid body component of this entity 
	
	private PlayerState		m_playerState = PlayerState.Standing;	//!< The current player state
	
	private bool			m_colliding = false;					//!< Is the player colliding with anything
	
	/* ----------------
	    Public Members 
	   ---------------- */
	
	public float			PlayerPathRadius = 10.0f;		//!< The radius of the circluar path the player should take
	
	public float			InitialAlphaPosition = 0.0f;	//!< The initial point on the circle where the player will start
	
	public Camera			MainCamera = null;				//!< The main viewport camera, which will follow the player
	
	public float			PlayerJumpHeight = 250.0f;		//!< The amount of force (in the y-axis) jump is represented by
	
	public float			AccelerationRate = 0.05f;		//!< The rate of acceleration
	
	public float			MaxSpeed = 0.5f;				//!< The maximum speed of the player 		

	/*
	 * \brief Called when the object is created. At the start.
	 *        Only called once per instaniation.
	*/
	public override void Start () {
		
		base.Start();
		
		m_playerPositionAlpha = InitialAlphaPosition;
		m_name = "Player";
		
		m_body = GetComponent<Rigidbody>();
		
	}
	
	/*
	 * \brief Called once per frame
	*/
	public override void Update () {
					
		// handle movement to the left and right
		if (!m_colliding)
		{
			m_volocity += Input.GetAxis("Horizontal") * AccelerationRate;
			if (m_volocity > MaxSpeed) m_volocity = MaxSpeed;
			if (m_volocity < -MaxSpeed) m_volocity = -MaxSpeed;
		}
		
		m_playerPositionAlpha -= m_volocity;
					
		m_position = new Vector3(
			Mathf.Sin(m_playerPositionAlpha * Mathf.Deg2Rad) * PlayerPathRadius,
			transform.position.y,
			Mathf.Cos(m_playerPositionAlpha * Mathf.Deg2Rad) * PlayerPathRadius
		);
		
		// handle jumping
		if (m_playerState != PlayerState.Jumping && Input.GetKeyDown(KeyCode.Space) && !m_colliding) {
			m_body.AddForce(new Vector3(0.0f, PlayerJumpHeight , 0.0f));	
			m_playerState = PlayerState.Jumping;
		}
		
		Vector3 lookat = new Vector3(0.0f, transform.position.y, 0.0f);
		
		// position the camera
		CCamera cameraClass = MainCamera.GetComponent<CCamera>();
		MainCamera.transform.position = new Vector3(
			Mathf.Sin(m_playerPositionAlpha * Mathf.Deg2Rad) * (cameraClass.DistanceFromPlayer + PlayerPathRadius),
			transform.position.y,
			Mathf.Cos(m_playerPositionAlpha * Mathf.Deg2Rad) * (cameraClass.DistanceFromPlayer + PlayerPathRadius)	
		);
		MainCamera.transform.LookAt(lookat);
		
		// rotate the collision box
		m_body.transform.LookAt(lookat);
		
		base.Update();
		
		// decelorate
		if (!m_colliding)
			m_volocity -= ((m_volocity * AccelerationRate) * 2.0f);
	}
	 
	/*
	 * \brief Called when this first collides with something
	*/
	void OnCollisionEnter(Collision collision) {			
		// spin through all the points of contact
        foreach (ContactPoint contact in collision.contacts) {
			// check the normal to see if the collision is in the horizontal plain
			if (!m_colliding && (contact.normal.y < 0.1 && contact.normal.y > -0.1))
			{
				// send them back the other way
				m_volocity *= -0.05f;
				m_colliding = true;
			}
			
			if (contact.normal.y >= 0.2) {
				m_playerState = PlayerState.Standing;
			}	
        }
	}
	
	/*
	 * \brief Called when this leaves a collosion
	*/
	void OnCollisionExit(Collision collision) {
		// we are no longer colliding
		m_colliding = false;
	}
	
}
