using UnityEngine;
using System.Collections;

public class CLogicEquation : Object {
	
	private CLogicExpression 				m_expressionOne = null;
	private CLogicExpression 				m_expressionTwo = null;
	private LogicOperator 					m_op = LogicOperator.AND;
	
	public bool Resolve() {
	
		bool ex1Result = m_expressionOne.Resolve();
		bool ex2Result = m_expressionTwo.Resolve();
		
		if (m_op == LogicOperator.AND) {
			return ex1Result && ex2Result;
		} else if (m_op == LogicOperator.OR) {
			return ex1Result || ex2Result;
		}
		
		Debug.LogError("Expression Operator '" + m_op + "' not handled in resolve!");
		
		return false;
	}
		
	public CLogicExpression GetExpression(
			int id
		)
	{
		if (id == 1) return m_expressionOne;
		if (id == 2) return m_expressionTwo;
		
		return null;
	}
	
	public void SetExpression(
			int id,
			CLogicExpression exp
		)
	{
		if (id == 1) m_expressionOne = exp;
		if (id == 2) m_expressionTwo = exp;
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
