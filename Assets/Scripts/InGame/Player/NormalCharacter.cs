﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadOnlys;
using UnityEngine.EventSystems;


[System.Serializable]
public class NormalCharacter : Character {
    
	public PlayerController playerController;

	private int m_nCheck = -1;


	//무기가 보일 말풍선(?) // 미정
	private GameObject WeaponBackground;

	//완성도 게이지
	private Transform ComplateScale;

	private SpriteRenderer backGround;

	//돌아가는지
	public bool m_bIsAllBack = false;

	//처음 목표지점에 도달했을 경우에만 아르바이트에게 맞는게 있는지 검사함
	private bool m_bIsFirst = false;

	private int nDay = 1;

	public override void Awake()
    {
        base.Awake();

		boxCollider = gameObject.GetComponent<BoxCollider2D>();

		WeaponBackground = transform.Find("Case").gameObject;

		backGround = WeaponBackground.GetComponent<SpriteRenderer> ();

		RepairShowObject = GameObject.Find("RepairPanel").GetComponentInChildren<RepairObject>();

		weaponsSprite = WeaponBackground.transform.Find("WeaponSprite").GetComponent<SpriteRenderer>();

		ComplateScale = WeaponBackground.transform.Find ("ComplateGaugeParent").GetComponent<Transform> ();

		ComplateScale.localScale = new Vector3 (1.0f, 0, 1.0f);

		playerController = GameObject.Find ("PlayerRig").GetComponent<PlayerController> ();
    }

    //활성화 됐을 때 초기화
	void OnEnable()
    {
		backGround.sprite = NoneSpeech;

        mySprite.flipX = true;

        m_bIsRepair = false;

		m_bIsAllBack = false;

		m_bIsFirstBack = false;

        mySprite.sortingOrder = (int)E_SortingSprite.E_WALK;

		fSpeed = 1.0f;

        m_nIndex = -1;

        m_fCharacterTime = 0.0f;

        E_STATE = ENORMAL_STATE.WALK;

        transform.position = m_VecStartPos;

        weaponData = GameManager.Instance.GetWeaponData((int)E_GRADE);

		if (weaponData == null)
			return;

		//무기 데이터가 있을 경우 날짜를 받아와 적용시켜 준다
		//수리력 3% 나머지 1%
		else 
		{
			nDay = cPlayerData.GetDay ();

			float fOriValue = nDay - 1;
			float fMinusValue = Mathf.Floor( (nDay - 1) * 0.1f ) * 10;
			float result = fOriValue - fMinusValue;

			double dCurComplete = 500 * Mathf.Max( Mathf.Pow (2, (Mathf.Floor((nDay - 1) * 0.1f))),1) * (0.5 + (result) * 0.08f);

			weaponData.dMaxComplate = dCurComplete; 
			weaponData.dMinusRepair = weaponData.dMinusRepair + (weaponData.dMinusRepair * nDay * 0.03f);
			weaponData.fMinusChargingWater = weaponData.fMinusChargingWater + (weaponData.fMinusChargingWater * nDay * 0.03f);
			weaponData.dMinusCriticalDamage = weaponData.dMinusCriticalDamage + (weaponData.dMinusCriticalDamage * nDay * 0.03f);
			weaponData.fMinusUseWater = weaponData.fMinusUseWater + (weaponData.fMinusUseWater * nDay * 0.03f);
			weaponData.fMinusCriticalChance = weaponData.fMinusCriticalChance + (weaponData.fMinusCriticalChance * nDay * 0.03f);
			weaponData.fMinusAccuracy = weaponData.fMinusAccuracy + (weaponData.fMinusAccuracy * nDay * 0.03f);
			weaponData.dGold = weaponData.dGold + (weaponData.dGold * nDay * 0.03f);
			weaponData.dHonor = weaponData.dHonor + (weaponData.dHonor * nDay * 0.03f);
		}



        m_dComplate = 0;

        m_fTemperator = 0;

        m_fCharacterWaitTime = weaponData.fLimitedTime;

        weaponsSprite.sprite = weaponData.WeaponSprite;

        boxCollider.isTrigger = false;
        
        WeaponBackground.SetActive(false);

		ComplateScale.localScale = new Vector3(1.0f, 0.0f , 1.0f);
    }

