using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BossSmallFireObject : MonoBehaviour, IPointerDownHandler
{
	GameObject getInfoGameObject;				//터치하는 오브젝트 정보
	public int nTouchCount;
	public SimpleObjectPool smallFireObjPull;	//해당 오브젝트 풀
	public RectTransform parentTransform;	
	public BossFire bossfire;
	public RepairObject repairObj;
	public RectTransform rectTrasform;


	public void OnPointerDown (PointerEventData eventData)
	{
		getInfoGameObject = eventData.pointerEnter;

		if (getInfoGameObject.gameObject == null)
			return;

		if (getInfoGameObject.gameObject.name == "SmallFireTouch") 
		{

			if (nTouchCount > 0) 
			{
				nTouchCount--;

				if (nTouchCount == 2) 
				{
					//rectTrasform = getInfoGameObject.GetComponentInChildren<RectTransform> ();
					rectTrasform.sizeDelta = new Vector2 (rectTrasform.sizeDelta.x - 100f, rectTrasform.sizeDelta.y - 100f);

				}
				if (nTouchCount == 1) 
				{
					//rectTrasform = getInfoGameObject.GetComponentInChildren<RectTransform> ();
					rectTrasform.sizeDelta = new Vector2 (rectTrasform.sizeDelta.x - 200f, rectTrasform.sizeDelta.y - 200f);

				}
				if (nTouchCount == 0) 
				{
					SoundManager.instance.PlaySound (eSoundArray.ES_BossFireExtingu);
					//불씨 하나당 물 충전량 -3%
					repairObj.fSmallFireMinusWater -= 0.03f;
					//불씨 하나당 온도 증가량 10%
					repairObj.fSmallFirePlusTemperatrue -= 0.1f;
					bossfire.nCurFireCount--;
			
					smallFireObjPull.ReturnObject (getInfoGameObject);
					//Debug.Log ("CurFireMinusWater : " + repairObj.fSmallFireMinusWater + "/ CurPlusTemperatrue : " +  repairObj.fSmallFirePlusTemperatrue);
				}
			}
		}

	}
}
