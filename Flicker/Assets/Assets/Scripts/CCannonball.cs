using UnityEngine;
using System.Collections;

public class CCannonball : MonoBehaviour {
	
	public float 			Speed = 0.02f;
	
	public float 			YOffset = 1.2f;
	
	public GameObject		Marker = null;
	public GameObject		Explosion = null;
	
	public float			DeathTimer = 0.5f;
	
	private Vector3 		m_velocity;
	private bool 			m_exploded;
	
	
	// Use this for initialization
	void Start ()
	{
		Vector3 adjustedOrigin = new Vector3(0.0f, this.transform.position.y - YOffset, 0.0f);
		Vector3 originToBall = this.transform.position - adjustedOrigin;
		m_velocity = Speed*(adjustedOrigin - this.transform.position);
		originToBall.Normalize();
		Vector3 markerPos = originToBall*6f;
		markerPos.y += this.transform.position.y - YOffset;
		Quaternion markerRot = Quaternion.LookRotation(-originToBall);
		//Vector3 up = new Vector3(0.0f, 1.0f, 0.0f);
		//markerRot.SetLookRotation(up);
		Object marker = Instantiate(Marker, markerPos, markerRot);
		Destroy (marker, 1.0f);
	}
	
	// Update is called once per frame
	void Update ()
	{
	}
	
	void FixedUpdate()
	{
		if( !m_exploded )
		{
			this.transform.position += m_velocity;
		}
	}
	
	void OnTriggerEnter(Collider collider)
	{
		Destroy(this.gameObject, DeathTimer);
		ParticleSystem pSystem = this.GetComponent<ParticleSystem>();
		pSystem.Stop();
		GameObject cannonMesh = this.transform.FindChild("SM_Cannonball").gameObject;
		MeshRenderer mRenderer = cannonMesh.GetComponent<MeshRenderer>();
		mRenderer.enabled = false;
		m_exploded = true;
		Object explosion = Instantiate(Explosion, this.transform.position, this.transform.rotation);
		Destroy(explosion, 3.0f);
	}
}
