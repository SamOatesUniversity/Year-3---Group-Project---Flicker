using UnityEngine;
using System.Collections;

public class CTriggerAirship : MonoBehaviour {

	public bool					EnableAirship = true;
	private CEntityAirship		m_airship = null;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter(Collider collider)
	{
		if(!m_airship)
		{
			m_airship = CEntityAirship.GetInstance();
		}
		if(EnableAirship)
		{
			m_airship.gameObject.SetActiveRecursively(true);
			//m_airship.StartCutScene();
			CEntityCaptain captain = CEntityCaptain.GetInstance();
			if (captain != null) {
				captain.StartCutScene();
			}
		}
		else
		{
			m_airship.gameObject.SetActiveRecursively(false);
		}
	}
}
