using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ReadOnlys;

public class EnhanceUI : MonoBehaviour {

	public int nLevel = 0;
	public float fCostGold = 0.0f;
	public float fCostHonor = 0.0f;

	public float fEnhanceValue = 0.0f;

	public string strEnhanceName = null;

	public Text EnhanceText;
	public Text CostGoldText;
	public Image EnhanceImage;
	public Button EnhanceButton;

	public Player cPlayer;
	public RepairObject repairObject;

	string[] unit = new string[]{ "G", "K", "M", "B", "T", "aa", "bb", "cc", "dd", "ee" }; 

	protected virtual void Awake()
	{
		cPlayer = GameManager.Instance.player;

		EnhanceButton.onClick.AddListener (EnhanceButtonClick);
	}

	protected virtual void EnhanceButtonClick() { }

	//값을 수치로 표기하기 위한 함수 
	public string ChangeValue(float _dValue)
	{ 
		if (_dValue == 0)
			return "0";

		int[] cVal = new int[10]; 

		int index = 0; 

		string strValue =  string.Format ("{0:####}", _dValue);

		while (true) { 
			string last4 = ""; 
			if (strValue.Length >= 4) { 

				last4 = strValue.Substring (strValue.Length - 4); 

				int intLast4 = int.Parse (last4); 

				cVal [index] = intLast4 % 1000; 

				strValue = strValue.Remove (strValue.Length - 3); 
			} else { 
				cVal [index] = int.Parse (strValue); 
				break; 
			} 

			index++; 
		} 

		//1000,00
		//1000,000,00
		//1000,000,000,00

		while (_dValue >= 1000) 
		{
			_dValue *= 0.001f;
		}

		if (index > 0) { 

			if (_dValue >= 100) 
			{
				int nResult = cVal [index] * 1000 + cVal [index - 1]; 

				string strFirstValue = nResult.ToString ().Substring (0, 3);

				string strSecondValue = nResult.ToString ().Substring (3, 1);

				return string.Format ("{0}.{1:##}{2}", strFirstValue, strSecondValue, unit [index]); 
			} 
			else 
			{
				int nResult = cVal [index] * 1000 + cVal [index - 1]; 

				string strFirstValue = nResult.ToString ().Substring (0, 2);

				string strSecondValue = nResult.ToString ().Substring (2, 2);

				return string.Format ("{0}.{1:##}{2}", strFirstValue, strSecondValue, unit [index]); 

			}
		} 

		return strValue; 
	}
}
