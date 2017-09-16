 using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using LitJson;
using System.IO;
using System.Text;
using ReadOnlys;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine.SocialPlatforms;
using UnityEngine.Advertisements;


//모든 데이터 및 로드, 세이브를 관리하는 클래스 
//어디서든 사용해야 하기 때문에 제네릭싱글톤을 통해 구현
public class GameManager : GenericMonoSingleton<GameManager>
{

    //읽어들이기만 하면 되기 때문에 유니코드 텍스트 방식 저장 후 읽어들인다.

    public CGameWeaponInfo[] cWeaponInfo = null;                //무기 정보들

    public CGameEquiment[] cEquimentInfo = null;                //장비 정보들

    public CGameQuestInfo[] cQusetInfo = null;                  //퀘스트 정보들

	public BossPanelInfo cBossPanelInfo = null;

	public CGamePlayerEnhance[] cPlayerArbaitEnhace = null;		//아르바이트 강화 

	public CGameSoundData[] cSoundsData = null;					//사운드 데이

	public CGameHammer[] cHammerNames = null;

	public ArbaitEnhance[] cArbaitEnhance = null;
	public SmithEnhance[] cSmithEnhance = null;
	public EquipmentEnhance[] cEquipmentEnhance = null;

	public GameCashItemShop[] cCashItemShopInfo = null;		//캐쉬샵 정보들

	public Boss[] bossInfo = null;

	public BossWeapon[] bossWeaponInfo = null;


	//세이브가 필요한 부분들은 LitJson을 사용함
	private List<CGamePlayerData> playerSave = new List<CGamePlayerData>();	//플레이어 데이터 저장

	private List<CreatorWeapon> CreateWeapon = new List<CreatorWeapon> ();

	private List<ArbaitData> ArbaitDataBase = new List<ArbaitData>();		//아르바이트 데이터 저장

	private List<CGameEquiment> equimnetData = new List<CGameEquiment>();	//상점 장비들 저장

	public List<CGameEquiment> cInvetoryInfo = null;            			//인벤토리 정보들

	public List<CGameQuestSaveInfo> cQuestSaveListInfo = null;					//퀘스트 저장


	public List<BossPanelInfo> cBossPanelListInfo = new List<BossPanelInfo>();

	public List<CGameMainWeaponOption> cMainWeaponOption = null;


    private JsonData itemData;
    private JsonData ArbaitData;

    private LogoManager logoManager;

    private const string strPlayerPath = "PlayerData.json";
	private const string strCreateWeapon = "CreateWeapon.json";
    private const string strArbaitPath = "ArbaitData.json";
    private const string strEquiementPath = "Equiment.json";
    private const string strInvetoryPath = "Inventory.json";
	private const string strQuestPath = "Quest.json";
	private const string strBossPanelInfoPath = "BossPanelInfo.json";

    private string strWeaponPath;

    public Player player;

    public CGamePlayerData playerData;
	public CreatorWeapon creatorWeaponData = null;

	public GameObject Root_ui;

	//GoogleSave
	private const string sSaveDataName = "BlackSmith_Save";
	public bool isGoogleClounSave = false;
	public bool isGoogleCloundDataDelete = false;

	//Ads
	private const string sAds_AndroidGameID = "1534491";
	private QusetManager questManager = null;
	private bool isQuestAdsOn = false;

	private ResultEpicUI resultEpicUI = null;

	private BossCreator bossCreator = null;

	public IEnumerator DataLoad()
    {
		//PlayerPrefs.DeleteKey ("BossRegenTime");
		//PlayerPrefs.DeleteKey ("BossInvitementSaveTime");
		//PlayerPrefs.DeleteKey("FirstLogin");

		Load_TableInfo_Hammer();
       
#if UNITY_EDITOR

		logoManager = GameObject.Find("LogoManager").GetComponent<LogoManager>();

		Load_TableInfo_Sound ();

		Load_TableInfo_Weapon();



		Load_TableInfo_Quest();

		Load_TableInfo_Equiment();

		Load_TableInfo_Boss ();

		Load_TableInfo_BossWeapon ();

		Load_TableInfo_CashShopInfo ();

        ArbaitDataBase = ConstructString<ArbaitData>(strArbaitPath);

        equimnetData = ConstructString<CGameEquiment>(strEquiementPath);

        cInvetoryInfo = ConstructString<CGameEquiment>(strInvetoryPath);

        playerData = ConstructString<CGamePlayerData>(strPlayerPath)[0];

		CreateWeapon = ConstructString<CreatorWeapon>(strCreateWeapon);

		if(CreateWeapon != null)
			creatorWeaponData = CreateWeapon[0];

		cQuestSaveListInfo = ConstructString<CGameQuestSaveInfo>(strQuestPath);
		if(cQuestSaveListInfo == null)
		{
			Debug.Log("Init QuestInfo");
			cQuestSaveListInfo = new List<CGameQuestSaveInfo>();
			CGameQuestSaveInfo tmpQuestSaveInfo = new CGameQuestSaveInfo();
			cQuestSaveListInfo.Add(tmpQuestSaveInfo);
		}

		if(ConstructString<BossPanelInfo> (strBossPanelInfoPath) == null)
		{
			Debug.Log("Init BossPanelInfo");
			cBossPanelInfo = new BossPanelInfo();
			cBossPanelListInfo.Add(cBossPanelInfo);
		}
		else
			cBossPanelListInfo = ConstructString<BossPanelInfo> (strBossPanelInfoPath);


		Debug.Log(playerData.strName);


		player = new Player();

		player.Init(cInvetoryInfo, playerData,creatorWeaponData);

        //ConstructEquimentDatabase();

        //ConstructWeaponDatabase();

        //ArbaitData = JsonMapper.ToObject(File.ReadAllText(
        //    Application.dataPath + "/StreamingAssets/ArbaitData.json"));

        //ConstructArbaitDatabase();

#elif UNITY_ANDROID
		logoManager = GameObject.Find("LogoManager").GetComponent<LogoManager>();

		Load_TableInfo_Sound ();

		Load_TableInfo_Weapon();

		Load_TableInfo_Quest();

		Load_TableInfo_Equiment();

		Load_TableInfo_Boss ();

		Load_TableInfo_BossWeapon ();

		Load_TableInfo_CashShopInfo ();

        string ArbaitFilePath = Path.Combine(Application.persistentDataPath, strArbaitPath);

		string EquimentFilePath = Path.Combine(Application.persistentDataPath, strEquiementPath);

		string InventoryFilePath = Path.Combine(Application.persistentDataPath, strInvetoryPath);

		string PlayerFilePath = Path.Combine (Application.persistentDataPath, strPlayerPath);

		string QuestFilePath = Path.Combine (Application.persistentDataPath, strQuestPath);

		string BossFilePath =  Path.Combine (Application.persistentDataPath, strBossPanelInfoPath);

		string strCreate = Path.Combine (Application.persistentDataPath, strCreateWeapon);

		//제작 무기 로드 
		if(Directory.Exists(strCreate)) 
		{
			Debug.Log("Search CreateWeapon");
			yield return StartCoroutine (LinkedCreateWeaponAccess (strCreate));
		}


		if(Directory.Exists(ArbaitFilePath)) 
		yield return StartCoroutine (LinkedArbaitAccess (ArbaitFilePath));

		else 
		{
			ArbaitFilePath = Path.Combine(Application.streamingAssetsPath, strArbaitPath);
		yield return StartCoroutine(LinkedArbaitAccess (ArbaitFilePath));
		}
		Debug.Log("1");


		if(Directory.Exists(InventoryFilePath)) 
		 yield return StartCoroutine (LinkedInventoryAccess (InventoryFilePath));


		Debug.Log("4");

		if(Directory.Exists(EquimentFilePath)) 
		yield return StartCoroutine (LinkedShopAccess (EquimentFilePath));

//		else 
//		{
//		EquimentFilePath = Path.Combine(Application.streamingAssetsPath, strEquiementPath);
//		yield return StartCoroutine(LinkedShopAccess (EquimentFilePath));
//		}

		if(Directory.Exists(PlayerFilePath)) 
		{
		Debug.Log("Search PlayerData");
		yield return StartCoroutine (LinkedPlayerAccess (PlayerFilePath));
		}
		else 
		{
			Debug.Log("No SearchPlayerData");
			PlayerFilePath = Path.Combine(Application.streamingAssetsPath, strPlayerPath);
			yield return StartCoroutine(LinkedPlayerAccess (PlayerFilePath));
		}
		Debug.Log("7");

		if(Directory.Exists(QuestFilePath)) 
			yield return StartCoroutine (LinkedQuestAccess (QuestFilePath));
		else
		{
			Debug.Log("No SavedQuestData Create New ");
			cQuestSaveListInfo = new List<CGameQuestSaveInfo>();
			CGameQuestSaveInfo tmpQuestSaveInfo = new CGameQuestSaveInfo();
			cQuestSaveListInfo.Add(tmpQuestSaveInfo);
		}

		if(Directory.Exists(BossFilePath)) 
			yield return StartCoroutine (LinkedBossPanelInfoAccess (BossFilePath));
		else
		{
			Debug.Log("No Saved Local BossPanel Info");
			cBossPanelInfo = new BossPanelInfo();
			cBossPanelListInfo.Add(cBossPanelInfo);
		}

		//Player
		player = new Player ();
		player.Init(cInvetoryInfo, playerData ,creatorWeaponData);

#endif
		InitAds ();

        logoManager.bIsSuccessed = true;

        yield break;
    }

