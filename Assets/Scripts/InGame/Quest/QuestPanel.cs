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


	public void GiveUpActive()
	{
		bIsQuest = false;
		questManager.GiveUpQuest ();
	}

	public void GetQuest(CGameQuestInfo _quest, QusetManager _questManager)
    {
		completeButton.SetActive (false);
		bIsQuest = true;
		bIsCheckComplete = false;
		giveUpButton.onClick.RemoveAllListeners();
		giveUpButton.onClick.AddListener(GiveUpActive);

		Debug.Log ("GetQuest!" +bIsQuest );
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

			
    }
	//저장된 데이터 불러올떄
	public void GetQuest(CGameQuestInfo _quest, QusetManager _questManager , int _compareValue , int _multiplyValue)
	{
		bIsQuest = true;
		bIsCheckComplete = false;
		giveUpButton.onClick.RemoveAllListeners();
		giveUpButton.onClick.AddListener(GiveUpActive);
	
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

		if (nMutiplyValue >= 3) 
		{
			potionImage.enabled = true;
			if (questData.nRewardRuby != 0) 
			{
				int getRuby = questData.nRewardRuby * _multiplyValue;
				textReward_Ruby.text = string.Format ("{0}", getRuby);
			}
		} 
		else 
		{
			potionImage.enabled = false;
			textReward_Ruby.text = string.Format ("{0}", 0);
		}

		if (questData.nRewardHonor != 0)
		{
			int getHonor = questData.nRewardHonor * _multiplyValue;
			textReward_Honor.text = string.Format ("{0}", getHonor);
		}
	
	}


	public void QuestCompleteActive()
	{
		Debug.Log ("CompleteActive");

		completeButton.SetActive (true);
		sButton = completeButton.GetComponent<Button> ();
		sButton.onClick.RemoveAllListeners ();
		sButton.onClick.AddListener (GetQuestCompleteReward);
		sButton.onClick.AddListener (questManager.CheckCompleteQuestDestroy);


	}
	public void GetQuestCompleteReward()
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
		nCost = 0;

		bIsQuest = true;
		bIsBuy  = false;

		nQuestPanelIndex = 0;
		nQuestIndex =0;
		getGold =0;
		nCompareCondition = 0;			//현재 퀘스트의 변하는 값
		nCompleteCondition = 0;			//퀘스트 완료 조건
		nMutiplyValue = 0;				//배수 조건

		questTypeIndex = QuestType.E_QUESTTYPE_NONE;
	}
}
