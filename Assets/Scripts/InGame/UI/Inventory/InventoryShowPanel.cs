using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ReadOnlys;

public class InventoryShowPanel : MonoBehaviour {

	public Text NameText;
	public Text GradeText;
	public Image WeaponImage;
	public SpriteRenderer MoruImage;
	public Text EnhanceCostText;

	public Button SellButton;
	public Button EquipButton;
	public Button EnhanceButton;
	public Button CloseButton;
	public Button NoneEquipButton;

	public Image EquiImage;

	public Sprite ClearSprite;
	public Sprite EquipSprite;

	public Transform contentsPanel;

	public SimpleObjectPool simpleTextPool;

	public GameObject EquipObject;
	public GameObject NoneEquipObject;

	CGameEquiment ItemData;

    Player player;

	string[] unit = new string[]{ "G", "K", "M", "B", "T", "aa", "bb", "cc", "dd", "ee" }; 

	void Awake()
	{
		SellButton.onClick.AddListener (SellItem);
        EquipButton.onClick.AddListener(EquipItem);
		EnhanceButton.onClick.AddListener (EnhanceItem);
		CloseButton.onClick.AddListener (ClosePanel);
		NoneEquipButton.onClick.AddListener (NoneEquipItem);

        player = GameManager.Instance.player;

		gameObject.SetActive (false);
		NoneEquipObject.SetActive (false);
	}

	private void EnhanceItem()
	{

		if (ItemData.nBasicGold * Mathf.Pow(1.1f, ItemData.nStrenthCount - 1) <= ScoreManager.ScoreInstance.GetGold ()) {
				
			ScoreManager.ScoreInstance.GoldPlus (-ItemData.nBasicGold * Mathf.Pow(1.1f, ItemData.nStrenthCount - 1));

			Debug.Log ("강화 성공!!");

			if (ItemData.fReapirPower 		!= 0) ItemData.fReapirPower 	+= ItemData.fOptionPlus;
			if (ItemData.fArbaitRepair      != 0) ItemData.fArbaitRepair 	+= ItemData.fOptionPlus;
			if (ItemData.fHonorPlus         != 0) ItemData.fHonorPlus 		+= ItemData.fOptionPlus;
			if (ItemData.fGoldPlus          != 0) ItemData.fGoldPlus 		+= ItemData.fOptionPlus;
			if (ItemData.fWaterChargePlus   != 0) ItemData.fWaterChargePlus += ItemData.fOptionPlus;
			if (ItemData.fCritical          != 0) ItemData.fCritical 		+= ItemData.fOptionPlus;
			if (ItemData.fCriticalDamage    != 0) ItemData.fCriticalDamage 	+= ItemData.fOptionPlus;
			if (ItemData.fBigCritical       != 0) ItemData.fBigCritical 	+= ItemData.fOptionPlus;
			if (ItemData.fAccuracyRate      != 0) ItemData.fAccuracyRate 	+= ItemData.fOptionPlus;

			if (ItemData.bIsBoss)
				ItemData.fBossOptionValue += ItemData.fBasicOptionValue * ItemData.fOptionPlus * 0.1f;

			ItemData.nStrenthCount++;

			EnhanceCostText.text = ChangeValue(ItemData.nBasicGold * Mathf.Pow(1.1f, ItemData.nStrenthCount - 1));

			ResetItemText ();

			player.PlayerStatsSetting ();

			NameText.text = string.Format("{0} +{1}", ItemData.strName , ItemData.nStrenthCount);
		}
	}

	private void SellItem()
	{
		GameManager.Instance.GetPlayer ().inventory.inventorySlots [ItemData.nSlotIndex].RemoveItem (ItemData);

		ItemData = null;

		RemoveText ();
	}

	private void EquipItem()
	{
		if (ItemData == null)
			return;

        player.EquipItem(ItemData);

		if (ItemData.nSlotIndex == (int)E_EQUIMNET_INDEX.E_WEAPON) 
		{
			MoruImage.sprite = ObjectCashing.Instance.LoadSpriteFromCache( ItemData.strResource);
		}

		EquipObject.SetActive (false);
		NoneEquipObject.SetActive (true);
	}

	private void NoneEquipItem()
	{
		if (ItemData == null)
			return;

		ItemData.bIsEquip = false;

		player.NoneEquipItem (ItemData);

		if (ItemData.nSlotIndex == (int)E_EQUIMNET_INDEX.E_WEAPON) {
			MoruImage.sprite = ObjectCashing.Instance.LoadSpriteFromCache( "ShopItem/0");
		}

		EquipObject.SetActive (true);
		NoneEquipObject.SetActive (false);
	}

	private void ClosePanel()
	{
		gameObject.SetActive (false);
	}

	private void OnDisable()
	{
		RemoveText ();
	}

	private void RemoveText()
	{
		while (contentsPanel.childCount > 0)
		{
			GameObject toRemove = contentsPanel.GetChild(0).gameObject;
			simpleTextPool.ReturnObject(toRemove);
		}
	}

