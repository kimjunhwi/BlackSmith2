using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadOnlys;

public class FreezingTuna : EpicOption {


	float fValue = 0.3f;
	float fResultValue = 0.0f;
	float fWaterPercent = 0.0f;
	SpawnManager spawnManager = null;

	RepairObject RepairShowObject;

	public override void Init (int _nDay,Player _player)
	{
		nIndex = (int)E_EPIC_INDEX.E_EPIC_FREEZING_TUNA;

		cPlayerData = _player;

		RepairShowObject = GameObject.Find("RepairPanel").GetComponentInChildren<RepairObject>();

		strExplain = string.Format("현재 남은 물 1%마다 수리력 {0}% 증가",fValue);
	}

	public override string GetExplain () { return strExplain; }

	public override bool CheckOption ()
	{
		fWaterPercent = (int)(RepairShowObject.fCurrentWater / cPlayerData.GetBasicMaxWaterPlus ()) * 100;

		fResultValue = fWaterPercent * fValue;

		return true;
	}

	public override void Relive ()
	{
		cPlayerData.SetRepairPower ();
	}
}
