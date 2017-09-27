using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ReadOnlys;

enum PANELINDEX
{
	PANEL_BOSS = 10000,
	PANEL_ARBAIT,
	PANEL_INVENTORY,
	PANEL_SHOP,
	PANEL_WEAR,
	PANEL_QUEST,
	PANEL_INHANCE,
}


public class BossPopUpWindow : MonoBehaviour
{
	public GameObject PopUpWindow_Yes;
	public GameObject PopUpWindow_YesNo;
	public GameObject PopUpWindow_Reward;
	public GameObject PopUpWindow_RewardPanel;
	public GameObject PopUpWindow_InviteMent;

	private Text PopUpWindow_Yes_Text;
	private Text PopUpWindow_YesNo_Text;

	public Button PopUpWindow_Yes_Button;
	public Button PopUpWindow_YesNo_YesButton;
	public Button PopUpWindow_YesNo_NoButton;
	public Button PopUpWindow_Reward_YesButton;




	//도전 팝업창이 뜰시 보여주는 초대장 개수
	public GameObject ShowInviteMentCount_Obj;
	public Text inviteMentCount_Text;

	public BossCreator bossCreator;
	public int nBossIndex = 0;
	public int [] nBossElementIndex = new int[4];


	public BossCharacter bossInfo;

	public Image BossRewardBackGround;

	public SimpleObjectPool backLightPool;
	public RectTransform backLightPosition;
	public RectTransform canvasRect;

	public BossIce bossIce;
	public BossFire bossFire;
	public BossMusic bossMusic;
	public BossSasin bossSasin;

	public bool isRewardPanelOn_Fail = false;
	public bool isRewardPanelOn_Success = false;
	public bool isRewardPanelOn_Finish = false;
	public bool isBossChallengeCountReCharge = false;

	//보스 초대장 버튼에 리스너를 넣었나 않넣었나?
	private bool isBossInivteMentButtonAddListenner = false;

	public SimpleObjectPool rewardObjPool;

	Vector3 ViewportPosition;

	public BossEffect bossEffect;


	Camera cam;

	public RepairObject repairObj;
	public int nCurBossLevel =0;

	//Image strPath
	private const string strGoldImagePath = "DungeonUI/dungeon_reward_gold";
	private const string strHonorImagePath = "DungeonUI/dungeon_reward_honor";
	private const string strRubyImagePath = "DungeonUI/dungeon_reward_roobie";

	void Start()
	{

		bossEffect = GameObject.Find ("BossEffect").GetComponent<BossEffect> ();
		 
		PopUpWindow_Yes_Text = PopUpWindow_Yes.GetComponentInChildren<Text> ();
		PopUpWindow_Yes_Button = PopUpWindow_Yes.GetComponentInChildren<Button> ();

		PopUpWindow_YesNo_Text = PopUpWindow_YesNo.GetComponentInChildren<Text> ();
		PopUpWindow_YesNo_YesButton = PopUpWindow_YesNo.transform.GetChild (1).GetComponent<Button> ();
		PopUpWindow_YesNo_NoButton = PopUpWindow_YesNo.transform.GetChild(2).GetComponent<Button>();

		PopUpWindow_Reward_YesButton = PopUpWindow_Reward.transform.GetChild (1).GetComponent<Button> ();

		cam = Camera.main;
		PopUpWindow_Yes.SetActive (false);
		PopUpWindow_YesNo.SetActive (false);
		PopUpWindow_Reward.SetActive (false);
		PopUpWindow_InviteMent.SetActive (false);

		PopUpWindow_Yes_Button.onClick.AddListener(() => PopUpWindowYes_Switch("", false));


		SetInvitementButtonListenner ();

	}


	public void PopUpWindowYes_Switch(string _string, bool _bool)
	{
		PopUpWindow_Yes_Text.text = _string;
		PopUpWindow_Yes.SetActive (_bool);
	}


