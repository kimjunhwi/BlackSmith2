using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadOnlys;

public class RubberChicken : EpicOption {

	float fValue = 5;
	bool bIsSuccessed = false;

	public override void Init (int _nDay,Player _player)
	{
		nIndex = (int)E_EPIC_INDEX.E_EPIC_RUBBER_CHICKEN;

		cPlayerData = _player;

		strExplain = string.Format("손님 성공시 대성공 확률 {0}% 증가 대성공 발동시 증가된 확률 초기화",fValue);
	}

	public override string GetExplain () { return strExplain; }

	public override bool CheckOption ()
	{
		return false;
	}

	public override void Relive ()
	{
		cPlayerData.SetBigSuccessed ();
	}
}
