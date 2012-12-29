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
			if (!m_animation.IsPlaying("Run"))
				m_animation.CrossFade("Run");
		}
		else if (playerState == PlayerState.Standing)
		{
			m_animation["Idle Simple"].speed = 0.2f;
			if (!m_animation.IsPlaying("Idle Simple"))
				m_animation.CrossFade("Idle Simple");
		}
		else if (playerState == PlayerState.Jumping)
		{
			if (!m_animation.IsPlaying("Running Jump (NEW)"))
				m_animation.Play("Running Jump (NEW)");
		}
		else if (playerState == PlayerState.FallJumping)
		{
			if (!m_animation.IsPlaying("Falling Loop"))
				m_animation.CrossFade("Falling Loop");
		}
		else if (playerState == PlayerState.LedgeHang)
		{
			if (!m_animation.IsPlaying("Wall Hang Idle")) {
				m_animation.CrossFade("Wall Hang Idle");
				m_startedLedgeClimb = false;
			}
		}
		else if (playerState == PlayerState.LedgeClimb)
		{
			if (!m_animation.IsPlaying("Climb from Free Hang") && !m_startedLedgeClimb) {
				m_animation.CrossFade("Climb from Free Hang");
				m_startedLedgeClimb = true;
			}	
			else if (m_startedLedgeClimb == true && !m_animation.IsPlaying("Climb from Free Hang")) {
				playerState = PlayerState.LedgeClimbComplete;
				m_startedLedgeClimb= false;
			}
		}
		else if (playerState == PlayerState.FallingFromTower)
		{
			if (!m_animation.IsPlaying("Falling Loop"))
				m_animation.CrossFade("Falling Loop");
		}
		else if (playerState == PlayerState.UpALadder)
		{
			float upDown = Input.GetAxis("Vertical");
			if (upDown != 0.0f)
			{
				bool forceAnimationChange = upDown > 0.0f && m_animation["Ladder Up"].speed > 0.0f ? false : true;
				if (!m_animation.IsPlaying("Ladder Up") || forceAnimationChange)
				{
					m_animation["Ladder Up"].speed = upDown > 0.0f ? 2.0f : -2.0f;
					m_animation.Play("Ladder Up");
				}
			}
			else
			{
				m_animation["Ladder Up"].speed = 0.0f;
			}
		}
	}
	
}
