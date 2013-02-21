using UnityEngine;
using System.Collections;

public class CSteamVentTrigger : CTriggerBase
{
	public GameObject PlayerEntity = null;
	public GameObject SteamVent = null;
	public float RangeFromTrigger = 3.62f;
	private CSteamVent m_steamVent = null;
	private bool m_showText = false;
	private bool m_TriggerEntered = false;
	
	new public void Start ()
	{
		m_steamVent = SteamVent.GetComponent<CSteamVent> ();			
	}
	
	void OnGUI ()
	{
		if (m_showText)
			GUI.Label (new Rect (10, 10, 280, 20), "Press Action Key To Disable Steam Vent!");
	}

	new public void Update ()
	{
		if (m_TriggerEntered) {
			m_showText = true;
			if (CheckContextButton ()) {
				m_steamVent.ToggleStreamLock (true);
				Animation lever = GetComponentInChildren<Animation> ();
				if (lever == null)
					lever.Play ("pull");
			}
		} else {
			m_showText = false;
		}
	}

	public void OnTriggerEnter (Collider other)
	{
		m_TriggerEntered = true;
	}

	public void OnTriggerExit (Collider other)
	{
		m_TriggerEntered = false;
	}
	
}
