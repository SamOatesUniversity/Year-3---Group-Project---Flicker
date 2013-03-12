using UnityEngine;
using System.Collections;

public class CForceVolume : CSceneObjectBase {
	
	public Vector3 ForceToApply;			//!< Force to apply to object which triggers the force volume
	public Vector3 MaximumVelocity;
		
	//called if a collidable object triggers the volume
	 void OnTriggerEnter(Collider collider) {
		if (!enabled)
			return;
		
        Rigidbody rBody = collider.gameObject.GetComponent<Rigidbody>();
		if (rBody == null)
			return;
		
		rBody.AddForce(ForceToApply);
		CEntityPlayer player = CEntityPlayer.GetInstance();
		if (player)
		{
			player.SetPlayerState(PlayerState.Jumping);
		}
    }
	
	void OnTriggerStay(Collider collider) {
		if (!enabled)
			return;
		
		Rigidbody rBody = collider.gameObject.GetComponent<Rigidbody>();
		if (rBody == null)
			return;
		
		if (rBody.velocity.y < MaximumVelocity.y)
		{
			rBody.AddForce(new Vector3(0.0f, ForceToApply.y, 0.0f));	
		}
		
		CEntityPlayer player = CEntityPlayer.GetInstance();
		if (player)
		{
			player.SetPlayerState(PlayerState.Jumping);
			player.Physics.Velocity += ForceToApply.x;	
			if(player.Physics.Velocity > MaximumVelocity.x)
				player.Physics.Velocity = MaximumVelocity.x;
		}
	}
	
	public override void LogicStateChange(bool newState) {
		enabled = newState;
		ParticleSystem ps = GetComponentInChildren<ParticleSystem>();
		if (ps != null) {
			ps.enableEmission = newState;	
		}
	}
}
