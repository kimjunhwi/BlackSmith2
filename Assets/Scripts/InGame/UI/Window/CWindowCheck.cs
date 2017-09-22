using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CWindowCheck : CWindow {

	public Button m_button_Ads;
	public Button m_button_Goods;
	public Button m_button_delete;

	public Text strTitleText;

	public void Show(string _strTitle, Action<string> _callback)
	{
		base.Show(null, _callback);

		m_button_Ads.onClick.AddListener(OnYes);
		m_button_Goods.onClick.AddListener(OnNo);
		m_button_delete.onClick.AddListener (OnDelete);

		strTitleText.text = _strTitle;

		//m_button_yes.text = CGame.Instance.GetText(10006); //yes
		//m_button_no.text = CGame.Instance.GetText(10007); //no

	}

	public void OnYes()
	{
		base.Close();

		if (callback_func != null)
			callback_func("0");

		Destroy(this.gameObject);
	}
	public void OnNo()
	{
		base.Close();

		if (callback_func != null)
			callback_func("1");

		Destroy(this.gameObject);
	}

	public void OnDelete()
	{
		base.Close ();

		Destroy (this.gameObject);
	}
}
