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

    private CCamera         m_cameraClass = null;

    //Jumping + wall jumping 

    private bool            m_canJump = true;

    private bool            m_wallJumpEnabled = false;

    public float           m_remainingWallStickTime = 0;

    private bool            m_wallStick = false;

    public float m_remainingWallJumpCooldown = 0;

	
	/* ----------------
	    Public Members 
	   ---------------- */
	
	public float			PlayerPathRadius = 10.0f;		//!< The radius of the circluar path the player should take
	
	public float			InitialAlphaPosition = 0.0f;	//!< The initial point on the circle where the player will start
	
	public Camera			MainCamera = null;				//!< The main viewport camera, which will follow the player
	
	public float			PlayerJumpHeight = 250.0f;		//!< The amount of force (in the y-axis) jump is represented by
	
	public float			AccelerationRate = 0.05f;		//!< The rate of acceleration
	
	public float			MaxSpeed = 0.5f;				//!< The maximum speed of the player

    public float            WallStickTime = 0.25f;

    public float            WallJumpCooldown = 1.0f;
	

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


        m_remainingWallStickTime = WallStickTime;
        m_remainingWallJumpCooldown = WallJumpCooldown;
	}
	
	/*
	 * \brief Called once per frame
	*/
    public  void Update()
    {
        

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


        //Sticky walls
        if (m_wallStick)
        {
            //Diable the use of gravity to create that 'sticky' feeling
            rigidbody.useGravity = false;
            m_canJump = true;
            //Count down how long we are allowed to stick here
            m_remainingWallStickTime -= Time.deltaTime;
            //Stop the player using it's starting velocity to 'catapult' up a ledge
            m_position += new Vector3(0, -1 * Time.deltaTime, 0);
            //Forcable add gravity back once the players time is up.
            if (m_remainingWallStickTime < 0)
            {
                rigidbody.useGravity = true;
            }
        }
        else
        {
            rigidbody.useGravity = true;     
        }

        //Reset the jump cooldown
        if (m_remainingWallJumpCooldown > -1)
        {
            m_remainingWallJumpCooldown -= Time.deltaTime;
        }

        // handle jumping
        if ((m_playerState != PlayerState.Jumping || ( m_wallJumpEnabled && m_remainingWallJumpCooldown < 0)) && Input.GetKeyDown(KeyCode.Space) && m_canJump)
        {          
            if (m_wallJumpEnabled)
            {
                //Disable the jump and start the cooldown.
                m_wallJumpEnabled = false;
                m_remainingWallJumpCooldown = WallJumpCooldown;
            }
                m_body.AddForce(new Vector3(0.0f, PlayerJumpHeight, 0.0f));
                m_playerState = PlayerState.Jumping;
                rigidbody.useGravity = true;        
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

        base.Update();

        // decelorate
        if (!m_colliding)
            m_volocity -= ((m_volocity * AccelerationRate) * 2.0f);
    }
	 
	/*
	 * \brief Called when this first collides with something
	*/
	void OnCollisionEnter(Collision collision) 
    {

        //Check for walljumpable objects, and check we are within the remaining stick time, otherwise we can't jump.
        CSceneObject sObject = collision.gameObject.GetComponent<CSceneObject>();
        if (sObject)
        {
            if (sObject.CanWallJump)
            {       
                m_wallJumpEnabled = true;
                m_canJump = true;
               
            }
            if (m_remainingWallStickTime > 0)
            {
                m_wallStick = true;           
            }
        }   

		// spin through all the points of contact
        foreach (ContactPoint contact in collision.contacts) {
			// check the normal to see if the collision is in the horizontal plain
            if (!m_colliding && (contact.normal.y < 0.1 && contact.normal.y > -0.1))
            {
                // send them back the other way
                m_volocity *= -0.05f;
                m_colliding = true;
                m_canJump = false;
            }
            else
            {
               m_canJump = true;
            }
			
			if (contact.normal.y >= 0.2) {
				m_playerState = PlayerState.Standing;
                m_canJump = true;
			}	
        }
	}
	
	/*
	 * \brief Called when this leaves a collosion
	*/
	void OnCollisionExit(Collision collision)
    {
		// we are no longer colliding
		m_colliding = false;
        m_remainingWallStickTime = WallStickTime;
        m_wallStick = false;
	}
	
	/*
	 * \brief Called whilst a collision is taking place
	*/
	void OnCollisionStay(Collision collision) 
    {
        
		foreach (ContactPoint contact in collision.contacts) 
        {
            //Debug.Log(contact.normal);
			//Debug.DrawRay(contact.point, contact.normal);
			// slide down slopes
			if (contact.normal.y >= 0.1f && contact.normal.y < 0.95f) 
            {
				
				CSceneObject sceneObject = contact.otherCollider.GetComponent<CSceneObject>();
				float scale = 1.0f;
				if (sceneObject != null)
					scale = sceneObject.ExtraSlide;
				
				float direction = contact.normal.x < 0.0f ? -1.0f : 1.0f;
				m_volocity += ((((1 - contact.normal.y) * 0.25f) * direction) * scale);	
			}      
		}
	}
	
}
