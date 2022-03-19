using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public enum E_BOOSTERTYPE
{
	E_BOOSTERTYPE_GOLD = 0,
	E_BOOSTERTYPE_HONOR,
	E_BOOSTERTYPE_STAFF,
	E_BOOSTERTYPE_ATTACK
}

public enum E_CASHSHOPTYPE
{
	E_CASHSHOPTYPE_CONSUME =0,
	E_CASHSHOPTYPE_HONOR,
	E_CASHSHOPTYPE_RUBY,
	E_CASHSHOPTYPE_PACKAGE

}

public class ShopCash : MonoBehaviour
{
	// 상품ID는 구글 개발자 콘솔에 등록한 상품ID와 동일하게 해주세요.
	public const string productId_ruby100 = "ruby.100";
	public const string productId_ruby400 = "ruby.400";
	public const string productId_ruby700 = "ruby.700";
	public const string productId_ruby1500 = "ruby_1500";
	public const string productId_packageForBeginner = "package.beginner";
	public const string productId_packageIce = "package.iceboss";
	public const string productId_packageSasin = "package.sasinboss";
	public const string productId_packageFire = "package.fireboss";
	public const string productId_packageMusic = "package.musicboss";

	public GameObject [] Cashpanels;				//tap시에 끄고 킬 obj
	public RectTransform [] addSlotObjs;			//아이템 슬롯 넣을때 넣는 obj
	public SimpleObjectPool cashItemSlotPool;
	public GameCashItemShop [] getCashItemInfo = null;

	//Buff 아이콘을 담고 있는 obj
	public GameObject BuffSlotPanel;

	private bool isSetUpConsume = false;
	private bool isSetUpRuby = false;
	private bool isSetUpPackage = false;
	private bool isSetUpHonor = false;

	private int nAddRuby = 0;
	private int nAddHonor = 0;


	public bool isConumeBuff_Gold = false;
	public bool isConumeBuff_Honor = false;
	public bool isConumeBuff_Staff = false;
	public bool isConumeBuff_Attack = false;

	private float fAddSlotWidth = 250.0f;

	public GameObject YesNoPopUp_Obj;
	public GameObject YesPopUp_Obj;

	//Buy Ruby Button
	public Button itemBuy_YesButton;
	public Text itemBuy_Text;

	public Text itemBuyNo_Text;

	//캐시버프 슬롯 풀
	public SimpleObjectPool CashBuffSlotPool;
	public RectTransform addCashBuff_transform;

	private const string sCashBuffGoldPath = "Store/CashItemImage/CashBuffIcon/buff_gold";
	private const string sCashBuffHonorPath = "Store/CashItemImage/CashBuffIcon/buff_honor";
	private const string sCashBuffStaffPath = "Store/CashItemImage/CashBuffIcon/buff_staff";
	private const string sCashBuffAttackPath = "Store/CashItemImage/CashBuffIcon/buff_attack";

	private string[] iconPaths = new string[4];

	public ShopCashBuffSlot[] shopCashBuffSlots = new ShopCashBuffSlot[4];

	public GameObject shopCash_Obj;


	public void InitShopIconData()
	{

		if (getCashItemInfo == null || getCashItemInfo.Length == 0 )
			getCashItemInfo = GameManager.Instance.cCashItemShopInfo;
		
		iconPaths [0] = sCashBuffGoldPath;
		iconPaths [1] = sCashBuffHonorPath;
		iconPaths [2] = sCashBuffStaffPath;
		iconPaths [3] = sCashBuffAttackPath;

	}

	//캐시 상점 실행에 호출
	public void StartSetUp()
	{
		Debug.Log ("StartChasShop");
		SetUpItemList ((int)E_CASHSHOPTYPE.E_CASHSHOPTYPE_CONSUME);
	}

	//_index를 제외한 모든 패널을 끔
	public void ShowPaenl(int _index)
	{
		for (int i = 0; i < Cashpanels.Length; i++) 
		{
			if (i == _index) 
			{
				SetUpItemList (_index);
				Cashpanels [i].SetActive (true);
			}
			else
				Cashpanels [i].SetActive (false);
		}
	}

	public void CallRubyCashShop()
	{
		shopCash_Obj.SetActive (true);
		gameObject.SetActive (true);
		StartSetUp ();
		ShowPaenl (2);
	}

