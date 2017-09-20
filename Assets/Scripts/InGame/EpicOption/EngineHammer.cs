using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadOnlys;

public class EngineHammer : EpicOption {

	float fValue = 50f;
	float fMaxPercent = 70.0f;
	float fMinPercent = 30.0f;



	RepairObject RepairShowObject;

	public override void Init (int _nDay,Player _player)
	{
		nIndex = (int)E_EPIC_INDEX.E_EPIC_FREEZING_TUNA;

		cPlayerData = _player;

		RepairShowObject = GameObject.Find("RepairPanel").GetComponentInChildren<RepairObject>();

		strExplain = string.Format("온도 70% 도달시 크리데미지 {0}%증가 온도 30% 이하일 때 효과 삭제",fValue);
	}

	public override string GetExplain () { return strExplain; }

	public override bool CheckOption ()
	{
		if (RepairShowObject.fCurrentTemperature >= 70) 
		{
			bIsApplyBuff = true;

			cPlayerData.SetCriticalDamage ();
			return true;
		}

		if (RepairShowObject.fCurrentTemperature <30) 
		{
			bIsApplyBuff = false;

			cPlayerData.SetCriticalDamage ();
			return false;
		}

		return false;
	}

	public override void Relive ()
	{
		bIsApplyBuff = false;

		cPlayerData.SetCriticalDamage ();
	}
}
