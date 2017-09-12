using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BossHint : MonoBehaviour ,IPointerDownHandler 
{
	public bool m_isCheckBossHint = false;

	public void OnPointerDown (PointerEventData eventData)
	{
		GameObject getInfoObj = eventData.pointerEnter;

		if (getInfoObj.name == "HintText")
			return;
		else 
		{
			m_isCheckBossHint = true;
			getInfoObj.SetActive (false);
		}

	}
}