	public void SetUpItemList(int _index)
	{
		if (addSlotObjs [_index].childCount != 0) 
		{
			if (_index == (int)E_CASHSHOPTYPE.E_CASHSHOPTYPE_PACKAGE)
			{
				Debug.Log ("패키지 블록 체크");
				if (GameManager.Instance.GetPlayer ().changeStats.bIsBeginnerPackageBuy == true) 
				{
					Debug.Log ("초보자 패키지 블록");
					ShopCashSlot shopCashSlot = addSlotObjs [_index].transform.GetChild (0).GetComponent<ShopCashSlot> ();
					shopCashSlot.BlockImage_Obj.SetActive (true);
				}
				if (GameManager.Instance.GetPlayer ().changeStats.bIsBossIcePackageBuy == true) 
				{
					Debug.Log ("보스 아이스 패키지 블록");
					ShopCashSlot shopCashSlot = addSlotObjs [_index].transform.GetChild (1).GetComponent<ShopCashSlot> ();
					shopCashSlot.BlockImage_Obj.SetActive (true);
				}

				if (GameManager.Instance.GetPlayer ().changeStats.bIsBossSasinPackageBuy == true) 
				{
					Debug.Log ("보스 사신 패키지 블록");
					ShopCashSlot shopCashSlot = addSlotObjs [_index].transform.GetChild (2).GetComponent<ShopCashSlot> ();
					shopCashSlot.BlockImage_Obj.SetActive (true);
				}

				if (GameManager.Instance.GetPlayer ().changeStats.bIsBossFirePackageBuy == true) 
				{
					Debug.Log ("보스 불 패키지 블록");
					ShopCashSlot shopCashSlot = addSlotObjs [_index].transform.GetChild (3).GetComponent<ShopCashSlot> ();
					shopCashSlot.BlockImage_Obj.SetActive (true);
				}

				if (GameManager.Instance.GetPlayer ().changeStats.bIsBossMusicPackageBuy == true) 
				{
					Debug.Log ("보스 음악 패키지 블록");
					ShopCashSlot shopCashSlot = addSlotObjs [_index].transform.GetChild (4).GetComponent<ShopCashSlot> ();
					shopCashSlot.BlockImage_Obj.SetActive (true);
				}
				return;
			}
			if (_index == (int)E_CASHSHOPTYPE.E_CASHSHOPTYPE_HONOR) 
			{

				Debug.Log ("명예 탭 체크");
				int nHonor = 0;
				for (int i = 0; i < 3; i++)
				{
					ShopCashSlot shopCashSlot = addSlotObjs [_index].transform.GetChild (i).GetComponent<ShopCashSlot> ();
					if(i==0)
						nHonor = Mathf.RoundToInt( 200 + (4.4f * (GameManager.Instance.GetPlayer ().GetDay () - 1)));
					else if(i==1)
						nHonor = Mathf.RoundToInt( 500 + (4.4f * (GameManager.Instance.GetPlayer ().GetDay () - 1)));
					else
						nHonor = Mathf.RoundToInt( 1000 + (4.4f * (GameManager.Instance.GetPlayer ().GetDay () - 1)));

					shopCashSlot.itemConstents_Text.text = string.Format("{0}",nHonor) + "명예 획득";
				}
				return;
			}

			if (_index == (int)E_CASHSHOPTYPE.E_CASHSHOPTYPE_RUBY) 
			{
				double freeGold = 0;
				Debug.Log ("재화 탭 체크");
				for(int i=4; i< 7; i++)
				{
					ShopCashSlot shopCashSlot = addSlotObjs [_index].transform.GetChild (i).GetComponent<ShopCashSlot> ();
					if(i==4)
						freeGold = ScoreManager.ScoreInstance.GetFreePassGold () * 20;
					else if(i==5)
						freeGold = ScoreManager.ScoreInstance.GetFreePassGold () * 50;
					else
						freeGold = ScoreManager.ScoreInstance.GetFreePassGold () * 100;

					string strGold = ScoreManager.ScoreInstance.ChangeMoney (freeGold);

					shopCashSlot.itemConstents_Text.text = strGold + " 획득";
				}
				return;
			}

			Debug.Log ("이미 상점 생성되어있음");
			return;
		}
		else 
		{
			for (int i = 0; i < getCashItemInfo.Length; i++) 
			{
				//아이템 추가
				if (getCashItemInfo [i].nType == _index)
				{
					//명예와 소모성 아이템은 루비로 나머지는 현금으로
					if (getCashItemInfo [i].nType == (int)E_CASHSHOPTYPE.E_CASHSHOPTYPE_CONSUME )
					{
						Debug.Log ("AddConsumeItem :" + getCashItemInfo [i].sItemName);
				
						GameObject consumeSlot = cashItemSlotPool.GetObject ();
						consumeSlot.transform.SetParent (addSlotObjs [_index].transform, false);
						consumeSlot.transform.localScale = Vector3.one;



						ShopCashSlot shopCashSlot = consumeSlot.GetComponent<ShopCashSlot> ();
						shopCashSlot.itemName_Text.text = getCashItemInfo [i].sItemName;
						shopCashSlot.itemConstents_Text.text = getCashItemInfo [i].sItemContents;
						shopCashSlot.item_Image.sprite = ObjectCashing.Instance.LoadSpriteFromCache (getCashItemInfo [i].sImagePath_01);
						shopCashSlot.itemTag_Image.sprite = ObjectCashing.Instance.LoadSpriteFromCache (getCashItemInfo [i].sImagePath_02);
						shopCashSlot.itemBuy_Image.sprite = ObjectCashing.Instance.LoadSpriteFromCache (getCashItemInfo [i].sImagePath_03);


						//값 표현
						shopCashSlot.itemBuyValue_Text.text = string.Format ("{0}", getCashItemInfo [i].fRuby);
						shopCashSlot.itemBuyValue_Text.alignment = TextAnchor.MiddleCenter;

						if (getCashItemInfo [i].sItemName == "골드부스터")
						{
							Debug.Log ("Add 골드부스터 Index : " + i);
							shopCashSlot.itemBuy_Button.onClick.AddListener (() => buyProductByRuby("골드부스터"));
						}

						if (getCashItemInfo [i].sItemName == "명예부스터") 
						{
							Debug.Log ("Add 명예부스터 Index : " + i);
							shopCashSlot.itemBuy_Button.onClick.AddListener (() => buyProductByRuby("명예부스터"));
						}

						if (getCashItemInfo [i].sItemName == "직원부스터") 
						{
							Debug.Log ("Add 직원부스터 Index : " + i);
							shopCashSlot.itemBuy_Button.onClick.AddListener (() => buyProductByRuby("직원부스터"));
						}

						if (getCashItemInfo [i].sItemName == "터치부스터") 
						{
							Debug.Log ("Add 직원부스터 Index : " + i);
							shopCashSlot.itemBuy_Button.onClick.AddListener (() => buyProductByRuby("터치부스터"));
						}

						if (getCashItemInfo [i].sItemName == "프리패스") 
						{
							Debug.Log ("Add 프리패스 Index : " + i);
							shopCashSlot.itemBuy_Button.onClick.AddListener (() => buyProductByRuby("프리패스"));
						}
						if (getCashItemInfo [i].sItemName == "프리패스5") 
						{
							Debug.Log ("Add 프리패스 Index : " + i);
							shopCashSlot.itemBuy_Button.onClick.AddListener (() => buyProductByRuby("프리패스5"));
						}
						if (getCashItemInfo [i].sItemName == "프리패스10") 
						{
							Debug.Log ("Add 프리패스 Index : " + i);
							shopCashSlot.itemBuy_Button.onClick.AddListener (() => buyProductByRuby("프리패스10"));
						}


					}

					if (getCashItemInfo [i].nType == (int)E_CASHSHOPTYPE.E_CASHSHOPTYPE_HONOR)
					{

						Debug.Log ("AddConsumeItem :" + getCashItemInfo [i].sItemName);

						GameObject consumeSlot = cashItemSlotPool.GetObject ();
						consumeSlot.transform.SetParent (addSlotObjs [_index].transform, false);
						consumeSlot.transform.localScale = Vector3.one;



						ShopCashSlot shopCashSlot = consumeSlot.GetComponent<ShopCashSlot> ();
						shopCashSlot.itemName_Text.text = getCashItemInfo [i].sItemName;
						shopCashSlot.itemConstents_Text.text = getCashItemInfo [i].sItemContents;
						shopCashSlot.item_Image.sprite = ObjectCashing.Instance.LoadSpriteFromCache (getCashItemInfo [i].sImagePath_01);
						shopCashSlot.itemTag_Image.sprite = ObjectCashing.Instance.LoadSpriteFromCache (getCashItemInfo [i].sImagePath_02);
						shopCashSlot.itemBuy_Image.sprite = ObjectCashing.Instance.LoadSpriteFromCache (getCashItemInfo [i].sImagePath_03);

						//값 표현
						shopCashSlot.itemBuyValue_Text.text = string.Format ("{0}", getCashItemInfo [i].fRuby);
						shopCashSlot.itemBuyValue_Text.alignment = TextAnchor.MiddleCenter;

						if (getCashItemInfo [i].sItemName == "200명예") 
						{
							Debug.Log ("200명예 Index : " + i);

							int nHonor = Mathf.RoundToInt( 200 + (4.4f * (GameManager.Instance.GetPlayer ().GetDay () - 1)));

							shopCashSlot.itemConstents_Text.text = string.Format("{0}",nHonor) + "명예 획득";

							shopCashSlot.itemBuy_Button.onClick.RemoveAllListeners ();
							shopCashSlot.itemBuy_Button.onClick.AddListener (() => buyProductByRuby("200명예"));
						}

						if (getCashItemInfo [i].sItemName == "500명예") 
						{
							Debug.Log ("200명예 Index : " + i);

							int nHonor = Mathf.RoundToInt( 500 + (4.4f * (GameManager.Instance.GetPlayer ().GetDay () - 1)));

							shopCashSlot.itemConstents_Text.text = string.Format("{0}",nHonor) + "명예 획득";

							shopCashSlot.itemBuy_Button.onClick.RemoveAllListeners ();
							shopCashSlot.itemBuy_Button.onClick.AddListener (() => buyProductByRuby("500명예"));
						}

						if (getCashItemInfo [i].sItemName == "1000명예") 
						{
							int nHonor = Mathf.RoundToInt( 1000 + (4.4f * (GameManager.Instance.GetPlayer ().GetDay () - 1)));

							shopCashSlot.itemConstents_Text.text = string.Format("{0}",nHonor) + "명예 획득";

							shopCashSlot.itemBuy_Button.onClick.RemoveAllListeners ();
							shopCashSlot.itemBuy_Button.onClick.AddListener (() => buyProductByRuby("1000명예"));
						}

					}
					
					if( getCashItemInfo [i].nType == (int)E_CASHSHOPTYPE.E_CASHSHOPTYPE_RUBY)
					{
						Debug.Log ("AddConsumeItem :" + getCashItemInfo [i].sItemName);
						//값 표현
						GameObject consumeSlot = cashItemSlotPool.GetObject ();
						consumeSlot.transform.SetParent (addSlotObjs [_index].transform, false);
						consumeSlot.transform.localScale = Vector3.one;

						ShopCashSlot shopCashSlot = consumeSlot.GetComponent<ShopCashSlot> ();
						shopCashSlot.itemName_Text.text = getCashItemInfo [i].sItemName;
						shopCashSlot.itemConstents_Text.text = getCashItemInfo [i].sItemContents;
						shopCashSlot.item_Image.sprite = ObjectCashing.Instance.LoadSpriteFromCache (getCashItemInfo [i].sImagePath_01);
						shopCashSlot.itemTag_Image.sprite = ObjectCashing.Instance.LoadSpriteFromCache (getCashItemInfo [i].sImagePath_02);
						shopCashSlot.itemBuy_Image.sprite = ObjectCashing.Instance.LoadSpriteFromCache (getCashItemInfo [i].sImagePath_03);

						if (getCashItemInfo [i].sItemName == "100보석")
						{
							shopCashSlot.itemBuyValue_Text.text = string.Format ("{0:#,###}", getCashItemInfo [i].fCash);
						}
						if (getCashItemInfo [i].sItemName == "400보석")
						{
							shopCashSlot.itemBuyValue_Text.text = string.Format ("{0:#,###}", getCashItemInfo [i].fCash);
						}

						if (getCashItemInfo [i].sItemName == "700보석")
						{
							shopCashSlot.itemBuyValue_Text.text = string.Format ("{0:#,###}", getCashItemInfo [i].fCash);
						}

						if (getCashItemInfo [i].sItemName == "1500보석")
						{
							shopCashSlot.itemBuyValue_Text.text = string.Format ("{0:#,###}", getCashItemInfo [i].fCash);
						}


						if (getCashItemInfo [i].sItemName == "골드20배") 
						{
							double freeGold = ScoreManager.ScoreInstance.GetFreePassGold () * 20;
							string strGold = ScoreManager.ScoreInstance.ChangeMoney (freeGold);

							shopCashSlot.itemConstents_Text.text = strGold + " 획득";

							shopCashSlot.itemBuyValue_Text.text = string.Format ("{0:#,###}", getCashItemInfo [i].fRuby);
							shopCashSlot.itemBuy_Button.onClick.RemoveAllListeners ();
							shopCashSlot.itemBuy_Button.onClick.AddListener (() => buyProductByRuby("골드20배"));
						}

						if (getCashItemInfo [i].sItemName == "골드50배") 
						{
							double freeGold = ScoreManager.ScoreInstance.GetFreePassGold () * 50;
							string strGold = ScoreManager.ScoreInstance.ChangeMoney (freeGold);

							shopCashSlot.itemConstents_Text.text = strGold + " 획득";

							shopCashSlot.itemBuyValue_Text.text = string.Format ("{0:#,###}", getCashItemInfo [i].fRuby);
							shopCashSlot.itemBuy_Button.onClick.RemoveAllListeners ();
							shopCashSlot.itemBuy_Button.onClick.AddListener (() => buyProductByRuby("골드50배"));
						}

						if (getCashItemInfo [i].sItemName == "골드100배") 
						{
							double freeGold = ScoreManager.ScoreInstance.GetFreePassGold () * 100;
							string strGold = ScoreManager.ScoreInstance.ChangeMoney (freeGold);

							shopCashSlot.itemConstents_Text.text = strGold + " 획득";

							shopCashSlot.itemBuyValue_Text.text = string.Format ("{0:#,###}", getCashItemInfo [i].fRuby);
							shopCashSlot.itemBuy_Button.onClick.RemoveAllListeners ();
							shopCashSlot.itemBuy_Button.onClick.AddListener (() => buyProductByRuby("골드100배"));
						}
					}

					if (getCashItemInfo [i].nType == (int)E_CASHSHOPTYPE.E_CASHSHOPTYPE_PACKAGE)
					{
						Debug.Log ("AddConsumeItem :" + getCashItemInfo [i].sItemName);
						//값 표현
						GameObject consumeSlot = cashItemSlotPool.GetObject ();
						consumeSlot.transform.SetParent (addSlotObjs [_index].transform, false);
						consumeSlot.transform.localScale = Vector3.one;

						ShopCashSlot shopCashSlot = consumeSlot.GetComponent<ShopCashSlot> ();
						shopCashSlot.itemName_Text.text = getCashItemInfo [i].sItemName;
						shopCashSlot.itemConstents_Text.text = getCashItemInfo [i].sItemContents;
						shopCashSlot.item_Image.sprite = ObjectCashing.Instance.LoadSpriteFromCache (getCashItemInfo [i].sImagePath_01);
						shopCashSlot.itemTag_Image.sprite = ObjectCashing.Instance.LoadSpriteFromCache (getCashItemInfo [i].sImagePath_02);
						shopCashSlot.itemBuy_Image.sprite = ObjectCashing.Instance.LoadSpriteFromCache (getCashItemInfo [i].sImagePath_03);

						if (getCashItemInfo [i].sItemName == "스타터패키지")
						{
							shopCashSlot.itemBuyValue_Text.text = string.Format ("{0:#,###}", getCashItemInfo [i].fCash);
							Debug.Log ("처음 패키지 블록 체크");
							if (GameManager.Instance.GetPlayer ().changeStats.bIsBeginnerPackageBuy == true) 
							{
								Debug.Log ("처음 초보자 패키지 블록");
								ShopCashSlot shopCashSlot_CheckBlock = addSlotObjs [(int)E_CASHSHOPTYPE.E_CASHSHOPTYPE_PACKAGE].transform.GetChild (0).GetComponent<ShopCashSlot> ();
								shopCashSlot_CheckBlock.BlockImage_Obj.SetActive (true);
							} 
							else 
							{
							}
						}

						if (getCashItemInfo [i].sItemName == "보스패키지1")
						{
							Debug.Log ("처음 패키지 블록 체크");
							shopCashSlot.itemBuyValue_Text.text = string.Format ("{0:#,###}", getCashItemInfo [i].fCash);
							if (GameManager.Instance.GetPlayer ().changeStats.bIsBossIcePackageBuy == true) 
							{
								Debug.Log ("처음 얼음 보스 블록");
								ShopCashSlot shopCashSlot_CheckBlock = addSlotObjs [(int)E_CASHSHOPTYPE.E_CASHSHOPTYPE_PACKAGE].transform.GetChild (1).GetComponent<ShopCashSlot> ();
								shopCashSlot_CheckBlock.BlockImage_Obj.SetActive (true);

							}
							else
							{
							}
						}

						if (getCashItemInfo [i].sItemName == "보스패키지2")
						{
							Debug.Log ("처음 패키지 블록 체크");
							shopCashSlot.itemBuyValue_Text.text = string.Format ("{0:#,###}", getCashItemInfo [i].fCash);
							if (GameManager.Instance.GetPlayer ().changeStats.bIsBossSasinPackageBuy == true) 
							{
								Debug.Log ("처음 사신 보스 패키지 블록");
								ShopCashSlot shopCashSlot_CheckBlock = addSlotObjs [(int)E_CASHSHOPTYPE.E_CASHSHOPTYPE_PACKAGE].transform.GetChild (2).GetComponent<ShopCashSlot> ();
								shopCashSlot_CheckBlock.BlockImage_Obj.SetActive (true);

							}
							else
							{
							}
						}

						if (getCashItemInfo [i].sItemName == "보스패키지3")
						{
							Debug.Log ("처음 패키지 블록 체크");
							shopCashSlot.itemBuyValue_Text.text = string.Format ("{0:#,###}", getCashItemInfo [i].fCash);
							if (GameManager.Instance.GetPlayer ().changeStats.bIsBossFirePackageBuy == true) {
								Debug.Log ("처음 불 보스 패키지 블록");
								ShopCashSlot shopCashSlot_CheckBlock = addSlotObjs [(int)E_CASHSHOPTYPE.E_CASHSHOPTYPE_PACKAGE].transform.GetChild (3).GetComponent<ShopCashSlot> ();
								shopCashSlot_CheckBlock.BlockImage_Obj.SetActive (true);

							}
						}

						if (getCashItemInfo [i].sItemName == "보스패키지4")
						{
							Debug.Log ("처음 패키지 블록 체크");
							shopCashSlot.itemBuyValue_Text.text = string.Format ("{0:#,###}", getCashItemInfo [i].fCash);
							if (GameManager.Instance.GetPlayer ().changeStats.bIsBossMusicPackageBuy == true) {
								Debug.Log ("처음 음악 보스 패키지 블록");
								ShopCashSlot shopCashSlot_CheckBlock = addSlotObjs [(int)E_CASHSHOPTYPE.E_CASHSHOPTYPE_PACKAGE].transform.GetChild (4).GetComponent<ShopCashSlot> ();
								shopCashSlot_CheckBlock.BlockImage_Obj.SetActive (true);

							}
						}
					}
				
					Vector2 addSize = new Vector2 (addSlotObjs [_index].sizeDelta.x + fAddSlotWidth, addSlotObjs [_index].sizeDelta.y);
					addSlotObjs [_index].sizeDelta = addSize;
				}
			}
		}
	}

