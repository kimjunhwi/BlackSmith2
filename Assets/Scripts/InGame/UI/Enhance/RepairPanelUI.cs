﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadOnlys;
using System;

public class RepairPanelUI : EnhanceUI {

	protected override void Awake ()
	{
		base.Awake ();

		nLevel = cPlayer.GetRepairLevel ();

		EnhanceText.text =string.Format("{0} {1}", strEnhanceName , nLevel);

		CostGoldText.text = ChangeValue(1000 * Math.Pow (1.095, nLevel - 1));

		double dCurComplete;

		dCurComplete = GameManager.Instance.GetPlayer().GetCreatorWeapon ().dRepair * Math.Pow (1.022, (Mathf.Floor ((nLevel + 1) * 0.1f))) * (1 + ((nLevel + 1) * 0.02f));

		dCurComplete += nLevel;

		NextPercentText.text = ScoreManager.ScoreInstance.ChangeMoney (dCurComplete);
	}

	public void ChagneRepairData ()
	{
		double dCurComplete;


		dCurComplete = GameManager.Instance.GetPlayer().GetCreatorWeapon().dRepair * Mathf.Pow (1.022f, (Mathf.Floor ((nLevel + 1) * 0.1f)) * (1 + ((nLevel + 1) * 0.02f)));

		dCurComplete += nLevel;
		

		NextPercentText.text = ScoreManager.ScoreInstance.ChangeMoney (dCurComplete);
	}

	protected override void EnhanceButtonClick ()
	{
		fCostGold = 1000 * Math.Pow (1.095, nLevel - 1);

		if (fCostGold <= ScoreManager.ScoreInstance.GetGold ()) {

			nLevel++;

			cPlayer.SetRepairLevel (nLevel);

			//제작무기공격*(1+(강화레벨*0.03))* 1.022^(QUOTIENT(강화레벨-1,10))

//			float fOriValue = nLevel - 1;
//			float fMinusValue = Mathf.Floor( (nLevel - 1) * 0.1f ) * 10;
//			float result = fOriValue - fMinusValue;

			double dCurComplete;


			dCurComplete = cPlayer.GetCreatorWeapon ().dRepair * Math.Pow (1.022, (Mathf.Floor ((nLevel - 1) * 0.1f))) * (1 + ((nLevel -1)  * 0.02f));

			dCurComplete += nLevel;


			cPlayer.SetBasicRepairPower (dCurComplete);

			EnhanceText.text =string.Format("{0} {1}", strEnhanceName , nLevel);


			dCurComplete = cPlayer.GetCreatorWeapon ().dRepair * Math.Pow (1.022, (Mathf.Floor ((nLevel + 1) * 0.1f))) * (1 + ((nLevel + 1) * 0.02f));

			dCurComplete += nLevel;


			NextPercentText.text = ScoreManager.ScoreInstance.ChangeMoney (dCurComplete);

			ScoreManager.ScoreInstance.GoldPlus (-fCostGold	);

			CostGoldText.text  = ChangeValue(1000 * Math.Pow (1.095, nLevel - 1));
		}
	}
}
