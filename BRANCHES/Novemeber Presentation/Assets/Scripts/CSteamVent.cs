using UnityEngine;
using System.Collections;

public class CSteamVent : MonoBehaviour 
{
	public GameObject SteamParticleSystem = null;
	
	private ParticleSystem m_pSystem = null;
	
	
	public void Start () {
		
		if (SteamParticleSystem)
		{		
			Instantiate(SteamParticleSystem, transform.position, Quaternion.identity);
			//m_pSystem.enableEmission = true;
		}
		else
		{
			Debug.Log("CSteamVent: SteamParticleSystem == null. Attach a particle system");
		}
			
	}
	
	
	public void Update () {
		
	}
}