	public void GameObjectActive(GameObject _obj)
	{
		if (_obj.activeSelf != true)
			_obj.SetActive (true);
		else
			_obj.SetActive (false);
	}

	public int GetIndex(string _string)
	{
		for (int i = 0; i < getCashItemInfo.Length; i++) 
		{
			if (getCashItemInfo [i].sItemName == _string)
				return i;
		}
		return 0;
	}

	public void buyProductByRuby(string _string)
	{
		int nIndex = GetIndex (_string); 

		//부족할때
		if (GameManager.Instance.player.GetRuby () < (int)getCashItemInfo [nIndex].fRuby) 
		{
			itemBuyNo_Text.text = "루비가 부족합니다.";
			GameObjectActive (YesPopUp_Obj);
			return;
		}

		if (getCashItemInfo [nIndex].sItemName == "골드부스터") 
		{
			itemBuy_Text.text = "골드부스터를 구입 하시겠습니까?";
			itemBuy_YesButton.onClick.RemoveAllListeners ();
			itemBuy_YesButton.onClick.AddListener (() => AddResource (nIndex, 0, getCashItemInfo [nIndex].fRuby ,  GameManager.Instance.player.GetDay(), 0));

			GameObjectActive (YesNoPopUp_Obj);

		}

		if (getCashItemInfo [nIndex].sItemName == "명예부스터") 
		{
			itemBuy_Text.text = "명예부스터를 구입 하시겠습니까?";
			itemBuy_YesButton.onClick.RemoveAllListeners ();
			itemBuy_YesButton.onClick.AddListener (() => AddResource (nIndex, 0, getCashItemInfo [nIndex].fRuby ,  GameManager.Instance.player.GetDay(), 0));

			GameObjectActive (YesNoPopUp_Obj);
		}

		if (getCashItemInfo [nIndex].sItemName == "직원부스터") 
		{
			itemBuy_Text.text = "직원부스터를 구입 하시겠습니까?";
			itemBuy_YesButton.onClick.RemoveAllListeners ();
			itemBuy_YesButton.onClick.AddListener (() => AddResource (nIndex, 0, getCashItemInfo [nIndex].fRuby ,  GameManager.Instance.player.GetDay(), 0));

			GameObjectActive (YesNoPopUp_Obj);
		}

		if (getCashItemInfo [nIndex].sItemName == "터치부스터") 
		{
			itemBuy_Text.text = "직원부스터를 구입 하시겠습니까?";
			itemBuy_YesButton.onClick.RemoveAllListeners ();
			itemBuy_YesButton.onClick.AddListener (() => AddResource (nIndex, 0, getCashItemInfo [nIndex].fRuby ,  GameManager.Instance.player.GetDay(), 0));

			GameObjectActive (YesNoPopUp_Obj);
		}



		if (getCashItemInfo [nIndex].sItemName == "프리패스") 
		{
			itemBuy_Text.text = "프리패스를 구입 하시겠습니까?";
			itemBuy_YesButton.onClick.RemoveAllListeners ();
			double freeGold = ScoreManager.ScoreInstance.GetFreePassGold ();
			itemBuy_YesButton.onClick.AddListener (() => AddResource (nIndex, 0, getCashItemInfo [nIndex].fRuby ,  GameManager.Instance.player.GetDay() + 1, freeGold));
			GameObjectActive (YesNoPopUp_Obj);
		}

		if (getCashItemInfo [nIndex].sItemName == "프리패스5") 
		{
			itemBuy_Text.text = "프리패스5를 구입 하시겠습니까?";
			itemBuy_YesButton.onClick.RemoveAllListeners ();
			double freeGold = ScoreManager.ScoreInstance.GetFreePassGold () * 5;
			itemBuy_YesButton.onClick.AddListener (() => AddResource (nIndex, 0, getCashItemInfo [nIndex].fRuby ,  GameManager.Instance.player.GetDay() + 5, freeGold));
			GameObjectActive (YesNoPopUp_Obj);
		}

		if (getCashItemInfo [nIndex].sItemName == "프리패스10") 
		{
			itemBuy_Text.text = "프리패스10를 구입 하시겠습니까?";
			itemBuy_YesButton.onClick.RemoveAllListeners ();
			double freeGold = ScoreManager.ScoreInstance.GetFreePassGold () * 10;
			itemBuy_YesButton.onClick.AddListener (() => AddResource (nIndex, 0, getCashItemInfo [nIndex].fRuby ,  GameManager.Instance.player.GetDay() + 10, freeGold));

			GameObjectActive (YesNoPopUp_Obj);
		}



		//{(기본값)+4.4*(현재일차-1)}

		if (getCashItemInfo [nIndex].sItemName == "200명예") 
		{
			int nHonor = Mathf.RoundToInt( 200 + (4.4f * (GameManager.Instance.GetPlayer ().GetDay () - 1)));

			itemBuy_Text.text = string.Format("{0}",nHonor) + "명예를 구입 하시겠습니까?";

			itemBuy_YesButton.onClick.RemoveAllListeners ();


			itemBuy_YesButton.onClick.AddListener (() => AddResource (nIndex, nHonor, getCashItemInfo [nIndex].fRuby , GameManager.Instance.player.GetDay() , 0f));
			GameObjectActive (YesNoPopUp_Obj);

		}

		if (getCashItemInfo [nIndex].sItemName == "500명예") 
		{
			int nHonor = Mathf.RoundToInt( 500 + (4.4f * (GameManager.Instance.GetPlayer ().GetDay () - 1)));

			itemBuy_Text.text = string.Format("{0}",nHonor) + "명예를 구입 하시겠습니까?";

			itemBuy_YesButton.onClick.RemoveAllListeners ();
			itemBuy_YesButton.onClick.AddListener (() => AddResource (nIndex, nHonor, getCashItemInfo [nIndex].fRuby, GameManager.Instance.player.GetDay()  , 0f));
			GameObjectActive (YesNoPopUp_Obj);
		}

		if (getCashItemInfo [nIndex].sItemName == "1000명예") 
		{
			int nHonor = Mathf.RoundToInt( 1000 + (4.4f * (GameManager.Instance.GetPlayer ().GetDay () - 1)));

			itemBuy_Text.text = string.Format("{0}",nHonor) + "명예를 구입 하시겠습니까?";

			itemBuy_YesButton.onClick.RemoveAllListeners ();
			itemBuy_YesButton.onClick.AddListener (() => AddResource (nIndex, nHonor, getCashItemInfo [nIndex].fRuby, GameManager.Instance.player.GetDay(), 0f ));
			GameObjectActive (YesNoPopUp_Obj);
		}

		if (getCashItemInfo [nIndex].sItemName == "골드20배") 
		{
			double freeGold = ScoreManager.ScoreInstance.GetFreePassGold () * 20;
			string strGold = ScoreManager.ScoreInstance.ChangeMoney (freeGold);
			itemBuy_Text.text = "골드" +  strGold + "를 구입 하시겠습니까?";

			itemBuy_YesButton.onClick.RemoveAllListeners ();
			itemBuy_YesButton.onClick.AddListener (() => AddResource (nIndex, 0, getCashItemInfo [nIndex].fRuby , GameManager.Instance.player.GetDay() , freeGold));
			GameObjectActive (YesNoPopUp_Obj);

		}

		if (getCashItemInfo [nIndex].sItemName == "골드50배") 
		{
			double freeGold = ScoreManager.ScoreInstance.GetFreePassGold () * 50;
			string strGold = ScoreManager.ScoreInstance.ChangeMoney (freeGold);
			itemBuy_Text.text = "골드" +  strGold + "를 구입 하시겠습니까?";

			itemBuy_YesButton.onClick.RemoveAllListeners ();
			itemBuy_YesButton.onClick.AddListener (() => AddResource (nIndex, 0, getCashItemInfo [nIndex].fRuby, GameManager.Instance.player.GetDay()  , freeGold));
			GameObjectActive (YesNoPopUp_Obj);
		}

		if (getCashItemInfo [nIndex].sItemName == "골드100배") 
		{
			
			double freeGold = ScoreManager.ScoreInstance.GetFreePassGold () * 100;
			string strGold = ScoreManager.ScoreInstance.ChangeMoney (freeGold);
			itemBuy_Text.text = "골드" +  strGold + "를 구입 하시겠습니까?";

			itemBuy_YesButton.onClick.RemoveAllListeners ();
			itemBuy_YesButton.onClick.AddListener (() => AddResource (nIndex, 0, getCashItemInfo [nIndex].fRuby, GameManager.Instance.player.GetDay(), freeGold ));
			GameObjectActive (YesNoPopUp_Obj);
		}

	}

