using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CGUIMainMenu : MonoBehaviour {
	
	public Texture			Title = null;
	public Texture			YoutubeTexture = null;
	public Texture			FacebookTexture = null;
	
	enum MenuState {
		Main,
		Play,
		Continue,
		Options,
		Exit,
		None
	};
	
	private MenuState		m_menuState = MenuState.Main;
	
	public GUISkin MainMenuSkin = null;
	
	private Vector2		m_itemCenter = Vector2.zero;
	
	private struct FlickerLevel {
		public FlickerLevel(string iname, string oname) 
		{ 
			internalName = iname; 
			name = oname; 
		}
		
		public string internalName;
		public string name;
	}
	
	private List<FlickerLevel> m_levels = new List<FlickerLevel>();
	private Vector2 m_scrollPosition = Vector2.zero;
	
	private string m_highlighted = "";
	private bool m_usingController = false;
	private float m_changeTime = 0.0f;
	private bool m_pressedOK = false;
		
	// Use this for initialization
	void Start () {
		
		m_levels.Add(new FlickerLevel("Level_1-1", "Baby Steps"));
		m_levels.Add(new FlickerLevel("Level_1-2", "Hop, Skip and Don't Fall"));
		m_levels.Add(new FlickerLevel("Level_1-3", "The Ol' Switcheroo"));
		m_levels.Add(new FlickerLevel("Level_1-4", "On and On"));
		m_levels.Add(new FlickerLevel("Level_1-5", "No Time To Spare"));
		m_levels.Add(new FlickerLevel("Level_1-6", "Peg It!"));
		m_levels.Add(new FlickerLevel("Level_1-7", "Batteries Not Included"));
		m_levels.Add(new FlickerLevel("Level_1-8", "A Shocking Solution"));
		m_levels.Add(new FlickerLevel("Level_1-9", "Eye Spy"));
		m_levels.Add(new FlickerLevel("Level_1-10", "Argh!"));
		m_levels.Add(new FlickerLevel("Level_BOSS", "End Game"));
		
	}
	
	// Update is called once per frame
	void Update () {
	
		float upDown = Input.GetAxis("Vertical");
		float leftRight = Input.GetAxis("Horizontal");
		if (upDown != 0.0f || leftRight != 0.0f) 
		{
			if (m_usingController == false)
			{
				m_highlighted = "play";
				m_usingController = true;
				m_changeTime = Time.time;
				return;
			}		
			
			if (Time.time - m_changeTime > 0.2f)
			{
				GoToNextConrol(upDown, leftRight);
				m_changeTime = Time.time;	
			}
		}	
		
		if (Input.GetButtonUp("Jump")) {
			m_pressedOK = true;	
		}
	}
		
	void OnGUI() {
	
		GUI.skin = MainMenuSkin;
		m_itemCenter = new Vector2((Screen.width * 0.3f) - (Title.width * 0.5f), 0.0f);
		
		Rect logo = new Rect(m_itemCenter.x, m_itemCenter.y, Title.width, Title.height);
		GUI.Label(logo, Title);
		
		switch (m_menuState)
		{
		case MenuState.Main:
			DoMainMenu();
			break;
			
		case MenuState.Play:
			Application.LoadLevel("Level_1-1");
			break;
			
		case MenuState.Continue:
			DoContinueMenu();
			break;
			
		case MenuState.Options:
			CGUIOptions.GetInstance().OnGUI(true);
			if (CGUIOptions.GetInstance().GetMenuState() == CGUIOptions.PausedMenuState.Main)
			{
				m_menuState = MenuState.Main;
				m_highlighted = "options";
				m_pressedOK = false;	
			}
				
			if (m_highlighted == "back" && m_pressedOK)
			{
				m_menuState = MenuState.Main;
				m_highlighted = "options";
				m_pressedOK = false;					
			}
			
			break;
			
		case MenuState.Exit:
			DoQuitMenu();
			break;			
		};
		
		if (m_usingController)
		{
			GUI.FocusControl (m_highlighted);	
		}
		
	}
	
	private void DoContinueMenu()
	{
		m_scrollPosition = GUI.BeginScrollView (
			new Rect(m_itemCenter.x - 10.0f, m_itemCenter.y + 260.0f, Title.width + 20.0f, 320.0f), 
			m_scrollPosition, 
			new Rect(0, 0, Title.width, m_levels.Count * 60.0f), 
			false, true
		);
		
		Rect buttonPosition = new Rect(0, 0, Title.width, 48.0f);
		foreach (FlickerLevel level in m_levels)
		{
			GUI.SetNextControlName (level.internalName);
			if (GUI.Button(buttonPosition, level.name) || (m_pressedOK && m_highlighted == level.internalName))
			{
				Application.LoadLevel(level.internalName);
			}
			buttonPosition.y += 60;
		}
		
		GUI.EndScrollView();
			
		buttonPosition = new Rect(m_itemCenter.x, m_itemCenter.y + 260.0f + 320.0f + 92.0f, Title.width, 48.0f);
		GUI.SetNextControlName ("back");
		if (GUI.Button(buttonPosition, "Back") || (m_pressedOK && m_highlighted == "back"))
		{
			m_menuState = MenuState.Main;
			m_highlighted = "levelselect";
			m_pressedOK = false;
		}
	}
	
	private void DoQuitMenu()
	{
		Vector2 labelSize = MainMenuSkin.GetStyle("label").CalcSize(new GUIContent("Are you sure you want to quit?"));
		Rect areYouSure = new Rect(m_itemCenter.x + ((Title.width * 0.5f) - (labelSize.x * 0.5f)), m_itemCenter.y + 320.0f, labelSize.x, 48);
		GUI.Label(areYouSure, "Are you sure you want to quit?");
		
		Rect yesRect = new Rect(m_itemCenter.x, m_itemCenter.y + 380.0f, (Title.width / 2) - 5, 48);
		GUI.SetNextControlName ("yes");
		if (GUI.Button(yesRect, "Yes")|| (m_pressedOK && m_highlighted == "yes"))
		{
			Application.Quit();
		}
		
		Rect noRect = new Rect(m_itemCenter.x + (Title.width / 2) + 5, m_itemCenter.y + 380.0f, (Title.width / 2) - 5, 48);
		GUI.SetNextControlName ("no");
		if (GUI.Button(noRect, "No")|| (m_pressedOK && m_highlighted == "no"))
		{
			m_menuState = MenuState.Main;
			m_highlighted = "quitgame";
			m_pressedOK = false;
		}
	}
	
	private void DoMainMenu()
	{
		Rect playRect = new Rect(m_itemCenter.x, m_itemCenter.y + 320.0f, Title.width, 48);
		GUI.SetNextControlName ("play");
		if (GUI.Button(playRect, "New Game") || (m_pressedOK && m_highlighted == "play"))
		{
			m_menuState = MenuState.Play;
		}
		
		Rect continueRect = new Rect(m_itemCenter.x, m_itemCenter.y + 380.0f, Title.width, 48);
		GUI.SetNextControlName ("levelselect");
		if (GUI.Button(continueRect, "Level Select") || (m_pressedOK && m_highlighted == "levelselect"))
		{
			m_menuState = MenuState.Continue;
			m_highlighted = "Level_1-1";
			m_pressedOK = false;
		}
		
		Rect optionsRect = new Rect(m_itemCenter.x, m_itemCenter.y + 440.0f, Title.width, 48);
		GUI.SetNextControlName ("options");
		if (GUI.Button(optionsRect, "Options") || (m_pressedOK && m_highlighted == "options"))
		{
			m_menuState = MenuState.Options;
			m_highlighted = "graphics";
			m_pressedOK = false;
		}
		
		Rect quitRect = new Rect(m_itemCenter.x, m_itemCenter.y + 560.0f, Title.width, 48);
		GUI.SetNextControlName ("quitgame");
		if (GUI.Button(quitRect, "Quit Game") || (m_pressedOK && m_highlighted == "quitgame"))
		{
			m_menuState = MenuState.Exit;
			m_highlighted = "no";
			m_pressedOK = false;
		}
		
		Rect youtubeRect = new Rect(Screen.width - 74, Screen.height - 74, 64, 64);
		if (GUI.Button (youtubeRect, YoutubeTexture))
		{
			Application.OpenURL("http://www.youtube.com/user/FlickerGame");
		}
		
		Rect facebookRect = new Rect(Screen.width - 148, Screen.height - 74, 64, 64);
		if (GUI.Button (facebookRect, FacebookTexture))
		{
			Application.OpenURL("http://www.facebook.com/FlickerTheGame");
		}
	}
	
	///////////////////////////////////////////////////////////////////////////////////////
	
	private void GoToNextConrol(float upDown, float leftRight)
	{
		if (m_menuState == MenuState.Main)
		{
			GoToNextMainMenuConrol(upDown, leftRight);
			return;	
		}
		
		if (m_menuState == MenuState.Exit)
		{
			GoToNextExitConrol(upDown, leftRight);
			return;	
		}
		
		if (m_menuState == MenuState.Options)
		{
			GoToNextOptionsConrol(upDown, leftRight);
			return;	
		}
		
		if (m_menuState == MenuState.Continue)
		{
			GoToNextContinueConrol(upDown, leftRight);
			return;	
		}

	}
	
	private void GoToNextContinueConrol(float upDown, float leftRight)
	{
		if (upDown == 0.0f)
			return;
		
		float moveamount = 48.0f;
		
		if (upDown < 0.0f)
		{
			if (m_highlighted == "back")
			{
				m_highlighted = "Level_1-1";
				m_scrollPosition.y = 0;
				return;	
			}
			
			if (m_highlighted == "Level_1-1")
			{
				m_highlighted = "Level_1-2";
				m_scrollPosition.y += moveamount;
				return;	
			}
			
			if (m_highlighted == "Level_1-2")
			{
				m_highlighted = "Level_1-3";
				m_scrollPosition.y += moveamount;
				return;	
			}
			
			if (m_highlighted == "Level_1-3")
			{
				m_highlighted = "Level_1-4";
				m_scrollPosition.y += moveamount;
				return;	
			}
			
			if (m_highlighted == "Level_1-4")
			{
				m_highlighted = "Level_1-5";
				m_scrollPosition.y += moveamount;
				return;	
			}
			
			if (m_highlighted == "Level_1-5")
			{
				m_highlighted = "Level_1-6";
				m_scrollPosition.y += moveamount;
				return;	
			}
			
			if (m_highlighted == "Level_1-6")
			{
				m_highlighted = "Level_1-7";
				m_scrollPosition.y += moveamount;
				return;	
			}
			
			if (m_highlighted == "Level_1-7")
			{
				m_highlighted = "Level_1-8";
				m_scrollPosition.y += moveamount;
				return;	
			}
			
			if (m_highlighted == "Level_1-8")
			{
				m_highlighted = "Level_1-9";
				m_scrollPosition.y += moveamount;
				return;	
			}
			
			if (m_highlighted == "Level_1-9")
			{
				m_highlighted = "Level_1-10";
				m_scrollPosition.y += moveamount;
				return;	
			}
			
			if (m_highlighted == "Level_1-10")
			{
				m_highlighted = "Level_BOSS";
				m_scrollPosition.y += moveamount;
				return;	
			}
			
			if (m_highlighted == "Level_BOSS")
			{
				m_highlighted = "back";
				return;	
			}
		}
		else
		{
			if (m_highlighted == "back")
			{
				m_highlighted = "Level_BOSS";
				m_scrollPosition.y = 600.0f;
				return;	
			}
			
			if (m_highlighted == "Level_BOSS")
			{
				m_highlighted = "Level_1-10";
				m_scrollPosition.y -= moveamount;
				return;	
			}
			
			if (m_highlighted == "Level_1-10")
			{
				m_highlighted = "Level_1-9";
				m_scrollPosition.y -= moveamount;
				return;	
			}
			
			if (m_highlighted == "Level_1-9")
			{
				m_highlighted = "Level_1-8";
				m_scrollPosition.y -= moveamount;
				return;	
			}
			
			if (m_highlighted == "Level_1-8")
			{
				m_highlighted = "Level_1-7";
				m_scrollPosition.y -= moveamount;
				return;	
			}
			
			if (m_highlighted == "Level_1-7")
			{
				m_highlighted = "Level_1-6";
				m_scrollPosition.y -= moveamount;
				return;	
			}
			
			if (m_highlighted == "Level_1-6")
			{
				m_highlighted = "Level_1-5";
				m_scrollPosition.y -= moveamount;
				return;	
			}
			
			if (m_highlighted == "Level_1-5")
			{
				m_highlighted = "Level_1-4";
				m_scrollPosition.y -= moveamount;
				return;	
			}
			
			if (m_highlighted == "Level_1-4")
			{
				m_highlighted = "Level_1-3";
				m_scrollPosition.y -= moveamount;
				return;	
			}
			
			if (m_highlighted == "Level_1-3")
			{
				m_highlighted = "Level_1-2";
				m_scrollPosition.y -= moveamount;
				return;	
			}
			
			if (m_highlighted == "Level_1-2")
			{
				m_highlighted = "Level_1-1";
				m_scrollPosition.y -= moveamount;
				return;	
			}
			
			if (m_highlighted == "Level_1-1")
			{
				m_highlighted = "back";
				m_scrollPosition.y = 0;
				return;	
			}
			
			
		}
	}
	
	private void GoToNextOptionsConrol(float upDown, float leftRight)
	{
		
		if (upDown != 0.0f)
		{
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
		
		if (leftRight != 0.0f)
		{
			int newLevel = leftRight > 0.0f ? 1 : -1;
			QualitySettings.SetQualityLevel(QualitySettings.GetQualityLevel() + newLevel);
		}
	}
	
	private void GoToNextExitConrol(float upDown, float leftRight)
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
	
	private void GoToNextMainMenuConrol(float upDown, float leftRight)
	{
		if (upDown == 0.0f)
			return;
		
		if (upDown < 0.0f)
		{
			if (m_highlighted == "play")
			{
				m_highlighted = "levelselect";
				return;
			}
			if (m_highlighted == "levelselect")
			{
				m_highlighted = "options";
				return;
			}
			if (m_highlighted == "options")
			{
				m_highlighted = "quitgame";
				return;
			}
			if (m_highlighted == "quitgame")
			{
				m_highlighted = "play";
				return;
			}
		}
		else
		{
			if (m_highlighted == "play")
			{
				m_highlighted = "quitgame";
				return;
			}
			if (m_highlighted == "levelselect")
			{
				m_highlighted = "play";
				return;
			}
			if (m_highlighted == "options")
			{
				m_highlighted = "levelselect";
				return;
			}
			if (m_highlighted == "quitgame")
			{
				m_highlighted = "options";
				return;
			}			
		}
	}
	
}
