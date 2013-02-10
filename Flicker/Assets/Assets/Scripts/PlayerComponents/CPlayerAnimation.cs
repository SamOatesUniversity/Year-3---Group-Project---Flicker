using UnityEngine;
using System.Collections;

public class CPlayerAnimation : MonoBehaviour {
	
	public AudioClip[]		FootstepsStone;
	public AudioClip[]		FootstepsWood;
	public AudioClip[]		FootstepsMetal;
	
	//----------------------//
	
	Animation m_animation = null;
	bool m_startedLedgeClimb = false;
	bool m_startedTurningRound = false;
	private string m_currentAnimation = null;
	
	private AudioSource m_audio;
	
	public void OnStart(Animation animation)
	{
		m_animation = animation;
		m_audio = GetComponent<AudioSource>();
	}
	
	public void OnFixedUpdate(ref PlayerState playerState, LadderState ladderState)
	{
		if (playerState == PlayerState.Walking)
		{			
			m_currentAnimation = "Run";
			if (!m_animation.IsPlaying("Run"))
			{
				m_animation.CrossFade("Run", 0.2f);
				
				int noofAudioClips = FootstepsStone.Length;
				if (noofAudioClips != 0)
				{
					int audioIndex = Random.Range(0, noofAudioClips);
					m_audio.clip = FootstepsStone[audioIndex];
					m_audio.Play();
				}
			}			
		}
		else if (playerState == PlayerState.Turning)
		{
			m_currentAnimation = "Turning Around";
			if (!m_animation.IsPlaying("Running Turn") && !m_startedTurningRound)
			{
				m_startedTurningRound = true;
				m_animation.CrossFade("Running Turn");
			}
			else if (!m_animation.IsPlaying("Running Turn"))
			{
				m_startedTurningRound = false;
				playerState = PlayerState.Walking;
			}
		}
		else if (playerState == PlayerState.Standing)
		{
			m_currentAnimation = "Idle";
			m_animation["Idle Simple"].speed = 0.2f;
			if (!m_animation.IsPlaying("Idle Simple"))
				m_animation.CrossFade("Idle Simple");
		}
		else if (playerState == PlayerState.Jumping)
		{
			m_currentAnimation = "Running";
			if (!m_animation.IsPlaying("Running Jump (NEW)"))
				m_animation.Play("Running Jump (NEW)");
		}
		else if (playerState == PlayerState.FallJumping)
		{
			m_currentAnimation = "Falling";
			if (!m_animation.IsPlaying("Falling Loop"))
				m_animation.CrossFade("Falling Loop");
		}
		else if (playerState == PlayerState.LedgeHang)
		{
			m_currentAnimation = "Ledge Hang";
			if (!m_animation.IsPlaying("Wall Hang Idle")) {
				m_animation.CrossFade("Wall Hang Idle");
				m_startedLedgeClimb = false;
			}
		}
		else if (playerState == PlayerState.LedgeClimb)
		{
			m_currentAnimation = "Ledge Climbing";
			if (!m_animation.IsPlaying("Climb from Free Hang") && !m_startedLedgeClimb) {
				m_animation.CrossFade("Climb from Free Hang");
				m_startedLedgeClimb = true;
			}	
			else if (m_startedLedgeClimb == true && !m_animation.IsPlaying("Climb from Free Hang")) {
				playerState = PlayerState.LedgeClimbComplete;
				m_startedLedgeClimb = false;
			}
		}
		else if (playerState == PlayerState.FallingFromTower)
		{
			m_currentAnimation = "Falling";
			if (!m_animation.IsPlaying("Falling Loop"))
				m_animation.CrossFade("Falling Loop");
		}
		else if (playerState == PlayerState.UpALadder)
		{
			float upDown = Input.GetAxis("Vertical");
			
			m_currentAnimation = "Ladder";
			
			if ((upDown != 0.0f && ladderState != LadderState.AtTop) || (ladderState == LadderState.AtTop && upDown < 0))
			{
				bool forceAnimationChange = upDown > 0.0f && m_animation["Ladder Up"].speed > 0.0f ? false : true;
				if (!m_animation.IsPlaying("Ladder Up") || forceAnimationChange)
				{
					m_animation["Ladder Up"].speed = upDown > 0.0f ? 2.0f : -2.0f;
					m_animation.Play("Ladder Up");
				}
				
				if (m_animation.IsPlaying("Ladder Up"))
				{
					int noofAudioClips = FootstepsMetal.Length;
					if (noofAudioClips != 0)
					{
						int audioIndex = Random.Range(0, noofAudioClips);
						if (!m_audio.isPlaying)
						{
							m_audio.clip = FootstepsMetal[audioIndex];
							m_audio.Play();
						}
					}	
				}
				
			}
			else
			{
				m_animation["Ladder Up"].speed = 0.0f;
				if (!m_animation.IsPlaying("Ladder Up"))
					m_animation.Play("Ladder Up");
			}
		}
	}
	
	public string CurrentAnimation()
	{
		return m_currentAnimation;	
	}
	
}
