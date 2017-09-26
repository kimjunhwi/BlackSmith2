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

	public Button giveUpButton;

	//Complete
	public GameObject completeButton;
	public Text completeText;
	public Button sButton;
	public Image potionImage;

	public int nQuestPanelIndex = 0;
	public int nQuestIndex =0;
	public int getGold =0;
	public int nCompareCondition;			//현재 퀘스트의 변하는 값
	public int nCompleteCondition;			//퀘스트 완료 조건
	public int nMutiplyValue;				//배수 조건

	public Text textReward_Honor;
	public Text textReward_Ruby;
	public Text textProgressValue;
	public Text textQuestContents;
	public Text textQuestUpValue;

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

	private bool bIsCheckComplete;

    private void Start()
    {
		bIsQuest = false;
		bIsCheckComplete = false;
		giveUpButton.onClick.RemoveAllListeners();
		giveUpButton.onClick.AddListener(GiveUpActive);
    }

	void Update()
	{
		if (nCompareCondition >= nCompleteCondition && bIsCheckComplete == false && bIsQuest == true) 
		{
			QuestCompleteActive ();
			textProgressValue.text = string.Format ("{0}", nCompleteCondition)  +"/" + string.Format ("{0}", nCompleteCondition);
			questManager.expressionMark.SetActive (true);
			bIsCheckComplete = true;
		}
	}

	public void GiveUpActive()
	{
		bIsQuest = false;
		questManager.GiveUpQuest ();
	}

	public void GetQuest(CGameQuestInfo _quest, QusetManager _questManager)
    {
      
		//1~5배 만큼 곱해준다
		int randomRange = Random.Range (1, 5);

		nQuestIndex = _quest.nIndex;			//index
        questData = _quest;						//퀘스트 정보
		questManager = _questManager;			//퀘스트 매니저  
       
		textQuestContents.text = questData.strExplain;

		//init
		nCompareCondition = 0;
		nCompleteCondition = 0;
		nCompleteCondition = questData.nCompleteCondition *  randomRange;
		nMutiplyValue = randomRange;

		textProgressValue.text = string.Format ("{0}", nCompareCondition)  +"/" + string.Format ("{0}", nCompleteCondition);
		textQuestUpValue.text = string.Format ("{0}" , nMutiplyValue) + " pt";

		if (nMutiplyValue >= 3) {
			potionImage.enabled = true;
		} else {
			potionImage.enabled = false;
		}

		if (questData.nRewardHonor != 0)
		{
			int getHonor = questData.nRewardHonor * randomRange;
			textReward_Honor.text = string.Format ("{0}", getHonor);
		}

		if (questData.nRewardRuby != 0) 
		{
			int getRuby = questData.nRewardRuby * randomRange;
			textReward_Ruby.text = string.Format ("{0}", getRuby);
		}

		if (questData.nRewardBossPotion != 0) 
		{
			//textReward.text = questData.nRewardBossPotion.ToString ();
		}
			
		bIsQuest = true;
    }
	//저장된 데이터 불러올떄
	public void GetQuest(CGameQuestInfo _quest, QusetManager _questManager , int _compareValue , int _multiplyValue)
	{
		bIsQuest = true;
	
		nQuestIndex = _quest.nIndex;			//index
		questData = _quest;						//퀘스트 정보
		questManager = _questManager;			//퀘스트 매니저  

		textQuestContents.text = questData.strExplain;
		questTypeIndex = questManager.ReturnQuestType (nQuestIndex);
		nCompareCondition = _compareValue;
		nCompleteCondition = 0;
		nCompleteCondition = questData.nCompleteCondition * _multiplyValue;
		nMutiplyValue = _multiplyValue;
		//1~5배 만큼 곱해준다
		textProgressValue.text = string.Format ("{0}", nCompareCondition)  +"/" + string.Format ("{0}", nCompleteCondition);
		textQuestUpValue.text = string.Format ("{0}" , nMutiplyValue) + " pt";

		if (nMutiplyValue >= 3) {
			potionImage.enabled = true;
		} else {
			potionImage.enabled = false;
		}

		if (questData.nRewardHonor != 0)
		{
			int getHonor = questData.nRewardHonor * _multiplyValue;
			textReward_Honor.text = string.Format ("{0}", getHonor);
		}

		if (questData.nRewardRuby != 0) 
		{
			int getRuby = questData.nRewardRuby * _multiplyValue;
			textReward_Ruby.text = string.Format ("{0}", getRuby);
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
			if (i == 0) 
			{
				if (nMutiplyValue <= 2) 
				{
					ScoreManager.ScoreInstance.HonorPlus (questData.nRewardHonor * nMutiplyValue);
				}
				else if (nMutiplyValue <= 3)
				{
					ScoreManager.ScoreInstance.HonorPlus (questData.nRewardHonor * nMutiplyValue);
					GameManager.Instance.cBossPanelListInfo [0].nBossPotionCount += questData.nRewardBossPotion;
				} 
				else
				{
					ScoreManager.ScoreInstance.RubyPlus (questData.nRewardRuby * nMutiplyValue);
					ScoreManager.ScoreInstance.HonorPlus (questData.nRewardHonor * nMutiplyValue);
					GameManager.Instance.cBossPanelListInfo [0].nBossPotionCount += questData.nRewardBossPotion;

				}

			}
			else if (i == 1) 
			{
				if (nMutiplyValue <= 2) 
				{
					ScoreManager.ScoreInstance.HonorPlus (questData.nRewardHonor * nMutiplyValue);
				}
				else if (nMutiplyValue <= 3)
				{
					ScoreManager.ScoreInstance.HonorPlus (questData.nRewardHonor * nMutiplyValue);
					GameManager.Instance.cBossPanelListInfo [0].nBossPotionCount += questData.nRewardBossPotion;
				} 
				else
				{
					ScoreManager.ScoreInstance.RubyPlus (questData.nRewardRuby * nMutiplyValue);
					ScoreManager.ScoreInstance.HonorPlus (questData.nRewardHonor * nMutiplyValue);
					GameManager.Instance.cBossPanelListInfo [0].nBossPotionCount += questData.nRewardBossPotion;

				}
			} 
			else 
			{
				if (nMutiplyValue <= 2) 
				{
					ScoreManager.ScoreInstance.HonorPlus (questData.nRewardHonor * nMutiplyValue);
				}
				else if (nMutiplyValue <= 3)
				{
					ScoreManager.ScoreInstance.HonorPlus (questData.nRewardHonor * nMutiplyValue);
					GameManager.Instance.cBossPanelListInfo [0].nBossPotionCount += questData.nRewardBossPotion;
				} 
				else
				{
					ScoreManager.ScoreInstance.RubyPlus (questData.nRewardRuby * nMutiplyValue);
					ScoreManager.ScoreInstance.HonorPlus (questData.nRewardHonor * nMutiplyValue);
					GameManager.Instance.cBossPanelListInfo [0].nBossPotionCount += questData.nRewardBossPotion;

				}
			}
		}

		//마일리지 체크
		questManager.nQuestMileCount += nMutiplyValue;

		questManager.expressionMark.SetActive (false);
		//꽉차면 마일리지
		if (questManager.silder.value >= (float)questManager.nQeustMaxMileCount)
		{
			questManager.silder.value = 0f;
			questManager.rewardCheckImage01.SetActive(false);
			questManager.rewardCheckImage01.SetActive(false);
			questManager.rewardCheckImage01.SetActive(false);
		}
		//마일리지 카운트에따라 이미지 온
		if (questManager.nQuestMileCount >= questManager.nFirstReward)
			questManager.rewardCheckImage01.SetActive(true);

		if (questManager.nQuestMileCount >= questManager.nSecondReward)
			questManager.rewardCheckImage02.SetActive(true);

		if (questManager.nQuestMileCount >= questManager.nThirdReward)
			questManager.rewardCheckImage03.SetActive(true);
	
		completeButton.SetActive (false);

	}

	public void ShowProgress()
	{
		textProgressValue.text = string.Format ("{0}", nCompareCondition)  +"/" + string.Format ("{0}", nCompleteCondition);

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
