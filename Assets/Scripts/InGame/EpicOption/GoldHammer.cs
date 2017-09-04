using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadOnlys;

public class GoldHammer : EpicOption {

	public float fValue = 0;

	public GoldHammer()
	{
		fValue = 50.0f;

		nIndex = (int)E_EPIC_INDEX.E_EPIC_GOLD_HAMMER;
	}

	public override void Init (int _nDay,Player _player)
	{
		cPlayerData = _player;

		strExplain = string.Format("골드 확득량 +{0}%",fValue);

		cPlayerData.SetGoldPlusPercent ();
	}

	public override string GetExplain () { return strExplain; }

	public override bool CheckOption ()
	{
		return true;
	}

	public override void Relive ()
	{
		cPlayerData.SetGoldPlusPercent ();
	}
}
