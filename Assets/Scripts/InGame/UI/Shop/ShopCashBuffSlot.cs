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

	public string sSlotName = "";

	private const string sCashTimeSaveName_Gold = "SaveCashConsume_GoldTime";
	private const string sCashTimeSaveName_Honor = "SaveCashConsume_HonorTime";
	private const string sCashTimeSaveName_Staff = "SaveCashConsume_StaffTime";
	private const string sCashTimeSaveName_Attack = "SaveCashConsume_AttackTime";

	System.DateTime StartData;
	System.DateTime EndData;

	public ShopCash shopCash;

	public void StartTimer()
	{
		int initMin = 29;
		int initSec = 59;

		StartCoroutine (Timer (initMin, initSec));
		EndData = System.DateTime.Now;

		if (sSlotName == "골드부스터")
		{
			PlayerPrefs.SetString (sCashTimeSaveName_Gold, EndData.ToString());
			shopCash.isConumeBuff_Gold = true;
		}

		if (sSlotName == "명예부스터")
		{
			PlayerPrefs.SetString (sCashTimeSaveName_Honor, EndData.ToString());
			shopCash.isConumeBuff_Honor = true;
		}


		if (sSlotName == "직원부스터")
		{
			PlayerPrefs.SetString (sCashTimeSaveName_Staff, EndData.ToString());
			shopCash.isConumeBuff_Staff = true;
		}


		if (sSlotName == "터치부스터")
		{
			PlayerPrefs.SetString (sCashTimeSaveName_Attack, EndData.ToString());
			shopCash.isConumeBuff_Attack = true;
		}
	}

	public void SaveTimer(E_BOOSTERTYPE _index)
	{
		if (_index == E_BOOSTERTYPE.E_BOOSTERTYPE_GOLD)
		{
			GameManager.Instance.player.changeStats.nGoldPlusBuffMinutes = curMin;
			GameManager.Instance.player.changeStats.fGoldPlusBuffSecond = fCurSec;
		}
		if (_index == E_BOOSTERTYPE.E_BOOSTERTYPE_HONOR)
		{
			GameManager.Instance.player.changeStats.nHonorPlusBuffMinutes = curMin;
			GameManager.Instance.player.changeStats.fHonorPlusBuffSecond = fCurSec;
		}
		if (_index == E_BOOSTERTYPE.E_BOOSTERTYPE_STAFF) 
		{
			GameManager.Instance.player.changeStats.nGuestBuffMinutes = curMin;
			GameManager.Instance.player.changeStats.fGuestBuffSecond = fCurSec;

		}
		if (_index == E_BOOSTERTYPE.E_BOOSTERTYPE_ATTACK) 
		{
			GameManager.Instance.player.changeStats.nAttackBuffMinutes = curMin;
			GameManager.Instance.player.changeStats.fAttackBuffSecond = fCurSec;
		}
	}

	public void LoadTimer(int _curMin, int _curSec)
	{
		if (sSlotName == "골드부스터")
			shopCash.isConumeBuff_Gold = true;

		if (sSlotName == "명예부스터")
			shopCash.isConumeBuff_Honor = true;

		if (sSlotName == "직원부스터")
			shopCash.isConumeBuff_Staff = true;

		if (sSlotName == "터치부스터")
			shopCash.isConumeBuff_Attack = true;
		

		StartCoroutine (Timer (_curMin, _curMin));
	}

	public void AddTimer(int _Min, float _Sec)
	{
		curMin += _Min;
		fCurSec += _Sec;
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

				if (sSlotName == "골드부스터")
					shopCash.isConumeBuff_Gold = false;

				if (sSlotName == "명예부스터")
					shopCash.isConumeBuff_Honor = false;

				if (sSlotName == "직원부스터")
					shopCash.isConumeBuff_Staff = false;
				
				if (sSlotName == "터치부스터")
					shopCash.isConumeBuff_Attack = false;
				

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
