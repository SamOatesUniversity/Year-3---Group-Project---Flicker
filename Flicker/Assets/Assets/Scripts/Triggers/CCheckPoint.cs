using UnityEngine;
using System.Collections;

public class CCheckPoint : MonoBehaviour {

	private float m_playerAlpha = 0.0f;
	private int m_direction = 1;
	private Object m_killPlane = null;
	
	public string LevelName = "Main_Menu";
	public GameObject KillPlane = null;
	public float KillYOffset = 2.0f;
	
	private GameObject m_spritesheet = null;
	
	void Start() {
		m_playerAlpha = calculateAlphaAngle();
		Transform childSprite = this.transform.FindChild("SM_SpriteSheetPlane");
		if (childSprite != null) {
			m_spritesheet = childSprite.gameObject;	
		}
	}
	
	void OnTriggerEnter(Collider collision) {
				
		GameObject gObject = collision.collider.gameObject;		
		CEntityPlayer player = gObject.GetComponent<CEntityPlayer>();
		
		if (player != null)
		{
			TriggerCheckpoint(player);
		}	
		
	}
	
	void OnTriggerStay(Collider collision) {
		
		GameObject gObject = collision.collider.gameObject;		
		CEntityPlayer player = gObject.GetComponent<CEntityPlayer>();
		
		if (player != null)
		{
			TriggerCheckpoint(player);
		}	
		
	}
	
	void TriggerCheckpoint(CEntityPlayer player)
	{
		player.LastCheckPoint = this;
		player.CurrentLevel = LevelName;
		m_playerAlpha = player.CurrentPlayerAlpha;
		m_direction = player.Physics.Direction;
		if (m_direction == 0) {
			m_direction = player.Physics.MovingDirection;
		}
		
		if (KillPlane != null && m_killPlane == null) {
			Vector3 yPos = new Vector3(0, transform.position.y - KillYOffset, 0);
			m_killPlane = Instantiate(KillPlane, yPos, Quaternion.identity);
		}
		
		if (m_spritesheet != null) {
			m_spritesheet.active = true;	
		}
	}
	
	private float calculateAlphaAngle()
	{
		Vector3 position = transform.localPosition;
		position.x = Mathf.Round(position.x * 10f) / 10f;
		position.y = Mathf.Round(position.y * 10f) / 10f;
		position.z = Mathf.Round(position.z * 10f) / 10f;
		
		// handle 0 cases
		if (position.x == 0 && position.z >= 0) {
			return 0;	
		} else if (position.x == 0 && position.z < 0) {
			return 180;
		} else if (position.x >= 0 && position.z == 0) {
			return 90;
		} else if (position.x < 0 && position.z == 0) {
			return 270;
		}
		
		// not a straight line, so do some trig to calculate the angle
		Vector2 xz = new Vector2(transform.localPosition.x, transform.localPosition.z);
		Vector2 or = new Vector2(0, position.x < 0 ? -xz.magnitude : xz.magnitude);
				
		float o = (or - xz).magnitude * 0.5f;
		float h = xz.magnitude;
		
		float oh = o / h;
		float alpha = Mathf.Asin(oh) * 2.0f;
		alpha = alpha * Mathf.Rad2Deg;
		
		alpha = position.x < 0 ? 180.0f + alpha : alpha;
		
		return alpha;
	}
	
	/*
	*	\brief Get the player alpha stored at the checkpoint
	*/
	public float PlayerCheckPointAlpha {
		get {
			return m_playerAlpha;
		}
	}
	
	public int Direction {
		get {
			return m_direction;	
		}	
	}
		
}