using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(CLogicManager))]
public class CLogicEditor : Editor {
			
	private CLogicManager			m_manager = null; 
	
	// 
	void OnEnable () {
		m_manager = (CLogicManager)target;
	}
	
	//
	public override void OnInspectorGUI()
	{
		EditorGUILayout.LabelField("Expression to evaluate");
		
		EditorGUILayout.BeginHorizontal();
			
		m_manager.expression = EditorGUILayout.ObjectField(m_manager.expression, typeof(CLogicExpression), true) as CLogicExpression;
			
		EditorGUILayout.EndHorizontal();
	}	
}
