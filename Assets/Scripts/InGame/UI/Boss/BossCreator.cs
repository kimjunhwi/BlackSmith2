
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum E_BOSSAPPEARDAYS
{
	E_BOSSAPPEARDAYS_ICE = 30,
	E_BOSSAPPEARDAYS_SASIN = 50,
	E_BOSSAPPEARDAYS_FIRE = 70,
	E_BOSSAPPEARDAYS_MUSIC = 90
}

public enum E_BOSSWORD
{
	E_BOSSWORD_BEGIN = 0,
	E_BOSSWORD_PHASE01,
	E_BOSSWORD_PHASE02,
	E_BOSSWORD_END,
}

public enum E_BOSSNAME 
{
	E_BOSSNAME_ICE = 0,
	E_BOSSNAME_SASIN = 1,
	E_BOSSNAME_FIRE = 2,
	E_BOSSNAME_MUSIC = 3,
	E_BOSSNAME_DARAGON = 4,
}

public class BossCreator : MonoBehaviour
{
	
	private UIManager uiManager;					//보스 소환시 모든 UIDisalbe시 사용
	public GameObject bossRespawnPoint;				//보스 리스폰 지점
	public BossConsumeItemInfo bossConsumeItemInfo; //보스가 소비하는 아이템 정보
	public BossRegenTimer bossRegenTimer;			//보스 도전 횟수 리젠되는 시간
	public BossBackGround bossBackGround;			//보스등장시 바뀌는 배경
	public Scrolling backGroundScolling;

	public BossPopUpWindow bossPopUpWindow;			//보스 결과창
	public GameObject bossUIDisable;				//보스 등장시 아래 UI를 못쓰게 하는 패널
	public GameObject bossTimer_Obj;				//보스 시간 Obj
	public GameObject bossPanel;					//보스 리스트
	public BossTalkPanel bossTalkPanel;				//보스 말풍선
	public GameObject bossWeapon_Obj;				//보스 무기 Obj
	public UIDisable uiDisable;
	public GameObject GuestPanel; 					//손님정보 
	public GameObject bossInivteReFillButton_Obj;	//보스 초대장 리필 버튼

	private BossTimer bossTimer;

	public GameObject[] bossList;					//보스 리스트
	public BossElement[] bossElementList;			//보스 리스트 원소
	public BossEffect bossEffect;					//보스 이펙트

	//보스정보
	public Sprite[] bossInfoSprite;
	public GameObject bossHint_Obj;

	//각각의 보스 남은 도전 횟수
	public int nBossSasinLeftCount = 3;
	public int nBossIceLeftCount = 3;
	public int nBossFireLeftCount = 3;
	public int nBossMusicLeftCount = 3;

	public int nBossMaxLeftCount = 3;

	//각보스의 현재 레벨의 체력
	public double dBossIceCompleteValue =0;
	public double dBossSasinCompleteValue =0;
	public double dBossFireCompleteValue =0;
	public double dBossMusicCompleteValue =0;

	//보스와 처음 싸울때 확인 
	private bool m_bIsFirstFightToIceBoss;
	private bool m_bIsFirstFightToSasinBoss;
	private bool m_bIsFirstFightToFireBoss;
	private bool m_bIsFirstFightToMusicBoss;

	public QusetManager questManager;

	private int nBossIndex =0; 

	public bool bIsFirstActive = false;

	void Awake()
	{
		uiManager = FindObjectOfType<UIManager> ();
		bossTimer = bossTimer_Obj.GetComponent<BossTimer> ();
		bossUIDisable.SetActive (false);
	}

