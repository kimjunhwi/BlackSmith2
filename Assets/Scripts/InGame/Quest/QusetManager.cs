using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum QuestType
{
	E_QUESTTYPE_CUSTOMERSUCCESS = 0,			//손님 x명 성공
	E_QUESTTYPE_DAYS ,							//x일차 증가
	E_QUESTTYPE_WATERUSE,						//물사용 증가
	E_QUESTTYPE_MISS, 							//수리시 빗나감
	E_QUESTTYPE_CRITICALSUCCESS,				//크리 성공
	E_QUESTTYPE_ARBAITSUCCESS,					//아르바이트 성공
	E_QUESTTYPE_BIGSUCCESS,						//대성공
	E_QUESTTYPE_BIGSUCCESSANDCUSTOMERSUCCESS,	//대성공중 손님 50명 성공
	E_QUESTTYPE_CREATEHAMMER,					//x회 망치 제작
	E_QUESTTYPE_INTIMECUTOMERSUCCESS,			//시간내에 x명 손님 성공
	E_QUESTTYPE_NOMISSCUTOMERSUCCESS,			//수리시 빗나가지 않고 x명 성공
	E_QUESTTYPE_NOWATERUSE,						//물 사용하지 않고 x명 성공
	E_QUESTTYPE_ANYBOSSSUCCESS,					//아무 보스 x회 성공
	E_QUESTTYPE_BOSSICESUCCESS,					//얼음 보스 x회 성공
	E_QUESTTYPE_BOSSSASINSUCCESS,				//사신 보스 x회 성공 
	E_QUESTTYPE_BOSSFIRESUCCESS,				//불 보스 x회 성공
	E_QUESTTYPE_BOSSMUSICSUCCESS,				//음악 보스 x회 성공
	E_QUESTTYPE_CONSTANTACCESS,					//접속유지 x분
	E_QUESTTYPE_NONE,
}

public class QusetManager : MonoBehaviour, IPointerClickHandler
{
	
	public int nQuestCount = 0;
	public int nQuestMaxCount = 0;
	public int nQuestMaxHaveCount = 3;
	public int nQuestMileCount = 0;
	public int nQeustMaxMileCount = 0;				
	private int nQuestTotalCount = 0;						//전체 퀘스트 개수
	private int nQuestTypeTotalCount = 18;

	public GameObject questAdsPopUpWindow_YesNo;			//Yes or No
	public Button questAdsPopUpWindow_AdsButton;			//Yes or No가 있는 창에서의 YesButton
	public Button questAdsPopUpWindow_RubyButton;			//Yes or No가 있는 창에서의 NoButton

	private bool isInitConfirm;

	public GameObject questYesAndExitPopUpWindow_Yes;		//Yes
	public Button questYesAndExitPopUpWindow_YesButton;		//Yes만 있는 창에서의 YesButton
	public Text questYesAndExitPopUpWindow_Yes_Text;					//Yes만 있는 창에서의 YesButton


	public GameObject questDay;								//QuestElement를 가지는 Obj
		

	//현재 퀘스트의 정보를 가지는 리스트
    public List<QuestPanel> questObjects = new List<QuestPanel>();
    public CGameQuestInfo[] questDatas;

	private float sliderSpeed = 0.5f;
	public Slider silder;
	public GameObject rewardCheckImage01;
	public GameObject rewardCheckImage02;
	public GameObject rewardCheckImage03;

	public Text rewardCurMile_Text;
	public Text rewardMile1_Text;
	public Text rewardMile2_Text;
	public Text rewardMile3_Text;

	public int nFirstReward = 10;
	public int nSecondReward = 25;
	public int nThirdReward = 40;

	public SimpleObjectPool questObjectPool;

	Color CheckColor;

	//Timer
	public 	Text timerText;
	public QuestTimer questTimer;

	private bool isLoginAndFirstActive = false;
	private bool isInGameOnOff = false;



