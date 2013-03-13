using UnityEngine;
using System.Collections;

public class CTriggerMonkey : MonoBehaviour {

	private CEntityMonkey		m_monkey = null;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

	}
	
	void OnTriggerEnter(Collider collider)
	{
		if(!m_monkey)
		{
			m_monkey = CEntityMonkey.GetInstance();
		}
		m_monkey.DoAnimation();
	}
}
