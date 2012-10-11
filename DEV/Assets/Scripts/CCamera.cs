using UnityEngine;
using System.Collections;

public class CCamera : MonoBehaviour {
	
	
	/* -----------------
	    Private Members 
	   ----------------- */
	private Transform m_transform;


	/* ----------------
	    Public Members 
	   ---------------- */
	
	public float CameraElevation = 0;
	
	public int					DistanceFromPlayer = 10;		//!< How far away from the player should the camera be

	/*
	 * \brief Called when the object is created. At the start.
	 *        Only called once per instaniation.
	*/
	public void Start () {
		
		m_transform = this.transform;
        
      	
	}
	
	/*
	 * \brief Called once per frame
	*/
	public void Update () {

       
	
	}
    public void SetLookAt(Vector3 lookAt)
    {
        m_transform.LookAt(lookAt);
    }
    public void SetPosition(Vector3 position)
    {
        m_transform.position = position + new Vector3(0.0f,CameraElevation,0.0f);
    }
}