    IEnumerator LinkedArbaitAccess(string filePath)
    {
		Debug.Log("2");

        WWW www = new WWW(filePath);

        yield return www;

        string dataAsJson = www.text.ToString();

		Debug.Log (dataAsJson);

        ArbaitDataBase = JsonHelper.ListFromJson<ArbaitData>(dataAsJson);

		Debug.Log ("ArbaitData Count" + ArbaitDataBase.Count);

		Debug.Log("3");
    }

	IEnumerator LinkedInventoryAccess(string filePath)
	{
		Debug.Log("1");

		WWW www = new WWW(filePath);

		yield return www;

		string dataAsJson = www.text.ToString();

		Debug.Log (dataAsJson);

		if (dataAsJson == "")
			yield break;

		cInvetoryInfo = JsonHelper.ListFromJson<CGameEquiment>(dataAsJson);

		Debug.Log ("cInvetoryInfo Count" + cInvetoryInfo.Count);

		Debug.Log("6");
	}

	IEnumerator LinkedShopAccess(string filePath)
	{
		Debug.Log("8");

		WWW www = new WWW(filePath);

		yield return www;
	
		string dataAsJson = www.text.ToString();

		Debug.Log (dataAsJson);

		if (dataAsJson == null)
			yield break;

		equimnetData = JsonHelper.ListFromJson<CGameEquiment>(dataAsJson);

		Debug.Log ("equimnetData Count" + equimnetData.Count);

		Debug.Log("9");
	}

	IEnumerator LinkedPlayerAccess(string filePath)
	{
		WWW www = new WWW(filePath);

		yield return www;

		string dataAsJson = www.text.ToString();

		Debug.Log (dataAsJson);

		playerData = JsonHelper.ListFromJson<CGamePlayerData>(dataAsJson)[0];
	}

	IEnumerator LinkedCreateWeaponAccess(string filePath)
	{
		WWW www = new WWW(filePath);

		yield return www;

		string dataAsJson = www.text.ToString();

		creatorWeaponData = JsonHelper.ListFromJson<CreatorWeapon>(dataAsJson)[0];
	}


	IEnumerator LinkedQuestAccess(string filePath)
	{
		Debug.Log ("Quest Loaded");
		
		WWW www = new WWW(filePath);

		yield return www;

		string dataAsJson = www.text.ToString();

		Debug.Log (dataAsJson);

		cQuestSaveListInfo = JsonHelper.ListFromJson<CGameQuestSaveInfo>(dataAsJson);
	}

	IEnumerator LinkedBossPanelInfoAccess(string filePath)
	{
		Debug.Log ("BossPanel Loaded");

		WWW www = new WWW(filePath);

		yield return www;

		string dataAsJson = www.text.ToString();

		Debug.Log (dataAsJson);

		cBossPanelListInfo = JsonHelper.ListFromJson<BossPanelInfo>(dataAsJson);
	}


    private List<T> ConstructString<T>(string _strPath)
    {
        List<T> getList = new List<T>();

        string filePath = Path.Combine(Application.streamingAssetsPath, _strPath);

		if (File.Exists (filePath)) {
			string dataAsJson = File.ReadAllText (filePath);

			getList = JsonHelper.ListFromJson<T> (dataAsJson);

			return getList;
		}


        return null;
    }

    void OnApplicationQuit()
    {
		DataSave ();
    }

    //데이터 저장시 호출된다.
    //저장이 되는 부분은 OnApplicationPuase가 TRUE 이고 플레이어가 존재할시 호출
    void DataSave()
    {
        Debug.Log("Quit And Local Save....");

        if (player == null)
            return;

		//부스터 아이템 시간 저장
		SpawnManager.Instance.shopCash.SaveCashActiveBoosterTime();

		//아르바이트 버프 해제
        SpawnManager.Instance.ReleliveArbait();

        playerData = player.changeStats;

		playerData.dGold = ScoreManager.ScoreInstance.GetGold ();
		playerData.dHonor = ScoreManager.ScoreInstance.GetHonor ();
		playerData.nRuby = ScoreManager.ScoreInstance.GetRuby ();

		creatorWeaponData = player.GetCreatorWeapon ();

        SaveArbaitData();

		SaveCreateWeaponData ();

        SavePlayerData();

        SaveEquiment();

        SaveQuestList();

		SaveBossPanelInfoList ();

        SpawnManager.Instance.ApplyArbait();

        PlayerPrefs.Save();

		Debug.Log("Save Complete!!");
        //StopAllCoroutines();
    }

    public void OnApplicationPause(bool bIsPause)
    {
        if (bIsPause)
        {
			if (player != null && SceneManager.GetActiveScene().buildIndex == 1)
            {
				DataSave();
            }
        }
    }


    public void SavePlayerData()
	{

		Debug.Log ("nMaxDays ; " + playerData.nMaxDay);
		#if UNITY_EDITOR
		string filePath = Path.Combine(Application.streamingAssetsPath, strPlayerPath);

		#elif UNITY_ANDROID
		string filePath = Path.Combine(Application.persistentDataPath, strPlayerPath);

		#endif

		playerSave.Clear ();

		playerSave.Add (playerData);

		string dataAsJson = JsonHelper.ListToJson<CGamePlayerData>(playerSave);

		Debug.Log (dataAsJson);

		File.WriteAllText(filePath, dataAsJson);
	}

    public void SaveArbaitData()
    {
#if UNITY_EDITOR
        string filePath = Path.Combine(Application.streamingAssetsPath, strArbaitPath);

#elif UNITY_ANDROID
		string filePath = Path.Combine(Application.persistentDataPath, strArbaitPath);

#endif
        string dataAsJson = JsonHelper.ListToJson<ArbaitData>(ArbaitDataBase);

        File.WriteAllText(filePath, dataAsJson);
    }

	public void SaveCreateWeaponData()
	{
		#if UNITY_EDITOR
		string filePath = Path.Combine(Application.streamingAssetsPath, strCreateWeapon);

		#elif UNITY_ANDROID
		string filePath = Path.Combine(Application.persistentDataPath, strCreateWeapon);

		#endif

		CreateWeapon = new List<CreatorWeapon> ();

		CreateWeapon.Add (creatorWeaponData);

		string dataAsJson = JsonHelper.ListToJson<CreatorWeapon>(CreateWeapon);

		File.WriteAllText(filePath, dataAsJson);
	}

    public void SaveEquiment()
    {

		#if UNITY_EDITOR
		string filePath = Path.Combine(Application.streamingAssetsPath, strInvetoryPath);

		#elif UNITY_ANDROID
		string filePath = Path.Combine(Application.persistentDataPath, strInvetoryPath);

		#endif

		if (player.GetItemListCount () != 0)
			cInvetoryInfo = player.inventory.GetItemList ();
		else
			return;

        string dataAsJson = JsonHelper.ListToJson<CGameEquiment>(cInvetoryInfo);

        File.WriteAllText(filePath, dataAsJson);
    }

    public void SaveShopList(List<CGameEquiment> _equimnet)
    {
		if (_equimnet == null)
			return;

		#if UNITY_EDITOR
		string filePath = Path.Combine(Application.streamingAssetsPath, strEquiementPath);

		#elif UNITY_ANDROID
		string filePath = Path.Combine(Application.persistentDataPath, strEquiementPath);

		#endif

        equimnetData = _equimnet;

        string dataAsJson = JsonHelper.ListToJson<CGameEquiment>(equimnetData);

        File.WriteAllText(filePath, dataAsJson);
    }

	public void SetQuestData(List<CGameQuestInfo> _QuestData)
	{

	}

	public void SaveQuestList()
	{
		if (cQuestSaveListInfo == null)
			return;

		#if UNITY_EDITOR
		string filePath = Path.Combine(Application.streamingAssetsPath, strQuestPath);

		#elif UNITY_ANDROID
		string filePath = Path.Combine(Application.persistentDataPath, strQuestPath);

		#endif


		string dataAsJson = JsonHelper.ListToJson<CGameQuestSaveInfo>(cQuestSaveListInfo);

		File.WriteAllText(filePath, dataAsJson);
	}


	public void SaveBossPanelInfoList()
	{
		if (cBossPanelListInfo == null)
			return;

		#if UNITY_EDITOR
		string filePath = Path.Combine(Application.streamingAssetsPath, strBossPanelInfoPath);

		#elif UNITY_ANDROID
		string filePath = Path.Combine(Application.persistentDataPath, strBossPanelInfoPath);

		#endif


		string dataAsJson = JsonHelper.ListToJson<BossPanelInfo>(cBossPanelListInfo);

		File.WriteAllText(filePath, dataAsJson);
	}


