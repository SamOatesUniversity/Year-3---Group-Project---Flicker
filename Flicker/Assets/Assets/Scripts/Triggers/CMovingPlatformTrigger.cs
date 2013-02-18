using UnityEngine;
using System.Collections.Generic;

public class CMovingPlatformTrigger : CTriggerBase {
	
	public struct PlatformStruct {
		public GameObject platform;
		public bool and;
	};
	
	public PlatformStruct[] 				Platforms = null;
		
	private bool 							m_triggerEntered = false;
	
	private MeshCollider 					m_selfCollider = null;
	
	private List<CSceneObjectPlatform>		m_platforms = null;
	
	// Use this for initialization
	void Start () {
		
		m_platforms = new List<CSceneObjectPlatform>();
		m_selfCollider = this.GetComponent<MeshCollider>();
		
		for (int platformIndex = 0; platformIndex < Platforms.Length; ++platformIndex)
		{
			CSceneObjectPlatform plat = Platforms[platformIndex].platform.GetComponent<CSceneObjectPlatform>();
			if (plat != null)
				m_platforms.Add(plat);
		}
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if (m_triggerEntered)
        {
			if (CheckContextButton())
            {
                for (int platformIndex = 0; platformIndex < m_platforms.Count; ++platformIndex)
				{
					m_platforms[platformIndex].ToggleTrigger();
				}
				
                Animation lever = GetComponentInChildren<Animation>();
                if (lever != null)
				{
                    lever.Play("pull");
				}
			}
        }
	}
	
	public void OnTriggerEnter(Collider other)
    {
        m_triggerEntered = true;
    }
	
    public void OnTriggerExit(Collider other)
    {
        m_triggerEntered = false;
    }
}