	public void BossPanelSetUp()
	{
		uiManager.uiBossFirstFightMark.SetActive (false);

		if (GameManager.Instance.cBossPanelListInfo [0].nBossInviteMentCount < 5)
			bossInivteReFillButton_Obj.SetActive (true);
		else
			bossInivteReFillButton_Obj.SetActive (false);
		
		if (bossConsumeItemInfo.bossCreator == null)
			bossConsumeItemInfo.bossCreator = this;
		if (bossRegenTimer.bossCreator == null)
			bossRegenTimer.bossCreator = this;

		//결과창에 쓰임
		if (bossPopUpWindow.bossIce == null)
			bossPopUpWindow.bossIce = bossList[0].GetComponent<BossIce>() ;
	
		if (bossPopUpWindow.bossSasin == null)
			bossPopUpWindow.bossSasin =  bossList[1].GetComponent<BossSasin>();

	
		if (bossPopUpWindow.bossFire == null)
			bossPopUpWindow.bossFire =bossList[2].GetComponent<BossFire>();

		if (bossPopUpWindow.bossMusic == null)
			bossPopUpWindow.bossMusic = bossList[3].GetComponent<BossMusic>();

	

		bossPopUpWindow.PopUpWindow_YesNo_NoButton.onClick.RemoveListener (bossPopUpWindow.PopUpWindowYesNo_Switch);
		bossPopUpWindow.PopUpWindow_YesNo_YesButton.onClick.RemoveListener (bossPopUpWindow.PopUpWindowYesNo_Switch);


		bossPopUpWindow.PopUpWindow_YesNo_NoButton.onClick.AddListener (bossPopUpWindow.PopUpWindowYesNo_Switch);
		bossPopUpWindow.PopUpWindow_YesNo_YesButton.onClick.AddListener (bossPopUpWindow.PopUpWindowYesNo_Switch);


		if (GameManager.Instance.cBossPanelListInfo[0].isSaved == true) 
		{
			Debug.Log ("Load Saved Info");
			bIsFirstActive = true;
			bossElementList[0].BossLeftCount_Text.text = string.Format("{0} / {1}", GameManager.Instance.cBossPanelListInfo[0].nBossIceLeftCount, nBossMaxLeftCount);
			bossElementList[0].bossLevel_Text.text = string.Format ("Lv {0}", GameManager.Instance.cBossPanelListInfo[0].nBossIceCurLevel);
			bossElementList [0].curLevel = GameManager.Instance.cBossPanelListInfo [0].nBossIceCurLevel;
			bossElementList[0].maxLevel = GameManager.Instance.cBossPanelListInfo [0].nBossIceMaxLevel;

			//보스 최대 레벨에 따른 화살표 표시
			if (bossElementList [0].curLevel >= bossElementList [0].maxLevel)
				bossElementList [0].bossLevelRight_Button.gameObject.SetActive (false);
			else
				bossElementList [0].bossLevelRight_Button.gameObject.SetActive (true);

			bossElementList [0].ShowBossHealth ();

			bossElementList[1].BossLeftCount_Text.text = string.Format("{0} / {1}",  GameManager.Instance.cBossPanelListInfo[0].nBossSasinLeftCount, nBossMaxLeftCount);
			bossElementList[1].bossLevel_Text.text = string.Format ("Lv {0}", GameManager.Instance.cBossPanelListInfo[0].nBossSasinCurLevel);
			bossElementList [1].curLevel = GameManager.Instance.cBossPanelListInfo [0].nBossSasinCurLevel;
			bossElementList[1].maxLevel = GameManager.Instance.cBossPanelListInfo [0].nBossSasinMaxLevel;

			//보스 최대 레벨에 따른 화살표 표시
			if (bossElementList [1].curLevel >= bossElementList [1].maxLevel)
				bossElementList [1].bossLevelRight_Button.gameObject.SetActive (false);
			else
				bossElementList [1].bossLevelRight_Button.gameObject.SetActive (true);

			bossElementList [1].ShowBossHealth ();

			bossElementList[2].BossLeftCount_Text.text = string.Format("{0} / {1}",  GameManager.Instance.cBossPanelListInfo[0].nBossFireLeftCount, nBossMaxLeftCount);
			bossElementList[2].bossLevel_Text.text = string.Format ("Lv {0}", GameManager.Instance.cBossPanelListInfo[0].nBossFireCurLevel);
			bossElementList [2].curLevel = GameManager.Instance.cBossPanelListInfo [0].nBossFireCurLevel;
			bossElementList[2].maxLevel = GameManager.Instance.cBossPanelListInfo [0].nBossFireMaxLevel;
			//보스 최대 레벨에 따른 화살표 표시
			if (bossElementList [2].curLevel >= bossElementList [2].maxLevel)
				bossElementList [2].bossLevelRight_Button.gameObject.SetActive (false);
			else
				bossElementList [2].bossLevelRight_Button.gameObject.SetActive (true);
			bossElementList [2].ShowBossHealth ();

			bossElementList[3].BossLeftCount_Text.text = string.Format("{0} / {1}",  GameManager.Instance.cBossPanelListInfo[0].nBossMusicLeftCount, nBossMaxLeftCount);
			bossElementList[3].bossLevel_Text.text = string.Format ("Lv {0}", GameManager.Instance.cBossPanelListInfo[0].nBossMusicCurLevel);
			bossElementList [3].curLevel = GameManager.Instance.cBossPanelListInfo [0].nBossMusicCurLevel;
			bossElementList[3].maxLevel = GameManager.Instance.cBossPanelListInfo [0].nBossMusicMaxLevel;

			//보스 최대 레벨에 따른 화살표 표시
			if (bossElementList [3].curLevel >= bossElementList [3].maxLevel)
				bossElementList [3].bossLevelRight_Button.gameObject.SetActive (false);
			else
				bossElementList [3].bossLevelRight_Button.gameObject.SetActive (true);
			bossElementList [3].ShowBossHealth ();

			bossConsumeItemInfo.nInviteMentCurCount = GameManager.Instance.cBossPanelListInfo [0].nBossInviteMentCount;
			bossConsumeItemInfo.inviteMentCount_Text.text = string.Format ("{0}/{1}" ,bossConsumeItemInfo.nInviteMentCurCount, bossConsumeItemInfo.nInviteMentMaxCount);

			bossConsumeItemInfo.nPotionCount = GameManager.Instance.cBossPanelListInfo [0].nBossPotionCount;
			bossConsumeItemInfo.positionCount_Text.text = string.Format ("{0}",GameManager.Instance.cBossPanelListInfo[0].nBossPotionCount );

			//보스 도전 횟수(개개인) 이 다 됬을시 충전 버튼 활성화
			if (GameManager.Instance.cBossPanelListInfo [0].nBossIceLeftCount <= 0)
				bossElementList [0].ReloadButton_Obj.SetActive (true);
			else
				bossElementList [0].ReloadButton_Obj.SetActive (false);
			
			if (GameManager.Instance.cBossPanelListInfo [0].nBossSasinLeftCount <= 0)
				bossElementList [1].ReloadButton_Obj.SetActive (true);
			else
				bossElementList [1].ReloadButton_Obj.SetActive (false);
			
			if (GameManager.Instance.cBossPanelListInfo [0].nBossFireLeftCount <= 0)
				bossElementList [2].ReloadButton_Obj.SetActive (true);
			else
				bossElementList [2].ReloadButton_Obj.SetActive (false);

			if (GameManager.Instance.cBossPanelListInfo [0].nBossMusicLeftCount <= 0)
				bossElementList [3].ReloadButton_Obj.SetActive (true);
			else
				bossElementList [3].ReloadButton_Obj.SetActive (false);
			
			nBossSasinLeftCount = GameManager.Instance.cBossPanelListInfo [0].nBossSasinLeftCount;
			nBossIceLeftCount = GameManager.Instance.cBossPanelListInfo [0].nBossIceLeftCount;
			nBossFireLeftCount = GameManager.Instance.cBossPanelListInfo [0].nBossFireLeftCount;
			nBossMusicLeftCount = GameManager.Instance.cBossPanelListInfo [0].nBossMusicLeftCount;

			m_bIsFirstFightToIceBoss = GameManager.Instance.cBossPanelListInfo [0].isFirstFightToIceBoss;
			m_bIsFirstFightToSasinBoss = GameManager.Instance.cBossPanelListInfo [0].isFirstFightToSasinBoss;
			m_bIsFirstFightToFireBoss = GameManager.Instance.cBossPanelListInfo [0].isFirstFightToFireBoss;
			m_bIsFirstFightToMusicBoss = GameManager.Instance.cBossPanelListInfo [0].isFirstFightToMusicBoss;

		}
		else
		{
			Debug.Log ("Load Init Info");
			bIsFirstActive = true;
			bossElementList[0].BossLeftCount_Text.text = string.Format("{0} / {1}", GameManager.Instance.cBossPanelListInfo[0].nBossIceLeftCount, nBossMaxLeftCount);
			bossElementList[0].bossLevel_Text.text = string.Format ("Lv {0}", GameManager.Instance.cBossPanelListInfo[0].nBossIceCurLevel);
			bossElementList [0].curLevel = GameManager.Instance.cBossPanelListInfo [0].nBossIceCurLevel;
			bossElementList [0].maxLevel = GameManager.Instance.cBossPanelListInfo [0].nBossIceMaxLevel;
			bossElementList [0].bossLevelRight_Button.gameObject.SetActive (false);
			bossElementList [0].ShowBossHealth ();



			bossElementList[1].BossLeftCount_Text.text = string.Format("{0} / {1}",  GameManager.Instance.cBossPanelListInfo[0].nBossSasinLeftCount, nBossMaxLeftCount);
			bossElementList[1].bossLevel_Text.text = string.Format ("Lv {0}", GameManager.Instance.cBossPanelListInfo[0].nBossSasinCurLevel);
			bossElementList [1].curLevel = GameManager.Instance.cBossPanelListInfo [0].nBossSasinCurLevel;
			bossElementList [1].maxLevel = GameManager.Instance.cBossPanelListInfo [0].nBossSasinMaxLevel;
			bossElementList [1].bossLevelRight_Button.gameObject.SetActive (false);
			bossElementList [1].ShowBossHealth ();

			bossElementList[2].BossLeftCount_Text.text = string.Format("{0} / {1}",  GameManager.Instance.cBossPanelListInfo[0].nBossFireLeftCount, nBossMaxLeftCount);
			bossElementList[2].bossLevel_Text.text = string.Format ("Lv {0}", GameManager.Instance.cBossPanelListInfo[0].nBossFireCurLevel);
			bossElementList [2].curLevel = GameManager.Instance.cBossPanelListInfo [0].nBossFireCurLevel;
			bossElementList [2].maxLevel = GameManager.Instance.cBossPanelListInfo [0].nBossFireMaxLevel;
			bossElementList [2].bossLevelRight_Button.gameObject.SetActive (false);
			bossElementList [1].ShowBossHealth ();

			bossElementList[3].BossLeftCount_Text.text = string.Format("{0} / {1}",  GameManager.Instance.cBossPanelListInfo[0].nBossMusicLeftCount, nBossMaxLeftCount);
			bossElementList[3].bossLevel_Text.text = string.Format ("Lv {0}", GameManager.Instance.cBossPanelListInfo[0].nBossMusicCurLevel);
			bossElementList [3].curLevel = GameManager.Instance.cBossPanelListInfo [0].nBossMusicCurLevel;
			bossElementList [3].maxLevel = GameManager.Instance.cBossPanelListInfo [0].nBossMusicMaxLevel;
			bossElementList [3].bossLevelRight_Button.gameObject.SetActive (false);
			bossElementList [3].ShowBossHealth ();


			bossConsumeItemInfo.nInviteMentCurCount = bossConsumeItemInfo.nInviteMentMaxCount;
			bossConsumeItemInfo.inviteMentCount_Text.text = string.Format ("{0}/{1}" ,bossConsumeItemInfo.nInviteMentCurCount, bossConsumeItemInfo.nInviteMentMaxCount);

			bossConsumeItemInfo.nPotionCount = GameManager.Instance.cBossPanelListInfo [0].nBossPotionCount;
			bossConsumeItemInfo.positionCount_Text.text = string.Format ("{0}",GameManager.Instance.cBossPanelListInfo[0].nBossPotionCount);

			nBossSasinLeftCount = GameManager.Instance.cBossPanelListInfo [0].nBossSasinLeftCount;
			nBossIceLeftCount = GameManager.Instance.cBossPanelListInfo [0].nBossIceLeftCount;
			nBossFireLeftCount = GameManager.Instance.cBossPanelListInfo [0].nBossFireLeftCount;
			nBossMusicLeftCount = GameManager.Instance.cBossPanelListInfo [0].nBossMusicLeftCount;

			m_bIsFirstFightToIceBoss = GameManager.Instance.cBossPanelListInfo [0].isFirstFightToIceBoss;
			m_bIsFirstFightToSasinBoss = GameManager.Instance.cBossPanelListInfo [0].isFirstFightToSasinBoss;
			m_bIsFirstFightToFireBoss = GameManager.Instance.cBossPanelListInfo [0].isFirstFightToFireBoss;
			m_bIsFirstFightToMusicBoss = GameManager.Instance.cBossPanelListInfo [0].isFirstFightToMusicBoss;

		}

	}


