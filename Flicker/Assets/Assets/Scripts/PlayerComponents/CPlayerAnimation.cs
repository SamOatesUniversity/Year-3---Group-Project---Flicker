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
			if (!m_animation.IsPlaying("idle simple"))
				m_animation.CrossFade("idle simple");
		}
		else if (playerState == PlayerState.Jumping)
		{
			if (!m_animation.IsPlaying("running jump"))
				m_animation.CrossFade("running jump");
		}
		else if (playerState == PlayerState.FallJumping)
		{
			if (!m_animation.IsPlaying("falling loop"))
				m_animation.CrossFade("falling loop");
		}
		else if (playerState == PlayerState.LedgeHang)
		{
			if (!m_animation.IsPlaying("wall hang idle")) {
				m_animation.CrossFade("wall hang idle");
				m_startedLedgeClimb = false;
			}
		}
		else if (playerState == PlayerState.LedgeClimb)
		{
			if (!m_animation.IsPlaying("climb from free hang") && !m_startedLedgeClimb) {
				m_animation.CrossFade("climb from free hang");
				m_startedLedgeClimb = true;
			}	
			else if (m_startedLedgeClimb == true && !m_animation.IsPlaying("climb from free hang")) {
				playerState = PlayerState.LedgeClimbComplete;
				m_startedLedgeClimb= false;
			}
		}
		else if (playerState == PlayerState.FallingFromTower)
		{
			if (!m_animation.IsPlaying("falling loop"))
				m_animation.CrossFade("falling loop");
		}
	}
	
}
