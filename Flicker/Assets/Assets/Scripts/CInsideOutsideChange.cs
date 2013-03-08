using UnityEngine;
using System.Collections;

public class CInsideOutsideChange : MonoBehaviour {
	
	public string				ThisLevel = "";
	public string				NextLevel = "";
	
	private bool				m_active = false;
	
	public Texture				LoadingScreenTexture = null;
	
	// Use this for initialization
	void Start () {
		m_active = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI() {
		
		if (!m_active || LoadingScreenTexture == null) {
			return;
		}
		
		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), LoadingScreenTexture);
		
	}
	
	void OnTriggerEnter(Collider collision) {
		
		if (m_active) {
			return;
		}
		
		m_active = true;
						
		Application.LoadLevel(NextLevel);
		
	}
}
