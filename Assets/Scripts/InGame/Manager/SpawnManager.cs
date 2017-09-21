using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using LitJson;
using System.IO;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using ReadOnlys;

public class SpawnManager : GenericMonoSingleton<SpawnManager>
{
    int m_nMaxArbaitAmount;                 //아르바이트 수

    const int m_nMaxBatchArbaitAmount = 3;  //배치될 아르바이트 최대 개수

    const int m_nMaxPollAmount = 5;         //오브젝트풀로 만들어둘 최대 개수
   
	public int m_nBatchArbaitCount = -1;    //배치 된 아르바이트 개수를 위함

	public Transform contentsPanel;         

	public GameObject ArbaitPanel;          //아르바이트 수 만큼 생성될 페널

	public Sprite[] arbaitSprite;           //아르바이트 이미지들

    public GameObject[] m_CharicPool;       //아르바이트 게임오브젝트

    public List<GameObject> list_Character = new List<GameObject>();    //손님 리스트를 저장하기 위함 중간 삭제 등이 있으므로 리스트로 구현

	public List<int> list_FreeazeCharacter = new List<int> ();

	public List<int> checkList = new List<int> ();

    public List<ArbaitCharacter> list_ArbaitUI = new List<ArbaitCharacter>();

	public int nRandomIndex;

    public Transform[] m_BatchPosition;

    public GameObject[] m_BatchArbait;      //배치 아르바이트

    public ArbaitBatch[] array_ArbaitData;  //아르바이트 데이터 캐싱

	public bool bIsBossCreate = false;
	public bool bIsCharacterBack = false;
    
    private float m_fCreateTime = 0.5f;
    private float m_fCreatePlusTime;            //몬스터 생성시간에 도달하면 몬스터 생성되는시간
    private float m_fLevelTime;             //다음 레벨 시간에 도달하게 하는 시간

	private int m_nCreateArbaitAmount;

    private CGameWeaponInfo cLevelData;         //Level에 따른 데이터를 받아오기 위함

    private float fSpeed = 5f;

	public CameraShake cameraShake;

	public SimpleObjectPool simpleSoundObjPool;

	public UIManager uiManager;

	public QusetManager questManager;

	public RepairObject repairObject;

	public Player playerData;

	public BuffPool BuffParticlePool;
	public DeBuffPool DeBuffParticlePool;

	int m_nDay = 1;

	private bool bIsFirst = false;

	public int nTutorialCurGuestCount = 0;

	public ShopCash shopCash;

	public TutorialPanel tutorialPanel;

	public BossCreator bossCreator;

	public MakingUI makingUI;


	///Goblin...
	System.DateTime StartDate = new System.DateTime();

	System.DateTime EndData;

	System.TimeSpan timeCal;

	private const int nInitTime_Sec = 299;

	public float fCurSec;

	private string strTime ="";

	public bool m_bIsGoblinCreate= false;

	enum E_GUEST
	{
		E_NONE = 0,
		E_GOBLIN = 10,
	}

    private void Awake()
    {
		if (bIsFirst == false) 
		{
			
			//게임매니저에서 아르바이트 수치를 받아옴
			m_nMaxArbaitAmount = GameManager.Instance.ArbaitLength (); 

			//받아온 수치만큼 할당
			array_ArbaitData = new ArbaitBatch[m_nMaxArbaitAmount];

			//몬스터 풀을 만듬
			CreateMonsterPool ();

			//SpawnManager를 미리 캐싱
			GameManager.Instance.player.GetSpawnManager (this);

			BuffParticlePool.Init ();
			DeBuffParticlePool.Init ();

			//터치 오브젝트들을 초기화 밑 할당 해줌 (추후 텍스트 추가)

			BreakBoomPool.Instance.Init ();

			NormalRepairPool.Instance.Init ();

			CriticalRepairPool.Instance.Init ();

			TemperatureBoomPool.Instance.Init ();

			NormalTouchPool.Instance.Init ();

			CriticalTouchPool.Instance.Init ();

			Input.multiTouchEnabled = true;

			//SoundInit
			SoundManager.instance.SetSoundObjPool (simpleSoundObjPool);
			SoundManager.instance.LoadSource ();
			SoundManager.instance.PlaySound (eSoundArray.BGM_Main);

			shopCash.InitShopIconData ();

			//CashShop Booster Load
			//저장된 시간이 1초 라도 있으면 부스터 생성
			if (GameManager.Instance.GetPlayer ().changeStats.fGoldPlusBuffSecond > 0) 
			{
				shopCash.LoadBooster (E_BOOSTERTYPE.E_BOOSTERTYPE_GOLD, GameManager.Instance.GetPlayer ().changeStats.nGoldPlusBuffMinutes,
					GameManager.Instance.GetPlayer ().changeStats.fGoldPlusBuffSecond);
			}

			if (GameManager.Instance.GetPlayer ().changeStats.fHonorPlusBuffSecond > 0) 
			{
				shopCash.LoadBooster (E_BOOSTERTYPE.E_BOOSTERTYPE_HONOR, GameManager.Instance.GetPlayer ().changeStats.nHonorPlusBuffMinutes,
					GameManager.Instance.GetPlayer ().changeStats.fHonorPlusBuffSecond);
			}

			if (GameManager.Instance.GetPlayer ().changeStats.fGuestBuffSecond > 0) 
			{
				shopCash.LoadBooster (E_BOOSTERTYPE.E_BOOSTERTYPE_STAFF, GameManager.Instance.GetPlayer ().changeStats.nGuestBuffMinutes,
					GameManager.Instance.GetPlayer ().changeStats.fGuestBuffSecond);
			}

			if (GameManager.Instance.GetPlayer ().changeStats.fAttackBuffSecond > 0) 
			{
				shopCash.LoadBooster (E_BOOSTERTYPE.E_BOOSTERTYPE_ATTACK, GameManager.Instance.GetPlayer ().changeStats.nAttackBuffMinutes,
					GameManager.Instance.GetPlayer ().changeStats.fAttackBuffSecond);
			}
			bIsFirst = true;
			//2개를 주석하면 튜토리얼 On
			tutorialPanel.eTutorialState = TutorialOrder.E_TUTORIAL_FINISH;
			tutorialPanel.gameObject.SetActive (false);

			//if (CheckIsTimer ()) {
			//	fCurSec = nInitTime_Sec;
			//
			//} 
			//else 
			//{
			//	fCurSec = playerData.changeStats.fGoblinSecond;
			//}
		}
	}
		