    public Player GetPlayer()
    {
        return player;
    }

	#region LoadTableInfo

    void Load_TableInfo_Weapon()
    {
        if (cWeaponInfo.Length != 0) return;

        string txtFilePath = "Weapon";
        TextAsset ta = LoadTextAsset(txtFilePath);
        List<string> line = LineSplit(ta.text);

        CGameWeaponInfo[] kInfo = new CGameWeaponInfo[line.Count - 1];

        for (int i = 0; i < line.Count; i++)
        {
            //Console.WriteLine("line : " + line[i]);
            if (line[i] == null) continue;
            if (i == 0) continue; 	// Title skip

            string[] Cells = line[i].Split("\t"[0]);	// cell split, tab
            if (Cells[0] == "") continue;

            kInfo[i - 1] = new CGameWeaponInfo();
            kInfo[i - 1].nIndex = int.Parse(Cells[0]);
			kInfo[i - 1].strPath = Cells[1];
			kInfo[i - 1].strName = Cells[2];
			kInfo [i - 1].dMaxComplate = double.Parse (Cells [3]);
			kInfo [i - 1].dMinusRepair= double.Parse(Cells[4]);
			kInfo [i - 1].fMinusChargingWater = float.Parse (Cells [5]);
			kInfo [i - 1].dMinusCriticalDamage = double.Parse (Cells [6]);
			kInfo [i - 1].fMinusUseWater= float.Parse(Cells[7]);
			kInfo [i - 1].fMinusCriticalChance = float.Parse (Cells [8]);
			kInfo [i - 1].fMinusAccuracy= float.Parse(Cells[9]);
			kInfo[i - 1].dGold = double.Parse(Cells[10]);
			kInfo[i - 1].dHonor = double.Parse(Cells[11]);
			kInfo[i - 1].fLimitedTime = float.Parse(Cells[12]);
			kInfo[i - 1].nGrade = int.Parse(Cells[13]);
            kInfo[i - 1].WeaponSprite = ObjectCashing.Instance.LoadSpriteFromCache(kInfo[i - 1].strPath);
        }
        cWeaponInfo = kInfo;
    }

	void Load_TableInfo_Hammer()
	{
		if (cHammerNames.Length != 0) return;

		string txtFilePath = "HammerName";
		TextAsset ta = LoadTextAsset(txtFilePath);
		List<string> line = LineSplit(ta.text);

		CGameHammer[] kInfo = new CGameHammer[line.Count - 1];

		for (int i = 0; i < line.Count; i++)
		{
			//Console.WriteLine("line : " + line[i]);
			if (line[i] == null) continue;
			if (i == 0) continue; 	// Title skip

			string[] Cells = line[i].Split("\t"[0]);	// cell split, tab
			if (Cells[0] == "") continue;

			kInfo[i - 1] = new CGameHammer();
			kInfo[i - 1].nIndex = int.Parse(Cells[0]);
			kInfo[i - 1].strName = Cells[1];

		}
		cHammerNames = kInfo;
	}

    void Load_TableInfo_Quest()
    {
        if (cQusetInfo.Length != 0) return;

        string txtFilePath = "Quest";

        TextAsset ta = LoadTextAsset(txtFilePath);

        List<string> line = LineSplit(ta.text);

        CGameQuestInfo[] kInfo = new CGameQuestInfo[line.Count - 1];

        for (int i = 0; i < line.Count; i++)
        {
            //Console.WriteLine("line : " + line[i]);
            if (line[i] == null) continue;
            if (i == 0) continue; 	// Title skip

            string[] Cells = line[i].Split("\t"[0]);	// cell split, tab
            if (Cells[0] == "") continue;

            kInfo[i - 1] = new CGameQuestInfo();
            kInfo[i - 1].nIndex = int.Parse(Cells[0]);
			kInfo[i - 1].nType = int.Parse(Cells[1]);
			kInfo[i - 1].strExplain = Cells[2];
			kInfo[i - 1].strMultipleCondition = Cells [3];
			kInfo[i - 1].nCompleteCondition = int.Parse(Cells[4]);
		
			kInfo[i - 1].nRewardHonor = int.Parse(Cells[5]);
			kInfo[i - 1].nRewardBossPotion = int.Parse(Cells[6]);
			kInfo[i - 1].nRewardRuby = int.Parse(Cells[7]);
        }

        cQusetInfo = kInfo;
    }

    void Load_TableInfo_Equiment()
    {
        if (cEquimentInfo.Length != 0) return;

        string txtFilePath = "Shop";

        TextAsset ta = LoadTextAsset(txtFilePath);

        List<string> line = LineSplit(ta.text);

        CGameEquiment[] kInfo = new CGameEquiment[line.Count - 1];

        for (int i = 0; i < line.Count; i++)
        {
            //Console.WriteLine("line : " + line[i]);
            if (line[i] == null) continue;
            if (i == 0) continue; 	// Title skip

            string[] Cells = line[i].Split("\t"[0]);	// cell split, tab
            if (Cells[0] == "") continue;

            kInfo[i - 1] = new CGameEquiment();
            kInfo[i - 1].nIndex = int.Parse(Cells[0]);
            kInfo[i - 1].strResource = Cells[1];
            kInfo[i - 1].strName = Cells[2];
            kInfo[i - 1].bIsBuy = bool.Parse(Cells[3]);
            kInfo[i - 1].nSlotIndex = int.Parse(Cells[4]);
            kInfo[i - 1].sGrade = Cells[5];
			kInfo[i - 1].fReapirPower = float.Parse(Cells[6]);
			kInfo[i - 1].fArbaitRepair = float.Parse(Cells[7]);
			kInfo[i - 1].fHonorPlus = float.Parse(Cells[8]);
			kInfo[i - 1].fGoldPlus = float.Parse(Cells[9]);
			kInfo[i - 1].fWaterChargePlus = float.Parse(Cells[10]);
			kInfo[i - 1].fCritical = float.Parse(Cells[11]);
			kInfo[i - 1].fCriticalDamage = float.Parse(Cells[12]);
			kInfo[i - 1].fBigCritical = float.Parse(Cells[13]);
			kInfo[i - 1].fAccuracyRate = float.Parse(Cells[14]);
			kInfo [i - 1].nStrenthCount = int.Parse (Cells [15]);
			kInfo [i - 1].fOptionPlus = float.Parse (Cells [16]);
			kInfo [i - 1].nBasicGold = int.Parse (Cells [17]);
        }

        cEquimentInfo = kInfo;
    }


	void Load_TableInfo_Boss()
	{
		if (bossInfo.Length != 0) return;

		string txtFilePath = "Boss";

		TextAsset ta = LoadTextAsset(txtFilePath);

		List<string> line = LineSplit(ta.text);

		Boss[] kInfo = new Boss[line.Count - 1];

		for (int i = 0; i < line.Count; i++)
		{
			//Console.WriteLine("line : " + line[i]);
			if (line[i] == null) continue;
			if (i == 0) continue; 	// Title skip

			string[] Cells = line[i].Split("\t"[0]);	// cell split, tab
			if (Cells[0] == "") continue;

			kInfo[i - 1] = new Boss();
			kInfo[i - 1].nIndex = int.Parse(Cells[0]);
			kInfo[i - 1].bossName = Cells [1];
			kInfo[i - 1].skillExplainOne =Cells[2];
			kInfo[i - 1].skillExplainTwo = Cells[3];
			kInfo[i - 1].bossWeaponName = Cells[4];
			kInfo[i - 1].GetWeaponsIndex = Cells[5];
			kInfo[i - 1].dComplate = double.Parse(Cells[6]);
			kInfo[i - 1].nGold = int.Parse(Cells[7]);
			kInfo[i - 1].nHonor = int.Parse(Cells[8]);
			kInfo[i - 1].nDia = int.Parse(Cells[9]);
			kInfo[i - 1].fDropPercent =float.Parse( Cells[10]);
			kInfo[i - 1].bossWord02 = Cells[11];
			kInfo[i - 1].bossWord03 = Cells[12];
			kInfo[i - 1].bossWord04 = Cells[13];
			kInfo[i - 1].bossWord04 = Cells[14];
		}

		bossInfo = kInfo;
	}

