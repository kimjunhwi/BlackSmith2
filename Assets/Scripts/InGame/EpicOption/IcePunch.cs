﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadOnlys;

public class IcePunch : EpicOption {

	public override void Init (int _nDay,Player _player)
	{
		fCriticalDamage = 50;
		fWaterPlus = 50;
		fMinusRepiar = -30;

		nIndex = (int)E_EPIC_INDEX.E_EPIC_ENGINE_ICEPUNCH;

		cPlayerData = _player;

		strExplain = string.Format("크리 데미지 +{0}%, 물 PLUS +{1}, 수리력 -{2}%",fCriticalDamage,fWaterPlus,fMinusRepiar);

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