    public SpawnManager GetSpawnManager() { return this; }

    private void Update()
    {
		//튜토 아닐때
		if (tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_FINISH  && bIsFirst == true)
		{
			m_fCreatePlusTime += Time.deltaTime;
			m_fLevelTime += Time.deltaTime;
			fCurSec += Time.deltaTime;

			//if (fCurSec > nInitTime_Sec && m_bIsGoblinCreate == false && list_Character.Count < m_nMaxPollAmount && bIsBossCreate == false && bIsCharacterBack == false) 
			//{
			//		CreateGoblin ();

			//	m_bIsGoblinCreate = true;
			//}

			//만들 수 있는 시간이 지났거나, 현재 손님이 없을경우,
			//캐릭터 카운트가 최대미만일 경우, 보스가 활성화 중이지 않을경우 캐릭터를 생성한다.
			if ((m_fCreatePlusTime >= m_fCreateTime || list_Character.Count == 0) &&
			    list_Character.Count < m_nMaxPollAmount && bIsBossCreate == false && bIsCharacterBack == false) {
				CreateCharacter ();
			}
		} 
		//튜토 일떄
		else
		{
			//처음 시작 설명 텍스트 
			if (tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_NONE) 
			{
				Debug.Log ("Tuto1 Start");
				tutorialPanel.StartContent ();
			}

			if (tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_WAIT_DRAGONSHOW)
			{
				if (SpawnManager.Instance.list_Character.Count <= 0)
				{
					tutorialPanel.eTutorialState = TutorialOrder.E_TUTORIAL_START_DRAGONSHOW;
					StartCoroutine (SpawnManager.Instance.bossCreator.BossCreate (4));
				}
			}

			if(tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_START_IMAGE04 && uiManager.uiPanels[0].activeSelf == true)
				SpawnManager.Instance.tutorialPanel.eTutorialState = TutorialOrder.E_TUTORIAL_START_SHOWCONSTRUCT;
		
		}
    }


    //피버 설정
    //손님 생성 시간과, 손님 속도를 조절한다.
    public void SettingFever(float _fCreateTime, float _fSpeed)
    {
       
    }

    //현재 아르바이트가 수리 중인지 확인을 위함
    //만약 수리중일 경우 그 인덱스의 아르바이트를 반환
	public ArbaitBatch OverlapArbaitData(GameObject obj)
    {
		for (int nIndex = 0; nIndex < m_BatchArbait.Length; nIndex++)
        {
            if (m_BatchArbait[nIndex].activeSelf)
                if (array_ArbaitData[nIndex].AfootOjbect == obj)
					return array_ArbaitData[nIndex];
        }
		return null;
    }
    
