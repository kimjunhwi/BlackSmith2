using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadOnlys;

public class CriticalPanelUI : EnhanceUI {

	public float fBasic = 10.0f;

	protected override void Awake ()
	{
		base.Awake ();

		nLevel = cPlayer.GetCriticalLevel ();

		EnhanceText.text =string.Format("{0} : {1}", strEnhanceName , nLevel);

		CostGoldText.text  = ChangeValue(3000 * Mathf.Pow (1.1f, Mathf.Min (nLevel - 1, 100)) * Mathf.Pow (1.06f, Mathf.Max (nLevel - 101, 0)));
	}

	protected override void EnhanceButtonClick ()
	{
		fCostGold = 3000 * Mathf.Pow (1.1f, Mathf.Min (nLevel - 1, 100)) * Mathf.Pow (1.06f, Mathf.Max (nLevel - 101, 0));

		if (fCostGold <= ScoreManager.ScoreInstance.GetGold ()) {

			nLevel++;

			cPlayer.SetCriticalLevel (nLevel);

			fEnhanceValue = cPlayer.GetBasicCriticalChance() + (fBasic * 0.01f);

			cPlayer.SetBasicCriticalChance(fEnhanceValue);

			EnhanceText.text = strEnhanceName + nLevel;

			ScoreManager.ScoreInstance.GoldPlus (-fCostGold	);

			CostGoldText.text  = ChangeValue(3000 * Mathf.Pow (1.1f, Mathf.Min (nLevel - 1, 100)) * Mathf.Pow (1.06f, Mathf.Max (nLevel - 101, 0)));
		}
	}
}
