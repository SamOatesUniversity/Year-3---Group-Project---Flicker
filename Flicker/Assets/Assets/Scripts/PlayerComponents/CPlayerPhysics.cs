using UnityEngine;
using System.Collections;

public class CPlayerPhysics : MonoBehaviour {
	
	/* -----------------
	    Private Members 
	   ----------------- */
	
	private float			m_velocity = 0.0f;						//!< The current speed of the player
	
	private Rigidbody		m_body = null;							//!< The rigid body component of this entity 
	
	private bool			m_colliding = false;					//!< Is the player colliding with anything
	
	private int				m_direction = 0;						//!< Stores the direction, 0 = not moving, 1 = left, -1 = right
	
	private bool			m_canJump = false;						//!< Can the player jump
	
	/* ----------------
	    Public Members 
	   ---------------- */		
	
	public float			PlayerJumpHeight = 5.0f;		//!< The amount of force (in the y-axis) jump is represented by
	
	public float			AccelerationRate = 0.05f;		//!< The rate of acceleration
	
	public float			MaxSpeed = 0.5f;				//!< The maximum speed of the player 
	
	/*
	 * \brief Initialise anything we don't know at construct time
	*/
	public void Create(Rigidbody body)
	{
		m_body = body;
	}
	
	/*
	 * \brief Called on player update
	*/
	public void OnUpdate(ref PlayerState playerState)
	{
		// handle movement to the left and right
		if (!m_colliding)
		{
			float input = Input.GetAxis("Horizontal");
			m_velocity += input * AccelerationRate;
			if (m_velocity > MaxSpeed) m_velocity = MaxSpeed;
			if (m_velocity < -MaxSpeed) m_velocity = -MaxSpeed;
			
			m_direction = (m_velocity != 0) ? (m_velocity > 0) ? 1 : -1 : 0;
		}	
		
		// handle jumping
		if (playerState != PlayerState.Jumping && Input.GetKeyDown(KeyCode.Space) && !m_colliding && m_canJump) {
			m_body.AddForce(new Vector3(0.0f, PlayerJumpHeight , 0.0f), ForceMode.Impulse);	
			playerState = PlayerState.Jumping;
		}
		
		// decelorate
		if (!m_colliding)
			m_velocity -= ((m_velocity * AccelerationRate) * 2.0f);
		
		if (playerState != PlayerState.Jumping)
		{
			if (!isNearly(m_velocity, 0, 0.1f)) 
			{
				playerState = PlayerState.Walking;
			}
			else
			{
				playerState = PlayerState.Standing;
			}
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
	
	/*
	 * \brief Gets the players last known direction
	*/
	public float Direction {
		get {
			return m_direction;
		}
	}
	
	/*
	 * \brief Gets and sets if the player can jump
	*/
	public bool CanJump {
		get {
			return m_canJump;
		}
		set {
			m_canJump = value;	
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
	public bool IsColliding {
		get {
			return m_colliding;
		}
	}
	
	/*
	 * \brief Works out if a value is almost another value (for floating point accuracy)
	*/
	public void CallOnCollisionContactEnter(ContactPoint contact)
	{
		// check the normal to see if the collision is in the horizontal plain
		if (!m_colliding && (contact.normal.y < 0.1 && contact.normal.y > -0.1))
		{
			m_velocity = 0.0f;
			m_colliding = true;
		}
	}
	
	/*
	 * \brief Called when the player leaves a collosion
	*/
	public void CallOnCollisionExit(Collision collision)
	{
		m_colliding = false;
		m_canJump = false;
	}
	
	/*
	 * \brief Called whilst a collision is taking place
	*/
	public void CallOnCollisionStay(Collision collision, ref CWallJump wallJump)
	{
		m_canJump = true;
		
		/*
		foreach (ContactPoint contact in collision.contacts) {
			Debug.DrawRay(contact.point, contact.normal, Color.green);
	
			// don't slide on points that arnt on the floor
			float yContact = collision.transform.position.y - contact.point.y;
			if (yContact >= 0.5f)
				continue;
			
			// slide down slopes
			if (!isNearly(contact.normal.x, 0.0f, 0.01f) || !isNearly(contact.normal.z, 0.0f, 0.01f) ) {
				CSceneObject sceneObject = contact.otherCollider.GetComponent<CSceneObject>();
				float scale = 1.0f;
				if (sceneObject != null)
					scale = sceneObject.ExtraSlide;
				
				float direction = contact.normal.x < 0.0f ? -1.0f : 1.0f;
				m_velocity += ((((1 - contact.normal.y) * 0.25f) * direction) * scale);	
				return;
			}
		}
		*/
		
		if (!wallJump.GetCanWallJump()) {
			foreach (ContactPoint contact in collision.contacts) {
				float yContact = collision.transform.position.y - contact.point.y;
				if (yContact < 0.5f)
					continue;
				
				// we must be colliding if we are in this method
				m_colliding = true;
				m_velocity = (-m_direction) * 0.25f;
				return;			
			}
		}
	}
	
	/*
	 * \brief Works out if a value is almost another value (for floating point accuracy)
	*/
	public static bool isNearly(float x, float amount, float varience) {
	
		if (x < amount - varience) return false;
		if (x > amount + varience) return false;
		return true;
		
	}
}
