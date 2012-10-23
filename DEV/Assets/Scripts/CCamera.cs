using UnityEngine;
using System.Collections;

public class CCamera : MonoBehaviour {
	
	
	/* -----------------
	    Private Members 
	   ----------------- */
	private Transform 			m_transform;					//!< 
	
	private float				m_cameraOffset = 0.0f;			//!< 

	/* ----------------
	    Public Members 
	   ---------------- */
	
	public float 				CameraElevation = 0;			//!< 
	
	public int					DistanceFromPlayer = 10;		//!< How far away from the player should the camera be
	
	public float				MaxCameraOffset = 10.0f;		//!< 

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
	
	public void TendToMaxOffset(float direction)
	{
		if (direction != 0.0f) 
		{
			if (direction < 0 && m_cameraOffset == -MaxCameraOffset) return;	
			if (direction > 0 && m_cameraOffset == MaxCameraOffset) return;	
			
			m_cameraOffset += (direction * 0.1f);
			
			if (m_cameraOffset > MaxCameraOffset) m_cameraOffset = MaxCameraOffset;
			if (m_cameraOffset < -MaxCameraOffset) m_cameraOffset = -MaxCameraOffset;
		}
		else
		{
			m_cameraOffset *= 0.96f;
		}
	}
	
	public float CameraOffset {
		get {
			return m_cameraOffset;	
		}
	}
	
    public void SetLookAt(Vector3 lookAt)
    {
        m_transform.LookAt(lookAt);
    }
	
    public void SetPosition(Vector3 position)
    {
        m_transform.position = position + new Vector3(0,CameraElevation,0);
    }
}
