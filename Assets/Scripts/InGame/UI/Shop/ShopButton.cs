using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ShopButton : MonoBehaviour {

    public int nIndex;
    public bool bIsBuy { get; set; }

    CGameEquiment equitMent;

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

		NoneItemImage.sprite = BuySprite;

		NoneItemObject.SetActive (true);

		shop.SaveShopList ();

		shop.AllNoneDisable ();

		shop.FirstCheck ();
	}
}
