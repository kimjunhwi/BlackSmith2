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

	public GameObject loginWindow;
	public LogoTextBlink logoBlink;



	// Use this for initialization
	void Start () 
	{
		#if UNITY_EDITOR
		StartCoroutine( GameManager.Instance.DataLoad());
		#elif UNITY_ANDROID
		//EnableGameSave
		// enables saving game progress.
		//PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
		//.EnableSavedGames()
		//.Build();
		//PlayGamesPlatform.InitializeInstance(config);
		//GoogleLogin Active
		//PlayGamesPlatform.Activate();

		StartCoroutine( GameManager.Instance.DataLoad());
		#endif
	}


	
	// Update is called once per frame
	void Update () 
	{
		#if UNITY_EDITOR
        if(bIsSuccessed == true)
        {
			Debug.Log("Successed");
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

		ao = SceneManager.LoadSceneAsync (1);
		ao.allowSceneActivation = false;

		while (!ao.isDone) 
		{
			if (ao.progress == 0.9f)
			{
				Debug.Log ("Ready To Start!!");
				loadingText.text = "화면을 터치하세요";
				bIsUnityEditorComplete = true;
				logoBlink.StartBlinkText();
				yield break;
			}
			yield return null;
		}
		#elif UNITY_ANDROID
		yield return new WaitForSeconds (0.3f);

		ao = SceneManager.LoadSceneAsync (1);
		ao.allowSceneActivation = false;

		while (!ao.isDone) 
		{
			if (ao.progress == 0.9f)
			{
				//Debug.Log ("Ready To Start!!");
				loadingText.text = "화면을 터치하세요";
				if(Social.localUser.authenticated == true)
				{
					bIsLoginComplete = true;
					logoBlink.StartBlinkText();
					yield break;
				}
				else
				{
					LoginWindow_Active(true);	
					yield break;
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
					Debug.Log("You've successfully logged in");
					bIsLoginComplete = true;
					LoginWindow_Active(false);
					logoBlink.StartBlinkText();
					//GameManager.Instance.LoadData();
					//yield return new WaitForSeconds (1.0f);
					//ao.allowSceneActivation = true;

				}
				else
				{
					Debug.Log("Login failed for some reason");
					return;
				}
			});
		

	}

	public void LoginGuest()
	{
		bIsLoginComplete = true;
		LoginWindow_Active(false);
		logoBlink.StartBlinkText();
	}


}
