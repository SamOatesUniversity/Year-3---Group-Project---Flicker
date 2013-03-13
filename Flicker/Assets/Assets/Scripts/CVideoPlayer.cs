using UnityEngine;
using System.Collections;

public class CVideoPlayer : MonoBehaviour {
	
	public MovieTexture		Movie = null;
	public string			NextLevel = "";

	// Use this for initialization
	void Start () {
		Movie.Play();
	}
	
	// Update is called once per frame
	void Update () {
		if (!Movie.isPlaying || Input.anyKey)
		{
			Application.LoadLevel(NextLevel);	
		}
	}
	
	void OnGUI()
	{		
		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Movie, ScaleMode.StretchToFill);
	}
}
