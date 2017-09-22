using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadOnlys;

public class Druid : ArbaitBatch
{
    private bool m_bIsApplyBuff = false;
    private float m_fBuffTime = 0.0f;

	private double dChangeRepair = 0.0f;

    private float fChangeCritical = 0.0f;

    protected override void Awake()
    {
        base.Awake();

        nIndex = (int)E_ARBAIT.E_ELLIE;

		strSkillExplain = string.Format ("대장장이 크리시 모든 직원 공격속도 {0}% 증가", m_CharacterChangeData.fSkillPercent);

		normalParticlePool = GameObject.Find ("NormalRepairPool").GetComponent<SimpleObjectPool> ();

		CriticalParticlePool = GameObject.Find ("CriticalRepairPool").GetComponent<SimpleObjectPool> ();
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

        nGrade = m_CharacterChangeData.grade;

        E_STATE = E_ArbaitState.E_WAIT;

        CheckCharacterState(E_STATE);

        SpawnManager.Instance.InsertWeaponArbait(m_CharacterChangeData.index, nBatchIndex);
		AuraObject.SetActive (false);

    }

    protected override void OnDisable()
    {
        if (m_bIsApplyBuff)
        {
            m_bIsApplyBuff = false;

			playerData.SetBasicRepairPower(playerData.GetBasicRepairPower() - dChangeRepair);

			playerData.SetBasicCriticalChance(playerData.GetBasicCriticalChance() - fChangeCritical);
        }

        base.OnDisable();

        m_bIsApplyBuff = false;

        m_fBuffTime = 0.0f;

		dChangeRepair = 0.0f;

        fChangeCritical = 0.0f;
    }

	public override void EnhacneArbait ()
	{
		m_CharacterChangeData.fSkillPercent += m_CharacterChangeData.fSkillPercent * 10 * 0.01f;

		strSkillExplain = string.Format ("대장장이 크리시 모든 직원 공격속도 {0}% 증가", m_CharacterChangeData.fSkillPercent);
	}

	public override void StartAura (float _fTime)
	{
		if (bIsAura)
			fAuraTime = 0;

		else
			StartCoroutine (AuraParticle ());
	}

	public override IEnumerator AuraParticle ()
	{
		yield return new WaitForSeconds(0.1f);

		AuraObject.SetActive (true);

		bIsAura = true;

		while (true)
		{
			yield return null;

			fAuraTime += Time.deltaTime;

			if (fAuraTime > 3.0f)
				break;
		}

		if (bIsAura == false)
			yield break;

		fAuraTime = 0.0f;

		AuraObject.SetActive (false);

		bIsAura = false;
	}

    public override void ApplySkill()
    {
        if (m_bIsApplyBuff)
            m_fBuffTime = 0.0f;

        else
            StartCoroutine(ApplyDruidSkill());
    }

    protected override void ReliveSkill()
    {
    }

    public override void RelivePauseSkill()
    {
        base.RelivePauseSkill();

        if (m_bIsApplyBuff)
        {
			playerData.SetBasicRepairPower(playerData.GetBasicRepairPower() - dChangeRepair);

			playerData.SetBasicCriticalChance(playerData.GetBasicCriticalChance() - fChangeCritical);
        }

    }

    public override void ApplyPauseSkill()
    {
        base.ApplyPauseSkill();

        if (m_bIsApplyBuff)
        {
			playerData.SetBasicRepairPower(playerData.GetBasicRepairPower() + dChangeRepair);

			playerData.SetBasicCriticalChance(playerData.GetBasicCriticalChance() + fChangeCritical);
        }
    }

    private IEnumerator ApplyDruidSkill()
    {
        yield return new WaitForSeconds(0.1f);

		if (m_bIsApplyBuff == false) {

			dChangeRepair = playerData.GetBasicRepairPower () * (m_CharacterChangeData.fSkillPercent * 0.01f);

			fChangeCritical = playerData.GetBasicCriticalChance () * (m_CharacterChangeData.fSkillPercent * 0.01f);

			fChangeCritical = Mathf.Round (fChangeCritical);

			playerData.SetBasicRepairPower (playerData.GetBasicRepairPower () + dChangeRepair);

			playerData.SetBasicCriticalChance (playerData.GetBasicCriticalChance () + fChangeCritical);

		}

		m_bIsApplyBuff = true;

        while (true)
        {
            yield return null;

			m_fBuffTime += Time.deltaTime;

			if (m_fBuffTime > 3.0f)
                break;
        }

		if (m_bIsApplyBuff == false)
			yield break;

		m_fBuffTime = 0.0f;

        m_bIsApplyBuff = false;

		playerData.SetBasicRepairPower(playerData.GetBasicRepairPower() - dChangeRepair);

		playerData.SetBasicCriticalChance(playerData.GetBasicCriticalChance() - fChangeCritical);
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

				if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Druid_Repair_Work"))
				{            
					// Do something
					animator.SetTrigger("bIsNormalRepair");
				}        

				if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Druid_Repair_Critical"))
				{
					// Do something
					animator.SetTrigger("bIsCriticalRepair");
				}


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
                if (AfootOjbect != null)
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
                if (fTime >= m_fRepairTime)
                {
				fTime = 0.0f;

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

				animator.SetTrigger("bIsRepair");

				RepairShowObject.SetCurCompletion(m_CharacterChangeData.dRepairPower );
			}

			break;
        }
    }
}