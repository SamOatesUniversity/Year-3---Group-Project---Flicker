using UnityEngine;
using System.Collections;

public class CPlayerAnimation : MonoBehaviour {
	
	Animation m_animation = null;
	
	bool m_startedLedgeClimb = false;
	
	public void OnStart(Animation animation)
	{
		m_animation = animation;
	}
	
	public void OnFixedUpdate(ref PlayerState playerState)
	{
		if (playerState == PlayerState.Walking)
		{
			if (!m_animation.IsPlaying("run"))
				m_animation.CrossFade("run");
		}
		else if (playerState == PlayerState.Standing)
		{
			if (!m_animation.IsPlaying("idle"))
				m_animation.CrossFade("idle");
		}
		else if (playerState == PlayerState.Jumping)
		{
			if (!m_animation.IsPlaying("jump"))
				m_animation.CrossFade("jump");
		}
		else if (playerState == PlayerState.LedgeHang)
		{
			if (!m_animation.IsPlaying("ledgehang")) {
				m_animation.CrossFade("ledgehang");
				m_startedLedgeClimb = false;
			}
		}
		else if (playerState == PlayerState.LedgeClimb)
		{
			if (!m_animation.IsPlaying("ledgeclimb") && !m_startedLedgeClimb) {
				m_animation.CrossFade("ledgeclimb");
				m_startedLedgeClimb = true;
			}	
			else if (m_startedLedgeClimb == true && !m_animation.IsPlaying("ledgeclimb")) {
				playerState = PlayerState.LedgeClimbComplete;
				m_startedLedgeClimb= false;
			}
		}
	}
	
}
