using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using System;

public enum E_CASHSHOPTYPE
{
	E_CASHSHOPTYPE_CONSUME =0,
	E_CASHSHOPTYPE_RUBY,
	E_CASHSHOPTYPE_PACKAGE,
	E_CASHSHOPTYPE_HONOR
}

public class ShopCash : MonoBehaviour , IStoreListener
{
	private static IStoreController storeController;
	private static IExtensionProvider extensionProvider;

	// 상품ID는 구글 개발자 콘솔에 등록한 상품ID와 동일하게 해주세요.
	public const string productId1 = "GoldBooster";
	public const string productId2 = "HonorBooster";
	public const string productId3 = "ArbaitBooster";
	public const string productId4 = "FreePass";
	public const string productId5 = "gem5";

	public GameObject [] Cashpanels;		//tap시에 끄고 킬 obj
	public RectTransform [] addSlotObjs;		//아이템 슬롯 넣을때 넣는 obj
	public SimpleObjectPool cashItemSlotPool;
	public GameCashItemShop [] getCashItemInfo = null;


	private bool isSetUpConsume = false;
	private bool isSetUpRuby = false;
	private bool isSetUpPackage = false;
	private bool isSetUpHonor = false;

	private float fAddSlotWidth = 250.0f;

	//캐시 상점 실행에 호출
	public void StartSetUp()
	{
		InitializePurchasing();

		if (getCashItemInfo == null || getCashItemInfo.Length == 0 )
			getCashItemInfo = GameManager.Instance.cCashItemShopInfo;
		
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
		//Debug.Log ("SetUp CashStore");
		if (addSlotObjs [_index].childCount != 0) 
		{
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

					//명예와 소모성 아이템은 루비로 나머지는 현금으로
					if (getCashItemInfo [i].nType == (int)E_CASHSHOPTYPE.E_CASHSHOPTYPE_CONSUME || getCashItemInfo [i].nType == (int)E_CASHSHOPTYPE.E_CASHSHOPTYPE_HONOR)
						shopCashSlot.itemBuyValue_Text.text = string.Format ("{0}", getCashItemInfo [i].fRuby);
					else
					{
						shopCashSlot.itemBuyValue_Text.text = string.Format ("{0}", getCashItemInfo [i].fCash);
						shopCashSlot.itemBuy_Button.onClick.RemoveListener (() => BuyProductID (productId1));
						shopCashSlot.itemBuy_Button.onClick.AddListener (() => BuyProductID (productId1));
					}
				
					Vector2 addSize = new Vector2 (addSlotObjs [_index].sizeDelta.x + fAddSlotWidth, addSlotObjs [_index].sizeDelta.y);
					addSlotObjs [_index].sizeDelta = addSize;
				}
			}
		}
	}


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

		builder.AddProduct(productId1, ProductType.Consumable, new IDs
			{
				{ productId1, AppleAppStore.Name },
				{ productId1, GooglePlay.Name },
			});

		builder.AddProduct(productId2, ProductType.Consumable, new IDs
			{
				{ productId2, AppleAppStore.Name },
				{ productId2, GooglePlay.Name }, }
		);

		builder.AddProduct(productId3, ProductType.Consumable, new IDs
			{
				{ productId3, AppleAppStore.Name },
				{ productId3, GooglePlay.Name },
			});

		builder.AddProduct(productId4, ProductType.Consumable, new IDs
			{
				{ productId4, AppleAppStore.Name },
				{ productId4, GooglePlay.Name },
			});

		builder.AddProduct(productId5, ProductType.Consumable, new IDs
			{
				{ productId5, AppleAppStore.Name },
				{ productId5, GooglePlay.Name },
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
		case productId1:

			// ex) gem 10개 지급

			break;

		case productId2:

			// ex) gem 50개 지급

			break;

		case productId3:

			// ex) gem 100개 지급

			break;

		case productId4:

			// ex) gem 300개 지급

			break;

		case productId5:

			// ex) gem 500개 지급

			break;
		}

		return PurchaseProcessingResult.Complete;
	}

	public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
	{
		Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
	}






}