	public void AddResource(int _index, int _honor , float _ruby ,int _day , double _gold)
	{
		if (getCashItemInfo[_index].sItemName == "프리패스" || getCashItemInfo[_index].sItemName == "프리패스5" || getCashItemInfo[_index].sItemName == "프리패스10")
		{
			SpawnManager.Instance.SetDayInitInfo (_day);
			//SpawnManager.Instance.questManager.QuestSuccessCheck (QuestType.E_QUESTTYPE_DAYS, _day);
		}

		ScoreManager.ScoreInstance.GoldPlus (_gold);
		ScoreManager.ScoreInstance.HonorPlus (_honor);
		ScoreManager.ScoreInstance.RubyPlus( ((int)-_ruby));

		//부스터가 이미 있다면 시간만 추가
		if (getCashItemInfo [_index].sItemName == "골드부스터")
		{
			if (shopCashBuffSlots [_index] != null) {
				shopCashBuffSlots [_index].AddTimer (30, 00f);
				shopCashBuffSlots[_index].shopCash.isConumeBuff_Gold = true;
				return;
			}
			AddCashBuffSlot (_index);
			return;
		} 


		if (getCashItemInfo [_index].sItemName == "명예부스터") {
			if (shopCashBuffSlots [_index] != null) {
				shopCashBuffSlots [_index].AddTimer (30, 00f);
				shopCashBuffSlots[_index].shopCash.isConumeBuff_Honor = true;
				return;
			}
			AddCashBuffSlot (_index);
			return;
		}
	
		if (getCashItemInfo [_index].sItemName == "직원부스터") {
			if (shopCashBuffSlots [_index] != null) {
				shopCashBuffSlots [_index].AddTimer (30, 00f);
				shopCashBuffSlots[_index].shopCash.isConumeBuff_Staff = true;
				return;
			}
			AddCashBuffSlot (_index);
			return;
		}
	
		if (getCashItemInfo [_index].sItemName == "터치부스터") {
			if (shopCashBuffSlots [_index] != null) {
				shopCashBuffSlots [_index].AddTimer (30, 00f);
				shopCashBuffSlots[_index].shopCash.isConumeBuff_Attack = true;
				return;
			}
			AddCashBuffSlot (_index);
			return;
		}
	}

