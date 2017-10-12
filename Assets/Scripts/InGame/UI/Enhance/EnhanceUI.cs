using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ReadOnlys;

public class EnhanceUI : MonoBehaviour {

	public int nLevel = 0;
	public double fCostGold = 0.0f;
	public double fCostHonor = 0.0f;

	public double fEnhanceValue = 0.0f;
	public double fNextEnhanceValue = 0.0f;

	public string strEnhanceName = null;

	public Text EnhanceText;
	public Text CostGoldText;
	public Image EnhanceImage;
	public Button EnhanceButton;

	public Player cPlayer;
	public RepairObject repairObject;

	public Text NextPercentText;

	string[] unit = new string[]{ "G", "K", "M", "B", "T", "aa", "bb", "cc", "dd", "ee","ff","gg","hh","ii","jj","kk","ll","mm","nn","oo","pp","qq","rr","ss","tt","uu","vv","ww","xx","yy","zz","aaa", "bbb", "ccc", "ddd", "eee","fff","ggg","hhh","iii","jjj","kkk","lll","mmm","nnn","ooo","ppp","qqq","rrr","sss","ttt","uuu","vvv","www","xxx","yyy","zzz" };
	 

	protected virtual void Awake()
	{
		cPlayer = GameManager.Instance.GetPlayer();

		EnhanceButton.onClick.AddListener (EnhanceButtonClick);
	}

	protected virtual void EnhanceButtonClick() { }

	//값을 수치로 표기하기 위한 함수 
	public string ChangeValue(double _dValue)
	{ 
		long[] cVal = new long[100]; 

		int index = 0; 

		string strValue =  string.Format ("{0:####}", _dValue);

		if (_dValue < 10000)
			return strValue;

		while (true) { 
			string last4 = ""; 
			if (strValue.Length >= 4) { 

				last4 = strValue.Substring (strValue.Length - 4); 

				long intLast4 = long.Parse (last4); 

				cVal [index] = intLast4 % 1000; 

				strValue = strValue.Remove (strValue.Length - 3); 
			} else { 
				cVal [index] = long.Parse (strValue); 
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
				long nResult = cVal [index] * 1000 + cVal [index - 1]; 

				string strFirstValue = nResult.ToString ().Substring (0, 3);

				string strSecondValue = nResult.ToString ().Substring (3, 1);

				return string.Format ("{0}.{1:##}{2}", strFirstValue, strSecondValue, unit [index]); 
			} else if (_dValue >= 10) 
			{
				long nResult = cVal [index] * 1000 + cVal [index - 1]; 

				string strFirstValue = nResult.ToString ().Substring (0, 2);

				string strSecondValue = nResult.ToString ().Substring (2, 2);

				return string.Format ("{0}.{1:##}{2}", strFirstValue, strSecondValue, unit [index]); 
			} else 
			{
				long nResult = cVal [index] * 1000 + cVal [index - 1]; 

				string strFirstValue = nResult.ToString ().Substring (0, 1);

				string strSecondValue = nResult.ToString ().Substring (1, 2);

				return string.Format ("{0}.{1:##}{2}", strFirstValue, strSecondValue, unit [index]); 
			}
		} 

		return strValue; 
	}
}
