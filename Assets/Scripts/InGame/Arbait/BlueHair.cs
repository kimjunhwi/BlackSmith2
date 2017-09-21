﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadOnlys;

public class BlueHair : ArbaitBatch {


    private float fChangeRepair = 0.0f;

    private float fGetRepairPower = 0.0f;

    private float fMinusRepair = 0.0f;

    protected override void Awake()
    {
        base.Awake();

        nIndex = (int)E_ARBAIT.E_ROY;

		strSkillExplain = string.Format ("대장장이 수리력 {0}% 증가", m_CharacterChangeData.fSkillPercent);
    }



    protected override void Update()
    {
        StartCoroutine(this.CharacterAction());
    }

    protected override void OnEnable()
    {
        if (m_CharacterChangeData == null || nBatchIndex == -1)
            return;

        bIsComplate = false;

        nGrade = m_CharacterChangeData.grade;

        E_STATE = E_ArbaitState.E_WAIT;

        CheckCharacterState(E_STATE);

		playerData.SetRepairPower ();

        SpawnManager.Instance.InsertWeaponArbait(m_CharacterChangeData.index, nBatchIndex);

		AuraObject.SetActive (false);
    }

    protected override void OnDisable()
	{
        base.OnDisable();

        fChangeRepair = 0.0f;

        fGetRepairPower = 0.0f;

        fMinusRepair = 0.0f;

		playerData.SetRepairPower ();
    }

	public override void StartAura (float _fTime)
	{


	}

	public override void EnhacneArbait ()
	{
		m_CharacterChangeData.fSkillPercent += m_CharacterChangeData.fSkillPercent * 10 * 0.01f;

		m_CharacterChangeData.strExplains = string.Format ("대장장이 수리력 {0}% 증가", m_CharacterChangeData.fSkillPercent);
	}

    public override void ApplySkill()
    {
//        if (fChangeRepair >= 1)
//            ReliveSkill();
//
//        fGetRepairPower = playerData.GetBasicRepairPower();
//
//        fChangeRepair = fGetRepairPower * (m_CharacterChangeData.fSkillPercent * 0.01f);
    }

    protected override void ReliveSkill()
    {
//        if (fChangeRepair <= 1)
//            return;
//
//		fGetRepairPower = playerData.GetBasicRepairPower();
//
//        fMinusRepair = fGetRepairPower - fChangeRepair;
    }

    public override void RelivePauseSkill()
    {
        base.RelivePauseSkill();

        ReliveSkill();
    }

    public override void ApplyPauseSkill()
    {
        base.ApplyPauseSkill();

        ApplySkill();
    }

    public override void CheckCharacterState(E_ArbaitState _E_STATE)
	{
        if (E_STATE == _E_STATE)
           return;

        //액션 변경
        E_STATE = _E_STATE;
		animator.speed = 1.0f;
        //추후 사용 될 수 있을 부분이 있기에 만들어둠
        switch(E_STATE)
        {
            case E_ArbaitState.E_WAIT:
                {
					animator.speed = 1.0f;
                }
                break;

		case E_ArbaitState.E_REPAIR:
			{
				
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
			}
			break;
        }
	}

	protected override IEnumerator CharacterAction()
	{
        yield return new WaitForSeconds(0.1f);

		switch(E_STATE)
		{
		case E_ArbaitState.E_WAIT:

			//대기중 수리 아이템이 있을 경우 수리로 바꿈
			if (AfootOjbect != null && bIsRepair == true)
				CheckCharacterState(E_ArbaitState.E_REPAIR);

			break;
		case E_ArbaitState.E_REPAIR:

			//수리
			fTime += Time.deltaTime;

			if(AfootOjbect == null || bIsRepair == false)
				CheckCharacterState(E_ArbaitState.E_WAIT);

			//수리 시간이 되면 0으로 초기화 하고 수리해줌
			if(fTime >= m_fRepairTime)
			{
				fTime = 0.0f;

				animator.SetTrigger("bIsRepair");

				dDodomchitRepair = m_CharacterChangeData.dRepairPower * fDodomchitRepairPercent * 0.01f;

				if (playerData.AccessoryEquipmnet != null) 
				{
					if (playerData.AccessoryEquipmnet.nIndex == (int)E_BOSS_ITEM.DODOM_GLASS) {
						fBossRepairPercent = playerData.AccessoryEquipmnet.fBossOptionValue;
						fBossCriticalPercent = playerData.AccessoryEquipmnet.fBossOptionValue;
					} else {
						fBossRepairPercent = 0;
						fBossCriticalPercent = 0;
					}
				}


				//크리티컬 확률 
				if (Random.Range (0, 100) <= Mathf.Round (m_CharacterChangeData.fCritical + fBossCriticalPercent)) 
					m_dComplate += m_CharacterChangeData.dRepairPower * 1.5f + dDodomchitRepair;
				else 
					m_dComplate += m_CharacterChangeData.dRepairPower + dDodomchitRepair;

				m_dComplate += m_dComplate * fBossRepairPercent * 0.01f;

				//완성 됐을 경우
				if (m_dComplate >= weaponData.dMaxComplate)
				{
					ScoreManager.ScoreInstance.GoldPlus(100);

					ComplateWeapon();
				}

				SpawnManager.Instance.CheckComplateWeapon (AfootOjbect, m_dComplate,m_fTemperator);
			}
			break;

		case E_ArbaitState.E_BOSSREPAIR:

			//수리
			fTime += Time.deltaTime;

			//수리 시간이 되면 0으로 초기화 하고 수리해줌
			if(fTime >= m_fRepairTime)
			{
				fTime = 0.0f;

				animator.SetTrigger("bIsRepair");

				RepairShowObject.SetCurCompletion(m_CharacterChangeData.dRepairPower );
			}

			break;
		}

	
	}
}
