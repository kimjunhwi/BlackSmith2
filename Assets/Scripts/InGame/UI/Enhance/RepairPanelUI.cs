using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadOnlys;

public class RepairPanelUI : EnhanceUI {

	protected override void Awake ()
	{
		base.Awake ();

		nLevel = cPlayer.GetRepairLevel ();

		EnhanceText.text =string.Format("{0} {1}", strEnhanceName , nLevel);

		CostGoldText.text = ChangeValue(1000 * Mathf.Pow (1.095f, nLevel - 1));
	}

	protected override void EnhanceButtonClick ()
	{
		fCostGold = 1000 * Mathf.Pow (1.095f, nLevel - 1);

		if (fCostGold <= ScoreManager.ScoreInstance.GetGold ()) {

			nLevel++;

			cPlayer.SetRepairLevel (nLevel);

			//제작무기공격*(1+(강화레벨*0.03))* 1.022^(QUOTIENT(강화레벨-1,10))

//			float fOriValue = nLevel - 1;
//			float fMinusValue = Mathf.Floor( (nLevel - 1) * 0.1f ) * 10;
//			float result = fOriValue - fMinusValue;

			double dCurComplete = cPlayer.GetCreatorWeapon().dRepair *  Mathf.Pow (1.022f, (Mathf.Floor((nLevel - 1) * 0.1f))) * (1 + (nLevel * 0.03f));

			dCurComplete = (nLevel < 10) ? dCurComplete + 5 : dCurComplete;

			cPlayer.SetBasicRepairPower (dCurComplete);

			EnhanceText.text =string.Format("{0} {1}", strEnhanceName , nLevel);

			ScoreManager.ScoreInstance.GoldPlus (-fCostGold	);

			CostGoldText.text  = ChangeValue(1000 * Mathf.Pow (1.095f, nLevel - 1));
		}
	}
}
