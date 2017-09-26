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

		EnhanceText.text =string.Format("{0} {1}", strEnhanceName , nLevel);

		CostGoldText.text = ChangeValue(800 * Mathf.Pow (1.095f, nLevel - 1));

		NextPercentText.text = (cPlayer.GetBasicWaterPlus() + (fBasic * 0.01f)).ToString();
	}

	protected override void EnhanceButtonClick ()
	{
		fCostGold = 800 * Mathf.Pow (1.095f, nLevel - 1);

		if (fCostGold <= ScoreManager.ScoreInstance.GetGold ()) {

			nLevel++;

			cPlayer.SetWaterPlusLevel (nLevel);

			fEnhanceValue = cPlayer.GetBasicWaterPlus() + (fBasic * 0.01f);

			cPlayer.SetBasicWaterPlus(fEnhanceValue);

			NextPercentText.text = (cPlayer.GetBasicWaterPlus() + (fBasic * 0.01f)).ToString();

			EnhanceText.text =string.Format("{0} {1}", strEnhanceName , nLevel);

			ScoreManager.ScoreInstance.GoldPlus (-fCostGold	);

			CostGoldText.text  = ChangeValue(800 * Mathf.Pow (1.095f, nLevel - 1));
		}
	}
}
