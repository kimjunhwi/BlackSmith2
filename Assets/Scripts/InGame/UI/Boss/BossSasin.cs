﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BossSasin : BossCharacter 
{
	//1,2 페이즈시 등장하는 해골 관련 변수들
	private RectTransform bossSkullRespawnPoint;				//해골 등장 포인트
	private float fXPos;										//해골 등장 포인트의 x좌표
	private float fYPos;										//해골 등장 포인트의 y좌표
	private float fRandomXPos;									//해골 등장 포인트의 랜덤 x좌표
	private float fRandomYPos;									//해골 등장 포인트의 랜덤 y좌표

	public SimpleObjectPool skullObjectPool;					//1,2페이즈 시에 등장 하는 해골들
	public int nSkullCount = 0;									//현재 나와있는 해골들의 개수

	//임시 해골 변수 
	private GameObject Skull;
	private float fTime = 0f;

	public int nCurLevel = 0;

	public double dBossComplete = 0;



	private void Start()
	{
		skullObjectPool = GameObject.Find ("SkullPool").GetComponent<SimpleObjectPool> ();
		bossSkullRespawnPoint = GameObject.Find ("BossSkullCreateArea2").GetComponent<RectTransform>();
		fXPos = bossSkullRespawnPoint.position.x;
		fYPos = bossSkullRespawnPoint.position.y;

		gameObject.SetActive (false);
		animator = gameObject.GetComponent<Animator> ();

		skullObjectPool.PreloadPool ();
	}

	private void OnEnable()
	{
		if (isFirstActive == false) {
			isFirstActive = true;
		} 
		else {
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
		while (true)
		{
			if (this.bossBackGround.isBossBackGround == true) 
			{
				//배경이 바뀌면 등장 애니메이션 시작
				animator.SetBool ("isBackGroundChanged", true);

				//애니메이션이 끝났는지 확인 끝났으면 다음애니메이션으로 넘긴다.
				//SasinAppear Animation length = 1.0f
				yield return new WaitForSeconds (1.0f);
				animator.SetBool ("isAppear", true);
				eCureentBossState = EBOSS_STATE.PHASE_00;

				if (eCureentBossState == EBOSS_STATE.PHASE_00) 
				{
					float fOriValue = (24 + (nCurLevel * 5));
					float fMinusValue = (Mathf.Floor( (24f + (float)nCurLevel * 5f) * 0.1f ) ) * 10;
					float result = fOriValue - fMinusValue;

					double dCurComplete = (bossInfo.dComplate * Mathf.Pow (2, (Mathf.Floor( Mathf.Max (((44 + (nCurLevel * 5)) * 0.1f), 1))))) * (0.5 + (result) * 0.08f) * 8;
					repairObj.GetBossWeapon (ObjectCashing.Instance.LoadSpriteFromCache (sBossWeaponSprite), dCurComplete, 0, 0, this);


					dBossComplete = dCurComplete;
					//타이머 시작
					ActiveTimer ();
					//보스가 소환되는 도중에 무기 패널을 터치하면 보스 creator가 꺼지므로 소환중일때는 막아놓는다. 
					uiDisable.isBossSummon = false;
					break;
				}

				//yield return new WaitForSeconds(0.1f);

			}
			else
				yield return null;
		}

		//Phase00 Start!
		StartCoroutine (BossSkillStandard ());
	}

	protected override IEnumerator BossSkillStandard ()
	{
		uiManager.AllDisable ();
		bossPanel.SetActive (true);
		//기본페이즈에서 실패시 보스 화난 이펙트가 안뜨게 하기 위한 변수
		isStandardPhaseFailed = true;
		//StandardPhase 말풍선
		bossTalkPanel.StartShowBossTalkWindow (2f, bossWord[(int)E_BOSSWORD.E_BOSSWORD_BEGIN]);
		while (true)
		{
			//해골 생성을 위한 타이머
			fTime += Time.deltaTime;

			double fCurComplete = repairObj.GetCurCompletion ();
			double fMaxComplete = dBossComplete;

			//Fail Condition
			if (fCurComplete < 0) {
				FailState ();
				yield break;
			}

		
			//해골 생성 
			if (fTime >= 2.0f && bossSkullRespawnPoint.childCount != 4) 
				CreateSkull ();
			

			if (fCurComplete >=	(fMaxComplete / 100) * 30)
				eCureentBossState = EBOSS_STATE.PHASE_01;

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
		//Boss Effect On
		bossEffect.ActiveEffect (BOSSEFFECT.BOSSEFFECT_SASINANGRY);
		isStandardPhaseFailed = false;

		//BossWord Start
		bossTalkPanel.StartShowBossTalkWindow (2f, bossWord[(int)E_BOSSWORD.E_BOSSWORD_PHASE01]);
		while (true)
		{
			fRandomXPos = Random.Range (fXPos - (bossSkullRespawnPoint.sizeDelta.x/2), fXPos + (bossSkullRespawnPoint.sizeDelta.x/2));
			fRandomYPos = Random.Range (fYPos - (bossSkullRespawnPoint.sizeDelta.y/2), fYPos + (bossSkullRespawnPoint.sizeDelta.y/2));

			fTime += Time.deltaTime;
			//해골 생성 
			if (fTime >= 2.0f && bossSkullRespawnPoint.childCount != 4) 
				CreateSkull ();
			
			double dCurComplete = repairObj.GetCurCompletion ();
			double dMaxComplete = dBossComplete;

			if (dCurComplete < 0) {
				FailState ();
				yield break;
			}

			if (dCurComplete >=	(dMaxComplete / 100) * 60)
				eCureentBossState = EBOSS_STATE.PHASE_02;

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
		bossTalkPanel.StartShowBossTalkWindow (2f, bossWord[(int)E_BOSSWORD.E_BOSSWORD_PHASE02]);
		while (true)
		{
			fRandomXPos = Random.Range (fXPos - (bossSkullRespawnPoint.sizeDelta.x/2), fXPos + (bossSkullRespawnPoint.sizeDelta.x/2));
			fRandomYPos = Random.Range (fYPos - (bossSkullRespawnPoint.sizeDelta.y/2), fYPos + (bossSkullRespawnPoint.sizeDelta.y/2));

			fTime += Time.deltaTime;
			//해골 생성 
			if (fTime >= 2.0f && bossSkullRespawnPoint.childCount != 4) 
				CreateSkull ();
			
			double dCurComplete = repairObj.GetCurCompletion ();
			double dMaxComplete = dBossComplete;

			if (dCurComplete < 0) {
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
		ActiveTimer ();
		bossTalkPanel.StartShowBossTalkWindow (2f, bossWord[(int)E_BOSSWORD.E_BOSSWORD_END]);
		animator.SetBool ("isDisappear", true);



		yield return new WaitForSeconds (1.0f);
		eCureentBossState = EBOSS_STATE.RESULT;
		if (eCureentBossState == EBOSS_STATE.RESULT) 
		{


			//효과 off
			if (isStandardPhaseFailed == false)
				bossEffect.ActiveEffect (BOSSEFFECT.BOSSEFFECT_SASINANGRY);
			//말풍선 off
			if (bossTalkPanel.bossTalkPanel.activeSelf == true)
				bossTalkPanel.bossTalkPanel.SetActive (false);

			//Animation 초기화
			animator.SetBool ("isAppear", false);
			animator.SetBool ("isDisappear", false);
			animator.SetBool ("isBackGroundChanged", false);	
			animator.Play ("BossIdle");


			//Weapon 터지는 효과
			repairObj.ShowBreakWeapon ();
			repairObj.SetFinishBoss ();									//수리 패널 초기화

		}
		StartCoroutine (BossResult ());
		yield break;
	}

	protected override IEnumerator BossResult ()
	{
		//ChangeSound
		//SoundManager.instance.ChangeBGM(eSoundArray.BGM_BossBattle, eSoundArray.BGM_Main);
		while (true)
		{
			//확인버튼을 누르면 피니쉬로 넘어간다
			if (eCureentBossState == EBOSS_STATE.FINISH) {
				StartCoroutine (BossFinish ());
				yield break;
			}
			//실패가 아닐시
			if (isFailed == false && bossPopUpWindow.isRewardPanelOn_Success == false) 
			{
				Debug.Log ("BossResult : Success");
				bossPopUpWindow.GetBossInfo (this);
				bossPopUpWindow.GetBossLevel (nCurLevel);
				bossPopUpWindow.GetBossIndex (nIndex);
				bossPopUpWindow.SetBossRewardBackGroundImage (isFailed);
				bossPopUpWindow.PopUpWindowReward_Switch_isSuccess ();

				bossPopUpWindow.PopUpWindow_Reward_YesButton.onClick.AddListener (bossPopUpWindow.PopUpWindowReward_Switch_isSuccess);

				//Quest Check
				qusetManager.QuestSuccessCheck (QuestType.E_QUESTTYPE_BOSSSASINSUCCESS, 1);
				qusetManager.QuestSuccessCheck (QuestType.E_QUESTTYPE_ANYBOSSSUCCESS, 1);
			} 
			//실패시
			if(isFailed == true && bossPopUpWindow.isRewardPanelOn_Fail == false)
			{
				Debug.Log ("BossResult : Fail");
				bossPopUpWindow.SetBossRewardBackGroundImage (isFailed);
				bossPopUpWindow.PopUpWindowReward_Switch_isFail ();
				bossPopUpWindow.PopUpWindow_Reward_YesButton.onClick.AddListener (bossPopUpWindow.PopUpWindowReward_Switch_isFail);
			
			}

			
		
			yield return new WaitForSeconds(0.1f);
		}
	


	}

	protected override IEnumerator BossFinish ()
	{
		yield return null;

		Debug.Log ("Boss Sasin Finish");
		//혹시나 도는 코루틴들 종료
		StopCoroutine (BossSkillStandard ());
		StopCoroutine (BossSkill_01 ());
		StopCoroutine (BossSKill_02 ());
		StopCoroutine (BossDie ());
		StopCoroutine (BossResult ());

		//상태 변수 초기화
		isFailed = false;
		isStandardPhaseFailed = false;

		//배경 초기화 
		bossBackGround.StartReturnBossBackGroundToBackGround ();	//배경 초기화

	
		if (bossBackGround.isBossBackGround == true) {
			SpawnManager.Instance.bIsBossCreate = false;			//손님들 재등장
		}
		//UIBloack off
		bossUIDisable.SetActive (false);						

		SpawnManager.Instance.ReliveArbaitBossRepair ();

		//남아 있는 해골 제거
		while (bossSkullRespawnPoint.childCount != 0) 
		{
			GameObject go = bossSkullRespawnPoint.GetChild (0).gameObject;
			skullObjectPool.ReturnObject(go);
		}
		eCureentBossState = EBOSS_STATE.CREATEBOSS;
		gameObject.SetActive (false);
		Debug.Log ("Finish Boss");
	}


	public void ActiveTimer()
	{
		if (bossTimer_Obj.activeSelf == true)
		{
            GuestPanel.SetActive(true);
            bossTimer_Obj.SetActive (false);
			bossTimer = bossTimer_Obj.GetComponent<BossTimer> ();
			bossTimer.StopTimer(0f,0f,(int)E_BOSSNAME.E_BOSSNAME_SASIN);
		}
		else 
		{
            GuestPanel.SetActive(false);
            bossTimer_Obj.SetActive (true);
			bossTimer = bossTimer_Obj.GetComponent<BossTimer> ();
			bossTimer.StartTimer (1f, 30f , (int)E_BOSSNAME.E_BOSSNAME_SASIN);
			bossTimer.bossSasin = this;
		}
	}

	public void FailState()
	{
		isFailed = true;


		StartCoroutine (BossDie ());

	}

	public void CreateSkull()
	{
		Skull = skullObjectPool.GetObject ();
		Skull.transform.SetParent (bossSkullRespawnPoint.transform,false);
		Skull.transform.localScale = Vector3.one;
		Skull.transform.position = new Vector3 (fRandomXPos, fRandomYPos, Skull.transform.position.z);
		Skull.name = "Skull";

		SkullObject skullObj = Skull.GetComponent<SkullObject> ();
		skullObj.skullObjPull = skullObjectPool;
		skullObj.parentTransform = bossSkullRespawnPoint;
		skullObj.fTime = 7f;
		skullObj.repairObj = repairObj;
		skullObj.dCurComplete = dBossComplete;
		fTime = 0f;
	}
}
