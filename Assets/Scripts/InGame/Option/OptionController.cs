using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionController : MonoBehaviour {

	public GameObject optionPopUpWindow;
	public GameObject optionSoundSwitch_On;
	public GameObject optionSoundSwitch_Off;

	public GameObject optionBgmSwitch_On;
	public GameObject optionBgmSwitch_Off;

	public GameObject optionCloudSaveData_Obj;
	public GameObject optionCloudLoadData_Obj;



	public void Start()
	{
		optionPopUpWindow.SetActive (false);
		optionCloudSaveData_Obj.GetComponent<Button> ().onClick.AddListener (GameManager.Instance.SaveCloudData);
		optionCloudLoadData_Obj.GetComponent<Button> ().onClick.AddListener (GameManager.Instance.LoadCloudData);
	}

	public void OptionWindowActive()
	{
		SoundManager.instance.PlaySound (eSoundArray.ES_TouchSound_Menu);
		if (optionPopUpWindow.activeSelf != true)
			optionPopUpWindow.SetActive (true);
		else
			optionPopUpWindow.SetActive (false);
	}


	//사운드 OnOff 스위치(중간 버튼) 
	public void SoundSwitch()
	{
		SoundManager.instance.PlaySound (eSoundArray.ES_TouchSound_Menu);
		if (optionSoundSwitch_Off.activeSelf != true) 
		{
			optionSoundSwitch_Off.SetActive (true);
			optionSoundSwitch_On.SetActive (false);
			SoundManager.instance.MuteES ();
		}
		else
		{
			optionSoundSwitch_On.SetActive (true);
			optionSoundSwitch_Off.SetActive (false);
			SoundManager.instance.UnMuteES ();
		}
	}

	//사운드 On
	public void SoundSwitchOn()
	{
		SoundManager.instance.PlaySound (eSoundArray.ES_TouchSound_Menu);
		optionSoundSwitch_On.SetActive (true);
		optionSoundSwitch_Off.SetActive (false);
		SoundManager.instance.UnMuteES ();
	}

	//사운드 Off
	public void SoundSwitchFalse()
	{
		SoundManager.instance.PlaySound (eSoundArray.ES_TouchSound_Menu);
		SoundManager.instance.PlaySound (eSoundArray.ES_TouchSound_Menu);
		optionSoundSwitch_On.SetActive (false);
		optionSoundSwitch_Off.SetActive (true);
		SoundManager.instance.MuteES ();

	}
	//BGM OnOff 스위치(중간 버튼) 
	public void BgmSwitch()
	{
		SoundManager.instance.PlaySound (eSoundArray.ES_TouchSound_Menu);
		if (optionBgmSwitch_Off.activeSelf != true) 
		{
			optionBgmSwitch_Off.SetActive (true);
			optionBgmSwitch_On.SetActive (false);
			SoundManager.instance.MuteBGM ();
		}
		else
		{
			optionBgmSwitch_On.SetActive (true);
			optionBgmSwitch_Off.SetActive (false);
			SoundManager.instance.UnMuteBGM ();
		}
	}

	//BGM On
	public void BgmSwitchOn()
	{
		SoundManager.instance.PlaySound (eSoundArray.ES_TouchSound_Menu);
		optionBgmSwitch_On.SetActive (true);
		optionBgmSwitch_Off.SetActive (false);
		SoundManager.instance.UnMuteBGM ();
	}

	//BGM Off
	public void BgmSwitchFalse()
	{
		SoundManager.instance.PlaySound (eSoundArray.ES_TouchSound_Menu);
		optionBgmSwitch_Off.SetActive (true);
		optionBgmSwitch_On.SetActive (false);
		SoundManager.instance.MuteBGM ();
	}

}