	public void PopUpWindowYesNo_Switch()
	{
		if (bossCreator.bossConsumeItemInfo.nInviteMentCurCount <= 0) {
			PopUpWindowYes_Switch ("초대장이 부족합니다", true);
			return;
		}
		else
		{
			if (PopUpWindow_YesNo.activeSelf != true) {
				PopUpWindow_YesNo_YesButton.onClick.RemoveListener (bossCreator.BossCreateInit);
				PopUpWindow_YesNo_YesButton.onClick.AddListener (bossCreator.BossCreateInit);
				SetBossChallengeLeftInvitementCount (true, bossCreator.bossConsumeItemInfo.nInviteMentCurCount);

				if (nBossIndex == (int)E_BOSSNAME.E_BOSSNAME_SASIN) {
					PopUpWindow_YesNo_Text.text = "보스(사신)을 소환 하시겠습니까?";

				} else if (nBossIndex == (int)E_BOSSNAME.E_BOSSNAME_ICE) {
					PopUpWindow_YesNo_Text.text = "보스(얼음)을 소환 하시겠습니까?";


				} else if (nBossIndex == (int)E_BOSSNAME.E_BOSSNAME_FIRE) {
					PopUpWindow_YesNo_Text.text = "보스(불)을 소환 하시겠습니까?";

				} else if (nBossIndex == (int)E_BOSSNAME.E_BOSSNAME_MUSIC) {
					PopUpWindow_YesNo_Text.text = "보스(음악)을 소환 하시겠습니까?";

				} else
					PopUpWindow_YesNo_Text.text = "보스 소환 실패";

				PopUpWindow_YesNo.SetActive (true);
			} 
			else 
			{
				PopUpWindow_YesNo_YesButton.onClick.RemoveListener (bossCreator.BossCreateInit);
				PopUpWindow_YesNo.SetActive (false);
			}
		}
	}


	public void PopUpWindowReward_Switch_isFail()
	{
		if (PopUpWindow_Reward.activeSelf != true && isRewardPanelOn_Fail == false) 
		{
			Debug.Log ("Active IsFail");
			SoundManager.instance.StopSound(eSoundArray.BGM_BossBattle);

			SoundManager.instance.PlaySound (eSoundArray.ES_BossSound_Fail);

			//bossInfo.ItemInfo.....
			PopUpWindow_Reward.SetActive (true);
			isRewardPanelOn_Fail = true;
		} 
		else
		{
			Debug.Log ("DeActive IsFail");

			//SoundManager.instance.ChangeBGM (eSoundArray.BGM_BossBattle, eSoundArray.BGM_Main);
			SoundManager.instance.PlaySound(eSoundArray.BGM_Main);

			if (bossIce.eCureentBossState == Character.EBOSS_STATE.RESULT)
				bossIce.eCureentBossState = Character.EBOSS_STATE.FINISH;
			if (bossFire.eCureentBossState == Character.EBOSS_STATE.RESULT)
				bossFire.eCureentBossState = Character.EBOSS_STATE.FINISH;
			if (bossSasin.eCureentBossState == Character.EBOSS_STATE.RESULT)
				bossSasin.eCureentBossState = Character.EBOSS_STATE.FINISH;
			if (bossMusic.eCureentBossState == Character.EBOSS_STATE.RESULT)
				bossMusic.eCureentBossState = Character.EBOSS_STATE.FINISH;
			
			PopUpWindow_Reward_YesButton.onClick.RemoveListener (PopUpWindowReward_Switch_isFail);
			isRewardPanelOn_Fail = false;
			isRewardPanelOn_Finish = true;
			PopUpWindow_Reward.SetActive (false);

		}
	}

