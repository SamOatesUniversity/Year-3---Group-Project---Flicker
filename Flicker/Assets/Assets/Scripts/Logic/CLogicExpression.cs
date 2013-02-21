using UnityEngine;
using System.Collections;

public class CLogicExpression : MonoBehaviour {
	
	private CLogicObject 					m_objectOne = null;
	private CLogicObject 					m_objectTwo = null;
	
	private CLogicExpression				m_expressionOne = null;
	private CLogicExpression				m_expressionTwo = null;
	
	private LogicOperator 					m_op = LogicOperator.AND;		
		
	public bool Resolve() {
	
		bool result1 = false;
		bool result2 = false;
		
		if (m_expressionOne != null) {
			result1 = m_expressionOne.Resolve();
		} else {
			result1 = m_objectOne != null ? m_objectOne.GetState() : false;	
		}
		
		if (m_expressionTwo != null) {
			result2 = m_expressionTwo.Resolve();
		} else {
			result2 = m_objectTwo != null ? m_objectTwo.GetState() : false;	
		}
		
		if (m_op == LogicOperator.AND) {
			return result1 && result2;
		} else if (m_op == LogicOperator.OR) {
			return result1 || result2;
		}
		
		Debug.LogError("Expression Operator '" + m_op + "' not handled in resolve!");
		
		return false;
	}
	
	public CLogicObject GetObject(
			int id
		)
	{
		if (id == 1) return m_objectOne;
		if (id == 2) return m_objectTwo;
		
		return null;
	}
	
	public void SetObject(
			int id,
			CLogicObject obj
		)
	{
		if (id == 1) m_objectOne = obj;
		if (id == 2) m_objectTwo = obj;
	}
	
	public LogicOperator GetOperator() {
		return m_op;	
	}
	
	public void SetOperator(
			LogicOperator op
		) 
	{
		m_op = op;	
	}
}
