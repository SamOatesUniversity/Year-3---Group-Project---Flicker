using UnityEngine;
using System.Collections;

public class CLightChargePoint : MonoBehaviour {
	
	public float ChargingRange = 1.0f;
	public float ChargingRate = 20.0f;
	public GameObject Player;
	private CPlayerLight m_light;
	

	// Use this for initialization
	void Start () {		
	
		//Save this to save a lookup every frame
		m_light = Player.GetComponent<CPlayerLight>();
	}
	
	// Update is called once per frame
	void Update () 
	{
				
		//Check range of player from charge point IF NEAR CHARGE!		
	 if ( Vector3.Distance ( Player.transform.position , this.transform.position)  < ChargingRange )
		{
			m_light.ChargeLight(ChargingRate * Time.deltaTime);				
		}
	}
}