	void Load_TableInfo_BossWeapon()
	{
		if (bossWeaponInfo.Length != 0) return;

		string txtFilePath = "BossWeapon";

		TextAsset ta = LoadTextAsset(txtFilePath);

		List<string> line = LineSplit(ta.text);

		BossWeapon[] kInfo = new BossWeapon[line.Count - 1];

		for (int i = 0; i < line.Count; i++)
		{
			//Console.WriteLine("line : " + line[i]);
			if (line[i] == null) continue;
			if (i == 0) continue; 	// Title skip

			string[] Cells = line[i].Split("\t"[0]);	// cell split, tab
			if (Cells[0] == "") continue;

			kInfo[i - 1] = new BossWeapon();
			kInfo[i - 1].nIndex = int.Parse(Cells[0]);
			kInfo[i - 1].strResource = Cells[1];
			kInfo[i - 1].strName = Cells[2];
			kInfo[i - 1].bIsBuy = bool.Parse(Cells[3]);
			kInfo[i - 1].nSlotIndex = int.Parse(Cells[4]);
			kInfo[i - 1].sGrade = Cells[5];
			kInfo[i - 1].fReapirPower = float.Parse(Cells[6]);
			kInfo[i - 1].fArbaitRepair = float.Parse(Cells[7]);
			kInfo[i - 1].fHonorPlus = float.Parse(Cells[8]);
			kInfo[i - 1].fGoldPlus = float.Parse(Cells[9]);
			kInfo[i - 1].fWaterChargePlus = float.Parse(Cells[10]);
			kInfo[i - 1].fCritical = float.Parse(Cells[11]);
			kInfo[i - 1].fCriticalDamage = float.Parse(Cells[12]);
			kInfo[i - 1].fBigCritical = float.Parse(Cells[13]);
			kInfo[i - 1].fAccuracyRate = float.Parse(Cells[14]);
			kInfo [i - 1].nStrenthCount = int.Parse (Cells [15]);
			kInfo [i - 1].explain = Cells [17];
		}

		bossWeaponInfo = kInfo;
	}

	void Load_TableInfo_Sound()
	{
		if (cSoundsData.Length != 0) return;

		string txtFilePath = "sound";

		TextAsset ta = LoadTextAsset(txtFilePath);

		List<string> line = LineSplit(ta.text);

		CGameSoundData[] kInfo = new CGameSoundData[line.Count - 1];

		for (int i = 0; i < line.Count; i++)
		{
			//Console.WriteLine("line : " + line[i]);
			if (line[i] == null) continue;
			if (i == 0) continue; 	// Title skip

			string[] Cells = line[i].Split("\t"[0]);	// cell split, tab
			if (Cells[0] == "") continue;

			kInfo[i - 1] = new CGameSoundData();
			kInfo[i - 1].nIndex = int.Parse(Cells[0]);
			kInfo[i - 1].strName = Cells[1];
			kInfo[i - 1].strResource = Cells[2];
			kInfo[i - 1].nType = int.Parse(Cells[3]);
			kInfo[i - 1].nVolume = int.Parse(Cells[4]);
			kInfo[i - 1].nLoop = int.Parse(Cells[5]);
		}

		cSoundsData = kInfo;
	}


	void Load_TableInfo_CashShopInfo()
	{
		if (cCashItemShopInfo.Length != 0) return;

		string txtFilePath = "CashShop";

		TextAsset ta = LoadTextAsset(txtFilePath);

		List<string> line = LineSplit(ta.text);

		GameCashItemShop[] kInfo = new GameCashItemShop[line.Count - 1];

		for (int i = 0; i < line.Count; i++)
		{
			//Console.WriteLine("line : " + line[i]);
			if (line[i] == null) continue;
			if (i == 0) continue; 	// Title skip

			string[] Cells = line[i].Split("\t"[0]);	// cell split, tab
			if (Cells[0] == "") continue;

			kInfo[i - 1] = new GameCashItemShop();
			kInfo [i - 1].sItemName = Cells [0];
			kInfo[i - 1].nType = int.Parse(Cells[1]);
			kInfo[i - 1].sImagePath_01 = Cells[2];
			kInfo[i - 1].sImagePath_02 = Cells[3];
			kInfo [i - 1].sImagePath_03 = Cells [4];
			kInfo[i - 1].fContinueSec = float.Parse( Cells[5]);
			kInfo[i - 1].fRuby = float.Parse(Cells[6]);
			kInfo[i - 1].fHonor = float.Parse(Cells[7]);
			kInfo[i - 1].fGold = float.Parse(Cells[8]);
			kInfo[i - 1].fCash = float.Parse(Cells[9]);
			kInfo[i - 1].sItemContents = Cells[10];
		}

		cCashItemShopInfo = kInfo;
	}

	#endregion

	#region SplitText

    TextAsset LoadTextAsset(string _txtFile)
    {
        TextAsset ta;
        ta = Resources.Load("Data/" + _txtFile) as TextAsset;
        return ta;
    }

    public List<string> LineSplit(string text)
    {
        //Console.WriteLine("LineSplit " + text.Length);

        char[] text_buff = text.ToCharArray();

        List<string> lines = new List<string>();

        int linenum = 0;
        bool makecell = false;

        StringBuilder sb = new StringBuilder("");

        for (int i = 0; i < text.Length; i++)
        {
            char c = text_buff[i];
            //int value = Convert.ToInt32(c); Console.WriteLine(String.Format("{0:x4}", value) + " " + c.ToString());

            if (c == '"')
            {
                char nc = text_buff[i + 1];
                if (nc == '"') { i++; } //next char
                else
                {
                    if (makecell == false) { makecell = true; c = nc; i++; } //next char
                    else { makecell = false; c = nc; i++; } //next char
                }
            }

            //0x0a : LF ( Line Feed : 다음줄로 캐럿을 이동 '\n')
            //0x0d : CR ( Carrage Return : 캐럿을 제일 처음으로 복귀 )			    
            if (c == '\n' && makecell == false)
            {
                char pc = text_buff[i - 1];
                if (pc != '\n')	//file end
                {
                    lines.Add(sb.ToString()); sb.Remove(0, sb.Length);
                    linenum++;
                }
            }
            else if (c == '\r' && makecell == false)
            {
            }
            else
            {
                sb.Append(c.ToString());
            }
        }

        return lines;
    }
	#endregion

	public EquipmentEnhance GetEnhanceArbaitData(string _nIndex)
	{
		if (_nIndex == "C")
			return cEquipmentEnhance [0];
		else if (_nIndex == "B")
			return cEquipmentEnhance [1];
		else
			return cEquipmentEnhance [2];

	}


    public CGameWeaponInfo GetWeaponData(int _nGrade)
    {
		if (player == null)
			return null;

        int nRandom;

        nRandom = Random.Range(0, 7);

		CGameWeaponInfo ResultWeapon = new CGameWeaponInfo (cWeaponInfo [nRandom]);

		return ResultWeapon;
    }

    public int EquimentShopLength()
    {
        return equimnetData.Count;
    }

    public List<CGameEquiment> GetEquimentShopData()
    {
        if (equimnetData == null)
            return null;

        return equimnetData;
    }

    public CGameEquiment GetEquimentData(int nIndex)
    {
        if (cEquimentInfo == null)
            return null;

        return cEquimentInfo[nIndex];
    }

	public int GetSearchName(string strName)
	{
		for (int nIndex = 0; nIndex < cHammerNames.Length; nIndex++) 
			if (cHammerNames [nIndex].strName == strName)
				return cHammerNames [nIndex].nIndex;
		
		return -1;
	}

    #region window popup

    // 윈도우 팝업 ---------------------------------------------------------------------------------------
	//CGame.Instance.Window_notice("213123 213123 ", rt => { if (rt == "0") print("notice");  });
	public void Window_notice(string _msg, System.Action<string> _callback)
	{
		//GameObject Root_ui = GameObject.Find("root_window)"); //ui attach
		GameObject go = GameObject.Instantiate(Resources.Load("prefabs/Window_notice"), Vector3.zero, Quaternion.identity) as GameObject;
		go.transform.parent = Root_ui.transform;
		go.transform.localPosition = Vector3.zero;
		go.transform.localRotation = Quaternion.identity;
		go.transform.localScale = Vector3.one;

		CWindowNotice w = go.GetComponent<CWindowNotice>();
		w.Show(_msg, _callback);
	}

	public void Window_yesno(string _title, string _msg, System.Action<string> _callback)
	{
		//GameObject Root_ui = GameObject.Find("root_window)"); //ui attach
		GameObject go = GameObject.Instantiate(Resources.Load("prefabs/Window_yesno"), Vector3.zero, Quaternion.identity) as GameObject;
		go.transform.parent = Root_ui.transform;
		go.transform.localPosition = Vector3.zero;
		go.transform.localRotation = Quaternion.identity;
		go.transform.localScale = Vector3.one;

		CWindowYesNo w = go.GetComponent<CWindowYesNo>();
		w.Show(_title, _msg, _callback);
	}
    #endregion

    #region Arbait
    public int ArbaitLength()
    {
        return ArbaitDataBase.Count;
    }

    public ArbaitData GetArbaitData(int _id)
    {
        if (_id < 0 || ArbaitDataBase.Count <= _id)
            return null;

        return ArbaitDataBase[_id];
    }
    #endregion

	#region Inventory
    public int GetEquimentLength()
    {
        if (cEquimentInfo == null)
            return 0;

        if (cEquimentInfo.Length != 0)
            return cEquimentInfo.Length;

        else
            return 0;
    }

    public int GetInvetoryListCount()
    {
        if (cInvetoryInfo.Count != 0)
            return cInvetoryInfo.Count;

        else
            return 0;
    }
    #endregion 


	#region GooglePlayCloud

