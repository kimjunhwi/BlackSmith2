using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadOnlys;

public class GoldHammer : EpicOption {

	public float fSaveValue = 0;

	public GoldHammer()
	{
		fValue = 50.0f;
		 

		nIndex = (int)E_EPIC_INDEX.E_EPIC_GOLD_HAMMER;
	}

	public override void Init (int _nDay,Player _player)
	{
		cPlayerData = _player;

		nCostDay = _nDay;

		nDightDay = (int)(nCostDay * fDivisionDay);

		fValue += fValue * nDightDay * fDivisionDay;

		strExplain = string.Format("골드 확득량 +{0}%",fValue);

		cPlayerData.SetBasicGoldPlusPercent (cPlayerData.GetBasicGoldPlusPercent() + fValue);
	}

	public override string GetExplain () { return strExplain; }

	public override bool CheckOption ()
	{
		return true;
	}

	public override void Relive ()
	{
		cPlayerData.SetBasicGoldPlusPercent (cPlayerData.GetBasicGoldPlusPercent() - fValue);
	}

	public override void Save()
	{
		cPlayerData.SetBasicGoldPlusPercent (cPlayerData.GetBasicGoldPlusPercent() - fValue);
	}

	public override void Load ()
	{
		cPlayerData.SetBasicGoldPlusPercent (cPlayerData.GetBasicGoldPlusPercent() + fValue);
	}
}
