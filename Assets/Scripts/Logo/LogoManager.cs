using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;

public class LogoManager : MonoBehaviour {

    AsyncOperation ao;

    public Text loadingText;

    public bool bIsSuccessed = false;

	// Use this for initialization
	void Start () 
	{
		//EnableGameSave
		PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
			// enables saving game progress.
			.EnableSavedGames()
			.Build();
		PlayGamesPlatform.InitializeInstance(config);
		//GoogleLogin Active
		PlayGamesPlatform.Activate();

		StartCoroutine( GameManager.Instance.DataLoad());
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
		#elif UNITY_ANDROID

		Debug.Log("Android Build");

		if(bIsSuccessed == true)
		{
			bIsSuccessed = false;

			Social.localUser.Authenticate((bool success) =>
			{
				if (success)
				{
					StartLoadScene();
					Debug.Log("You've successfully logged in");
				}
				else
				{
					Debug.Log("Login failed for some reason");
				}
			});


		}
		#endif

	}
    private void StartLoadScene()
    {
        StartCoroutine (this.LoadScene());
	}

	IEnumerator LoadScene(){
		Debug.Log ("StartLoadScene");
		yield return new WaitForSeconds (0.3f);

		ao = SceneManager.LoadSceneAsync (1);
		ao.allowSceneActivation = false;

		while (!ao.isDone) 
		{
			Debug.Log ("While In!!");
			if (ao.progress == 0.9f)
			{

				Debug.Log ("Ready To Start!!");
				loadingText.text = "Press Button";
				if (Input.GetMouseButtonDown (0)) 
				{
					yield return new WaitForSeconds (1.0f);
					ao.allowSceneActivation = true;
				}
			}
			yield return null;
		}
	}
}
