using UnityEngine;
using System.Collections;

public class CCheckPoint : MonoBehaviour {

	private float m_playerAlpha = 0.0f;
	private int m_direction = 1;
	
	void OnTriggerEnter(Collider collision) {
				
		GameObject gObject = collision.collider.gameObject;		
		CEntityPlayer player = gObject.GetComponent<CEntityPlayer>();
		
		if (player != null)
		{
			player.LastCheckPoint = this;
			m_playerAlpha = player.CurrentPlayerAlpha;
			m_direction = player.Physics.Direction;
		}	
		
	}
	
	/*
	*	\brief Get the player alpha stored at the checkpoint
	*/
	public float PlayerCheckPointAlpha {
		get {
			return m_playerAlpha;
		}
		set {
			m_playerAlpha = value;	
		}
	}
	
	public int Direction {
		get {
			return m_direction;	
		}	
	}
		
}