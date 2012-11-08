using UnityEngine;
using System.Collections;

public class CTriggerBase : MonoBehaviour 
{
	
	
	protected bool CheckContextButton()
	{
		return Input.GetKeyDown(KeyCode.R);
	}
	
	protected float CheckRange(Vector3 _from, Vector3 _to)
	{
		return Vector3.Distance(_from,_to);
	}
	
	
	public void Start () {

			
	}
	

	
	
	public void Update () 
	{
		

	}
}

