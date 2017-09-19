using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ReadOnlys;

public class Shop : MonoBehaviour {

    public int nEquimentLength;

	public int nCurMin;
	public float fCurSec;

    private int nShopCount = 0;
    private int nShopMaxLength = 6;

	private const int nInitTime_Min = 59;
	private const int nInitTime_Sec = 59;

    public Transform parentPanel;
    public GameObject shopButton;

    public Inventory inventory;
    public ShopShowPanel showPanel;

    ShopButton[] ShopList;

    List<CGameEquiment> EquimentList = new List<CGameEquiment>();

    System.DateTime StartDate = new System.DateTime();

    System.DateTime EndData;

    System.TimeSpan timeCal;

    private string strTime ="";

	Player playerData;

	public Text TimerText;

	public System.Action rt;

    void Awake()
    {
        ShopList = new ShopButton[nShopMaxLength];

        for(int nIndex = 0; nIndex < nShopMaxLength; nIndex++)
        {
            GameObject obj = Instantiate(shopButton) as GameObject;

            ShopButton shopScript = obj.GetComponent<ShopButton>();

			obj.transform.SetParent(parentPanel,false);

			obj.transform.localScale = Vector3.one;

            ShopList[nIndex] = shopScript;
        }

        nEquimentLength = GameManager.Instance.GetEquimentLength();

		playerData = GameManager.Instance.GetPlayer ();

		GameManager.Instance.shop = this;

		StartCoroutine (Timer (playerData.changeStats.nShopMinutes, (int)playerData.changeStats.fShopSecond));
    }

	public void SaveTime()
	{
		System.DateTime EndData = System.DateTime.Now;
		PlayerPrefs.SetString ("NowTime", EndData.ToString ());
		PlayerPrefs.Save ();

		playerData.changeStats.nShopMinutes = nCurMin;
		playerData.changeStats.fShopSecond = fCurSec;

		Debug.Log ("EndTime :" + EndData.ToString ());
	}

    void OnEnable()
    {
        EquimentList = GameManager.Instance.GetEquimentShopData();

        if (EquimentList == null)
            EquimentList = new List<CGameEquiment>();

		if(CheckIsTimer())
		{
            PlayerPrefs.SetString("NowTime", EndData.ToString());

            EquimentList.Clear();

			for (int nIndex = 0; nIndex < playerData.GetShopMaxCount(); nIndex++)
            {
                CGameEquiment cGameEquiment = GetEquiment();

				ShopList[nIndex].GetEquiment(this, inventory, showPanel, cGameEquiment);

                EquimentList.Add(cGameEquiment);
            }

			FirstCheck ();

			StartCoroutine (Timer (nInitTime_Min, nInitTime_Sec));
        }
        else
        {
			if (EquimentList.Count != 0) {
				
				for (int nIndex = 0; nIndex < EquimentList.Count; nIndex++) {
					CGameEquiment cGameEquiment = EquimentList [nIndex];

					ShopList [nIndex].GetEquiment (this, inventory, showPanel, cGameEquiment);
				}

				FirstCheck ();

			}
            //완전 처음 일 경우 
            else
            {
				for(int nIndex = 0; nIndex < playerData.GetShopMaxCount(); nIndex++)
                {
                    CGameEquiment cGameEquiment = GetEquiment();

					ShopList[nIndex].GetEquiment(this,inventory, showPanel,cGameEquiment);

                    EquimentList.Add(cGameEquiment);
                }

				FirstCheck ();

				StartCoroutine (Timer (nInitTime_Min, nInitTime_Sec));
			}
        }

        GameManager.Instance.SaveShopList(EquimentList);
    }

