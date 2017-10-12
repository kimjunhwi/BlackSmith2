using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeverTimeUI : EnhanceUI{

	const int nMaxLevel = 100;

	private float fBasic = 10;

	const int nBasicHonor = 100;
	const int nPlusPercent = 50;

	protected override void Awake ()
	{
		base.Awake ();

		nLevel = cPlayer.GetFeverLevel ();

		if (cPlayer.GetFeverLevel () < nMaxLevel) {

			EnhanceText.text = string.Format("{0} : {1}", strEnhanceName ,nLevel);

			fCostHonor = nBasicHonor + (nPlusPercent * (nLevel - 1));

			CostGoldText.text = ChangeValue(fCostHonor);

			NextPercentText.text = (cPlayer.GetBasicFeverTime() + (fBasic * 0.05f)).ToString();
		} else {
			EnhanceText.text = string.Format("{0} : {1}", strEnhanceName ,"Max");

			CostGoldText.text = "Max";

			NextPercentText.text = "Max";
		}


	}

	protected override void EnhanceButtonClick ()
	{
		fCostHonor = nBasicHonor + (nPlusPercent * (nLevel - 1));

		if (fCostHonor <= ScoreManager.ScoreInstance.GetHonor () && cPlayer.GetFeverLevel() < nMaxLevel) 
		{
			
			nLevel++;

			cPlayer.SetFeverLevel (nLevel);

			fEnhanceValue = cPlayer.GetBasicFeverTime() + (fBasic * 0.05f);

			cPlayer.SetBasicFeverTime(fEnhanceValue);

			EnhanceText.text =string.Format("{0} {1}", strEnhanceName , nLevel);

			NextPercentText.text = (cPlayer.GetBasicFeverTime() + (fBasic * 0.05f)).ToString();

			ScoreManager.ScoreInstance.HonorPlus (-fCostHonor);

			CostGoldText.text  = ChangeValue((float)(nBasicHonor + (nPlusPercent * (nLevel-1))));

			if (cPlayer.GetFeverLevel () < nMaxLevel) {
				CostGoldText.text = ChangeValue ((float)(nBasicHonor + (nPlusPercent * (nLevel - 1))));

				EnhanceText.text = string.Format ("{0} {1}", strEnhanceName, nLevel);
			} else {
				CostGoldText.text = "Max";
				EnhanceText.text =string.Format("{0} {1}", strEnhanceName , "Max");
				NextPercentText.text = "Max";
			}
		}
	}
}
