﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadOnlys;

public class Ice : ArbaitBatch {

	string name = "얼음 골렘";

	protected override void Awake()
	{
		base.Awake();

		AuraObject.SetActive (false);

		nIndex = (int)E_ARBAIT.E_ICE;

		m_CharacterChangeData.name = name;

		normalParticlePool = GameObject.Find ("IceNormalRepairPool").GetComponent<SimpleObjectPool> ();

		CriticalParticlePool = GameObject.Find ("IceCriticalRepairPool").GetComponent<SimpleObjectPool> ();

		strSkillExplain = string.Format ("물 Plus {0}%, 크리데미지 {1}% 증가", m_CharacterChangeData.fSkillPercent, m_CharacterChangeData.fSkillPercent);
	
		m_CharacterChangeData.strExplains = string.Format ("물 Plus {0}%, 크리데미지 {1}% 증가", m_CharacterChangeData.fSkillPercent, m_CharacterChangeData.fSkillPercent);

		m_CharacterChangeData.strPurchasing = string.Format ("{0} / 20 이상 클리어", m_CharacterChangeData.nScoutCount);
	}



	public override void Purchasing ()
	{
		m_CharacterChangeData.strPurchasing = string.Format ("{0} / 20 이상 클리어", m_CharacterChangeData.nScoutCount);
	}

	// Update is called once per frame
	protected override void Update()
	{
		StartCoroutine(this.CharacterAction());
	}


	protected override void OnEnable()
	{
		if (m_CharacterChangeData == null || nBatchIndex == -1)
			return;

		bIsComplate = false;

		bIsAura = false;

		nGrade = m_CharacterChangeData.grade;

		E_STATE = E_ArbaitState.E_WAIT;

		CheckCharacterState(E_STATE);

		SpawnManager.Instance.InsertWeaponArbait(m_CharacterChangeData.index,nBatchIndex);
		AuraObject.SetActive (false);

	}

	protected override void OnDisable()
	{
		ReliveSkill();

		base.OnDisable();
	}

	public override void ApplySkill ()
	{

	}

	protected override void ReliveSkill()
	{

	}

	public override void Setting ()
	{
		m_CharacterChangeData.name = name;

		m_CharacterChangeData.strExplains = string.Format ("물 Plus {0}%, 크리데미지 {1}% 증가", m_CharacterChangeData.fSkillPercent, m_CharacterChangeData.fSkillPercent);
	}

	public override void EnhacneArbait ()
	{
		m_CharacterChangeData.fSkillPercent += m_CharacterChangeData.fSkillPercent * 1 * 0.01f;

		m_CharacterChangeData.strExplains = string.Format ("물 Plus {0}%, 크리데미지 {1}% 증가", m_CharacterChangeData.fSkillPercent, m_CharacterChangeData.fSkillPercent);
	}

	public override void CheckCharacterState(E_ArbaitState _E_STATE)
	{
		if (E_STATE == _E_STATE)
			return;

		//액션 변경
		E_STATE = _E_STATE;
		animator.speed = 1.0f;

		//추후 사용 될 수 있을 부분이 있기에 만들어둠
		switch (E_STATE)
		{
		case E_ArbaitState.E_WAIT:
			{
				animator.speed = 1.0f;

				if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Ice_Repair_Work"))
				{            
					// Do something
					animator.SetTrigger("bIsNormalRepair");
				}        

				if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Ice_Repair_Critical"))
				{
					// Do something
					animator.SetTrigger("bIsCriticalRepair");
				}


			}
			break;

		case E_ArbaitState.E_REPAIR:
			{
				animator.speed = 1.0f;
			}

			break;
		case E_ArbaitState.E_FREEZE:
			{
				animator.speed = 0.0f;
			}
			break;
		case E_ArbaitState.E_BOSSREPAIR:
			{
				fTime = 0.0F;
				animator.speed = 1.0f;
			}
			break;
		}
	}

