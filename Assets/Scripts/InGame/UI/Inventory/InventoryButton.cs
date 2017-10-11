using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InventoryButton : MonoBehaviour {

    private Button button;

	public Image WeaponImage;
	public Text EnhanceText;

	public GameObject EnhanceObject;
	public GameObject WeaponObject;
    public GameObject EquipWeapon;

	private InventoryShowPanel inventoryPanel;

    private CGameEquiment equimentData;

    void Awake()
    {
        button = GetComponent<Button>();

        button.onClick.AddListener(HandleClick);
    }

	void OnEnable()
	{
		
	}

	public void SetInit()
	{
		equimentData = null;

		EnhanceObject.SetActive (false);

		WeaponObject.SetActive (false);

		EquipWeapon.SetActive (false);
	}

	public void Setup(CGameEquiment currentEquiment,InventoryShowPanel _inventoryPanel)
    {
        equimentData = currentEquiment;
        
		inventoryPanel = _inventoryPanel;

		currentEquiment.strName = GameManager.Instance.cUnicodeData [currentEquiment.nUnicode].strName;

		EnhanceText.text = string.Format ("+{0}", equimentData.nStrenthCount);

		WeaponImage.sprite = ObjectCashing.Instance.LoadSpriteFromCache(equimentData.strResource);

		WeaponObject.SetActive (true);
		EnhanceObject.SetActive (true);

        if (currentEquiment.bIsEquip)
            EquipWeapon.SetActive(true);

        else
            EquipWeapon.SetActive(false);
        //이미지 등록
    }

    //버튼 클릭시 아이템을 보여주기 위함
    public void HandleClick()
    {
		if (equimentData == null)
			return;

		if (equimentData.nIndex >= 10000) 
		{
			inventoryPanel.SetUp (equimentData);
		}
		else
			inventoryPanel.SetUp (equimentData);
    }
}