    //몬스터 오브젝트풀을 생성한다.
    private void CreateMonsterPool()
    {
        if (m_CharicPool == null)
            return;

        for (int i = 0; i < m_CharicPool.Length; i++)
        {
            m_CharicPool[i] = Instantiate(m_CharicPool[i]);
            m_CharicPool[i].SetActive(false);
        }

        for (int nIndex = 0; nIndex < m_BatchArbait.Length; nIndex++)
        {
            //화면에 보이는 배치 오브젝트
            m_BatchArbait[nIndex] = Instantiate(m_BatchArbait[nIndex]);

            //미리 ArbaitBatch를 캐싱해준후 아르바이트 데이터를 넣어줌
            array_ArbaitData[nIndex] = m_BatchArbait[nIndex].GetComponent<ArbaitBatch>();
			array_ArbaitData [nIndex].BuffEffectPool = BuffParticlePool;
			array_ArbaitData [nIndex].DeBuffEffectPool = DeBuffParticlePool;

            array_ArbaitData[nIndex].spawnManager = this;
            array_ArbaitData[nIndex].m_CharacterChangeData = GameManager.Instance.GetArbaitData(nIndex);
            

            m_BatchArbait[nIndex].SetActive(false);
        }

        for (int nIndex = 0; nIndex < m_BatchArbait.Length; nIndex++)
        {
            ArbaitCharacter arbiatCharacter = null;

            GameObject createArbaitUI = Instantiate(ArbaitPanel);

            arbiatCharacter = createArbaitUI.GetComponent<ArbaitCharacter>();

            arbiatCharacter.SetUp(nIndex, arbaitSprite[nIndex]);

            createArbaitUI.transform.SetParent(contentsPanel, false);

            createArbaitUI.transform.localScale = Vector3.one;

            list_ArbaitUI.Add(arbiatCharacter);
        }
    }

	public void CreateGoblin()
	{
		//만약 이미 활성화 돼있다면 뒤로 돌림
		if (m_CharicPool [(int)E_GUEST.E_GOBLIN].activeSelf)
			return;

		//만약 비활성화 상태라면 활성화 시킨후
		m_CharicPool[(int)E_GUEST.E_GOBLIN].SetActive(true);

		//손님 리스트에 추가
		InsertCharacter(m_CharicPool[(int)E_GUEST.E_GOBLIN]);
	}

	//날짜가 바뀔시 그것에 대한 초기화를 진행
	public void SetDayInitInfo(int _nDay)
	{
		if (_nDay < 1)
			_nDay = 1;

		m_fCreatePlusTime = 0.0f;
		m_fLevelTime = 0.0f;

		//손님을 전부 되돌림
		if (list_Character.Count != 0)
		{
			for(int nIndex = 0; nIndex < list_Character.Count; nIndex++)
				list_Character[nIndex].GetComponent<NormalCharacter>().RetreatCharacter(4.0f,true,true);
			
			m_nDay = _nDay;



			if (m_nDay > GameManager.Instance.player.GetMaxDay ()) {
				ScoreManager.ScoreInstance.SetMaxDays (m_nDay);
			}

		

			if (m_nDay >= (int)E_BOSSAPPEARDAYS.E_BOSSAPPEARDAYS_ICE && GameManager.Instance.cBossPanelListInfo [0].isFirstFightToIceBoss == false)
			{
				GameManager.Instance.cBossPanelListInfo [0].isUnlockIceBoss = true;
				uiManager.uiBossFirstFightMark.SetActive (true);
			}
			if (m_nDay >= (int)E_BOSSAPPEARDAYS.E_BOSSAPPEARDAYS_SASIN && GameManager.Instance.cBossPanelListInfo [0].isFirstFightToSasinBoss == false) 
			{
				GameManager.Instance.cBossPanelListInfo [0].isUnlockSasinBoss = true;
				uiManager.uiBossFirstFightMark.SetActive (true);
			}
			if (m_nDay >= (int)E_BOSSAPPEARDAYS.E_BOSSAPPEARDAYS_FIRE && GameManager.Instance.cBossPanelListInfo [0].isFirstFightToFireBoss == false) 
			{
				GameManager.Instance.cBossPanelListInfo [0].isUnlockFireBoss = true;
				uiManager.uiBossFirstFightMark.SetActive (true);
			}

			if (m_nDay >= (int)E_BOSSAPPEARDAYS.E_BOSSAPPEARDAYS_MUSIC && GameManager.Instance.cBossPanelListInfo [0].isFirstFightToMusicBoss == false) 
			{
				GameManager.Instance.cBossPanelListInfo [0].isUnlockMusicBoss = true;
				uiManager.uiBossFirstFightMark.SetActive (true);
			}
		
		}

		ScoreManager.ScoreInstance.SetSuccessedGuestCount (0);
		ScoreManager.ScoreInstance.SetCurrentDays (m_nDay);
		SpawnManager.Instance.questManager.QuestSuccessCheck (QuestType.E_QUESTTYPE_DAYS, m_nDay);
	}

    //WeaponData
    #region

    //배치 된 손님이 있을 경우 지우기 위함 
    public void DeleteObject(GameObject _obj)
    {
        int nSearchIndex = 0;

        nSearchIndex = list_Character.IndexOf(_obj);

        //캐릭터를 리스트에서 지움
        list_Character.Remove(_obj);

        //재이동시킴
        OrderMove(nSearchIndex);
    }

