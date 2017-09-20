﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadOnlys;

public class SledeHammer : EpicOption {
	
	public override void Init (int _nDay,Player _player)
	{
		nIndex = (int)E_EPIC_INDEX.E_EPIC_SLEDE_HAMMER;

		cPlayerData = _player;

		strExplain = string.Format("터치 시 크리 데미지 +{0}% 명중률 -{1}%(빗나갈시 증가 및 감소 능력 초기화",fSledeCriticalDamage,fSledeAccuracyRate);
	}

	public override string GetExplain () { return strExplain; }

	public override bool CheckOption ()
	{
		fSledeAccuracyRate--;
		fSledeCriticalDamage++;

		cPlayerData.SetAccuracyRate ();
		cPlayerData.SetCriticalDamage ();

		strExplain = string.Format("터치 시 크리 데미지 +{0}% 명중률 -{1}%(빗나갈시 증가 및 감소 능력 초기화",fSledeCriticalDamage,fSledeAccuracyRate);

		return true;
	}

	public void Miss()
	{
		fSledeAccuracyRate = 0;
		fSledeCriticalDamage = 0;

		cPlayerData.SetAccuracyRate ();
		cPlayerData.SetCriticalDamage ();
	}

	public override void Relive ()
	{
		fSledeAccuracyRate++;
		fSledeCriticalDamage++;

		cPlayerData.SetAccuracyRate ();
		cPlayerData.SetCriticalDamage ();
	}
}
