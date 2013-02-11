using UnityEngine;
using System.Collections.Generic;

public class CMovingPlatformTrigger : CTriggerBase {
	
	public GameObject platform1 = null;
	public GameObject platform2 = null;
	public GameObject platform3 = null;
	public GameObject platform4 = null;
	
	private bool m_TriggerEntered = false;
	private MeshCollider selfCollider = null;
	 
	private List<CSceneObjectPlatform> platforms;
	
	// Use this for initialization
	void Start () {
		platforms = new List<CSceneObjectPlatform>();
		selfCollider = this.GetComponent<MeshCollider>();
		
		if (platform1 != null){
			CSceneObjectPlatform plat = platform1.GetComponent<CSceneObjectPlatform>();
			platforms.Add(plat);
		}
		if ( platform2 != null){
			CSceneObjectPlatform plat = platform2.GetComponent<CSceneObjectPlatform>();
			platforms.Add(plat);
		}
		if ( platform3 != null){
		CSceneObjectPlatform plat = platform3.GetComponent<CSceneObjectPlatform>();
			platforms.Add(plat);
		}
	
		if ( platform4 != null){
		CSceneObjectPlatform plat = platform4.GetComponent<CSceneObjectPlatform>();
			platforms.Add(plat);
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {

	if (m_TriggerEntered)
        {
			if (CheckContextButton())
            {
                for (int i = 0; i < platforms.Count; i++)
				{
					platforms[i].ToggleTrigger(true);
				}
                Animation lever = GetComponentInChildren<Animation>();
                if (lever == null)
                    lever.Play("pull");
			}
        }
	}
	 public void OnTriggerEnter(Collider other)
    {
        m_TriggerEntered = true;
    }
    public void OnTriggerExit(Collider other)
    {
        m_TriggerEntered = false;
    }
}