	public void DeleteData()
	{
		string dataAsString = "";

		if (!PlayerPrefs.HasKey(sSaveDataName))
			PlayerPrefs.SetString(sSaveDataName, string.Empty);
		
		dataAsString = sSaveDataName;

		if (Social.localUser.authenticated == true)
		{
			Debug.Log ("Success LoadDataGoogleCloud");
			((PlayGamesPlatform)Social.Active).SavedGame.OpenWithAutomaticConflictResolution (dataAsString,
				DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, OnDeleteGameData);
		}
		else 
		{
			Debug.Log ("Fail LoadDataGoogleCloud -> Load Local Data");
			//Load Local data -> json
		}
	}

	public void LoadDataCloud()
	{
		string nullText = "";
		string dataAsString = "";
		//string playerfilePath = Path.Combine(Application.streamingAssetsPath, strPlayerPath);

		if (!PlayerPrefs.HasKey ("FirstLogin"))
			PlayerPrefs.SetInt ("FirstLogin", 1);

		if (!PlayerPrefs.HasKey(sSaveDataName))
			PlayerPrefs.SetString(sSaveDataName, string.Empty);
		
		dataAsString = sSaveDataName;

		//isGoogleCloundDataDelete = true;

		//로그인이 되었다면 data Load
		if (Social.localUser.authenticated == true)
		{
			Debug.Log ("Success LoadDataGoogleCloud");
			((PlayGamesPlatform)Social.Active).SavedGame.OpenWithAutomaticConflictResolution (dataAsString,
				DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, OnSavedGameOpened);
		}
		else 
		{
			Debug.Log ("Fail LoadDataGoogleCloud -> Load Local Data");
			//Load Local data -> json


		}
	}


	private void LoadLocal()
	{
		//StringToGameData(PlayerPrefs.GetString(SAVE_NAME));
	}

	private void LoadGame(ISavedGameMetadata game)
	{
		Debug.Log ("LoadGame");
		Debug.Log("GetInt : " + PlayerPrefs.GetInt("FirstLogin"));
		if (PlayerPrefs.GetInt("FirstLogin") == 1)
		{
			Debug.Log ("No Player Data -> player Data init");

			//Player
			player = new Player ();
			player.Init(cInvetoryInfo, playerData,creatorWeaponData);
			//Quest
			CGameQuestSaveInfo tmpQuestSave = new CGameQuestSaveInfo();
			cQuestSaveListInfo.Add (tmpQuestSave);
		
			//BossPanelInfo
			cBossPanelInfo = new BossPanelInfo();
			cBossPanelListInfo.Add(cBossPanelInfo);

			Debug.Log (player);
			Debug.Log (cBossPanelListInfo[0]);

			PlayerPrefs.SetInt ("FirstLogin", 0);
			return;
		}
		else
			((PlayGamesPlatform)Social.Active).SavedGame.ReadBinaryData(game, OnSavedGameDataRead);
		
	}

	private void SaveGame(ISavedGameMetadata game)
	{
		

		Debug.Log ("GoogleGameSave Active");
		string divideMark = "^";

		SpawnManager.Instance.ReleliveArbait();
		//PlayerData
		string dataAsString_PlayerData = JsonHelper.ListToJson<CGamePlayerData>(playerSave);
		//ArbaitData
		string dataAsString_ArabaitData = JsonHelper.ListToJson<ArbaitData>(ArbaitDataBase);
		//Inventory
		string dataAsString_Inventory = JsonHelper.ListToJson<CGameEquiment>(cInvetoryInfo);
		//Quest
		string dataAsString_Quest = JsonHelper.ListToJson<CGameQuestSaveInfo>(cQuestSaveListInfo);
		//BossPanelInfo
		string dataAsString_BossPaenl = JsonHelper.ListToJson<BossPanelInfo>(cBossPanelListInfo);
	
		SpawnManager.Instance.ApplyArbait();

		string ResultData = dataAsString_PlayerData + divideMark + dataAsString_ArabaitData + divideMark + dataAsString_Inventory
		                    + divideMark + dataAsString_Quest + divideMark + dataAsString_BossPaenl;
		
		Debug.Log (ResultData);
		//encoding to byte array
		byte[] dataToSave = Encoding.ASCII.GetBytes(ResultData);
		//updating metadata with new description
		SavedGameMetadataUpdate update = new SavedGameMetadataUpdate.Builder().Build();
		//uploading data to the cloud
		((PlayGamesPlatform)Social.Active).SavedGame.CommitUpdate(game, update, dataToSave,
			OnSavedGameDataWritten);

		isGoogleClounSave = false;
	}
	//callback for CommitUpdate
	private void OnSavedGameDataWritten(SavedGameRequestStatus status, ISavedGameMetadata game)
	{
		Debug.Log ("OnSaveGameDataWritten");
	}


	private void OnSavedGameOpened(SavedGameRequestStatus status, ISavedGameMetadata game)
	{
		//if we are connected to the internet
		if (status == SavedGameRequestStatus.Success)
		{
			Debug.Log ("Success To Connected Internet And SaveAndLoad Data");
		

			//if we're LOADING game data
			if (!isGoogleClounSave)
				LoadGame (game);
			//if we're SAVING game data
			else
			{
				SaveGame (game);
			}
		}
		//if we couldn't successfully connect to the cloud, runs while on device,
		//the same code that is in else statements in LoadData() and SaveData()
		else
		{
			Debug.Log ("Fail To Connected Internet And SaveAndLoad Data");
			//if (!isSaving)
			//	LoadLocal();
			//else
			//	SaveLocal();
		}
	}

	//Callback For deleteData
	private void OnDeleteGameData(SavedGameRequestStatus status, ISavedGameMetadata game)
	{
		//if we are connected to the internet
		if (status == SavedGameRequestStatus.Success)
		{
			if (isGoogleCloundDataDelete == true) 
			{
				Debug.Log ("Delete Cloud Data");
				ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
				savedGameClient.Delete(game);
				isGoogleCloundDataDelete = false;
				return;
			}

		}
		else
		{
			Debug.Log ("Fail To Connected Internet And SaveAndLoad Data");
		}

	}

	//callback for ReadBinaryData
	private void OnSavedGameDataRead(SavedGameRequestStatus status, byte[] savedData)
	{

		//if reading of the data was successful
		if (status == SavedGameRequestStatus.Success)
		{
			playerData = null;
			string cloudDataString = "";
			string getCloudDataString = "";
			//if we've never played the game before, savedData will have length of 0

			if (savedData.Length == 0) 
			{
				//in such case, we want to assign "0" to our string
				Debug.Log("No Saved Data");
				//player = new Player ();
				//player.Init(cInvetoryInfo, playerData);

				//cloudDataString = "0";
			}
			//otherwise take the byte[] of data and encode it to string
			else
			{


				int inputCount = 0;
				string divideMark = "^";
				cloudDataString = Encoding.ASCII.GetString (savedData);
				//Debug.Log ("DataLength : " + cloudDataString);
				Debug.Log(cloudDataString.Length);
				for (int i = 0; i < cloudDataString.Length; i++)
				{

					if (cloudDataString [i] == '^') {
						Debug.Log ("I : " + i);
						//getCloudDataString.Remove(i - 1 ,1);
						//Debug.Log ("Delete Divide Mark : " + getCloudDataString[i]);
						Debug.Log (getCloudDataString);

						inputCount++;
						if (inputCount == 1) {
							playerData = JsonHelper.ListFromJson<CGamePlayerData> (getCloudDataString) [0];
							player.Init (cInvetoryInfo, playerData,creatorWeaponData);
							Debug.Log ("Google Loaded GameData  Player = RepairPower : " + playerData.dRepairPower + "  nDays : " + playerData.nDay + "  nMaxDays : " + playerData.nMaxDay);
							getCloudDataString = "";
							continue;
						}

						if (inputCount == 2) {
							ArbaitDataBase = JsonHelper.ListFromJson<ArbaitData> (getCloudDataString);
							Debug.Log (" Google Loaded GameData Arbait Data");
							getCloudDataString = "";
							continue;
						}

						if (inputCount == 3) {
							Debug.Log ("Google Loaded GameData Invetory Data");
							cInvetoryInfo = JsonHelper.ListFromJson<CGameEquiment> (getCloudDataString);
							getCloudDataString = "";
							continue;
						}
						if (inputCount == 4) {
							Debug.Log ("Google Loaded GameData QuestInfo Data");
							cQuestSaveListInfo = JsonHelper.ListFromJson<CGameQuestSaveInfo> (getCloudDataString);
							getCloudDataString = "";
							continue;
						}
					


					} 
					else 
					{
						getCloudDataString += cloudDataString [i];
						if (i == cloudDataString.Length - 1) 
						{
							Debug.Log (getCloudDataString);
							Debug.Log ("Google Loaded GameData BossPanelInfo Data");
							cBossPanelListInfo = JsonHelper.ListFromJson<BossPanelInfo> (getCloudDataString);
							getCloudDataString = "";
							continue;
						}
					}
					
						
				}

				//playerData = JsonHelper.ListFromJson<CGamePlayerData>(cloudDataString)[0];


				//player.Init (cInvetoryInfo, playerData);

			}
			//getting local data (if we've never played before on this device, localData is already
			//"0", so there's no need for checking as with cloudDataString)
			//string localDataString = PlayerPrefs.GetString(SAVE_NAME);

			//StartCoroutine(playerDataLoad(cloudDataString));

		
			//this method will compare cloud and local data
			//StringToGameData(cloudDataString, localDataString);
		}


	}

