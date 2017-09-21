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



	//tmpValue

	private float fTime = 0f;

	public int nCurLevel = 0;

	public float fplayerOriginWaterValue = 0f;

	public double dBossComplete = 0;


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
		while (true)
		{
			//무기 이미지 추가
			if (bossBackGround.isBossBackGround == true) 
			{

				animator.SetBool ("isBackGroundChanged", true);

				yield return new WaitForSeconds (1.0f);

				if (animator.GetCurrentAnimatorStateInfo (0).IsName("DragonAppear")) 
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
		uiManager.AllDisable ();
		bossPanel.SetActive (true);

		bossTalkPanel.StartShowBossTalkWindow (2f, bossWord[(int)E_BOSSWORD.E_BOSSWORD_BEGIN]);
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

		bossTalkPanel.StartShowBossTalkWindow (2f, bossWord[(int)E_BOSSWORD.E_BOSSWORD_PHASE01]);

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
		bossTalkPanel.StartShowBossTalkWindow (2f,bossWord[(int)E_BOSSWORD.E_BOSSWORD_PHASE02]);
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

		yield return null;
		//isSmallFireActive = false;
		Debug.Log ("Boss Die");
		//화면에 남아있는 불씨들을 없앤다
		//while (smallFireRespawnPoint.childCount != 0) 
		//{
		//	GameObject go = smallFireRespawnPoint.GetChild (0).gameObject;
		//	smallFirePool.ReturnObject(go);
		//}

		//사라질때의 말풍선
		bossTalkPanel.StartShowBossTalkWindow (2f, bossWord[(int)E_BOSSWORD.E_BOSSWORD_END]);

	
		//Animator Bool change
		animator.SetBool ("isDisappear", true);

		bossDisappearFire.BossDragonDisappearAnimator.SetBool ("isBossDisapperFire", true);

		yield return new WaitForSeconds (0.6f);

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
					bossBackGround.StartReturnBossBackGroundToBackGround ();	//배경 초기화
					//Weapon 터지는 효과
					repairObj.ShowBreakWeapon ();
					repairObj.SetFinishBoss ();									//수리 패널 초기화

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
		yield return null;
		Debug.Log ("BossResult");

		//Vector3 vector = new Vector3 (gameObject.transform.position.x, gameObject.transform.position.y + 300f, gameObject.transform.position.z);


		//gameObject.transform.position = vector;


		ActiveTimer ();
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

		if (SpawnManager.Instance.tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_START_DRAGONSHOW) {
			SpawnManager.Instance.tutorialPanel.eTutorialState = TutorialOrder.E_TUTORIAL_START_DAYS;

			SpawnManager.Instance.tutorialPanel.DeActiveObj.SetActive (true);
			SpawnManager.Instance.tutorialPanel.StartContent ();
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

		if (bossBackGround.isBossBackGround == true)
		{
			SpawnManager.Instance.bIsBossCreate = false;
			bossBackGround.isBossBackGround = false;
			bossBackGround.isOriginBackGround = true;
		}
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
		isFailed = true;

		//StopCoroutine (BossSkillStandard ());
		//StopCoroutine (BossSkill_01 ());
		//StopCoroutine (BossSKill_02 ());

		StartCoroutine (BossDie ());
	}

}
