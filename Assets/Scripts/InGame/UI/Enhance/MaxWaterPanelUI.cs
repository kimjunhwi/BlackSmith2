using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadOnlys;

public class MaxWaterPanelUI : EnhanceUI {
	

	public float fBasic = 2000.0f;

	const int nMaxLevel = 6;

	const int nBasicHonor = 100;

	protected override void Awake ()
	{
		base.Awake ();

		nLevel = cPlayer.GetMaxWaterLevel ();

		if (cPlayer.GetMaxWaterLevel () < nMaxLevel) {

			EnhanceText.text = string.Format("{0} : {1}", strEnhanceName ,nLevel);

			fCostHonor = nBasicHonor * nLevel * 3;

			CostGoldText.text = fCostHonor.ToString();

			NextPercentText.text = (cPlayer.GetBasicMaxWater () + 1000).ToString();
		} else {
			EnhanceText.text = string.Format("{0} : {1}", strEnhanceName ,"Max");

			CostGoldText.text = "Max";

			NextPercentText.text = "Max";
		}
	}

	protected override void EnhanceButtonClick ()
	{
		fCostHonor = nBasicHonor * nLevel * 3;

		if (fCostHonor <= ScoreManager.ScoreInstance.GetHonor () && cPlayer.GetMaxWaterLevel() <  nMaxLevel) 
		{
			nLevel++;

			cPlayer.SetMaxWaterLevel (nLevel);

			fEnhanceValue = cPlayer.GetBasicMaxWater() + (fBasic * 0.5f);

			cPlayer.SetBasicMaxWater((float)fEnhanceValue);

			repairObject.fMaxWater = (float)fEnhanceValue;

			EnhanceText.text =string.Format("{0} : {1}", strEnhanceName , nLevel);

			NextPercentText.text = (cPlayer.GetBasicMaxWater () + 1000).ToString();

			ScoreManager.ScoreInstance.HonorPlus (-fCostHonor);

			CostGoldText.text  = (nBasicHonor * nLevel * 3).ToString();

			repairObject.WaterSlider.maxValue = cPlayer.GetBasicMaxWater ();

			if (cPlayer.GetMaxWaterLevel () < nMaxLevel) {
				EnhanceText.text = string.Format("{0} : {1}", strEnhanceName ,nLevel);

				fCostHonor = nBasicHonor * nLevel * 3;

				CostGoldText.text = fCostHonor.ToString();

				NextPercentText.text = (cPlayer.GetBasicMaxWater () + 1000).ToString();
			} else {
				CostGoldText.text = "Max";
				EnhanceText.text =string.Format("{0} {1}", strEnhanceName , "Max");
				NextPercentText.text = "Max";
			}
		}
	}
}