    //게임오브젝트를 통해 배치된 손님을 찾음
    public GameObject SearchCharacter(GameObject _obj)
    {
        foreach(GameObject obj in list_Character)
        {
            if (obj == _obj)
                return obj;
        }

        return null;
    }

    //게임 오브젝트를 찾아서 데이터를 넣어줌
	public void ReturnInsertData(GameObject obj,bool bIsRepair,bool bIsResearch, double _dComplate,float _fTemperator)
    {
        GameObject tempObject = null;

        //손님 중에 있는지 확인
        tempObject = SearchCharacter(obj);

        //있을 경우 데이터를 넣어줌
        if (tempObject)
			tempObject.GetComponent<NormalCharacter>().GetRepairData(bIsRepair,bIsResearch, _dComplate, _fTemperator);
    }

	public void ComplateCharacter(GameObject _obj,double dComplate)
    {
        GameObject tempObject = null;

        tempObject = SearchCharacter(_obj);

        if (tempObject)
			tempObject.GetComponent<NormalCharacter>().m_bIsBack = true;

    }

    //무기가 완성이 됐는지 확인을 해주는 함수 이다.
	public bool CheckComplateWeapon(GameObject _obj, double _dComplate,float _fTemperator)
	{
		GameObject tempObject = null;

		tempObject = SearchCharacter(_obj);

		if (tempObject) 
			return tempObject.GetComponent<NormalCharacter> ().CheckComplate (_dComplate,_fTemperator);

		return false;
	}

    //만약 유저의 수리 페널에 아무것도 없을 경우 호출된다.
    //배치된 손님이 없을 경우 반환하며
    //수리중이지 않고, 대기상태인 손님의 무기를 가져온다.
	public void AutoInputWeaponData()
	{
		if (list_Character.Count <= 0)
			return;

		for (int nIndex = 0; nIndex < array_ArbaitData.Length; nIndex++) 
		{
			if (array_ArbaitData [nIndex].AfootOjbect == list_Character [0] && 
				list_Character[0].GetComponent<NormalCharacter>().E_STATE == Character.ENORMAL_STATE.WAIT) 
			{
				array_ArbaitData [nIndex].ArbaitDataInit ();
				list_Character [0].GetComponent<NormalCharacter> ().RepairObjectInputWeapon ();
				array_ArbaitData [nIndex].InsertWeaponData ();

				OrderMove ();
				return;
			}
		}

		if(list_Character[0].GetComponent<NormalCharacter>().E_STATE == Character.ENORMAL_STATE.WAIT)
			list_Character [0].GetComponent<NormalCharacter> ().RepairObjectInputWeapon ();

		OrderMove ();
	}

    //보스 소환시 호출 캐릭터를 이동속도를 4로 한후 전부 되돌림
	public void AllCharacterComplate()
	{
		if (list_Character.Count == 0)
			return;

		int nIndex = 0;

		while (true)
		{
			list_Character [nIndex++].GetComponent<Character> ().RetreatCharacter (4.0f, true,true);

			if (nIndex >= list_Character.Count) 
			{
				bIsBossCreate = true;
				break;
			}
		}
	}

    //이동
    void OrderMove(int nIndex = 0)
    {
        for (int i = nIndex; i < list_Character.Count; i++)
        {
			list_Character[i].GetComponent<Character>().Move(i);
        }
    }

    //캐릭터를 추가함
    void InsertCharacter(GameObject _obj)
    {
        list_Character.Add(_obj);

		_obj.GetComponent<Character>().Move(list_Character.Count-1);
		_obj.GetComponent<Character>().fSpeed = fSpeed;
    }

	public void CreateCharacter()
    {
        int nSelectCharacter;

        while(true)
        { 
            //범위안에 랜덤으로 선택
            nSelectCharacter = Random.Range(0, m_CharicPool.Length - 1);

            //만약 이미 활성화 돼있다면 뒤로 돌림
            if (m_CharicPool[nSelectCharacter].activeSelf)
                continue;

            //만약 비활성화 상태라면 활성화 시킨후
            m_CharicPool[nSelectCharacter].SetActive(true);

            //손님 리스트에 추가
            InsertCharacter(m_CharicPool[nSelectCharacter]);

            //루프 탈출
            break;
        }
      

        //몬스터가 생성됐을 경우에 m_fCreateTime을 0으로 해줌
        m_fCreatePlusTime = 0;

    }

    #endregion 

    //ArbaitData
    #region

    ////////////////////////////////아르바이트 추가가 가능한지 부분 함수 

