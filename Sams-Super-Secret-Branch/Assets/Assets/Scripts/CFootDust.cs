using UnityEngine;
using System.Collections;

[RequireComponent (typeof (GameObject))]

public class CFootDust : MonoBehaviour {
	
	public GameObject DustEffect;
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter(Collider collider)
	{
		Object footstep = Instantiate(DustEffect, transform.position, transform.rotation);
		Destroy(footstep, 2);
	}
}
