using UnityEngine;
using System.Collections;

public class CCutSceneTrigger : MonoBehaviour {
	
	private bool					m_active = false;
	private CCamera					m_camera = null;
	private CEntityPlayer			m_player = null;
	private Animation				m_animation = null;
	
	public float					DistanceFromLookAt = 3.0f;
	private float					m_initialDistanceFromPlayer = 0.0f;
	
	public int						NumberOfLerpKeys = 60;
	private int						m_maxPositionsStored = 20;
	
	// Use this for initialization
	void Start () {
	
		m_animation = GetComponent<Animation>();
		
	}
	
	// Update is called once per frame
	void Update () {
	
		if (!m_active)
			return;
		
		m_camera.SetLookAtTransform(transform);
		m_camera.DistanceFromPlayer = DistanceFromLookAt;
		
	}
	
	// Something entered the trigger area
	void OnTriggerEnter(Collider other) {
		
		if (!enabled)
			return;
		
		if (m_animation == null || m_animation.GetClipCount() == 0)
		{
			Debug.LogWarning("Cutscene '" + name + "' is missing an animation!");
			return;
		}
		
		if (other.name != "Player Spawn")
			return;

		m_player = CEntityPlayer.GetInstance();
		if (m_player == null)
			return;

		m_camera = CCamera.GetInstance();
		if (m_camera == null)
			return;
				
		m_player.SetPlayerState(PlayerState.InCutScene);
		
		m_camera.ClearFrames();
		m_initialDistanceFromPlayer = m_camera.DistanceFromPlayer;
		m_maxPositionsStored = m_camera.MaxPositionsStored;
		m_camera.MaxPositionsStored = NumberOfLerpKeys;
		
		m_animation.Play();
		m_active = true;
		
	}
	
	// Called once the cutscene has ended
	void OnCutSceneEnd() {
		
		m_camera.ResetLookAtTransform();
		m_player.SetPlayerState(PlayerState.Standing);
		m_camera.DistanceFromPlayer = m_initialDistanceFromPlayer;
		m_camera.MaxPositionsStored = m_maxPositionsStored;
		m_active = false;
		enabled = false;
		
	}
}