    //만약 넣었을 경우 실행하는 함수 한번에 하지 않은 이유는 만약 넣을 수 없는데
    //현재 이 함수의 인자 값을 전부 복사해서 확인하면 부하가 커질거 같기 때문이다.
	public int AddArbaitCheck()
    {
		bool bIsSearch = true;

		for (int nIndex = 0; nIndex < m_nMaxBatchArbaitAmount; nIndex++)
        {
			for (int nSearch = 0; nSearch < array_ArbaitData.Length; nSearch++) 
			{
				if (nIndex == array_ArbaitData [nSearch].nBatchIndex) {
					bIsSearch = false;
					break;
				}
			}

			if (bIsSearch) 
				return nIndex;
			
			else
				bIsSearch = true;
        }

		int nCount = 0;

		for (int nIndex = 0; nIndex <m_BatchArbait.Length; nIndex++) {
			if (m_BatchArbait [nIndex].activeSelf)
				nCount++;
		}

		return (nCount >= m_nMaxBatchArbaitAmount) ? (int)E_CHECK.E_FAIL : nCount;
    }

    //아르바이트 추가
    //추가될 캐릭터 인덱스, 배치되는 인덱스,오브젝트,데이터를 추가해준다.
    public bool AddArbait(int _nCharacterIndex, int _nIndex,GameObject _obj, ArbaitData _data)
    {
        if (m_BatchArbait[_nCharacterIndex].activeSelf == false)
        {
			array_ArbaitData[_nCharacterIndex].GetArbaitData(_nIndex, _obj, _data);
            m_BatchArbait[_nCharacterIndex].transform.position = m_BatchPosition[_nIndex].position;
            m_BatchArbait[_nCharacterIndex].SetActive(true);

			m_nBatchArbaitCount++;
            return true;
        }

        return false;
    }
    

    ///////// 무기에서 아르바이트 를 넣을 수 있는지 판별 하는 함수 부분
    //인자로 넣은 값을 넣을 수 있는지 없는지를 확인한다.
    public int InsertArbatiWeaponCheck(int _nGrade)
    {
		int nMinValue = 10;

		for (int nIndex = 0; nIndex < m_BatchArbait.Length; nIndex++) {

            if (m_BatchArbait[nIndex] == null)
                return (int)E_CHECK.E_FAIL;

            if (m_BatchArbait [nIndex].activeSelf) {

				if ((array_ArbaitData [nIndex].bIsRepair == false)) {

					if (array_ArbaitData [nIndex].nBatchIndex < nMinValue)
						nMinValue = array_ArbaitData [nIndex].nBatchIndex;

					if (nMinValue == 0)
						return nMinValue;
				}
			}
		}

		return (nMinValue > m_nMaxBatchArbaitAmount) ? (int)E_CHECK.E_FAIL : nMinValue;
    }

    //만약 넣었을 경우 실행하는 함수 한번에 하지 않은 이유는 만약 넣을 수 없는데
    //현재 이 함수의 인자 값을 전부 복사해서 확인하면 부하가 커질거 같기 때문이다.
	public void InsertArbaitWeapon(int _nIndex, GameObject _obj, CGameWeaponInfo _data, double _dComplate, float _fTemperator)
    {
		int nIndex;

		for (nIndex = 0; nIndex < array_ArbaitData.Length; nIndex++) 
		{
			if (array_ArbaitData [nIndex].nBatchIndex == _nIndex)
				break;
		}
        array_ArbaitData[nIndex].GetWeaponData(_obj, _data, _dComplate, _fTemperator);
    }
    ////////////////////////////////////////////////////////////////


    //만약 아르바이트에서 무기를 가져갈때, 현재 캐릭터의 인덱스와, 배치된 인덱스를 통해 가져온다.
	public void InsertWeaponArbait(int _nIndex,int _nBatchIndex)
    {
		NormalCharacter charData;

        foreach(GameObject _obj in list_Character)
        {
			charData = _obj.GetComponent<NormalCharacter>();

			if (charData.m_bIsRepair == false && charData.E_STATE == Character.ENORMAL_STATE.WAIT)
            {
                charData.m_bIsRepair = true;

				charData.SpeechSelect (_nBatchIndex);

                array_ArbaitData[_nIndex].GetWeaponData(_obj, charData.weaponData, charData.m_dComplate, charData.m_fTemperator);
                break;
            }
        }
    }

    public void DeleteArbait(GameObject _obj)
    {
		for (int nIndex = 0; nIndex < m_BatchArbait.Length; nIndex++) 
		{
			if (m_BatchArbait [nIndex].activeSelf) 
			{
				if (array_ArbaitData [nIndex].ArbaitPanelObject == _obj) {
					m_BatchArbait[nIndex].SetActive(false);

					m_nBatchArbaitCount--;
					break;
				}
			}
		}
    }

	public void ArbaitScoutCount()
	{
		for (int nIndex = 0; nIndex < list_ArbaitUI.Count; nIndex++) 
			list_ArbaitUI [nIndex].CheckArbaitScoutCount (false);
	}

