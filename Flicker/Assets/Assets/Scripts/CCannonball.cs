using UnityEngine;
using System.Collections;

public class CCannonball : MonoBehaviour {
	
	public float Speed = 1.0f;
	private Vector3 m_velocity; 
	
	// Use this for initialization
	void Start () {
		Vector3 adjustedOrigin = new Vector3(0.0f, this.transform.position.y, 0.0f);
		m_velocity = Speed*(adjustedOrigin - this.transform.position);
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.position += m_velocity;
	}
}
