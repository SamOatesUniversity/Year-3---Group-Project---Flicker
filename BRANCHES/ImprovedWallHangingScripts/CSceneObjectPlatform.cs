using UnityEngine;
using System.Collections;

public class CSceneObjectPlatform : CSceneObject {
		
	/* -----------------
	    Private Members 
	   ----------------- */
	
	private float m_deltaA;			//!< Rotation delta
	
	private float m_deltaY;			//!< Height delta
	
	private Vector3 m_lastPos;		//!< Object position last frame
	
	private float m_lastRotY;
	
	// Use this for initialization
	void Start() 
    {
		m_lastPos = gameObject.transform.position;
		m_lastRotY = gameObject.transform.rotation.eulerAngles.y;
	}
	
	// Update is called once per frame
	void Update() 
    {
		
	}
	
	void FixedUpdate()
	{
		Animation platAnim = gameObject.GetComponent<Animation>();
		if (platAnim == null)
		{
			return;
			//Debug.Log ("Platform Pos = " + platAnim.transform.position); 
		}
		Vector3 currentPos = platAnim.transform.position;
		
		float rotY = platAnim.transform.rotation.eulerAngles.y;
		m_deltaA += rotY - m_lastRotY;
		m_lastRotY = rotY;
		
		//print ("currentPos is: " + currentPos);
		//Vector3 vec1 = m_lastPos - new Vector3( 0.0f, m_lastPos.y, 0.0f );
		//Vector3 vec2 = currentPos - new Vector3( 0.0f, currentPos.y, 0.0f );
		//m_deltaA = Vector3.Angle( vec1, vec2 );
		//m_deltaY = currentPos.y - m_lastPos.y;
		//m_lastPos = currentPos;
		
		//print ( "vec1 is: " + vec1 );
		//print( "m_deltaA: " + m_deltaA );
	}
	
	public float DeltaA {
		get {
			return m_deltaA;
		}
	}
	
	public float DeltaY {
		get {
			return m_deltaY;
		}
	}
	
	public void resetDeltaA() {
		m_deltaA = 0.0f;	
	}
}