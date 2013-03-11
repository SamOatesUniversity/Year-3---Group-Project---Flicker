using UnityEngine;
using System.Collections;

public class CEntityAirship : MonoBehaviour {
	public float 			Speed = 1.0f;
	public float 			PathRadius = 10.0f;
	public float 			YOffset = 1.0f;
	public uint 			NumYPositions = 20;
	public float			RateOfFire = 3.5f;
	
	private Transform 	 	m_initialPosition;
	private ArrayList		m_storedYPositions;
	private CEntityPlayer	m_playerEntity;
	private Transform		m_playerTransform;
	private float 			m_fireTimer;
	
	// Use this for initialization
	void Start () {
		m_initialPosition = this.transform;
		
		m_storedYPositions = new ArrayList();
		
		m_playerEntity = CEntityPlayer.GetInstance();
		if (m_playerEntity == null) {
			m_playerEntity = GameObject.Find("Player Spawn").GetComponent<CEntityPlayer>();	
		}
		
		m_playerTransform = m_playerEntity.transform.FindChild("Player_Mesh/Bip001/Bip001 Pelvis");
		
		float playerY = m_playerTransform.position.y;
		for( int posCount = 0; posCount < NumYPositions; posCount++ )
		{
			m_storedYPositions.Add(playerY);
		}
		m_fireTimer = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void FixedUpdate() {
		//If player falling from tower, reset ship position
		if( m_playerEntity.GetPlayerState() == PlayerState.FallingFromTower )
		{
			this.transform = m_initialPosition;	
		}
		
		//update list of player Y pos
		if( m_playerEntity.GetPlayerState() != PlayerState.Jumping )
		{
			float playerY = m_playerTransform.position.y;
			m_storedYPositions.Add(playerY);
			m_storedYPositions.RemoveAt(0);
		}
		
		//Calculate Y bos based on past NumYPositions frames
		float sumYPos = 0.0f;
		foreach( float yPos in m_storedYPositions )
		{
			sumYPos += yPos;
		}
		float averageY = 0.0f;
		if( NumYPositions > 0 )
		{
			averageY = sumYPos/NumYPositions;
		}
		float correctedY = averageY + YOffset;
		
		//update ship position
		Transform currentTransform = this.transform;
		Vector3 origin = new Vector3(0.0f, 0.0f, 0.0f);
		Vector3 up = new Vector3(0.0f, 1.0f, 0.0f);
		this.transform.RotateAround(origin, up, Speed*0.1f);
		
		Vector3 pos = this.transform.position;
		pos.y = correctedY;
		this.transform.position = pos;
	
		if( m_fireTimer >= RateOfFire )
		{
			FireCannon();
			m_fireTimer = 0.0f;
		}
		else
		{
			m_fireTimer += Time.deltaTime;	
		}
	}
	
	void FireCannon()
	{
		Object cannonball = Instantiate(Cannonball, transform.position, transform.rotation);
	}
}
