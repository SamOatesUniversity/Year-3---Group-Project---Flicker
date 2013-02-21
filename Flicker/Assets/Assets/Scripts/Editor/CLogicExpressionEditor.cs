using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(CLogicExpression))]
public class CLogicExpressionEditor : Editor {
			
	private CLogicExpression			m_expression = null; 
	
	// 
	void OnEnable () {
		m_expression = (CLogicExpression)target;
	}
	
	//
	public override void OnInspectorGUI()
	{
		if (m_expression == null)
		{
			return;	
		}
		
		EditorGUILayout.BeginHorizontal();
		
		// object one
		CLogicObject obj1 = m_expression.GetObject(1);
		obj1 = EditorGUILayout.ObjectField(obj1, typeof(CLogicObject), true) as CLogicObject;
		m_expression.SetObject(1, obj1);
		
		// operator
		LogicOperator op = m_expression.GetOperator();
		op = (LogicOperator)EditorGUILayout.EnumPopup(op);
		m_expression.SetOperator(op);
		
		// object two
		CLogicObject obj2 = m_expression.GetObject(2);
		obj2 = EditorGUILayout.ObjectField(obj2, typeof(CLogicObject), true) as CLogicObject;
		m_expression.SetObject(2, obj2);
		
		EditorGUILayout.EndHorizontal();
	}	
}
