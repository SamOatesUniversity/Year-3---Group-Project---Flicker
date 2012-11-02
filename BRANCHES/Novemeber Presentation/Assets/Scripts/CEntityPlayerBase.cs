using UnityEngine;
using System.Collections;

public class CEntityPlayerBase : MonoBehaviour {
	
	/* -------------------
	    Protected Members 
	   ------------------- */

	protected Vector3		m_position;				//!< The current position of the entity
	
	protected string		m_name;					//!< The name of the entity (used on guis)
	
	/* ----------------
	    Public Members 
	   ---------------- */
	
	public int				MaxHealth = 100;
	
	protected int 			m_playerHealth = 0;
	

	/*
	 * \brief Called when the object is created. At the start.
	 *        Only called once per instaniation.
	*/
	public virtual void Start () {
		m_name = null;
		m_position = transform.position;		
	}
	
	/*
	 * \brief Called once per frame
	*/
	public virtual void Update () {
	
		// rotate the collision box
		Vector3 lookat = new Vector3(0.0f, transform.position.y, 0.0f);
		rigidbody.transform.LookAt(lookat);
		transform.position = m_position;
				
	}
}
