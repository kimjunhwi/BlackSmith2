﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigSucceseedPanelUI : EnhanceUI {

	private float fBasic = 1;

	const int nBasicHonor = 100;
	const int nPlusPercent = 50;

	protected override void Awake ()
	{
		base.Awake ();

		nLevel = cPlayer.GetBigSuccessedLevel ();

		EnhanceText.text = string.Format("{0} {1}", strEnhanceName ,nLevel);

		fCostHonor = nBasicHonor + (nPlusPercent * (nLevel - 1));

		CostGoldText.text = ChangeValue(fCostHonor);

		NextPercentText.text = (cPlayer.GetBasicBigSuccessedPercent() + (fBasic * 0.05f)).ToString();
	}

	protected override void EnhanceButtonClick ()
	{
		fCostHonor = nBasicHonor + (nPlusPercent * (nLevel - 1));

		if (fCostHonor <= ScoreManager.ScoreInstance.GetHonor ()) {

			nLevel++;

			cPlayer.SetBigSuccessedLevel (nLevel);

			fEnhanceValue = cPlayer.GetBasicBigSuccessedPercent() + (fBasic * 0.05f);

			cPlayer.SetBasicBigSuccessedPercent((float)fEnhanceValue);

			EnhanceText.text =string.Format("{0} {1}", strEnhanceName , nLevel);

			NextPercentText.text = (cPlayer.GetBasicBigSuccessedPercent() + (fBasic * 0.05f)).ToString();

			ScoreManager.ScoreInstance.HonorPlus (-fCostHonor);

			CostGoldText.text  = ChangeValue((float)(nBasicHonor + (nPlusPercent * (nLevel - 1))));
		}
	}
}
