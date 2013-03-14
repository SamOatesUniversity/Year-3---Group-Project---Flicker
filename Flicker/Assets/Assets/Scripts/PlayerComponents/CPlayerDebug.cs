using UnityEngine;
using System.Collections;

public class CPlayerDebug : MonoBehaviour
{
	private bool m_visible = true;
	
	private ArrayList m_information = new ArrayList();
	
	private CEntityPlayer m_player = null;
		
	private int m_fps = 30;
	private float m_time = 0.0f;
	
	public CPlayerDebug ()
	{
	}
	
	public void SetPlayer(CEntityPlayer player)
	{
		m_player = player;	
	}
	
	void Update()
	{
		m_time += Time.deltaTime;
		if (m_time >= 1.0f)
		{
			m_time = 0.0f;
			m_fps = Mathf.RoundToInt(1.0f / Time.deltaTime);	
		}	
	}
	
	void OnGUI ()
	{
		if (!Application.isEditor)
			return;
		
		if (!m_visible || m_player == null) 
			return;
				
		if (m_player.CurrentGameState == GameState.Paused)
			return;
		
		m_information.Clear();
		m_information.Add("FPS: " + m_fps);
		m_information.Add("----------");
		m_information.Add("Player Position: " + m_player.transform.position.ToString());
		m_information.Add("Player Alpha: " + m_player.CurrentPlayerAlpha.ToString());
		m_information.Add("----------");
		m_information.Add("Player State: " + m_player.GetPlayerState().ToString()); 
		m_information.Add("Collision State: " + m_player.Physics.CurrentCollisionState.ToString());
		m_information.Add("Jump State: " + m_player.Physics.JumpType.ToString());
		m_information.Add("Ladder State: " + m_player.Physics.GetLadder.state.ToString());
		m_information.Add("----------");
		m_information.Add("Ladder Moving: " + m_player.Physics.GetLadder.moving.ToString());
		m_information.Add("Ladder Offset: " + m_player.Physics.GetLadder.offset.ToString());
		m_information.Add("Ladder Direction: " + m_player.Physics.GetLadder.direction.ToString());
		m_information.Add("----------");
		m_information.Add("Direction: " + m_player.Physics.Direction.ToString ());
		m_information.Add("Moving Direction: " + m_player.Physics.MovingDirection.ToString ());
		m_information.Add("----------");
		m_information.Add("Animation: " + m_player.CurrentAnimation());
		
		Rect labelPosition = new Rect(4, 4, 512, 512);
		GUI.Label(labelPosition, "Player Debug Information");
		labelPosition.y += 20;
		
		foreach (string info in m_information)
		{
			labelPosition.y += 20;
			GUI.Label(labelPosition, info);
		}
		
	}
	
}

