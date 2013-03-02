using UnityEngine;
using System.Collections;

public class MathHelpers : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public Vector2 Bezier2(Vector2 Start, Vector2 Control, Vector2 End, float t)
	{
	    return (((1-t)*(1-t)) * Start) + (2 * t * (1 - t) * Control) + ((t * t) * End);
	}
	 
	public Vector3 Bezier2(Vector3 Start, Vector3 Control, Vector3 End, float t)
	{
	    return (((1-t)*(1-t)) * Start) + (2 * t * (1 - t) * Control) + ((t * t) * End);
	}
	 
	public Vector2 Bezier3(Vector2 s, Vector2 st, Vector2 et, Vector2 e, float t)
	{
	    return (((-s + 3*(st-et) + e)* t + (3*(s+et) - 6*st))* t + 3*(st-s))* t + s;
	}
	 
	public Vector3 Bezier3(Vector3 s, Vector3 st, Vector3 et, Vector3 e, float t)
	{
	    return (((-s + 3*(st-et) + e)* t + (3*(s+et) - 6*st))* t + 3*(st-s))* t + s;
	}
}
