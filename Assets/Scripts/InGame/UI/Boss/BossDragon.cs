using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossDragon :  BossCharacter 
{

	public BossDragonDisapperFire bossDisappearFire;

	//SmallFire Pool
	private bool isActivePassiveSkill01 = false;
	private bool isActivePassiveSkill02 = false;

	private const string sTutorialDragonText01 = "크아아ㅏㅏ아ㅏㅏ!!";
	private const string sTutorialDragonText02 = "너의 명성을 듣고 내 무기를 맡기러 왔다.";
	private const string sTutorialDragonText03 = "시간이 없군 얼른 시작하지";
	private const string sTutorialDragonText04 = "....ㅎㅎ";
	private const string sTutorialDragonText05 = "ㅡㅡ";

	//tmpValue

	private float fTime = 0f;

	public int nCurLevel = 0;

	public float fplayerOriginWaterValue = 0f;

	public double dBossComplete = 0;

	private bool isBossDragonBgmOn = false;

	private bool isBossDragonTalkBox01 = false;
	private bool isBossDragonTalkBox02 = false;
	private bool isBossDragonTalkBox03 = false;
	private bool isBossDragonRepairOn = false;
	private bool isBossDragonTalkBox04 = false;		//playerTalk
	private bool isBossDragonTalkBox05 = false;
	private bool isBossDragonTalkBox06 = false;
	private bool isBossDragonBreath = false;
	private bool isBossDragonPlayerTalkBox08 = false;
	private bool isBossDragonFinish = false;

	private int nTimerOverCheck = 0;

	private void Start()
	{
		animator = gameObject.GetComponent<Animator> ();

		gameObject.SetActive (false);
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
		
		while (isBossDragonRepairOn == false)
		{
			
			while (true) 
			{
				//무기 이미지 추가
				if (backGroundScolling.isQuadChangeFinsihed == true) 
				{

					animator.SetBool ("isBackGroundChanged", true);
					if (isBossDragonBgmOn == false) 
					{
						isBossDragonBgmOn = true;
						SoundManager.instance.PlaySound (eSoundArray.ES_BossDragonAppear);
					}
				
					yield return new WaitForSeconds (1.0f);

					if (animator.GetCurrentAnimatorStateInfo (0).IsName ("DragonAppear")) 
					{
			
						
						animator.SetBool ("isAppear", true);
						eCureentBossState = EBOSS_STATE.PHASE_00;
					} else
						yield return null;


					if (eCureentBossState == EBOSS_STATE.PHASE_00)
					{
						

						float fOriValue = (24 + (nCurLevel * 5));
						float fMinusValue = (Mathf.Floor ((24f + (float)nCurLevel * 5f) * 0.1f)) * 10;
						float result = fOriValue - fMinusValue;

						double dCurComplete = (bossInfo.dComplate * Mathf.Pow (2, (Mathf.Floor (Mathf.Max (((64 + (nCurLevel * 5)) * 0.1f), 1))))) * (0.5 + (result) * 0.08f) * 8;
						repairObj.GetBossWeapon (ObjectCashing.Instance.LoadSpriteFromCache (sBossWeaponSprite), dCurComplete, 0, 0, this);


						dBossComplete = dCurComplete;

						break;
					}
				} 
				else
					yield return null;
			}



			//해당 상태이면 계속 체크
			while (SpawnManager.Instance.tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_START_DRAGONTALKBOX01) 
			{
				if (isBossDragonTalkBox01 == false)
				{
					SpawnManager.Instance.tutorialPanel.DragonWepaonBlack_Obj.SetActive (true);
					Debug.Log ("E_TUTORIAL_START_DRAGONTALKBOX01");
					bossTalkPanel.StartShowTutorialBossTalkWindow (2, sTutorialDragonText01);
					isBossDragonTalkBox01 = true;
				}
				yield return null;

			}


			while (SpawnManager.Instance.tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_START_DRAGONTALKBOX02) {
				if (isBossDragonTalkBox02 == false) {
					Debug.Log ("E_TUTORIAL_START_DRAGONTALKBOX02");
					bossTalkPanel.StartShowTutorialBossTalkWindow (2, sTutorialDragonText02);
					isBossDragonTalkBox02 = true;
				}
				yield return null;

			}

			while (SpawnManager.Instance.tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_START_DRAGONTALKBOX03) 
			{
				if (isBossDragonTalkBox03 == false) {
					Debug.Log ("E_TUTORIAL_START_DRAGONTALKBOX03");
					bossTalkPanel.StartShowTutorialBossTalkWindow (2, sTutorialDragonText03);
					isBossDragonTalkBox03 = true;
				}
				yield return null;
			}

	

			while (SpawnManager.Instance.tutorialPanel.eTutorialState == TutorialOrder.E_TUTORAIL_START_DRAGONREPAIR) 
			{
			
				ActiveTimer ();
				uiDisable.isBossSummon = false;
				StartCoroutine (BossSkillStandard ());
				isBossDragonRepairOn = true;
				SpawnManager.Instance.tutorialPanel.DragonWepaonBlack_Obj.SetActive (false);
				break;

			}
	
			yield return null;
		}
		yield return null;
	}

	protected override IEnumerator BossSkillStandard ()
	{
		uiManager.AllDisable ();
		bossPanel.SetActive (true);

		isStandardPhaseFailed = true;
		while (true)
		{
			//fRandomXPos = Random.Range (fXPos - (smallFireRespawnPoint.sizeDelta.x/2), fXPos + (smallFireRespawnPoint.sizeDelta.x/2));
			//fRandomYPos = Random.Range (fYPos - (smallFireRespawnPoint.sizeDelta.y/2), fYPos + (smallFireRespawnPoint.sizeDelta.y/2));

			fTime += Time.deltaTime;

			double dCurComplete = repairObj.GetCurCompletion ();
			double dMaxComplete = dBossComplete;

			//if (fTime >= 2.0f && nCurFireCount < nSmallFireMaxCount && isSmallFireActive == true)
			//	CreateSmallFire ();



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

	//	bossTalkPanel.StartShowBossTalkWindow (2f, bossWord[(int)E_BOSSWORD.E_BOSSWORD_PHASE01]);

		bossEffect.ActiveEffect (BOSSEFFECT.BOSSEFFECT_FIREANGRY);
		isStandardPhaseFailed = false;

		while (true)
		{
			//fRandomXPos = Random.Range (fXPos - (smallFireRespawnPoint.sizeDelta.x/2), fXPos + (smallFireRespawnPoint.sizeDelta.x/2));
			//fRandomYPos = Random.Range (fYPos - (smallFireRespawnPoint.sizeDelta.y/2), fYPos + (smallFireRespawnPoint.sizeDelta.y/2));

			fTime += Time.deltaTime;

			//BossWeapon info
			double dCurComplete = repairObj.GetCurCompletion ();
			double dMaxComplete = dBossComplete;

			//if (fTime >= 1.5f  && nCurFireCount < nSmallFireMaxCount && isSmallFireActive == true )
			//	CreateSmallFire ();


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
		//nSmallFireMaxCount = 20;
	//	bossTalkPanel.StartShowBossTalkWindow (2f,bossWord[(int)E_BOSSWORD.E_BOSSWORD_PHASE02]);
		while (true)
		{
			//fRandomXPos = Random.Range (fXPos - (smallFireRespawnPoint.sizeDelta.x/2), fXPos + (smallFireRespawnPoint.sizeDelta.x/2));
			//fRandomYPos = Random.Range (fYPos - (smallFireRespawnPoint.sizeDelta.y/2), fYPos + (smallFireRespawnPoint.sizeDelta.y/2));

			fTime += Time.deltaTime;

			//GetCompletion
			double dCurComplete = repairObj.GetCurCompletion ();
			double dMaxComplete = dBossComplete;

			//if (fTime >= 1.0f  && nCurFireCount < 20 && isSmallFireActive == true)
			//	CreateSmallFire ();

			//불씨 개수 10개 일시 터진다
			//if (nCurFireCount >= 10)
			//{
			//	bossFireBoom.StartBoolFireSmall ();
			//}

			if (isActivePassiveSkill02 == false)
				GameManager.Instance.player.SetBasicWaterPlus (GameManager.Instance.player.GetBasicWaterPlus() * 0.5f);



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
		
		bossDisappearFire.gameObject.SetActive(true);
		nTimerOverCheck = 1;
		yield return null;
		//isSmallFireActive = false;
		Debug.Log ("Boss Die");
		ActiveTimer ();
		SpawnManager.Instance.tutorialPanel.eTutorialState = TutorialOrder.E_TUTORIAL_END_DRAGONREPAIR;
	
		//Weapon 터지는 효과
		repairObj.ShowBreakWeapon ();
		repairObj.SetFinishBoss ();		//수리 패널 초기화

		while (true) 
		{
			if (SpawnManager.Instance.tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_START_DRAGONBREATH) {
				break;
			}

			while (SpawnManager.Instance.tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_END_DRAGONREPAIR) 
			{
				if (isBossDragonTalkBox04 == false) 
				{
					//SpawnManager.Instance.tutorialPanel.eTutorialState = TutorialOrder.E_TUTORIAL_START_DRAGONTALKBOX04;

					SpawnManager.Instance.tutorialPanel.playerTalk.TalkBoxOnOff (true);
					SpawnManager.Instance.tutorialPanel.eTutorialState = TutorialOrder.E_TUTORIAL_START_PLAYERTALKBOX05;
					SpawnManager.Instance.tutorialPanel.playerTalk.StartPlayerTalk (4);
	
					isBossDragonTalkBox04 = true;
				}
				yield return null;
			}

			while (SpawnManager.Instance.tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_START_DRAGONTALKBOX04) 
			{
				if (isBossDragonTalkBox05 == false) 
				{
					bossTalkPanel.StartShowTutorialBossTalkWindow (2, sTutorialDragonText04);
					isBossDragonTalkBox05 = true;
				}
				yield return null;
			}


			while (SpawnManager.Instance.tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_START_DRAGONTALKBOX05) 
			{
				if (isBossDragonTalkBox06 == false) 
				{
					bossTalkPanel.StartShowTutorialBossTalkWindow (2, sTutorialDragonText05);
					isBossDragonTalkBox06 = true;
				}
				yield return null;
			}

	

			yield return null;
		}
		//사라질때의 말풍선
		//bossTalkPanel.StartShowBossTalkWindow (2f, bossWord[(int)E_BOSSWORD.E_BOSSWORD_END]);



		SoundManager.instance.PlaySound (eSoundArray.ES_BossDragonDisappear);
		//Animator Bool change
		animator.SetBool ("isDisappear", true);

		bossDisappearFire.BossDragonDisappearAnimator.SetBool ("isBossDisapperFire", true);
		yield return new WaitForSeconds (0.75f);
		SpawnManager.Instance.balckSmithSetting.SettingSmith (11);	//집 체인지
		yield return new WaitForSeconds (0.75f);



		//사라지는 애니메이션이 끝날때 까지 기달인다.
		while (true) 
		{
			if (animator.GetCurrentAnimatorStateInfo (0).IsName ("DragonDisappear"))
			{
				yield return new WaitForSeconds (0.1f);


				eCureentBossState = EBOSS_STATE.RESULT;
				if (eCureentBossState == EBOSS_STATE.RESULT) 
				{
					bossDisappearFire.BossDragonDisappearAnimator.SetBool ("isBossDisapperFire", false);
					bossDisappearFire.BossDragonDisappearAnimator.Play ("BossIdle");
					bossDisappearFire.gameObject.SetActive(false);

					//Effect Off
					//if(isStandardPhaseFailed == false)
					//	bossEffect.ActiveEffect (BOSSEFFECT.BOSSEFFECT_FIREANGRY);

					//말풍선 off
					if (bossTalkPanel.bossTalkPanel.activeSelf == true)
						bossTalkPanel.bossTalkPanel.SetActive (false);

					animator.SetBool ("isAppear", false);
					animator.SetBool ("isDisappear", false);
					animator.SetBool ("isBackGroundChanged", false);	
					animator.Play ("BossIdle");

					Debug.Log ("Finish Boss");
					//bossBackGround.StartReturnBossBackGroundToBackGround ();	//배경 초기화
					backGroundScolling.StartChangeBackground(eBackgroundMat.E_BackgroundMat_Main);
					SpawnManager.Instance.bIsBossCreate = false;


				
					break;
				}
			}
			else
				yield return null;
		}


		while (true)
		{
			if (SpawnManager.Instance.tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_START_DAYS)
			{
				StartCoroutine (BossResult ());
				yield break;
			}

			while (SpawnManager.Instance.tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_START_DRAGONBREATH)
			{
				if (isBossDragonBreath == false) 
				{
					
					SpawnManager.Instance.tutorialPanel.eTutorialState = TutorialOrder.E_TUTORIAL_START_PLAYERTALKBOX07;
					SpawnManager.Instance.tutorialPanel.playerTalk.TalkBoxOnOff (true);
					SpawnManager.Instance.tutorialPanel.playerTalk.StartPlayerTalk (6);
			
					isBossDragonBreath = true;
				}
				yield return null;
			}


			while (SpawnManager.Instance.tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_START_PLAYERTALKBOX08)
			{
				if (isBossDragonPlayerTalkBox08 == false)
				{

					SpawnManager.Instance.tutorialPanel.playerTalk.TalkBoxOnOff (true);
					SpawnManager.Instance.tutorialPanel.playerTalk.StartPlayerTalk (7);

					isBossDragonPlayerTalkBox08 = true;
				}
				yield return null;
			}

			yield return null;

		}
			
	}

	protected override IEnumerator BossResult ()
	{
		yield return null;
		Debug.Log ("BossResult");

		eCureentBossState = EBOSS_STATE.FINISH;
		if (eCureentBossState == EBOSS_STATE.FINISH) 
		{
			StartCoroutine (BossFinish ());
			yield break;
		}
		/*
		while (true) 
		{
			//확인버튼을 누르면 피니쉬로 넘어간다


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
			} 
			//실패시
			if (isFailed == true && bossPopUpWindow.isRewardPanelOn_Fail == false) {
				bossPopUpWindow.SetBossRewardBackGroundImage (isFailed);
				bossPopUpWindow.PopUpWindowReward_Switch_isFail ();
				bossPopUpWindow.PopUpWindow_Reward_YesButton.onClick.AddListener (bossPopUpWindow.PopUpWindowReward_Switch_isFail);
			}

			yield return new WaitForSeconds (0.1f);
		}
		*/
	}	


	protected override IEnumerator BossFinish ()
	{
		yield return null;
		Debug.Log ("BossFinish");

		while (true) 
		{
			if(SpawnManager.Instance.tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_START_SHOWCONSTRUCT)
				break;

			if (SpawnManager.Instance.tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_START_DAYS)
			{
				if (isBossDragonFinish == false) 
				{
					//ChangeSound
					SoundManager.instance.ChangeBGM(eSoundArray.BGM_BossBattle, eSoundArray.BGM_Main);
					SpawnManager.Instance.tutorialPanel.eTutorialState = TutorialOrder.E_TUTORIAL_START_DAYS;

					SpawnManager.Instance.tutorialPanel.DeActiveObj.SetActive (true);
					SpawnManager.Instance.tutorialPanel.StartTutorialFullScreenTextPanelAlpha (TutorialOption.E_TUTORIAL_OPTION_FADEIN);
				
					isBossDragonFinish = true;
				}
			}

			yield return null;


		}




		StopCoroutine (BossSkillStandard ());
		StopCoroutine (BossSkill_01 ());
		StopCoroutine (BossSKill_02 ());
		StopCoroutine (BossDie ());
		StopCoroutine (BossResult ());

		//물 충전량 원래대로
		//GameManager.Instance.player.changeStats.fWaterPlus = fplayerOriginWaterValue;
		//GameManager.Instance.player.SetWaterPlus ();
		Debug.Log ("Cur Player WaterPlus :" + GameManager.Instance.player.GetWaterPlus ()); 

		isFailed = false;
		isStandardPhaseFailed = false;
		//nSmallFireMaxCount = 12;
		//nCurFireCount = 0;

		SpawnManager.Instance.bIsBossCreate = false;

		/*
		if (bossBackGround.isBossBackGround == true)
		{
			bossBackGround.isBossBackGround = false;
			bossBackGround.isOriginBackGround = true;
		}
		*/
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
			bossTimer.StopTimer(0f,0f,(int)E_BOSSNAME.E_BOSSNAME_DARAGON);
		}
		else 
		{
			GuestPanel.SetActive(false);
			bossTimer_Obj.SetActive (true);
			bossTimer = bossTimer_Obj.GetComponent<BossTimer> ();
			bossTimer.StartTimer (0f, 30f , (int)E_BOSSNAME.E_BOSSNAME_DARAGON);
			bossTimer.bossDragon = this;
		}

	}

	public void FailState()
	{
		if (nTimerOverCheck == 1)
			return;

		isFailed = true;
	
		//StopCoroutine (BossSkillStandard ());
		//StopCoroutine (BossSkill_01 ());
		//StopCoroutine (BossSKill_02 ());

		StartCoroutine (BossDie ());
	}

}
