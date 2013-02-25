using UnityEngine;
using System.Collections;

public class CSteamVent : MonoBehaviour 
{
	public GameObject SteamParticleSystem = null;
	
	public GameObject SteamCollisionBox = null;
	
	public float SteamDuration = 3.0f;
	
	public float SteamIntervals = 10.0f;

    public float SteamOffset = 0.0f;
	
	
	
	
	private ParticleSystem m_pSystem = null;
	
	private GameObject m_collisionBox = null;
		
	private Vector3 m_colliderScale = new Vector3(1f,0.5f,0.5f);
		
	private bool m_streamOn = false;
	
	private float m_currentSteam = 0.0f;
	
	private float m_timeSinceLastBurst = 0;
	
	private int m_timeIncrement = 1;
	
	private bool m_lockStream = false;
	
	
	
	public void Start () {
		
		if (SteamParticleSystem)
		{		
			m_collisionBox =  (GameObject)Instantiate(SteamCollisionBox, transform.position, Quaternion.identity);
			m_collisionBox.transform.parent = this.transform;
			m_collisionBox.transform.rotation = this.transform.rotation;
			m_collisionBox.transform.Translate( new Vector3(0,0,-3.7f));
			m_collisionBox.transform.localScale = m_colliderScale;
			m_collisionBox.renderer.enabled = false;
			
			GameObject obj = (GameObject)Instantiate(SteamParticleSystem, m_collisionBox.transform.position, Quaternion.identity);
			m_pSystem = obj.GetComponent<ParticleSystem>();
			m_pSystem.transform.parent = m_collisionBox.transform;
			m_pSystem.transform.rotation = Quaternion.LookRotation( m_collisionBox.transform.position, Vector3.up);
			m_pSystem.enableEmission = true;

            if (SteamIntervals == 0)
            {
                SteamIntervals = SteamDuration;
                Debug.LogWarning("Do not set steam vent duration to zero, it removes collision");
            }
			
		}
		else
		{
			Debug.LogError("CSteamVent: SteamParticleSystem == null. Attach a particle system to '" + this.name + "'");
		}
			
	}
	
	private void ToggleStream(bool toggle)
	{
		
		//Break out early if it's setting the same value again.
		if (toggle == m_streamOn || m_pSystem == null) 
			return;
			
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
	
	public bool StreamOn {
		get {
			return m_streamOn;	
		}
	}
	
	public void ToggleStreamLock(bool toggle)
	{
		m_lockStream = toggle;
	}
	
	public bool GetLockStatus()
	{
		return m_lockStream;
	}
	
	
	
	public void Update () 
	{
		
		
		if (m_lockStream)
		{
			ToggleStream(false);
			return;
		}
		
		
		
		
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

