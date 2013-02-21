using UnityEngine;
using System.Collections;

public class CInsideOutsideChange : MonoBehaviour {
	
	public string				ThisLevel = "";
	public string				NextLevel = "";
	
	private bool				m_active = false;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter(Collider collision) {
		
		if (m_active)
			return;
		
		m_active = true;
		
		Debug.Log ("CHANGING SCENE");
				
		Application.LoadLevel(NextLevel);
		
	}
}
