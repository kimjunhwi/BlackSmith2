using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerDaysInfo : MonoBehaviour , IPointerDownHandler
{
	private Player player;

	public GameObject CurrentDaysPanel;
	public GameObject MaxDaysPanel;

	public Text CurrentDaysText;
	public Text MaxDaysText;


	void Awake()
	{
		if (player == null)
			player = GameManager.Instance.player;


		if (CurrentDaysText.text == "")
			CurrentDaysText.text = player.GetDay ().ToString();

	}
	public void RefreshDayText()
	{
		if (GameManager.Instance.GetPlayer ().GetMaxDay() < 100)
			MaxDaysText.fontSize = 30;
		else if(GameManager.Instance.GetPlayer ().GetMaxDay() < 1000)
			MaxDaysText.fontSize = 25;
		else
			MaxDaysText.fontSize = 20;


		if (GameManager.Instance.GetPlayer ().GetDay() < 100)
			CurrentDaysText.fontSize = 30;
		else if(GameManager.Instance.GetPlayer ().GetDay() < 1000)
			CurrentDaysText.fontSize = 25;
		else
			CurrentDaysText.fontSize = 20;
	}

	public void OnPointerDown (PointerEventData eventData)
	{
		GameObject getInfoGameObject = eventData.pointerEnter;
		//Debug.Log ("Touched : " + getInfoGameObject.name);
		if (getInfoGameObject.gameObject == null)
			return;

		SoundManager.instance.PlaySound (eSoundArray.ES_TouchSound_Menu);

		//MaxDay
		if (getInfoGameObject.gameObject.name == "CurrentDaysText" || 
			getInfoGameObject.gameObject.name == "DayText") 
		{
			Debug.Log ("Touched : " + getInfoGameObject.name);

			if (player.GetMaxDay () < 100)
				MaxDaysText.fontSize = 30;
			else if(player.GetMaxDay () < 1000)
				MaxDaysText.fontSize = 25;
			else
				MaxDaysText.fontSize = 20;


			CurrentDaysText.text = "";
			MaxDaysText.text = player.GetMaxDay ().ToString ();


			CurrentDaysPanel.SetActive (false);
			MaxDaysPanel.SetActive (true);


		}

		//CurDay
		if (getInfoGameObject.gameObject.name == "MaxDaysText"|| 
			getInfoGameObject.gameObject.name == "DayText") 
		{
			Debug.Log ("Touched : " + getInfoGameObject.name);

			if (player.GetDay () < 100)
				CurrentDaysText.fontSize = 30;
			else if(player.GetDay () < 1000)
				CurrentDaysText.fontSize = 25;
			else
				CurrentDaysText.fontSize = 20;


			CurrentDaysText.text = player.GetDay ().ToString ();
			MaxDaysText.text = "";

			CurrentDaysPanel.SetActive (true);
			MaxDaysPanel.SetActive (false);

		}
	}
}