	//SaveLocal -> byte -> cloud
	public void SaveCloudData()
	{
		Debug.Log ("Save Data Cloud !!");
		DataSave();					
		GetPlayerSaveList ();		//Confirm

		isGoogleClounSave = true;
		LoadDataCloud ();				//cloud Save
	}

	public void LoadCloudData()
	{
		Debug.Log ("Load Cloud Data!!");
		isGoogleClounSave = false;
		LoadDataCloud ();


	}

	public void GetPlayerSaveList()
	{
		Debug.Log (playerSave [0].fAccuracyRate + "/" + playerSave [0].fArbaitsPower + "/" + playerSave [0].fBigSuccessed + "/" + playerSave [0].fCriticalChance
		+ "/" + playerSave [0].dRepairPower + "/" + playerSave [0].strName + "/" + playerSave [0].nMaxDay + "/" + playerSave [0].nEnhanceMaxWaterLevel);
	}

	#endregion

	public CGameSoundData Get_TableInfo_sound(int _nIndex)
	{
		for (int nIndex = 0; nIndex < cSoundsData.Length; nIndex++)
			if (cSoundsData [nIndex].nIndex == _nIndex)
				return cSoundsData [nIndex];

		return null;
	}

	#region UnityAds

	//ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ

	public void ShowRewardedAd_Creator(ResultEpicUI _ResultEpicUI)
	{
		if (resultEpicUI == null)
			resultEpicUI = _ResultEpicUI;

		if (Advertisement.IsReady("rewardedVideo"))
		{
			var options = new ShowOptions { resultCallback = HandleShowResult_Creator };
			Advertisement.Show("rewardedVideo", options);
		}
	}

	private void HandleShowResult_Creator(ShowResult result)
	{
		switch (result)
		{
		case ShowResult.Finished:
			Debug.Log ("The ad was successfully shown.");
			//
			// YOUR CODE TO REWARD THE GAMER
			// Give coins etc.

			resultEpicUI.ShowAd ();

			break;

		case ShowResult.Failed:
			Debug.LogError("The ad failed to be shown.");
			break;
		}
	}


	public void InitAds()
	{
		if(Advertisement.isSupported)
			Advertisement.Initialize (sAds_AndroidGameID, true);
	}

	public void ShowSkipAd_Quest(QusetManager _questManager)
	{
		if (questManager == null)
			questManager = _questManager;

		isQuestAdsOn = true;

		if (Advertisement.IsReady("video"))
		{
			var options = new ShowOptions { resultCallback = HandleShowResult };
			Advertisement.Show("video", options);
		}
	}

	public void ShowRewardedAd_Quest(QusetManager _questManager)
	{
		if (questManager == null)
			questManager = _questManager;

		isQuestAdsOn = true;

		if (Advertisement.IsReady("rewardedVideo"))
		{
			var options = new ShowOptions { resultCallback = HandleShowResult };
			Advertisement.Show("rewardedVideo", options);
		}
	}



	private void HandleShowResult(ShowResult result)
	{
		switch (result)
		{
		case ShowResult.Finished:
			Debug.Log ("The ad was successfully shown.");
			//
			// YOUR CODE TO REWARD THE GAMER
			// Give coins etc.
			if (isQuestAdsOn == true) 
			{
				isQuestAdsOn = false;
				if (questManager.questObjects.Count == questManager.nQuestMaxHaveCount) 
				{
					questManager.ShowEmptyQuestFull ();
					return;
				}
				else
					questManager.QuestInit ();
			}
			break;
		case ShowResult.Skipped:
			Debug.Log("The ad was skipped before reaching the end.");
			if (isQuestAdsOn == true) 
			{
				if (questManager.questObjects.Count == questManager.nQuestMaxHaveCount) 
				{
					questManager.ShowEmptyQuestFull ();
					return;
				}
				else
					questManager.QuestInit ();
			}
			break;

		case ShowResult.Failed:
			Debug.LogError("The ad failed to be shown.");
			break;
		}
	}


	public void ShowRewardAdd_Boss(BossCreator _bossCreator)
	{
		if (bossCreator == null)
			bossCreator = _bossCreator;

		if (Advertisement.IsReady("rewardedVideo"))
		{
			var options = new ShowOptions { resultCallback = HandleShowResult_Boss };
			Advertisement.Show("rewardedVideo", options);
		}
	}

	private void HandleShowResult_Boss(ShowResult result)
	{
		switch (result)
		{
		case ShowResult.Finished:
			Debug.Log ("The ad was successfully shown.");
			Debug.Log ("보스 초대장 풀 충전!!");
			//
			// YOUR CODE TO REWARD THE GAMER
			// Give coins etc.

			cBossPanelListInfo [0].nBossInviteMentCount = 5;

			break;

		case ShowResult.Failed:
			Debug.LogError("The ad failed to be shown. (BossAds)");
			break;
		}
	}




           
	#endregion

	#region UnityInAppPurchase


	#endregion



}

enum E_Equiment
{
	E_REPAIR = 6,
	E_ARBAIT,
	E_HONOR,
	E_GOLD,
	E_WATERCHARGE,
	E_CRITICAL,
	E_CRITICALD,
	E_BIGCRITICAL,
	E_ACCURACY,
	E_MAX,
}

enum E_CREATOR
{
	E_REPAIRPERCENT = 0,
	E_ARBAIT,
	E_HONOR,
	E_GOLD,
	E_WATERCHARGE,
	E_WATERUSE,
	E_CRITICAL,
	E_CRITICALD,
	E_BIGCRITICAL,
	E_ACCURACY,
	E_MAX,
	E_BOSS_ICE,
	E_BOSS_SASIN,
	E_BOSS_FIRE,
	E_BOSS_RUSIU,
	E_EPIC,
}

#region Classese

[System.Serializable]
public class AllArbaitData
{
	public ArbaitData[] allArbaitData;
}

[System.Serializable]
public class CGameHammer
{
	public int nIndex = 0;
	public string strName = "";
}

[System.Serializable]
public class CGameEquiment
{
    public int nIndex 				= 0;
    public string strResource 		= "";
    public string strName 			= "";
    public bool bIsBuy 				= false;
    public int nSlotIndex 			= 0;
	public string sGrade			= "";
	public float fReapirPower 		= 0;
	public float fArbaitRepair 		= 0;
	public float fHonorPlus 		= 0;
	public float fGoldPlus 		= 0;
	public float fWaterChargePlus 	= 0;
	public float fCritical 			= 0;
	public float fCriticalDamage 	= 0;
	public float fBigCritical 		= 0;
	public float fAccuracyRate 		= 0;
	public int nStrenthCount 		= 0;
    public bool bIsEquip 			= false;
	public float fOptionPlus = 1.0f;
	public int nBasicGold = 1000;

	public CGameEquiment(){}

	public CGameEquiment(CGameEquiment _equimentData)
	{
		nIndex = _equimentData.nIndex;
		strResource = _equimentData.strResource;
		strName = _equimentData.strName;
		bIsBuy = _equimentData.bIsBuy;
		nSlotIndex = _equimentData.nSlotIndex;
		sGrade = _equimentData.sGrade;
		fReapirPower = _equimentData.fReapirPower;
		fArbaitRepair = _equimentData.fArbaitRepair;
		fHonorPlus = _equimentData.fHonorPlus;
		fGoldPlus = _equimentData.fGoldPlus;
		fWaterChargePlus = _equimentData.fWaterChargePlus;
		fCriticalDamage = _equimentData.fCriticalDamage;
		fBigCritical = _equimentData.fBigCritical;
		fAccuracyRate = _equimentData.fAccuracyRate;
		nStrenthCount = _equimentData.nStrenthCount;
		bIsEquip = _equimentData.bIsEquip;
		fOptionPlus = _equimentData.fOptionPlus;
		nBasicGold = _equimentData.nBasicGold;
	}

	public virtual string GetExplain(){
		return "";
	}
}


[System.Serializable]
public class CreatorWeapon
{
	public string strResource 		= "";
	public string strName 			= "";
	public int nGrade 				= 0;		//옵션 등급
	public double dRepair		 	= 0.0f;		//수리력
	public float fRepairPercent		= 0.0f;		//수리력 퍼센트
	public float fArbaitRepair 		= 0.0f;		//아르바이트 수리력
	public float fPlusHonorPercent 	= 0.0f ;	//명예 추가 증가량
	public float fPlusGoldPercent 	= 0.0f;		//골드 추가 증가량
	public float fWaterPlus 		= 0.0f;		//물 추가 수치
	public float fActiveWater 		= 0.0f;		//물 사용시 추가 증가
	public float fCriticalChance 	= 0.0f;		//크리티컬 찬스
	public float fCriticalDamage 	= 0.0f;		//크리티컬 데미지
	public float fBigSuccessed 		= 0.0f;		//대 성공 확률 
	public float fAccuracyRate 		= 0.0f;		//명중률

