using UnityEngine;
using System.Collections;

public class CGUIOptions {
	
	private static CGUIOptions instance = null;
	
	public enum PausedMenuState {
		Main,
		Restart,
		Options,
		MainMenu,
		Quit
	};
	
	private PausedMenuState		m_pausedMenuState = PausedMenuState.Main;
	
	public static CGUIOptions GetInstance() 
	{
		if (instance == null)
			instance = new CGUIOptions();
		
		return instance;
	}
	
	public PausedMenuState GetMenuState() {
		return m_pausedMenuState;	
	}
	
	public void OnGUI(bool mainMenuOption)
	{
		if (mainMenuOption) {
			m_pausedMenuState = PausedMenuState.Options;
			OnOptionsMenu(true);
			return;
		}
		
		switch (m_pausedMenuState)
		{
		case PausedMenuState.Main:
			OnMainPausedMenu();
			break;
			
		case PausedMenuState.Options:
			OnOptionsMenu(false);
			break;
			
		case PausedMenuState.MainMenu:
		case PausedMenuState.Quit:
		case PausedMenuState.Restart:
			AreYouSureMenu();
			break;
		};		
	}
	
	private void OnOptionsMenu(bool mainMenuOption)
	{
		const float BUTTON_WIDTH = 256;
		float yPosition = 100;
		
		if (!mainMenuOption) {
			string pausedText = "Options";
			Vector2 textDimensions = GUI.skin.label.CalcSize(new GUIContent(pausedText));
			Rect pausedLabelRect = new Rect((Screen.width * 0.5f) - (textDimensions.x * 0.5f), yPosition, textDimensions.x, textDimensions.y);
			GUI.Label(pausedLabelRect, pausedText);
		}
		
		yPosition += 96;
		
		GUI.skin.button.fontSize = 22;		
		string[] graphicsLevels = QualitySettings.names;		
		Rect selectionRect = new Rect((Screen.width * 0.5f) - 256, yPosition, 512, 64);
		if (mainMenuOption)
			selectionRect = new Rect((Screen.width * 0.3f) - 256, yPosition + 120.0f, 512, 64);
			
		GUI.SetNextControlName ("graphics");
		int selected = GUI.SelectionGrid(selectionRect, QualitySettings.GetQualityLevel(), graphicsLevels, graphicsLevels.Length);
		if (selected != -1)
		{
			QualitySettings.SetQualityLevel(selected);
			
			Water water = null;
			GameObject waterObject = GameObject.Find("Nighttime Water");
			if (waterObject != null) water = waterObject.GetComponent<Water>();
			
			SSAOEffect ssao = null;
			if (CCamera.GetInstance())
				ssao = CCamera.GetInstance().GetComponent<SSAOEffect>();
						
			switch(selected)
			{
			case 0:
			case 1:
			{
				if (water != null)
				{
					water.m_WaterMode = Water.WaterMode.Simple;
					water.m_TextureSize = 64;
				}
				if (ssao != null) ssao.enabled = false;
			}
			break;
				
			case 2:
			case 3:
			{
				if (water != null)
				{
					water.m_WaterMode = Water.WaterMode.Reflective;
					water.m_TextureSize = 128;
				}
				if (ssao != null) ssao.enabled = false;
			}
			break;
				
			case 4:
			case 5:
			{
				if (water != null)
				{
					water.m_WaterMode = Water.WaterMode.Refractive;
					water.m_TextureSize = 256;
				}
				if (ssao != null) ssao.enabled = true;
			}
			break;
			}
		}
		
		GUI.skin.button.fontSize = 48;	
		
		yPosition += 64;		
		yPosition += 42;
		
		Rect backRect = new Rect((Screen.width * 0.5f) - (BUTTON_WIDTH * 0.5f), yPosition, BUTTON_WIDTH, 48);
		if (mainMenuOption)
			backRect = new Rect((Screen.width * 0.3f) - (BUTTON_WIDTH * 0.5f), yPosition + 120.0f, BUTTON_WIDTH, 48);
		GUI.SetNextControlName ("back");
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
				m_pausedMenuState = PausedMenuState.Main;
			}		
			else if (m_pausedMenuState == PausedMenuState.Restart)
			{
				CCheckPoint cp = CEntityPlayer.GetInstance().LastCheckPoint;
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
				
				CEntityPlayer.GetInstance().LastCheckPoint = cp;
				CEntityPlayer.GetInstance().OnDeath();
				m_pausedMenuState = PausedMenuState.Main;
				
				Time.timeScale = 1.0f;
				CEntityPlayer.GetInstance().CurrentGameState = GameState.Running;
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
		Rect continueRect = new Rect((Screen.width * 0.5f) - (BUTTON_WIDTH * 0.5f), yPosition, BUTTON_WIDTH, 48);
		if (GUI.Button(continueRect, "Continue"))
		{
			CEntityPlayer.GetInstance().CurrentGameState = GameState.Running;
			Time.timeScale = 1.0f;
			return;
		}
		
		yPosition += 42;
		Rect restartRect = new Rect((Screen.width * 0.5f) - (BUTTON_WIDTH * 0.5f), yPosition, BUTTON_WIDTH, 48);
		if (GUI.Button(restartRect, "Restart Level"))
		{
			m_pausedMenuState = PausedMenuState.Restart;
			return;
		}
		
		yPosition += 42;
		Rect optionsRect = new Rect((Screen.width * 0.5f) - (BUTTON_WIDTH * 0.5f), yPosition, BUTTON_WIDTH, 48);
		if (GUI.Button(optionsRect, "Options"))
		{
			m_pausedMenuState = PausedMenuState.Options;
			return;
		}
		
		yPosition += 84;
		Rect mainMenuRect = new Rect((Screen.width * 0.5f) - (BUTTON_WIDTH * 0.5f), yPosition, BUTTON_WIDTH, 48);
		if (GUI.Button(mainMenuRect, "Main Menu"))
		{
			m_pausedMenuState = PausedMenuState.MainMenu;
			return;
		}
		
		yPosition += 42;
		Rect quitRect = new Rect((Screen.width * 0.5f) - (BUTTON_WIDTH * 0.5f), yPosition, BUTTON_WIDTH, 48);
		if (GUI.Button(quitRect, "Quit"))
		{
			m_pausedMenuState = PausedMenuState.Quit;
			return;
		}
	}
}
