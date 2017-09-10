using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadOnlys;

public class WaterPlusPanelUI : EnhanceUI {

	float fBasic = 150.0f;

	protected override void Awake ()
	{
		base.Awake ();

		nLevel = cPlayer.GetWaterPlusLevel ();

		EnhanceText.text =string.Format("{0} : {1}", strEnhanceName , nLevel);

		CostGoldText.text = ChangeValue(2000 * Mathf.Pow (1.1f, Mathf.Min (nLevel -1, 100)) * Mathf.Pow (1.06f, Mathf.Max (nLevel - 101, 0)));
	}

	protected override void EnhanceButtonClick ()
	{
		fCostGold = 2000 * Mathf.Pow (1.1f, Mathf.Min (nLevel - 1, 100)) * Mathf.Pow (1.06f, Mathf.Max (nLevel - 101, 0));

		if (fCostGold <= ScoreManager.ScoreInstance.GetGold ()) {

			nLevel++;

			cPlayer.SetWaterPlusLevel (nLevel);

			fEnhanceValue = cPlayer.GetBasicWaterPlus() + (fBasic * 0.01f);

			cPlayer.SetBasicWaterPlus(fEnhanceValue);

			EnhanceText.text =string.Format("{0} : {1}", strEnhanceName , nLevel);

			ScoreManager.ScoreInstance.GoldPlus (-fCostGold	);

			CostGoldText.text  = ChangeValue(2000 * Mathf.Pow (1.1f, Mathf.Min (nLevel - 1, 100)) * Mathf.Pow (1.06f, Mathf.Max (nLevel - 101, 0)));
		}
	}
}
