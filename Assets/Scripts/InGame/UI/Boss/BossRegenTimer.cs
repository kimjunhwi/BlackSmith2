using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossRegenTimer : MonoBehaviour 
{
	public Text bossRegenTimerText;

	private bool isTimeOn = false;
	private string strTime ="";

	System.DateTime StartedTime = new System.DateTime();				//시작일 
	System.DateTime EndData;											//게임을 끌 때의 데이터
	System.TimeSpan timeCal;



	private int curMin;													//현재 분
	private float fCurSec;
	private int nInitTime_Min = 119;
	private int nInitTime_sec = 59;

	public BossCreator bossCreator;

	public void BossRegenTimeSave()
	{
		EndData = System.DateTime.Now;
		PlayerPrefs.SetString ("BossRegenTime", EndData.ToString ());
		GameManager.Instance.cBossPanelListInfo [0].nBossRegenCurMin = curMin;
		GameManager.Instance.cBossPanelListInfo [0].fBossRegenCurSec = fCurSec;
		Debug.Log ("BossRegen Time Save : " + EndData.ToString ());
	}
	void Update()
	{
		if (isTimeOn == true) 
		{
			StartTimer (nInitTime_Min, nInitTime_sec);
			isTimeOn = false;
		}
	}

	public void BossRegenTimeLoad()
	{
		//BossInvitementSaveTime
		//저장된 초대장 시간이 있다면
		if (PlayerPrefs.HasKey ("BossRegenTime"))
		{
			strTime = PlayerPrefs.GetString ("BossRegenTime");
			EndData = System.Convert.ToDateTime (strTime);

			Debug.Log ("BossRegen Time Load : " + EndData.ToString ());
		} 
		//없으면
		else 
		{
			EndData = System.DateTime.Now;
			PlayerPrefs.SetString ("BossRegenTime", EndData.ToString ());
			Debug.Log ("BossRegen init Time : " + EndData.ToString ());
			StartTimer(nInitTime_Min,  nInitTime_sec);
			return;
		}

		StartedTime = System.DateTime.Now;
		timeCal = StartedTime - EndData;

		int nStartTime = StartedTime.Hour * 3600 + StartedTime.Minute * 60 + StartedTime.Second;
		int nEndTime = EndData.Hour * 3600 + EndData.Minute * 60 + EndData.Second;
		int nCheck = Mathf.Abs(nEndTime - nStartTime);

		//하루가 지나거나 2시간이 지나거나 현재 초대장이 가득 찼으면
		if (timeCal.Days != 0 || nCheck >= 7200) 
		{
			Debug.Log ("BossChanllege ReFill");
			//남은시간을 계산해서 계속 해서 흐른다
			bossRegenTimerText.enabled = true;
			//지난 분에서 전체 분을 나머지 계산해서 구한다
			int nPassedTime_Min = (int)timeCal.TotalMinutes;		//전체 분
			nPassedTime_Min -= nInitTime_Min;

			int nPassedTime_Sec = (int)timeCal.TotalSeconds % 60; 	//전채 초에서 나머지
			nPassedTime_Sec  = nInitTime_sec - nPassedTime_Sec;

			//bossCreator.BossChanllengeCountToMax ();
			//시간은 다 지나도 계속해서 흐른다
			StartTimer(nInitTime_Min,  nInitTime_sec);

		}
		//
		else 
		{
			int nPassedTime_Min = (int)timeCal.TotalMinutes;	//전체 분s
			int nPassedTime_Sec = (int)timeCal.TotalSeconds % 60; 	//전채 초에서 나머지

			//120분이 지나지 않았다면 저장된 분에서 지나간 분 만큼 뺀 시간을 시작한다
			if (nPassedTime_Min < 119) 
			{
				int ResultTime_Min = GameManager.Instance.cBossPanelListInfo [0].nBossRegenCurMin - nPassedTime_Min;
				if (ResultTime_Min < 0)
					ResultTime_Min = -ResultTime_Min;

				int ResultTime_Sec = (int)GameManager.Instance.cBossPanelListInfo [0].fBossRegenCurSec - nPassedTime_Sec;
				if (ResultTime_Sec < 0)
					ResultTime_Sec = -ResultTime_Sec;


				StartTimer(ResultTime_Min, ResultTime_Sec);
			} 
			bossRegenTimerText.enabled = true;
		
		}
	}


	public void StartTimer(int _Min, int _Sec)
	{
		StartCoroutine (Timer(_Min, _Sec));
	}

	public IEnumerator Timer(int _curMin, int _curSec)
	{
		int second = 0;

		fCurSec = (float)_curSec;
		curMin = _curMin;


		while (curMin >= 0f) 
		{
			fCurSec -= Time.deltaTime;
			second = (int)fCurSec;

			if(second < 10)
				bossRegenTimerText.text = curMin.ToString () + ":" +"0"+second.ToString ();
			else
				bossRegenTimerText.text = curMin.ToString () + ":" + second.ToString ();
			


			nInitTime_Min = curMin;
			nInitTime_sec = second;

			if (curMin == 0 && second == 0f)
			{
				isTimeOn = true;
				//break;

				bossCreator.nBossSasinLeftCount = 3;
				bossCreator.nBossIceLeftCount = 3;
				bossCreator.nBossMusicLeftCount = 3;
				bossCreator.nBossFireLeftCount = 3;

				bossCreator.bossElementList[0].BossLeftCount_Text.text = 
					string.Format("{0} / {1}",  bossCreator.nBossIceLeftCount, bossCreator.nBossMaxLeftCount);

				bossCreator.bossElementList[1].BossLeftCount_Text.text = 
					string.Format("{0} / {1}",  bossCreator.nBossSasinLeftCount, bossCreator.nBossMaxLeftCount);
				
				bossCreator.bossElementList[2].BossLeftCount_Text.text = 
					string.Format("{0} / {1}",  bossCreator.nBossFireLeftCount, bossCreator.nBossMaxLeftCount);
				
				bossCreator.bossElementList[3].BossLeftCount_Text.text = 
					string.Format("{0} / {1}",  bossCreator.nBossMusicLeftCount, bossCreator.nBossMaxLeftCount);

				bossCreator.BossPanelInfoSave ();
				

				nInitTime_Min = 119;
				nInitTime_sec = 59;
				break;
			}

			if (curMin != 0 && second == 0f) 
			{
				fCurSec = 59f;
				curMin--;
			}
				
			yield return null;
		}
		yield  break;
	}


}
