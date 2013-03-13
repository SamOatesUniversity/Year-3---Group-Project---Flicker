using UnityEngine;
using System.Collections;

public enum CaptainState {
	Idle,				//!< 
	Angry,				//!< 
	Cutscene
};

public class CEntityCaptain : MonoBehaviour {


	
	private CaptainState	m_state;
	
	private Animation 		m_animation = null;
	private string 			m_currentAnimation = null;
	private ArrayList		m_cutsceneAnims;
	private ArrayList 		m_angryAnims;
	private string			m_lastKnownIdle;
	
	private int 			m_cutsceneAnimCounter = 0;
	
	private static CEntityCaptain INSTANCE = null;
	
	// Use this for initialization
	void Start () {
		
		INSTANCE = this;
		m_state = CaptainState.Idle;
		m_animation = GetComponentInChildren<Animation>();
		
		//setup cutscene anim list
		m_cutsceneAnims = new ArrayList();
		m_cutsceneAnims.Add("taunt");
		m_cutsceneAnims.Add("launch");
		m_cutsceneAnims.Add("angryidle_0");
		m_cutsceneAnims.Add("fire");
		
		//setup angry anim list
		m_angryAnims = new ArrayList();
		m_angryAnims.Add("taunt");
		m_angryAnims.Add("angryidle_0");
		m_angryAnims.Add("stagger");
		m_angryAnims.Add("facepalm");
		m_angryAnims.Add("launch");
		m_angryAnims.Add("fire");
		
		m_lastKnownIdle = "";
	}
	
	public static CEntityCaptain GetInstance()
	{
		return INSTANCE;	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void FixedUpdate()
	{	
		DoAnimations();
	}
	
	public void StartCutScene()
	{
		m_state = CaptainState.Cutscene;
		m_lastKnownIdle = m_cutsceneAnims[0] as string;
	}
	
	public void EndCutScene()
	{
		m_state = CaptainState.Angry;
		m_lastKnownIdle = m_angryAnims[0] as string;
	}
	
	void DoAnimations()
	{
		if( m_state == CaptainState.Idle )
		{
			m_currentAnimation = "calmidle";
			if (!m_animation.IsPlaying(m_currentAnimation))
			{
				m_animation.CrossFade(m_currentAnimation, 0.2f);
			}	
		}
		else if ( m_state == CaptainState.Angry )
		{	
			//random animations from the angry list
			if (!m_animation.IsPlaying(m_lastKnownIdle))
			{
				if( m_currentAnimation == "calmidle" )
				{
					int randomIndex = Random.Range(0, m_angryAnims.Count-1);
					
					m_currentAnimation = m_angryAnims[randomIndex] as string;
				}
				else
				{
					m_currentAnimation = "calmidle";
				}
				m_lastKnownIdle = m_currentAnimation;
				m_animation.CrossFade(m_currentAnimation, 0.3f);
			}				
		}
		else if ( m_state == CaptainState.Cutscene )
		{
			//runs through all cutscene anims in order
			if (!m_animation.IsPlaying(m_lastKnownIdle))
			{
				if( m_currentAnimation == "calmidle" )
				{
					m_cutsceneAnimCounter++;
					
					if( m_cutsceneAnimCounter > m_cutsceneAnims.Count-1 )
					{
						EndCutScene();
					}
					else
					{
						m_currentAnimation = m_cutsceneAnims[m_cutsceneAnimCounter] as string;
					}
				}
				else
				{
					m_currentAnimation = "calmidle";
				}
				m_lastKnownIdle = m_currentAnimation;
				m_animation.CrossFade(m_currentAnimation, 0.3f);
			}		
		}
		if(m_currentAnimation == "calmidle")
		{
			m_animation[m_currentAnimation].speed = 1.6f;
		}
	}
}
