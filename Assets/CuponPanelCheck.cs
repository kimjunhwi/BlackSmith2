using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuponPanelCheck : MonoBehaviour {

	public GameObject CuponPanelObject;

	public void ShowCuponPanel()
	{
		CuponPanelObject.SetActive (true);
	}
}
