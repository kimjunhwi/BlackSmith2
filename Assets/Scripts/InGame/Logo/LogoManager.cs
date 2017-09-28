using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using System.Text;
using UnityEngine.SocialPlatforms;

public class LogoManager : MonoBehaviour
{

    AsyncOperation ao;

    public Text loadingText;

    public bool bIsSuccessed = false;
	public bool bIsUnityEditorComplete = false;
	public bool bIsLoginComplete = false;
	public bool bIsCloundDataLoaded = false;
	private bool bIsLoginWindowOn = false;

	private bool bIsGoogleLoginComplete = false;
	private bool bIsGuestLoginComplete = false;

	public GameObject loginWindow;
	public LogoTextBlink logoBlink;

	public GameObject nickInputFieldWindow_Obj;

	private string strPlayerNick;

	// Use this for initialization
	void Start () 
	{
		strPlayerNick = "";
		#if UNITY_EDITOR
		StartCoroutine( GameManager.Instance.DataLoad());
		#elif UNITY_ANDROID
		//EnableGameSave
		// enables saving game progress.
		PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
		.EnableSavedGames()
		.Build();
		PlayGamesPlatform.InitializeInstance(config);
		//GoogleLogin Active
		PlayGamesPlatform.Activate();
		StartCoroutine( GameManager.Instance.DataLoad());
		#endif
	}

	// Update is called once per frame
	void Update () 
	{
		#if UNITY_EDITOR
        if(bIsSuccessed == true)
        {
			//Debug.Log("Successed");
			StartLoadScene();
			bIsSuccessed = false;
		}

		if(bIsUnityEditorComplete == true)
		{
			if (Input.GetMouseButtonDown (0)) 
			{
				ao.allowSceneActivation = true;
			}
		}

		#elif UNITY_ANDROID

		//Debug.Log("Android Build");


		if(bIsSuccessed == true)
		{
			bIsSuccessed = false;

			StartLoadScene();
		}

		if(bIsLoginComplete == true)
		{
			if (Input.GetMouseButtonDown (0)) 
			{
				ao.allowSceneActivation = true;
			}
		}

		#endif

	}
    private void StartLoadScene()
    {
        StartCoroutine (this.LoadScene());
	}

	IEnumerator LoadScene()
	{
		#if UNITY_EDITOR
		yield return new WaitForSeconds (0.3f);

		ao = SceneManager.LoadSceneAsync (2);
		ao.allowSceneActivation = false;

		while (true) 
		{
			//end Condition
			if(ao.progress == 0.9f && bIsUnityEditorComplete == true)
			{
				loadingText.text = "화면을 터치하세요";
				//bIsUnityEditorComplete = true;
				logoBlink.StartBlinkText();
				yield break;
			}
			
			//Skip Login
			if (GameManager.Instance.GetPlayer().changeStats.bIsGoogleLogin == true || 
				GameManager.Instance.GetPlayer().changeStats.bIsGusetLogin == true)
			{
				//Debug.Log ("Loading...");
				if(GameManager.Instance.GetPlayer().changeStats.bIsGoogleLogin == true && bIsGoogleLoginComplete==false )
				{
					bIsLoginWindowOn = true;
					LoginGoogle();
					continue;
				}

				if(GameManager.Instance.GetPlayer().changeStats.bIsGusetLogin == true && bIsGuestLoginComplete == false)
				{
					bIsLoginWindowOn = true;
					bIsGuestLoginComplete = true;
					bIsUnityEditorComplete = true;
					continue;
				}
				yield return null;
			}

			if(GameManager.Instance.GetPlayer().changeStats.bIsGoogleLogin == false && 
				GameManager.Instance.GetPlayer().changeStats.bIsGusetLogin == false)
			{
				if(bIsLoginWindowOn == false)
				{	
					LoginWindow_Active(true);	
					bIsLoginWindowOn = true;
				}

			}
			yield return null;
		}


		#elif UNITY_ANDROID
		yield return new WaitForSeconds (0.3f);

		ao = SceneManager.LoadSceneAsync (2);
		ao.allowSceneActivation = false;
	

		while (true) 
		{

		//end Condition
		if(ao.progress == 0.9f && bIsLoginComplete == true)
		{
			loadingText.text = "화면을 터치하세요";
			logoBlink.StartBlinkText();
			yield break;
		}

		//Skip Login
		if (GameManager.Instance.GetPlayer().changeStats.bIsGoogleLogin == true || 
		GameManager.Instance.GetPlayer().changeStats.bIsGusetLogin == true)
		{
			Debug.Log ("Loading...");
			if(GameManager.Instance.GetPlayer().changeStats.bIsGoogleLogin == true && bIsGoogleLoginComplete==false )
			{
				bIsLoginWindowOn = true;
				bIsGoogleLoginComplete = true;
				LoginGoogle();
				
				continue;
			}

			if(GameManager.Instance.GetPlayer().changeStats.bIsGusetLogin == true && bIsGuestLoginComplete == false)
			{
				bIsLoginWindowOn = true;
				bIsGuestLoginComplete = true;
				bIsLoginComplete = true;
				continue;
			}
			yield return null;
		}

		if(GameManager.Instance.GetPlayer().changeStats.bIsGoogleLogin == false && 
		GameManager.Instance.GetPlayer().changeStats.bIsGusetLogin == false)
		{
			if(bIsLoginWindowOn == false)
			{	
				LoginWindow_Active(true);	
				bIsLoginWindowOn = true;
			}

		}
	
			yield return null;
		}
		#endif
	}