    private CGameEquiment GetEquiment()
    {
        CGameEquiment resultEquiment = new CGameEquiment();


        CGameEquiment getEquiment = GameManager.Instance.GetEquimentData(Random.Range(0, nEquimentLength-1));

        if (getEquiment == null)
            return GetEquiment();

        resultEquiment.nIndex = getEquiment.nIndex;
		resultEquiment.sGrade = getEquiment.sGrade;
        resultEquiment.strName = getEquiment.strName;
        resultEquiment.nSlotIndex = getEquiment.nSlotIndex;
        resultEquiment.strResource = getEquiment.strResource;

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

	public bool CheckIsTimer()
	{
		if (PlayerPrefs.HasKey("NowTime"))
		{
			strTime = PlayerPrefs.GetString("NowTime");

			StartDate = System.Convert.ToDateTime(strTime);
		}

		EndData = System.DateTime.Now;

		timeCal = EndData - StartDate;

		int nStartTime = StartDate.Hour * 3600 + StartDate.Minute * 60 + StartDate.Second;
		int nEndTime = EndData.Hour * 3600 + EndData.Minute * 60 + EndData.Second;

		int nCheck = Mathf.Abs(nEndTime - nStartTime);

		//1시간이 지났거나 하루차이가 있을 경우
		if (timeCal.Days != 0 || nCheck >= 3600)
			return true;
		
		else
			return false;

		

	}

	public IEnumerator Timer(int _curMin, int _curSec)
	{
		int second = 0;

		fCurSec = (float)_curSec;
		nCurMin = _curMin;


		while (nCurMin >= 0f) 
		{
			fCurSec -= Time.deltaTime;
			second = (int)fCurSec;

			if(second < 10)
				TimerText.text = nCurMin.ToString () + ":" +"0"+second.ToString ();
			else
				TimerText.text = nCurMin.ToString () + ":" + second.ToString ();

			if (nCurMin == 0 && second <= 0f)
				break;	

			if (nCurMin != 0 && second == 0f) 
			{
				fCurSec = 59f;
				nCurMin--;
			}
				
			yield return null;
		}

		PlayerPrefs.SetString("NowTime", EndData.ToString());

		EquimentList.Clear();

		for (int nIndex = 0; nIndex < playerData.GetShopMaxCount(); nIndex++)
		{
			CGameEquiment cGameEquiment = GetEquiment();

			ShopList[nIndex].GetEquiment(this,inventory, showPanel, cGameEquiment);

			EquimentList.Add(cGameEquiment);
		}

		FirstCheck ();

		StartCoroutine (Timer (nInitTime_Min, nInitTime_Sec));

		yield  break;
	}

	public void ShopInit()
	{
		GameManager.Instance.Window_yesno ("50", rt => 
		{ 
			if (rt == "0") 
			{
				GameManager.Instance.ShowRewardedAd_Shop(this);
			} 
			else if (rt == "1") 
			{
					if(50 <= ScoreManager.ScoreInstance.GetRuby())
					{
						PlayerPrefs.SetString("NowTime", EndData.ToString());

						EquimentList.Clear();

						for (int nIndex = 0; nIndex < playerData.GetShopMaxCount(); nIndex++)
						{
							CGameEquiment cGameEquiment = GetEquiment();

							ShopList[nIndex].GetEquiment(this,inventory, showPanel, cGameEquiment);

							EquimentList.Add(cGameEquiment);
						}

						nCurMin = nInitTime_Min;
						fCurSec = (float)nInitTime_Sec;
					}
					else
					{
						GameManager.Instance.Window_notice("루비가 부족합니다",null);
					}
			}
		}
		);

		GameManager.Instance.SaveShopList(EquimentList);
	}

	public void FinishedAds()
	{
		PlayerPrefs.SetString("NowTime", EndData.ToString());

		EquimentList.Clear();

		for (int nIndex = 0; nIndex < playerData.GetShopMaxCount(); nIndex++)
		{
			CGameEquiment cGameEquiment = GetEquiment();

			ShopList[nIndex].GetEquiment(this,inventory, showPanel, cGameEquiment);

			EquimentList.Add(cGameEquiment);
		}

		FirstCheck ();

		StartCoroutine (Timer (nInitTime_Min, nInitTime_Sec));
	}

	public void SaveShopList()
	{
		GameManager.Instance.SaveShopList(EquimentList);
	}

	public void FirstCheck()
	{
		for (int nIndex = 0; nIndex < EquimentList.Count; nIndex++) 
		{
			if (ShopList [nIndex].bIsBuy == false) 
			{
				ShopList [nIndex].ClickButton ();
				break;
			}
		}
	}

	public void AllNoneDisable()
	{
		for (int nIndex = 0; nIndex < EquimentList.Count; nIndex++) 
		{
			if (ShopList [nIndex].bIsBuy == false) {
				ShopList [nIndex].WeaponPanelImage.sprite = ShopList [nIndex].NoneSelectSprite;
			}
		}
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

	public int GetGradeAmount(string strGrade)
	{
		switch (strGrade) 
		{
		case "C":
			return 1;
		case "B":
			return 2;
		case "A":
			return 3;
		case "S":
			return 3;
		}

		return 0;
	}

    //void OnDisable()
    //{
    //    if (EquimentList == null)
    //        return;

    //    GameManager.Instance.SaveEquiment(EquimentList);
    //}
}