	public void PopUpWindowReward_Switch_isSuccess()
	{
		if (PopUpWindow_Reward.activeSelf != true) 
		{
			Debug.Log ("Active IsSuccess");

			SoundManager.instance.StopSound(eSoundArray.BGM_BossBattle);


			SoundManager.instance.PlaySound (eSoundArray.ES_BossSound_Success);
			//bossInfo.ItemInfo.....

			if (nBossIndex == (int)E_BOSSNAME.E_BOSSNAME_ICE)
			{
				GameManager.Instance.GetPlayer ().changeStats.nIceMaterial++;
				SpawnManager.Instance.list_ArbaitUI [(int)E_ARBAIT.E_ICE].CheckArbaitScoutCount (true);
				RewardShowAndAdd (0, 1);
			}

			if (nBossIndex == (int)E_BOSSNAME.E_BOSSNAME_SASIN)
			{
				GameManager.Instance.GetPlayer ().changeStats.nSasinMaterial++;
				SpawnManager.Instance.list_ArbaitUI [(int)E_ARBAIT.E_SASIN].CheckArbaitScoutCount (true);
				RewardShowAndAdd (2, 3);
			}

			if (nBossIndex == (int)E_BOSSNAME.E_BOSSNAME_FIRE)
			{
				GameManager.Instance.GetPlayer ().changeStats.nFireMaterial++;
				SpawnManager.Instance.list_ArbaitUI [(int)E_ARBAIT.E_SKULL].CheckArbaitScoutCount (true);
				RewardShowAndAdd (4, 1);
			}

			if (nBossIndex == (int)E_BOSSNAME.E_BOSSNAME_MUSIC)
			{
				GameManager.Instance.GetPlayer ().changeStats.nRusiuMaterial++;
				SpawnManager.Instance.list_ArbaitUI [(int)E_ARBAIT.E_DODOMCHIT].CheckArbaitScoutCount (true);
				RewardShowAndAdd (6, 7);
			}


		


			PopUpWindow_Reward.SetActive (true);
			isRewardPanelOn_Success = true;
		} 
		else
		{

			Debug.Log ("DeActive IsSuccess");

			SoundManager.instance.PlaySound(eSoundArray.ES_TouchSound_Menu);
			SoundManager.instance.PlaySound(eSoundArray.BGM_Main);

			int rewardCount = PopUpWindow_RewardPanel.transform.childCount;

			while (rewardCount != 0) {
				GameObject reward = PopUpWindow_RewardPanel.transform.GetChild (0).gameObject;
				rewardObjPool.ReturnObject (reward);
				rewardCount--;
			}

			if (bossIce.eCureentBossState == Character.EBOSS_STATE.RESULT)
				bossIce.eCureentBossState = Character.EBOSS_STATE.FINISH;
			if (bossFire.eCureentBossState == Character.EBOSS_STATE.RESULT)
				bossFire.eCureentBossState = Character.EBOSS_STATE.FINISH;
			if (bossSasin.eCureentBossState == Character.EBOSS_STATE.RESULT)
				bossSasin.eCureentBossState = Character.EBOSS_STATE.FINISH;
			if (bossMusic.eCureentBossState == Character.EBOSS_STATE.RESULT)
				bossMusic.eCureentBossState = Character.EBOSS_STATE.FINISH;

			PopUpWindow_Reward_YesButton.onClick.RemoveListener (PopUpWindowReward_Switch_isSuccess);
			isRewardPanelOn_Success = false;
			isRewardPanelOn_Finish = true;
			PopUpWindow_Reward.SetActive (false);
		}
	}

	public void GetBossIndex(int _index)
	{
		nBossIndex = _index;
	}
	public void GetBossInfo(BossCharacter _bossCharacter)
	{
		bossInfo = _bossCharacter;
	}

	public void GetBossLevel(int _Value)
	{
		nCurBossLevel = _Value;
	}

	public void SetBossRewardBackGroundImage(bool _isFailed)
	{
		if (_isFailed == false) {
			PopUpWindow_RewardPanel.SetActive (true);
			BossRewardBackGround.sprite = Resources.Load ("DungeonUI/dungeon_reward_clear", typeof(Sprite)) as Sprite;
		}
		else
		{
			BossRewardBackGround.sprite = Resources.Load ("DungeonUI/dungeon_reward_fail", typeof(Sprite)) as Sprite;
			PopUpWindow_RewardPanel.SetActive (false);
		}
	}