	//물 사용시 아르바이트중에 물 사용시 버프가 있을 경우 적용
	public void UseWater()
	{
		//ArbaitData arbait;
		float fTime = 0.0f;
		float fValue = 0.0f;

		if (m_BatchArbait [(int)E_ARBAIT.E_DODOMCHIT].activeSelf) 
		{
			for (int nIndex = 0; nIndex < m_BatchArbait.Length; nIndex++) 
			{
				array_ArbaitData [nIndex].fDodomchitRepairPercent = 0;
			}

		}

		if (m_BatchArbait [(int)E_ARBAIT.E_CLEA].activeSelf) 
		{
			fTime = array_ArbaitData [(int)E_ARBAIT.E_CLEA].m_CharacterChangeData.fCurrentFloat;
			fValue = array_ArbaitData [(int)E_ARBAIT.E_CLEA].m_CharacterChangeData.fSkillPercent;

			array_ArbaitData [(int)E_ARBAIT.E_CLEA].StartAura (fTime);

            for (int nIndex = 0; nIndex < m_BatchArbait.Length; nIndex++)
            {
                if (m_BatchArbait[nIndex].activeSelf)
                {
                    StartCoroutine(array_ArbaitData[nIndex].ApplyWaterBuffAttackSpeed(fValue, fTime));
                    list_ArbaitUI[nIndex].ChangeArbaitText();
                }
            }
		}

		if (m_BatchArbait [(int)E_ARBAIT.E_ROSA].activeSelf) 
        {
			fTime = array_ArbaitData [(int)E_ARBAIT.E_ROSA].m_CharacterChangeData.fCurrentFloat;
			fValue = array_ArbaitData [(int)E_ARBAIT.E_ROSA].m_CharacterChangeData.fSkillPercent;

			array_ArbaitData [(int)E_ARBAIT.E_ROSA].StartAura (fTime);

            for (int nIndex = 0; nIndex < array_ArbaitData.Length; nIndex++)
            {
                if (m_BatchArbait[nIndex].activeSelf)
                {
                    StartCoroutine(array_ArbaitData[nIndex].ApplyWaterBuffRepairPower(fValue, fTime));
                    list_ArbaitUI[nIndex].ChangeArbaitText();
                }
            }
        }

		if (m_BatchArbait [(int)E_ARBAIT.E_LUNA].activeSelf) 
        {
			fTime = array_ArbaitData [(int)E_ARBAIT.E_LUNA].m_CharacterChangeData.fCurrentFloat;
			fValue = array_ArbaitData [(int)E_ARBAIT.E_LUNA].m_CharacterChangeData.fSkillPercent;

			array_ArbaitData [(int)E_ARBAIT.E_LUNA].StartAura (fTime);

            for (int nIndex = 0; nIndex < array_ArbaitData.Length; nIndex++) 
				if(m_BatchArbait[nIndex].activeSelf)
					StartCoroutine(array_ArbaitData [nIndex].ApplyWaterBuffCritical (fValue,fTime));
        }
	}

    //플레이어가 크리티컬을 했을 경우 호출되며,
    //해당 아르바이트가 있을 경우 버프를 적용하기 위함이다.
    public void PlayerCritical()
    {

		float fTime = 0.0f;
		float fValue = 0.0f;

		if (m_BatchArbait[(int)E_ARBAIT.E_GLAUS].activeSelf)
		{
			fTime = array_ArbaitData [(int)E_ARBAIT.E_GLAUS].m_CharacterChangeData.fCurrentFloat;
			fValue = array_ArbaitData [(int)E_ARBAIT.E_GLAUS].m_CharacterChangeData.fSkillPercent;

			array_ArbaitData [(int)E_ARBAIT.E_GLAUS].StartAura (fTime);

			for (int nIndex = 0; nIndex < array_ArbaitData.Length; nIndex++) 
				if(m_BatchArbait[nIndex].activeSelf)
					StartCoroutine(array_ArbaitData[nIndex].ApplySmithCriticalBuffAccuracy(fValue,fTime));

			array_ArbaitData [(int)E_ARBAIT.E_GLAUS].ApplySkill ();
		}

		if (m_BatchArbait[(int)E_ARBAIT.E_ELLIE].activeSelf)
        {
			fTime = array_ArbaitData [(int)E_ARBAIT.E_ELLIE].m_CharacterChangeData.fCurrentFloat;
			fValue = array_ArbaitData [(int)E_ARBAIT.E_ELLIE].m_CharacterChangeData.fSkillPercent;

			array_ArbaitData [(int)E_ARBAIT.E_ELLIE].StartAura (fTime);

			for (int nIndex = 0; nIndex < array_ArbaitData.Length; nIndex++) 
				if(m_BatchArbait[nIndex].activeSelf)
					array_ArbaitData[nIndex].ApplyCriticalArbaitBuffAttackSpeed(fValue,fTime);
        }

		if (m_BatchArbait [(int)E_ARBAIT.E_MICHEAL].activeSelf) {

			array_ArbaitData [(int)E_ARBAIT.E_MICHEAL].StartAura (3);

			array_ArbaitData [(int)E_ARBAIT.E_MICHEAL].ApplySkill ();
		}
    }

