using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum E_SMITH
{
	E_MACHA_UNDER = 1,
	E_MACHA_UP,
	E_STREET_UNDER,
	E_STREET_UP,
	E_STORE_HOUSE,
	E_STORE_LOGO,
	E_STORE_SLOT,
	E_SMITH_HOUSE,
	E_SMITH_LOGO,
	E_SMITH_SLOT,
}

public class BlackSmith : MonoBehaviour {

	public GameObject MachaObject;
	public GameObject StreetShopObject;
	public GameObject StoreHouseObject;
	public GameObject BlackSmithObject;

	// Use this for initialization
	void Start () {
		SettingSmith (GameManager.Instance.GetPlayer ().GetSmithLevel ());
	}
	
	public void SettingSmith(int nLevel)
	{
		if (nLevel < 1 || nLevel > 10)
			return;

		MachaObject.SetActive (false);
		StreetShopObject.SetActive (false);
		StoreHouseObject.SetActive (false);
		BlackSmithObject.SetActive (false);

		switch (nLevel) 
		{
		case (int)E_SMITH.E_MACHA_UNDER:
			MachaObject.SetActive (true);
			MachaObject.transform.GetChild (0).gameObject.SetActive (true);
			MachaObject.transform.GetChild (1).gameObject.SetActive (false);
			break;
		case (int)E_SMITH.E_MACHA_UP:
			MachaObject.SetActive (true);
			MachaObject.transform.GetChild (0).gameObject.SetActive (true);
			MachaObject.transform.GetChild (1).gameObject.SetActive (true);
			break;
		case (int)E_SMITH.E_STREET_UNDER:
			StreetShopObject.SetActive (true);
			StreetShopObject.transform.GetChild (0).gameObject.SetActive (true);
			StreetShopObject.transform.GetChild (1).gameObject.SetActive (false);
			break;
		case (int)E_SMITH.E_STREET_UP:
			StreetShopObject.SetActive (true);
			StreetShopObject.transform.GetChild (0).gameObject.SetActive (true);
			StreetShopObject.transform.GetChild (1).gameObject.SetActive (true);
			break;
		case (int)E_SMITH.E_STORE_HOUSE:
			StoreHouseObject.SetActive (true);
			StoreHouseObject.transform.GetChild (0).gameObject.SetActive (true);
			StoreHouseObject.transform.GetChild (1).gameObject.SetActive (false);
			StoreHouseObject.transform.GetChild (2).gameObject.SetActive (false);
			break;
		case (int)E_SMITH.E_STORE_LOGO:
			StoreHouseObject.SetActive (true);
			StoreHouseObject.transform.GetChild (0).gameObject.SetActive (true);
			StoreHouseObject.transform.GetChild (1).gameObject.SetActive (true);
			StoreHouseObject.transform.GetChild (2).gameObject.SetActive (false);
			break;
		case (int)E_SMITH.E_STORE_SLOT:
			StoreHouseObject.SetActive (true);
			StoreHouseObject.transform.GetChild (0).gameObject.SetActive (true);
			StoreHouseObject.transform.GetChild (1).gameObject.SetActive (true);
			StoreHouseObject.transform.GetChild (2).gameObject.SetActive (true);
			break;
		case (int)E_SMITH.E_SMITH_HOUSE:
			BlackSmithObject.SetActive (true);
			BlackSmithObject.transform.GetChild (0).gameObject.SetActive (true);
			BlackSmithObject.transform.GetChild (1).gameObject.SetActive (false);
			BlackSmithObject.transform.GetChild (2).gameObject.SetActive (false);
			break;
		case (int)E_SMITH.E_SMITH_LOGO:
			BlackSmithObject.SetActive (true);
			BlackSmithObject.transform.GetChild (0).gameObject.SetActive (true);
			BlackSmithObject.transform.GetChild (1).gameObject.SetActive (true);
			BlackSmithObject.transform.GetChild (2).gameObject.SetActive (false);
			break;
		case (int)E_SMITH.E_SMITH_SLOT:
			BlackSmithObject.SetActive (true);
			BlackSmithObject.transform.GetChild (0).gameObject.SetActive (true);
			BlackSmithObject.transform.GetChild (1).gameObject.SetActive (true);
			BlackSmithObject.transform.GetChild (2).gameObject.SetActive (true);
			break;
		}
	}
}