	protected override IEnumerator CharacterAction()
	{
		yield return new WaitForSeconds(0.1f);

		switch (E_STATE)
		{
		case E_ArbaitState.E_WAIT:
			SpawnManager.Instance.InsertWeaponArbait(m_CharacterChangeData.index, nBatchIndex);

			//대기중 수리 아이템이 있을 경우 수리로 바꿈
			if (AfootOjbect != null && bIsRepair == true)
				CheckCharacterState(E_ArbaitState.E_REPAIR);

			break;
		case E_ArbaitState.E_REPAIR:

			//수리
			fTime += Time.deltaTime;

			if(AfootOjbect == null || bIsRepair == false)
				CheckCharacterState(E_ArbaitState.E_WAIT);

			if(AfootOjbect == RepairShowObject.AfootObject)
				SpawnManager.Instance.InsertWeaponArbait(m_CharacterChangeData.index, nBatchIndex);

			//수리 시간이 되면 0으로 초기화 하고 수리해줌
			if (fTime >= m_CharacterChangeData.fAttackSpeed)
			{
				fTime = 0.0f;

				m_dComplate = 0;

				dDodomchitRepair = m_CharacterChangeData.dRepairPower * fDodomchitRepairPercent * 0.01f;

				if (playerData.AccessoryEquipmnet != null) {
					if (playerData.AccessoryEquipmnet.nIndex == (int)E_BOSS_ITEM.DODOM_GLASS) {
						fBossRepairPercent = playerData.AccessoryEquipmnet.fBossOptionValue;
						fBossCriticalPercent = playerData.AccessoryEquipmnet.fBossOptionValue;
					} else {
						fBossRepairPercent = 0;
						fBossCriticalPercent = 0;
					}
				}


				//크리티컬 확률 
				if (Random.Range (0, 100) <= Mathf.Round (m_CharacterChangeData.fCritical + fBossCriticalPercent)) {
					animator.SetTrigger ("bIsCriticalRepair");
					m_dComplate += m_CharacterChangeData.dRepairPower * 1.5f + dDodomchitRepair;
				} else {
					animator.SetTrigger ("bIsNormalRepair");
					m_dComplate += m_CharacterChangeData.dRepairPower + dDodomchitRepair;
				}

				m_dComplate += m_dComplate * fBossRepairPercent * 0.01f;
				//완성 됐을 경우
				if (m_dComplate >= weaponData.dMaxComplate)
				{
					ScoreManager.ScoreInstance.GoldPlus(100);

					ComplateWeapon();
				}

				SpawnManager.Instance.CheckComplateWeapon(AfootOjbect, m_dComplate,m_fTemperator);
			}
			break;
		case E_ArbaitState.E_BOSSREPAIR:

			//수리
			fTime += Time.deltaTime;

			//수리 시간이 되면 0으로 초기화 하고 수리해줌
			if(fTime >= m_fRepairTime)
			{
				fTime = 0.0f;
				m_dComplate = 0;

				dDodomchitRepair = m_CharacterChangeData.dRepairPower * fDodomchitRepairPercent * 0.01f;

				if (playerData.AccessoryEquipmnet != null) {
					if (playerData.AccessoryEquipmnet.nIndex == (int)E_BOSS_ITEM.DODOM_GLASS) {
						fBossRepairPercent = playerData.AccessoryEquipmnet.fBossOptionValue;
						fBossCriticalPercent = playerData.AccessoryEquipmnet.fBossOptionValue;
					} else {
						fBossRepairPercent = 0;
						fBossCriticalPercent = 0;
					}
				}


				//크리티컬 확률 
				if (Random.Range (0, 100) <= Mathf.Round (m_CharacterChangeData.fCritical + fBossCriticalPercent)) {
					animator.SetTrigger ("bIsCriticalRepair");
					m_dComplate += m_CharacterChangeData.dRepairPower * 1.5f + dDodomchitRepair;
				} else {
					animator.SetTrigger ("bIsNormalRepair");
					m_dComplate += m_CharacterChangeData.dRepairPower + dDodomchitRepair;
				}

				m_dComplate += m_dComplate * fBossRepairPercent * 0.01f;

				m_dComplate += m_dComplate * playerData.changeStats.fArbaitsPower * 0.01f;

				if (spawnManager.shopCash.isConumeBuff_Staff)
					m_dComplate *= 2;

				RepairShowObject.SetCurCompletion(m_dComplate );
			}

			break;
		}
	}
}