    void OnDisable()
    {
        m_bIsBack = false;

        m_bIsFirst = false;

		m_bIsRepair = false;

        m_bIsFirstBack = false;

        m_bIsArrival = false;

        m_dComplate = 0;

        m_fTemperator = 0;

		m_fCharacterTime = 0.0f;

        weaponsSprite.sprite = null;

        WeaponBackground.SetActive(false);

        m_VecMoveDistance = new Vector3(0.0f, 0.0f, 0.0f);
    }

    void Update()
    {
        m_fCharacterTime += Time.deltaTime;

        StartCoroutine(this.CheckCharacterState());

        StartCoroutine(this.CharacterAction());
    }

    IEnumerator CheckCharacterState()
    {
        yield return new WaitForSeconds(0.3f);

        //시간이 지났거나 m_bIsBack이 트루일경우 돌려보냄
		if (m_bIsBack == true)
			E_STATE = ENORMAL_STATE.BACK;
		
            //시간이 지나지 않았거나 도착하지 않았다면 Walk
		else if(m_bIsArrival == false)
			E_STATE = ENORMAL_STATE.WALK;
		
            //그 외에는 대기
		else
			E_STATE = ENORMAL_STATE.WAIT;
    }

    IEnumerator CharacterAction()
    {
		switch (E_STATE) {
		case ENORMAL_STATE.WALK:

			m_anim.SetBool ("bIsWalk", true);

                //Move 함수를 통해 지정된 위치로 자연스럽게 이동하기 위해 MoveTowards 함수를 사용
			transform.position = Vector3.MoveTowards (transform.position, m_VecMoveDistance, fSpeed * Time.deltaTime);

                //만약 도착했다면
			if ((transform.position.x == m_VecMoveDistance.x)) {

                //처음일 경우 true로 바꿔주고 수리 할 수 있는 무기를 보여줌
				if (m_bIsFirst == false) {
					m_bIsFirst = true;

					WeaponBackground.SetActive (true);
				}

                //만약 수리중이라면 도착했다는것으로 간주하고 리턴
				if (m_bIsRepair) {
					m_bIsArrival = true;
					yield break;
				}

				//만약 현재 수리중인 오브젝트가 없을 경우  
                //수리 중 및 도착한것으로 바꾸고 현재 무기를 넣어준다.
				if (RepairShowObject.AfootObject == null) {
					m_bIsRepair = true;
					m_bIsArrival = true;

					RepairShowObject.GetWeapon (gameObject, weaponData, m_dComplate, m_fTemperator);

					SpeechSelect ((int)E_SPEECH.E_PLAYER);
					yield break;
				}

                //지정된 위치로 도착했다면
				if (m_bIsArrival == false) {
					
					m_bIsArrival = true;

                    //수리할 수 있는 아르바이트가 있는지 체크한다.
					m_nCheck = SpawnManager.Instance.InsertArbatiWeaponCheck (weaponData.nGrade);

					if (m_nCheck != (int)E_CHECK.E_FAIL) {
						m_bIsRepair = true;

						SpeechSelect (m_nCheck);

						SpawnManager.Instance.InsertArbaitWeapon (m_nCheck, gameObject, weaponData, m_dComplate, m_fTemperator);
					}
				}
			}
			break;

		case ENORMAL_STATE.WAIT:
			m_anim.SetBool ("bIsWalk", false);
			break;

		case ENORMAL_STATE.BACK:

                //딱 한번만 호출 되야 하는 부분
			if (!m_bIsFirstBack) 
			{
				fSpeed = 3.0f;

				m_bIsFirstBack = true;

				boxCollider.isTrigger = true;

				m_anim.SetBool ("bIsWalk", true);

				//이미지 변경이나 효과 애니메이션 변경등을 진행
				mySprite.flipX = false;

				mySprite.sortingOrder = (int)E_SortingSprite.E_BACK;

				//이미지 변경이나 효과 애니메이션 변경등을 진행
				mySprite.flipX = false; 

				mySprite.sortingOrder = (int)E_SortingSprite.E_BACK;

				WeaponBackground.SetActive(false);

                //현재 오브젝트를 보내서 있는지 확인
				RepairShowObject.CheckMyObject (gameObject);

				//현재 캐릭터를 지움
				SpawnManager.Instance.DeleteObject(gameObject);

				if(RepairShowObject.AfootObject == null && m_bIsAllBack == false)
					SpawnManager.Instance.AutoInputWeaponData ();

                //결과값 호출
				Complate (m_dComplate);

				//현재 아르바이트가 수리중인지 확인 
				ArbaitBatch arbait =  SpawnManager.Instance.OverlapArbaitData (gameObject);
                
                //수리 중 이였다면 아르바이트 초기화
				if (arbait != null)
					arbait.ResetWeaponData();
				
				WeaponBackground.SetActive (false);

			}

			transform.position = Vector3.MoveTowards (transform.position, m_VecStartPos, fSpeed * Time.deltaTime);

			if (Vector3.Distance (transform.position, m_VecStartPos) < 0.5f) 
			{
				gameObject.SetActive (false);
			}
			break;

		default:
			break;
		}

        yield return null;
    }

