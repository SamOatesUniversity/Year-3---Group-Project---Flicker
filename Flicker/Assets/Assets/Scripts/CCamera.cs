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
	
	public float 				CameraElevation = 1.0f;			//!< 
	
	public float				InitialDistanceFromPlayer = 0f;		//!< How far away from the player should the camera be
	public float				DistanceFromPlayer = 0f;
	public float 				MinimumCameraDistance = 1f;
	public float				MaximumCameraDistance = 3f;
	
	public float 				CameraZoomTimerMs = 3000;
	
	public float				MaxCameraOffset = 1.0f;		//!< 
	
	private Transform			m_playerPelvis;
	private CEntityPlayer 		m_playerEntity;
	private Vector3				m_pelvisOffset;
	
	private ArrayList			m_storedPositions;
	
	public int m_maxPositionsStored = 10;

	/*
	 * \brief Called when the object is created. At the start.
	 *        Only called once per instaniation.
	*/
	public void Start () {	
		m_transform = this.transform;
		DistanceFromPlayer = InitialDistanceFromPlayer;
		m_playerEntity = this.transform.FindChild("../Player Spawn").GetComponent<CEntityPlayer>();
		m_playerPelvis = this.transform.FindChild("../Player Spawn/Player_Mesh/Bip001/Bip001 Pelvis");
		m_storedPositions = new ArrayList();
		m_storedPositions.Add(m_playerPelvis.position);
	}

	/*
	 * \brief Called once per frame
	*/
	public void FixedUpdate () {
		m_storedPositions.Add(m_playerPelvis.position);
		
		if( m_storedPositions.Count > m_maxPositionsStored )
		{
			m_storedPositions.RemoveAt(0);
		}
		
		Vector3 sumPositions = new Vector3(0.0f, 0.0f, 0.0f);
		foreach( Vector3 pos in m_storedPositions )
		{
			sumPositions += pos;
		}
		Vector3 avgPosition = sumPositions / m_storedPositions.Count;
		
		Transform player = m_playerPelvis;
		this.TendToMaxOffset( m_playerEntity.Physics.Direction );
	
		
		/*
		float adjustedCameraHeight = player.position.y + CameraElevation;
		Vector3 playerToCamera = new Vector3(0.0f, adjustedCameraHeight, 0.0f) - player.position;
		Vector3 normPlayerToCamera = Vector3.Normalize( playerToCamera );
		Vector3 camPosition = player.position + ( normPlayerToCamera * DistanceFromPlayer );
		*/
		float adjustedCameraHeight = avgPosition.y + CameraElevation;
		Vector3 camPosition;
		
		if( m_playerEntity.Physics.InsideTower )
		{
			Vector3 playerToCamera = new Vector3(0.0f, adjustedCameraHeight, 0.0f) - avgPosition;
			Vector3 normPlayerToCamera = Vector3.Normalize( playerToCamera );
			camPosition = avgPosition + ( normPlayerToCamera * DistanceFromPlayer );
		}
		else
		{
			Vector3 playerToOrigin = new Vector3(0.0f, avgPosition.y, 0.0f) - avgPosition;
			Vector3 normPlayerToOrigin = Vector3.Normalize( playerToOrigin );
			camPosition = avgPosition - ( normPlayerToOrigin * DistanceFromPlayer );
			camPosition.y += CameraElevation;
		}
	
		m_transform.position = camPosition;
		this.SetLookAt( avgPosition );
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
		//Debug.Log(lookAt);
        m_transform.LookAt(lookAt);
    }
	
    public void SetPosition(Vector3 localposition)
    {
    	//m_transform.localPosition = localposition + new Vector3(-m_cameraOffset + DistanceFromPlayer, CameraElevation, 0);
    }
}
