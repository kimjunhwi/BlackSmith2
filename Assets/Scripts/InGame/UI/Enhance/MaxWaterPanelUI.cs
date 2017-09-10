using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadOnlys;

public class MaxWaterPanelUI : EnhanceUI {
	
	public float fBasic = 2000.0f;

	const int nMaxLevel = 5;

	const int nBasicHonor = 100;

	protected override void Awake ()
	{
		base.Awake ();

		nLevel = cPlayer.GetMaxWaterLevel ();

		EnhanceText.text = string.Format("{0} : {1}", strEnhanceName , nLevel);

		fCostHonor = nBasicHonor * nLevel;

		CostGoldText.text = fCostHonor.ToString();
	}

	protected override void EnhanceButtonClick ()
	{
		fCostHonor = nBasicHonor * nLevel;

		if (fCostHonor <= ScoreManager.ScoreInstance.GetHonor () && cPlayer.GetMaxWaterLevel() <  nMaxLevel) {

			nLevel++;

			cPlayer.SetMaxWaterLevel (nLevel);

			fEnhanceValue = cPlayer.GetBasicMaxWater() + (fBasic * 0.5f);

			cPlayer.SetBasicMaxWater(fEnhanceValue);

			EnhanceText.text =string.Format("{0} : {1}", strEnhanceName , nLevel);

			ScoreManager.ScoreInstance.HonorPlus (-fCostHonor);

			CostGoldText.text  = (nBasicHonor * nLevel).ToString();
		}
	}
}
