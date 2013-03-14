using UnityEngine;
using System.Collections;

public class CEndCredits : MonoBehaviour {
	private bool					m_active = false;
	private CCamera					m_camera = null;
	private Animation				m_animation = null;
	private float					m_backgroundAlpha = 0.0f;
	public float 					FadeInTimer = 1.0f;
	private float					m_alphaIncrement = 0.0f;
	public float					m_scrollSpeed = 1.0f;
	// Use this for initialization
	void Start () {
		m_animation = GetComponent<Animation>();
		if(FadeInTimer > 0)
		{
			m_alphaIncrement = 1.0f/(60.0f*FadeInTimer);
		}
		else
		{
			m_alphaIncrement = 1.0f;	
		}
		GameObject background = this.transform.parent.FindChild("Background").gameObject;
		MeshRenderer backgroundRenderer = background.GetComponent<MeshRenderer>();
		Color backgroundColour = new Color(0.0f, 0.0f, 0.0f, m_backgroundAlpha);
		backgroundRenderer.material.color = backgroundColour;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void SetActive(bool is_active)
	{
		m_active = is_active;	
	}
	
	void FixedUpdate()
	{
		if(m_active)
		{
			GameObject background = this.transform.parent.FindChild("Background").gameObject;
			MeshRenderer backgroundRenderer = background.GetComponent<MeshRenderer>();
			m_backgroundAlpha += m_alphaIncrement;
			Color backgroundColour = new Color(0.0f, 0.0f, 0.0f, m_backgroundAlpha);
			backgroundRenderer.material.color = backgroundColour;
			
			float scrollIncrement = m_scrollSpeed*0.01f;
			Vector3 pos = this.transform.position;
			Vector3 increment = new Vector3(0.0f, scrollIncrement, 0.0f);
			pos += increment;
			this.transform.position = pos;
			
			if (pos.y > 56.3f)
			{
				Application.LoadLevel("Main_Menu");	
			}
		}
	}
	
	void OnTriggerEnter()
	{
		if(!m_active)
		{
			CEntityPlayer player = CEntityPlayer.GetInstance();
			if (player == null)
				return;		
			player.SetPlayerState(PlayerState.InCutScene);
			m_active = true;
			//m_animation.Play();
			this.transform.rotation.SetLookRotation(this.transform.position-player.transform.position);
		}
	}
}
