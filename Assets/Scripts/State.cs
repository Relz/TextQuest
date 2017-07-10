using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TextQuest
{
	class State : IEnumerator, IEnumerable
	{
		public State()
		{
			state = new List<KeyValuePair<String, StateElement>>();
		}

		public KeyValuePair<String, StateElement> this[int key]
		{
			get
			{
				return state[key];
			}
			set
			{
				state[key] = value;
			}
		}

		public IEnumerator GetEnumerator()
		{
			return state.GetEnumerator();
		}

		public bool MoveNext()
		{
			return GetEnumerator().MoveNext();
		}

		public void Reset()
		{
			//state.GetEnumerator().Reset();
		}

		public object Current
		{
			get { return GetEnumerator().Current; }
		}

		public int Count
		{
			get { return state.Count; }
		}

		public int GetElementIndexByName(String stateElementName)
		{
			for (var i = 0; i < state.Count; i++)
			{
				if (state[i].Key == stateElementName)
				{
					return i;
				}
			}
			return -1;
		}

		public bool DoesContainsElement(String stateElementName)
		{
			return GetElementIndexByName(stateElementName) != -1;
		}

		public bool RemoveElement(String stateElementName)
		{
			int stateElementIndexToRemove = GetElementIndexByName(stateElementName);
			if (stateElementIndexToRemove == -1)
			{
				return false;
			}
			state.RemoveAt(stateElementIndexToRemove);
			return true;
		}

		public void AddElement(KeyValuePair<String, StateElement> element)
		{
			if (!state.Contains(element))
			{
				state.Add(element);
			}
		}

		private List<KeyValuePair<String, StateElement>> state;
	}
}
