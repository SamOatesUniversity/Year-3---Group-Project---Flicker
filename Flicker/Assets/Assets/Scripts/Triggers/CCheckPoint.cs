using UnityEngine;
using System.Collections;

public class CCheckPoint : MonoBehaviour {

	private float m_playerAlpha = 0.0f;
	
	void OnTriggerEnter(Collider collision) {
		
		Debug.Log ("Entered a checkpoint");
		
		GameObject gObject = collision.collider.gameObject;		
		CEntityPlayer player = gObject.GetComponent<CEntityPlayer>();
		
		if (player != null)
		{
			player.LastCheckPoint = this;
			m_playerAlpha = player.CurrentPlayerAlpha;
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
		
}