using UnityEngine;
using System.Collections;

public class CSceneObject : MonoBehaviour {
	
	/* ----------------
	    Public Members 
	   ---------------- */
	
	public bool CanWallJump 			= true;			//!< Can a player wall jump on this object?
	
	public bool IsLadder				= false; 		//!< Is the object a ladder?
	
	public float ExtraSlide				= 1.0f; 		//!< Can the player wall jump upon this object?
	
	public bool CanLedgeGrab			= true;			//!< Can the player ledge grab on the object?
	
	public bool KillPlayerOnTouch		= false;		//!< 
	
	// Use this for initialization
	void Start() 
    {
	
	}
	
	// Update is called once per frame
	void Update() 
    {
	
	}
	
	public bool CheckLedgeGrab(Collider collision) {		
		
		if (this.name == "ledge_grab_area" && collision.name == "Ledge_Grab_Detection")
		{
			GameObject playerObject = collision.attachedRigidbody.gameObject;
			if (playerObject == null)
				return false;
			
			CEntityPlayer player = playerObject.GetComponent<CEntityPlayer>();
			if (player == null)
				return false;
			
			CPlayerPhysics phy = player.Physics;
			phy.SetLedgeGrabState(player, PlayerState.LedgeHang);
			
			return true;
		}
		
		return false;
	}
	
	private void CheckKillOnTouch(Collision collision)
	{
		if (!KillPlayerOnTouch)
			return;
		
		foreach (ContactPoint contact in collision)
		{
			GameObject player = null;
			if (contact.otherCollider != null && contact.otherCollider.gameObject.name == "Player Spawn")
				player = contact.otherCollider.gameObject;
			if (contact.thisCollider != null && contact.thisCollider.gameObject.name == "Player Spawn")
				player = contact.thisCollider.gameObject;
			
			if (player == null)
				continue;
			
			CEntityPlayer entityPlayer = player.GetComponent<CEntityPlayer>();
			if (entityPlayer == null)
				return;
			
			entityPlayer.PushPlayerFromTower();
			
		}
	}
	
	void OnCollisionEnter(Collision collision) {
		CheckKillOnTouch(collision);
	}
	
	void OnCollisionStay(Collision collision) {
		CheckKillOnTouch(collision);
	}
	
	void OnTriggerEnter(Collider other) {
		CheckLedgeGrab(other);	
	}
	
	public virtual void LogicStateChange(bool newState) {
			
	}
}
