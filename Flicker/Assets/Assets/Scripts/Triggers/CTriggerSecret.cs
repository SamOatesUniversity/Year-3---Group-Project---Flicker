 using UnityEngine;
using System.Collections;

public class CTriggerSecret : MonoBehaviour {
	
	private bool m_read = false;
	private bool m_reading = false;
	
	public Texture SecretLetter = null;
	public Texture PressToContinue = null;
	
	private float m_flashTime = 0.0f;
	private bool m_showX = true;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if (m_reading && Input.GetButtonUp("Action"))
		{
			m_read = true;
			m_reading = false;
			Time.timeScale = 1.0f;
			GameObject.Destroy(this.gameObject);
		}		
		
	}
	
	void OnGUI()
	{
		if (m_read)
			return;
		
		if (m_reading)
		{
			float scale = 0.8f;
			GUI.DrawTexture(new Rect((Screen.width * 0.5f) - ((Screen.width * scale) * 0.5f), (Screen.height * 0.5f) - ((Screen.height * scale) * 0.5f) - (PressToContinue.height * 0.5f), Screen.width * scale, Screen.height * scale), SecretLetter);			
			
			if ((Time.realtimeSinceStartup - m_flashTime > 1.0f))
			{
				m_showX = !m_showX;
				m_flashTime = Time.realtimeSinceStartup;
			}
			
			if (PressToContinue != null && m_showX)
			{
				GUI.DrawTexture(new Rect((Screen.width * 0.5f) - (PressToContinue.width * 0.5f), Screen.height - PressToContinue.height, PressToContinue.width, PressToContinue.height), PressToContinue);	
			}
		}
	
	}
	
	public void OnTriggerEnter(Collider other)
	{
		if (m_read && !m_reading)
			return;
		
		m_reading = true;
		Time.timeScale = 0.0f;
		m_flashTime = Time.realtimeSinceStartup;
		
	}
}
