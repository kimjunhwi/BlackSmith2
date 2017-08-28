using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadOnlys;

public class FreezingTuna : EpicOption {

	float fValue = 0.3f;

	public override void Init (int _nDay,Player _player)
	{
		nIndex = (int)E_EPIC_INDEX.E_EPIC_FREEZING_TUNA;

		cPlayerData = _player;

		strExplain = string.Format("현재 남은 물 1%마다 수리력 {0}% 증가",fValue);
	}

	public override string GetExplain () { return strExplain; }

	public override int CheckOption ()
	{
		return (int)E_EPIC_INDEX.E_EPIC_FREEZING_TUNA;
	}
}
