using UnityEngine;
using System.Collections;

public class CCamera : MonoBehaviour {
	
	/* -----------------
	    Private Members 
	   ----------------- */
	private Transform 			m_transform;					//!< 
	
	private float				m_cameraOffset = 0.0f;			//!< 
	
	private enum PausedMenuState {
		Main,
		Restart,
		Options,
		MainMenu,
		Quit
	};
	
	private PausedMenuState		m_pausedMenuState = PausedMenuState.Main;

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
	private Vector3				m_averagePos;
	private int 				m_countIgnoredFrames;
	
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
		m_countIgnoredFrames = 0;
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
			if( m_playerPelvis.position.y > m_averagePos.y-maxPosJumpFactor && m_playerPelvis.position.y < m_averagePos.y+maxPosJumpFactor )
			{
					m_storedPositions.Add(m_playerPelvis.position);
					posIgnored = false;
			}
		}
		else
		{
			m_storedPositions.Add(m_playerPelvis.position);
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
		
		this.TendToMaxOffset( m_playerEntity.Physics.Direction );
		m_averagePos = avgPosition;
		
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
	
		if (m_playerEntity.GetPlayerState() != PlayerState.FallingFromTower)
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
        m_transform.LookAt(lookAt);
    }
	
	
	void OnGUI()
	{
		if (m_playerEntity.CurrentGameState == GameState.Running)
			return;
		
		switch (m_pausedMenuState)
		{
		case PausedMenuState.Main:
			OnMainPausedMenu();
			break;
			
		case PausedMenuState.Options:
			OnOptionsMenu();
			break;
			
		case PausedMenuState.MainMenu:
		case PausedMenuState.Quit:
		case PausedMenuState.Restart:
			AreYouSureMenu();
			break;
		};		
	}
	
	private void OnOptionsMenu()
	{
		const float BUTTON_WIDTH = 256;
		float yPosition = 100;
		
		string pausedText = "Options";
		Vector2 textDimensions = GUI.skin.label.CalcSize(new GUIContent(pausedText));
		Rect pausedLabelRect = new Rect((Screen.width * 0.5f) - (textDimensions.x * 0.5f), yPosition, textDimensions.x, textDimensions.y);
		GUI.Label(pausedLabelRect, pausedText);
		
		yPosition += 96;
		string[] graphicsLevels = QualitySettings.names;		
		Rect selectionRect = new Rect((Screen.width * 0.5f) - 256, yPosition, 512, 96);
		int selected = GUI.SelectionGrid(selectionRect, QualitySettings.GetQualityLevel(), graphicsLevels, graphicsLevels.Length);
		if (selected != -1)
		{
			QualitySettings.SetQualityLevel(selected);
		}
		
		yPosition += 96 + 42;
		Rect backRect = new Rect((Screen.width * 0.5f) - (BUTTON_WIDTH * 0.5f), yPosition, BUTTON_WIDTH, 32);
		if (GUI.Button(backRect, "Back"))
		{
			m_pausedMenuState = PausedMenuState.Main;
			return;
		}
	}
	
	private void AreYouSureMenu()
	{
		string labelText = "Are you sure you want to ";
		switch (m_pausedMenuState)
		{
		case PausedMenuState.MainMenu:
			labelText += "return to the main menu?";
			break;
		case PausedMenuState.Quit:
			labelText += "quit the game?";
			break;
		case PausedMenuState.Restart:
			labelText += "restart the current level?";
			break;
		};
		
		const float BUTTON_WIDTH = 256;
		float yPosition = 100;
		
		Vector2 textDimensions = GUI.skin.label.CalcSize(new GUIContent(labelText));
		Rect labelRect = new Rect((Screen.width * 0.5f) - (textDimensions.x * 0.5f), yPosition, textDimensions.x, textDimensions.y);
		GUI.Label(labelRect, labelText);
		
		yPosition += 96;
		
		Rect yesRect = new Rect((Screen.width * 0.5f) - (BUTTON_WIDTH * 0.5f) - 5, yPosition, BUTTON_WIDTH * 0.5f, 32);
		if (GUI.Button(yesRect, "Yes"))
		{
			if (m_pausedMenuState == PausedMenuState.Quit)
			{
				Application.Quit();	
			}
			else if (m_pausedMenuState == PausedMenuState.MainMenu)
			{
				Time.timeScale = 1.0f;
				Application.LoadLevel("Main_Menu");
			}		
			else if (m_pausedMenuState == PausedMenuState.Restart)
			{
				CCheckPoint cp = m_playerEntity.LastCheckPoint;
				CCheckPoint startCp = cp;
				while (cp.IsLevelStart == false)
				{
					cp = cp.NextCheckPoint;
					if (cp == startCp)
					{
						Debug.LogError("No check points in list where marked as level start");
						return;
					}
				}
				
				m_playerEntity.LastCheckPoint = cp;
				m_playerEntity.OnDeath();
				m_pausedMenuState = PausedMenuState.Main;
				
				Time.timeScale = 1.0f;
				m_playerEntity.CurrentGameState = GameState.Running;
			}
			
			return;
		}
		
		Rect noRect = new Rect((Screen.width * 0.5f) + 5, yPosition, BUTTON_WIDTH * 0.5f, 32);
		if (GUI.Button(noRect, "No"))
		{
			m_pausedMenuState = PausedMenuState.Main;
			return;
		}
	}
	
	private void OnMainPausedMenu()
	{
		const float BUTTON_WIDTH = 256;
		
		float yPosition = 100;
		
		string pausedText = "Paused";
		Vector2 textDimensions = GUI.skin.label.CalcSize(new GUIContent(pausedText));
		Rect pausedLabelRect = new Rect((Screen.width * 0.5f) - (textDimensions.x * 0.5f), yPosition, textDimensions.x, textDimensions.y);
		GUI.Label(pausedLabelRect, pausedText);
		
		yPosition += 96;
		Rect continueRect = new Rect((Screen.width * 0.5f) - (BUTTON_WIDTH * 0.5f), yPosition, BUTTON_WIDTH, 32);
		if (GUI.Button(continueRect, "Continue"))
		{
			m_playerEntity.CurrentGameState = GameState.Running;
			Time.timeScale = 1.0f;
			return;
		}
		
		yPosition += 42;
		Rect restartRect = new Rect((Screen.width * 0.5f) - (BUTTON_WIDTH * 0.5f), yPosition, BUTTON_WIDTH, 32);
		if (GUI.Button(restartRect, "Restart Level"))
		{
			m_pausedMenuState = PausedMenuState.Restart;
			return;
		}
		
		yPosition += 42;
		Rect optionsRect = new Rect((Screen.width * 0.5f) - (BUTTON_WIDTH * 0.5f), yPosition, BUTTON_WIDTH, 32);
		if (GUI.Button(optionsRect, "Options"))
		{
			m_pausedMenuState = PausedMenuState.Options;
			return;
		}
		
		yPosition += 84;
		Rect mainMenuRect = new Rect((Screen.width * 0.5f) - (BUTTON_WIDTH * 0.5f) - 5, yPosition, BUTTON_WIDTH * 0.5f, 32);
		if (GUI.Button(mainMenuRect, "Main Menu"))
		{
			m_pausedMenuState = PausedMenuState.MainMenu;
			return;
		}
		
		Rect quitRect = new Rect((Screen.width * 0.5f) + 5, yPosition, BUTTON_WIDTH * 0.5f, 32);
		if (GUI.Button(quitRect, "Quit"))
		{
			m_pausedMenuState = PausedMenuState.Quit;
			return;
		}
	}
	
}
