using UnityEngine;
using System.Collections;

public enum MonkeyState {
	IdlePre,			//!< 
	Animate,				//!<
	IdlePost
};

public enum MonkeyLevel {
	Unspecified,
	OneTwo,
	OneEight
};

public class CEntityMonkey : MonoBehaviour {

	public AudioClip		Casual;
	public AudioClip		Loud;
	public float			MeanTimeBetweenIdleAudio = 14.0f;
	public float			IdleAudioTimeVariance = 4.0f;
	
	private AudioSource 	m_audio;
	
	private MonkeyState		m_state;
	public MonkeyLevel		m_level = MonkeyLevel.Unspecified;
	
	private Animation 		m_animation = null;
	private string 			m_currentAnimation = null;
	private string			m_lastKnownIdle;
	private bool			m_startedMainAnim = false;
	private float 			m_idleAudioTimer = 5.0f;

	
	private static CEntityMonkey INSTANCE = null;
	
	// Use this for initialization
	void Start () {
		
		INSTANCE = this;
		m_state = MonkeyState.IdlePre;
		m_animation = GetComponentInChildren<Animation>();
		
		m_lastKnownIdle = "";
		m_audio = GetComponent<AudioSource>();
	}
	
	public static CEntityMonkey GetInstance()
	{
		return INSTANCE;	
	}
	
	public void DoAnimation()
	{
		m_state = MonkeyState.Animate;
		PlayAudio(Loud);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void FixedUpdate()
	{	
		DoAnimations();
		m_idleAudioTimer -= Time.deltaTime;
		if(m_idleAudioTimer <= 0.0f)
		{
			PlayAudio(Casual);
			float split = IdleAudioTimeVariance/2;
			float variance = Random.Range(-split, split);
			m_idleAudioTimer = MeanTimeBetweenIdleAudio+variance;
		}
	}
	
	void PlayAudio(AudioClip clip)
	{
		m_audio.clip = clip;
		m_audio.Play();
		if(clip == Loud)
		{
			m_idleAudioTimer = (float)MeanTimeBetweenIdleAudio;	
		}
	}

	void DoAnimations()
	{
		if( m_level == MonkeyLevel.Unspecified )
		{
			if( m_state == MonkeyState.IdlePre )
			{
				m_currentAnimation = "MONKEY_idle";
				if (!m_animation.IsPlaying(m_currentAnimation))
				{
					m_animation.CrossFade(m_currentAnimation, 0.2f);
				}	
			}
		}
		else if( m_level == MonkeyLevel.OneTwo )
		{
			if( m_state == MonkeyState.IdlePre )
			{
				m_currentAnimation = "MONKEY_1-2_idle";
				if (!m_animation.IsPlaying(m_currentAnimation))
				{
					m_animation.CrossFade(m_currentAnimation, 0.2f);
				}	
			}
			else if( m_state == MonkeyState.Animate )
			{
				m_currentAnimation = "MONKEY_1-2";
				if (!m_animation.IsPlaying("MONKEY_1-2") && !m_startedMainAnim)
				{
					//Debug.Log("On attack start");
					m_startedMainAnim = true;
					m_animation["MONKEY_1-2"].speed = 1.0f;
					m_animation.CrossFade("MONKEY_1-2");
				}
				else if (!m_animation.IsPlaying("MONKEY_1-2"))
				{
					m_startedMainAnim = false;
					m_state = MonkeyState.IdlePost;
					this.gameObject.SetActiveRecursively(false);
					//Debug.Log("On attack complete");
				}
			}
			if( m_state == MonkeyState.IdlePost )
			{
				m_currentAnimation = "MONKEY_idle";
				if (!m_animation.IsPlaying(m_currentAnimation))
				{
					m_animation.CrossFade(m_currentAnimation, 0.2f);
				}	
			}
			
		}
		else if( m_level == MonkeyLevel.OneEight )
		{
			if( m_state == MonkeyState.IdlePre )
			{
				m_currentAnimation = "MONKEY_idle";
				if (!m_animation.IsPlaying(m_currentAnimation))
				{
					m_animation.CrossFade(m_currentAnimation, 0.2f);
				}	
			}
			else if( m_state == MonkeyState.Animate )
			{
				m_currentAnimation = "MONKEY_idle-to-attack";
				if (!m_animation.IsPlaying("MONKEY_idle-to-attack") && !m_startedMainAnim)
				{
					//Debug.Log("On attack start");
					m_startedMainAnim = true;
					m_animation["MONKEY_idle-to-attack"].speed = 1.0f;
					m_animation.CrossFade("MONKEY_idle-to-attack");
				}
				else if (!m_animation.IsPlaying("MONKEY_idle-to-attack"))
				{
					m_startedMainAnim = false;
					m_state = MonkeyState.IdlePost;
					this.gameObject.SetActiveRecursively(false);
					//Debug.Log("On attack complete");
				}
			}
			if( m_state == MonkeyState.IdlePost )
			{
				m_currentAnimation = "MONKEY_idle";
				if (!m_animation.IsPlaying(m_currentAnimation))
				{
					m_animation.CrossFade(m_currentAnimation, 0.2f);
				}	
			}
		}
		
	}
	
}