	public void BossCreateInit()
	{
		Debug.Log ("보스 CreateInit");
		uiDisable.isBossSummon = true;

		//ChangeSound
		SoundManager.instance.ChangeBGM(eSoundArray.BGM_Main, eSoundArray.BGM_BossBattle);

		//bossWeaponBlock_Obj.SetActive (true);
		SpawnManager.Instance.cameraShake.Shake (0.1f, 1.0f);

		SpawnManager.Instance.AllCharacterComplate ();

		//보스 선택 창
		bossPanel.SetActive (false);
		bossUIDisable.SetActive (true);
		nBossIndex = bossPopUpWindow.nBossIndex;


		while (SpawnManager.Instance.bIsBossCreate == false)
		{
			Debug.Log ("손님들 들어가는중...");
		}
	
		StartBossCreateInit ();
	

	}
	public void StartBossCreateInit()
	{
		StartCoroutine (BossCreate (nBossIndex));
	}


	public IEnumerator BossCreate(int _index) 
	{
		yield return null;



		if(bossConsumeItemInfo.nInviteMentCurCount != 0)
			bossConsumeItemInfo.nInviteMentCurCount--;

		SpawnManager.Instance.ApplyArbaitBossRepair ();

		if (_index == (int)E_BOSSNAME.E_BOSSNAME_ICE) 
		{
			//보스 리스트에서 해당 보스의 정보와 보스를 셋팅 한다.
			BossIce bossIce = bossList[0].GetComponent<BossIce> ();

			bossIce.nIndex = _index;
			bossIce.bossInfo = GameManager.Instance.bossInfo [_index];
			bossIce.bossEffect = bossEffect;
			bossIce.bossBackGround = bossBackGround;
			bossIce.backGroundScolling = backGroundScolling;
			bossIce.bossPopUpWindow = bossPopUpWindow;
			bossIce.sBossWeaponSprite = "Weapons/Boss/BOSS1_Weapon";
			bossIce.bossTimer_Obj = bossTimer_Obj;
			bossIce.bossUIDisable = bossUIDisable;
			bossIce.bossTalkPanel = bossTalkPanel;
			bossIce.bossWeapon = bossWeapon_Obj;
			bossIce.uiDisable = uiDisable;
			bossIce.uiManager = uiManager;
			bossIce.bossPanel = bossPanel;
			bossIce.nCurLevel = bossElementList [0].curLevel;
			bossIce.GuestPanel = GuestPanel;
			bossIce.qusetManager = questManager;

			bossIce.bossWord [(int)E_BOSSWORD.E_BOSSWORD_BEGIN] = "여자친구를 조각해줘...";
			bossIce.bossWord [(int)E_BOSSWORD.E_BOSSWORD_PHASE01] = "날이 좀 추워진 거 같지 않아?";
			bossIce.bossWord [(int)E_BOSSWORD.E_BOSSWORD_PHASE02] = "나도 이젠 솔로 탈출인거임?";
			bossIce.bossWord [(int)E_BOSSWORD.E_BOSSWORD_END] = "고마워!!";

			if (GameManager.Instance.cBossPanelListInfo [0].isFirstFightToIceBoss == false )
			{
				m_bIsFirstFightToIceBoss = true;
				StartCoroutine (StartShowBossHint (0));
				uiManager.uiBossFirstFightMark.SetActive (false);
			} 

			else 
			{
				//bossBackGround.StartChangeBackGroundToBossBackGround ();
				backGroundScolling.StartChangeBackground(eBackgroundMat.E_BackgroundMat_Boss);
				bossList [_index].SetActive (true);
			}
		
			nBossIceLeftCount--;
			//사
		}

		else if (_index == (int)E_BOSSNAME.E_BOSSNAME_SASIN) 
		{
			BossSasin bossSasin = bossList[1].GetComponent<BossSasin> ();
		
			bossSasin.nIndex = _index;
			bossSasin.bossInfo = GameManager.Instance.bossInfo [_index];
			bossSasin.bossEffect = bossEffect;
			bossSasin.bossBackGround = bossBackGround;
			bossSasin.backGroundScolling = backGroundScolling;
			bossSasin.bossPopUpWindow = bossPopUpWindow;
			bossSasin.sBossWeaponSprite = "Weapons/Boss/BOSS1_Weapon";
			bossSasin.bossTimer_Obj = bossTimer_Obj;
			bossSasin.bossTimer = bossTimer;
			bossSasin.bossUIDisable = bossUIDisable;
			bossSasin.bossTalkPanel = bossTalkPanel;
			bossSasin.bossWeapon = bossWeapon_Obj;
			bossSasin.uiDisable = uiDisable;
			bossSasin.uiManager = uiManager;
			bossSasin.bossPanel = bossPanel;
			bossSasin.nCurLevel = bossElementList [1].curLevel;
			bossSasin.GuestPanel = GuestPanel;
			bossSasin.qusetManager = questManager;

			bossSasin.bossWord [(int)E_BOSSWORD.E_BOSSWORD_BEGIN] = "시련을 시작하지...";
			bossSasin.bossWord [(int)E_BOSSWORD.E_BOSSWORD_PHASE01] =  "지금부터 시작이다!!";
			bossSasin.bossWord [(int)E_BOSSWORD.E_BOSSWORD_PHASE02] = "꽤 잘 버티는군";
			bossSasin.bossWord [(int)E_BOSSWORD.E_BOSSWORD_END] = "인간치곤 대단하군";

			if (GameManager.Instance.cBossPanelListInfo [0].isFirstFightToSasinBoss == false)
			{
				m_bIsFirstFightToSasinBoss = true; 
				StartCoroutine (StartShowBossHint (1));
				uiManager.uiBossFirstFightMark.SetActive (false);
			}
			else 
			{
				//backGroundScolling.StartChangeBackground(eBackgroundMat.E_BackgroundMat_Boss);
				backGroundScolling.StartChangeBackground(eBackgroundMat.E_BackgroundMat_Boss);
				bossList [_index].SetActive (true);
			}
			//bossList [1].SetActive (true);

			nBossSasinLeftCount--;
		}

		else if (_index == (int)E_BOSSNAME.E_BOSSNAME_FIRE) 
		{
			Debug.Log ("Fire Created!!!");	

			BossFire bossFire = bossList[2].GetComponent<BossFire> ();

			bossFire.nIndex = _index;
			bossFire.bossInfo = GameManager.Instance.bossInfo [_index];
			bossFire.bossEffect = bossEffect;
			bossFire.bossBackGround = bossBackGround;
			bossFire.backGroundScolling = backGroundScolling;
			bossFire.bossPopUpWindow = bossPopUpWindow;
			bossFire.sBossWeaponSprite = "Weapons/Boss/deathnote";
			bossFire.bossTimer_Obj = bossTimer_Obj;
			bossFire.bossTimer = bossTimer;
			bossFire.bossUIDisable = bossUIDisable;
			bossFire.bossTalkPanel = bossTalkPanel;
			bossFire.bossWeapon = bossWeapon_Obj;
			bossFire.uiDisable = uiDisable;
			bossFire.uiManager = uiManager;
			bossFire.bossPanel = bossPanel;
			bossFire.nCurLevel = bossElementList [2].curLevel;
			bossFire.GuestPanel = GuestPanel;
			bossFire.qusetManager = questManager;

			bossFire.bossWord [(int)E_BOSSWORD.E_BOSSWORD_BEGIN] = "왕관을 고칠 기회를 주지!";
			bossFire.bossWord [(int)E_BOSSWORD.E_BOSSWORD_PHASE01] = "벌써 지친건가?";
			bossFire.bossWord [(int)E_BOSSWORD.E_BOSSWORD_PHASE02] = "고작 이정도로 힘들어 하다니";
			bossFire.bossWord [(int)E_BOSSWORD.E_BOSSWORD_END] = "인간치곤 괜찮은 실력이었다.";

			if (GameManager.Instance.cBossPanelListInfo [0].isFirstFightToFireBoss == false)
			{
				m_bIsFirstFightToFireBoss = true;
				StartCoroutine (StartShowBossHint (2));
				uiManager.uiBossFirstFightMark.SetActive (false);
			}
			else 
			{
				//bossBackGround.StartChangeBackGroundToBossBackGround ();
				backGroundScolling.StartChangeBackground(eBackgroundMat.E_BackgroundMat_Boss);
				bossList [_index].SetActive (true);
			}

			//bossList [2].SetActive (true);


			nBossFireLeftCount--;

		
		}

		else if (_index == (int)E_BOSSNAME.E_BOSSNAME_MUSIC) 
		{
			BossMusic bossMusic = bossList[3].GetComponent<BossMusic> ();

	

			bossMusic.nIndex = _index;
			bossMusic.bossInfo = GameManager.Instance.bossInfo [_index];
			bossMusic.bossEffect = bossEffect;
			bossMusic.bossBackGround = bossBackGround;
			bossMusic.backGroundScolling = backGroundScolling;
			bossMusic.bossPopUpWindow = bossPopUpWindow;
			bossMusic.sBossWeaponSprite = "Weapons/Boss/deathnote";
			bossMusic.bossTimer_Obj = bossTimer_Obj;
			bossMusic.bossUIDisable = bossUIDisable;
			bossMusic.bossTalkPanel = bossTalkPanel;
			bossMusic.bossWeapon = bossWeapon_Obj;
			bossMusic.uiDisable = uiDisable;
			bossMusic.uiManager = uiManager;
			bossMusic.bossPanel = bossPanel;
			bossMusic.nCurLevel = bossElementList [3].curLevel;
			bossMusic.GuestPanel = GuestPanel;
			bossMusic.qusetManager = questManager;

			bossMusic.bossWord [(int)E_BOSSWORD.E_BOSSWORD_BEGIN] = "흥이 나질않아..";
			bossMusic.bossWord [(int)E_BOSSWORD.E_BOSSWORD_PHASE01] = "바로 그거야!!";
			bossMusic.bossWord [(int)E_BOSSWORD.E_BOSSWORD_PHASE02] = "제대로 놀아보자!!";
			bossMusic.bossWord [(int)E_BOSSWORD.E_BOSSWORD_END] = "휴우 좋은 파티였어 브로~!";

			if (GameManager.Instance.cBossPanelListInfo [0].isFirstFightToMusicBoss == false)
			{
				m_bIsFirstFightToMusicBoss = true;
				StartCoroutine (StartShowBossHint (3));
				uiManager.uiBossFirstFightMark.SetActive (false);
			}
			else 
			{
				backGroundScolling.StartChangeBackground(eBackgroundMat.E_BackgroundMat_Boss);
				bossList [_index].SetActive (true);
			}

			nBossMusicLeftCount--;


		}


		else if (_index == (int)E_BOSSNAME.E_BOSSNAME_DARAGON) 
		{
			BossDragon bossDragon = bossList[4].GetComponent<BossDragon> ();



			bossDragon.nIndex = _index;
			bossDragon.bossInfo = GameManager.Instance.bossInfo [4];
			bossDragon.bossEffect = bossEffect;
			bossDragon.bossBackGround = bossBackGround;
			bossDragon.bossPopUpWindow = bossPopUpWindow;
			bossDragon.backGroundScolling = backGroundScolling;
			bossDragon.sBossWeaponSprite ="";
			bossDragon.bossTimer_Obj = bossTimer_Obj;
			bossDragon.bossUIDisable = bossUIDisable;
			bossDragon.bossTalkPanel = bossTalkPanel;
			bossDragon.bossWeapon = bossWeapon_Obj;
			bossDragon.uiDisable = uiDisable;
			bossDragon.uiManager = uiManager;
			bossDragon.bossPanel = bossPanel;
			bossDragon.nCurLevel = bossElementList [3].curLevel;
			bossDragon.GuestPanel = GuestPanel;
			bossDragon.qusetManager = questManager;

			bossDragon.bossWord [(int)E_BOSSWORD.E_BOSSWORD_BEGIN] = "소리 질러~!";
			bossDragon.bossWord [(int)E_BOSSWORD.E_BOSSWORD_PHASE01] = "Whoh~";
			bossDragon.bossWord [(int)E_BOSSWORD.E_BOSSWORD_PHASE02] = "Drop the beat~!";
			bossDragon.bossWord [(int)E_BOSSWORD.E_BOSSWORD_END] = "SeeYa!";

			//bossBackGround.StartChangeBackGroundToBossBackGround ();
			backGroundScolling.StartChangeBackground(eBackgroundMat.E_BackgroundMat_Boss);
			bossList [_index].SetActive (true);


			//Dragon Save Pass
			yield break;

		}
		bossConsumeItemInfo.BossInviteMentSaveTime ();
		BossPanelInfoSave ();
	}