	public void AddCashBuffSlot(int _index)
	{

		GameObject consumeSlot = CashBuffSlotPool.GetObject ();
		consumeSlot.transform.SetParent (addCashBuff_transform.transform, false);
		consumeSlot.transform.localScale = Vector3.one;

		ShopCashBuffSlot shopCashBuffSlot = consumeSlot.GetComponent<ShopCashBuffSlot> ();
		shopCashBuffSlot.icon_Image.sprite =  ObjectCashing.Instance.LoadSpriteFromCache (iconPaths[_index]);
		shopCashBuffSlot.StartTimer ();
		shopCashBuffSlot.shopCash = this;

		if (_index == (int)E_BOOSTERTYPE.E_BOOSTERTYPE_GOLD)
			shopCashBuffSlot.shopCash.isConumeBuff_Gold = true;
		if (_index == (int)E_BOOSTERTYPE.E_BOOSTERTYPE_ATTACK)
			shopCashBuffSlot.shopCash.isConumeBuff_Attack = true;
		if (_index == (int)E_BOOSTERTYPE.E_BOOSTERTYPE_HONOR)
			shopCashBuffSlot.shopCash.isConumeBuff_Honor = true;
		if (_index == (int)E_BOOSTERTYPE.E_BOOSTERTYPE_STAFF)
			shopCashBuffSlot.shopCash.isConumeBuff_Staff = true;

		shopCashBuffSlot.sSlotName = getCashItemInfo [_index].sItemName;
		shopCashBuffSlot.pool_Obj = CashBuffSlotPool;
		shopCashBuffSlots [_index] = shopCashBuffSlot;
	}

