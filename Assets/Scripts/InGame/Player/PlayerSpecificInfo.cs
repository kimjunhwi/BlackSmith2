using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSpecificInfo : MonoBehaviour {

    Player player;

	public Text PlayerNameText;
    public Text RepairPowerText;
    public Text ArbaitRepairIncreaseText;
    public Text CriticalDamageText;
    public Text CriticalChanceText;
	public Text GoldPlusText;
	public Text HonorPlusText;
    public Text BigSuccessesChanceText;
    public Text AccuracyRateText;
    public Text WaterMaxText;
    public Text WaterRechargeAmountText;
	public Text FeverTimeText;

	string strMinusRepair = "0";
	string strCriticalDamage = "0"; 
	string strCriticalChance = "0";
	string strWaterCharge = "0";
	string strAccuracyRatePercent = "0";

	string[] unit = new string[]{ "G", "K", "M", "B", "T", "aa", "bb", "cc", "dd", "ee" }; 

    void Awake()
    {
        player = GameManager.Instance.player;

		player.SetPlayerInfo (this);

		//PlayerNameText.text = player.changeStats.strName + "의 대장간";

		gameObject.SetActive (false);

    }

	public void OnEnable()
    {
        if(player == null)
			player = GameManager.Instance.GetPlayer();

		ReSetting ();
    }

	public void ReSetting()
	{
		SetRepairpowerText ();
		SetArbaitRepairText ();

		SetCriticalDamamgeText ();
		SetCriticalChance ();

		SetGoldPlusText ();
		SetHonorPlusText ();

		SetBigSuccessedText ();
		SetAccuracyRateText ();

		SetWaterMaxText ();
		SetWaterCharginText ();

		SetFeverTimeText ();
	}

	public void SetGuestWeapon(CGameWeaponInfo _weapon)
	{
		if (_weapon != null) {
			strMinusRepair = _weapon.dMinusRepair.ToString("N2");
			strCriticalDamage = _weapon.dMinusCriticalDamage.ToString("N2"); 
			strCriticalChance = _weapon.fMinusCriticalChance.ToString("N2");
			strAccuracyRatePercent = _weapon.fMinusAccuracy.ToString("N2");
			strWaterCharge = _weapon.fMinusChargingWater.ToString("N2");
		}
		else
		{
			strMinusRepair = "0";
			strCriticalDamage = "0"; 
			strCriticalChance = "0";
			strAccuracyRatePercent = "0";
			strWaterCharge = "0";
		}
	}

	public void SetRepairpowerText()
	{
		RepairPowerText.text = string.Format("수리력 : {0} {1}%", ChangeValue(player.GetRepairPower()),strMinusRepair);
	}

	public void SetArbaitRepairText()
	{
		ArbaitRepairIncreaseText.text = string.Format("아르바이트 수리력 : {0} ", player.GetArbaitRepairPower ().ToString ("N2"));
	}

	public void SetCriticalDamamgeText()
	{
		CriticalDamageText.text = string.Format("크리데미지 : {0}% {1}%", player.GetCriticalDamage().ToString ("N2"),strCriticalDamage);
	}

	public void SetCriticalChance()
	{
		CriticalChanceText.text = string.Format("크리 확률 : {0}% {1}%", player.GetCriticalChance().ToString ("N2"),strCriticalChance);
	}

	public void SetGoldPlusText()
	{
		GoldPlusText.text = string.Format("골드 추가 증가량 : {0}%", player.GetGoldPlusPercent().ToString ("N2"));
	}

	public void SetHonorPlusText()
	{
		HonorPlusText.text = string.Format("명예 추가 증가량 : {0}%", player.GetHonorPlusPercent().ToString ("N2"));
	}

	public void SetBigSuccessedText()
	{
		BigSuccessesChanceText.text = string.Format("대성공 확률 : {0}%", player.GetBigSuccessed().ToString ("N2"));
	}

	public void SetAccuracyRateText()
	{
		AccuracyRateText.text = string.Format("명중률 : {0} {1}%", player.GetAccuracyRate().ToString ("N2"),strAccuracyRatePercent);
	}

	public void SetWaterMaxText()
	{
		WaterMaxText.text = string.Format("최대 물 충전량 : {0}", player.GetBasicMaxWaterPlus().ToString ());
	}

	public void SetWaterCharginText()
	{
		WaterRechargeAmountText.text = string.Format("물 충전량 : {0} {1}%", player.GetWaterPlus().ToString ("N2"),strWaterCharge);
	}

	public void SetFeverTimeText()
	{
		FeverTimeText.text = string.Format("피버 시간  : {0}초", player.GetFeverTime().ToString ("N2"));
	}


	//값을 수치로 표기하기 위한 함수 
	public string ChangeValue(double _dValue)
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
			} else if (_dValue >= 10) 
			{
				int nResult = cVal [index] * 1000 + cVal [index - 1]; 

				string strFirstValue = nResult.ToString ().Substring (0, 2);

				string strSecondValue = nResult.ToString ().Substring (2, 2);

				return string.Format ("{0}.{1:##}{2}", strFirstValue, strSecondValue, unit [index]); 
			} else 
			{
				int nResult = cVal [index] * 1000 + cVal [index - 1]; 

				string strFirstValue = nResult.ToString ().Substring (0, 1);

				string strSecondValue = nResult.ToString ().Substring (1, 2);

				return string.Format ("{0}.{1:##}{2}", strFirstValue, strSecondValue, unit [index]); 
			}
		} 

		return strValue; 
	}
}
