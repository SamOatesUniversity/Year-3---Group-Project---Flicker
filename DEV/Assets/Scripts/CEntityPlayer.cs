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
	
    private CCamera 		m_cameraClass = null;					//!< Todo: Haydn fill these in.
	
	private bool 			m_canWallJump = false;					//!< Todo: Haydn fill these in.
	
	private int				m_direction = 0;						//!< Stores the direction, 0 = not moving, 1 = left, -1 = right
	
	private CSceneObject	m_lastWallJumpObject = null;
	
	private int				m_startWallTime = 0;
	
	private bool			m_canJump = false;
	
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
        m_cameraClass = MainCamera.GetComponent<CCamera>();
	}
	
	/*
	 * \brief Called once per frame
	*/
	public override void Update () {
					
		// handle movement to the left and right
		if (!m_colliding)
		{
			float input = Input.GetAxis("Horizontal");
			m_volocity += input * AccelerationRate;
			if (m_volocity > MaxSpeed) m_volocity = MaxSpeed;
			if (m_volocity < -MaxSpeed) m_volocity = -MaxSpeed;
			
			m_direction = m_volocity != 0 ? m_volocity > 0 ? 1 : -1 : 0;
		}
		
		m_playerPositionAlpha -= m_volocity;
					
		m_position = new Vector3(
			Mathf.Sin(m_playerPositionAlpha * Mathf.Deg2Rad) * PlayerPathRadius,
			transform.position.y,
			Mathf.Cos(m_playerPositionAlpha * Mathf.Deg2Rad) * PlayerPathRadius
		);
		
		// handle jumping
		if (m_playerState != PlayerState.Jumping && Input.GetKeyDown(KeyCode.Space) && !m_colliding && m_canJump) {
			m_body.AddForce(new Vector3(0.0f, PlayerJumpHeight , 0.0f));	
			m_playerState = PlayerState.Jumping;
		}
		
		if (m_canWallJump && Input.GetKeyDown(KeyCode.Space))
		{
			m_body.AddForce(new Vector3(0.0f, PlayerJumpHeight * 1.1f, 0.0f));	
			m_volocity = (-m_direction) * 0.5f;
			m_playerState = PlayerState.Jumping;
		}
		
        //position the lookat
		Vector3 lookat = new Vector3(0.0f, transform.position.y, 0.0f);
		// position the camera
	    Vector3 camPostion = new Vector3(
            Mathf.Sin(m_playerPositionAlpha * Mathf.Deg2Rad) * (m_cameraClass.DistanceFromPlayer + PlayerPathRadius),
			transform.position.y,
            Mathf.Cos(m_playerPositionAlpha * Mathf.Deg2Rad) * (m_cameraClass.DistanceFromPlayer + PlayerPathRadius)	
		);

        m_cameraClass.SetPosition(camPostion);
        m_cameraClass.SetLookAt(lookat);

		//MainCamera.transform.LookAt(lookat);
				
		base.Update();
		
		// decelorate
		if (!m_colliding)
			m_volocity -= ((m_volocity * AccelerationRate) * 2.0f);
	}
	
	/*
	 * \brief public function to disable jumping
	*/
	public void SetJumping()
	{
		m_playerState = PlayerState.Jumping;
	}
	
	public void AddHorizontalForce(float force)
	{
		m_volocity += force;
	}
	
	public float GetVelocity()
	{
		return m_volocity;
	}
	
	/*
	 * \brief Called when this first collides with something
	*/
	void OnCollisionEnter(Collision collision) {
		CRope mount = collision.gameObject.GetComponent<CRope>();
		if (mount)
		{
			CRopeMount m = mount.parent;
			m.SetPlayerAttached(true);
		}

		// spin through all the points of contact
        foreach (ContactPoint contact in collision.contacts) {
			// check the normal to see if the collision is in the horizontal plain
			if (!m_colliding && (contact.normal.y < 0.1 && contact.normal.y > -0.1))
			{
				// send them back the other way
				m_volocity = 0.0f;
				m_colliding = true;
				m_startWallTime = 0;
			}
			
			if (contact.normal.y >= 0.2) {
				m_playerState = PlayerState.Standing;
				m_lastWallJumpObject = null;
			}			
        }

		CSceneObject sObject = collision.gameObject.GetComponent<CSceneObject>();
		if (sObject && (m_lastWallJumpObject == null || m_lastWallJumpObject != sObject) && sObject.CanWallJump)
		{
			m_canWallJump = true;
			m_lastWallJumpObject = sObject;
		}
		
		m_canJump = true;
	}
	
	/*
	 * \brief Called when this leaves a collosion
	*/
	void OnCollisionExit(Collision collision) {
		
		CRope mount = collision.gameObject.GetComponent<CRope>();
		if (mount)
		{
			CRopeMount m = mount.parent;
		//	m.SetPlayerAttached(false);
		}
		
		m_canWallJump = false;
		m_colliding = false;
		m_canJump = false;
	}
	
	/*
	 * \brief Called whilst a collision is taking place
	*/
	void OnCollisionStay(Collision collision) {
		
		m_canJump = true;
		
		if (m_colliding && m_startWallTime == 10)
		{
			m_volocity = (-m_direction) * 0.1f;
			m_startWallTime = -1;	
		}
		m_startWallTime++;
		
		foreach (ContactPoint contact in collision.contacts) {
			Debug.DrawRay(contact.point, contact.normal, Color.green);
	
			// don't slide on points that arnt on the floor
			float yContact = collision.transform.position.y - contact.point.y;
			if (yContact >= 0.5f)
				continue;
			
			// slide down slopes
			if (!isNearly(contact.normal.x, 0.0f) || !isNearly(contact.normal.z, 0.0f) ) {
				CSceneObject sceneObject = contact.otherCollider.GetComponent<CSceneObject>();
				float scale = 1.0f;
				if (sceneObject != null)
					scale = sceneObject.ExtraSlide;
				
				float direction = contact.normal.x < 0.0f ? -1.0f : 1.0f;
				m_volocity += ((((1 - contact.normal.y) * 0.25f) * direction) * scale);	
				return;
			}
		}
		
		if (!m_canWallJump) {
			foreach (ContactPoint contact in collision.contacts) {
				float yContact = collision.transform.position.y - contact.point.y;
				if (yContact < 0.5f)
					continue;
				
				// we must be colliding if we are in this method
				m_colliding = true;
				m_volocity = (-m_direction) * 0.1f;
				return;			
			}
		}
		
	}
	
	/*
	 * \brief Works out if a value is almost another value (for floating point accuracy)
	*/
	private bool isNearly(float x, float amount) {
	
		if (x < amount - 0.01f) return false;
		if (x > amount + 0.01f) return false;
		return true;
		
	}
}
