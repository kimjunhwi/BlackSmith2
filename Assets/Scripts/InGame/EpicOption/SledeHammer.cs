using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadOnlys;

public class SledeHammer : EpicOption {

	private float m_fBasicPlusAccuracy = 1.0f;
	private float m_fBasicPlusDamage = 1.0f;

	private float m_fSaveAccuracy = 0;
	private float m_fSaveDamage = 0;

	public override void Init (int _nDay,Player _player)
	{
		nCostDay = _nDay;	

		nDightDay = (int)(nCostDay * fDivisionDay);

		nIndex = (int)E_EPIC_INDEX.E_EPIC_SLEDE_HAMMER;

		cPlayerData = _player;

		nDightDay = (int)(nCostDay * fDivisionDay);

		m_fBasicPlusDamage += m_fBasicPlusDamage * nDightDay *fDivisionDay;
		m_fBasicPlusAccuracy += m_fBasicPlusAccuracy * nDightDay * fDivisionDay; 

		strExplain = string.Format("터치 시 크리 데미지 +{0}% 명중률 -{1}%(빗나갈시 증가 및 감소 능력 초기화",m_fBasicPlusDamage,m_fBasicPlusAccuracy);
	}

	public override string GetExplain () { return strExplain; }

	public override bool CheckOption ()
	{
		fSledeAccuracyRate -= m_fBasicPlusDamage;
		fSledeCriticalDamage += m_fBasicPlusAccuracy;

		cPlayerData.SetAccuracyRate ();
		cPlayerData.SetCriticalDamage ();

		strExplain = string.Format("터치 시 크리 데미지 +{0}% 명중률 -{1}%(빗나갈시 증가 및 감소 능력 초기화",fSledeCriticalDamage,fSledeAccuracyRate);

		return true;
	}

	public override void Relive ()
	{
		fSledeAccuracyRate = 0;
		fSledeCriticalDamage = 0;

		cPlayerData.SetAccuracyRate ();
		cPlayerData.SetCriticalDamage ();
	}

	public override void Save ()
	{
		m_fSaveAccuracy = fSledeAccuracyRate;
		m_fSaveDamage = fSledeCriticalDamage;

		fSledeAccuracyRate = 0;
		fSledeCriticalDamage = 0;

		cPlayerData.SetAccuracyRate ();
		cPlayerData.SetCriticalDamage ();
	}

	public override void Load ()
	{
		fSledeAccuracyRate = m_fSaveAccuracy;
		fSledeCriticalDamage = m_fSaveDamage;

		cPlayerData.SetAccuracyRate ();
		cPlayerData.SetCriticalDamage ();
	}
}
