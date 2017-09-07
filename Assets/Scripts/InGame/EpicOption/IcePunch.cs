using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadOnlys;

public class IcePunch : EpicOption {

	public float fWater = 0;
	public float fWaterPlus = 0;
	public float fMinusRepiar = 0;

	public override void Init (int _nDay,Player _player)
	{
		fWater = 30;
		fWaterPlus = 50;
		fMinusRepiar = -30;

		nIndex = (int)E_EPIC_INDEX.E_EPIC_ENGINE_ICEPUNCH;

		cPlayerData = _player;

		strExplain = string.Format("물 수치 +{0}%, 물 PLUS +{1}, 수리력 -{2}%",fWater,fWaterPlus,fMinusRepiar);

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
