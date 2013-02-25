using UnityEngine;
using System.Collections;

public class CGUIMainMenu : MonoBehaviour {
	
	public Texture			Title = null;
	
	enum MenuState {
		Main,
		Play,
		Continue,
		Exit
	};
	
	private MenuState		m_menuState = MenuState.Main;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI() {
	
		Rect logo = new Rect(120, 100, Title.width, Title.height);
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
			
		case MenuState.Exit:
			DoQuitMenu();
			break;			
		};
		
	}
	
	private void DoContinueMenu()
	{
		string[] levels = {"Level_1-1", "Level_1-2", "Level_1-3", "Level_1-4", "Level_1-5", "Level_1-6"}; 
		Rect selectionRect = new Rect(100, 320, Title.width, 120);
		int selected = GUI.SelectionGrid(selectionRect, -1, levels, levels.Length);
		if (selected != -1)
		{
			string level = levels[selected];
			Application.LoadLevel(level);
		}
		
		Rect backRect = new Rect(100, 500, Title.width, 48);
		if (GUI.Button(backRect, "Back"))
		{
			m_menuState = MenuState.Main;
		}
	}
	
	private void DoQuitMenu()
	{
		Rect areYouSure = new Rect(268, 320, 600, 48);
		GUI.Label(areYouSure, "Are you sure you want to quit?");
		
		Rect yesRect = new Rect(100, 380, (Title.width / 2) - 5, 48);
		if (GUI.Button(yesRect, "Yes"))
		{
			Application.Quit();
		}
		
		Rect noRect = new Rect(100 + (Title.width / 2) + 5, 380, (Title.width / 2) - 5, 48);
		if (GUI.Button(noRect, "No"))
		{
			m_menuState = MenuState.Main;
		}
	}
	
	private void DoMainMenu()
	{
		Rect playRect = new Rect(100, 320, Title.width, 48);
		if (GUI.Button(playRect, "Start"))
		{
			m_menuState = MenuState.Play;
		}
		
		Rect continueRect = new Rect(100, 380, Title.width, 48);
		if (GUI.Button(continueRect, "Load Level"))
		{
			m_menuState = MenuState.Continue;
		}
		
		Rect quitRect = new Rect(100, 440, Title.width, 48);
		if (GUI.Button(quitRect, "Exit"))
		{
			m_menuState = MenuState.Exit;
		}
	}
}
