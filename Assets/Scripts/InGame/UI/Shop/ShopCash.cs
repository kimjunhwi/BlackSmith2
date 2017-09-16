﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
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

public class ShopCash : MonoBehaviour , IStoreListener
{
	private static IStoreController storeController;
	private static IExtensionProvider extensionProvider;

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

	public GameObject [] Cashpanels;		//tap시에 끄고 킬 obj
	public RectTransform [] addSlotObjs;		//아이템 슬롯 넣을때 넣는 obj
	public SimpleObjectPool cashItemSlotPool;
	public GameCashItemShop [] getCashItemInfo = null;


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


		InitializePurchasing();

		
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

	public void SetUpItemList(int _index)
	{
		

		
		if (addSlotObjs [_index].childCount != 0) 
		{
			if (_index == (int)E_CASHSHOPTYPE.E_CASHSHOPTYPE_PACKAGE)
			{
				Debug.Log ("패키지 블록 체크");
				if (addSlotObjs [_index].transform.GetChild (0) && GameManager.Instance.GetPlayer ().changeStats.bIsBeginnerPackageBuy == true) 
				{
					Debug.Log ("초보자 패키지 블록");
					ShopCashSlot shopCashSlot = addSlotObjs [_index].transform.GetChild (0).GetComponent<ShopCashSlot> ();
					shopCashSlot.BlockImage_Obj.SetActive (true);
				}
				if (addSlotObjs [_index].transform.GetChild (1) && GameManager.Instance.GetPlayer ().changeStats.bIsBossIcePackageBuy == true) 
				{
					Debug.Log ("보스 아이스 패키지 블록");
					ShopCashSlot shopCashSlot = addSlotObjs [_index].transform.GetChild (0).GetComponent<ShopCashSlot> ();
					shopCashSlot.BlockImage_Obj.SetActive (true);
				}

				if (addSlotObjs [_index].transform.GetChild (2) && GameManager.Instance.GetPlayer ().changeStats.bIsBossSasinPackageBuy == true) 
				{
					Debug.Log ("보스 사신 패키지 블록");
					ShopCashSlot shopCashSlot = addSlotObjs [_index].transform.GetChild (0).GetComponent<ShopCashSlot> ();
					shopCashSlot.BlockImage_Obj.SetActive (true);
				}

				if (addSlotObjs [_index].transform.GetChild (3) && GameManager.Instance.GetPlayer ().changeStats.bIsBossFirePackageBuy == true) 
				{
					Debug.Log ("보스 불 패키지 블록");
					ShopCashSlot shopCashSlot = addSlotObjs [_index].transform.GetChild (0).GetComponent<ShopCashSlot> ();
					shopCashSlot.BlockImage_Obj.SetActive (true);
				}

				if (addSlotObjs [_index].transform.GetChild (4) && GameManager.Instance.GetPlayer ().changeStats.bIsBossMusicPackageBuy == true) 
				{
					Debug.Log ("보스 음악 패키지 블록");
					ShopCashSlot shopCashSlot = addSlotObjs [_index].transform.GetChild (0).GetComponent<ShopCashSlot> ();
					shopCashSlot.BlockImage_Obj.SetActive (true);
				}
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
					Debug.Log ("AddConsumeItem");
					//isSetUpConsume = true;
					GameObject consumeSlot = cashItemSlotPool.GetObject ();
					consumeSlot.transform.SetParent (addSlotObjs [_index].transform, false);
					consumeSlot.transform.localScale = Vector3.one;



					ShopCashSlot shopCashSlot = consumeSlot.GetComponent<ShopCashSlot> ();
					shopCashSlot.itemName_Text.text = getCashItemInfo [i].sItemName;
					shopCashSlot.itemConstents_Text.text = getCashItemInfo [i].sItemContents;
					shopCashSlot.item_Image.sprite = ObjectCashing.Instance.LoadSpriteFromCache (getCashItemInfo [i].sImagePath_01);
					shopCashSlot.itemTag_Image.sprite = ObjectCashing.Instance.LoadSpriteFromCache (getCashItemInfo [i].sImagePath_02);
					shopCashSlot.itemBuy_Image.sprite = ObjectCashing.Instance.LoadSpriteFromCache (getCashItemInfo [i].sImagePath_03);


					//명예와 소모성 아이템은 루비로 나머지는 현금으로
					if (getCashItemInfo [i].nType == (int)E_CASHSHOPTYPE.E_CASHSHOPTYPE_CONSUME || getCashItemInfo [i].nType == (int)E_CASHSHOPTYPE.E_CASHSHOPTYPE_HONOR )
					{
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
							//Debug.Log ("Add 프리패스 Index : " + i);
							shopCashSlot.itemBuy_Button.onClick.AddListener (() => buyProductByRuby("프리패스"));
						}
						if (getCashItemInfo [i].sItemName == "프리패스5") 
						{
							//Debug.Log ("Add 프리패스 Index : " + i);
							shopCashSlot.itemBuy_Button.onClick.AddListener (() => buyProductByRuby("프리패스5"));
						}
						if (getCashItemInfo [i].sItemName == "프리패스10") 
						{
							//Debug.Log ("Add 프리패스 Index : " + i);
							shopCashSlot.itemBuy_Button.onClick.AddListener (() => buyProductByRuby("프리패스10"));
						}


						if (getCashItemInfo [i].sItemName == "100명예") 
						{
							Debug.Log ("Add 100명예 Index : " + i);
							shopCashSlot.itemBuy_Button.onClick.AddListener (() => buyProductByRuby("100명예"));
						}

						if (getCashItemInfo [i].sItemName == "300명예") 
						{
							Debug.Log ("Add 300명예 Index : " + i);
							shopCashSlot.itemBuy_Button.onClick.AddListener (() => buyProductByRuby("300명예"));
						}

						if (getCashItemInfo [i].sItemName == "500명예") 
						{
							Debug.Log ("Add 500명예 Index : " + i);
							shopCashSlot.itemBuy_Button.onClick.AddListener (() => buyProductByRuby("500명예"));
						}




					}
					
					if( getCashItemInfo [i].nType == (int)E_CASHSHOPTYPE.E_CASHSHOPTYPE_RUBY ||  getCashItemInfo [i].nType == (int)E_CASHSHOPTYPE.E_CASHSHOPTYPE_PACKAGE )
					{
						//값 표현


						if (getCashItemInfo [i].sItemName == "100보석")
						{
							shopCashSlot.itemBuyValue_Text.text = string.Format ("{0:#,###}", getCashItemInfo [i].fCash);
							shopCashSlot.itemBuy_Button.onClick.RemoveListener (() => BuyProductID (productId_ruby100));
							shopCashSlot.itemBuy_Button.onClick.AddListener (() => BuyProductID (productId_ruby100));
						}
						if (getCashItemInfo [i].sItemName == "400보석")
						{
							shopCashSlot.itemBuyValue_Text.text = string.Format ("{0:#,###}", getCashItemInfo [i].fCash);
							shopCashSlot.itemBuy_Button.onClick.RemoveListener (() => BuyProductID (productId_ruby400));
							shopCashSlot.itemBuy_Button.onClick.AddListener (() => BuyProductID (productId_ruby400));
						}

						if (getCashItemInfo [i].sItemName == "700보석")
						{
							shopCashSlot.itemBuyValue_Text.text = string.Format ("{0:#,###}", getCashItemInfo [i].fCash);
							shopCashSlot.itemBuy_Button.onClick.RemoveListener (() => BuyProductID (productId_ruby700));
							shopCashSlot.itemBuy_Button.onClick.AddListener (() => BuyProductID (productId_ruby700));
						}

						if (getCashItemInfo [i].sItemName == "1500보석")
						{
							shopCashSlot.itemBuyValue_Text.text = string.Format ("{0:#,###}", getCashItemInfo [i].fCash);
							shopCashSlot.itemBuy_Button.onClick.RemoveListener (() => BuyProductID (productId_ruby1500));
							shopCashSlot.itemBuy_Button.onClick.AddListener (() => BuyProductID (productId_ruby1500));
						}


						if (getCashItemInfo [i].sItemName == "골드20배") 
						{
							shopCashSlot.itemBuyValue_Text.text = string.Format ("{0:#,###}", getCashItemInfo [i].fRuby);
							shopCashSlot.itemBuy_Button.onClick.AddListener (() => buyProductByRuby("골드20배"));
						}

						if (getCashItemInfo [i].sItemName == "골드50배") 
						{
							shopCashSlot.itemBuyValue_Text.text = string.Format ("{0:#,###}", getCashItemInfo [i].fRuby);
							shopCashSlot.itemBuy_Button.onClick.AddListener (() => buyProductByRuby("골드50배"));
						}

						if (getCashItemInfo [i].sItemName == "골드100배") 
						{
							shopCashSlot.itemBuyValue_Text.text = string.Format ("{0:#,###}", getCashItemInfo [i].fRuby);
							shopCashSlot.itemBuy_Button.onClick.AddListener (() => buyProductByRuby("골드100배"));
						}

						if (getCashItemInfo [i].sItemName == "스타터패키지")
						{
							shopCashSlot.itemBuyValue_Text.text = string.Format ("{0:#,###}", getCashItemInfo [i].fCash);
							Debug.Log ("패키지 블록 체크");
							if (GameManager.Instance.GetPlayer ().changeStats.bIsBeginnerPackageBuy == true) {
								Debug.Log ("초보자 패키지 블록");
								ShopCashSlot shopCashSlot_CheckBlock = addSlotObjs [(int)E_CASHSHOPTYPE.E_CASHSHOPTYPE_PACKAGE].transform.GetChild (0).GetComponent<ShopCashSlot> ();
								shopCashSlot_CheckBlock.BlockImage_Obj.SetActive (true);

							}

							shopCashSlot.itemBuy_Button.onClick.RemoveListener (() => BuyProductID (productId_packageForBeginner));
							shopCashSlot.itemBuy_Button.onClick.AddListener (() => BuyProductID (productId_packageForBeginner));
						}

						if (getCashItemInfo [i].sItemName == "보스패키지1")
						{
							shopCashSlot.itemBuyValue_Text.text = string.Format ("{0:#,###}", getCashItemInfo [i].fCash);
							if (GameManager.Instance.GetPlayer ().changeStats.bIsBeginnerPackageBuy == true) {
								Debug.Log ("얼음 보스 블록");
								ShopCashSlot shopCashSlot_CheckBlock = addSlotObjs [(int)E_CASHSHOPTYPE.E_CASHSHOPTYPE_PACKAGE].transform.GetChild (1).GetComponent<ShopCashSlot> ();
								shopCashSlot_CheckBlock.BlockImage_Obj.SetActive (true);
							
							}
							shopCashSlot.itemBuy_Button.onClick.RemoveListener (() => BuyProductID (productId_packageIce));
							shopCashSlot.itemBuy_Button.onClick.AddListener (() => BuyProductID (productId_packageIce));
						}

						if (getCashItemInfo [i].sItemName == "보스패키지2")
						{
							shopCashSlot.itemBuyValue_Text.text = string.Format ("{0:#,###}", getCashItemInfo [i].fCash);
							if (GameManager.Instance.GetPlayer ().changeStats.bIsBossIcePackageBuy == true) {
								Debug.Log ("사신 보스 패키지 블록");
								ShopCashSlot shopCashSlot_CheckBlock = addSlotObjs [(int)E_CASHSHOPTYPE.E_CASHSHOPTYPE_PACKAGE].transform.GetChild (2).GetComponent<ShopCashSlot> ();
								shopCashSlot_CheckBlock.BlockImage_Obj.SetActive (true);

							}
							shopCashSlot.itemBuy_Button.onClick.RemoveListener (() => BuyProductID (productId_packageSasin));
							shopCashSlot.itemBuy_Button.onClick.AddListener (() => BuyProductID (productId_packageSasin));
						}

						if (getCashItemInfo [i].sItemName == "보스패키지3")
						{
							shopCashSlot.itemBuyValue_Text.text = string.Format ("{0:#,###}", getCashItemInfo [i].fCash);
							if (GameManager.Instance.GetPlayer ().changeStats.bIsBossSasinPackageBuy == true) {
								Debug.Log ("불 보스 패키지 블록");
								ShopCashSlot shopCashSlot_CheckBlock = addSlotObjs [(int)E_CASHSHOPTYPE.E_CASHSHOPTYPE_PACKAGE].transform.GetChild (3).GetComponent<ShopCashSlot> ();
								shopCashSlot_CheckBlock.BlockImage_Obj.SetActive (true);
	
							}
							shopCashSlot.itemBuy_Button.onClick.RemoveListener (() => BuyProductID (productId_packageFire));
							shopCashSlot.itemBuy_Button.onClick.AddListener (() => BuyProductID (productId_packageFire));
						}

						if (getCashItemInfo [i].sItemName == "보스패키지4")
						{
							shopCashSlot.itemBuyValue_Text.text = string.Format ("{0:#,###}", getCashItemInfo [i].fCash);
							if (GameManager.Instance.GetPlayer ().changeStats.bIsBeginnerPackageBuy == true) {
								Debug.Log ("음악 보스 패키지 블록");
								ShopCashSlot shopCashSlot_CheckBlock = addSlotObjs [(int)E_CASHSHOPTYPE.E_CASHSHOPTYPE_PACKAGE].transform.GetChild (4).GetComponent<ShopCashSlot> ();
								shopCashSlot_CheckBlock.BlockImage_Obj.SetActive (true);

							}
							shopCashSlot.itemBuy_Button.onClick.RemoveListener (() => BuyProductID (productId_packageMusic));
							shopCashSlot.itemBuy_Button.onClick.AddListener (() => BuyProductID (productId_packageMusic));
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
			itemBuy_YesButton.onClick.AddListener (() => AddResource (nIndex, 0, getCashItemInfo [nIndex].fRuby ,  GameManager.Instance.player.GetDay() + 1, 10000.0));
			GameObjectActive (YesNoPopUp_Obj);
		}

		if (getCashItemInfo [nIndex].sItemName == "프리패스5") 
		{
			itemBuy_Text.text = "프리패스5를 구입 하시겠습니까?";
			itemBuy_YesButton.onClick.RemoveAllListeners ();
			itemBuy_YesButton.onClick.AddListener (() => AddResource (nIndex, 0, getCashItemInfo [nIndex].fRuby ,  GameManager.Instance.player.GetDay() + 5, 10000.0));
			GameObjectActive (YesNoPopUp_Obj);
		}

		if (getCashItemInfo [nIndex].sItemName == "프리패스10") 
		{
			itemBuy_Text.text = "프리패스10를 구입 하시겠습니까?";
			itemBuy_YesButton.onClick.RemoveAllListeners ();
			itemBuy_YesButton.onClick.AddListener (() => AddResource (nIndex, 0, getCashItemInfo [nIndex].fRuby ,  GameManager.Instance.player.GetDay() + 10, 10000.0));
			GameObjectActive (YesNoPopUp_Obj);
		}





		if (getCashItemInfo [nIndex].sItemName == "100명예") 
		{
			itemBuy_Text.text = "100명예를 구입 하시겠습니까?";
			itemBuy_YesButton.onClick.RemoveAllListeners ();
			itemBuy_YesButton.onClick.AddListener (() => AddResource (nIndex, 100, getCashItemInfo [nIndex].fRuby , GameManager.Instance.player.GetDay() , 0f));
			GameObjectActive (YesNoPopUp_Obj);

		}

		if (getCashItemInfo [nIndex].sItemName == "300명예") 
		{
			itemBuy_Text.text = "300명예를 구입 하시겠습니까?";
			itemBuy_YesButton.onClick.RemoveAllListeners ();
			itemBuy_YesButton.onClick.AddListener (() => AddResource (nIndex, 300, getCashItemInfo [nIndex].fRuby, GameManager.Instance.player.GetDay()  , 0f));
			GameObjectActive (YesNoPopUp_Obj);
		}

		if (getCashItemInfo [nIndex].sItemName == "500명예") 
		{
			itemBuy_Text.text = "500명예를 구입 하시겠습니까?";
			itemBuy_YesButton.onClick.RemoveAllListeners ();
			itemBuy_YesButton.onClick.AddListener (() => AddResource (nIndex, 500, getCashItemInfo [nIndex].fRuby, GameManager.Instance.player.GetDay(), 0f ));
			GameObjectActive (YesNoPopUp_Obj);
		}

		if (getCashItemInfo [nIndex].sItemName == "골드20배") 
		{
			itemBuy_Text.text = "골드20배를 구입 하시겠습니까?";
			itemBuy_YesButton.onClick.RemoveAllListeners ();
			itemBuy_YesButton.onClick.AddListener (() => AddResource (nIndex, 0, getCashItemInfo [nIndex].fRuby , GameManager.Instance.player.GetDay() , 200000f));
			GameObjectActive (YesNoPopUp_Obj);

		}

		if (getCashItemInfo [nIndex].sItemName == "골드50배") 
		{
			itemBuy_Text.text = "골드50배를 구입 하시겠습니까?";
			itemBuy_YesButton.onClick.RemoveAllListeners ();
			itemBuy_YesButton.onClick.AddListener (() => AddResource (nIndex, 0, getCashItemInfo [nIndex].fRuby, GameManager.Instance.player.GetDay()  , 500000f));
			GameObjectActive (YesNoPopUp_Obj);
		}

		if (getCashItemInfo [nIndex].sItemName == "골드100배") 
		{
			itemBuy_Text.text = "골드100배를 구입 하시겠습니까?";
			itemBuy_YesButton.onClick.RemoveAllListeners ();
			itemBuy_YesButton.onClick.AddListener (() => AddResource (nIndex, 0, getCashItemInfo [nIndex].fRuby, GameManager.Instance.player.GetDay(), 1000000f ));
			GameObjectActive (YesNoPopUp_Obj);
		}

	}

	public void AddResource(int _index, int _honor , float _ruby ,int _day , double _gold)
	{
		if (getCashItemInfo[_index].sItemName == "프리패스" || getCashItemInfo[_index].sItemName == "프리패스5" || getCashItemInfo[_index].sItemName == "프리패스10")
		{
			SpawnManager.Instance.SetDayInitInfo (_day);
		}


		if (getCashItemInfo [_index].sItemName == "골드부스터" || getCashItemInfo [_index].sItemName == "명예부스터" || getCashItemInfo [_index].sItemName == "직원부스터"
			|| getCashItemInfo [_index].sItemName == "터치부스터") 
		{
			GameObject consumeSlot = CashBuffSlotPool.GetObject ();
			consumeSlot.transform.SetParent (addCashBuff_transform.transform, false);
			consumeSlot.transform.localScale = Vector3.one;

			ShopCashBuffSlot shopCashBuffSlot = consumeSlot.GetComponent<ShopCashBuffSlot> ();
			shopCashBuffSlot.icon_Image.sprite =  ObjectCashing.Instance.LoadSpriteFromCache (iconPaths[_index]);
			shopCashBuffSlot.StartTimer ();
			shopCashBuffSlot.shopCash = this;

			shopCashBuffSlot.sSlotName = getCashItemInfo [_index].sItemName;
			shopCashBuffSlot.pool_Obj = CashBuffSlotPool;
			shopCashBuffSlots [_index] = shopCashBuffSlot;
		}



		ScoreManager.ScoreInstance.GoldPlus (_gold);
		ScoreManager.ScoreInstance.HonorPlus (_honor);
		ScoreManager.ScoreInstance.RubyPlus( ((int)-_ruby));

	}

	public void LoadBooster(E_BOOSTERTYPE _index, int _CurMin, float _CurSec)
	{
		if (_index == E_BOOSTERTYPE.E_BOOSTERTYPE_GOLD)
		{
			GameObject consumeSlot = CashBuffSlotPool.GetObject ();
			consumeSlot.transform.SetParent (addCashBuff_transform.transform, false);
			consumeSlot.transform.localScale = Vector3.one;

			ShopCashBuffSlot shopCashBuffSlot = consumeSlot.GetComponent<ShopCashBuffSlot> ();
			shopCashBuffSlot.icon_Image.sprite =  ObjectCashing.Instance.LoadSpriteFromCache (iconPaths[(int)_index]);
			shopCashBuffSlot.shopCash = this;
			shopCashBuffSlot.sSlotName = getCashItemInfo [(int)_index].sItemName;
			shopCashBuffSlot.LoadTimer (_CurMin, (int)_CurSec);
		
			shopCashBuffSlot.pool_Obj = CashBuffSlotPool;

		}
		if (_index == E_BOOSTERTYPE.E_BOOSTERTYPE_HONOR)
		{
			GameObject consumeSlot = CashBuffSlotPool.GetObject ();
			consumeSlot.transform.SetParent (addCashBuff_transform.transform, false);
			consumeSlot.transform.localScale = Vector3.one;

			ShopCashBuffSlot shopCashBuffSlot = consumeSlot.GetComponent<ShopCashBuffSlot> ();
			shopCashBuffSlot.icon_Image.sprite =  ObjectCashing.Instance.LoadSpriteFromCache (iconPaths[(int)_index]);
			shopCashBuffSlot.sSlotName = getCashItemInfo [(int)_index].sItemName;
			shopCashBuffSlot.shopCash = this;
			shopCashBuffSlot.LoadTimer (_CurMin, (int)_CurSec);
	
			shopCashBuffSlot.pool_Obj = CashBuffSlotPool;
		}
		if (_index == E_BOOSTERTYPE.E_BOOSTERTYPE_STAFF) 
		{
			GameObject consumeSlot = CashBuffSlotPool.GetObject ();
			consumeSlot.transform.SetParent (addCashBuff_transform.transform, false);
			consumeSlot.transform.localScale = Vector3.one;

			ShopCashBuffSlot shopCashBuffSlot = consumeSlot.GetComponent<ShopCashBuffSlot> ();
			shopCashBuffSlot.icon_Image.sprite =  ObjectCashing.Instance.LoadSpriteFromCache (iconPaths[(int)_index]);
			shopCashBuffSlot.sSlotName = getCashItemInfo [(int)_index].sItemName;
			shopCashBuffSlot.shopCash = this;
			shopCashBuffSlot.LoadTimer (_CurMin, (int)_CurSec);

			shopCashBuffSlot.pool_Obj = CashBuffSlotPool;
		}
		if (_index == E_BOOSTERTYPE.E_BOOSTERTYPE_ATTACK) 
		{
			GameObject consumeSlot = CashBuffSlotPool.GetObject ();
			consumeSlot.transform.SetParent (addCashBuff_transform.transform, false);
			consumeSlot.transform.localScale = Vector3.one;

			ShopCashBuffSlot shopCashBuffSlot = consumeSlot.GetComponent<ShopCashBuffSlot> ();
			shopCashBuffSlot.icon_Image.sprite =  ObjectCashing.Instance.LoadSpriteFromCache (iconPaths[(int)_index]);
			shopCashBuffSlot.sSlotName = getCashItemInfo [(int)_index].sItemName;
			shopCashBuffSlot.shopCash = this;
			shopCashBuffSlot.LoadTimer (_CurMin, (int)_CurSec);
		
			shopCashBuffSlot.pool_Obj = CashBuffSlotPool;
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

	#region InApp
	private bool IsInitialized()
	{
		return (storeController != null && extensionProvider != null);
	}

	public void InitializePurchasing()
	{
		if (IsInitialized())
			return;

		var module = StandardPurchasingModule.Instance();

		ConfigurationBuilder builder = ConfigurationBuilder.Instance(module);

		builder.AddProduct(productId_ruby100, ProductType.Consumable, new IDs
			{
				{ productId_ruby100, AppleAppStore.Name },
				{ productId_ruby100, GooglePlay.Name },
			});

		builder.AddProduct(productId_ruby400, ProductType.Consumable, new IDs
			{
				{ productId_ruby400, AppleAppStore.Name },
				{ productId_ruby400, GooglePlay.Name }, }
		);

		builder.AddProduct(productId_ruby700, ProductType.Consumable, new IDs
			{
				{ productId_ruby700, AppleAppStore.Name },
				{ productId_ruby700, GooglePlay.Name },
			});

		builder.AddProduct(productId_ruby1500, ProductType.Consumable, new IDs
			{
				{ productId_ruby1500, AppleAppStore.Name },
				{ productId_ruby1500, GooglePlay.Name },
			});

		builder.AddProduct(productId_packageForBeginner, ProductType.Consumable, new IDs
			{
				{ productId_packageForBeginner, AppleAppStore.Name },
				{ productId_packageForBeginner, GooglePlay.Name },
			});

		builder.AddProduct(productId_packageIce, ProductType.Consumable, new IDs
			{
				{ productId_packageIce, AppleAppStore.Name },
				{ productId_packageIce, GooglePlay.Name },
			});

		builder.AddProduct(productId_packageSasin, ProductType.Consumable, new IDs
			{
				{ productId_packageSasin, AppleAppStore.Name },
				{ productId_packageSasin, GooglePlay.Name },
			});
		


		builder.AddProduct(productId_packageFire, ProductType.Consumable, new IDs
			{
				{ productId_packageFire, AppleAppStore.Name },
				{ productId_packageFire, GooglePlay.Name },
			});



		builder.AddProduct(productId_packageMusic, ProductType.Consumable, new IDs
			{
				{ productId_packageMusic, AppleAppStore.Name },
				{ productId_packageMusic, GooglePlay.Name },
			});



		UnityPurchasing.Initialize(this, builder);
	}

	public void BuyProductID(string productId)
	{
		try
		{
			if (IsInitialized())
			{
				Product p = storeController.products.WithID(productId);

				if (p != null && p.availableToPurchase)
				{
					Debug.Log(string.Format("Purchasing product asychronously: '{0}'", p.definition.id));
					storeController.InitiatePurchase(p);
				}
				else
				{
					Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
				}
			}
			else
			{
				Debug.Log("BuyProductID FAIL. Not initialized.");
			}
		}
		catch (Exception e)
		{
			Debug.Log("BuyProductID: FAIL. Exception during purchase. " + e);
		}
	}

	public void RestorePurchase()
	{
		if (!IsInitialized())
		{
			Debug.Log("RestorePurchases FAIL. Not initialized.");
			return;
		}

		if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer)
		{
			Debug.Log("RestorePurchases started ...");

			var apple = extensionProvider.GetExtension<IAppleExtensions>();

			apple.RestoreTransactions
			(
				(result) => { Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore."); }
			);
		}
		else
		{
			Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
		}
	}

	public void OnInitialized(IStoreController sc, IExtensionProvider ep)
	{
		Debug.Log("OnInitialized : PASS");

		storeController = sc;
		extensionProvider = ep;
	}

	public void OnInitializeFailed(InitializationFailureReason reason)
	{
		Debug.Log("OnInitializeFailed InitializationFailureReason:" + reason);
	}

	public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
	{
		Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));

		switch (args.purchasedProduct.definition.id)
		{
		case productId_ruby100:


			ScoreManager.ScoreInstance.RubyPlus (100);
			break;

		case productId_ruby400:

			ScoreManager.ScoreInstance.RubyPlus (400);

			break;

		case productId_ruby700:
			// ex) gem 100개 지급
			ScoreManager.ScoreInstance.RubyPlus (700);
			break;

		case productId_ruby1500:
			ScoreManager.ScoreInstance.RubyPlus (1500);
		
			break;

		case productId_packageForBeginner:
			ScoreManager.ScoreInstance.RubyPlus (100);
			ScoreManager.ScoreInstance.HonorPlus (200);
			SpawnManager.Instance.list_ArbaitUI [(int)ReadOnlys.E_ARBAIT.E_CLEA].BuyCharacter ();
			GameManager.Instance.GetPlayer ().changeStats.bIsBeginnerPackageBuy = true;
			ShopCashSlot shopCashSlot = addSlotObjs [(int)E_CASHSHOPTYPE.E_CASHSHOPTYPE_PACKAGE].transform.GetChild (0).GetComponent<ShopCashSlot> ();
			shopCashSlot.BlockImage_Obj.SetActive (true);

			break;
		case productId_packageIce:
			ScoreManager.ScoreInstance.RubyPlus (200);
			ScoreManager.ScoreInstance.HonorPlus (500);
			GameManager.Instance.GetPlayer ().changeStats.bIsBossIcePackageBuy = true;
			ShopCashSlot shopCashSlot2 = addSlotObjs [(int)E_CASHSHOPTYPE.E_CASHSHOPTYPE_PACKAGE].transform.GetChild (1).GetComponent<ShopCashSlot> ();
			shopCashSlot2.BlockImage_Obj.SetActive (true);
			break;
		case productId_packageSasin:
			ScoreManager.ScoreInstance.RubyPlus (200);
			ScoreManager.ScoreInstance.HonorPlus (500);
			GameManager.Instance.GetPlayer ().changeStats.bIsBossSasinPackageBuy = true;
			ShopCashSlot shopCashSlot3 = addSlotObjs [(int)E_CASHSHOPTYPE.E_CASHSHOPTYPE_PACKAGE].transform.GetChild (2).GetComponent<ShopCashSlot> ();
			shopCashSlot3.BlockImage_Obj.SetActive (true);
			break;
		case productId_packageFire:
			ScoreManager.ScoreInstance.RubyPlus (200);
			ScoreManager.ScoreInstance.HonorPlus (500);
			GameManager.Instance.GetPlayer ().changeStats.bIsBossFirePackageBuy = true;
			ShopCashSlot shopCashSlot4 = addSlotObjs [(int)E_CASHSHOPTYPE.E_CASHSHOPTYPE_PACKAGE].transform.GetChild (3).GetComponent<ShopCashSlot> ();
			shopCashSlot4.BlockImage_Obj.SetActive (true);
		
			break;
		case productId_packageMusic:
			ScoreManager.ScoreInstance.RubyPlus (200);
			ScoreManager.ScoreInstance.HonorPlus (500);
			GameManager.Instance.GetPlayer ().changeStats.bIsBossMusicPackageBuy = true;
			ShopCashSlot shopCashSlot5 = addSlotObjs [(int)E_CASHSHOPTYPE.E_CASHSHOPTYPE_PACKAGE].transform.GetChild (4).GetComponent<ShopCashSlot> ();
			shopCashSlot5.BlockImage_Obj.SetActive (true);
			break;
		}


		return PurchaseProcessingResult.Complete;
	}

	public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
	{
		Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
	}
	#endregion





}
