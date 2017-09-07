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
}

public class QusetManager : MonoBehaviour, IPointerClickHandler
{
	
	public int nQuestCount = 0;
	public int nQuestMaxCount = 0;
	public int nQuestMaxHaveCount = 3;
	private int nQuestMileCount = 0;
	private int nQeustMaxMileCount = 0;				
	private int nQuestTotalCount = 0;						//전체 퀘스트 개수

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
	public Image rewardCheckImage01;
	public Image rewardCheckImage02;
	public Image rewardCheckImage03;

	private int nFirstReward = 3;
	private int nSecondReward = 1;
	private int nThirdReward = 7;

	public SimpleObjectPool questObjectPool;

	Color CheckColor;

	//Timer
	public 	Text timerText;
	public QuestTimer questTimer;

	private bool isLoginAndFirstActive = false;

	public void SetUp()
	{
		gameObject.SetActive (true);

		questObjectPool.PreloadPool ();
		questDatas = GameManager.Instance.cQusetInfo;	//data push
		nQuestMaxCount = questDatas.Length;
		nQeustMaxMileCount = 7;

		CheckColor = new Color (255.0f, 0, 0, 255.0f);

		//처음 실행시
		if (GameManager.Instance.cQuestSaveListInfo[0].bIsGoogleSave == false &&
			GameManager.Instance.cQuestSaveListInfo[0].bIsFirstActive == true) 
		{
			Debug.Log ("Quest first Active");
			QuestInitStart ();
		}
		//
		else
		{
			//초기화 시간이 지나 있으면 추가하기 버튼이 활성화
			if (questTimer.checkIsTimeGone() == true)
			{
				isInitConfirm = true;
				questTimer.addQuestToEmptySpace.SetActive(true);
				//QuestInitStart ();
			} 
			else 
			{
				//시간이 지나있지 않다면 시간만 로드 한다
				isInitConfirm = false;
				questTimer.LoadTime();
			}
		}
	}


	void Update()
	{
		//마일리지 슬라이더
		if (silder.value <= ((float)nQuestMileCount / (float)nQeustMaxMileCount))
			silder.value += ((float)nQuestMileCount / (float)nQeustMaxMileCount) * sliderSpeed * Time.deltaTime;

		//if (questTimer.isTimeOn == false)
		//	questTimer.StartQuestTimer ();
	}

	public void OnPointerClick (PointerEventData eventData)
	{
		GameObject  getInfoGameObject = eventData.pointerEnter;

		if (getInfoGameObject.gameObject.name == "QuestPanel")
		{
			/*
			GameManager.Instance.cQuestSaveListInfo.Clear ();
			int curItemCount = questDay.transform.childCount;
			for (int i = 0; i < curItemCount; i++) 
			{
				Transform child = questDay.transform.GetChild (i);
				QuestPanel childQuestPanel = child.GetComponent<QuestPanel> ();
				//GameManager.Instance.cQuestSaveListInfo.Add (childQuestPanel.questData);
			}
			*/
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

	//퀘스트완료  
	public void CompleteQuest()
	{
		questYesAndExitPopUpWindow_Yes.SetActive (false);
		questTimer.addQuestToEmptySpace.SetActive (false);
		//questTimer.StartQuestTimer ();
	
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
				questYesAndExitPopUpWindow_YesButton.onClick.RemoveListener (CheckQuestDestroy);

				deleteQuestPanel = null;
			}
		}

		//퀘스트 타이머의 시간이 꺼져있을때
		if (questTimer.isTimeOn == false)
			questTimer.StartQuestTimer ();
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
			questAdsPopUpWindow_AdsButton.onClick.AddListener (SpawnManager.Instance.ShowAdsSkipInGameManager); 	//QuestInit에서 다시 지움
			questAdsPopUpWindow_RubyButton.onClick.AddListener(SpawnManager.Instance.ShowAdsSkipInGameManager);		
		}
		else
		{
			QuestInitStart ();
		}
	}

	//시간이 지나면 호출되는 초기화
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
		for (int i = 0; i < GameManager.Instance.cQuestSaveListInfo.Count; i++)
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
		questAdsPopUpWindow_AdsButton.onClick.RemoveListener (SpawnManager.Instance.ShowAdsSkipInGameManager);
		questAdsPopUpWindow_RubyButton.onClick.RemoveListener (SpawnManager.Instance.ShowAdsSkipInGameManager);		

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

			if (questObjects.Count == nQuestMaxHaveCount) 
			{
				ShowEmptyQuestFull ();
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


			isInitConfirm = false;
			questTimer.isTimeOn = false;
			questTimer.InitQuestTimer ();
		

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
			//questPanel.GetQuest (GameManager.Instance.cQuestSaveListInfo[i] , this);
		} 
	}

	//Data 할당
	public void QuestDataDispatch()
    {
		nQuestTotalCount = GameManager.Instance.cQusetInfo.Length;
		for(int i = 0 ; i< questObjects.Count; i++)
		{
			QuestPanel questPanel = questObjects[i].gameObject.GetComponent<QuestPanel> ();
				
			int random = Random.Range (0, nQuestTotalCount - 1 );

			questPanel.GetQuest (questDatas [random], this);
			questPanel.InitQuestValue ();
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

	}


	public void QuestSuccessCheck(QuestType _questTypeIndex, int _value)
	{
		//해당 타입의 value를 받아서 각각
		switch (_questTypeIndex) 
		{
		case QuestType.E_QUESTTYPE_CUSTOMERSUCCESS:
			break;
		case QuestType.E_QUESTTYPE_DAYS:
			break;
		case QuestType.E_QUESTTYPE_WATERUSE:
			break;
		case QuestType.E_QUESTTYPE_MISS:
			break;
		case QuestType.E_QUESTTYPE_CRITICALSUCCESS:
			break;
		case QuestType.E_QUESTTYPE_ARBAITSUCCESS:
			break;
		case QuestType.E_QUESTTYPE_BIGSUCCESS:
			break;
		case QuestType.E_QUESTTYPE_BIGSUCCESSANDCUSTOMERSUCCESS:
			break;
		case QuestType.E_QUESTTYPE_CREATEHAMMER:
			break;
		case QuestType.E_QUESTTYPE_INTIMECUTOMERSUCCESS:
			break;
		case QuestType.E_QUESTTYPE_NOMISSCUTOMERSUCCESS:
			break;
		case QuestType.E_QUESTTYPE_NOWATERUSE:
			break;
		case QuestType.E_QUESTTYPE_ANYBOSSSUCCESS:
			break;
		case QuestType.E_QUESTTYPE_BOSSICESUCCESS:
			break;
		case QuestType.E_QUESTTYPE_BOSSSASINSUCCESS:
			break;
		case QuestType.E_QUESTTYPE_BOSSFIRESUCCESS:
			break;
		case QuestType.E_QUESTTYPE_BOSSMUSICSUCCESS:
			break;
		case QuestType.E_QUESTTYPE_CONSTANTACCESS:
			break;
		default :
			break;
		}
	}
}
