using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class QuestPanel : MonoBehaviour
{

    public int nCost;

	public bool bIsQuest = true;
	public bool bIsBuy  = false;

	[HideInInspector]
	public CGameQuestInfo questData;

    public GameObject runningObject;
	public Button giveUpButton;

	//Complete
	public GameObject completeButton;
	public Text completeText;
	public Button sButton;

	public int nQuestPanelIndex = 0;
	public int nQuestIndex =0;
	public int getGold =0;
	public int nCompareCondition;			//현재 퀘스트의 변하는 값

	public Text textReward_Gold;
	public Text textReward_Honor;
	public Text textProgressValue;
	public Text textQuestContents;

	public QuestType questTypeIndex;

	private GameObject getInfoGameObject;

    private float fTime;

	public QusetManager questManager;

	//QuestValue
	public int nCustomCount;
	public int nDayCount;
	public int nWaterUseCount;
	public int nRepairMissCount;
	public int nCriticalSuccessCount;
	public int nArbaitRepairSuccessCount;
	public int nBigSuccessCount;
	public int nBigSuccessCustomSuccessCount;
	public int nCreateHammerCount;
	public int nInTimeCustomerSuccessCount;
	public int nNoRepairMissCount;
	public int nNoWaterUseCount;
	public int nAnyBossSuccessCount;
	public int nBossIceSuccessCount;
	public int nBossSasinSuccessCount;
	public int nBossFireSuccessCount;
	public int nBossMusicSuccessCount;
	public int nConstantAccessCount;




    private void Start()
    {
		
        //runningObject = transform.FindChild("CheckImage").gameObject;
        //runningButton = transform.FindChild("RunningImage").gameObject;
		//startButton = transform.FindChild("StartButton").gameObject;
		giveUpButton.onClick.AddListener(GiveUpActive);
    }

	void Update()
	{


		if (questData.nCompleteCondition == nCompareCondition) 
		{
			QuestCompleteActive ();

		}

	}

	public void GiveUpActive()
	{
		bIsQuest = false;
		questManager.GiveUpQuest ();
	}

	public void GetQuest(CGameQuestInfo _quest, QusetManager _questManager)
    {
        bIsQuest = true;
		//_quest.bIsActive = true;
		nQuestIndex = _quest.nIndex;			//index
        questData = _quest;						//퀘스트 정보
		questManager = _questManager;			//퀘스트 매니저  
       
		textQuestContents.text = questData.strExplain;

		nCompareCondition = 0;
		textProgressValue.text = string.Format ("{0}", nCompareCondition)  +"/" + string.Format ("{0}", questData.nCompleteCondition);

		//1~5배 만큼 곱해준다
		int randomRange = Random.Range (1, 5);

		if (questData.nRewardGold != 0)
		{
			int getGold = questData.nRewardGold * randomRange;
			textReward_Gold.text = string.Format ("{0}", getGold);
		}

		if (questData.nRewardHonor != 0) 
		{
			int getHonor = questData.nRewardGold * randomRange;
			textReward_Honor.text = string.Format ("{0}", getHonor);
		}

		if (questData.nRewardBossPotion != 0) 
		{
			//textReward.text = questData.nRewardBossPotion.ToString ();
		}
			

    }
	public void GetQuest(CGameQuestInfo _quest, QusetManager _questManager , int _compareValue)
	{
		bIsQuest = true;
		//_quest.bIsActive = true;
		nQuestIndex = _quest.nIndex;			//index
		questData = _quest;						//퀘스트 정보
		questManager = _questManager;			//퀘스트 매니저  

		textQuestContents.text = questData.strExplain;
		questTypeIndex = questManager.ReturnQuestType (nQuestIndex);
		nCompareCondition = _compareValue;
		textProgressValue.text = string.Format ("{0}", nCompareCondition)  +"/" + string.Format ("{0}", questData.nCompleteCondition);

		//1~5배 만큼 곱해준다
		int randomRange = Random.Range (1, 5);

		if (questData.nRewardGold != 0)
		{
			int getGold = questData.nRewardGold * randomRange;
			textReward_Gold.text = string.Format ("{0}", getGold);
		}

		if (questData.nRewardHonor != 0) 
		{
			int getHonor = questData.nRewardGold * randomRange;
			textReward_Honor.text = string.Format ("{0}", getHonor);
		}

		if (questData.nRewardBossPotion != 0) 
		{
			//textReward.text = questData.nRewardBossPotion.ToString ();
		}
	}



	public void QuestCompleteActive()
	{

		bIsQuest = false;
		completeButton.SetActive (true);
		sButton = completeButton.GetComponent<Button> ();
		sButton.onClick.RemoveListener (GetQuestCompleteReward);
		sButton.onClick.AddListener (GetQuestCompleteReward);

		sButton.onClick.RemoveListener (questManager.CheckCompleteQuestDestroy);
		sButton.onClick.AddListener (questManager.CheckCompleteQuestDestroy);

	}
	public void GetQuestCompleteReward()
	{
		for (int i = 0; i < 3; i++)
		{
			int randomRange = Random.Range (1, 5);
			if (i == 0) 
			{
				GameManager.Instance.player.changeStats.dGold += questData.nRewardGold * randomRange;
				Debug.Log ("Get Gold :" + questData.nRewardGold * randomRange);
			}
			else if (i == 1) 
			{
				GameManager.Instance.player.changeStats.dHonor += questData.nRewardHonor * randomRange;
				Debug.Log ("Get Honor :" + questData.nRewardHonor * randomRange);
			} 
			else 
			{
				GameManager.Instance.cBossPanelListInfo [0].nBossPotionCount += questData.nRewardBossPotion;
				Debug.Log ("Get Potion :" + questData.nRewardBossPotion);
			}
		}

		completeButton.SetActive (false);

	}

	public void ShowProgress()
	{
		textProgressValue.text = string.Format ("{0}", nCompareCondition)  +"/" + string.Format ("{0}", questData.nCompleteCondition);
	}


	public void InitQuestValue()
	{
		nCustomCount = 0;
		nDayCount = 0;
     	nWaterUseCount = 0;
		nRepairMissCount = 0;
		nCriticalSuccessCount = 0;
		nArbaitRepairSuccessCount = 0;
		nBigSuccessCount = 0;
		nBigSuccessCustomSuccessCount = 0;
		nCreateHammerCount = 0;
		nInTimeCustomerSuccessCount = 0;
		nNoRepairMissCount = 0;
		nNoWaterUseCount = 0;
		nAnyBossSuccessCount = 0;
		nBossIceSuccessCount = 0;
		nBossSasinSuccessCount = 0;
		nBossFireSuccessCount = 0;
		nBossMusicSuccessCount = 0;
		nConstantAccessCount = 0;
	}
}