    //손님의 이동속도와 뒤로 가는지에 대한 선택을 위함
	public override void RetreatCharacter(float _fSpeed,bool _bIsBack, bool _bIsAllBack = false)
	{
		fSpeed = _fSpeed;

        m_bIsBack = _bIsBack;

		m_bIsAllBack = _bIsAllBack;
	}

    //지정한 인덱스로 손님을 이동시키기 위함
	public override void Move(int _nIndex)
	{
		m_nIndex = _nIndex;

		m_bIsArrival = false;

		float fDistance = 0.8f;

		fDistance *= _nIndex;

		m_VecMoveDistance = new Vector3(m_VecEndPos.x + fDistance, transform.position.y, 0);
	}

	public void GetRepairData(bool _bIsRepair,bool _bIsResearch, double _dComplate, float _fTemperator)
	{
		m_bIsRepair = _bIsRepair;
		m_dComplate = _dComplate;
		m_fTemperator = m_fTemperator;

		if (_bIsResearch) 
		{
			m_nCheck = SpawnManager.Instance.InsertArbatiWeaponCheck (weaponData.nGrade);

			if (m_nCheck != (int)E_CHECK.E_FAIL) {
				m_bIsRepair = true;

				SpeechSelect (m_nCheck);

				SpawnManager.Instance.InsertArbaitWeapon (m_nCheck, gameObject, weaponData, m_dComplate, m_fTemperator);

				return;
			}
		}

		SpeechSelect ((int)E_SPEECH.E_NONE);
	}

    //현재 무기가 누구에게 수리중인지를 지정하기 위함
	public void SpeechSelect(int _nIndex)
	{
		switch (_nIndex) {
		case (int)E_SPEECH.E_NONE: backGround.sprite = NoneSpeech; break;
		case (int)E_SPEECH.E_ARBAITONE: backGround.sprite = ArbaitOneSpeech; break;
		case (int)E_SPEECH.E_ARBAITTWO: backGround.sprite = ArbaitTwoSpeech; break;
		case (int)E_SPEECH.E_ARBAITTHREE: backGround.sprite = ArbaitThreeSpeech;break;
		case (int)E_SPEECH.E_PLAYER: backGround.sprite = PlayerRepairSpeech; break;
		}
	}

	void OnMouseDown()
	{
		if (SpawnManager.Instance.tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_START_IMAGE01 || SpawnManager.Instance.tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_START_IMAGE02)
			return;

		if (Input.GetMouseButtonDown (0) && (E_STATE == ENORMAL_STATE.WAIT || WeaponBackground.activeSelf))
		{
			//onPointerDown 보다 먼저 호출
			//if (!EventSystem.current.IsPointerOverGameObject ()) {

				m_bIsRepair = true;

				//현재 아르바이트가 수리중인지 확인 
				ArbaitBatch arbait =  SpawnManager.Instance.OverlapArbaitData (gameObject);

                if (arbait != null)
                    arbait.ResetWeaponData();

				RepairShowObject.GetWeapon (gameObject, weaponData, m_dComplate, m_fTemperator);

				backGround.sprite = PlayerRepairSpeech;
			//}
		}
	}

