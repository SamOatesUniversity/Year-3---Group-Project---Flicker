using UnityEngine;
using System.Collections;

public class CRopeMount : MonoBehaviour {
	
	public GameObject RopeLink;
	public float RopeLength = 2.0f;
	public int RopeSegments = 10;
	public float SwingSpeed = 1.0f;
	public float MaxAngle = 90.0f;
	
	private CRopeLink[] m_ropeLinks = null;
	//private bool m_isPlayerAttached = false;
	private Transform m_transform;
	private float PlayerPathRadius = 10.0f;
	
	
	
	// Use this for initialization
	void Start () 
	{
		m_transform = this.transform;
		
		
		m_ropeLinks = new CRopeLink[ RopeSegments ];
		float offset = RopeLength / RopeSegments;
		float totalOffset = 0 + offset;
		
		for (int i = 0; i < RopeSegments; i++)
		{
			
			
			GameObject obj = (GameObject)Instantiate( RopeLink, m_transform.position + new Vector3(0, -totalOffset,0),Quaternion.identity);
			
			CRopeLink link = obj.GetComponent<CRopeLink>();
			link.m_parent = this.GetComponent<CRopeMount>();
			//link.transform.parent = m_transform;
			m_ropeLinks[i] = link;
			totalOffset += offset;
		}
		
		
		
		
		
	}
	
	// Update is called once per frame
	void Update () {
		
		//ensure the nodes are attached to the radius of the center column
		//pull + push them to keep them on the correct x + z  planes
		
		
		//start node position
		Vector3 startNode = this.transform.position;
		
		
		//last node position
		Vector3 endNode = startNode + new Vector3(0,-5,0); //TODO CALCULATE THIS
		/*
			Vector3 test  = new Vector3(
			Mathf.Sin(m_playerPositionAlpha * Mathf.Deg2Rad) * PlayerPathRadius,
			transform.position.y,
			Mathf.Cos(m_playerPositionAlpha * Mathf.Deg2Rad) * PlayerPathRadius
		);
		*/
		
		
		//Vector from last to start
		Vector3 direction = endNode - startNode;
		Vector3 step = direction / RopeSegments;
		
		
		for (int i = 0; i < RopeSegments; i++)
		{
			m_ropeLinks[i].transform.position = startNode + (step * i);
		}	
	
	}
}
