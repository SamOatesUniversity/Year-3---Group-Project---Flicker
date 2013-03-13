using UnityEngine;
using System.Collections;

public class CSceneSetup : MonoBehaviour {
	
	private bool                        m_hasSetup = false;                     //! Have we setup the scene position, unity require one tick to update streamed level transforms
	
	public string                       ThisScene = null;                       //! The next scene to stream in
    public string                       NextScene = null;                       //! The next scene to stream in
	
	public int							NumberOfFloors = 1;
	
	public Texture 						LoadingScreenTexture = null;
	private bool						m_showLoadingScreen = true;
	private float						m_loadCompleteTime = 0.0f;
	
	// Use this for initialization
    void Start() {
				
		GameObject[] allLevels = GameObject.FindGameObjectsWithTag("Level");
		
		if (allLevels.Length > 1)
		{
			GameObject scene = GameObject.Find(ThisScene);
			Transform playerXform = scene.transform.FindChild("Scene Setup");
			if (playerXform != null)
			{
				playerXform.gameObject.SetActiveRecursively(false);
			}
		}
				
        if (NextScene == null || NextScene.Length == 0)
        {
			m_showLoadingScreen = false;
            return;
        }
		
		Application.LoadLevelAdditive(NextScene);
    }
	
	void OnGUI() {
	
		if (LoadingScreenTexture == null || !m_showLoadingScreen)
			return;
		
		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), LoadingScreenTexture);
		
	}
	
	// Update is called once per frame
	void Update () {
		
		if (NextScene == null || NextScene.Length == 0)
			return;
		
		if (m_hasSetup) {
			if (m_showLoadingScreen && (Time.time - m_loadCompleteTime > 2.0f)) {
				m_showLoadingScreen = false;	
			}
			return;
		}
		
		GameObject scene = GameObject.Find(NextScene);
		if (scene != null)
		{
			m_hasSetup = true;
			m_loadCompleteTime = Time.time;
			scene.transform.position = new Vector3(scene.transform.position.x, this.transform.position.y + (NumberOfFloors * 0.64f), scene.transform.position.z);
		}
	}
}
