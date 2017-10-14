using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSpecificInfo : MonoBehaviour {

    Player player;

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

    void Awake()
    {
		player = GameManager.Instance.GetPlayer();

		player.SetPlayerInfo (this);

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
			strMinusRepair = _weapon.dMinusRepair.ToString("N1");
			strCriticalDamage = _weapon.dMinusCriticalDamage.ToString("N1"); 
			strCriticalChance = _weapon.fMinusCriticalChance.ToString("N1");
			strAccuracyRatePercent = _weapon.fMinusAccuracy.ToString("N1");
			strWaterCharge = _weapon.fMinusChargingWater.ToString("N1");
		}
		else
		{
			strMinusRepair = "-0";
			strCriticalDamage = " -0"; 
			strCriticalChance = "-0";
			strAccuracyRatePercent = "-0";
			strWaterCharge = "-0";
		}

		ReSetting ();
	}

	public void SetRepairpowerText()
	{
		RepairPowerText.text = string.Format("수리력 : {0} {1}%", ScoreManager.ScoreInstance.ChangeMoney(player.GetRepairPower()),strMinusRepair);
	}

	public void SetArbaitRepairText()
	{
		ArbaitRepairIncreaseText.text = string.Format("아르바이트 수리력 : +{0} ", player.GetArbaitRepairPower ().ToString ("N1"));
	}

	public void SetCriticalDamamgeText()
	{
		CriticalDamageText.text = string.Format("크리데미지 : {0}% {1}%", player.GetCriticalDamage().ToString ("N1"),strCriticalDamage);
	}

	public void SetCriticalChance()
	{
		CriticalChanceText.text = string.Format("크리 확률 : {0}% {1}%", player.GetCriticalChance().ToString ("N1"),strCriticalChance);
	}

	public void SetGoldPlusText()
	{
		GoldPlusText.text = string.Format("골드 추가 증가량 : {0}%", player.GetGoldPlusPercent().ToString ("N1"));
	}

	public void SetHonorPlusText()
	{
		HonorPlusText.text = string.Format("명예 추가 증가량 : {0}%", player.GetHonorPlusPercent().ToString ("N1"));
	}

	public void SetBigSuccessedText()
	{
		BigSuccessesChanceText.text = string.Format("대성공 확률 : {0}%", player.GetBigSuccessed().ToString ("N1"));
	}

	public void SetAccuracyRateText()
	{
		AccuracyRateText.text = string.Format("명중률 : {0} {1}%", player.GetAccuracyRate().ToString ("N1"),strAccuracyRatePercent);
	}

	public void SetWaterMaxText()
	{
		WaterMaxText.text = string.Format("최대 물 충전량 : {0}", player.GetBasicMaxWaterPlus().ToString ());
	}

	public void SetWaterCharginText()
	{
		WaterRechargeAmountText.text = string.Format("물 충전량 : {0} {1}%", player.GetWaterPlus().ToString ("N1"),strWaterCharge);
	}

	public void SetFeverTimeText()
	{
		FeverTimeText.text = string.Format("피버 시간  : {0}초", player.GetFeverTime().ToString ("N1"));
	}
}
