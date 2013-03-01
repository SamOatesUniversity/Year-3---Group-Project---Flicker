using UnityEngine;
using System.Collections;

public class CGruntAnimation : MonoBehaviour {
	
	public AudioClip[]		FootstepsStone;
	public AudioClip[]		FootstepsWood;
	public AudioClip[]		FootstepsMetal;
	
	//----------------------//
	
	Animation m_animation = null;
	bool m_startedTurningRound = false;
	bool m_startedAttacking = false;
	private string m_currentAnimation = null;
	
	private string m_lastKnownIdle = "idle-0";
	
	private AudioSource m_audio;
	
	public void OnStart(Animation animation)
	{
		m_animation = animation;
		m_audio = GetComponent<AudioSource>();
	}
	
	public void OnFixedUpdate(ref GruntState playerState, ref int movingDirection, CGruntPhysics physics, FootMaterial footMaterial, bool isDetected)
	{
		if (playerState == GruntState.Walking)
		{	
			if(isDetected)
			{
				m_currentAnimation = "run";
				if (!m_animation.IsPlaying("run"))
				{
					m_animation.CrossFade("run", 0.2f);
					PlayFootstepAudio(footMaterial);
				}
			}
			else
			{
				m_currentAnimation = "walk";
				if (!m_animation.IsPlaying("walk"))
				{
					m_animation.CrossFade("walk", 0.2f);
					PlayFootstepAudio(footMaterial);
				}				
			}
		}
		else if (playerState == GruntState.Turning)
		{
			m_currentAnimation = "walk180turn";
			if (!m_animation.IsPlaying("walk180turn") && !m_startedTurningRound)
			{
				Debug.Log("On turn start");
				m_startedTurningRound = true;
				m_animation["walk180turn"].speed = 1.0f;
				m_animation.CrossFade("walk180turn");
			}
			else if (!m_animation.IsPlaying("walk180turn"))
			{
				m_startedTurningRound = false;
				playerState = GruntState.Walking;
				Debug.Log("On turn complete");
				/*
				if( movingDirection == 1)
				{
					movingDirection = -1;	
				}
				else if( movingDirection == -1 )
				{
					movingDirection = 1;	
				}
				*/
			}
		}
		else if (playerState == GruntState.Attacking)
		{
			m_currentAnimation = "alert_attack1";
			if (!m_animation.IsPlaying("alert_attack1") && !m_startedAttacking)
			{
				Debug.Log("On attack start");
				m_startedAttacking = true;
				m_animation["alert_attack1"].speed = 1.0f;
				m_animation.CrossFade("alert_attack1");
			}
			else if (!m_animation.IsPlaying("alert_attack1"))
			{
				m_startedTurningRound = false;
				playerState = GruntState.Walking;
				Debug.Log("On attack complete");
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
