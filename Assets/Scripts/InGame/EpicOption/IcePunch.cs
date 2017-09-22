using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadOnlys;

public class IcePunch : EpicOption {

	private float m_fSaveCritical;
	private float m_fSaveWaterPlus;
	private float m_fSaveMinusRepair;

	public override void Init (int _nDay,Player _player)
	{
		fCriticalDamage = 50;
		fWaterPlus = 50;
		fMinusRepiar = -30;

		nCostDay = _nDay;

		nDightDay = (int)(nCostDay * fDivisionDay);

		nIndex = (int)E_EPIC_INDEX.E_EPIC_ICEPUNCH;

		cPlayerData = _player;

		fCriticalDamage += fCriticalDamage * nDightDay * fDivisionDay;
		fWaterPlus += fWaterPlus * nDightDay * fDivisionDay; 
		fMinusRepiar += fMinusRepiar * nDightDay * fDivisionDay; 

		strExplain = string.Format("크리 데미지 +{0}%, 물 PLUS +{1}, 수리력 -{2}%",fCriticalDamage,fWaterPlus,fMinusRepiar);

	}

	public override string GetExplain () { return strExplain; }

	public override bool CheckOption ()
	{
		return true;
	}

	public override void Relive ()
	{
		
	}

	public override void Save ()
	{
		m_fSaveCritical = fCriticalDamage;
		m_fSaveWaterPlus = fWaterPlus;
		m_fSaveMinusRepair = fMinusRepiar;

		fCriticalDamage = 0;
		fWaterPlus = 0;
		fMinusRepiar = 0;

		cPlayerData.SetCriticalDamage ();
		cPlayerData.SetWaterPlus ();
		cPlayerData.SetRepairPower ();
	}

	public override void Load ()
	{
		fCriticalDamage = m_fSaveCritical;
		fWaterPlus = m_fSaveWaterPlus;
		fMinusRepiar = m_fSaveMinusRepair;

		cPlayerData.SetCriticalDamage ();
		cPlayerData.SetWaterPlus ();
		cPlayerData.SetRepairPower ();
	}
}