	public void LoadBooster(E_BOOSTERTYPE _index, int _CurMin, float _CurSec)
	{
		if (_index == E_BOOSTERTYPE.E_BOOSTERTYPE_GOLD)
		{
			if (CheckBuffIcon ((int)_index) == false) 
			{

				GameObject consumeSlot = CashBuffSlotPool.GetObject ();
				consumeSlot.transform.SetParent (addCashBuff_transform.transform, false);
				consumeSlot.transform.localScale = Vector3.one;

				ShopCashBuffSlot shopCashBuffSlot = consumeSlot.GetComponent<ShopCashBuffSlot> ();
				shopCashBuffSlot.icon_Image.sprite = ObjectCashing.Instance.LoadSpriteFromCache (iconPaths [(int)_index]);
				shopCashBuffSlot.shopCash = this;

				shopCashBuffSlots [(int)_index] = shopCashBuffSlot;

				shopCashBuffSlot.sSlotName = getCashItemInfo [(int)_index].sItemName;
				shopCashBuffSlot.LoadTimer (_CurMin, (int)_CurSec);
				shopCashBuffSlot.shopCash.isConumeBuff_Gold = true;

				shopCashBuffSlot.pool_Obj = CashBuffSlotPool;
				shopCashBuffSlots [(int)_index] = shopCashBuffSlot;
			} 
			else 
			{
				shopCashBuffSlots [(int)_index].AddTimer (_CurMin, _CurSec);
				shopCashBuffSlots[(int)_index].shopCash.isConumeBuff_Gold = true;
			}
				
		}
		if (_index == E_BOOSTERTYPE.E_BOOSTERTYPE_HONOR)
		{
			if (CheckBuffIcon ((int)_index) == false) 
			{

				GameObject consumeSlot = CashBuffSlotPool.GetObject ();
				consumeSlot.transform.SetParent (addCashBuff_transform.transform, false);
				consumeSlot.transform.localScale = Vector3.one;

				ShopCashBuffSlot shopCashBuffSlot = consumeSlot.GetComponent<ShopCashBuffSlot> ();
				shopCashBuffSlot.icon_Image.sprite = ObjectCashing.Instance.LoadSpriteFromCache (iconPaths [(int)_index]);
				shopCashBuffSlot.shopCash = this;

				shopCashBuffSlots [(int)_index] = shopCashBuffSlot;

				shopCashBuffSlot.sSlotName = getCashItemInfo [(int)_index].sItemName;
				shopCashBuffSlot.LoadTimer (_CurMin, (int)_CurSec);
				shopCashBuffSlots[(int)_index].shopCash.isConumeBuff_Honor = true;

				shopCashBuffSlot.pool_Obj = CashBuffSlotPool;
				shopCashBuffSlots [(int)_index] = shopCashBuffSlot;
			} 
			else 
			{
				shopCashBuffSlots [(int)_index].AddTimer (_CurMin, _CurSec);
				shopCashBuffSlots[(int)_index].shopCash.isConumeBuff_Honor = true;
			}

		}
		if (_index == E_BOOSTERTYPE.E_BOOSTERTYPE_STAFF) 
		{
			if (CheckBuffIcon ((int)_index) == false) 
			{

				GameObject consumeSlot = CashBuffSlotPool.GetObject ();
				consumeSlot.transform.SetParent (addCashBuff_transform.transform, false);
				consumeSlot.transform.localScale = Vector3.one;

				ShopCashBuffSlot shopCashBuffSlot = consumeSlot.GetComponent<ShopCashBuffSlot> ();
				shopCashBuffSlot.icon_Image.sprite = ObjectCashing.Instance.LoadSpriteFromCache (iconPaths [(int)_index]);
				shopCashBuffSlot.shopCash = this;
			
				shopCashBuffSlots [(int)_index] = shopCashBuffSlot;

				shopCashBuffSlot.sSlotName = getCashItemInfo [(int)_index].sItemName;
				shopCashBuffSlot.LoadTimer (_CurMin, (int)_CurSec);
				shopCashBuffSlots[(int)_index].shopCash.isConumeBuff_Staff = true;

				shopCashBuffSlot.pool_Obj = CashBuffSlotPool;
				shopCashBuffSlots [(int)_index] = shopCashBuffSlot;
			} 
			else 
			{
				shopCashBuffSlots [(int)_index].AddTimer (_CurMin, _CurSec);

				shopCashBuffSlots[(int)_index].shopCash.isConumeBuff_Staff = true;
			}

		}
		if (_index == E_BOOSTERTYPE.E_BOOSTERTYPE_ATTACK) 
		{
			if (CheckBuffIcon ((int)_index) == false) 
			{

				GameObject consumeSlot = CashBuffSlotPool.GetObject ();
				consumeSlot.transform.SetParent (addCashBuff_transform.transform, false);
				consumeSlot.transform.localScale = Vector3.one;

				ShopCashBuffSlot shopCashBuffSlot = consumeSlot.GetComponent<ShopCashBuffSlot> ();
				shopCashBuffSlot.icon_Image.sprite = ObjectCashing.Instance.LoadSpriteFromCache (iconPaths [(int)_index]);
				shopCashBuffSlot.shopCash = this;

				shopCashBuffSlots [(int)_index] = shopCashBuffSlot;
			
				shopCashBuffSlot.sSlotName = getCashItemInfo [(int)_index].sItemName;
				shopCashBuffSlot.LoadTimer (_CurMin, (int)_CurSec);
				shopCashBuffSlots[(int)_index].shopCash.isConumeBuff_Attack = true;


				shopCashBuffSlot.pool_Obj = CashBuffSlotPool;
				shopCashBuffSlots [(int)_index] = shopCashBuffSlot;
			} 
			else 
			{
				shopCashBuffSlots [(int)_index].AddTimer (_CurMin, _CurSec);
				shopCashBuffSlots[(int)_index].shopCash.isConumeBuff_Attack = true;
			}
		}
	}

	public void SaveCashActiveBoosterTime()
	{
		for (int i = 0; i < shopCashBuffSlots.Length; i++) 
		{
			if (shopCashBuffSlots [i] == null)
				continue;
			else 
			{
				shopCashBuffSlots [i].SaveTimer ((E_BOOSTERTYPE)i);
			}
				
		}
	}

	public bool  CheckBuffIcon(int _index)
	{
		for (int i = 0; i < BuffSlotPanel.transform.childCount; i++) 
		{
			ShopCashBuffSlot shopCashBuffSlot = BuffSlotPanel.transform.GetChild (i).GetComponent<ShopCashBuffSlot> ();

			if (shopCashBuffSlot.sSlotName == getCashItemInfo [(int)_index].sItemName)
				return  true;
			
		}
		return false;
	}
}
