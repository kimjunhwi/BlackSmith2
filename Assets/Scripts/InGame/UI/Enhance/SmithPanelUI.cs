using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadOnlys;

public class SmithPanelUI : EnhanceUI {

	const int nMaxLevel = 10;

	const int nBasicHonor = 300;
	const int nPlusPercent = 100;

	public GameObject UpgradeEffectObject;
	public HanabiTest Hanabi;

	protected override void Awake ()
	{
		base.Awake ();

		nLevel = cPlayer.GetSmithLevel ();

		if (cPlayer.GetSmithLevel () < nMaxLevel) {

			EnhanceText.text = string.Format("{0} : {1}", strEnhanceName ,nLevel);

			fCostHonor = nBasicHonor + (nPlusPercent * nLevel - 1);

			CostGoldText.text = ChangeValue(fCostHonor);
		} else {
			EnhanceText.text = string.Format("{0} : {1}", strEnhanceName ,"Max");

			CostGoldText.text = "Max";
		}

		NextPercentText.text = (cPlayer.GetBasicGoldPlusPercent() + 50).ToString();
	}

	public void OnEnable()
	{
//		fCostHonor;



	}

	protected override void EnhanceButtonClick ()
	{
		fCostHonor = nBasicHonor + (nPlusPercent * nLevel - 1);

		if (fCostHonor <= ScoreManager.ScoreInstance.GetHonor () && cPlayer.GetSmithLevel() < nMaxLevel) {

			double dEnhanceValue = 0;

			nLevel++;

			SpawnManager.Instance.ArbaitScoutCount ();

			SpawnManager.Instance.balckSmithSetting.SettingSmith (nLevel);

			cPlayer.SetSmithLevel (nLevel);

			SetShopMaxCount ();

			UpgradeEffectObject.SetActive (true);

			Hanabi.Play ();

			dEnhanceValue = cPlayer.GetBasicGoldPlusPercent() + 50;

			cPlayer.SetBasicGoldPlusPercent(dEnhanceValue);

			dEnhanceValue = cPlayer.GetBasicHonorGoldPlusPercent() + 50;

			cPlayer.SetBasicHonorPlusPercent (dEnhanceValue);

			EnhanceText.text =string.Format("{0} {1}", strEnhanceName , nLevel);

			NextPercentText.text = (cPlayer.GetBasicGoldPlusPercent() + 50).ToString();

			ScoreManager.ScoreInstance.HonorPlus (-fCostHonor);

			if (cPlayer.GetSmithLevel () < nMaxLevel) {
				CostGoldText.text = ChangeValue ((float)(nBasicHonor + (nPlusPercent * nLevel - 1)));
			
				EnhanceText.text = string.Format ("{0} {1}", strEnhanceName, nLevel);
			} else {
				CostGoldText.text = "Max";
				EnhanceText.text =string.Format("{0} {1}", strEnhanceName , "Max");
			}
		}
	}

	public void SetShopMaxCount()
	{
		int nDight = cPlayer.GetSmithLevel () % 3;

		if (nDight == 1)
			cPlayer.SetShopMaxCount (cPlayer.GetShopMaxCount () + 1);
	}
}
