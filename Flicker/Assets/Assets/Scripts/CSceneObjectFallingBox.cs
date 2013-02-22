using UnityEngine;
using System.Collections;

public class CSceneObjectFallingBox : MonoBehaviour {
	
	public CTriggerPressurePlate	PressurePlate = null;
	
	private bool m_ran = false;
	
	public void OnAnimationComplete() {
		if (PressurePlate != null)
			PressurePlate.state = true;
	}
	
	void OnCollisionEnter(Collision collision) {
		
		if (m_ran)
			return;
		
		m_ran = true;
		
		if (collision.gameObject.name == "Player Spawn")
		{
			Animation anim = GetComponent<Animation>();
			if (anim != null)
			{
				// no way to just play the first animation in the anim,
				// so do a fake loop to get the first, then leave
				foreach (AnimationState state in anim)
				{
					anim.Play(state.name);
					break;
				}
			}
		}
	}
	
}