	public void SetUp()
	{
		gameObject.SetActive (true);

		questObjectPool.PreloadPool ();
		questDatas = GameManager.Instance.cQusetInfo;	//data push
		nQuestMaxCount = questDatas.Length;
		nQeustMaxMileCount = 40;

		CheckColor = new Color (255.0f, 0, 0, 255.0f);

		//rewardCurMile_Text = string.Format ("{0}", 0);
		//마일리지 포인트 
		rewardMile1_Text.text = string.Format ("{0}",nFirstReward );
		rewardMile2_Text.text = string.Format ("{0}",nSecondReward );
		rewardMile3_Text.text = string.Format ("{0}",nThirdReward );


		//처음 실행시
		if (GameManager.Instance.cQuestSaveListInfo[0].bIsGoogleSave == false &&
			GameManager.Instance.cQuestSaveListInfo[0].bIsFirstActive == true && isInGameOnOff == false) 
		{
			isInGameOnOff = true;
			Debug.Log ("Quest first Active");
			QuestInitStart ();
			GameManager.Instance.SaveQuestList ();
			return;
		}
	
		//저장된 데이터 로드시
		if (GameManager.Instance.cQuestSaveListInfo[0].bIsGoogleSave == false &&
			GameManager.Instance.cQuestSaveListInfo[0].bIsFirstActive == false && isInGameOnOff == false) 
		{
			isInGameOnOff = true;
			Debug.Log ("Quest Load Data");
			QuestSaveInitStart ();
		}
		//인게임에서 키고 끌시
		if (GameManager.Instance.cQuestSaveListInfo[0].bIsGoogleSave == false &&
			GameManager.Instance.cQuestSaveListInfo[0].bIsFirstActive == false && isInGameOnOff == true) 
		{
			Debug.Log ("Just Check Time");
		}


		//초기화 시간이 지나 있으면 추가하기 버튼이 활성화
		if (questTimer.checkIsTimeGone() == true)
		{
			isInitConfirm = true;
			questTimer.addQuestToEmptySpace.SetActive(true);
		} 
		else 
		{
			//시간이 지나있지 않다면 시간만 로드 한다
			isInitConfirm = false;
			questTimer.LoadTimeAndCheckTimeEnd();
		}

		GameManager.Instance.SaveQuestList ();

	}


	void Update()
	{
		//마일리지 슬라이더
		if (silder.value < ((float)nQuestMileCount / (float)nQeustMaxMileCount)) 
		{
			silder.value += ((float)nQuestMileCount / (float)nQeustMaxMileCount) * sliderSpeed * Time.deltaTime;

			//해당 포인트 이상이면 적용
			if (silder.value >= nFirstReward)
				rewardCheckImage01.SetActive (true);
			else if (silder.value >= nFirstReward)
				rewardCheckImage02.SetActive (true);
			else
				rewardCheckImage03.SetActive (true);

			rewardCurMile_Text.text = string.Format("{0}", nQuestMileCount );
		}


	
	}

	public void OnPointerClick (PointerEventData eventData)
	{
		GameObject  getInfoGameObject = eventData.pointerEnter;

		if (getInfoGameObject.gameObject.name == "QuestPanel")
		{
			SaveQuestData ();
			questTimer.SaveTime ();
			getInfoGameObject.SetActive (false);
		}
	}

	//포기 버튼을 누를시
	public void GiveUpQuest()
	{
		if (!questAdsPopUpWindow_YesNo.activeSelf) 
		{
			questYesAndExitPopUpWindow_Yes.SetActive (true);
			questYesAndExitPopUpWindow_Yes_Text.text = "퀘스트를 포기 하시겠습니까?";
			questYesAndExitPopUpWindow_YesButton.onClick.RemoveAllListeners ();
			questYesAndExitPopUpWindow_YesButton.onClick.AddListener (CheckQuestDestroy);
		}
	}

	public void GameObjectSetActive(GameObject _gameObject,  bool _bool)
	{
		if (_bool == true)
		{
			_gameObject.SetActive (true);
		} else
			_gameObject.SetActive (false);
	}

	public void CheckQuestDestroy()
	{
		QuestPanel deleteQuestPanel = null;

		for (int i = 0; i < questDay.transform.childCount; i++)
		{
			GameObject go = questDay.transform.GetChild (i).gameObject;
			deleteQuestPanel = go.GetComponent<QuestPanel> ();

			if (deleteQuestPanel.bIsQuest == false)
			{
				deleteQuestPanel.bIsQuest = false;
				deleteQuestPanel.completeButton.SetActive (false);
				questObjectPool.ReturnObject (go);
				questObjects.Remove (deleteQuestPanel);
			}
			deleteQuestPanel = null;
		}

		//퀘스트 타이머의 시간이 꺼져있을때
		if (questTimer.isTimeOn == false)
			questTimer.StartQuestTimer ();
	}
		
