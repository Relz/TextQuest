using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TextQuest
{
	class StateElement
	{
		public object value;
		public String label;
		public bool visible;

		public StateElement()
		{
			value = "";
			label = "";
			visible = true;
		}
	}
}
