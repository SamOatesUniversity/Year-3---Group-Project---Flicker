using UnityEngine;
using System.Collections;

public class CSceneSetup : MonoBehaviour {
	
	private bool                        m_hasSetup = false;                     //! Have we setup the scene position, unity require one tick to update streamed level transforms
	
	public string                       LastScene = null;						//! The name of the previous scene
	public string                       ThisScene = null;                       //! The name of this scene
    public string                       NextScene = null;                       //! The next scene to stream in
	
	public bool							RequiresDummyLevel = false;
	
	public Texture 						LoadingScreenTexture = null;
	private bool						m_showLoadingScreen = true;
	private float						m_loadCompleteTime = 0.0f;
	
	// Use this for initialization
    void Start() {
												
        if ((NextScene == null || NextScene.Length == 0) && (LastScene == null || LastScene.Length == 0))
        {
			m_showLoadingScreen = false;
            return;
        }
		
		GameObject[] allLevels = GameObject.FindGameObjectsWithTag("Level");
		
		if (!(NextScene == null || NextScene.Length == 0)) {
			
			bool alreadyLoaded = false;
			for (int levelIndex = 0; levelIndex < allLevels.Length; ++levelIndex)
			{
				if (allLevels[levelIndex].name == NextScene)
				{
					alreadyLoaded = true;
					break;
				}
			}
			
			if (!alreadyLoaded) {
				Application.LoadLevelAdditive(NextScene);
			}
		}
		
		if (!(LastScene == null || LastScene.Length == 0)) {
			
			bool alreadyLoaded = false;
			for (int levelIndex = 0; levelIndex < allLevels.Length; ++levelIndex)
			{
				if (allLevels[levelIndex].name == LastScene)
				{
					alreadyLoaded = true;
					break;
				}
			}
			
			if (!alreadyLoaded) {
				Application.LoadLevelAdditive(LastScene);
			}
		}
		
		if (allLevels.Length > 1)
		{
			GameObject scene = GameObject.Find(ThisScene);
			Transform playerXform = scene.transform.FindChild("Scene Setup");
			if (playerXform != null)
			{
				playerXform.gameObject.SetActiveRecursively(false);
			}
		}
    }
	
	void OnGUI() {
	
		if (LoadingScreenTexture == null || !m_showLoadingScreen)
			return;
		
		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), LoadingScreenTexture);
		
	}
	
	// Update is called once per frame
	void Update () {
		
		if ((NextScene == null || NextScene.Length == 0) && (LastScene == null || LastScene.Length == 0))
			return;
		
		if (m_hasSetup) {
			if (m_showLoadingScreen && (Time.time - m_loadCompleteTime > 0.1f)) {
				m_showLoadingScreen = false;	
			}
			return;
		}
		
		m_hasSetup = true;
	}
}