	public void CheckCompleteQuestDestroy()
	{
		QuestPanel deleteQuestPanel = null;

		for (int i = 0; i < questDay.transform.childCount; i++)
		{
			GameObject go = questDay.transform.GetChild (i).gameObject;
			deleteQuestPanel = go.GetComponent<QuestPanel> ();

			if (deleteQuestPanel.bIsQuest == false)
			{
				deleteQuestPanel.bIsQuest = false;
				deleteQuestPanel.completeButton.SetActive (false);
				questObjectPool.ReturnObject (go);
				questObjects.Remove (deleteQuestPanel);
				questYesAndExitPopUpWindow_YesButton.onClick.RemoveListener (CheckQuestDestroy);

				deleteQuestPanel = null;
			}
		}
	}


	public void AllDestroyQuest()
	{
		while (questDay.transform.childCount != 0) 
		{
			GameObject go = questDay.transform.GetChild (0).gameObject;
			questObjectPool.ReturnObject(go);
		}
	}
		
	//시간이 지나가지 않아도 초기화 버튼으로 초기화 할때
	public void QuestInit_ShowWindow()
	{
		if (!questAdsPopUpWindow_YesNo.activeSelf) 
		{
			//추후 루비 추가 해야됨 ,광고 추가 
			questAdsPopUpWindow_YesNo.SetActive (true);
			//questAdsPopUpWindowYesNo_Text.text = "퀘스트를 초기화 하기위해 광고나 루비 무엇을 쓰시겠습니까?";
			questAdsPopUpWindow_AdsButton.onClick.RemoveAllListeners(); 
			questAdsPopUpWindow_RubyButton.onClick.RemoveAllListeners ();

			questAdsPopUpWindow_AdsButton.onClick.AddListener (() => SpawnManager.Instance.ShowAdsSkipInGameManager(false)); 	//QuestInit에서 다시 지움
			questAdsPopUpWindow_RubyButton.onClick.AddListener(() => SpawnManager.Instance.ShowAdsSkipInGameManager(true));		
		}
		else
		{
			QuestInitStart ();
		}
	}

	//추가하기 버튼으로 초기화하는 퀘스트 호출
	public void QuestInit()
	{
		isInitConfirm = true;
		QuestInitStart ();
	}

	//게임을 시작하고 저장되어 있고 처음 퀘스트를 켰을때
	public void QuestSaveInitStart()
	{
		
		GameObject quest;
		nQuestCount = 0;
		AllDestroyQuest ();
		questObjects.Clear ();
		//Add
		for (int i = 0; i < 3; i++)
		{
			nQuestCount++;
			quest = questObjectPool.GetObject ();
			quest.transform.SetParent (questDay.transform,false);
			quest.transform.localScale = Vector3.one;
		
			QuestPanel questPanel = quest.gameObject.GetComponent<QuestPanel> ();
			questPanel.nQuestPanelIndex = i;
			questObjects.Add (questPanel);
		}

		QuestSaveDataDispatch ();	//Data Dispatch

		isInitConfirm = false;

	}