	public void BossPanelInfoSave()
	{
		Debug.Log ("Save BossPanel Info!! " + bossConsumeItemInfo.nInviteMentCurCount );
		GameManager.Instance.cBossPanelListInfo [0].isSaved = true;
		//현재 남은 보스 (개별)도전횟수 
		GameManager.Instance.cBossPanelListInfo [0].nBossMusicLeftCount = nBossMusicLeftCount;
		GameManager.Instance.cBossPanelListInfo [0].nBossFireLeftCount = nBossFireLeftCount;
		GameManager.Instance.cBossPanelListInfo [0].nBossSasinLeftCount = nBossSasinLeftCount;
		GameManager.Instance.cBossPanelListInfo [0].nBossIceLeftCount = nBossIceLeftCount;
		GameManager.Instance.cBossPanelListInfo [0].nBossInviteMentCount = 	bossConsumeItemInfo.nInviteMentCurCount;
		//현재 남은 보스 포션 개수
		GameManager.Instance.cBossPanelListInfo [0].nBossPotionCount = bossConsumeItemInfo.nPotionCount;
		//현재 보스 (개별)레벨
		GameManager.Instance.cBossPanelListInfo [0].nBossIceCurLevel = bossElementList [0].curLevel;
		GameManager.Instance.cBossPanelListInfo [0].nBossSasinCurLevel = bossElementList [1].curLevel;
		GameManager.Instance.cBossPanelListInfo [0].nBossFireCurLevel = bossElementList [2].curLevel;
		GameManager.Instance.cBossPanelListInfo [0].nBossMusicCurLevel = bossElementList [3].curLevel;
		//현재 도전 할수 있는 보스 최대 레벨
	
		GameManager.Instance.cBossPanelListInfo [0].nBossIceMaxLevel = bossElementList [0].maxLevel;
		GameManager.Instance.cBossPanelListInfo [0].nBossSasinMaxLevel = bossElementList [1].maxLevel;
		GameManager.Instance.cBossPanelListInfo [0].nBossFireMaxLevel = bossElementList [2].maxLevel;
		GameManager.Instance.cBossPanelListInfo [0].nBossMusicMaxLevel = bossElementList [3].maxLevel;


		//현재 보스 첫 싸움을 했는지 아닌지
		GameManager.Instance.cBossPanelListInfo [0].isFirstFightToIceBoss = m_bIsFirstFightToIceBoss;
		GameManager.Instance.cBossPanelListInfo [0].isFirstFightToSasinBoss = m_bIsFirstFightToSasinBoss;
		GameManager.Instance.cBossPanelListInfo [0].isFirstFightToFireBoss = m_bIsFirstFightToFireBoss;
		GameManager.Instance.cBossPanelListInfo [0].isFirstFightToMusicBoss = m_bIsFirstFightToMusicBoss;
		//보스 초대장 시간과 리젠 시간
		bossConsumeItemInfo.BossInviteMentSaveTime ();
		bossRegenTimer.BossRegenTimeSave ();

		//GameManager.Instance.playerData = GameManager.Instance.player.changeStats;
		//GameManager.Instance.SavePlayerData ();			//Local Save
		//GameManager.Instance.GetPlayerSaveList ();		//Confirm
		//GameManager.Instance.SaveBossPanelInfoList();	//SaveBossPanel;


		//GameManager.Instance.isGoogleClounSave = true;
		//GameManager.Instance.LoadData ();				//cloud Save
	}

