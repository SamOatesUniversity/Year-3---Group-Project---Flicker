using UnityEngine;
using System.Collections;

public enum eLedgeType {
	Wall,
	Free
}

public class CSceneObject : MonoBehaviour {
		
	/* ----------------
	    Public Members 
	   ---------------- */
	
	public bool CanWallJump 			= true;			//!< Can a player wall jump on this object?
	
	public bool IsLadder				= false; 		//!< Is the object a ladder?
	
	public float ExtraSlide				= 1.0f; 		//!< Can the player wall jump upon this object?
	
	public bool CanLedgeGrab			= true;			//!< Can the player ledge grab on the object?
	
	public eLedgeType LedgeType 		= eLedgeType.Free;
	
	public bool KillPlayerOnTouch		= false;		//!< 
	
	// Use this for initialization
	void Start() 
    {
	
	}
	
	// Update is called once per frame
	void Update() 
    {
	
	}
	
	public bool CheckLadder(Collider collision) {	
		
		if (this.name == "LadderBASE" && collision.name == "Player Spawn")
		{
			GameObject playerObject = collision.gameObject;
			if (playerObject == null)
				return false;
			
			CEntityPlayer player = playerObject.GetComponent<CEntityPlayer>();
			if (player == null)
				return false;
			
			LadderState currentLadderState = player.Physics.GetLadder.state;
			
			if (currentLadderState == LadderState.None || currentLadderState == LadderState.OnMiddle || currentLadderState == LadderState.JumpingOff) {
				player.Physics.GetLadder.state = LadderState.AtBase;
			}
						
			return true;
		}
		
		if (this.name == "LadderMID" && collision.name == "Player Spawn")
		{
			GameObject playerObject = collision.gameObject;
			if (playerObject == null)
				return false;
			
			CEntityPlayer player = playerObject.GetComponent<CEntityPlayer>();
			if (player == null)
				return false;
			
			LadderState currentLadderState = player.Physics.GetLadder.state;
			
			if (player.GetPlayerState() == PlayerState.OnLadder || currentLadderState == LadderState.None) {
				player.Physics.GetLadder.state = LadderState.OnMiddle;
				player.SetPlayerState(PlayerState.OnLadder);
				player.Physics.CurrentCollisionState = CollisionState.None;
			}
						
			return true;
		}
		
		if (this.name == "LadderTOP" && collision.name == "Player Spawn")
		{
			GameObject playerObject = collision.gameObject;
			if (playerObject == null)
				return false;
			
			CEntityPlayer player = playerObject.GetComponent<CEntityPlayer>();
			if (player == null)
				return false;
			
			LadderState currentLadderState = player.Physics.GetLadder.state;
			
			if (currentLadderState == LadderState.None || currentLadderState == LadderState.OnMiddle) {
				player.Physics.GetLadder.state = LadderState.OnTop;
				player.SetPlayerState(PlayerState.OnLadder);
				player.Physics.CurrentCollisionState = CollisionState.None;
			}
						
			return true;
		}
		
		return false;
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
			phy.SetLedgeGrabState(player, PlayerState.LedgeHang, LedgeType);
			
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
		CheckLadder(other);
	}
	
	void OnTriggerExit(Collider collision) {
		
		if (this.name == "LadderBASE" && collision.name == "Player Spawn")
		{
			GameObject playerObject = collision.gameObject;
			if (playerObject == null)
				return;
			
			CEntityPlayer player = playerObject.GetComponent<CEntityPlayer>();
			if (player == null)
				return;
			
			LadderState currentLadderState = player.Physics.GetLadder.state;
			
			if (currentLadderState == LadderState.AtBase) {
				player.Physics.GetLadder.state = LadderState.None;
			}
		}
		
	}
	
	public virtual void LogicStateChange(bool newState) {
			
	}
}
