using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(CLogicBase))]
public class CLogicEditor : Editor {
		
	private SerializedObject m_logicHandler = null;
	private SerializedProperty m_logicInputs = null;
	
	private static GUIContent
		insertContent = new GUIContent("+", "Add New Logic Operator");
	
	private static GUILayoutOption
		buttonWidth = GUILayout.MaxWidth(20f);
	
	private int m_noofOperators = 0;
	
	void OnEnable () {		
		m_logicHandler = new SerializedObject(target);
		m_logicInputs = m_logicHandler.FindProperty("LogicInputs");
	}
	
	public override void OnInspectorGUI()
	{
		m_logicHandler.Update();

		for(int i = 0; i < m_logicInputs.arraySize; i++){
			
			EditorGUILayout.BeginHorizontal();
			
			SerializedProperty logicObject = m_logicInputs.GetArrayElementAtIndex(i);
			
			EditorGUILayout.PropertyField(logicObject.FindPropertyRelative("logicObject"));
			
			SerializedProperty operators = logicObject.FindPropertyRelative("logicOperator");
			for(int opIndex = 0; opIndex < operators.arraySize; opIndex++){
				EditorGUILayout.PropertyField(operators.GetArrayElementAtIndex(opIndex));
			}
			
			EditorGUILayout.EndHorizontal();
		}
		
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Number of operations", GUILayout.MaxWidth(200.0f));
		m_noofOperators = EditorGUILayout.IntField(m_noofOperators, GUILayout.MaxWidth(40.0f));
		if (m_noofOperators != m_logicInputs.arraySize)
		{
			CLogicBase logicBase = (CLogicBase)target;
			logicBase.LogicInputs = new CLogicBase.LogicInput[m_noofOperators];
			m_logicInputs = m_logicHandler.FindProperty("LogicInputs");
		}
		EditorGUILayout.EndHorizontal();


		m_logicHandler.ApplyModifiedProperties();
	}	
}
