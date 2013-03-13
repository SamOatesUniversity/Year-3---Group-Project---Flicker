using UnityEngine;
using System.Collections;

public class CCamera : MonoBehaviour {
	
	/* -----------------
	    Private Members 
	   ----------------- */
	private Transform 			m_transform;					//!< 
	
	private float				m_cameraOffset = 0.0f;			//!< 
		
	private static CCamera				INSTANCE = null;

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
	private Transform			m_currentTransform;
	
	private CEntityPlayer 		m_playerEntity;
	
	private ArrayList			m_storedPositions;
	private ArrayList			m_storedCameraPositions;
	private Vector3				m_averagePos;
	private int 				m_countIgnoredFrames;
	
	public int 					MaxPositionsStored = 10;
	public int 					MaxCamPositionsStored = 10;
	
	public GUISkin MainMenuSkin = null; 
	public Texture2D PauseMenuBackground = null;
	
	public static CCamera GetInstance() {
		return INSTANCE;	
	}
	
	/*
	 * \brief Called when the object is created. At the start.
	 *        Only called once per instaniation.
	*/
	public void Start () {	
		
		INSTANCE = this;
		
		m_transform = this.transform;
		DistanceFromPlayer = InitialDistanceFromPlayer;
		m_playerEntity = CEntityPlayer.GetInstance();
		if (m_playerEntity == null) {
			m_playerEntity = GameObject.Find("Player Spawn").GetComponent<CEntityPlayer>();	
		}
		m_playerPelvis = m_playerEntity.transform.FindChild("Player_Mesh/Bip001/Bip001 Pelvis");
		
		m_currentTransform = m_playerPelvis;
		
		m_storedPositions = new ArrayList();
		m_storedPositions.Add(m_currentTransform.position);
		m_countIgnoredFrames = 0;
		
		m_storedCameraPositions = new ArrayList();
	}
	
	public void SetLookAtTransform(Transform newLookat)
	{
		m_currentTransform = newLookat;
	}
	
	public void ResetLookAtTransform()
	{
		m_currentTransform = m_playerPelvis;	
	}
	
	public void ClearFrames()
	{
		Vector3 lastposition = (Vector3)m_storedPositions[m_storedPositions.Count-1];
		m_storedPositions.Clear();
		
		// keep one frame, so it smoothly changes, rather than jumping
		m_storedPositions.Add(lastposition);
	}

	/*
	 * \brief Called once per frame
	*/
	public void FixedUpdate () {
		int maxFramesIgnorePos = 1;
		float maxPosJumpFactor = 0.5f;//0.05f;
		bool posIgnored = true;
		if( m_countIgnoredFrames < maxFramesIgnorePos )
		{
			if( m_currentTransform.position.y > m_averagePos.y-maxPosJumpFactor && m_currentTransform.position.y < m_averagePos.y+maxPosJumpFactor )
			{
					m_storedPositions.Add(m_currentTransform.position);
					posIgnored = false;
			}
		}
		else
		{
			m_storedPositions.Add(m_currentTransform.position);
			posIgnored = false;
		}
		
		if( posIgnored )
		{
			m_countIgnoredFrames++;
		}
		else
		{
			m_countIgnoredFrames = 0;
		}
		
		int removeAmount = 2;
		while( m_storedPositions.Count > MaxPositionsStored )
		{
			m_storedPositions.RemoveAt(0);
			removeAmount--;
			if (removeAmount <= 0)
				break;
		}
		
		Vector3 sumPositions = new Vector3(0.0f, 0.0f, 0.0f);
		
		foreach( Vector3 pos in m_storedPositions )
		{
			sumPositions += pos;
		}
		Vector3 avgPosition = sumPositions / m_storedPositions.Count;
		
		this.TendToMaxOffset( m_playerEntity.Physics.Direction );
		m_averagePos = avgPosition;
		
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
		
		m_storedCameraPositions.Add(camPosition);
		removeAmount = 1;
		while( m_storedCameraPositions.Count > MaxCamPositionsStored )
		{
			m_storedCameraPositions.RemoveAt(0);
			removeAmount--;
			if (removeAmount <= 0)
				break;
		}
		
		Vector3 sumCamPositions = new Vector3(0.0f, 0.0f, 0.0f);
		
		foreach( Vector3 pos in m_storedCameraPositions )
		{
			sumCamPositions += pos;
		}
		Vector3 avgCamPosition = sumCamPositions / m_storedCameraPositions.Count;
			
		if (m_playerEntity.GetPlayerState() != PlayerState.FallingFromTower)
			m_transform.position = avgCamPosition;
		
		this.SetLookAt( avgPosition );
	}
	
	void Update() {
		if (m_playerEntity.CurrentGameState == GameState.Running)
			return;
		
		CGUIOptions.GetInstance().OnUpdate();
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
	
	void OnGUI() {
	
		if (m_playerEntity.CurrentGameState == GameState.Running)
			return;
		
		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), PauseMenuBackground);
		
		GUI.skin = MainMenuSkin;
		CGUIOptions.GetInstance().OnGUI(false);		
	}
}
