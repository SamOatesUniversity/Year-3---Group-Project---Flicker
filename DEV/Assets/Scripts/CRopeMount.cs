using UnityEngine;
using System.Collections;

public class CRopeMount : MonoBehaviour {
	
	public GameObject RopeClass;
	public float RopeLength = 1.0f;
	public float RopeGirth = 0.2f;
	public float RopeSpeed = 1.0f;
	public float MaxAngle = 90.0f;
	
	private CRope m_rope = null;
	public float m_currentAngle = 0;
	private bool m_playerAttached = false;
	public float direction = 1;
	// Use this for initialization
	void Start () {
		
		GameObject obj = (GameObject)Instantiate(RopeClass, transform.position + new Vector3(0,-RopeLength,0), Quaternion.identity);
		
		m_rope = obj.GetComponent<CRope>();
		m_rope.transform.localScale = new Vector3(RopeGirth,RopeLength,RopeGirth);
		m_rope.transform.parent = this.transform;
		m_rope.rigidbody.useGravity = false;
		m_rope.rigidbody.isKinematic = true;
		m_rope.parent = this.GetComponent<CRopeMount>();
	
	}
	
	public void SetPlayerAttached(bool val)
	{
		m_playerAttached = val;
	}
	
	// Update is called once per frame
	void Update () {
		
		
		if (!m_playerAttached)
		{
			ReturnToStationary();
		}
		else if (m_playerAttached)
		{
			SimulateSwing();
		}
		
	}
	
	void SimulateSwing()
	{
		if ( m_currentAngle > MaxAngle || m_currentAngle < -MaxAngle)
		{
			direction = -direction;
		}


		transform.Rotate(Vector3.right * (RopeSpeed * direction));
		m_currentAngle += RopeSpeed * direction;
	}
	
	void ReturnToStationary()
	{
		
		
		if (m_currentAngle > 1)
		{
			transform.Rotate(Vector3.right * -RopeSpeed);
			m_currentAngle -= RopeSpeed;
		}
		if (m_currentAngle < 1)
		{
			transform.Rotate(Vector3.right * RopeSpeed);
			m_currentAngle += RopeSpeed;
		}
		
		
	}
}
