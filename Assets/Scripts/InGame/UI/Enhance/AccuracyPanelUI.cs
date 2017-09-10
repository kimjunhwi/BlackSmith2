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

		EnhanceText.text =string.Format("{0} : {1}", strEnhanceName , nLevel);

		CostGoldText.text = ChangeValue(2000 * Mathf.Pow (1.1f, Mathf.Min (nLevel -1, 100)) * Mathf.Pow (1.06f, Mathf.Max (nLevel - 101, 0)));
	}

	protected override void EnhanceButtonClick ()
	{
		fCostGold = 2000 * Mathf.Pow (1.1f, Mathf.Min (nLevel - 1, 100)) * Mathf.Pow (1.06f, Mathf.Max (nLevel - 101, 0));

		if (fCostGold <= ScoreManager.ScoreInstance.GetGold ()) {

			nLevel++;

			cPlayer.SetAccuracyRateLevel (nLevel);

			fEnhanceValue = cPlayer.GetBasicAccuracyRate() + (fBasic * 0.01f);

			cPlayer.SetBasicAccuracyRate(fEnhanceValue);

			EnhanceText.text =string.Format("{0} : {1}", strEnhanceName , nLevel);

			ScoreManager.ScoreInstance.GoldPlus (-fCostGold	);

			CostGoldText.text  = ChangeValue(2000 * Mathf.Pow (1.1f, Mathf.Min (nLevel - 1, 100)) * Mathf.Pow (1.06f, Mathf.Max (nLevel - 101, 0)));
		}
	}
}
