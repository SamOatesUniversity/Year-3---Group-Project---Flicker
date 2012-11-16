using UnityEngine;
using System.Collections;

public class CSteamVentTrigger : CTriggerBase
{
	public GameObject PlayerEntity = null;
	
	public GameObject SteamVent = null;
	
	public float RangeFromTrigger = 0.5f;
	
	private CSteamVent m_steamVent = null;
	
	private CEntityPlayer m_playerEntity = null;
	
	private bool m_showText = false;
		
	public void Start () 
	{
		//m_playerEntity = PlayerEntity.GetComponent<CEntityPlayer>();
		//m_steamVent = SteamVent.GetComponent<CSteamVent>();
			
	}
	
	void OnGUI() 
	{
		if (m_showText)
        	GUI.Label(new Rect(10, 10, 280, 20), "Press R To Disable Steam Vent!");
    }
		
	public void Update () 
	{
		/*if ( RangeFromTrigger > CheckRange( m_playerEntity.transform.position, this.transform.position) && !m_steamVent.GetLockStatus())
		{
			m_showText = true;
			if (CheckContextButton())
			{
				m_steamVent.ToggleStreamLock(true);
				Animation lever = GetComponentInChildren<Animation>();
				if (lever == null)
					lever.Play("pull");
			}
		}
		else
		{
			m_showText = false;
		}*/
	}
}

