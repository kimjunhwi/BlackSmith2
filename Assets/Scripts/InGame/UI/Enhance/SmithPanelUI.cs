using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadOnlys;

public class SmithPanelUI : EnhanceUI {

	private float fBasic = 1;

	const int nBasicHonor = 100;
	const int nPlusPercent = 50;

	protected override void Awake ()
	{
		base.Awake ();

		nLevel = cPlayer.GetSmithLevel ();

		EnhanceText.text = string.Format("{0} : {1}", strEnhanceName ,nLevel);

		fCostHonor = nBasicHonor + (nPlusPercent * nLevel - 1);

		CostGoldText.text = ChangeValue(fCostHonor);
	}

	protected override void EnhanceButtonClick ()
	{
		fCostHonor = nBasicHonor + (nPlusPercent * nLevel - 1);

		if (fCostHonor <= ScoreManager.ScoreInstance.GetHonor ()) {

			nLevel++;

			SpawnManager.Instance.ArbaitScoutCount ();

			cPlayer.SetSmithLevel (nLevel);

			fEnhanceValue = cPlayer.GetBasicBigSuccessedPercent() + (fBasic * 0.05f);

			cPlayer.SetBasicBigSuccessedPercent(fEnhanceValue);

			EnhanceText.text =string.Format("{0} : {1}", strEnhanceName , nLevel);

			ScoreManager.ScoreInstance.HonorPlus (-fCostHonor);

			CostGoldText.text  = ChangeValue((float)(nBasicHonor + (nPlusPercent * nLevel - 1)));
		}
	}
}
