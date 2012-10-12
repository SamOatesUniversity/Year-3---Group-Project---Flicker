using UnityEngine;
using System.Collections;

public class CForceVolume : MonoBehaviour {
	
	public Vector3 ForceToApply;			//!< Force to apply to object which triggers the force volume
	public Vector3 MaximumVelocity;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	//called if a collidable object triggers the volume
	 void OnTriggerEnter(Collider collider) {
        Rigidbody rBody = collider.gameObject.GetComponent<Rigidbody>();
		rBody.AddForce(ForceToApply);
		CEntityPlayer player = collider.gameObject.GetComponent<CEntityPlayer>();
		if(player)
		{
			player.SetJumping();
		}
    }
	
	void OnTriggerStay(Collider collider) {
		Rigidbody rBody = collider.gameObject.GetComponent<Rigidbody>();
		if(rBody.velocity.y < MaximumVelocity.y)
		{
			rBody.AddForce(new Vector3(0.0f, ForceToApply.y, 0.0f));	
		}
		
		CEntityPlayer player = collider.gameObject.GetComponent<CEntityPlayer>();
		if(player)
		{
			if(player.GetVelocity() < MaximumVelocity.x)
			{
				player.AddHorizontalForce(ForceToApply.x);	
			}
		}
	}
}
