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
	
	public float				InitialDistanceFromPlayer = 3;		//!< How far away from the player should the camera be
	public float				DistanceFromPlayer = 3;
	public float 				MinimumCameraDistance = 1;
	public float				MaximumCameraDistance = 3;
	
	public float 				CameraZoomTimerMs = 3000;
	
	public float				MaxCameraOffset = 1.0f;		//!< 

	/*
	 * \brief Called when the object is created. At the start.
	 *        Only called once per instaniation.
	*/
	public void Start () {	
		m_transform = this.transform;
		DistanceFromPlayer = InitialDistanceFromPlayer;
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
			m_cameraOffset += (direction * 0.01f);
			
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
	
	public Vector3 WorldTransform {
		get {
			return m_transform.position;	
		}
	}
	
    public void SetLookAt(Vector3 lookAt)
    {
        m_transform.LookAt(lookAt);
    }
	
    public void SetPosition(Vector3 localposition)
    {
    	m_transform.localPosition = localposition + new Vector3(-m_cameraOffset, CameraElevation, 0);
    }
}