	public float fIceBossValue = 0.0f;
	public float fRusiuBossValue = 0.0f;
	public float fSasinBossValue = 0.0f;
	public float fFireBossValue = 0.0f;

	public int nEpicIndex = -1;
	public int nCostDay = 0;
	public bool bIsLock = false;
	public Sprite WeaponSprite = null;

	public CreatorWeapon()
	{
	}

	public CreatorWeapon(CreatorWeapon _creatorWeapon)
	{
		strResource = _creatorWeapon.strResource;
		strName = _creatorWeapon.strName;
		nGrade = _creatorWeapon.nGrade;
		dRepair = _creatorWeapon.dRepair;
		fRepairPercent = _creatorWeapon.fRepairPercent;
		fArbaitRepair = _creatorWeapon.fArbaitRepair;
		fPlusHonorPercent = _creatorWeapon.fPlusHonorPercent;
		fPlusGoldPercent = _creatorWeapon.fPlusGoldPercent;
		fWaterPlus = _creatorWeapon.fWaterPlus;
		fActiveWater = _creatorWeapon.fActiveWater;
		fCriticalChance = _creatorWeapon.fCriticalChance;
		fCriticalDamage = _creatorWeapon.fCriticalDamage;
		fBigSuccessed = _creatorWeapon.fBigSuccessed;
		fAccuracyRate = _creatorWeapon.fAccuracyRate;
		fIceBossValue = _creatorWeapon.fIceBossValue;
		fRusiuBossValue = _creatorWeapon.fRusiuBossValue;
		fSasinBossValue = _creatorWeapon.fSasinBossValue;
		fFireBossValue = _creatorWeapon.fFireBossValue;
		nEpicIndex = _creatorWeapon.nEpicIndex;
		nCostDay = _creatorWeapon.nCostDay;
		bIsLock = _creatorWeapon.bIsLock;
	}
}

[System.Serializable]
public class ArbaitData
{
    //id값
    public int index;
    //레벨
    public int level;
    //이름
    public string name;

    //현재 등급
	public int grade;

    //배치 위치 (-1 = 배치안함) 0, 1, 2
    public int batch;

    //수리 량
	public double dRepairPower;

	//공격속도
	public float fAttackSpeed;

	//크리티컬 
	public float fCritical;

	//명중률
	public float fAccuracyRate;

	//특수능력 설명 
	public string strExplains;

    //특스 능력들 증가량
    public float fSkillPercent;

	//스킬 지속시간 
	public float fCurrentFloat;

	//영입 카운트 
	public int nScoutCount;

	//영입 골드 
	public int nScoutGold;

	//영입 명예
	public int nScoutHonor;

	//영입 다이아 
	public int nScoutDia;

	//다이아로 구매 가능한지
	public int nIsDia;

	//구매 했는지
	public bool bIsBuyCheck;


	public ArbaitData(ArbaitData _data)
	{
		index = _data.index;

		level= _data.level;
		name = _data.name;

		grade = _data.grade;

		batch = _data.batch;

		dRepairPower = _data.dRepairPower;

		fAttackSpeed = _data.fAttackSpeed;

		fCritical = _data.fCritical;

		fAccuracyRate = _data.fAccuracyRate;

		strExplains= _data.strExplains;

		fSkillPercent= _data.fSkillPercent;

		fCurrentFloat= _data.fCurrentFloat;

		nScoutCount= _data.nScoutCount;

		nScoutGold= _data.nScoutGold;

		nScoutHonor= _data.nScoutHonor;

		nScoutDia= _data.nScoutDia;

		nIsDia= _data.nIsDia;

		bIsBuyCheck= _data.bIsBuyCheck;
	}
}

[System.Serializable]

public class CGameWeaponInfo
{
    public int nIndex = 0;
    public string strPath = string.Empty;
	public string strName = string.Empty;
	public double dMaxComplate = 0;
	public double dMinusRepair = 0;
	public float fMinusChargingWater = 0f;
	public float fMinusCriticalChance = 0f;
	public float fMinusUseWater = 0f;
	public double dMinusCriticalDamage = 0f;
	public float fMinusAccuracy = 0f;
	public double dGold = 0.0f;
	public double dHonor = 0.0f;
	public float fLimitedTime = 0.0f;
	public int nGrade = 0;
    public Sprite WeaponSprite = null;

	public CGameWeaponInfo()
	{
	}

	public CGameWeaponInfo(CGameWeaponInfo weaponData)
	{
		nIndex = weaponData.nIndex;
		strName = weaponData.strName;
		strPath = weaponData.strPath;
		dMaxComplate = weaponData.dMaxComplate;
		dMinusRepair = weaponData.dMinusRepair;
		fMinusChargingWater = weaponData.fMinusChargingWater;
		fMinusCriticalChance = weaponData.fMinusCriticalChance;
		fMinusUseWater = weaponData.fMinusUseWater;
		dMinusCriticalDamage = weaponData.dMinusCriticalDamage;
		fMinusAccuracy = weaponData.fMinusAccuracy;
		dGold = weaponData.dGold;
		dHonor = weaponData.dHonor;
		fLimitedTime = weaponData.fLimitedTime;
		nGrade = weaponData.nGrade;
		WeaponSprite = weaponData.WeaponSprite;
	}
}

[System.Serializable]
public class CGameArbaitGrade
{
	public int nNextLevel;
	public int nPercentPlusRepair;
	public int nPercentPlusAccuracy;
	public int nPercentPlusCritical;
	public int nPercentPlusSkill;
	public int nGoldCost;
	public int nHonorCost;
}


[System.Serializable]
public class CGamePlayerEnhance
{
    public int nIndex;
	public string strName;
	public float fPlusPercentValue;
    public int nGoldCost;
	public int nHonorCost;
}

//퀘스트 파싱 클래스
[System.Serializable]
public class CGameQuestInfo
{
    public int nIndex = 0;
    public int nType = 0;
	public string strExplain = "";
	public string strMultipleCondition = "";
	public int nCompleteCondition = 0;
    

	public int nRewardHonor=0;
	public int nRewardBossPotion =0;
	public int nRewardRuby = 0;

}
//퀘스트 저장 클래스
[System.Serializable]
public class CGameQuestSaveInfo
{
	//처음 실행시
	public bool bIsFirstActive;
	//구글 클라우드 저장이 되어있는지
	public bool bIsGoogleSave;
	//구글 클라우드에서 로드 하는지 않하는지
	public bool bIsGoogleLoad;
	//해당 퀘스트의 인덱스와 진행도
	public int nQuestIndex01;
	public int nQuestIndex01_ProgressValue;
	public int nQuestIndex02;
	public int nQuestIndex02_ProgressValue;
	public int nQuestIndex03;
	public int nQuestIndex03_ProgressValue;
	public int nCurLeftMin;
	public float fCurLeftSec;

	public CGameQuestSaveInfo()
	{
		bIsFirstActive = true;
		bIsGoogleSave = false;
		bIsGoogleLoad = false;
		nQuestIndex01 = -1;
		nQuestIndex01_ProgressValue = -1;
		nQuestIndex02 = -1;
		nQuestIndex02_ProgressValue = -1;
		nQuestIndex03 = -1;
		nQuestIndex03_ProgressValue = -1;
		nCurLeftMin = -1;
		fCurLeftSec = -1.0f;
	}
}


[System.Serializable]
public class CGameEnhanceData
{
    public int nLevel;
    public int nPercent;
    public int nGoldCost;
    public int nHonorCost;
}

