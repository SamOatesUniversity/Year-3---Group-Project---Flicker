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
	
	public static bool CheckLedgeGrab(Collision collision) {
		
		foreach (ContactPoint contact in collision)
		{
			Collider ledgeGrab = null;
			CSceneObject sceneLedge = null;
			
			if (contact.otherCollider != null && contact.otherCollider.gameObject.name == "Ledge_Grab_Detection")
			{
				ledgeGrab = contact.otherCollider;
				
				if (contact.thisCollider != null && contact.thisCollider.gameObject != null)
					sceneLedge = contact.thisCollider.gameObject.GetComponent<CSceneObject>();
			}
			else if (contact.thisCollider != null && contact.thisCollider.gameObject.name == "Ledge_Grab_Detection")
			{
				ledgeGrab = contact.thisCollider;
				
				if (contact.otherCollider != null && contact.otherCollider.gameObject != null)
					sceneLedge = contact.otherCollider.gameObject.GetComponent<CSceneObject>();
			}
				
			if (sceneLedge == null || !sceneLedge.CanLedgeGrab)
			{
				if (ledgeGrab != null) ledgeGrab.enabled = false;
				continue;
			}
				
			if (ledgeGrab != null)
			{
				if (CPlayerPhysics.isNearly(contact.normal.normalized.y, -1.0f, 0.1f))
				{
					// player hit the ledge grab area	
					GameObject player = contact.otherCollider.gameObject.transform.parent.gameObject;
					if (player != null)
					{
						CEntityPlayer playerEntity = player.GetComponent<CEntityPlayer>();	
						if ( playerEntity != null && 
							playerEntity.GetPlayerState() != PlayerState.LedgeHang && 
							playerEntity.GetPlayerState() != PlayerState.LedgeClimb &&
							playerEntity.GetPlayerState() != PlayerState.LedgeClimbComplete
						)
						{
							CPlayerPhysics phy = playerEntity.Physics;
							phy.SetLedgeGrabState(playerEntity, PlayerState.LedgeHang);
							contact.otherCollider.enabled = false;
							return true;
						}
					}
				}
				else
				{
					ledgeGrab.enabled = false;
					return false;
				}
			}
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
		CheckLedgeGrab(collision);
		CheckKillOnTouch(collision);
	}
	
	void OnCollisionStay(Collision collision) {
		CheckLedgeGrab(collision);
		CheckKillOnTouch(collision);
	}

}
