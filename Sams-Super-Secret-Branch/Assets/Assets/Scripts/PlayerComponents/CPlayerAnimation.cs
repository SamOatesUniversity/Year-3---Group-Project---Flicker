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
	
	private string m_lastKnownIdle = "idle-0";
	
	private AudioSource m_audio;
	
	public void OnStart(Animation animation)
	{
		m_animation = animation;
		m_audio = GetComponent<AudioSource>();
	}
	
	public void OnFixedUpdate(ref PlayerState playerState, CEntityPlayer player)
	{
		if (playerState == PlayerState.Walking)
		{			
			m_currentAnimation = "run";
			if (!m_animation.IsPlaying("run"))
			{
				m_animation.CrossFade("run", 0.2f);
				PlayFootstepAudio(player.Physics.GetFootMaterial());
			}			
		}
		else if (playerState == PlayerState.Turning)
		{
			m_currentAnimation = "running-turn";
			if (!m_animation.IsPlaying("running-turn") && !m_startedTurningRound)
			{
				m_startedTurningRound = true;
				m_animation["running-turn"].speed = 1.2f;
				m_animation.CrossFade("running-turn");
			}
			else if (!m_animation.IsPlaying("running-turn"))
			{
				m_startedTurningRound = false;
				playerState = PlayerState.Walking;
			}
		}
		else if (playerState == PlayerState.Standing)
		{
			if (!m_animation.IsPlaying(m_lastKnownIdle))
			{
				m_currentAnimation = "idle-0"; // "idle-" + Random.Range(0, 4);
				m_lastKnownIdle = m_currentAnimation;
				m_animation[m_currentAnimation].speed = Random.Range(10, 30) / 100.0f;
				m_animation.CrossFade(m_currentAnimation);
			}						
		}
		else if (playerState == PlayerState.Jumping)
		{
			m_currentAnimation = "run-jump";
			if (!m_animation.IsPlaying("run-jump"))
				m_animation.Play("run-jump");
		}
		else if (playerState == PlayerState.FallJumping)
		{
			m_currentAnimation = "falling";
			if (!m_animation.IsPlaying("falling"))
				m_animation.CrossFade("falling");
		}
		else if (playerState == PlayerState.WallJumpStart)
		{
			m_currentAnimation = "wall-hang-idle";
			if (!m_animation.IsPlaying(m_currentAnimation))
				m_animation.CrossFade(m_currentAnimation);
		}
		else if (playerState == PlayerState.LedgeHang)
		{
			m_currentAnimation = "free-hang-idle";
			if (player.Physics.GetLedgeGrabType() == eLedgeType.Wall)
				m_currentAnimation = "wall-hang-idle";
			
			if (!m_animation.IsPlaying(m_currentAnimation)) {
				m_animation.CrossFade(m_currentAnimation);
				m_startedLedgeClimb = false;
			}
		}
		else if (playerState == PlayerState.LedgeClimb)
		{
			m_currentAnimation = "free-hang-climb";
			if (!m_animation.IsPlaying("free-hang-climb") && !m_startedLedgeClimb) {
				m_animation.CrossFade("free-hang-climb");
				m_startedLedgeClimb = true;
			}	
			else if (m_startedLedgeClimb == true && !m_animation.IsPlaying("free-hang-climb")) {
				playerState = PlayerState.LedgeClimbComplete;
				m_startedLedgeClimb = false;
			}
		}
		else if (playerState == PlayerState.FallingFromTower)
		{
			m_currentAnimation = "falling";
			if (!m_animation.IsPlaying("falling"))
				m_animation.CrossFade("falling");
		}
		else if (playerState == PlayerState.OnLadder)
		{
			m_currentAnimation = "ladder-climb";

			m_animation[m_currentAnimation].speed = player.Physics.GetLadder.offset * 100.0f;			

			if (!m_animation.IsPlaying(m_currentAnimation))
			{
				m_animation.Play(m_currentAnimation);	
				PlayFootstepAudio(FootMaterial.Metal);
			}
		}
	}
	
	public string CurrentAnimation()
	{
		return m_currentAnimation;	
	}
	
	public void PlayFootstepAudio(FootMaterial material)
	{
		AudioClip[] clips = FootstepsStone;		
		switch(material)
		{
		case FootMaterial.Wood:
			clips = FootstepsWood;
			break;
		case FootMaterial.Metal:
			clips = FootstepsMetal;
			break;
		}		
		
		int noofAudioClips = clips.Length;
		if (noofAudioClips != 0)
		{
			int audioIndex = Random.Range(0, noofAudioClips);
			if (!m_audio.isPlaying)
			{
				m_audio.clip = clips[audioIndex];
				m_audio.Play();
			}
		}
	}	
}