	public void ApplyArbaitBossRepair()
	{
		for (int nIndex = 0; nIndex < array_ArbaitData.Length; nIndex++)
			if (m_BatchArbait [nIndex].activeSelf)
				array_ArbaitData [nIndex].CheckCharacterState (E_ArbaitState.E_BOSSREPAIR);
	}

	public void ReliveArbaitBossRepair()
	{
		for (int nIndex = 0; nIndex < array_ArbaitData.Length; nIndex++)
			if (m_BatchArbait [nIndex].activeSelf)
				array_ArbaitData [nIndex].CheckCharacterState (E_ArbaitState.E_WAIT);
	}



	public int FreezeArbait()
	{
		list_FreeazeCharacter.Clear ();
		nRandomIndex = -1;

		for (int nIndex = 0; nIndex < m_BatchArbait.Length; nIndex++) 
		{
			if (m_BatchArbait [nIndex].activeSelf && array_ArbaitData[nIndex].E_STATE != E_ArbaitState.E_FREEZE) {
				list_FreeazeCharacter.Add (nIndex);
			}
		}

		if (list_FreeazeCharacter.Count != 0) 
		{
			nRandomIndex = list_FreeazeCharacter [Random.Range (0, list_FreeazeCharacter.Count)];

			array_ArbaitData[nRandomIndex].CheckCharacterState (E_ArbaitState.E_FREEZE);

			return array_ArbaitData[nRandomIndex].nBatchIndex;
		}

		//Debug.Log ("List FreezeCharacter : " + list_FreeazeCharacter.Count + "RandomIndex = " + nRandomIndex);

		return nRandomIndex;
	}
	public void GetFreezeArbait()
	{
		checkList.Clear ();

		for (int i = 0; i < 10; i++)
		{
			if (array_ArbaitData [i].E_STATE == E_ArbaitState.E_FREEZE && m_BatchArbait [i].activeSelf)
				checkList.Add (array_ArbaitData[i].nBatchIndex);
		}
	}

	public void Active_IcePassive02()
	{
		Debug.Log ("Active_IcePassive02");
		for (int i = 0; i < m_BatchArbait.Length; i++)
		{
			if (m_BatchArbait [i].activeSelf) 
				array_ArbaitData [i].SetAttackSpeed (0.5f);
			
		}
	}

	public void DeActive_IcePassive02()
	{
		Debug.Log ("DeActive_IcePassive02");
		for (int i = 0; i < m_BatchArbait.Length; i++)
		{
			if (m_BatchArbait [i].activeSelf) 
				array_ArbaitData [i].SetAttackSpeed (1.0f);
		}
	}

	public void Active_MusicPassive01()
	{
		Debug.Log ("Active_MusicPassive02");
		for (int i = 0; i < m_BatchArbait.Length; i++)
		{
			if (m_BatchArbait [i].activeSelf) 
				array_ArbaitData [i].SetArbaitRepair (0.5f);
		}
	}

	public void DeActive_MusicPassive01()
	{
		Debug.Log ("DeActive_MusicPassive02");
		for (int i = 0; i < m_BatchArbait.Length; i++)
		{
			if (m_BatchArbait [i].activeSelf) 
				array_ArbaitData [i].SetArbaitRepair (1.0f);
		}
	}


	public bool FreezeArbaitCheck()
	{
		bool checkTrue = false;

		for (int i = 0; i < 10; i++)
		{
			if (array_ArbaitData [i].E_STATE != E_ArbaitState.E_FREEZE && m_BatchArbait [i].activeSelf)
				checkTrue = true;
			if (i == 9 && checkTrue == true)
				return true;		
		}
		return false;
	}


	public void DeFreezeArbait(int _nIndex)
	{
		for (int i = 0; i < 10; i++)
		{
			Debug.Log ("ChangeState");
			if (array_ArbaitData [i].nBatchIndex == _nIndex) 
			{
				array_ArbaitData[i].CheckCharacterState (E_ArbaitState.E_BOSSREPAIR);
				Debug.Log (array_ArbaitData [i].name + " curState = " + array_ArbaitData [i].E_STATE);
				return;
			}
		}
	}

    public void ReleliveArbait()
    {
        for (int nIndex = 0; nIndex < m_BatchArbait.Length; nIndex++)
        {
			if(m_BatchArbait[nIndex].activeSelf)
           	 array_ArbaitData[nIndex].RelivePauseSkill();
        }
    }

    public void ApplyArbait()
    {
        for (int nIndex = 0; nIndex < m_BatchArbait.Length; nIndex++)
        {
			if(m_BatchArbait[nIndex].activeSelf)
            	array_ArbaitData[nIndex].ApplyPauseSkill();
        }
    }

