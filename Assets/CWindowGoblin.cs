using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class CWindowGoblin : CWindow ,IPointerDownHandler {

	GameObject getInfoGameObject;

	public Button m_button_Ads;
	public Button m_button_Goods;
	public Button m_button_delete;

	public Image m_Goblin_Buff_Image;
	public Image m_Goblin_Goods_Image;

	public GameObject m_Goblin_Buff_Ojbect;
	public GameObject m_Goblin_Goods_Object;

	public Text GoodsText;


	public void Show(string _strTitle, string strValue,Sprite _spriteGoods, Action<string> _callback)
	{
		base.Show(null, _callback);

		m_button_Ads.onClick.AddListener(OnYes);
		m_button_Goods.onClick.AddListener(OnNo);
		m_button_delete.onClick.AddListener (OnDelete);

		m_Goblin_Buff_Ojbect.SetActive (false);
		m_Goblin_Goods_Object.SetActive (false);

		//버프 일경우
		if (strValue == "") {
			GoodsText.text = strValue;
			m_Goblin_Buff_Image.sprite = _spriteGoods;
			m_Goblin_Buff_Ojbect.SetActive (true);
		} else 
		{
			GoodsText.text = strValue;
			m_Goblin_Goods_Image.sprite = _spriteGoods;
			m_Goblin_Goods_Object.SetActive (true);
		}
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

	public void OnPointerDown (PointerEventData eventData)
	{
		getInfoGameObject = eventData.pointerEnter;

		if (getInfoGameObject.gameObject == null)
			return;
		if (SpawnManager.Instance.tutorialPanel.eTutorialState == TutorialOrder.E_TUTORIAL_START_IMAGE04)
			return;

		if (getInfoGameObject.gameObject.name == "BackGroundPanel")
			getInfoGameObject.transform.parent.gameObject.SetActive (false);

	}
}