	public void RewardShowAndAdd(int _bossWeaponIndex01, int _bossWeaponIndex02)
	{
		BossCharacter bossCharacter = null;
		if (_bossWeaponIndex01 == 0 && _bossWeaponIndex02 == 1)
			bossCharacter = bossIce;
		if (_bossWeaponIndex01 == 2 && _bossWeaponIndex02 == 3)
			bossCharacter = bossSasin;
		if (_bossWeaponIndex01 == 4 && _bossWeaponIndex02 == 1)
			bossCharacter = bossFire;
		if (_bossWeaponIndex01 == 6 && _bossWeaponIndex02 == 7)
			bossCharacter = bossMusic;


		float nRandom_Weapon01 = Random.Range (0f, 1f);	
		if (nRandom_Weapon01 <= 1) 
		{
			GameObject Reward = rewardObjPool.GetObject ();
			Reward.transform.SetParent (PopUpWindow_RewardPanel.transform, false);
			Reward.transform.localScale = Vector3.one;
			Text RewardText = Reward.GetComponentInChildren<Text> ();
			RewardText.text = "장비";
			Image RewardImage = Reward.transform.GetChild(1).GetComponent<Image> ();
			RewardImage.sprite = ObjectCashing.Instance.LoadSpriteFromCache (GameManager.Instance.bossWeaponInfo [_bossWeaponIndex01].strResource);
			GameManager.Instance.player.inventory.GetEquimnet (GetEquiment(_bossWeaponIndex01));
		}

		float nRandom_Weapon02 = Random.Range (0f, 1f);	
		if (nRandom_Weapon02 <= bossCharacter.bossInfo.fDropPercent)
		{
			GameObject Reward = rewardObjPool.GetObject ();
			Reward.transform.SetParent (PopUpWindow_RewardPanel.transform, false);
			Reward.transform.localScale = Vector3.one;
			Text RewardText = Reward.GetComponentInChildren<Text> ();
			RewardText.text = "장비";
			Image RewardImage = Reward.transform.GetChild(1).GetComponent<Image> ();
			RewardImage.sprite = ObjectCashing.Instance.LoadSpriteFromCache (GameManager.Instance.bossWeaponInfo [_bossWeaponIndex02].strResource);
			GameManager.Instance.player.inventory.GetEquimnet (GetEquiment(_bossWeaponIndex02));
		}
		//Gold
		GameObject Gold = rewardObjPool.GetObject ();
		Gold.transform.SetParent (PopUpWindow_RewardPanel.transform, false);
		Gold.transform.localScale = Vector3.one;
		Text GoldText = Gold.GetComponentInChildren<Text> ();

		Image GoldRewardImage = Gold.transform.GetChild(1).GetComponent<Image> ();
		GoldRewardImage.sprite = ObjectCashing.Instance.LoadSpriteFromCache (strGoldImagePath);

		double goldValue =0f;

		if (nBossIndex == (int)E_BOSSNAME.E_BOSSNAME_ICE)
			goldValue = 250 * Mathf.Pow (1.09f, 24 + nCurBossLevel * 5) * 8;
		else if (nBossIndex == (int)E_BOSSNAME.E_BOSSNAME_SASIN)
			goldValue = 250 * Mathf.Pow (1.09f, 44 + nCurBossLevel * 5) * 8;
		else if (nBossIndex == (int)E_BOSSNAME.E_BOSSNAME_FIRE)
			goldValue = 250 * Mathf.Pow (1.09f, 64 + nCurBossLevel * 5) * 8;
		else if (nBossIndex == (int)E_BOSSNAME.E_BOSSNAME_MUSIC)
			goldValue = 250 * Mathf.Pow (1.09f, 84 + nCurBossLevel * 5) * 8;
		else
			goldValue = 0;
		
		string strGold =  repairObj.ChangeValue (goldValue);

		GoldText.text = string.Format("{0}", strGold);

		ScoreManager.ScoreInstance.GoldPlus (goldValue);

		//{(기본값)+2.4*(현재일차-1)}


		//Honor
		GameObject Honor = rewardObjPool.GetObject ();
		Honor.transform.SetParent (PopUpWindow_RewardPanel.transform, false);
		Honor.transform.localScale = Vector3.one;
		Text HonorText = Honor.GetComponentInChildren<Text> ();

		Image HonorRewardImage = Honor.transform.GetChild(1).GetComponent<Image> ();
		HonorRewardImage.sprite = ObjectCashing.Instance.LoadSpriteFromCache (strHonorImagePath);
	

		double nHonor = 0;

		if (nCurBossLevel <= 1) 
		{
			nHonor = GameManager.Instance.bossInfo [nBossIndex].nHonor + (2.4f);
			ScoreManager.ScoreInstance.HonorPlus (nHonor);
			HonorText.text = string.Format("{0}", GameManager.Instance.bossInfo [nBossIndex].nHonor + (2.4f));
		} 
		else 
		{
			nHonor = (double) Mathf.RoundToInt (GameManager.Instance.bossInfo [nBossIndex].nHonor + (2.4f * (nCurBossLevel -1)));
			ScoreManager.ScoreInstance.HonorPlus (nHonor);
			HonorText.text = string.Format("{0}",nHonor);
		}
	

		//Dia
		/*
		GameObject Dia = rewardObjPool.GetObject ();
		Dia.transform.SetParent (PopUpWindow_RewardPanel.transform, false);
		Dia.transform.localScale = Vector3.one;
		Text DiaText = Dia.GetComponentInChildren<Text> ();
	
		Image DiaRewardImage = Dia.transform.GetChild(1).GetComponent<Image> ();
		DiaRewardImage.sprite = ObjectCashing.Instance.LoadSpriteFromCache (strRubyImagePath);
		DiaText.text = string.Format("{0}", bossCharacter.bossInfo.nDia);
		*/

	}



	public void SetBossChallengeLeftInvitementCount(bool _bool, int _leftCount)
	{
		//도전 팝업창이 뜰시 보여주는 초대장 개수
		ShowInviteMentCount_Obj.SetActive (_bool);
		inviteMentCount_Text.text = string.Format ("{0} / {1}", _leftCount, bossCreator.bossConsumeItemInfo.nInviteMentMaxCount);
	}


