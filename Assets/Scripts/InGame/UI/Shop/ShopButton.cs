using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ShopButton : MonoBehaviour {

    public int nIndex;
    public bool bIsBuy { get; set; }

	public CGameEquiment equitMent = null;

    ShopShowPanel showPanel;

	public GameObject ItemButton;
	public GameObject NoneItemObject;


	public Image NoneItemImage;
	public Image WeaponPanelImage;

	public Sprite BuySprite;
	public Sprite LockSprite;
	public Sprite SelectSprite; 
	public Sprite NoneSelectSprite;


    public Button buttonCheck;
    public Text EquitName;
    public Text EquitGold;
	public Text PurchaingText;
    public Image EquitImage;

    public Inventory inventory;

	public Shop shop;

    
    private void Awake()
    {
		ItemButton = transform.Find ("ShopButton").gameObject;

		buttonCheck = ItemButton.GetComponent<Button>();
        buttonCheck.onClick.AddListener(ClickButton);

		ItemButton.SetActive (false);
		NoneItemObject.SetActive (true);

		NoneItemImage.sprite = LockSprite;
    }

	public void SetUp(int _nIndex)
	{
		if (_nIndex < GameManager.Instance.GetPlayer ().changeStats.nShopMaxCount) {
			return;
		} else 
		{
			if (_nIndex == 2) {
				PurchaingText.text = "대장간 레벨 4";

				NoneItemObject.SetActive (true);
			} else if (_nIndex == 3) {
				PurchaingText.text = "대장간 레벨 7";

				NoneItemObject.SetActive (true);
			} else if (_nIndex == 4) {
				PurchaingText.text = "대장간 레벨 10";

				NoneItemObject.SetActive (true);
			}
		}
	}


	public void GetEquiment(Shop _shop, Inventory _inventory,ShopShowPanel _showPanel, CGameEquiment _equimnet)
    {
		shop = _shop;

		bIsBuy = _equimnet.bIsBuy;

        equitMent = _equimnet;

        inventory = _inventory;

        showPanel = _showPanel;

		if (equitMent.bIsBuy) 
		{
			ItemButton.SetActive (false);
			NoneItemObject.SetActive (true);

			NoneItemImage.sprite = BuySprite;
		} 
		else 
		{
			ItemButton.SetActive (true);
			NoneItemObject.SetActive (false);

			WeaponPanelImage.sprite = NoneSelectSprite;
		}

        EquitName.text = equitMent.strName;

        EquitImage.sprite = ObjectCashing.Instance.LoadSpriteFromCache(equitMent.strResource);
    }

    public void ClickButton()
    {
        //옆에 설명창에 보내줌
        Debug.Log("click");

		if (showPanel == null)
			return;

		if (equitMent.bIsBuy)
			return;

		shop.AllNoneDisable ();

		WeaponPanelImage.sprite = SelectSprite;

        showPanel.Setting(this,equitMent);
    }

	public void ItemBuy()
	{
		bIsBuy = true;

		equitMent.bIsBuy = true;

		PurchaingText.text = null;

		NoneItemImage.sprite = BuySprite;

		NoneItemObject.SetActive (true);
		PurchaingText.text = null;

		shop.SaveShopList ();

		shop.AllNoneDisable ();

		shop.FirstCheck ();
	}
}
