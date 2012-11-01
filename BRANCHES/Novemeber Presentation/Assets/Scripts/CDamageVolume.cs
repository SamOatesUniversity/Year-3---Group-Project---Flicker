using UnityEngine;
using System.Collections;

public class CDamageVolume : MonoBehaviour {
	
	public int Damage;           		  	//!< Amount of damage done every tick
	public bool EnabledByDefault;         	//!< Use to enable/disable damage volume on scene start
	public float TickTimer;					//!< Set the time that must lapse before damage applied again  
	
	private bool m_enabled;
	private float m_timeDamaged;
	// Use this for initialization
	void Start () {
		m_enabled = EnabledByDefault;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	//called if a collidable object triggers the volume
	 void OnTriggerEnter(Collider collider) {
		CEntityPlayer player = collider.gameObject.GetComponent<CEntityPlayer>();
		if (player)
		{
			player.DoDamage(Damage);
			m_timeDamaged = Time.time;
		}
    }
	
	void OnTriggerStay(Collider collider) {
		CEntityPlayer player = collider.gameObject.GetComponent<CEntityPlayer>();
		if (player)
		{
			if (Time.time >= (m_timeDamaged+TickTimer))
			{
				player.DoDamage(Damage);
				m_timeDamaged = Time.time;
			}
		}
	}
}
