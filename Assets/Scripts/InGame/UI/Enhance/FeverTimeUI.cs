using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeverTimeUI : EnhanceUI{

	private float fBasic = 10;

	const int nBasicHonor = 100;
	const int nPlusPercent = 50;

	protected override void Awake ()
	{
		base.Awake ();

		nLevel = cPlayer.GetFeverLevel ();

		EnhanceText.text = string.Format("{0} {1}", strEnhanceName ,nLevel);

		fCostHonor = nBasicHonor + (nPlusPercent * nLevel - 1);

		CostGoldText.text = ChangeValue(fCostHonor);
	}

	protected override void EnhanceButtonClick ()
	{
		fCostHonor = nBasicHonor + (nPlusPercent * nLevel - 1);

		if (fCostHonor <= ScoreManager.ScoreInstance.GetHonor ()) {

			nLevel++;

			cPlayer.SetFeverLevel (nLevel);

			fEnhanceValue = cPlayer.GetBasicFeverTime() + (fBasic * 0.05f);

			cPlayer.SetBasicFeverTime(fEnhanceValue);

			EnhanceText.text =string.Format("{0} {1}", strEnhanceName , nLevel);

			ScoreManager.ScoreInstance.HonorPlus (-fCostHonor);

			CostGoldText.text  = ChangeValue((float)(nBasicHonor + (nPlusPercent * nLevel-1)));
		}
	}
}
