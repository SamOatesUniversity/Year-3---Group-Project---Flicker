using UnityEngine;
using System.Collections;

public class CCheckPoint : MonoBehaviour {

	private float m_playerAlpha = 0.0f;
	private int m_direction = 1;
	
	public CCheckPoint NextCheckPoint = null;
	public CCheckPoint PreviousCheckPoint = null;
	
	void Start() {
		m_playerAlpha = calculateAlphaAngle();
		m_playerAlpha = m_playerAlpha;
		Debug.Log(gameObject.name + " alpha calculated as " + m_playerAlpha);
	}
	
	void OnTriggerEnter(Collider collision) {
				
		GameObject gObject = collision.collider.gameObject;		
		CEntityPlayer player = gObject.GetComponent<CEntityPlayer>();
		
		if (player != null)
		{
			player.LastCheckPoint = this;
			m_playerAlpha = player.CurrentPlayerAlpha;
			m_direction = player.Physics.Direction;
			if (m_direction == 0)
				m_direction = player.Physics.MovingDirection;
			
			Debug.Log("Hit checkpoint: " + gameObject.name);
		}	
		
	}
	
	private float calculateAlphaAngle()
	{
		Vector3 position = transform.localPosition;
		position.x = Mathf.Round(position.x * 10f) / 10f;
		position.y = Mathf.Round(position.y * 10f) / 10f;
		position.z = Mathf.Round(position.z * 10f) / 10f;
		
		// handle 0 cases
		if (position.x == 0 && position.z >= 0) {
			return 0;	
		} else if (position.x == 0 && position.z < 0) {
			return 180;
		} else if (position.x >= 0 && position.z == 0) {
			return 90;
		} else if (position.x < 0 && position.z == 0) {
			return 270;
		}
		
		// not a straight line, so do some trig to calculate the angle
		float r = 2.8f; // hard code for now
		
		Vector2 or = new Vector2(0, r);
		Vector2 xz = new Vector2(transform.localPosition.x, transform.localPosition.z);
		
		float o = (or - xz).magnitude * 0.5f;
		float h = xz.magnitude;
		
		float alpha = Mathf.Asin(o / h) * 2.0f;
		alpha = alpha * Mathf.Rad2Deg;
		
		return alpha;
	}
	
	/*
	*	\brief Get the player alpha stored at the checkpoint
	*/
	public float PlayerCheckPointAlpha {
		get {
			Debug.Log(gameObject.name + " alpha: " + m_playerAlpha);
			return m_playerAlpha;
		}
		set {
			//m_playerAlpha = value;	
		}
	}
	
	public int Direction {
		get {
			return m_direction;	
		}	
	}
		
}