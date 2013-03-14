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
	
	private bool m_isController = false;
	private string m_highlighted = "";
	private bool m_pressedOK = false;
	private float m_changeTime = 0.0f;
	
	public static CGUIOptions GetInstance() 
	{
		if (instance == null)
			instance = new CGUIOptions();
		
		return instance;
	}
	
	public PausedMenuState GetMenuState() {
		return m_pausedMenuState;	
	}
	
	public void OnUpdate()
	{
		if (Input.GetButtonUp("Jump"))
		{
			m_pressedOK = true;	
		}
		
		float upDown = Input.GetAxis("Vertical");
		float leftRight = Input.GetAxis("Horizontal");
		
		if (upDown != 0.0f || leftRight != 0.0f)
		{
			if (!m_isController)
			{
				m_highlighted = "continue";				
				m_isController = true;
				m_changeTime = Time.realtimeSinceStartup;
				return;
			}
	
			if (Time.realtimeSinceStartup - m_changeTime > 0.2f)
			{
				GoToNextConrol(upDown, leftRight);
				m_changeTime = Time.realtimeSinceStartup;
			}
		}			
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
		
		if (m_isController) 
		{
			GUI.FocusControl (m_highlighted);	
		}
	}
	
	public void ApplyOptions() {
	
		Water water = null;
		GameObject waterObject = GameObject.Find("Nighttime Water");
		if (waterObject != null) water = waterObject.GetComponent<Water>();
		
		SSAOEffect ssao = null;
		if (CCamera.GetInstance())
			ssao = CCamera.GetInstance().GetComponent<SSAOEffect>();
									
		switch(QualitySettings.GetQualityLevel())
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
			ApplyOptions();	
		}
		
		GUI.skin.button.fontSize = 48;	
		
		yPosition += 64;		
		yPosition += 42;
		
		Rect backRect = new Rect((Screen.width * 0.5f) - (BUTTON_WIDTH * 0.5f), yPosition, BUTTON_WIDTH, 48);
		if (mainMenuOption)
			backRect = new Rect((Screen.width * 0.3f) - (BUTTON_WIDTH * 0.5f), yPosition + 120.0f, BUTTON_WIDTH, 48);
		
		GUI.SetNextControlName ("back");
		if (GUI.Button(backRect, "Back") || (m_pressedOK && m_highlighted == "back"))
		{
			m_pressedOK = false;
			m_highlighted = "options";
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
		
		Rect yesRect = new Rect((Screen.width * 0.5f) - (BUTTON_WIDTH * 0.5f) - 5, yPosition, BUTTON_WIDTH * 0.5f, 48);
		GUI.SetNextControlName ("yes");
		if (GUI.Button(yesRect, "Yes") || (m_pressedOK && m_highlighted == "yes"))
		{
			m_pressedOK = false;
			m_highlighted = "continue";
			
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
				string currentLevel = CEntityPlayer.GetInstance().CurrentLevel;
				Time.timeScale = 1.0f;
				Application.LoadLevel(currentLevel);
				m_pausedMenuState = PausedMenuState.Main;
			}
			
			return;
		}
		
		Rect noRect = new Rect((Screen.width * 0.5f) + 5, yPosition, BUTTON_WIDTH * 0.5f, 48);
		GUI.SetNextControlName ("no");
		if (GUI.Button(noRect, "No") || (m_pressedOK && m_highlighted == "no"))
		{
			m_pausedMenuState = PausedMenuState.Main;
			m_pressedOK = false;
			m_highlighted = "continue";
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
		GUI.SetNextControlName ("continue");
		if (GUI.Button(continueRect, "Continue") || (m_pressedOK && m_highlighted == "continue"))
		{
			m_pressedOK = false;
			m_highlighted = "continue";
			CEntityPlayer.GetInstance().CurrentGameState = GameState.Running;
			Time.timeScale = 1.0f;
			CEntityPlayer.GetInstance().Physics.SkipNextJump();
			return;
		}
		
		yPosition += 42;
		Rect restartRect = new Rect((Screen.width * 0.5f) - (BUTTON_WIDTH * 0.5f), yPosition, BUTTON_WIDTH, 48);
		GUI.SetNextControlName ("restart");
		if (GUI.Button(restartRect, "Restart Level") || (m_pressedOK && m_highlighted == "restart"))
		{
			m_pressedOK = false;
			m_highlighted = "no";
			m_pausedMenuState = PausedMenuState.Restart;
			return;
		}
		
		yPosition += 42;
		Rect optionsRect = new Rect((Screen.width * 0.5f) - (BUTTON_WIDTH * 0.5f), yPosition, BUTTON_WIDTH, 48);
		GUI.SetNextControlName ("options");
		if (GUI.Button(optionsRect, "Options") || (m_pressedOK && m_highlighted == "options"))
		{
			m_pressedOK = false;
			m_highlighted = "graphics";
			m_pausedMenuState = PausedMenuState.Options;
			return;
		}
		
		yPosition += 84;
		Rect mainMenuRect = new Rect((Screen.width * 0.5f) - (BUTTON_WIDTH * 0.5f), yPosition, BUTTON_WIDTH, 48);
		GUI.SetNextControlName ("mainmenu");
		if (GUI.Button(mainMenuRect, "Main Menu") || (m_pressedOK && m_highlighted == "mainmenu"))
		{
			m_pressedOK = false;
			m_highlighted = "no";
			m_pausedMenuState = PausedMenuState.MainMenu;
			return;
		}
		
		yPosition += 42;
		Rect quitRect = new Rect((Screen.width * 0.5f) - (BUTTON_WIDTH * 0.5f), yPosition, BUTTON_WIDTH, 48);
		GUI.SetNextControlName ("quit");
		if (GUI.Button(quitRect, "Quit") || (m_pressedOK && m_highlighted == "quit"))
		{
			m_pressedOK = false;
			m_highlighted = "no";
			m_pausedMenuState = PausedMenuState.Quit;
			return;
		}
	}
	
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	private void GoToNextConrol(float upDown, float leftRight)
	{
		if (m_pausedMenuState == PausedMenuState.Main)
		{
			GoToNextMainControl(upDown, leftRight);
			return;
		}
		
		if (m_pausedMenuState == PausedMenuState.MainMenu || m_pausedMenuState == PausedMenuState.Restart || m_pausedMenuState == PausedMenuState.Quit)
		{
			GoToNextYesNoControl(upDown, leftRight);
			return;
		}
		
		if (m_pausedMenuState == PausedMenuState.Options)
		{
			GoToNextOptionsConrol(upDown, leftRight);
			return;
		}
	}
	
	private void GoToNextOptionsConrol(float upDown, float leftRight)
	{
		
		if (upDown != 0.0f)
		{
			m_pressedOK = false;
			if (m_highlighted == "graphics")
			{
				m_highlighted = "back";
				return;
			}
			
			if (m_highlighted == "back")
			{
				m_highlighted = "graphics";
				return;
			}
		}
		
		if (m_highlighted == "graphics" && leftRight != 0.0f)
		{
			int newLevel = leftRight > 0.0f ? 1 : -1;
			QualitySettings.SetQualityLevel(QualitySettings.GetQualityLevel() + newLevel);
			m_pressedOK = false;
		}
	}
	
	private void GoToNextYesNoControl(float upDown, float leftRight)
	{
		if (leftRight == 0.0f)
			return;
		
		if (m_highlighted == "no")
		{
			m_highlighted = "yes";
			return;
		}
		
		if (m_highlighted == "yes")
		{
			m_highlighted = "no";
			return;
		}
	}
		
	private void GoToNextMainControl(float upDown, float leftRight)
	{
		if (upDown == 0.0f)
			return;
		
		if (upDown < 0.0f)
		{
			if (m_highlighted == "continue")
			{
				m_highlighted = "restart";
				return;
			}
			
			if (m_highlighted == "restart")
			{
				m_highlighted = "options";
				return;
			}
			
			if (m_highlighted == "options")
			{
				m_highlighted = "mainmenu";
				return;
			}
			
			if (m_highlighted == "mainmenu")
			{
				m_highlighted = "quit";
				return;
			}
			
			if (m_highlighted == "quit")
			{
				m_highlighted = "continue";
				return;
			}
		}
		else
		{
			if (m_highlighted == "continue")
			{
				m_highlighted = "quit";
				return;
			}
			
			if (m_highlighted == "quit")
			{
				m_highlighted = "mainmenu";
				return;
			}
			
			if (m_highlighted == "mainmenu")
			{
				m_highlighted = "options";
				return;
			}
			
			if (m_highlighted == "options")
			{
				m_highlighted = "restart";
				return;
			}
			
			if (m_highlighted == "restart")
			{
				m_highlighted = "continue";
				return;
			}	
		}
	}
}

