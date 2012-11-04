using UnityEngine;
using System.Collections;

public class CSteamVent : MonoBehaviour 
{
	public GameObject SteamParticleSystem = null;
	
	private ParticleSystem m_pSystem = null;
	
	public GameObject SteamCollisionBox = null;
	
	private GameObject m_collisionBox = null;
	
	
	private Vector3 m_colliderScale = new Vector3(1f,0.5f,0.5f);
	
	
	private bool m_streamOn = false;
	public float SteamDuration = 3.0f;
	private float m_currentSteam = 0.0f;
	public float SteamIntervals = 10.0f;
	private float m_timeSinceLastBurst = 0;
	private int m_timeIncrement = 1;
	
	
	
	public void Start () {
		
		if (SteamParticleSystem)
		{		
			//m_pSystem.transform.RotateAround(new Vector3(0,0,0),new Vector3(1,0,0), 180);
		//	m_pSystem.transform.Translate(new Vector3(0,0,0));
			
			m_collisionBox =  (GameObject)Instantiate(SteamCollisionBox, transform.position, Quaternion.identity);
			m_collisionBox.transform.parent = this.transform;
			m_collisionBox.transform.rotation = this.transform.rotation;
			m_collisionBox.transform.Translate( new Vector3(0,0,-3.7f));
			m_collisionBox.transform.localScale = m_colliderScale;
			m_collisionBox.renderer.enabled = false;
		
			
			GameObject obj = (GameObject)Instantiate(SteamParticleSystem, transform.position - new Vector3(0,0.2f,0), Quaternion.identity);
			m_pSystem = obj.GetComponent<ParticleSystem>();
			m_pSystem.transform.parent = m_collisionBox.transform;
			m_pSystem.transform.rotation = Quaternion.LookRotation( m_collisionBox.transform.position, Vector3.up);	
			m_pSystem.name = "VENTSYSTEM";
			
			
			
			m_pSystem.enableEmission = true;
		}
		else
		{
			Debug.Log("CSteamVent: SteamParticleSystem == null. Attach a particle system");
		}
			
	}
	
	private void ToggleStream(bool toggle)
	{
		
		//Break out early if it's setting the same value again.
		if (toggle == m_streamOn) { return; }
			
		m_streamOn = toggle;
		
		if (m_streamOn)
		{
			m_pSystem.enableEmission = true;
			m_collisionBox.collider.transform.localScale = m_colliderScale;
		}
		else
		{
			m_pSystem.enableEmission = false;
			m_collisionBox.collider.transform.localScale = Vector3.zero;			
		}
		
	}
	
	
	public void Update () 
	{
		
		if ( m_timeSinceLastBurst < SteamIntervals)
		{
			//TIME TO BURST!!
			if (m_currentSteam < SteamDuration)
			{
				ToggleStream(true);
				m_currentSteam += m_timeIncrement * Time.deltaTime;
			}
			else
			{
				ToggleStream(false);
			}
			
			m_timeSinceLastBurst += m_timeIncrement * Time.deltaTime;
		}
		else
		{
			m_timeSinceLastBurst = 0;
			m_currentSteam = 0;
		}
	}
}