	//퀘스트 초기화 시작
	public void QuestInitStart()
	{
		GameObject quest;
		questAdsPopUpWindow_YesNo.SetActive (false);
		//questAdsPopUpWindow_AdsButton.onClick.RemoveListener (SpawnManager.Instance.ShowAdsSkipInGameManager);
		//questAdsPopUpWindow_RubyButton.onClick.RemoveListener (SpawnManager.Instance.ShowAdsSkipInGameManager);		

		//처음 퀘스트 켜질시
		if (GameManager.Instance.cQuestSaveListInfo [0].bIsFirstActive == true)
		{
			GameManager.Instance.cQuestSaveListInfo [0].bIsFirstActive = false;

			nQuestCount = 0;
			AllDestroyQuest ();
			questObjects.Clear ();
			//Add
			for (int i = 0; i < nQuestMaxHaveCount; i++)
			{
				nQuestCount++;
				quest = questObjectPool.GetObject ();
				quest.transform.SetParent (questDay.transform,false);
				quest.transform.localScale = Vector3.one;

				QuestPanel questPanel = quest.gameObject.GetComponent<QuestPanel> ();
				questPanel.nQuestPanelIndex = i;
				questObjects.Add (questPanel);



			}

			QuestDataDispatch ();	//Data Dispatch

			questTimer.isTimeOn = false;
			questTimer.isTimeEnd = false;
			questTimer.InitQuestTimer ();
			return;
		}

		//시간이 다 안되고 초기화 버튼을 누를시
		if (questTimer.isTimeEnd == false && isInitConfirm == true)
		{
			Debug.Log ("NoTime And Init Call!");
			nQuestCount = 0;
			//AllDestroyQuest ();
			//questObjects.Clear ();
			//Add

			//퀘가 꽉차 있다면 
			if (questObjects.Count >= 3)
				return;

			for (int i = questObjects.Count ; questObjects.Count < nQuestMaxHaveCount; i++)
			{
				nQuestCount++;
				quest = questObjectPool.GetObject ();
				quest.transform.SetParent (questDay.transform,false);
				quest.transform.localScale = Vector3.one;

				QuestPanel questPanel = quest.gameObject.GetComponent<QuestPanel> ();
				questPanel.nQuestPanelIndex = i;
				questObjects.Add (questPanel);
			}

			QuestDataDispatch ();	//Data Dispatch

			questTimer.isTimeOn = false;
			questTimer.isTimeEnd = false;
			questTimer.InitQuestTimer ();
			questTimer.addQuestToEmptySpace.SetActive (false);
			return;

		}

		//초기화 버튼을 누를시 빈곳에 랜덤으로 넣어준다
		if(isInitConfirm == true && questTimer.isTimeEnd == true) 
		{
			nQuestCount = 0;
			//AllDestroyQuest ();
			//questObjects.Clear ();
			//Add


			isInitConfirm = false;
			questTimer.isTimeOn = false;
			questTimer.InitQuestTimer ();

			if (questObjects.Count == nQuestMaxHaveCount) 
			{
				ShowEmptyQuestFull ();
				return;
			}

			for (int i = questObjects.Count ; questObjects.Count < nQuestMaxHaveCount  ; i++)
			{
				nQuestCount++;
				quest = questObjectPool.GetObject ();
				quest.transform.SetParent (questDay.transform,false);
				quest.transform.localScale = Vector3.one;

				QuestPanel questPanel = quest.gameObject.GetComponent<QuestPanel> ();
				questObjects.Add (questPanel);
				QuestDataDispatch (i);
			}



			return;
		}
		//

	}

	public void ShowEmptyQuestFull()
	{
		questYesAndExitPopUpWindow_Yes.SetActive (true);
		questYesAndExitPopUpWindow_Yes_Text.text = "주의 : 빈칸의 퀘스트를 채움니다.";

	}

	//저장되어있던 데이터 배치
	public void QuestSaveDataDispatch()
	{
		for(int i = 0 ; i< questObjects.Count; i++)
		{
			QuestPanel questPanel = questObjects[i].gameObject.GetComponent<QuestPanel> ();
			if(i==0)
				questPanel.GetQuest (questDatas[GameManager.Instance.cQuestSaveListInfo[0].nQuestIndex01] , this ,
					GameManager.Instance.cQuestSaveListInfo[0].nQuestIndex01_ProgressValue , 
					GameManager.Instance.cQuestSaveListInfo[0].nQuestIndex01_MultiplyValue);
			if(i ==1)
				questPanel.GetQuest (questDatas[GameManager.Instance.cQuestSaveListInfo[0].nQuestIndex02] , this ,
					GameManager.Instance.cQuestSaveListInfo[0].nQuestIndex02_ProgressValue,
					GameManager.Instance.cQuestSaveListInfo[0].nQuestIndex01_MultiplyValue);
			if(i == 2)
				questPanel.GetQuest (questDatas[GameManager.Instance.cQuestSaveListInfo[0].nQuestIndex03] , this ,
					GameManager.Instance.cQuestSaveListInfo[0].nQuestIndex03_ProgressValue,
					GameManager.Instance.cQuestSaveListInfo[0].nQuestIndex01_MultiplyValue);
		} 
	}

