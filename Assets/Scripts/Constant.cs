using UnityEngine;

namespace TextQuest
{
	public static class Constant
	{
		public const char PLUS_SYMBOL = '+';
		public const char MINUS_SYMBOL = '-';

		public const string EQUAL_SYMBOL = "==";
		public const string NOT_EQUAL_SYMBOL = "!=";
		public const string MORE_SYMBOL = ")";
		public const string LESS_SYMBOL = "(";

		public const string AND = "AND";
		public const string OR = "||";

		public const char THEN = '?';
		public const char ELSE= ':';

		public const char REFERENCE_PREFIX_SYMBOL = 'R';
		public const char SHOW_PREFIX_SYMBOL = 'S';
		public const char HIDE_PREFIX_SYMBOL = 'H';

		public const int TEXT_MARGIN_TOP = 6;

		public static class VARIABLE_IN_TEXT
		{
			public const string PREFIX = "${";
			public const char POSTFIX = '}';
		}

		public static class STATE
		{
			public const string FULL_PATH = "State";

			public static class ELEMENT
			{
				public const string TAG_NAME = "StateElement";
				public const string NAME = "Name";
				public const string VALUE = "Value";
			}
		}
		public static class SCREEN
		{
			public const string PATH = "Screens/";

			public static class STATE_CHANGING
			{
				public const string TAG_NAME = "StateChanging";
				public static class ELEMENT
				{
					public const string TAG_NAME = "StateElement";
					public static class ATTRIBUTE
					{
						public const string ACTION = "Action";
						public const string NAME = "Name";
						public const string VALUE = "Value";
						public const string VISIBLE = "Visible";
						public const string IF = "If";
					}
					public static class ACTION
					{
						public const string ADD = "Add";
						public const string REMOVE = "Remove";
						public const string SHOW = "Show";
						public const string SHOW_IF = "ShowIf";
						public const string HIDE = "Hide";
					}
				}
			}

			public static class TEXT
			{
				public const string PATH = "Texts/";
				public const string TAG_NAME = "Text";

				public static class ATTRIBUTE
				{
					public const string LOAD_FROM = "LoadFrom";
				}
			}

			public static class CHOICES
			{
				public const string PATH = Constant.SCREEN.PATH + "Choices/";
				public const string TAG_NAME = "Choices";

				public static class ATTRIBUTE
				{
					public const string LOAD_FROM = "LoadFrom";
				}
				public static class CHOICE
				{
					public const string TAG_NAME = "Choice";
					public static class ATTRIBUTE
					{
						public const string TO = "To";
						public const string TO_IF = "ToIf";
						public const string DISABLE_IF = "DisableIf";
						public const string HIDE_IF = "HideIf";
					}
				}
			}

			public const string IMAGES_PATH = Constant.SCREEN.PATH + "Images/";
				public static class IMAGE
			{
				public const string TAG_NAME = "Image";

				public static class ATTRIBUTE
				{
					public const string LOAD_FROM = "LoadFrom";
				}
			}
		}

		public static class ERROR
		{
			public static class XML
			{
				public static class SCREEN
				{
					public const string NO_TEXT_ELEMENT = "Wrong Screen file, Text element is required";
					public const string NO_CHOICES_ELEMENT = "Wrong Screen file, Choices element is required";
					public const string NO_IMAGE_ELEMENT = "Wrong Screen file, Image element is required"; 
					public const string IMAGE_NOT_FOUND = "Wrong Screen file, Image not found"; 
					public static class STATE_CHANGING
					{
						public const string NO_STATE_CHANGING_ELEMENT = "Wrong Screen file, StateChanging element is required";
						public static class ELEMENT
						{
							public const string NO_ACTION_ATTRIBUTE = "Wrong Screen file, StateChanging element attribute 'Action' not specified";
							public const string NO_ACTION_ATTRIBUTE_AT_FIRST_POSITION = "Wrong Screen file, StateChanging element attribute 'Action' must be specified at first position";
						}
					}
				}

				public static class CHOICE
				{
					public const string STATE_ELEMENT_NOT_FOUND = "Wrong Choice file, State element not found";
					public const string WRONG_TO_IF_SYNTAX = "Wrong Choice file, Wrong ToIf syntax";
				}

				public const string CANNOT_PARSE_FILE = "The file could not be parsed: ";
			}

			public const string CANNOT_READ_FILE = "The file could not be read: ";
		}
	}
}