	public void LoginWindow_Active(bool _isBool)
	{
		//Debug.Log ("WindowActive : " + _isBool);
		loginWindow.SetActive (_isBool);
	}

	public void LoginGoogle()
	{
		Social.localUser.Authenticate((bool success) =>
			{
				if (success)
				{

					//Debug.Log("You've successfully logged in");

					LoginWindow_Active(false);

					if(GameManager.Instance.GetPlayer().changeStats.strName == "player")
					{
						//Debug.Log("Google Login First Login Set Nick");
						bIsGoogleLoginComplete = true;
						//nickInputFieldWindow_Obj.SetActive(true);
						GameManager.Instance.GetPlayer().changeStats.bIsGoogleLogin = true;
						bIsLoginComplete = true;
					}
					else
					{
						#if UNITY_EDITOR
						//Debug.Log("Google Login Skip Login");
						bIsUnityEditorComplete = true;
						bIsGoogleLoginComplete = true;
						#elif UNITY_ANDROID
						Debug.Log("Google Login Skip Login");
						bIsLoginComplete = true;
						#endif
					}
				}
				else
				{
					//Debug.Log("Login failed for some reason");
					return;
				}
			});
		

	}

	public void LoginGuest()
	{

		LoginWindow_Active(false);


		#if UNITY_EDITOR

		//처음실행시 닉x 로그인후
		if(GameManager.Instance.GetPlayer().changeStats.strName == "player")
		{
			//Debug.Log("GuestLogin First Login Set Nick");
			nickInputFieldWindow_Obj.SetActive(true);

		}
		else
		{
			//Debug.Log("GuestLogin Nick Skip");
			bIsUnityEditorComplete = true;
			bIsGuestLoginComplete = true;
		}

		#elif UNITY_ANDROID
		//처음실행시 닉x 로그인후
		if(GameManager.Instance.GetPlayer().changeStats.strName == "player")
		{
			Debug.Log("GuestLogin First Login Set Nick");
			bIsLoginComplete = true;
			bIsGuestLoginComplete = true;
			GameManager.Instance.GetPlayer().changeStats.bIsGusetLogin = true;
			//nickInputFieldWindow_Obj.SetActive(true);
		}
	
		else
		{
			Debug.Log("GuestLogin Nick Skip");
			bIsLoginComplete = true;
			bIsGuestLoginComplete = true;
			GameManager.Instance.GetPlayer().changeStats.bIsGusetLogin = true;
		}
		#endif

	}

	public void GetPlayerNick()
	{
		InputField nickField =  nickInputFieldWindow_Obj.GetComponentInChildren<InputField> ();
		strPlayerNick = nickField.text;
	}

	public void SetPlayerNick()
	{
		GameManager.Instance.GetPlayer ().changeStats.strName = strPlayerNick;

		#if UNITY_EDITOR
		if(bIsGoogleLoginComplete == true)
		{
			//Debug.Log("Nick Google Login");
			bIsUnityEditorComplete = true;			//Loading Complete
			bIsGuestLoginComplete = false;
			GameManager.Instance.GetPlayer().changeStats.bIsGoogleLogin = true;
		}
		else
		{
			//Debug.Log("Guset Google Login");
			bIsUnityEditorComplete = true;			//Loading Complete
			bIsGuestLoginComplete = true;
			GameManager.Instance.GetPlayer().changeStats.bIsGusetLogin = true;
		}

		#elif UNITY_ANDROID
		if(bIsGoogleLoginComplete == true)
		{
			Debug.Log("Nick Google Login");
			bIsLoginComplete = true;			//Loading Complete
			bIsGuestLoginComplete = false;
			GameManager.Instance.GetPlayer().changeStats.bIsGoogleLogin = true;
		}
		else
		{
			Debug.Log("Guset Google Login");
			bIsLoginComplete = true;			//Loading Complete
			bIsGuestLoginComplete = true;
			GameManager.Instance.GetPlayer().changeStats.bIsGusetLogin = true;
		}
		#endif

	}

}
