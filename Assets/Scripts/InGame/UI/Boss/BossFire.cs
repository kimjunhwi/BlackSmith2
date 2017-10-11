using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFire : BossCharacter 
{
	//SmallFire Pool

	private float fXPos;								//불 등장 포인트의 x좌표
	private float fYPos;								//불 등장 포인트의 y좌표
	private float fRandomXPos;							//불 등장 포인트의 랜덤 x좌표
	private float fRandomYPos;							//불 등장 포인트의 랜덤 y좌표

	public SimpleObjectPool smallFirePool;				//불시 ObjPool
	public RectTransform smallFireRespawnPoint;			//불씨 생성지점

	private int nSmallFireMaxCount;						//작은 불 개수(최대)
	public int nCurFireCount;							//작은 불 개수(현재)

	public BossFireBoom bossFireBoom;
	private bool isSmallFireActive;

	private bool isActivePassiveSkill01 = false;
	private bool isActivePassiveSkill02 = false;

	//tmpValue
	private GameObject smallFire;
	private float fTime = 0f;

	public int nCurLevel = 0;

	public float fplayerOriginWaterValue = 0f;

	public double dBossComplete = 0;

	public float fOriginPlayerWaterValue;


	private void Start()
	{
		fOriginPlayerWaterValue = 0;
		isSmallFireActive = false;
		nSmallFireMaxCount = 5;
		bossFireBoom.bossFire = this;
		bossFireBoom.repairObj = repairObj;
		animator = gameObject.GetComponent<Animator> ();
		fXPos = smallFireRespawnPoint.position.x;
		fYPos = smallFireRespawnPoint.position.y;

		gameObject.SetActive (false);
		smallFirePool.PreloadPool ();
	}
	private void OnEnable()
	{
		if (isFirstActive == false) {
			isFirstActive = true;
		} 
		else 
		{
			fplayerOriginWaterValue = GameManager.Instance.player.GetWaterPlus ();
			eCureentBossState = EBOSS_STATE.CREATEBOSS;
			StartCoroutine (BossWait ());
		}

	}
	private void OnDisable()
	{
		StopCoroutine (BossWait ());
		StopCoroutine (BossSkillStandard ());
		StopCoroutine (BossSkill_01 ());
		StopCoroutine (BossSKill_02 ());
		StopCoroutine (BossDie ());
		StopCoroutine (BossResult ());
	} 
		
	protected override IEnumerator BossWait ()
	{
		fOriginPlayerWaterValue = GameManager.Instance.GetPlayer ().GetWaterPlus ();

		while (true)
		{
			//무기 이미지 추가
			if (backGroundScolling.isQuadChangeFinsihed == true) 
			{

				animator.SetBool ("isBackGroundChanged", true);

				if (animator.GetCurrentAnimatorStateInfo (0).IsName("FireAppear")) 
				{
					//yield return new WaitForSeconds (0.1f);
					animator.SetBool ("isAppear", true);
					eCureentBossState = EBOSS_STATE.PHASE_00;
				} 
				else
					yield return null;


				if (eCureentBossState == EBOSS_STATE.PHASE_00) 
				{
					float fOriValue = (24 + (nCurLevel * 5));
					float fMinusValue = (Mathf.Floor( (24f + (float)nCurLevel * 5f) * 0.1f ) ) * 10;
					float result = fOriValue - fMinusValue;

					double dCurComplete = (bossInfo.dComplate * Mathf.Pow (2, (Mathf.Floor( Mathf.Max (((64 + (nCurLevel * 5)) * 0.1f), 1))))) * (0.5 + (result) * 0.08f) * 8;
					repairObj.GetBossWeapon (ObjectCashing.Instance.LoadSpriteFromCache (sBossWeaponSprite), dCurComplete, 0, 0, this);


					dBossComplete = dCurComplete;
					ActiveTimer ();
					uiDisable.isBossSummon = false;
					break;
				}
			}
			else
				yield return null;


		}
		StartCoroutine (BossSkillStandard ());
		yield break;
	}

	protected override IEnumerator BossSkillStandard ()
	{
		nSmallFireMaxCount = 5;
		uiManager.AllDisable ();
		bossPanel.SetActive (true);
		isSmallFireActive = true;
		
		bossTalkPanel.StartShowBossTalkWindow (2f, bossWord[(int)E_BOSSWORD.E_BOSSWORD_BEGIN]);
		isStandardPhaseFailed = true;
		while (true)
		{
			fRandomXPos = Random.Range (fXPos - (smallFireRespawnPoint.sizeDelta.x/2), fXPos + (smallFireRespawnPoint.sizeDelta.x/2));
			fRandomYPos = Random.Range (fYPos - (smallFireRespawnPoint.sizeDelta.y/2), fYPos + (smallFireRespawnPoint.sizeDelta.y/2));

			fTime += Time.deltaTime;

			double dCurComplete = repairObj.GetCurCompletion ();
			double dMaxComplete = dBossComplete;

			if (fTime >= 2.0f && nCurFireCount < nSmallFireMaxCount && isSmallFireActive == true)
				CreateSmallFire ();
		
			if (dCurComplete < 0) 
			{
				FailState ();
				yield break;
			}

			if (dCurComplete >=	(dMaxComplete / 100) * 30) {
				eCureentBossState = EBOSS_STATE.PHASE_01;
			}
			if (eCureentBossState == EBOSS_STATE.PHASE_01)
				break;
			else
				yield return null;

		}
		StartCoroutine (BossSkill_01 ());
		yield break;
	}


	protected override IEnumerator BossSkill_01 ()
	{
		nSmallFireMaxCount = 5;
		bossTalkPanel.StartShowBossTalkWindow (2f, bossWord[(int)E_BOSSWORD.E_BOSSWORD_PHASE01]);

		bossEffect.ActiveEffect (BOSSEFFECT.BOSSEFFECT_FIREANGRY);
		isStandardPhaseFailed = false;

		while (true)
		{
			fRandomXPos = Random.Range (fXPos - (smallFireRespawnPoint.sizeDelta.x/2), fXPos + (smallFireRespawnPoint.sizeDelta.x/2));
			fRandomYPos = Random.Range (fYPos - (smallFireRespawnPoint.sizeDelta.y/2), fYPos + (smallFireRespawnPoint.sizeDelta.y/2));

			fTime += Time.deltaTime;

			//BossWeapon info
			double dCurComplete = repairObj.GetCurCompletion ();
			double dMaxComplete = dBossComplete;

			if (fTime >= 1.5f  && nCurFireCount < nSmallFireMaxCount && isSmallFireActive == true )
				CreateSmallFire ();

		
			if (dCurComplete < 0) {
				FailState ();
				yield break;
			}

			if (dCurComplete >=	(dMaxComplete / 100) * 60) {
				eCureentBossState = EBOSS_STATE.PHASE_02;
			}
			if (eCureentBossState == EBOSS_STATE.PHASE_02)
				break;
			else
				yield return null;
		}
		StartCoroutine (BossSKill_02 ());
		yield break;

	}

	protected override IEnumerator BossSKill_02 ()
	{
		nSmallFireMaxCount = 5;
		bossTalkPanel.StartShowBossTalkWindow (2f,bossWord[(int)E_BOSSWORD.E_BOSSWORD_PHASE02]);
		while (true)
		{
			fRandomXPos = Random.Range (fXPos - (smallFireRespawnPoint.sizeDelta.x/2), fXPos + (smallFireRespawnPoint.sizeDelta.x/2));
			fRandomYPos = Random.Range (fYPos - (smallFireRespawnPoint.sizeDelta.y/2), fYPos + (smallFireRespawnPoint.sizeDelta.y/2));

			fTime += Time.deltaTime;

			//GetCompletion
			double dCurComplete = repairObj.GetCurCompletion ();
			double dMaxComplete = dBossComplete;

			if (fTime >= 1.0f  && nCurFireCount < nSmallFireMaxCount && isSmallFireActive == true)
				CreateSmallFire ();

			//불씨 개수 5개 일시 터진다
			if (nCurFireCount >= 5)
			{
				bossFireBoom.StartBoolFireSmall ();
			}

			if (isActivePassiveSkill02 == false) 
			{
				isActivePassiveSkill02 = true;
				GameManager.Instance.player.SetBasicWaterPlus (GameManager.Instance.player.GetBasicWaterPlus () * 0.5f);
			}
				
		
			if (dCurComplete < 0) 
			{
				FailState ();
				yield break;
			}

			if (dCurComplete >= dMaxComplete)
				eCureentBossState = EBOSS_STATE.DIE;

			if (eCureentBossState == EBOSS_STATE.DIE)
				break;
			else
				yield return null;

		}
		StartCoroutine (BossDie ());
		yield break;
	}



	protected override IEnumerator BossDie ()
	{
		yield return null;
		isSmallFireActive = false;
		Debug.Log ("Boss Die");
		//화면에 남아있는 불씨들을 없앤다
		while (smallFireRespawnPoint.childCount != 0) 
		{
			GameObject go = smallFireRespawnPoint.GetChild (0).gameObject;
			smallFirePool.ReturnObject(go);
		}

		//사라질때의 말풍선
		if(isFailed == false)
			bossTalkPanel.StartShowBossTalkWindow (2f, bossWord[(int)E_BOSSWORD.E_BOSSWORD_END]);
		else
			bossTalkPanel.StartShowBossTalkWindow (2f, "흥. 기대를 하다니 어리석었군");
		//Animator Bool change
		repairObj.SetFinishBoss ();									//수리 패널 초기화


		animator.SetBool ("isDisappear", true);

	

		//사라지는 애니메이션이 끝날때 까지 기달인다.
		while (true) 
		{
			if (animator.GetCurrentAnimatorStateInfo (0).IsName ("FireDisappear"))
			{
				yield return new WaitForSeconds (0.8f);

			

				eCureentBossState = EBOSS_STATE.RESULT;
				if (eCureentBossState == EBOSS_STATE.RESULT) 
				{
					

					//Effect Off
					if(isStandardPhaseFailed == false)
						bossEffect.ActiveEffect (BOSSEFFECT.BOSSEFFECT_FIREANGRY);

					//말풍선 off
					if (bossTalkPanel.bossTalkPanel.activeSelf == true)
						bossTalkPanel.bossTalkPanel.SetActive (false);

					animator.SetBool ("isAppear", false);
					animator.SetBool ("isDisappear", false);
					animator.SetBool ("isBackGroundChanged", false);	
					animator.Play ("BossIdle");



					Debug.Log ("Finish Boss");
				
					//Weapon 터지는 효과
					repairObj.ShowBreakWeapon ();


					break;
				}
			}
			else
				yield return null;
		}
		StartCoroutine (BossResult ());


	}

	protected override IEnumerator BossResult ()
	{
		//ChangeSound
		//SoundManager.instance.ChangeBGM(eSoundArray.BGM_BossBattle, eSoundArray.BGM_Main);
		ActiveTimer ();

		while (true) 
		{
			//확인버튼을 누르면 피니쉬로 넘어간다
			if (eCureentBossState == EBOSS_STATE.FINISH) 
			{
				StartCoroutine (BossFinish ());
				yield break;
			}

			if (isFailed == false && bossPopUpWindow.isRewardPanelOn_Success == false) {
				bossPopUpWindow.GetBossInfo (this);
				bossPopUpWindow.GetBossLevel (nCurLevel);
				bossPopUpWindow.GetBossIndex (nIndex);
				bossPopUpWindow.SetBossRewardBackGroundImage (isFailed);
				bossPopUpWindow.PopUpWindowReward_Switch_isSuccess ();

				bossPopUpWindow.PopUpWindow_Reward_YesButton.onClick.AddListener (bossPopUpWindow.PopUpWindowReward_Switch_isSuccess);

				//Quest Check
				qusetManager.QuestSuccessCheck (QuestType.E_QUESTTYPE_BOSSFIRESUCCESS, 1);
				qusetManager.QuestSuccessCheck (QuestType.E_QUESTTYPE_ANYBOSSSUCCESS, 1);

				if (GameManager.Instance.cBossPanelListInfo [0].nBossFireMaxLevel <= nCurLevel) 
				{
					GameManager.Instance.cBossPanelListInfo [0].nBossFireMaxLevel = nCurLevel + 1;
					GameManager.Instance.cBossPanelListInfo [0].nBossFireCurLevel = GameManager.Instance.cBossPanelListInfo [0].nBossFireMaxLevel;
					GameManager.Instance.SaveBossPanelInfoList ();
				}
			} 
			//실패시
			if (isFailed == true && bossPopUpWindow.isRewardPanelOn_Fail == false) {
				bossPopUpWindow.SetBossRewardBackGroundImage (isFailed);
				bossPopUpWindow.PopUpWindowReward_Switch_isFail ();
				bossPopUpWindow.PopUpWindow_Reward_YesButton.onClick.AddListener (bossPopUpWindow.PopUpWindowReward_Switch_isFail);
			}

			yield return new WaitForSeconds (0.1f);
		}
	}	


	protected override IEnumerator BossFinish ()
	{
		yield return null;

		StopCoroutine (BossSkillStandard ());
		StopCoroutine (BossSkill_01 ());
		StopCoroutine (BossSKill_02 ());
		StopCoroutine (BossDie ());
		StopCoroutine (BossResult ());

		//물 충전량 원래대로
		GameManager.Instance.player.changeStats.fWaterPlus = fplayerOriginWaterValue;
		GameManager.Instance.player.SetWaterPlus ();
		Debug.Log ("Cur Player WaterPlus :" + GameManager.Instance.player.GetWaterPlus ()); 

		isFailed = false;
		isStandardPhaseFailed = false;
		nSmallFireMaxCount = 12;
		nCurFireCount = 0;

		//bossBackGround.StartReturnBossBackGroundToBackGround ();	//배경 초기화
		backGroundScolling.StartChangeBackground(eBackgroundMat.E_BackgroundMat_Main);

		SpawnManager.Instance.bIsBossCreate = false;
		bossUIDisable.SetActive (false);

		SpawnManager.Instance.ReliveArbaitBossRepair ();



		eCureentBossState = EBOSS_STATE.CREATEBOSS;

		gameObject.SetActive (false);
	
	}




	public void ActiveTimer()
	{


		if (bossTimer_Obj.activeSelf == true)
		{
            GuestPanel.SetActive(true);
            bossTimer_Obj.SetActive (false);
			bossTimer = bossTimer_Obj.GetComponent<BossTimer> ();
			bossTimer.StopTimer(0f,0f,(int)E_BOSSNAME.E_BOSSNAME_FIRE);
		}
		else 
		{
            GuestPanel.SetActive(false);
            bossTimer_Obj.SetActive (true);
			bossTimer = bossTimer_Obj.GetComponent<BossTimer> ();
			bossTimer.StartTimer (1f, 30f , (int)E_BOSSNAME.E_BOSSNAME_FIRE);
			bossTimer.bossFire = this;
		}

	}

	public void FailState()
	{
		isFailed = true;
		StartCoroutine (BossDie ());
	}


	public void CreateSmallFire()
	{
		Debug.Log ("SmallFire Count = " + nCurFireCount);
		smallFire = smallFirePool.GetObject ();
		smallFire.transform.SetParent (smallFireRespawnPoint.transform, false);
		smallFire.transform.localScale = Vector3.one;
		smallFire.transform.position = new Vector3 (fRandomXPos, fRandomYPos, smallFire.transform.position.z);
		smallFire.name = "SmallFireTouch";

		//불씨 하나당 물 충전량 -3%
		//repairObj.fSmallFireMinusWater += 0.03f;
		//불씨 하나당 온도 증가량 10%
		repairObj.fSmallFirePlusTemperatrue += 0.1f;

		BossSmallFireObject smallFireObj = smallFire.GetComponent<BossSmallFireObject> ();
		smallFireObj.smallFireObjPull = smallFirePool;
		smallFireObj.repairObj = repairObj;
		smallFireObj.nTouchCount = 3;
		smallFireObj.parentTransform = smallFireRespawnPoint;
		smallFireObj.bossfire = this;
		smallFireObj.rectTrasform.sizeDelta = new Vector2 (512f, 512f);

		Debug.Log ("CurFireMinusWater : " + repairObj.fSmallFireMinusWater + "/ CurPlusTemperatrue : " +  repairObj.fSmallFirePlusTemperatrue);
		fTime = 0f;
		nCurFireCount++;
	}

}
