using UnityEngine;
using System.Collections;

public class CGruntAnimation : MonoBehaviour {
	
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
	
	public void OnFixedUpdate(ref GruntState playerState, CGruntPhysics physics, FootMaterial footMaterial)
	{
		if (playerState == GruntState.Walking)
		{			
			m_currentAnimation = "walk";
			if (!m_animation.IsPlaying("walk"))
			{
				m_animation.CrossFade("walk", 0.2f);
				PlayFootstepAudio(footMaterial);
			}			
		}
		else if (playerState == GruntState.Turning)
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
				playerState = GruntState.Walking;
			}
		}
		else if (playerState == GruntState.Standing)
		{
			if (!m_animation.IsPlaying(m_lastKnownIdle))
			{
				m_currentAnimation = "idle-" + Random.Range(0, 2);
				m_lastKnownIdle = m_currentAnimation;
				m_animation[m_currentAnimation].speed = Random.Range(10, 30) / 100.0f;
				m_animation.CrossFade(m_currentAnimation);
			}						
		}
		else if (playerState == GruntState.Jumping)
		{
			m_currentAnimation = "run-jump";
			if (!m_animation.IsPlaying("run-jump"))
				m_animation.Play("run-jump");
		}
		else if (playerState == GruntState.FallJumping)
		{
			m_currentAnimation = "falling";
			if (!m_animation.IsPlaying("falling"))
				m_animation.CrossFade("falling");
		}
		else if (playerState == GruntState.FallingFromTower)
		{
			m_currentAnimation = "falling";
			if (!m_animation.IsPlaying("falling"))
				m_animation.CrossFade("falling");
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
