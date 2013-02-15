using UnityEngine;
using System.Collections;

public class CSceneSetup : MonoBehaviour {
	
	private bool m_hasSetup = false;

	// Use this for initialization
    void Start() {
        Application.LoadLevelAdditive("Level_1-3");
    }
	
	// Update is called once per frame
	void Update () {
		
		if (m_hasSetup)
			return;
		
		GameObject scene = GameObject.Find("Scene_1-3");
		if (scene != null)
		{
			m_hasSetup = true;
			scene.transform.position = new Vector3(scene.transform.position.x, 20.0f * 0.64f, scene.transform.position.z);
		}
	
	}
}
