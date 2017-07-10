using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace TextQuest
{
	public class GameControllerScript : MonoBehaviour {

		void Start()
		{
			LoadState();
			LoadScreen("0");
		}

		private void LoadState()
		{
			state = new State();
			XmlTextReader xmlTextReader = null;
			ParseFileToXML(Constant.STATE.FULL_PATH, ref xmlTextReader);
			while (xmlTextReader.ReadToFollowing(Constant.STATE.ELEMENT.TAG_NAME))
			{
				String stateElementName = "";
				StateElement stateElement = new StateElement();
				for (int i = 0; i < xmlTextReader.AttributeCount; ++i)
				{
					xmlTextReader.MoveToAttribute(i);
					switch (xmlTextReader.Name)
					{
						case Constant.STATE.ELEMENT.NAME:
							stateElementName = xmlTextReader.Value;
							break;
						case Constant.STATE.ELEMENT.VALUE:
							stateElement.value = xmlTextReader.Value;
							break;
					}
				}
				xmlTextReader.MoveToContent();
				stateElement.label = xmlTextReader.ReadInnerXml();
				state.AddElement(new KeyValuePair<String, StateElement>(stateElementName, stateElement));
			}
		}

		private void LoadScreen(String screenFileName)
		{
			scrollRect.verticalNormalizedPosition = 1f;
			XmlTextReader xmlTextReader = null;
			ParseFileToXML(Constant.SCREEN.PATH + screenFileName, ref xmlTextReader);
			ApplyScreenStateChanging(ref xmlTextReader);
			UpdateState();
			LoadScreenText(ref xmlTextReader);
			LoadScreenChoices(ref xmlTextReader);
			LoadScreenImage(ref xmlTextReader);
			xmlTextReader.Close();
		}

		private void ApplyScreenStateChanging(ref XmlTextReader xmlTextReader)
		{
			if (!xmlTextReader.ReadToFollowing(Constant.SCREEN.STATE_CHANGING.TAG_NAME))
			{
				Debug.Log(Constant.ERROR.XML.SCREEN.STATE_CHANGING.NO_STATE_CHANGING_ELEMENT);
				Application.Quit();
			}

			while (xmlTextReader.Read())
			{
				if (xmlTextReader.Name == Constant.SCREEN.STATE_CHANGING.TAG_NAME && 
					xmlTextReader.NodeType == XmlNodeType.EndElement)
				{
					break;
				}
				if (xmlTextReader.Name != Constant.SCREEN.STATE_CHANGING.ELEMENT.TAG_NAME)
				{
					continue;
				}
				if (xmlTextReader.AttributeCount == 0)
				{
					Debug.Log(Constant.ERROR.XML.SCREEN.STATE_CHANGING.ELEMENT.NO_ACTION_ATTRIBUTE);
					Application.Quit();
				}
				xmlTextReader.MoveToAttribute(0);
				if (xmlTextReader.Name != Constant.SCREEN.STATE_CHANGING.ELEMENT.ATTRIBUTE.ACTION)
				{
					Debug.Log(Constant.ERROR.XML.SCREEN.STATE_CHANGING.ELEMENT.NO_ACTION_ATTRIBUTE_AT_FIRST_POSITION);
					Application.Quit();
				}
				String stateElementAction = xmlTextReader.Value;
				String stateElementName = "";
				String stateElementIfStatement = "";
				StateElement stateElement = new StateElement();
				for (int i = 1; i < xmlTextReader.AttributeCount; ++i)
				{
					xmlTextReader.MoveToAttribute(i);
					switch (xmlTextReader.Name)
					{
						case Constant.SCREEN.STATE_CHANGING.ELEMENT.ATTRIBUTE.NAME:
							stateElementName = xmlTextReader.Value;
							break;
						case Constant.SCREEN.STATE_CHANGING.ELEMENT.ATTRIBUTE.VALUE:
							stateElement.value = xmlTextReader.Value;
							break;
						case Constant.SCREEN.STATE_CHANGING.ELEMENT.ATTRIBUTE.VISIBLE:
							stateElement.visible = Boolean.Parse(xmlTextReader.Value);
							break;
						case Constant.SCREEN.STATE_CHANGING.ELEMENT.ATTRIBUTE.IF:
							stateElementIfStatement = xmlTextReader.Value;
							break;
					}
				}
				switch (stateElementAction)
				{
					case Constant.SCREEN.STATE_CHANGING.ELEMENT.ACTION.ADD:
						{
							state.AddElement(new KeyValuePair<String, StateElement>(stateElementName, stateElement));
							break;
						}
					case Constant.SCREEN.STATE_CHANGING.ELEMENT.ACTION.REMOVE:
						{
							if (!state.RemoveElement(stateElementName))
							{
								Debug.Log(Constant.ERROR.XML.CHOICE.STATE_ELEMENT_NOT_FOUND + ": " + stateElementName);
								Application.Quit();
							}
							break;
						}
					case Constant.SCREEN.STATE_CHANGING.ELEMENT.ACTION.SHOW:
						{
							int stateElementIndex = state.GetElementIndexByName(stateElementName);
							if (stateElementIndex == -1)
							{
								Debug.Log(Constant.ERROR.XML.CHOICE.STATE_ELEMENT_NOT_FOUND + ": " + stateElementName);
								Application.Quit();
							}
							state[stateElementIndex].Value.visible = true;
							break;
						}
					case Constant.SCREEN.STATE_CHANGING.ELEMENT.ACTION.SHOW_IF:
						{
							int stateElementIndex = state.GetElementIndexByName(stateElementName);
							if (stateElementIndex == -1)
							{
								Debug.Log(Constant.ERROR.XML.CHOICE.STATE_ELEMENT_NOT_FOUND + ": " + stateElementName);
								Application.Quit();
							}
							Debug.Log(stateElementIfStatement);
							if (EvaluateExpression(stateElementIfStatement))
							{
								Debug.Log(state[stateElementIndex].Key);
								state[stateElementIndex].Value.visible = true;
							}
							break;
						}
					case Constant.SCREEN.STATE_CHANGING.ELEMENT.ACTION.HIDE:
						{
							int stateElementIndex = state.GetElementIndexByName(stateElementName);
							if (stateElementIndex == -1)
							{
								Debug.Log(Constant.ERROR.XML.CHOICE.STATE_ELEMENT_NOT_FOUND + ": " + stateElementName);
								Application.Quit();
							}
							state[stateElementIndex].Value.visible = false;
							break;
						}
				}
				xmlTextReader.MoveToContent();
				stateElement.label = xmlTextReader.ReadInnerXml();
			}
		}

		private void UpdateState()
		{
			DestroyChildren(statePanel.transform);
			float stateElementHeight = stateElementPrefab.GetComponent<RectTransform>().sizeDelta.y;
			int currentStateIndex = 0;
			foreach (KeyValuePair<String, StateElement> stateElement in state)
			{
				if (!stateElement.Value.visible)
				{
					continue;
				}
				Transform newStateElementTransform = Instantiate(
						stateElementPrefab,
						new Vector2(
							statePanel.transform.position.x,
							statePanel.transform.position.y - currentStateIndex * (stateElementHeight + Constant.TEXT_MARGIN_TOP)
						),
						Quaternion.identity,
						statePanel.transform
				);
				newStateElementTransform.GetComponent<Text>().text =
					stateElement.Value.label + ": " + stateElement.Value.value;
				++currentStateIndex;
			}
		}

		private void LoadScreenText(ref XmlTextReader xmlTextReader)
		{
			if (!xmlTextReader.ReadToFollowing(Constant.SCREEN.TEXT.TAG_NAME))
			{
				Debug.Log(Constant.ERROR.XML.SCREEN.NO_TEXT_ELEMENT);
				Application.Quit();
			}
			String textFileName = xmlTextReader.GetAttribute(Constant.SCREEN.TEXT.ATTRIBUTE.LOAD_FROM);
			String parsedFileContent = GetFileContent(Constant.SCREEN.TEXT.PATH + textFileName);
			InsertVariablesToText(ref parsedFileContent);
			contentText.text = parsedFileContent;
		}

		private void InsertVariablesToText(ref String parsedFileContent)
		{
			int variableInTextPrefixPosition = 0;
			while ((variableInTextPrefixPosition = parsedFileContent.IndexOf(Constant.VARIABLE_IN_TEXT.PREFIX)) != -1)
			{
				String variableInTextName = "";
				int i = variableInTextPrefixPosition + Constant.VARIABLE_IN_TEXT.PREFIX.Length;
				while (parsedFileContent[i] != Constant.VARIABLE_IN_TEXT.POSTFIX)
				{
					variableInTextName += parsedFileContent[i];
					++i;
				}
				int variableInTextNameIndex = state.GetElementIndexByName(variableInTextName);
				if (variableInTextNameIndex == -1)
				{
					Debug.Log(Constant.ERROR.XML.CHOICE.STATE_ELEMENT_NOT_FOUND + ": " + variableInTextName);
					Application.Quit();
				}
				parsedFileContent = parsedFileContent.Replace(
					Constant.VARIABLE_IN_TEXT.PREFIX + variableInTextName + Constant.VARIABLE_IN_TEXT.POSTFIX,
					state[variableInTextNameIndex].Value.value.ToString()
				);
			}
		}

		private void LoadScreenChoices(ref XmlTextReader xmlTextReader)
		{
			DestroyChildren(choicesPanel.transform);
			if (!xmlTextReader.ReadToFollowing(Constant.SCREEN.CHOICES.TAG_NAME))
			{
				Debug.Log(Constant.ERROR.XML.SCREEN.NO_CHOICES_ELEMENT);
				Application.Quit();
			}
			String choicesFileName = xmlTextReader.GetAttribute(Constant.SCREEN.CHOICES.ATTRIBUTE.LOAD_FROM);
			XmlTextReader xmlTextReaderChoices = null;
			ParseFileToXML(Constant.SCREEN.CHOICES.PATH + choicesFileName, ref xmlTextReaderChoices);

			statesChanging = new Dictionary<String, Dictionary<String, object>>();
			float choiceHeight = choicePrefab.GetComponent<RectTransform>().sizeDelta.y;
			int currentChoice = 0;
			while (xmlTextReaderChoices.ReadToFollowing(Constant.SCREEN.CHOICES.CHOICE.TAG_NAME))
			{
				Transform newChoiceTransform = Instantiate(
						choicePrefab,
						new Vector2(
							choicesPanel.transform.position.x,
							choicesPanel.transform.position.y - currentChoice * (choiceHeight + Constant.TEXT_MARGIN_TOP)
						),
						Quaternion.identity,
						choicesPanel.transform
				);
				Button newChoiceButton = newChoiceTransform.GetComponent<Button>();
				newChoiceButton.onClick.AddListener(() => OnChoiceClicked());
				Dictionary<String, object> stateChanging = new Dictionary<String, object>();
				for (int i = 0; i < xmlTextReaderChoices.AttributeCount; ++i)
				{
					xmlTextReaderChoices.MoveToAttribute(i);
					switch (xmlTextReaderChoices.Name)
					{
						case Constant.SCREEN.CHOICES.CHOICE.ATTRIBUTE.TO:
							newChoiceTransform.name = currentChoice + "_" + xmlTextReaderChoices.Value;
							break;
						case Constant.SCREEN.CHOICES.CHOICE.ATTRIBUTE.TO_IF:
							String[] ifStatementParts = xmlTextReaderChoices.Value.Split(
								new Char[] { Constant.THEN, Constant.ELSE }
							);
							if (ifStatementParts.Length != 3)
							{
								Debug.Log(Constant.ERROR.XML.CHOICE.WRONG_TO_IF_SYNTAX);
								Application.Quit();
							}
							newChoiceTransform.name = currentChoice + "_" + 
								(EvaluateExpression(ifStatementParts[0]) ? ifStatementParts[1] : ifStatementParts[2]);
							break;
						case Constant.SCREEN.CHOICES.CHOICE.ATTRIBUTE.DISABLE_IF:
							if (EvaluateExpression(xmlTextReaderChoices.Value))
							{
								newChoiceButton.interactable = false;
							}
							break;
						case Constant.SCREEN.CHOICES.CHOICE.ATTRIBUTE.HIDE_IF:
							if (EvaluateExpression(xmlTextReaderChoices.Value))
							{
								Destroy(newChoiceTransform.gameObject);
								--currentChoice;
							}
							break;
						default:
							if (!stateChanging.ContainsKey(xmlTextReaderChoices.Name))
							{
								stateChanging.Add(xmlTextReaderChoices.Name, xmlTextReaderChoices.Value);
							}
							break;
					}
				}
				if (!statesChanging.ContainsKey(newChoiceTransform.name))
				{
					statesChanging.Add(newChoiceTransform.name, stateChanging);
				}
				xmlTextReaderChoices.MoveToContent();
				newChoiceTransform.GetComponentInChildren<Text>().text = xmlTextReaderChoices.ReadInnerXml();
				++currentChoice;
			}
		}
		private void LoadScreenImage(ref XmlTextReader xmlTextReader)
		{
			if (!xmlTextReader.ReadToFollowing(Constant.SCREEN.IMAGE.TAG_NAME))
			{
				Debug.Log(Constant.ERROR.XML.SCREEN.NO_IMAGE_ELEMENT);
				Application.Quit();
			}
			String imageScreenPath = "";
			for (int i = 0; i < xmlTextReader.AttributeCount; ++i)
			{
				xmlTextReader.MoveToAttribute(i);
				switch (xmlTextReader.Name)
				{
					case Constant.SCREEN.IMAGE.ATTRIBUTE.LOAD_FROM:
						imageScreenPath = xmlTextReader.Value;
						break;
				}
			}
			Texture2D imageScreenTexture = LoadPNG(imageScreenPath);
			Sprite imageScreenSprite = new Sprite();
			imageScreenSprite = Sprite.Create(
				imageScreenTexture, 
				new Rect(0, 0, imageScreenTexture.width, imageScreenTexture.height), 
				new Vector2(0, 0), 
				1
			);

			imageScreen.sprite = imageScreenSprite;
		}

		private Texture2D LoadPNG(string filePath)
		{
			Texture2D texture = null;
			byte[] fileData;

			if (!File.Exists(Constant.SCREEN.IMAGES_PATH + filePath))
			{
				Debug.Log(Constant.ERROR.XML.SCREEN.IMAGE_NOT_FOUND + ": " + Constant.SCREEN.IMAGES_PATH + filePath);
				Application.Quit();
			}

			fileData = File.ReadAllBytes(Constant.SCREEN.IMAGES_PATH + filePath);
			texture = new Texture2D(2, 2);
			texture.LoadImage(fileData);
			return texture;
		}

		private bool EvaluateExpression(String expressionStr)
		{
			String[] subOrExpressionsStr = expressionStr.Split(
				new String[] { Constant.OR },
				StringSplitOptions.None
			);
			bool expressionTruth = false;
			foreach (String subOrExpressionStr in subOrExpressionsStr)
			{
				expressionTruth = expressionTruth || EvaluateAndExpression(subOrExpressionStr);
			}
			return expressionTruth;
		}

		private bool EvaluateAndExpression(String expressionStr)
		{
			String[] subExpressionsStr = expressionStr.Split(
				new String[] { Constant.AND },
				StringSplitOptions.None
			);
			foreach (String subExpressionStr in subExpressionsStr)
			{
				String[] operands = subExpressionStr.Split(
					new String[] {
						Constant.EQUAL_SYMBOL,
						Constant.NOT_EQUAL_SYMBOL,
						Constant.MORE_SYMBOL,
						Constant.LESS_SYMBOL
					},
					StringSplitOptions.None
				);
				String operand0 = "";
				int indexToReferencedStateElementName = -1;
				if (operands[0][0] == Constant.REFERENCE_PREFIX_SYMBOL)
				{
					indexToReferencedStateElementName = state.GetElementIndexByName(operands[0].Substring(1));
					if (indexToReferencedStateElementName == -1)
					{
						Debug.Log(Constant.ERROR.XML.CHOICE.STATE_ELEMENT_NOT_FOUND + ": " + operands[0].Substring(1));
						Application.Quit();
					}
				}
				int stateElementIndex =
					(indexToReferencedStateElementName != -1) ?
					state.GetElementIndexByName(state[indexToReferencedStateElementName].Value.value.ToString()) :
					state.GetElementIndexByName(operands[0]);
				operand0 = (stateElementIndex != -1) ? state[stateElementIndex].Value.value.ToString() : operands[0];
				String operand1 = "";
				indexToReferencedStateElementName = -1;
				if (operands[1][0] == Constant.REFERENCE_PREFIX_SYMBOL)
				{
					indexToReferencedStateElementName = state.GetElementIndexByName(operands[1].Substring(1));
					if (indexToReferencedStateElementName == -1)
					{
						Debug.Log(Constant.ERROR.XML.CHOICE.STATE_ELEMENT_NOT_FOUND + ": " + operands[1].Substring(1));
						Application.Quit();
					}
				}
				stateElementIndex =
					(indexToReferencedStateElementName != -1) ?
					state.GetElementIndexByName(state[indexToReferencedStateElementName].Value.value.ToString()) :
					state.GetElementIndexByName(operands[1]);
				operand1 = (stateElementIndex != -1) ? state[stateElementIndex].Value.value.ToString() : operands[1];

				if (!Expression.GetCompareFunction(subExpressionStr)(operand0, operand1))
				{
					return false;
				}
			}
			return true;
		}

		public void OnChoiceClicked()
		{
			String nextScreen = EventSystem.current.currentSelectedGameObject.name;
			foreach (KeyValuePair<String, object> stateChanging in statesChanging[nextScreen])
			{
				int oldValue = 0;
				int updateValue = 0;

				int indexToStateElementNameToChange = -1;
				if (stateChanging.Key[0] == Constant.REFERENCE_PREFIX_SYMBOL)
				{
					indexToStateElementNameToChange = state.GetElementIndexByName(stateChanging.Key.Substring(1));
					if (indexToStateElementNameToChange == -1)
					{
						Debug.Log(Constant.ERROR.XML.CHOICE.STATE_ELEMENT_NOT_FOUND + ": " + stateChanging.Key.Substring(1));
						Application.Quit();
					}
				}
				if (stateChanging.Key[0] == Constant.SHOW_PREFIX_SYMBOL || stateChanging.Key[0] == Constant.HIDE_PREFIX_SYMBOL)
				{
					int stateElementIndexToShow = state.GetElementIndexByName(stateChanging.Key.Substring(1));
					if (stateElementIndexToShow == -1)
					{
						Debug.Log(Constant.ERROR.XML.CHOICE.STATE_ELEMENT_NOT_FOUND + ": " + stateChanging.Key);
						Application.Quit();
					}
					state[stateElementIndexToShow].Value.visible = stateChanging.Key[0] == Constant.SHOW_PREFIX_SYMBOL;
					continue;
				}
				int stateElementIndexToChange = 
					(indexToStateElementNameToChange != -1) ? 
					state.GetElementIndexByName(state[indexToStateElementNameToChange].Value.value.ToString()) : 
					state.GetElementIndexByName(stateChanging.Key);
				if (stateElementIndexToChange == -1)
				{
					Debug.Log(Constant.ERROR.XML.CHOICE.STATE_ELEMENT_NOT_FOUND + ": " + stateChanging.Key);
					Application.Quit();
				}
				if (Int32.TryParse(state[stateElementIndexToChange].Value.value.ToString(), out oldValue) &&
					(
						stateChanging.Value.ToString()[0] == Constant.PLUS_SYMBOL ||
						stateChanging.Value.ToString()[0] == Constant.MINUS_SYMBOL
					) &&
					Int32.TryParse(stateChanging.Value.ToString(), out updateValue))
				{
					state[stateElementIndexToChange].Value.value = oldValue + updateValue;
				}
				else
				{ 
					state[stateElementIndexToChange].Value.value = stateChanging.Value;
				}
			}
			LoadScreen(nextScreen.Split('_')[1]);
		}

		private void DestroyChildren(Transform root)
		{
			int childCount = root.childCount;
			for (int i = 0; i < childCount; ++i)
			{
				GameObject.Destroy(root.GetChild(i).gameObject);
			}
		}

		private String GetFileContent(String fileName)
		{
			try
			{
				using (StreamReader sr = new StreamReader(Constant.SCREEN.PATH + fileName))
				{
					return sr.ReadToEnd();
				}
			}
			catch (Exception e)
			{
				Debug.Log(Constant.ERROR.CANNOT_READ_FILE + e.Message);
				Application.Quit();
			}
			return "";
		}
		private void ParseFileToXML(String fileName, ref XmlTextReader xmlTextReader)
		{
			try
			{
				xmlTextReader = new XmlTextReader(fileName);
			}
			catch (Exception e)
			{
				Debug.Log(Constant.ERROR.XML.CANNOT_PARSE_FILE + e.Message);
				Application.Quit();
			}
		}

		public Text contentText;
		public GameObject choicesPanel;
		public Transform choicePrefab;
		public GameObject statePanel;
		public Transform stateElementPrefab;
		public Image imageScreen;
		public ScrollRect scrollRect;

		private State state;
		private Dictionary<String, Dictionary<String, object>> statesChanging;
	}
}