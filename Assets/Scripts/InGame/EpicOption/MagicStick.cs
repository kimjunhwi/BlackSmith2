using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadOnlys;

public class MagicStick : EpicOption {

	public override void Init (int _nDay,Player _player)
	{
		fValue = 1;

		nCostDay = _nDay;

		cPlayerData = _player;

		nIndex = (int)E_EPIC_INDEX.E_EPIC_MAGIC;

		nDightDay = (int)(nCostDay * fDivisionDay);

		fValue += fValue * nDightDay * fDivisionDay;  

		strExplain = string.Format("{0}% 확률로 3회 터치",fValue);
	}

	public override string GetExplain () { return strExplain; }

	public override bool CheckOption ()
	{
		if (Random.Range (0, 100) <= fValue)
			return true;

		return false;
	}

	public override void Relive ()
	{
	}
}