	//Data 할당
	public void QuestDataDispatch()
    {
		
		nQuestTotalCount = GameManager.Instance.cQusetInfo.Length;


		if (GameManager.Instance.cBossPanelListInfo [0].isUnlockIceBoss == false &&
			GameManager.Instance.cBossPanelListInfo [0].isUnlockSasinBoss == false &&
			GameManager.Instance.cBossPanelListInfo [0].isUnlockSasinBoss == false && 
			GameManager.Instance.cBossPanelListInfo [0].isUnlockSasinBoss == false)
			nQuestTotalCount -= 6;
		
		if (GameManager.Instance.cBossPanelListInfo [0].isUnlockIceBoss == true && GameManager.Instance.cBossPanelListInfo [0].isUnlockSasinBoss == false
			&&  GameManager.Instance.cBossPanelListInfo [0].isUnlockSasinBoss == false && GameManager.Instance.cBossPanelListInfo [0].isUnlockSasinBoss == false)
			nQuestTotalCount -= 5;
	
		if (GameManager.Instance.cBossPanelListInfo [0].isUnlockIceBoss == true && GameManager.Instance.cBossPanelListInfo [0].isUnlockSasinBoss == true
			&&  GameManager.Instance.cBossPanelListInfo [0].isUnlockSasinBoss == false && GameManager.Instance.cBossPanelListInfo [0].isUnlockSasinBoss == false) 
			nQuestTotalCount -= 4;
		
		if (GameManager.Instance.cBossPanelListInfo [0].isUnlockIceBoss == true && GameManager.Instance.cBossPanelListInfo [0].isUnlockSasinBoss == true
			&&  GameManager.Instance.cBossPanelListInfo [0].isUnlockSasinBoss == true && GameManager.Instance.cBossPanelListInfo [0].isUnlockSasinBoss == false)
			nQuestTotalCount -= 3;
		

		int RepeatCount = questObjects.Count;

		for(int i = 0 ; i< RepeatCount; i++)
		{
			QuestPanel questPanel = questObjects[i].gameObject.GetComponent<QuestPanel> ();
				
			int random = Random.Range (0, nQuestTotalCount - 1 );


			questPanel.GetQuest (questDatas [random], this);
			questPanel.InitQuestValue ();
			questPanel.questTypeIndex = (QuestType)questDatas [random].nType;

		} 
    }
	//Saved Quest Data 할당
	public void QuestDataDispatch(int _index)
	{
		nQuestTotalCount = GameManager.Instance.cQusetInfo.Length;

		QuestPanel questPanel = questObjects[_index].gameObject.GetComponent<QuestPanel> ();

		int random = Random.Range (0, nQuestTotalCount - 1 );

		questPanel.GetQuest (questDatas [random], this);
		questPanel.InitQuestValue ();
		questPanel.questTypeIndex = (QuestType)questDatas [random].nType;

	}

	//퀘스트를 체크하는 곳에서 해당 퀘스트에 해당되는 enum 값을 넘겨서 퀘스트 목록중 해당되는 것을 찾아 값을 올린다
	public void QuestSuccessCheck(QuestType _questTypeIndex, int _value)
	{
		if (questObjects[0] == null)
			return;
		
		for (int i = 0; i < questObjects.Count; i++) 
		{
			QuestPanel questPanel = questObjects[i].gameObject.GetComponent<QuestPanel> ();

			if (questPanel.questTypeIndex == _questTypeIndex) 
			{
				questPanel.nCompareCondition += _value;
				questPanel.ShowProgress ();
			}
		}

	}



