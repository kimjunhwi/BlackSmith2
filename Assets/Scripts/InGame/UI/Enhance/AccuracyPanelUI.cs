using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadOnlys;

public class AccuracyPanelUI : EnhanceUI {

	float fBasic = 100.0f;

	protected override void Awake ()
	{
		base.Awake ();

		nLevel = cPlayer.GetAccuracyRateLevel ();

		EnhanceText.text =string.Format("{0} {1}", strEnhanceName , nLevel);

		CostGoldText.text = ChangeValue(800 * Mathf.Pow (1.095f, nLevel - 1));
	}

	protected override void EnhanceButtonClick ()
	{
		fCostGold = 800 * Mathf.Pow (1.095f, nLevel - 1);

		if (fCostGold <= ScoreManager.ScoreInstance.GetGold ()) {

			nLevel++;

			cPlayer.SetAccuracyRateLevel (nLevel);

			fEnhanceValue = cPlayer.GetBasicAccuracyRate() + (fBasic * 0.01f);

			cPlayer.SetBasicAccuracyRate(fEnhanceValue);

			EnhanceText.text =string.Format("{0} {1}", strEnhanceName , nLevel);

			ScoreManager.ScoreInstance.GoldPlus (-fCostGold	);

			CostGoldText.text  = ChangeValue(800 * Mathf.Pow (1.095f, nLevel - 1));
		}
	}
}
