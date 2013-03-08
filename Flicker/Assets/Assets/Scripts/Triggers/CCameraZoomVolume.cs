using UnityEngine;
using System.Collections;

public class CCameraZoomVolume : MonoBehaviour {
	
	public float ZoomLevel = 3.0f;
	
	private float m_oldZoomLevel = 3.0f; 
	private bool m_inTrigger = false;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	// Something entered the trigger area
	void OnTriggerEnter(Collider other) {
		if (!m_inTrigger) {
			m_inTrigger = true;
			CCamera camera = CCamera.GetInstance();
			m_oldZoomLevel = camera.DistanceFromPlayer;
			camera.DistanceFromPlayer = ZoomLevel;
			camera.MaxCamPositionsStored = 100;
		}
	}
	
	void OnTriggerExit(Collider other) {
		if (m_inTrigger) {
			m_inTrigger = false;
			CCamera camera = CCamera.GetInstance();
			camera.DistanceFromPlayer = m_oldZoomLevel;
			camera.MaxCamPositionsStored = 10;
		}
	}
}
