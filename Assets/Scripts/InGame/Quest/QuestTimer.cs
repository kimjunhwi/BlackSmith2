using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestTimer : MonoBehaviour 
{
	public Text QuestTimer_Text;

	private string strTime ="";

	System.DateTime StartedTime = new System.DateTime();				//시작일 
	System.DateTime EndData;											//게임을 끌 때의 데이터
	System.TimeSpan timeCal;

	public float fCurSec;
	public int curMin;													//현재 분
	private int nInitTime_Min = 59;
	private int nInitTime_sec = 59;

	public bool isTimeOn = false;										//시간이 켜져 있는지 아닌지
	public bool isTimeEnd = false;										//시간이 끝났는지 아닌지

	public QusetManager questManager;
	public GameObject addQuestToEmptySpace;


	public void SaveTime()
	{
		System.DateTime EndData = System.DateTime.Now;
		PlayerPrefs.SetString ("EndSaveTime", EndData.ToString ());
		PlayerPrefs.Save ();

		GameManager.Instance.cQuestSaveListInfo [0].nCurLeftMin = curMin;
		GameManager.Instance.cQuestSaveListInfo [0].fCurLeftSec = fCurSec;

		GameManager.Instance.SaveQuestList ();

		Debug.Log ("EndTime :" + EndData.ToString ());
	}


	//초기화 시간이 지났는지
	public bool checkIsTimeGone()
	{
		if (PlayerPrefs.HasKey ("EndSaveTime")) 
		{
			strTime = PlayerPrefs.GetString ("EndSaveTime");
			EndData = System.Convert.ToDateTime (strTime);
		}
		else 
		{
			EndData = System.DateTime.Now;
		}


		StartedTime = System.DateTime.Now;
		Debug.Log ("StartTime :"+ StartedTime + " / EndTime :" + EndData);
		timeCal = StartedTime - EndData;

		int nStartTime = StartedTime.Hour * 3600 + StartedTime.Minute * 60 + StartedTime.Second;
		int nEndTime = EndData.Hour * 3600 + EndData.Minute * 60 + EndData.Second;

		int nCheck = Mathf.Abs(nEndTime - nStartTime);

		//하루가 지나거나 60분이 지나면 그냥 현재 퀘스트 개수만 띄워준다.
		if (timeCal.Days != 0 || nCheck >= 3600) 
		{
			//QuestTimer_Text.text = questManager.nQuestCount.ToString () + " / " + questManager.nQuestMaxCount.ToString ();
			return true;
		} else
			return false;
	}

	public void LoadTimeAndCheckTimeEnd()
	{
		//gameObject.SetActive (true);

		if (PlayerPrefs.HasKey ("EndSaveTime")) 
		{
			strTime = PlayerPrefs.GetString ("EndSaveTime");
			EndData = System.Convert.ToDateTime (strTime);
		}
		else
		{
			Debug.Log ("init Time");
			InitQuestTimer ();
			return;
		}
		StartedTime = System.DateTime.Now;
		Debug.Log ("StartTime :"+ StartedTime + " / EndTime :" + EndData);
		timeCal = StartedTime - EndData;

		int nStartTime = StartedTime.Hour * 3600 + StartedTime.Minute * 60 + StartedTime.Second;
		int nEndTime = EndData.Hour * 3600 + EndData.Minute * 60 + EndData.Second;

		int nCheck = Mathf.Abs(nEndTime - nStartTime);

		//하루가 지나거나 60분이 지나면 추가하기 버튼 활성화 및 시간 비활성화
		if (timeCal.Days != 0 || nCheck >= 3600 ) 
		{
			isTimeEnd = true;
			addQuestToEmptySpace.SetActive (true);
			QuestTimer_Text.enabled = false;
		}
		//
		else 
		{
			int nPassedTime_Min = (int)timeCal.TotalMinutes;
			int nPassedTime_Sec = (int)timeCal.TotalSeconds % 60;

			//60분이 지나지 않았으면 현재까지 지난 시간을 띄워준다
			//Local Load
			if (nPassedTime_Min < 60 && GameManager.Instance.cQuestSaveListInfo [0].bIsGoogleLoad == false)
			{
				Debug.Log ("Local Load");

				int ResultTime_Min = nInitTime_Min - nPassedTime_Min;
				if (ResultTime_Min < 0)
					ResultTime_Min = -ResultTime_Min;

				int ResultTime_Sec = nInitTime_sec - nPassedTime_Sec;
				if (ResultTime_Sec < 0)
					ResultTime_Sec = -ResultTime_Sec;

				if (isTimeOn != true)
					StartQuestTimer (ResultTime_Min, ResultTime_Sec);
				else
				{
					curMin = ResultTime_Min;
					fCurSec = ResultTime_Sec;
				}
			}
			//Google Saved Load
			else 
			{
				Debug.Log ("Cloud Load");
				int ResultTime_Min =  nInitTime_Min - GameManager.Instance.cQuestSaveListInfo [0].nCurLeftMin;
				if (ResultTime_Min < 0)
					ResultTime_Min = -ResultTime_Min;
				
				int ResultTime_Sec =  nInitTime_sec - (int)GameManager.Instance.cQuestSaveListInfo [0].fCurLeftSec;
				if (ResultTime_Sec < 0)
					ResultTime_Sec = -ResultTime_Sec;

				GameManager.Instance.cQuestSaveListInfo [0].bIsGoogleLoad = false;
				GameManager.Instance.SaveQuestList ();

				if (isTimeOn != true)
					StartQuestTimer (ResultTime_Min, ResultTime_Sec);
				else
				{
					curMin = ResultTime_Min;
					fCurSec = ResultTime_Sec;
				}
			}
			//QuestTimer_Text.enabled = true;
		}
	}



	public void StartQuestTimer()
	{
		gameObject.SetActive (true);
		QuestTimer_Text.enabled = true;
		isTimeOn = true;

		StartCoroutine (Timer (nInitTime_Min, nInitTime_sec));
	}

	public void StartQuestTimer(int _min , int _sec)
	{
		gameObject.SetActive (true);
		QuestTimer_Text.enabled = true;
		isTimeOn = true;

		StartCoroutine (Timer (_min, _sec));
	}
	public void InitQuestTimer()
	{
		nInitTime_Min = 59;
		nInitTime_sec = 59;
	
		QuestTimer_Text.enabled = false;
		isTimeOn = false;
		gameObject.SetActive (false);
		//addQuestToEmptySpace.SetActive (false);
		StartQuestTimer ();
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
				QuestTimer_Text.text = curMin.ToString () + ":" +"0"+second.ToString ();
			else
				QuestTimer_Text.text = curMin.ToString () + ":" + second.ToString ();

			//inviteMentCount_Text.text = nInviteMentCurCount.ToString () + " / " + nInviteMentMaxCount.ToString ();


			nInitTime_Min = curMin;
			nInitTime_sec = second;

			if (curMin == 0 && second <= 0f)
			{
				isTimeEnd = true;
				//break;
				curMin = 0;
				fCurSec = 10;
				//시간이 다되면 추가하기 버튼 active
				//questManager.QuestInit ();
				addQuestToEmptySpace.SetActive(true);
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
