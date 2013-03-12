using UnityEngine;
using System.Collections;

public class CEntityAirship : MonoBehaviour {
	public float 			FiringSpeed = 1.0f;
	public float 			CirclingSpeed = 5.0f;
	public float 			PathRadius = 10.0f;
	public float 			YOffset = 1.0f;
	public uint 			NumYPositions = 20;
	public float			RateOfFire = 3.5f;
	public GameObject		CanonballObject = null;
	public float 			firingArcLength = 10.0f;
	
	private Transform 	 	m_initialTransform;
	private ArrayList		m_storedYPositions;
	private CEntityPlayer	m_playerEntity;
	private Transform		m_playerTransform;
	private float 			m_fireTimer;
	private float 			testLength;
	
	private bool			m_isFiring;
	
	// Use this for initialization
	void Start () {
		m_initialTransform = this.transform;
		
		m_storedYPositions = new ArrayList();
		
		m_fireTimer = 0.0f;
		m_isFiring = false;
		testLength = 0.0f;
		
		m_playerEntity = CEntityPlayer.GetInstance();
		if (m_playerEntity == null) {
			GameObject playerObject = GameObject.Find("Player Spawn");
			if (playerObject != null) {
				m_playerEntity = playerObject.GetComponent<CEntityPlayer>();	
			}
			if (m_playerEntity == null) {
				Debug.LogError("CEntityAirship: Cannot find player");
				return;
			}
		}
		
		m_playerTransform = m_playerEntity.transform.FindChild("Player_Mesh/Bip001/Bip001 Pelvis");
		
		float playerY = m_playerTransform.position.y;
		for( int posCount = 0; posCount < NumYPositions; posCount++ )
		{
			m_storedYPositions.Add(playerY);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void FixedUpdate() {
		//test if in arc of player
		//Vector3 adjustedOriginPlayer = new Vector3(0.0f, m_playerEntity.transform.position.y, 0.0f); 
		//Vector3 playerOriginVec = m_playerEntity.transform.position - adjustedOriginPlayer;
		//Vector3 adjustedOriginShip = new Vector3(0.0f, this.transform.position.y, 0.0f);
		//Vector3 shipOriginVec = this.transform.position - adjustedOriginShip;
		
		if( m_playerEntity )
		{
			//float shipDotPlayer = Vector3.Dot(playerOriginVec, shipOriginVec);
			Vector3 shipToPlayer = m_playerEntity.transform.position - this.transform.position;
			float distanceFromPlayer = shipToPlayer.magnitude;
			if( distanceFromPlayer < firingArcLength )
			{
				m_isFiring = true;
			}
			else
			{
				m_isFiring = false;	
			}
			testLength = distanceFromPlayer;
			
			//If player falling from tower, reset ship position
			if( m_playerEntity.GetPlayerState() == PlayerState.FallingFromTower )
			{
				this.transform.position = m_initialTransform.position;
				this.transform.rotation = m_initialTransform.rotation;
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
			float speed = 0.0f;
			if( m_isFiring )
			{
				speed = FiringSpeed;	
			}
			else
			{
				speed = CirclingSpeed;
			}
			
			//update ship position
			Transform currentTransform = this.transform;
			Vector3 origin = new Vector3(0.0f, 0.0f, 0.0f);
			Vector3 up = new Vector3(0.0f, 1.0f, 0.0f);
			this.transform.RotateAround(origin, up, speed*0.1f);
			
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
	}
	
	void FireCannon()
	{
		Object cannonball = Instantiate(CanonballObject, this.transform.position, this.transform.rotation);
	}
}
