using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//제네릭 싱글톤을 사용하지 않은 이유는
//이 씬에서만 유지되게 하기 위함
public class ScoreManager : MonoBehaviour 
{

    private static ScoreManager scireInstance;

    public static ScoreManager ScoreInstance
    {
        get
        {
            if (scireInstance == null)
            {
                scireInstance = GameObject.FindObjectOfType<ScoreManager>();

                //없을경우 만들어서 넣어줌
                if (null == scireInstance)
                {
                    var go = new GameObject("ScoreManager");
                    scireInstance = go.AddComponent<ScoreManager>();
                }
            }

            return scireInstance;
        }
    }

	public PlayerDaysInfo playerDaysInfo;

    public Text goldText;
	public Text honorText;
	public Text rubyText;

	public Text SuccessedGuestCount;



	private double m_dGetGold = 0;
	private double m_dGetHonor = 0;
	private int m_nGetRuby = 0;

	string[] unit = new string[]{ "G", "K", "M", "B", "T", "aa", "bb", "cc", "dd", "ee","ff","gg","hh","ii","jj","kk","ll","mm","nn","oo","pp","qq","rr","ss","tt","uu","vv","ww","xx","yy","zz","aaa", "bbb", "ccc", "ddd", "eee","fff","ggg","hhh","iii","jjj","kkk","lll","mmm","nnn","ooo","ppp","qqq","rrr","sss","ttt","uuu","vvv","www","xxx","yyy","zzz" };

    private void Awake()
    {
        scireInstance = this;

		if (GameManager.Instance.GetPlayer () == null)
			return;

		m_dGetGold = GameManager.Instance.player.GetGold ();
		m_dGetHonor = GameManager.Instance.player.GetHonor ();
		m_nGetRuby = GameManager.Instance.player.GetRuby ();

		goldText.text = ChangeMoney (m_dGetGold);
		honorText.text = ChangeMoney (m_dGetHonor);
		rubyText.text = m_nGetRuby.ToString ();

		SetCurrentDays (GameManager.Instance.player.GetDay ());
		SetMaxDays (GameManager.Instance.player.GetMaxDay ());
    }

	public double GetGold() { return m_dGetGold; }
	public double GetHonor() { return m_dGetHonor; }
	public int GetRuby(){return m_nGetRuby; }

	public void SetSuccessedGuestCount(int _nValue)
	{
		SuccessedGuestCount.text = string.Format ("남은손님 {0} / 10", _nValue);
	}


	//값을 수치로 표기하기 위한 함수 
	public string ChangeMoney(double _dValue)
	{ 
		long[] cVal = new long[100]; 

		int index = 0; 

		string strValue =  string.Format ("{0:####}", _dValue);

		if (_dValue < 10000)
			return strValue;

		while (true) { 
			string last4 = ""; 
			if (strValue.Length >= 4) { 

				last4 = strValue.Substring (strValue.Length - 4); 

				long intLast4 = long.Parse (last4); 

				cVal [index] = intLast4 % 1000; 

				strValue = strValue.Remove (strValue.Length - 3); 
			} else { 
				cVal [index] = long.Parse (strValue); 
				break; 
			} 

			index++; 
		} 

		//1000,00
		//1000,000,00
		//1000,000,000,00

		while (_dValue >= 1000) 
		{
			_dValue *= 0.001f;
		}

		if (index > 0) { 

			if (_dValue >= 100) 
			{
				long nResult = cVal [index] * 1000 + cVal [index - 1]; 

				string strFirstValue = nResult.ToString ().Substring (0, 3);

				string strSecondValue = nResult.ToString ().Substring (3, 1);

				return string.Format ("{0}.{1:##}{2}", strFirstValue, strSecondValue, unit [index]); 
			} else if (_dValue >= 10) 
			{
				long nResult = cVal [index] * 1000 + cVal [index - 1]; 

				string strFirstValue = nResult.ToString ().Substring (0, 2);

				string strSecondValue = nResult.ToString ().Substring (2, 2);

				return string.Format ("{0}.{1:##}{2}", strFirstValue, strSecondValue, unit [index]); 
			} else 
			{
				long nResult = cVal [index] * 1000 + cVal [index - 1]; 

				string strFirstValue = nResult.ToString ().Substring (0, 1);

				string strSecondValue = nResult.ToString ().Substring (1, 2);

				return string.Format ("{0}.{1:##}{2}", strFirstValue, strSecondValue, unit [index]); 
			}
		} 

		return strValue; 
	}

	public void GoldPlus(double _fValue)
    {
		
        m_dGetGold += _fValue;
		if (m_dGetGold <= 0)
			m_dGetGold = 0;
		goldText.text = ChangeMoney(m_dGetGold);
    }

	public void HonorPlus(double _fValue)
	{
		m_dGetHonor += _fValue;
		if (m_dGetHonor <= 0)
			m_dGetHonor = 0;
		honorText.text = ChangeMoney (m_dGetHonor);
	}

	public void RubyPlus(int _nValue)
	{
		m_nGetRuby += _nValue;
		if (m_nGetRuby <= 0)
			m_nGetRuby = 0;
		rubyText.text = m_nGetRuby.ToString ();
	}

	public void SetCurrentDays(int _Days)
	{
		playerDaysInfo.CurrentDaysText.text = string.Format ("{0}", _Days);
		GameManager.Instance.player.SetDay (_Days);
		if (_Days >= 100)
			playerDaysInfo.RefreshDayText ();
		else if (_Days >= 1000)
			playerDaysInfo.RefreshDayText ();
		
	}
		
	public void SetMaxDays(int _Days)
	{
		playerDaysInfo.MaxDaysText.text = string.Format ("{0}", _Days);
		GameManager.Instance.player.SetMaxDay (_Days);
	}

	public double GetFreePassGold()
	{
		int nCurDay = GameManager.Instance.player.GetDay ();
		//double asd = Mathf.Pow (1.09, (double)nCurDay - 1);
		//double gold = (250 * (double)Mathf.Pow (1.09f, nCurDay - 1)) * 10;
		return nCurDay;
	}


}
