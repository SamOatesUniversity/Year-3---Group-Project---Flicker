using UnityEngine;
using System.Collections;

public class CTriggerSecret : MonoBehaviour {
	
	private bool m_read = false;
	private bool m_reading = false;
	
	public Texture SecretLetter = null;
	
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
			GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), SecretLetter);			
		}
	
	}
	
	public void OnTriggerEnter(Collider other)
	{
		if (m_read && !m_reading)
			return;
		
		m_reading = true;
		Time.timeScale = 0.0f;
		
	}
}
