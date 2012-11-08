using UnityEngine;
using System.Collections;

public class CPlayerAnimation : MonoBehaviour {
	
	Animation m_animation = null;
	
	public void OnStart(Animation animation)
	{
		m_animation = animation;
	}
	
	public void OnFixedUpdate(PlayerState playerState)
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
			if (!m_animation.IsPlaying("ledgehang"))
				m_animation.CrossFade("ledgehang");
		}
	}
	
}
