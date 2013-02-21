using UnityEngine;
using System.Collections;

[System.Serializable]
public enum ExpressionType {
	Expression,
	Obj
};

[System.Serializable] 
public class CLogicExpression : MonoBehaviour {
		
	public ExpressionType					expressionTypeA = ExpressionType.Obj;
	public ExpressionType					expressionTypeB = ExpressionType.Obj;
	
	public CLogicObject 					objectOne = null;
	public CLogicObject 					objectTwo = null;
	
	public CLogicExpression					expressionOne = null;
	public CLogicExpression					expressionTwo = null;
	
	public LogicOperator 					op = LogicOperator.AND;		
		
	public bool Resolve() {
	
		bool result1 = false;
		bool result2 = false;
		
		if (expressionOne != null) {
			result1 = expressionOne.Resolve();
		} else {
			result1 = objectOne != null ? objectOne.state : false;	
		}
		
		if (expressionTwo != null) {
			result2 = expressionTwo.Resolve();
		} else {
			result2 = objectTwo != null ? objectTwo.state : false;	
		}
		
		if (op == LogicOperator.AND) {
			return result1 && result2;
		} else if (op == LogicOperator.OR) {
			return result1 || result2;
		}
		
		Debug.LogError("Expression Operator '" + op + "' not handled in resolve!");
		
		return false;
	}
}