	public void SetUp(CGameEquiment _equiment,string strExplain = null)
	{
		ItemData = _equiment;

		NameText.text = string.Format("{0} +{1}", ItemData.strName , ItemData.nStrenthCount);

		GradeText.text = ItemData.sGrade;

		WeaponImage.sprite = ObjectCashing.Instance.LoadSpriteFromCache(ItemData.strResource);
	
		ResetItemText ();

		EnhanceCostText.text = ChangeValue(ItemData.nBasicGold * Mathf.Pow(1.1f, ItemData.nStrenthCount - 1));

		if (strExplain != null)
			CreateText (strExplain, 0f);

		if (_equiment.bIsEquip == true) 
		{
			if(ItemData.nSlotIndex == (int)E_EQUIMNET_INDEX.E_WEAPON) 
				MoruImage.sprite = ObjectCashing.Instance.LoadSpriteFromCache( _equiment.strResource);
			
			EquipObject.SetActive (false);
			NoneEquipObject.SetActive (true);
		}
		else 
		{
			EquipObject.SetActive (true);
			NoneEquipObject.SetActive (false);
		}

		gameObject.SetActive (true);
	}

	public void CreateText(string strName, float nValue)
	{
		GameObject textObject = simpleTextPool.GetObject();
		textObject.transform.SetParent(contentsPanel,false);
		textObject.transform.localScale = Vector3.one;

		Text newText = textObject.GetComponent<Text>();

		if(nValue == 0.0f)
			newText.text = string.Format("{0}%",  strName);

		else
			newText.text = string.Format("{0}{1}%",  strName , nValue.ToString());
	}


	private void ResetItemText()
	{
		RemoveText ();

		if (ItemData.fReapirPower       != 0) CreateText("수리력 : ", ItemData.fReapirPower);
		if (ItemData.fArbaitRepair      != 0) CreateText("알바수리력증가 : ", ItemData.fArbaitRepair);
		if (ItemData.fHonorPlus         != 0) CreateText("명예증가량 : ", ItemData.fHonorPlus);
		if (ItemData.fGoldPlus          != 0) CreateText("골드증가량 : ", ItemData.fGoldPlus);
		if (ItemData.fWaterChargePlus   != 0) CreateText("물 충전 증가량 : ", ItemData.fWaterChargePlus);
		if (ItemData.fCritical          != 0) CreateText("크리티컬확률 : ", ItemData.fCritical);
		if (ItemData.fCriticalDamage    != 0) CreateText("크리티컬데미지 : ", ItemData.fCriticalDamage);
		if (ItemData.fBigCritical       != 0) CreateText("대성공 : ", ItemData.fBigCritical);
		if (ItemData.fAccuracyRate      != 0) CreateText("명중률 : ", ItemData.fAccuracyRate);

		if (ItemData.bIsBoss)
			CreateText (ItemData.strWeaponExplain, ItemData.fBossOptionValue);
	}

	//값을 수치로 표기하기 위한 함수 
	string ChangeValue(double _dValue)
	{ 
		int[] cVal = new int[10]; 

		int index = 0; 

		string strValue =  string.Format ("{0:####}", _dValue);

		if (_dValue < 10000)
			return strValue;

		while (true) { 
			string last4 = ""; 
			if (strValue.Length >= 4) { 

				last4 = strValue.Substring (strValue.Length - 4); 

				int intLast4 = int.Parse (last4); 

				cVal [index] = intLast4 % 1000; 

				strValue = strValue.Remove (strValue.Length - 3); 
			} else { 
				cVal [index] = int.Parse (strValue); 
				break; 
			} 

			index++; 
		} 

		//1000,00
		//1000,000,00
		//1000,000,000,00

		while (_dValue >= 1000) 
		{
			_dValue *= 0.001f;
		}

		if (index > 0) { 

			if (_dValue >= 100) 
			{
				int nResult = cVal [index] * 1000 + cVal [index - 1]; 

				string strFirstValue = nResult.ToString ().Substring (0, 3);

				string strSecondValue = nResult.ToString ().Substring (3, 1);

				return string.Format ("{0}.{1:##}{2}", strFirstValue, strSecondValue, unit [index]); 
			} else if (_dValue >= 10) 
			{
				int nResult = cVal [index] * 1000 + cVal [index - 1]; 

				string strFirstValue = nResult.ToString ().Substring (0, 2);

				string strSecondValue = nResult.ToString ().Substring (2, 2);

				return string.Format ("{0}.{1:##}{2}", strFirstValue, strSecondValue, unit [index]); 
			} else 
			{
				int nResult = cVal [index] * 1000 + cVal [index - 1]; 

				string strFirstValue = nResult.ToString ().Substring (0, 1);

				string strSecondValue = nResult.ToString ().Substring (1, 2);

				return string.Format ("{0}.{1:##}{2}", strFirstValue, strSecondValue, unit [index]); 
			}
		} 

		return strValue; 
	}

}
