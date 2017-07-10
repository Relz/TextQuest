using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TextQuest
{
	class Expression
	{
		public String str;
		public bool truth;

		public Expression()
		{
			str = "";
			truth = false;
		}

		public static bool Equal(object operand0, object operand1)
		{
			int operand0Number = 0;
			int operand1Number = 0;

			if (Int32.TryParse(operand0.ToString(), out operand0Number) &&
				Int32.TryParse(operand1.ToString(), out operand1Number))
			{
				return operand0Number == operand1Number;
			}
			else
			{
				return false;
			}
		}

		public static bool NotEqual(object operand0, object operand1)
		{
			return !Equal(operand0, operand1);
		}

		public static bool More(object operand0, object operand1)
		{
			int operand0Number = 0;
			int operand1Number = 0;

			if (Int32.TryParse(operand0.ToString(), out operand0Number) &&
				Int32.TryParse(operand1.ToString(), out operand1Number))
			{
				return operand0Number > operand1Number;
			}
			else
			{
				return false;
			}
		}

		public static bool Less(object operand0, object operand1)
		{
			int operand0Number = 0;
			int operand1Number = 0;

			if (Int32.TryParse(operand0.ToString(), out operand0Number) &&
				Int32.TryParse(operand1.ToString(), out operand1Number))
			{
				return operand0Number < operand1Number;
			}
			else
			{
				return false;
			}
		}

		public static Func<object, object, bool> GetCompareFunction(String subExpressionStr)
		{
			if (subExpressionStr.IndexOf(Constant.EQUAL_SYMBOL) != -1)
			{
				return Expression.Equal;
			}
			if (subExpressionStr.IndexOf(Constant.NOT_EQUAL_SYMBOL) != -1)
			{
				return Expression.NotEqual;
			}
			if (subExpressionStr.IndexOf(Constant.MORE_SYMBOL) != -1)
			{
				return Expression.More;
			}
			if (subExpressionStr.IndexOf(Constant.LESS_SYMBOL) != -1)
			{
				return Expression.Less;
			}
			return (operand0, operand1) => false;
		}
	}
}
