using UnityEngine;
using System.Collections;

public class CSceneObject : MonoBehaviour {
	
	/* ----------------
	    Public Members 
	   ---------------- */
	
	public bool CanWallJump 			= true;			//!< Can a player wall jump on this object?
	
	public bool IsLadder				= false; 			//!< Is the object a ladder?
	
	public float ExtraSlide				= 1.0f; 			//!< Can the player wall jump upon this object?
	
	public bool CanLedgeGrab			= true;			//!< Can the player ledge grab on the object?

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
			if (contact.otherCollider != null && contact.otherCollider.gameObject.name == "Ledge_Grab_Detection")
				ledgeGrab = contact.otherCollider;
			
			if (contact.thisCollider != null && contact.thisCollider.gameObject.name == "Ledge_Grab_Detection")
				ledgeGrab = contact.thisCollider;
			
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
	
	void OnCollisionEnter(Collision collision) {
		CheckLedgeGrab(collision);
	}
	
	void OnCollisionStay(Collision collision) {
		CheckLedgeGrab(collision);
	}

}
