using UnityEngine;
using System.Collections;

public class CCheckPoint : MonoBehaviour {

	private float m_playerAlpha = 0.0f;

	void OnTriggerEnter(Collider collision) {

		/*
		CEnitityPlayer player = collision.other.gameobject.getComponent<CEntityPlayer>();
		if (player != null)
		{
			player.LastCheckPoint = this;
			m_playerAlpha = player.CurrentPlayerAlpha;
		}
		*/		
		
	}
	
	/*
	*	\brief Get the player alpha stored at the checkpoint
	*/
	float PlayerCheckPointAlpha {
		get {
			return m_playerAlpha;
		}
	}
		
}