	public void SaveQuestData()
	{

		if (GameManager.Instance.cQuestSaveListInfo [0].bIsFirstActive == false && questObjects.Count != 0)
		{
			Debug.Log ("Save Local Quest Data");

			GameManager.Instance.cQuestSaveListInfo [0].bIsGoogleSave = false;

			for (int i = 0; i < questObjects.Count; i++) 
			{
				if (questObjects.Count == 1) 
				{
					if (i == 0)
					{
						QuestPanel questPanel = questObjects [i].gameObject.GetComponent<QuestPanel> ();
						GameManager.Instance.cQuestSaveListInfo [0].nQuestIndex01 = questPanel.nQuestIndex;
						GameManager.Instance.cQuestSaveListInfo [0].nQuestIndex01_ProgressValue = questPanel.nCompareCondition;
						if(questPanel.nMutiplyValue <= 0)
							GameManager.Instance.cQuestSaveListInfo [0].nQuestIndex01_MultiplyValue = 1;
						else
							GameManager.Instance.cQuestSaveListInfo [0].nQuestIndex01_MultiplyValue = questPanel.nMutiplyValue;

					}
					if (i == 1) {
						QuestPanel questPanel = questObjects [i].gameObject.GetComponent<QuestPanel> ();
						GameManager.Instance.cQuestSaveListInfo [0].nQuestIndex02 = -1;
						GameManager.Instance.cQuestSaveListInfo [0].nQuestIndex02_ProgressValue = -1;
						GameManager.Instance.cQuestSaveListInfo [0].nQuestIndex02_MultiplyValue = -1;
					}
					if (i == 2) {
						QuestPanel questPanel = questObjects [i].gameObject.GetComponent<QuestPanel> ();
						GameManager.Instance.cQuestSaveListInfo [0].nQuestIndex03 = -1;
						GameManager.Instance.cQuestSaveListInfo [0].nQuestIndex03_ProgressValue = -1;
						GameManager.Instance.cQuestSaveListInfo [0].nQuestIndex02_MultiplyValue = -1;
					}
				} else if (questObjects.Count == 2) {
					if (i == 0) {
						QuestPanel questPanel = questObjects [i].gameObject.GetComponent<QuestPanel> ();
						GameManager.Instance.cQuestSaveListInfo [0].nQuestIndex01 = questPanel.nQuestIndex;
						GameManager.Instance.cQuestSaveListInfo [0].nQuestIndex01_ProgressValue = questPanel.nCompareCondition;
						if(questPanel.nMutiplyValue <= 0)
							GameManager.Instance.cQuestSaveListInfo [0].nQuestIndex01_MultiplyValue = 1;
						else
							GameManager.Instance.cQuestSaveListInfo [0].nQuestIndex01_MultiplyValue = questPanel.nMutiplyValue;
					}
					if (i == 1) {
						QuestPanel questPanel = questObjects [i].gameObject.GetComponent<QuestPanel> ();
						GameManager.Instance.cQuestSaveListInfo [0].nQuestIndex02 = questPanel.nQuestIndex;
						GameManager.Instance.cQuestSaveListInfo [0].nQuestIndex02_ProgressValue = questPanel.nCompareCondition;
						if(questPanel.nMutiplyValue <= 0)
							GameManager.Instance.cQuestSaveListInfo [0].nQuestIndex02_MultiplyValue = 1;
						else
							GameManager.Instance.cQuestSaveListInfo [0].nQuestIndex02_MultiplyValue = questPanel.nMutiplyValue;
					}
					if (i == 2) {
						QuestPanel questPanel = questObjects [i].gameObject.GetComponent<QuestPanel> ();
						GameManager.Instance.cQuestSaveListInfo [0].nQuestIndex03 = -1;
						GameManager.Instance.cQuestSaveListInfo [0].nQuestIndex03_ProgressValue = -1;
						GameManager.Instance.cQuestSaveListInfo [0].nQuestIndex03_MultiplyValue = -1;
					}
				} 
				else
				{
					if (i == 0) 
					{
						QuestPanel questPanel = questObjects [i].gameObject.GetComponent<QuestPanel> ();
						GameManager.Instance.cQuestSaveListInfo [0].nQuestIndex01 = questPanel.nQuestIndex;
						GameManager.Instance.cQuestSaveListInfo [0].nQuestIndex01_ProgressValue = questPanel.nCompareCondition;
						if(questPanel.nMutiplyValue <= 0)
							GameManager.Instance.cQuestSaveListInfo [0].nQuestIndex01_MultiplyValue = 1;
						else
							GameManager.Instance.cQuestSaveListInfo [0].nQuestIndex01_MultiplyValue = questPanel.nMutiplyValue;
					}
					if (i == 1)
					{
						QuestPanel questPanel = questObjects [i].gameObject.GetComponent<QuestPanel> ();
						GameManager.Instance.cQuestSaveListInfo [0].nQuestIndex02 = questPanel.nQuestIndex;
						GameManager.Instance.cQuestSaveListInfo [0].nQuestIndex02_ProgressValue = questPanel.nCompareCondition;
						if(questPanel.nMutiplyValue <= 0)
							GameManager.Instance.cQuestSaveListInfo [0].nQuestIndex02_MultiplyValue = 1;
						else
							GameManager.Instance.cQuestSaveListInfo [0].nQuestIndex02_MultiplyValue = questPanel.nMutiplyValue;
					}
					if (i == 2) 
					{
						QuestPanel questPanel = questObjects [i].gameObject.GetComponent<QuestPanel> ();
						GameManager.Instance.cQuestSaveListInfo [0].nQuestIndex03 = questPanel.nQuestIndex;
						GameManager.Instance.cQuestSaveListInfo [0].nQuestIndex03_ProgressValue = questPanel.nCompareCondition;
						if(questPanel.nMutiplyValue <= 0)
							GameManager.Instance.cQuestSaveListInfo [0].nQuestIndex03_MultiplyValue = 1;
						else
							GameManager.Instance.cQuestSaveListInfo [0].nQuestIndex03_MultiplyValue = questPanel.nMutiplyValue;
					}
				}
			}
			GameManager.Instance.SaveQuestList ();
		} else {
			Debug.Log ("Not Save Data");
		}


	}