    //완성도를 체크하고 온도를 저장하기 위한 함수이다.
	public override bool CheckComplate (double _dComplate,float _fTemperator)
	{
		float fCurCompletY;

		m_dComplate = _dComplate;
		m_fTemperator = _fTemperator;

		fCurCompletY = (float)(m_dComplate / weaponData.dMaxComplate);

        //만약 완성도가100% 라면
        //뒤로 이동 후 true 반환
		if (fCurCompletY >= 1.0f)
		{
			//Quest
			if(	SpawnManager.Instance.repairObject.GetIsFever () == true)
				SpawnManager.Instance.questManager.QuestSuccessCheck(QuestType.E_QUESTTYPE_BIGSUCCESSANDCUSTOMERSUCCESS, 1);

			if (SpawnManager.Instance.repairObject.bIsMissShowUp == false)
				SpawnManager.Instance.questManager.QuestSuccessCheck (QuestType.E_QUESTTYPE_NOMISSCUTOMERSUCCESS, 1);
			else
				SpawnManager.Instance.repairObject.bIsMissShowUp = false;

			if (SpawnManager.Instance.repairObject.bIsWaterUse == false)
				SpawnManager.Instance.questManager.QuestSuccessCheck (QuestType.E_QUESTTYPE_NOWATERUSE, 1);
			else
				SpawnManager.Instance.repairObject.bIsWaterUse = false;


			m_bIsBack = true;

			ComplateScale.localScale = new Vector3 (ComplateScale.localScale.x, 1.0f, ComplateScale.localScale.z);

			return true;
		}

		ComplateScale.localScale = new Vector3 (ComplateScale.localScale.x, fCurCompletY, ComplateScale.localScale.z);
	
		return false;
	}

	public override void Complate(double _dComplate = 0.0f)
	{
		//Quest
		SpawnManager.Instance.questManager.QuestSuccessCheck(QuestType.E_QUESTTYPE_CUSTOMERSUCCESS, 1);

		//70%이상
		if ((weaponData.dMaxComplate * 0.7) < _dComplate) {

			nDay = cPlayerData.GetDay ();

			playerController.GuestSuccessed ();

			dGold = 250 * Mathf.Pow (1.09f, nDay - 1);

			if (cPlayerData.GearEquipmnet != null)
			{
				if (cPlayerData.GearEquipmnet.nIndex == (int)E_BOSS_ITEM.DODOM_FLOWER) {
					int nValue = 0; 

					for (int nIndex = 0; nIndex < SpawnManager.Instance.m_BatchArbait.Length; nIndex++) {
						if (SpawnManager.Instance.array_ArbaitData [nIndex].bIsAura)
							nValue++;
					}
					dGold += dGold * cPlayerData.GearEquipmnet.fBossOptionValue * nValue * 0.01f;

				}
			}

			dGold += dGold * cPlayerData.GetGoldPlusPercent() * 0.01f;

			if (SpawnManager.Instance.shopCash.isConumeBuff_Gold)
				dGold *= 2;

			if (cPlayerData.GetEpicOption () != null) 
			{
				if (cPlayerData.GetEpicOption ().nIndex == (int)E_EPIC_INDEX.E_EPIC_RUBBER_CHICKEN) 
				{
					cPlayerData.GetEpicOption ().CheckOption ();

					cPlayerData.SetBigSuccessed ();
				}
			}

			ScoreManager.ScoreInstance.GoldPlus (dGold);

			cPlayerData.SetSuccessedGuestCount (cPlayerData.GetSuccessedGuestCount () + 1);
            ScoreManager.ScoreInstance.SetSuccessedGuestCount(cPlayerData.GetSuccessedGuestCount());

			//셩공 손님이 10명 이상 이라면 
			if (cPlayerData.GetSuccessedGuestCount () >= 10) 
			{
				int nHonor =  Mathf.RoundToInt((float)(10 + 1.3 * (nDay - 1)));

				if (SpawnManager.Instance.shopCash.isConumeBuff_Honor)
					nHonor *= 2;

				ScoreManager.ScoreInstance.HonorPlus (nHonor);

                //날짜 초기화
                cPlayerData.SetSuccessedGuestCount(0);

                ScoreManager.ScoreInstance.SetSuccessedGuestCount(0);

                //날짜를 1일 추가
                SpawnManager.Instance.SetDayInitInfo (cPlayerData.GetDay () + 1);	
			}
		}

		m_bIsRepair = true;
	}

	public void RepairObjectInputWeapon()
	{
		m_bIsRepair = true;
		m_bIsArrival = true;

		RepairShowObject.GetWeapon (gameObject, weaponData, m_dComplate, m_fTemperator);

		SpeechSelect ((int)E_SPEECH.E_PLAYER);
	}
}
