using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadOnlys;

public class MagicStick : EpicOption {

	float fValue = 1;

	public override void Init (int _nDay,Player _player)
	{
		nIndex = (int)E_EPIC_INDEX.E_EPIC_MAGIC;

		cPlayerData = _player;

		strExplain = string.Format("{0}% 확률로 5회 터치",fValue);
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
