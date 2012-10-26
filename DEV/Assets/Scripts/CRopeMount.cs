using UnityEngine;
using System.Collections;

public class CRopeMount : MonoBehaviour {
	
	public GameObject RopeLink;
	public float RopeLength = 2.0f;
	public int RopeSegments = 10;
	public float SwingSpeed = 1.0f;
	public float MaxAngle = 1.0f;
	
	private CRopeLink[] m_ropeLinks = null;
	//private bool m_isPlayerAttached = false;
	private Transform m_transform;
	private float PlayerPathRadius = 11.0f;
	private float m_ropeAlpha  = 0.0f;
	private float m_ropeTheta = 0.0f;
	private float m_ropeInitialTheta = 0.0f;
	private int m_direction = -1;
	
	
	
	
	// Use this for initialization
	void Start () 
	{
		m_transform = this.transform;
		
		
		
		m_ropeInitialTheta = 180.0f; //Mathf.Tan( (this.transform.position.x - PlayerPathRadius) / this.transform.position.z);
		m_ropeTheta = m_ropeInitialTheta;
		
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
		//Vector3 endNode = startNode + new Vector3(0,-RopeLength,0); //TODO CALCULATE THIS
					
		
		Vector3 endNode = new Vector3(			
			(Mathf.Sin(m_ropeTheta * Mathf.Deg2Rad) * PlayerPathRadius) - startNode.x,
			startNode.y -RopeLength,
			(Mathf.Cos(m_ropeTheta * Mathf.Deg2Rad) * PlayerPathRadius) - startNode.z
		);		
		
		if (m_ropeInitialTheta + MaxAngle < m_ropeTheta  )
		{
			Debug.Log ("Go back");
			m_direction = -1;
		}
		else if(m_ropeInitialTheta - MaxAngle > m_ropeTheta)
		{
			Debug.Log ("Go forward");
			m_direction = 1;
		}
		
		Debug.Log(m_ropeTheta);
		
		m_ropeTheta += (1 * m_direction);
		
		Vector3 direction = endNode - startNode;
		
		Vector3 stepSize = direction / RopeSegments;
		
		
		for (int i = 0; i < RopeSegments; i++)
		{
			m_ropeLinks[i].transform.position = startNode + (stepSize * i);
		}
		
	}
}
