using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(CLogicManager))]
public class CLogicEditor : Editor {
			
	public override void OnInspectorGUI()
	{
		CLogicManager manager = (CLogicManager)target;
		CLogicEquation equation = manager.equation;
		
		if (equation == null)
		{
			//Debug.LogError ("Equation is null!");
			//return;	
			equation = new CLogicEquation(); 
		}
		
		EditorGUILayout.BeginHorizontal();
		
		// expression one
		CLogicExpression exp1 = equation.GetExpression(1);
		exp1 = EditorGUILayout.ObjectField("A", exp1, typeof(CLogicExpression), true) as CLogicExpression;
		//manager.equation.SetExpression(1, exp1);
		
		// operator
		LogicOperator op = equation.GetOperator();
		op = (LogicOperator)EditorGUILayout.EnumPopup(op);
		//manager.equation.SetOperator(op);
		
		// expression two
		CLogicExpression exp2 = equation.GetExpression(2);
		exp2 = EditorGUILayout.ObjectField("B", exp2, typeof(CLogicExpression), true) as CLogicExpression;
		//manager.equation.SetExpression(2, exp2);
		
		EditorGUILayout.EndHorizontal();
		
		if (GUI.changed) {
			EditorUtility.SetDirty(manager);
		}
		
	}	
}
