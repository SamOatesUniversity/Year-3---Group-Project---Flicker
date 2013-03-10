using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class FacebookManager : MonoBehaviour 
{
    private string 											m_userid;
    private string 											m_username;
	private string[] 										m_userdata;

	private bool											m_active = false;
	private bool											m_connectionFalied = false;
	
	void Start()
	{
		Application.ExternalCall("GetCurrentUser");	
		m_userid = "";
	}
	
    void OnGUI()
    {		
		String labelDisplay = "Connecting to Facebook...";
		if (m_connectionFalied == true) {
			labelDisplay = "";
		} else if (m_active == true && m_userid != "") {
			labelDisplay = "Welcome, " + m_username;
		}
		
		int textHeight = 24;
		Rect labelRect = new Rect(4, Screen.height - textHeight - 4, textHeight * labelDisplay.Length, textHeight);
        GUI.Label(labelRect, labelDisplay);
    }
		
    public void GetCurrentUserComplete(string fbdata)
    {
        m_userdata = fbdata.Split(',');
		if (m_userdata[0] == "undefined")
		{
			m_userid = "";
			m_username = "";
			m_active = false;
			Application.ExternalCall("GetCurrentUser");
			return;	
		}
		
        m_userid = m_userdata[0];
        m_username = m_userdata[1];
		m_active = true;
		m_connectionFalied = false;
    }
}