	//Bell
	public void WaterCheck()
	{
		float fValue = 0.0f;

		if (m_BatchArbait [(int)E_ARBAIT.E_BELL].activeSelf) 
		{
			array_ArbaitData [(int)E_ARBAIT.E_BELL].AuraObject.SetActive (true);

			fValue = array_ArbaitData [(int)E_ARBAIT.E_BELL].m_CharacterChangeData.fSkillPercent;

			for (int nIndex = 0; nIndex < array_ArbaitData.Length; nIndex++) 
				if(m_BatchArbait[nIndex].activeSelf)
					array_ArbaitData[nIndex].ApplyWaterUp(fValue);


		}
	}

	public void UnWaterCheck()
	{
		if (m_BatchArbait [(int)E_ARBAIT.E_BELL].activeSelf) {

			array_ArbaitData [(int)E_ARBAIT.E_BELL].AuraObject.SetActive (false);

			for (int nIndex = 0; nIndex < array_ArbaitData.Length; nIndex++)
				if (m_BatchArbait [nIndex].activeSelf)
					array_ArbaitData [nIndex].ReliveWaterUp ();
		}
	}

	public void ComplateCheckArbait(double _dComplateValue,double _dMaxComplateValue)
	{

		if (m_BatchArbait [(int)E_ARBAIT.E_SASIN].activeSelf) 
		{
			double dPercent = _dComplateValue / _dMaxComplateValue;

			if(dPercent < 0.5)
				array_ArbaitData [(int)E_ARBAIT.E_SASIN].ApplySasin (true);
			
			else
				array_ArbaitData [(int)E_ARBAIT.E_SASIN].ApplySasin (false);
				
		}
	}

	public void SkullArbaitCheck(float _fValue)
	{
		int nValue = (int)_fValue;

		if (m_BatchArbait [(int)E_ARBAIT.E_SKULL].activeSelf) 
			array_ArbaitData [(int)E_ARBAIT.E_SKULL].fSkullCritical = nValue;
		
	}

	public void DodomchitArbaitCheck()
	{

		if (m_BatchArbait [(int)E_ARBAIT.E_DODOMCHIT].activeSelf) 
		{
			float fValue = array_ArbaitData [(int)E_ARBAIT.E_DODOMCHIT].fDodomchitRepairPercent;

			for (int nIndex = 0; nIndex < m_BatchArbait.Length; nIndex++) 
			{
				array_ArbaitData [nIndex].fDodomchitRepairPercent = fValue;
			}
		}
	}

    #endregion

	public void SaveCloudDataInGameManager()
	{
		GameManager.Instance.SaveCloudData ();
	}

	public void LoadCloudDataInGameManager()
	{
		GameManager.Instance.LoadCloudData ();
	}


	public void ShowAdsSkipInGameManager(bool _isRuby)
	{
		questManager.questAdsPopUpWindow_YesNo.SetActive (false);

		if (_isRuby == true) 
		{
			ScoreManager.ScoreInstance.RubyPlus (-50);
			questManager.QuestInit ();
			return;
		}

		
		GameManager.Instance.ShowSkipAd_Quest (questManager);
	}

	public void ShowRewardInGameManager(BossCreator bossCreator , bool _isRuby)
	{
		if (_isRuby == true) {
			ScoreManager.ScoreInstance.RubyPlus (-50);
			GameManager.Instance.cBossPanelListInfo [0].nBossInviteMentCount = 5;
			SpawnManager.Instance.bossCreator.bossConsumeItemInfo.nInviteMentCurCount = GameManager.Instance.cBossPanelListInfo [0].nBossInviteMentCount;
			bossCreator.bossConsumeItemInfo.inviteMentCount_Text.text = string.Format ("{0}/{1}",SpawnManager.Instance.bossCreator.bossConsumeItemInfo.nInviteMentCurCount, 
				SpawnManager.Instance.bossCreator.bossConsumeItemInfo .nInviteMentMaxCount);
			return;
		}
		GameManager.Instance.ShowRewardAdd_Boss (bossCreator);
	}

	public bool CheckIsTimer()
	{
		if (PlayerPrefs.HasKey("Goblin"))
		{
			strTime = PlayerPrefs.GetString("Goblin");

			StartDate = System.Convert.ToDateTime(strTime);
		}

		EndData = System.DateTime.Now;

		timeCal = EndData - StartDate;

		int nStartTime = StartDate.Hour * 3600 + StartDate.Minute * 60 + StartDate.Second;
		int nEndTime = EndData.Hour * 3600 + EndData.Minute * 60 + EndData.Second;

		int nCheck = Mathf.Abs(nEndTime - nStartTime);

		//1시간이 지났거나 하루차이가 있을 경우
		if (timeCal.Days != 0 || nCheck >= 300)
			return true;

		else
			return false;

	}

}



 