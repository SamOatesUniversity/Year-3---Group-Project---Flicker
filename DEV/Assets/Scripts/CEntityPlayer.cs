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
			m_volocity = Input.GetAxis("Horizontal") * 0.5f;
		}
		
		m_playerPositionAlpha -= m_volocity;
					
		m_position = new Vector3(
			Mathf.Sin(m_playerPositionAlpha * Mathf.Deg2Rad) * PlayerPathRadius,
			transform.position.y,
			Mathf.Cos(m_playerPositionAlpha * Mathf.Deg2Rad) * PlayerPathRadius
		);
		
		// handle jumping
		if (m_playerState != PlayerState.Jumping && Input.GetKeyUp(KeyCode.Space) && !m_colliding) {
			m_body.AddForce(new Vector3(0.0f, 250.0f, 0.0f));	
			m_playerState = PlayerState.Jumping;
		}
		
		// position the camera
		CCamera cameraClass = MainCamera.GetComponent<CCamera>();
		MainCamera.transform.position = new Vector3(
			Mathf.Sin(m_playerPositionAlpha * Mathf.Deg2Rad) * (cameraClass.DistanceFromPlayer + PlayerPathRadius),
			transform.position.y,
			Mathf.Cos(m_playerPositionAlpha * Mathf.Deg2Rad) * (cameraClass.DistanceFromPlayer + PlayerPathRadius)	
		);
		MainCamera.transform.LookAt(new Vector3(0.0f, transform.position.y, 0.0f));
		
		base.Update();
	}
	 
	void OnCollisionEnter(Collision collision) {		
		if (m_playerState == PlayerState.Jumping) {
			m_playerState = PlayerState.Standing;	
		}
		
		if (m_colliding)
			return;
		
        foreach (ContactPoint contact in collision.contacts) {
            Debug.DrawRay(contact.point, contact.normal, Color.red);
			if (contact.normal.y < 0.95 && contact.normal.y > -0.95)
			{
				m_volocity *= -0.05f;
				m_colliding = true;
				return;
			}
        }
	}
	
	void OnCollisionExit(Collision collision) {
		m_colliding = false;
	}
	
}
