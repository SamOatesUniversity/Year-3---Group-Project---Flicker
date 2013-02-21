using UnityEngine;
using System.Collections;
using UnityEditor;

public class CSceneSetup : MonoBehaviour {
	
	private bool                        m_hasSetup = false;                     //! Have we setup the scene position, unity require one tick to update streamed level transforms

    public string                       NextScene = null;                       //! The next scene to stream in
	
	public int							NumberOfFloors = 1;

	// Use this for initialization
    void Start() {
		
        if (NextScene == null || NextScene.Length == 0)
        {
			Debug.LogError("The next scene has not been set on a scene setup script. (" + name + ")");
            return;
        }

        Application.LoadLevelAdditive(NextScene);
    }
	
	// Update is called once per frame
	void Update () {
		
		if (NextScene == null || NextScene.Length == 0)
			return;
		
		if (m_hasSetup)
			return;
		
		GameObject scene = GameObject.Find(NextScene);
		if (scene != null)
		{
			m_hasSetup = true;
			scene.transform.position = new Vector3(scene.transform.position.x, this.transform.position.y + (NumberOfFloors * 0.64f), scene.transform.position.z);
		}
	
	}
}
