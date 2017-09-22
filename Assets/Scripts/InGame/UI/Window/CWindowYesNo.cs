using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine.UI;

public class CWindowYesNo : CWindow
{
    public Button m_button_Ads;
    public Button m_button_Goods;
	public Button m_button_delete;

	public Text RubyText;
	public Text strTitleText;

	public void Show(string _strTitle, string strValue, Action<string> _callback)
    {
        base.Show(null, _callback);

		m_button_Ads.onClick.AddListener(OnYes);
		m_button_Goods.onClick.AddListener(OnNo);
		m_button_delete.onClick.AddListener (OnDelete);

		RubyText.text = strValue;
		strTitleText.text = _strTitle;
    }
    //public override void Show(GameObject _root, Action<string> _callback)
    //{
    //    base.Show(_root, _callback);
    //}

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
