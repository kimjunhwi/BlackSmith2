using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopCashBuffSlot : MonoBehaviour {

	public Image icon_Image;
	public Text Timer_Text;

	private float fCurSec = 0f;
	private int curMin = 0;

	public SimpleObjectPool pool_Obj;

	public void StartTimer()
	{
		int initMin = 30;
		int initSec = 59;

		StartCoroutine (Timer (initMin, initSec));

	}

	public IEnumerator Timer(int _curMin, int _curSec)
	{
		int second = 0;

		fCurSec = (float)_curSec;
		curMin = _curMin;


		while (curMin >= 0f) {
			fCurSec -= Time.deltaTime;
			second = (int)fCurSec;

			if (second < 10)
				Timer_Text.text = curMin.ToString () + ":" + "0" + second.ToString ();
			else
				Timer_Text.text = curMin.ToString () + ":" + second.ToString ();

			//inviteMentCount_Text.text = nInviteMentCurCount.ToString () + " / " + nInviteMentMaxCount.ToString ();


			//nInitTime_Min = curMin;
			//nInitTime_sec = second;

			if (curMin == 0 && second <= 0f) {
				//isTimeEnd = true;
				//break;
				curMin = 0;
				fCurSec = 0;
				//시간이 다되면 추가하기 버튼 active
				//questManager.QuestInit ();
				pool_Obj.ReturnObject (gameObject);
				
			}

			if (curMin != 0 && second == 0f) {
				fCurSec = 59f;
				curMin--;
			}



			yield return null;
		}
		yield  break;
	}
}
