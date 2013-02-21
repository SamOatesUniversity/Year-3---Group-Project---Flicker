using UnityEngine;
using System.Collections;

public class CGruntDebug : MonoBehaviour
{
	private bool m_visible = true;
	
	private ArrayList m_information = new ArrayList();
	
	private CEntityGrunt m_player = null;
	
	public CGruntDebug ()
	{
	}
	
	public void SetPlayer(CEntityGrunt player)
	{
		m_player = player;	
	}
	
	void OnGUI ()
	{
		if (!m_visible || m_player == null) return;
		
		m_information.Clear();
		m_information.Add("Player Position: " + m_player.transform.position.ToString());
		m_information.Add("Player Alpha: " + m_player.CurrentPlayerAlpha.ToString());
		m_information.Add("Player State: " + m_player.GetGruntState().ToString()); 
		m_information.Add("Collision State: " + m_player.Physics.CurrentCollisionState.ToString());
		m_information.Add("Jump State: " + m_player.Physics.JumpType.ToString());
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
