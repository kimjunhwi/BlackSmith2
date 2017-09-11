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

	string[] unit = new string[]{ "G", "K", "M", "B", "T", "aa", "bb", "cc", "dd", "ee" }; 

    private void Awake()
    {
        scireInstance = this;

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
		SuccessedGuestCount.text = string.Format ("Successed {0} / 10", _nValue);
	}


	//값을 수치로 표기하기 위한 함수 
	string ChangeMoney(double _dValue)
	{ 
		if (_dValue == 0)
			return "0";

		int[] cVal = new int[10]; 

		int index = 0; 

		string strValue =  string.Format ("{0:####}", _dValue);

		while (true) { 
			string last4 = ""; 
			if (strValue.Length >= 4) { 

				last4 = strValue.Substring (strValue.Length - 4); 

				int intLast4 = int.Parse (last4); 

				cVal [index] = intLast4 % 1000; 

				strValue = strValue.Remove (strValue.Length - 3); 
			} else { 
				cVal [index] = int.Parse (strValue); 
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
				int nResult = cVal [index] * 1000 + cVal [index - 1]; 

				string strFirstValue = nResult.ToString ().Substring (0, 3);

				string strSecondValue = nResult.ToString ().Substring (3, 1);

				return string.Format ("{0}.{1:##}{2}", strFirstValue, strSecondValue, unit [index]); 
			} 
			else 
			{
				int nResult = cVal [index] * 1000 + cVal [index - 1]; 

				string strFirstValue = nResult.ToString ().Substring (0, 2);

				string strSecondValue = nResult.ToString ().Substring (2, 2);

				return string.Format ("{0}.{1:##}{2}", strFirstValue, strSecondValue, unit [index]); 

			}
		} 

		return strValue; 
	}

	public void GoldPlus(double _fValue)
    {
        m_dGetGold += _fValue;

		goldText.text = ChangeMoney(m_dGetGold);
    }

	public void HonorPlus(double _fValue)
	{
		m_dGetHonor += _fValue;

		honorText.text = ChangeMoney (m_dGetHonor);
	}

	public void RubyPlus(int _nValue)
	{
		m_nGetRuby += _nValue;
		rubyText.text = m_nGetRuby.ToString ();
	}

	public void SetCurrentDays(int _Days)
	{
		playerDaysInfo.CurrentDaysText.text = string.Format ("{0}", _Days);
		GameManager.Instance.player.SetDay (_Days);
	}




	public void SetMaxDays(int _Days)
	{
		playerDaysInfo.MaxDaysText.text = string.Format ("{0}", _Days);
		GameManager.Instance.player.SetMaxDay (_Days);
	}


}