	public QuestType ReturnQuestType(int _index)
	{
		if (_index == (int)QuestType.E_QUESTTYPE_CUSTOMERSUCCESS)
			return QuestType.E_QUESTTYPE_CUSTOMERSUCCESS;
		else if (_index == (int)QuestType.E_QUESTTYPE_DAYS)
			return QuestType.E_QUESTTYPE_DAYS;
		else if (_index == (int)QuestType.E_QUESTTYPE_WATERUSE)
			return QuestType.E_QUESTTYPE_WATERUSE;
		else if (_index == (int)QuestType.E_QUESTTYPE_MISS)
			return QuestType.E_QUESTTYPE_MISS;
		else if (_index == (int)QuestType.E_QUESTTYPE_CRITICALSUCCESS)
			return QuestType.E_QUESTTYPE_CRITICALSUCCESS;
		else if (_index == (int)QuestType.E_QUESTTYPE_ARBAITSUCCESS)
			return QuestType.E_QUESTTYPE_ARBAITSUCCESS;
		else if (_index == (int)QuestType.E_QUESTTYPE_BIGSUCCESS)
			return QuestType.E_QUESTTYPE_BIGSUCCESS;
		else if (_index == (int)QuestType.E_QUESTTYPE_BIGSUCCESSANDCUSTOMERSUCCESS)
			return QuestType.E_QUESTTYPE_BIGSUCCESSANDCUSTOMERSUCCESS;
		else if (_index == (int)QuestType.E_QUESTTYPE_CREATEHAMMER)
			return QuestType.E_QUESTTYPE_CREATEHAMMER;
		else if (_index == (int)QuestType.E_QUESTTYPE_INTIMECUTOMERSUCCESS)
			return QuestType.E_QUESTTYPE_INTIMECUTOMERSUCCESS;
		else if (_index == (int)QuestType.E_QUESTTYPE_NOMISSCUTOMERSUCCESS)
			return QuestType.E_QUESTTYPE_NOMISSCUTOMERSUCCESS;
		else if (_index == (int)QuestType.E_QUESTTYPE_NOWATERUSE)
			return QuestType.E_QUESTTYPE_NOWATERUSE;
		else if (_index == (int)QuestType.E_QUESTTYPE_ANYBOSSSUCCESS)
			return QuestType.E_QUESTTYPE_ANYBOSSSUCCESS;
		else if (_index == (int)QuestType.E_QUESTTYPE_BOSSICESUCCESS)
			return QuestType.E_QUESTTYPE_BOSSICESUCCESS;
		else if (_index == (int)QuestType.E_QUESTTYPE_BOSSSASINSUCCESS)
			return QuestType.E_QUESTTYPE_BOSSSASINSUCCESS;
		else if (_index == (int)QuestType.E_QUESTTYPE_BOSSFIRESUCCESS)
			return QuestType.E_QUESTTYPE_BOSSFIRESUCCESS;
		else if (_index == (int)QuestType.E_QUESTTYPE_BOSSMUSICSUCCESS)
			return QuestType.E_QUESTTYPE_BOSSMUSICSUCCESS;
		else if (_index == (int)QuestType.E_QUESTTYPE_CONSTANTACCESS)
			return QuestType.E_QUESTTYPE_CONSTANTACCESS;
		else
			return QuestType.E_QUESTTYPE_NONE;

	}

	public void AddMileReward01()
	{
		GameManager.Instance.cBossPanelListInfo [0].nBossPotionCount += 1;
		rewardCheckImage01.SetActive (false);
	}
	public void AddMileReward02()
	{
		ScoreManager.ScoreInstance.RubyPlus (5);
		rewardCheckImage02.SetActive (false);
	}

	public void AddMileReward03()
	{
		GameManager.Instance.cBossPanelListInfo [0].nBossPotionCount += 1;
		ScoreManager.ScoreInstance.RubyPlus (10);
		rewardCheckImage03.SetActive (false);
	}
}
