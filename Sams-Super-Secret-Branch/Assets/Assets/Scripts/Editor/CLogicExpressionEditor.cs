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
		m_expression.expressionTypeA = (ExpressionType)EditorGUILayout.EnumPopup(m_expression.expressionTypeA);
		m_expression.expressionTypeB = (ExpressionType)EditorGUILayout.EnumPopup(m_expression.expressionTypeB);		
		EditorGUILayout.EndHorizontal();
				
		EditorGUILayout.BeginHorizontal();
		
		// object one
		if (m_expression.expressionTypeA == ExpressionType.Obj)
		{
			CLogicObject obj1 = m_expression.objectOne;
			obj1 = EditorGUILayout.ObjectField(obj1, typeof(CLogicObject), true) as CLogicObject;
			m_expression.objectOne = obj1;
		}
		else
		{
			CLogicExpression exp1 = m_expression.expressionOne;
			exp1 = EditorGUILayout.ObjectField(exp1, typeof(CLogicExpression), true) as CLogicExpression;
			m_expression.expressionOne = exp1;			
		}
		
		// operator
		LogicOperator op = m_expression.op;
		op = (LogicOperator)EditorGUILayout.EnumPopup(op, GUILayout.MaxWidth(50));
		m_expression.op = op;
		
		// object two
		if (m_expression.expressionTypeB == ExpressionType.Obj)
		{
			CLogicObject obj2 = m_expression.objectTwo;
			obj2 = EditorGUILayout.ObjectField(obj2, typeof(CLogicObject), true) as CLogicObject;
			m_expression.objectTwo = obj2;
		}
		else
		{
			CLogicExpression exp2 = m_expression.expressionTwo;
			exp2 = EditorGUILayout.ObjectField(exp2, typeof(CLogicExpression), true) as CLogicExpression;
			m_expression.expressionTwo = exp2;			
		}
		
		EditorGUILayout.EndHorizontal();
		
		
	}	
}