[System.Serializable]
public class CGamePlayerData
{
    public string strName;					//닉네임			
	public double dRepairPower;				//수리력 
    public float fArbaitsPower;				//알바수리력 
	public double dGoldPlusPercent;			//골드추가 증가량
	public double dHornorPlusPercent;		//명예추가 증가량
	public double dGold;
	public double dHonor;
	public int nRuby;
    public float fWaterPlus;				//물증가량
    public float fMaxWater;					//물최대치 증가량 
    public float fCriticalChance;			//크리티컬 확률
	public double dCriticalDamage;			//크리데미지
    public float fBigSuccessed;				//대성공
	public float fFeverTime;				//피버 지속시간 
    public float fAccuracyRate;				//정확도
    public int nBlackSmithLevel;			//대장간 레벨
    public int nEnhanceRepaireLevel;		//수리력 레벨
    public int nEnhanceMaxWaterLevel;		//물최대치 레벨
	public int nEnhanceWaterPlusLevel;		//물 추가 증가치 레벨
	public int nEnhanceAccuracyRateLevel;	//명중률 증가 레벨
	public int nEnhanceCriticalLevel;		//크리티컬 확률 레벨
	public int nEnhanceArbaitLevel;			//아르바이트 강화 레벨
	public int nFeverTimeLevel;				//피버 타임 강화 레벨 
    public int nSasinMaterial;				//보스 소울 
	public int nRusiuMaterial;				//보스 소울 
	public int nIceMaterial;				//보스 소울 
	public int nFireMaterial;				//보스 소울 
    public int nDay;						//현재 일차
	public int nMaxDay;						//맥스 일차
	public int nFaieldGuest;				//실패 손님
	public int nSuccessedGuest;				//성공 손님
    public int nGuestCount;					//손님 수
	public int nShopMaxCount;				//상점 리젠 맥스 수치
	public int nGuestBuffMinutes;
	public float fGuestBuffSecond;
	public int nGoldPlusBuffMinutes;
	public float fGoldPlusBuffSecond;
	public int nHonorPlusBuffMinutes;
	public float fHonorPlusBuffSecond;
	public int nAttackBuffMinutes;
	public float fAttackBuffSecond;
	public bool bIsShopChange;
	public int nShopMinutes;
	public float fShopSecond;
	public bool bIsBeginnerPackageBuy;
	public bool bIsBossIcePackageBuy;
	public bool bIsBossSasinPackageBuy;
	public bool bIsBossFirePackageBuy;
	public bool bIsBossMusicPackageBuy;


	public CGamePlayerData(CGamePlayerData playerData)
	{
		strName = playerData.strName;
		dRepairPower = playerData.dRepairPower;
		fArbaitsPower = playerData.fArbaitsPower;		 
		dHornorPlusPercent = playerData.dHornorPlusPercent;		
		dGold = playerData.dGold;
		dHonor = playerData.dHonor;
		nRuby = playerData.nRuby;
		dGoldPlusPercent= playerData.dGoldPlusPercent;		
		fWaterPlus= playerData.fWaterPlus;		
		fMaxWater = playerData.fMaxWater;		 
		fCriticalChance= playerData.fCriticalChance;		
		dCriticalDamage = playerData.dCriticalDamage;		
		fBigSuccessed = playerData.fBigSuccessed;	
		fFeverTime = playerData.fFeverTime;
		fAccuracyRate= playerData.fAccuracyRate;		
		nBlackSmithLevel= playerData.nBlackSmithLevel;
		nEnhanceRepaireLevel= playerData.nEnhanceRepaireLevel;
		nEnhanceMaxWaterLevel= playerData.nEnhanceMaxWaterLevel;
		nEnhanceWaterPlusLevel= playerData.nEnhanceWaterPlusLevel;
		nEnhanceAccuracyRateLevel= playerData.nEnhanceAccuracyRateLevel;
		nEnhanceCriticalLevel= playerData.nEnhanceCriticalLevel;
		nEnhanceArbaitLevel = playerData.nEnhanceArbaitLevel;
		nFeverTimeLevel = playerData.nFeverTimeLevel;
        nSasinMaterial = playerData.nSasinMaterial;
        nRusiuMaterial = playerData.nRusiuMaterial;
        nIceMaterial = playerData.nIceMaterial;
        nFireMaterial = playerData.nFireMaterial;
        nDay = playerData.nDay;
		nMaxDay = playerData.nMaxDay;
		nFaieldGuest = playerData.nFaieldGuest;
		nSuccessedGuest = playerData.nSuccessedGuest;
        nGuestCount = playerData.nGuestCount;
		nShopMaxCount = playerData.nShopMaxCount;
		nGuestBuffMinutes = playerData.nGuestBuffMinutes;
		fGuestBuffSecond = playerData.fGuestBuffSecond;
		nGoldPlusBuffMinutes = playerData.nGoldPlusBuffMinutes;
		fGoldPlusBuffSecond = playerData.fGoldPlusBuffSecond;
		nHonorPlusBuffMinutes = playerData.nHonorPlusBuffMinutes;
		fHonorPlusBuffSecond = playerData.fHonorPlusBuffSecond;
		nAttackBuffMinutes = playerData.nAttackBuffMinutes;
		fAttackBuffSecond = playerData.fAttackBuffSecond;
		bIsShopChange = playerData.bIsShopChange;
		nShopMinutes = playerData.nShopMinutes;
		fShopSecond = playerData.fShopSecond;
		bIsBeginnerPackageBuy = playerData.bIsBeginnerPackageBuy;
		bIsBossIcePackageBuy = playerData.bIsBossIcePackageBuy;
		bIsBossSasinPackageBuy = playerData.bIsBossSasinPackageBuy;
		bIsBossFirePackageBuy = playerData.bIsBossFirePackageBuy;
		bIsBossMusicPackageBuy = playerData.bIsBossMusicPackageBuy;
	}
}


[System.Serializable]
public class CGameMainWeaponOption
{
	public int nIndex = 0;
	public string strOptionName = "";
	public string strOptionExplain = "";
	public int nValue = 0;
	public bool bIsLock = false;
}

[System.Serializable]
public class Boss
{
	//id값
	public int nIndex;
	//이름
	public string bossName;
	//Passive 01
	public string skillExplainOne;
	//Passive 02
	public string skillExplainTwo;
	//BossWeaponName
	public string bossWeaponName;
	//Weapon reward index
	public string GetWeaponsIndex;
	//완성도
	public double dComplate;
	//골드
	public int nGold;
	//명예
	public int nHonor;
	//보석
	public int nDia;
	//장비 드랍 확률
	public float fDropPercent;
	//보스 대사 01
	public string bossWord01;
	//보스 대사 02
	public string bossWord02;
	//보스 대사 03
	public string bossWord03;
	//보스 대사 04
	public string bossWord04;
}

[System.Serializable]
public class BossWeapon : CGameEquiment
{
	public string explain;

	public override string GetExplain ()
	{
		return explain;
	}
}

[System.Serializable]
public class BossPanelInfo
{
	public bool isSaved;

	public bool isUnlockIceBoss;
	public bool isUnlockSasinBoss;
	public bool isUnlockFireBoss;
	public bool isUnlockMusicBoss;

	public bool isFirstFightToIceBoss;
	public bool isFirstFightToSasinBoss;
	public bool isFirstFightToFireBoss;
	public bool isFirstFightToMusicBoss;

	public int nBossInviteMentCount;
	public int nBossPotionCount;

	public int nBossSasinLeftCount;
	public int nBossMusicLeftCount;
	public int nBossIceLeftCount;
	public int nBossFireLeftCount;

	public int nBossInviteMentCurMin;
	public float fBossInviteMentCurSec;
	public int nBossRegenCurMin;
	public float fBossRegenCurSec;

	public int nBossSasinCurLevel;
	public int nBossMusicCurLevel;
	public int nBossIceCurLevel;
	public int nBossFireCurLevel;

	


	public BossPanelInfo()
	{
		isSaved = false;

		isUnlockIceBoss = false;
		isUnlockSasinBoss = false;
		isUnlockFireBoss = false;
		isUnlockMusicBoss = false;

		isFirstFightToIceBoss = false;
		isFirstFightToSasinBoss = false;
		isFirstFightToFireBoss = false;
		isFirstFightToMusicBoss = false;

		nBossInviteMentCount = 10;
		nBossPotionCount = 0;

		nBossSasinLeftCount = 3;
		nBossMusicLeftCount = 3;
		nBossIceLeftCount = 3;
		nBossFireLeftCount = 3;


		nBossSasinCurLevel = 1;
		nBossMusicCurLevel = 1;
		nBossIceCurLevel = 1;
		nBossFireCurLevel = 1;

		nBossInviteMentCurMin = 0;
		fBossInviteMentCurSec = 0f;
		nBossRegenCurMin = 0;
		fBossRegenCurSec = 0f;

	}
}

[System.Serializable]
public class CGameSoundData
{
	public int nIndex= 0;
	public string strName ="";
	public string strResource = "";
	public int nType = 0;
	public int nVolume = 100;
	public int nLoop = 0;
}

[System.Serializable]
public class ArbaitEnhance
{
	public int nIndex;
	public float fRepairPercent;
	public float fCriticalPercent;
	public float fSpecialPercent;
	public float fBasicGold;
	public float fBasicHonor;
	public float fPlusGoldValue;
	public float fPlusHonorValue;
}

[System.Serializable]
public class SmithEnhance
{
	public int nIndex;
	public float fBasicPercent;
	public float fPlusPercent;
	public float fBasicGold;
	public float fBasicHonor;
	public float fPlusGoldValue;
	public float fPlusHonorValue;
}

[System.Serializable]
public class EquipmentEnhance
{
	public int nIndex;
	public float fPlusPercent;
	public float fBasicGold;
	public float fBasicHonor;
	public float fPlusGoldValue;
	public float fPlusHonorValue;
}

[System.Serializable]
public class GameCashItemShop
{
	public string sItemName;
	public int nType;
	public string sImagePath_01;
	public string sImagePath_02;
	public string sImagePath_03;
	public float fContinueSec;
	public float fRuby;
	public float fHonor;
	public float fGold;
	public float fCash;
	public string sItemContents;
}


#endregion