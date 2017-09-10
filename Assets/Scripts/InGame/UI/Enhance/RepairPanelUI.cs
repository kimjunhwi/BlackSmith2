using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadOnlys;

public class RepairPanelUI : EnhanceUI {


	protected override void Awake ()
	{
		base.Awake ();

		nLevel = cPlayer.GetRepairLevel ();

		EnhanceText.text =string.Format("{0} : {1}", strEnhanceName , nLevel);

		CostGoldText.text = ChangeValue(3000 * Mathf.Pow (1.1f, Mathf.Min (nLevel -1, 100)) * Mathf.Pow (1.06f, Mathf.Max (nLevel - 101, 0)));
	}

	protected override void EnhanceButtonClick ()
	{
		fCostGold = 3000 * Mathf.Pow (1.1f, Mathf.Min (nLevel - 1, 100)) * Mathf.Pow (1.06f, Mathf.Max (nLevel - 101, 0));

		if (fCostGold <= ScoreManager.ScoreInstance.GetGold ()) {

			nLevel++;

			cPlayer.SetRepairLevel (nLevel);

			fEnhanceValue = cPlayer.GetCreatorWeapon().fRepair * Mathf.Pow (1.125f, nLevel - 1);

			cPlayer.SetBasicRepairPower(fEnhanceValue);

			EnhanceText.text =string.Format("{0} : {1}", strEnhanceName , nLevel);

			ScoreManager.ScoreInstance.GoldPlus (-fCostGold	);

			CostGoldText.text  = ChangeValue(3000 * Mathf.Pow (1.1f, Mathf.Min (nLevel - 1, 100)) * Mathf.Pow (1.06f, Mathf.Max (nLevel - 101, 0)));
		}
	}
}
