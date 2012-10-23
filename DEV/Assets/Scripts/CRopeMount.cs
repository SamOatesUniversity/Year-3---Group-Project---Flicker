using UnityEngine;
using System.Collections;

public class CRopeMount : MonoBehaviour {
	
	public GameObject RopeLink;
	public float RopeLength = 1.0f;
	public int RopeSegments = 10;
	public float SwingSpeed = 1.0f;
	public float MaxAngle = 90.0f;
	
	//private CRopeLink[] m_ropeLinks = null;
	private bool m_isPlayerAttached = false;
	
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
