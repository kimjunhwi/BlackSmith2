using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CuponPanel : MonoBehaviour {

	public Button CheckButton;
	public InputField inputString;
	public bool bIsCheck = false;

	public void CuponCheckButton()
	{
		PlayerPrefs.DeleteKey ("HasCupon");

		if(PlayerPrefs.HasKey("HasCupon"))
		{
			GameManager.Instance.Window_notice ("이미 쿠폰을 사용 하셨습니다.",null);

			return;
		}

		bIsCheck = GameManager.Instance.CheckCupon (inputString.text);

		if (bIsCheck) 
		{
			PlayerPrefs.SetString ("HasCupon", "true");

			PlayerPrefs.Save ();

			ScoreManager.ScoreInstance.RubyPlus (300);

			gameObject.SetActive (false);
		}
	}
}
