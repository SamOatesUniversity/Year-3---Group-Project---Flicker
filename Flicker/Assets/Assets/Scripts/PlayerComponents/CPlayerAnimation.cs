using UnityEngine;
using System.Collections;

public class CPlayerAnimation : MonoBehaviour {
	
	public AudioClip[]		FootstepsStone;
	public AudioClip[]		FootstepsWood;
	public AudioClip[]		FootstepsMetal;
	public AudioClip[]		Hurt;
	public AudioClip[]		Jump;
	public AudioClip[]		Climb;
	public AudioClip[]		Push;
	public AudioClip[]		FallFromTower;
	
	//----------------------//
	
	Animation m_animation = null;
	bool m_startedLedgeClimb = false;
	bool m_startedTurningRound = false;
	private string m_currentAnimation = null;
	
	private string m_lastKnownIdle = "idle-0";
	
	private bool m_hasWallJumped = false;
	
	private bool m_pulledLever = false;
	
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
			if (!m_animation.IsPlaying(m_currentAnimation) && !m_startedTurningRound)
			{
				m_startedTurningRound = true;
				m_animation[m_currentAnimation].speed = 1.2f;
				m_animation.CrossFade(m_currentAnimation);
			}
			else if (!m_animation.IsPlaying(m_currentAnimation))
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
				m_hasWallJumped = false;
			}						
		}
		else if (playerState == PlayerState.Jumping)
		{
			m_currentAnimation = "run-jump";
			if (!m_animation.IsPlaying("run-jump")) {
				m_animation.Play("run-jump");
				PlayRandomAudio(Jump, true);	
				m_hasWallJumped = false;
			}
		}
		else if (playerState == PlayerState.FallJumping)
		{
			m_currentAnimation = "falling";
			if (!m_animation.IsPlaying("falling")) {
				m_animation.CrossFade("falling");
			}
		}
		else if (playerState == PlayerState.WallJumpStart)
		{
			m_currentAnimation = "wall-jump";
			if (!m_animation.IsPlaying(m_currentAnimation) && !m_hasWallJumped){
				m_hasWallJumped = true;
				m_animation.CrossFade(m_currentAnimation, 0.01f);
			}
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
				PlayRandomAudio(Climb, true);
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
			if (!m_animation.IsPlaying("falling")) {
				PlayRandomAudio(FallFromTower, false);
				m_animation.CrossFade("falling");
			}				
		}
		else if (playerState == PlayerState.OnLadder)
		{
			m_currentAnimation = "ladder-climb";

			m_animation[m_currentAnimation].speed = player.Physics.GetLadder.offset * 150.0f;			

			if (!m_animation.IsPlaying(m_currentAnimation))
			{
				m_animation.Play(m_currentAnimation);	
				PlayFootstepAudio(FootMaterial.Metal);
			}
		}
		else if (playerState == PlayerState.PullingWallLeverDown)
		{
			if (m_pulledLever && !m_animation.IsPlaying(m_currentAnimation))
			{
				playerState = PlayerState.Standing;
				m_pulledLever = false;
			}
			m_currentAnimation = "switch-down";	
			if (!m_animation.IsPlaying(m_currentAnimation))
			{
				m_animation.CrossFade(m_currentAnimation);	
				m_pulledLever = true;
			}
		}
		else if (playerState == PlayerState.PullingWallLeverUp)
		{
			if (m_pulledLever && !m_animation.IsPlaying(m_currentAnimation))
			{
				playerState = PlayerState.Standing;
				m_pulledLever = false;
			}
			m_currentAnimation = "switch-up";	
			if (!m_animation.IsPlaying(m_currentAnimation))
			{
				m_animation.CrossFade(m_currentAnimation);	
				m_pulledLever = true;
			}
		}
		else if (playerState == PlayerState.NormalFloorLever)
		{
			if (m_pulledLever && !m_animation.IsPlaying(m_currentAnimation))
			{
				playerState = PlayerState.Standing;
				m_pulledLever = false;
			}
			m_currentAnimation = "bigswitch-on";	
			if (!m_animation.IsPlaying(m_currentAnimation))
			{
				m_animation.CrossFade(m_currentAnimation);	
				m_pulledLever = true;
			}	
		}
		else if (playerState == PlayerState.KickFloorLever)
		{
			if (m_pulledLever && !m_animation.IsPlaying(m_currentAnimation))
			{
				playerState = PlayerState.Standing;
				m_pulledLever = false;
			}
			m_currentAnimation = "bigswitch-kick";	
			if (!m_animation.IsPlaying(m_currentAnimation))
			{
				m_animation.CrossFade(m_currentAnimation);	
				m_pulledLever = true;
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
		
		PlayRandomAudio(clips, false);
	}	
	
	public void PlayRandomAudio(AudioClip[] audioClips, bool forcePlay)
	{
		int noofAudioClips = audioClips.Length;
		if (noofAudioClips != 0)
		{
			int audioIndex = Random.Range(0, noofAudioClips);
			if (!m_audio.isPlaying || forcePlay)
			{
				m_audio.clip = audioClips[audioIndex];
				m_audio.Play();
			}
		}
	}
}
