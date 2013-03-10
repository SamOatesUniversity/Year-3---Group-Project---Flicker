using UnityEngine;
using System.Collections;

public class CSteamVent : CSceneObjectBase 
{
	public bool				Enabled = true;
	private ParticleSystem	m_particle = null;
	
	public GameObject		BladeAnimation = null;
	private Animation		m_anim = null;
	
	public float			DeactiveTime = 0.0f;
	private float			m_offTime = 0.0f;
	
	public void Start () 
	{
		if (BladeAnimation != null)
		{
			m_anim = BladeAnimation.GetComponent<Animation>();	
		}
		
		m_particle = GetComponentInChildren<ParticleSystem>();
		if (m_particle == null)
		{
			Debug.LogWarning("Could not find particle system");	
			return;
		}
	}
		
	public void Update () 
	{
		if (m_particle == null)
			return;
		
		if (DeactiveTime > 0.0f && Time.time - m_offTime > DeactiveTime)
		{
			Enabled = true;
		}
		
		m_particle.enableEmission = Enabled;
		if (m_anim != null)
		{
			if (Enabled) {
				m_anim.Play();
			} else {
				m_anim.Stop();
			}	
		}
	}
	
	void OnTriggerEnter(Collider collider)
	{
		if (collider.name != "Player Spawn")
			return;
		
		if (Enabled)
		{
			CEntityPlayer.GetInstance().PushPlayerFromTower();	
		}
	}
	
	public override void LogicStateChange(bool newState) {
		if (newState)
		{
			Enabled = false;
			m_offTime = Time.time;
		}
	}
}