	public void SetInvitementButtonListenner()
	{

		//한번만 들어오게
		if (isBossInivteMentButtonAddListenner == false)
		{
			//광고 루비x
			PopUpWindow_InviteMent.transform.GetChild (0).GetComponent<Button> ().onClick.AddListener (() => SpawnManager.Instance.ShowRewardInGameManager (bossCreator, false));
			//광고 루비o
			PopUpWindow_InviteMent.transform.GetChild (1).GetComponent<Button> ().onClick.AddListener (() => SpawnManager.Instance.ShowRewardInGameManager (bossCreator, true));
			isBossInivteMentButtonAddListenner = true;
		}

	}

	private CGameEquiment GetEquiment(int nIndex)
	{
		CGameEquiment resultEquiment = new CGameEquiment();
	
		CGameEquiment getEquiment = GameManager.Instance.bossWeaponInfo [nIndex];

		resultEquiment.nIndex = getEquiment.nIndex;
		resultEquiment.sGrade = getEquiment.sGrade;
		resultEquiment.strName = getEquiment.strName;
		resultEquiment.nSlotIndex = getEquiment.nSlotIndex;
		resultEquiment.strResource = getEquiment.strResource;
		resultEquiment.strWeaponExplain = getEquiment.strWeaponExplain;

		if (getEquiment.nIndex == (int)E_BOSS_ITEM.ICE_MORU)
			resultEquiment.fBossOptionValue = 3;
		else if (getEquiment.nIndex == (int)E_BOSS_ITEM.ICE_RING)
			resultEquiment.fBossOptionValue = 30;
		else if (getEquiment.nIndex == (int)E_BOSS_ITEM.SASIN_MORU)
			resultEquiment.fBossOptionValue = 30;
		else if (getEquiment.nIndex == (int)E_BOSS_ITEM.SASIN_CLOAK)
			resultEquiment.fBossOptionValue = 40;
		else if (getEquiment.nIndex == (int)E_BOSS_ITEM.FIRE_CLOAK)
			resultEquiment.fBossOptionValue = 10;
		else if (getEquiment.nIndex == (int)E_BOSS_ITEM.FIRE_MORU)
			resultEquiment.fBossOptionValue = 1;
		else if (getEquiment.nIndex == (int)E_BOSS_ITEM.DODOM_GLASS)
			resultEquiment.fBossOptionValue = 20;
		else if (getEquiment.nIndex == (int)E_BOSS_ITEM.DODOM_FLOWER)
			resultEquiment.fBossOptionValue = 20;

		resultEquiment.bIsBoss = getEquiment.bIsBoss;

		int nLength = int.Parse( resultEquiment.sGrade);

		int nInsertIndex = 0;

		while(nLength > 0)
		{
			nInsertIndex = Random.Range((int)E_Equiment.E_REPAIR, (int)E_Equiment.E_MAX);

			if (CheckData(resultEquiment, nInsertIndex))
				nLength--;
		}

		return resultEquiment;
	}

	private bool CheckData(CGameEquiment _equiment, int nIndex)
	{
		switch(nIndex)
		{
		case (int)E_Equiment.E_REPAIR:
			if (_equiment.fReapirPower == 0)
			{
				_equiment.fReapirPower = 5;
				return true;
			}
			break;
		case (int)E_Equiment.E_ARBAIT:
			if (_equiment.fArbaitRepair == 0)
			{
				_equiment.fArbaitRepair = 5;
				return true;
			}
			break;
		case (int)E_Equiment.E_HONOR:
			if (_equiment.fHonorPlus == 0)
			{
				_equiment.fHonorPlus = 5;
				return true;
			}
			break;
		case (int)E_Equiment.E_GOLD:
			if (_equiment.fGoldPlus == 0)
			{
				_equiment.fGoldPlus = 5;
				return true;
			}
			break;
		case (int)E_Equiment.E_WATERCHARGE:
			if (_equiment.fWaterChargePlus == 0)
			{
				_equiment.fWaterChargePlus = 5;
				return true;
			}
			break;
		case (int)E_Equiment.E_CRITICAL:
			if (_equiment.fCritical == 0)
			{
				_equiment.fCritical = 5;
				return true;
			}
			break;
		case (int)E_Equiment.E_CRITICALD:
			if (_equiment.fCriticalDamage == 0)
			{
				_equiment.fCriticalDamage = 5;
				return true;
			}
			break;
		case (int)E_Equiment.E_BIGCRITICAL:
			if (_equiment.fBigCritical == 0)
			{
				_equiment.fBigCritical = 5;
				return true;
			}
			break;
		case (int)E_Equiment.E_ACCURACY:
			if (_equiment.fAccuracyRate == 0)
			{
				_equiment.fAccuracyRate = 5;
				return true;
			}
			break;
		}

		return false;
	}

}