	public void BossChanllengeCountToMax()
	{
		nBossMusicLeftCount = nBossMaxLeftCount;
		nBossIceLeftCount = nBossMaxLeftCount;
		nBossFireLeftCount = nBossMaxLeftCount;
		nBossSasinLeftCount = nBossMaxLeftCount;
	}

	public IEnumerator StartShowBossHint(int _index)
	{
		BossHint bossHint = bossHint_Obj.GetComponent<BossHint> ();
		bossElementList [_index].ShowBossInfo (_index);
		bossHint.m_isCheckBossHint = false;
		while (bossHint.m_isCheckBossHint == false)
		{
			yield return  new WaitForSeconds(0.5f);


			Debug.Log ("보스 힌트 보는중...");

		

			if (bossHint.m_isCheckBossHint == true) 
			{
				//배경화면 전환
				Debug.Log("보스 소환!!");
				//bossBackGround.StartChangeBackGroundToBossBackGround ();
				backGroundScolling.StartChangeBackground(eBackgroundMat.E_BackgroundMat_Boss);
				bossList [_index].SetActive (true);
				//bossHint.m_isCheckBossHint = false;

			}

		}

	}


	public void CheckCurDaysAndBossUnlock()
	{
		if (GameManager.Instance.cBossPanelListInfo [0].isUnlockIceBoss == true) 
		{
			bossElementList [0].BossUnlock_Obj.SetActive (false);
		}
		if (GameManager.Instance.cBossPanelListInfo [0].isUnlockSasinBoss == true)
		{
	
			bossElementList [1].BossUnlock_Obj.SetActive (false);
		}
			
		if (GameManager.Instance.cBossPanelListInfo [0].isUnlockFireBoss == true) 
		{

			bossElementList [2].BossUnlock_Obj.SetActive (false);
		}
		if (GameManager.Instance.cBossPanelListInfo [0].isUnlockMusicBoss == true)
		{

			bossElementList [3].BossUnlock_Obj.SetActive (false);
		}
	}

