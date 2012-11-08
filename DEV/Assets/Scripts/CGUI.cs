using UnityEngine;
using System.Collections;

public class CGUI : MonoBehaviour {
	
	public Texture2D HealthBackgroundFrame = null;
	private Rect m_healthBarBackgroundRect;
	
	public Texture2D HealthForegroundFrame = null;
	private Rect m_healthBarForegroundRect;	
	
	public Vector2 HealthBarPosition;
	public GameObject PlayerEntity = null;
	private CEntityPlayer m_playerEnt;
	
	private float hScrollbarValue;
	
	// Use this for initialization
	void Start () {
		//m_healthBarForegroundRect = new Rect();
		m_playerEnt = PlayerEntity.GetComponent<CEntityPlayer>();
	}
	
	

	// Update is called once per frame
	
	
	void OnGUI()
	{
		//hScrollbarValue = GUI.HorizontalScrollbar (new Rect (25, 25, 100, 30), hScrollbarValue, 1.0f, 0.0f, 10.0f);
	
		float barWidth = 100;
		GUI.DrawTexture( new Rect( 0,0, barWidth, 28), HealthBackgroundFrame);
		
		float healthPercentage = m_playerEnt.GetCurrentHealth() - 4;
		Debug.Log(healthPercentage);
		
		
		GUI.DrawTexture( new Rect( 2 , 2, healthPercentage ,22), HealthForegroundFrame);
	}
	
	
	
	
	void Update () {
		

		
		
	
	}
}
