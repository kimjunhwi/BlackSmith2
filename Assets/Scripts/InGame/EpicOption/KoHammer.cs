using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadOnlys;

public class KoHammer : EpicOption {

	int nTouchCount = 0;
	const int nTouchValue = 3;

	public override void Init (int _nDay,Player _player)
	{
		nIndex = (int)E_EPIC_INDEX.E_EPIC_KO_HAMMER;

		cPlayerData = _player;

		strExplain = string.Format("{0}회 터치시 다음 크리",nTouchValue);
	}

	public override string GetExplain () { return strExplain; }

	public override bool CheckOption ()
	{
		nTouchCount++;

		if (nTouchCount >= nTouchValue) 
		{
			nTouchCount = 0;

			return true;
		}

		return false;
	}
}