	public void ReloadBossChallengeCountToFull(int _index)
	{
		if (GameManager.Instance.cBossPanelListInfo [0].nBossPotionCount == 0)
		{
			bossPopUpWindow.PopUpWindow_Yes.SetActive (true);
			bossPopUpWindow.PopUpWindow_Yes.transform.GetChild (0).GetComponent<Text> ().text = "물약이 부족합니다.";
			return;
		}

		if (_index == (int)E_BOSSNAME.E_BOSSNAME_ICE) 
		{
			nBossIceLeftCount = 3;
			bossElementList [_index].BossLeftCount_Text.text =
				string.Format ("{0} / {0}", nBossIceLeftCount, nBossMaxLeftCount);
			bossElementList [_index].ReloadButton_Obj.SetActive (false);
		} 
		else if (_index == (int)E_BOSSNAME.E_BOSSNAME_SASIN) {
			
			nBossSasinLeftCount = 3;
			bossElementList [_index].BossLeftCount_Text.text =
				string.Format ("{0} / {0}", nBossSasinLeftCount, nBossMaxLeftCount);
			bossElementList [_index].ReloadButton_Obj.SetActive (false);
		} 
		else if (_index == (int)E_BOSSNAME.E_BOSSNAME_FIRE)
		{
			nBossFireLeftCount = 3;
			bossElementList [_index].BossLeftCount_Text.text =
				string.Format ("{0} / {0}", nBossFireLeftCount, nBossMaxLeftCount);
			bossElementList [_index].ReloadButton_Obj.SetActive (false);
		}
		else 
		{
			nBossMusicLeftCount = 3;
			bossElementList [_index].BossLeftCount_Text.text =
				string.Format ("{0} / {0}", nBossMusicLeftCount, nBossMaxLeftCount);
			bossElementList [_index].ReloadButton_Obj.SetActive (false);
		}

		bossConsumeItemInfo.nPotionCount -= 1;
		bossConsumeItemInfo.positionCount_Text.text = string.Format ("{0}",	bossConsumeItemInfo.nPotionCount );
		BossPanelInfoSave ();
	}


}
