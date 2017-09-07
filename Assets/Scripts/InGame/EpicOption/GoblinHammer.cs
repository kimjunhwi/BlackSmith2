using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadOnlys;

public class GoblinHammer : EpicOption {

	float fValue = 0;

	public override void Init (int _nDay,Player _player)
	{
		nIndex = (int)E_EPIC_INDEX.E_EPIC_GOBLIN_HAMMER;

		cPlayerData = _player;

		strExplain = string.Format("터치 시 수리력이 0% ~ 200% 적용(10% 단위로 적용)");
	}

	public override string GetExplain () { return strExplain; }

	public override bool CheckOption ()
	{
		fValue = (int)Random.Range (0, 20 + 1) * 10;

		return true;
	}
}
