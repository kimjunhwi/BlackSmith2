using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadOnlys;

public class RepairPanelUI : EnhanceUI {

	SmithEnhance m_EnhanceData;

	protected override void Awake ()
	{
		base.Awake ();

		nLevel = cPlayer.GetRepairLevel ();

		EnhanceText.text = strEnhanceName + nLevel;

		m_EnhanceData = enhanceDatas[(int)E_SMITH_INDEX.E_REPAIR];

		CostGoldText.text = (3000 * Mathf.Pow (1.1f, Mathf.Min (nLevel -1, 100)) * Mathf.Pow (1.06f, Mathf.Max (nLevel - 101, 0))).ToString();
	}

	protected override void EnhanceButtonClick ()
	{
//		if (nLevel != 0 && (nLevel % 10 == 0)) 
//		{
//			if (ScoreManager.ScoreInstance.GetHonor() >= m_EnhanceData.fBasicHonor + (nLevel * m_EnhanceData.fPlusHonorValue))
//			{
//				ScoreManager.ScoreInstance.HonorPlus (-(m_EnhanceData.fBasicHonor + (nLevel * m_EnhanceData.fPlusHonorValue)));
//
//				nLevel++;
//
//				cPlayer.SetBasicRepairPower(cPlayer.GetBasicRepairPower() + 1 * m_EnhanceData.fPlusPercent);
//
//				cPlayer.SetRepairLevel(nLevel);
//
//				EnhanceText.text = strEnhanceName + nLevel;
//			}
//
//			return;
//		}
//
//
//		if (ScoreManager.ScoreInstance.GetGold() >= m_EnhanceData.fBasicGold + (nLevel * m_EnhanceData.fPlusGoldValue)) {
//
//			ScoreManager.ScoreInstance.GoldPlus (-(m_EnhanceData.fBasicGold + (nLevel * m_EnhanceData.fPlusGoldValue)));
//
//			nLevel++;
//
//			cPlayer.SetBasicRepairPower(cPlayer.GetBasicRepairPower() + 1 * m_EnhanceData.fPlusPercent);
//
//			cPlayer.SetRepairLevel(nLevel);
//
//			EnhanceText.text = strEnhanceName + nLevel;
//		}

		fCostGold = 3000 * Mathf.Pow (1.1f, Mathf.Min (nLevel - 1, 100)) * Mathf.Pow (1.06f, Mathf.Max (nLevel - 101, 0));

		if (fCostGold <= ScoreManager.ScoreInstance.GetGold ()) {

			nLevel++;

			cPlayer.SetRepairLevel (nLevel);

			fEnhanceValue = cPlayer.GetCreatorWeapon().fRepair * Mathf.Pow (1.125f, nLevel - 1);

			cPlayer.SetBasicRepairPower(fEnhanceValue);

			EnhanceText.text = strEnhanceName + nLevel;

			ScoreManager.ScoreInstance.GoldPlus (-fCostGold	);

			CostGoldText.text  = (3000 * Mathf.Pow (1.1f, Mathf.Min (nLevel - 1, 100)) * Mathf.Pow (1.06f, Mathf.Max (nLevel - 101, 0))).ToString();
		}
	}
}
