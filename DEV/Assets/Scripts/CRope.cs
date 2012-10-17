using UnityEngine;
using System.Collections;

public class CRope : MonoBehaviour {
	
	public CRopeMount parent;
	
	private float m_velocity = 0;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	
	
	}
	void OnCollisionEnter(Collision collision) 
	{
		foreach (ContactPoint contact in collision.contacts) 
		{
			Debug.DrawRay(contact.point, contact.normal, Color.green);
			parent.direction = contact.normal.x < 0.0f ? -1.0f : 1.0f;
		}
		
	}
}
