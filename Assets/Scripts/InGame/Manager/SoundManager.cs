using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Cooltime
{
	public int iId = 0;
	public float fTime = 0;
}

public enum eSound : int
{
	BGM_Main   = 101,
	BGM_BossBattle    = 102,
	TempratureExplosion = 201,
	WaterActiveSound = 202,
	TouchSoundWeapon00 = 203,
	TouchSoundWeapon01 = 204,
	TouchSoundWeapon02 = 205,
	TouchSound_Cri = 206,
	TouchSound_Miss = 207,
	TouchSound_Menu = 208,
	FixedSound_Success = 209,
	BlackSmithRankUp = 210,
	Fever= 211,
	FeverTouch1 = 212,
	FeverTouch2 = 213,
	BossSound_Success = 214,
	BossSound_Fail = 215,
	CraftSound_Finish = 216,
	CraftSound_GreatFinish = 217,
	BossIceTouch = 218,
	BossIceBreak = 219,
	BossIceFreeze = 220,
	BossSasinSkullTouch = 221,
	BossFireExplosion = 222,
	BossFireExtingu = 223,
	BossMusicPiano1 = 224,
	BossMusicPiano2 = 225,
	BossMusicDJScratch = 226,
	BossDragonAppear =227,
	BossDragonDisappear = 228,
	TemperatureSound = 999,
};

public enum eSoundArray 
{
	BGM_Main = 0,
	BGM_BossBattle,
	ES_TempratureExplosion,
	ES_WaterActiveSound,
	ES_TouchWeapon00,
	ES_TouchWeapon01,
	ES_TouchWeapon02,
	ES_TouchSound_Cri,
	ES_TouchSound_Miss,
	ES_TouchSound_Menu,
	ES_FixedSound_Success,
	ES_BlackSmithRankUp,
	ES_Fever,
	ES_FeverTouch1,
	ES_FeverTouch2,
	ES_BossSound_Success,
	ES_BossSound_Fail,
	ES_CraftSound_Finish,
	ES_CraftSound_GreatFinish,
	ES_BossIceTouch,
	ES_BossIceBreak,
	ES_BossIceFreeze,
	ES_BossSasinSkullTouch,
	ES_BossFireExplosion,
	ES_BossFireExtingu,
	ES_BossMusicPiano1,
	ES_BossMusicPiano2,
	ES_BossMusicDJScratch,
	ES_BossDragonAppear,
	ES_BossDragonDisappear,
	ES_BigSuccess
}

public class SoundManager : MonoBehaviour 
{				
	//GameObject kRoot = null;

	//public float fVolume = 1.0f; 		
	public float fVolume_bgm = 1.0f;
	public float fVolume_fx = 1.0f;

	public GameObject kBgm = null;

	//ArrayList SourceArray = new ArrayList();
	//ArrayList CloneArray = new ArrayList();

	//ArrayList CooltimeArray = new ArrayList();

	public List<GameObject> SoundArray = new List<GameObject>();
	//FeverSound Array 소리 중복 안되게
	public List<GameObject> FeverSoundArray_Obj = new List<GameObject>();
	int curFeverIndex= 0;
	//IceSound Array
	public List<GameObject> IceTouchArray_Obj = new List<GameObject>();
	int curIceTouchIndex = 0;
	//NormalTouch
	public List<GameObject> NormalTouch_Obj = new List<GameObject>();
	int curNormalTouchIndex = 0;
	//Critical Touch

	public List<GameObject> Critical_Obj = new List<GameObject> ();

	public SimpleObjectPool simpleSoundObjPool;

	private static SoundManager s_instance = null;

	public static SoundManager instance
	{
		get 
		{
			if (s_instance == null)
			{
				s_instance //= new CGameSnd();
				= FindObjectOfType(typeof(SoundManager)) as SoundManager;
			}
			return s_instance;
		}
	}

	void Awake () 
	{
		DontDestroyOnLoad(this);		
		//Debug.Log("CGameSnd Awake");		
	}


	//----------------------------------------------
	public void LoadSource()
	{
		AddSource((int)eSound.BGM_Main, "Sound_BGM_Main");
		AddSource ((int)eSound.BGM_BossBattle, "Sound_BGM_BossBattle");
		AddSource((int)eSound.TempratureExplosion, "Sound_ES_WeaponExplosion");
		AddSource((int)eSound.WaterActiveSound, "Sound_ES_WaterActive");
		AddSource((int)eSound.TouchSoundWeapon00, "Sound_ES_TouchWeapon00");
		AddSource((int)eSound.TouchSoundWeapon00, "Sound_ES_TouchWeapon01");
		AddSource((int)eSound.TouchSoundWeapon00, "Sound_ES_TouchWeapon02");
		AddSource((int)eSound.TouchSound_Cri, "Sound_ES_TouchCri");
		AddSource((int)eSound.TouchSound_Miss, "Sound_ES_TouchMiss");
		AddSource((int)eSound.TouchSound_Menu, "Sound_ES_TouchMenu");
		AddSource((int)eSound.FixedSound_Success, "Sound_ES_FixedSuccess");
		AddSource((int)eSound.BlackSmithRankUp, "Sound_ES_BlackSmithRankUp");
		AddSource((int)eSound.Fever, "Sound_ES_Fever");
		AddSource((int)eSound.FeverTouch1, "Sound_ES_FeverTouch1");
		AddSource((int)eSound.FeverTouch2, "Sound_ES_FeverTouch2");
		AddSource((int)eSound.BossSound_Success, "Sound_ES_BossSuccess");
		AddSource((int)eSound.BossSound_Fail, "Sound_ES_BossFail");
		AddSource((int)eSound.CraftSound_Finish, "Sound_ES_CraftFinish");
		AddSource((int)eSound.CraftSound_GreatFinish, "Sound_ES_CraftGreatSuccess");
		AddSource((int)eSound.BossIceTouch, "Sound_ES_BossIceTouch");
		AddSource((int)eSound.BossIceBreak, "Sound_ES_BossIceBreak");
		AddSource((int)eSound.BossIceFreeze, "Sound_ES_BossIceFreeze");
		AddSource((int)eSound.BossSasinSkullTouch, "Sound_ES_BossSasinSkullTouch");
		AddSource((int)eSound.BossFireExplosion, "Sound_ES_BossFireExplosion");
		AddSource((int)eSound.BossFireExtingu, "Sound_ES_BossFireExtingu");
		AddSource((int)eSound.BossMusicPiano1, "Sound_ES_BossMusicPiano1");
		AddSource((int)eSound.BossMusicPiano2, "Sound_ES_BossMusicPiano2");
		AddSource((int)eSound.BossMusicDJScratch, "Sound_ES_BossMusicDJScratch");
		AddSource((int)eSound.BossDragonAppear, "Sound_ES_BossDragonAppear");
		AddSource((int)eSound.BossDragonDisappear, "Sound_ES_BossDragonDisappear");
		AddSource((int)eSound.BossIceTouch, "Sound_ES_BossIceTouch");
		AddSource((int)eSound.BossIceTouch, "Sound_ES_BossIceTouch");
		AddSource((int)eSound.BossIceTouch, "Sound_ES_BossIceTouch");
		AddSource((int)eSound.BossIceTouch, "Sound_ES_BossIceTouch");
		AddSource((int)eSound.BossIceTouch, "Sound_ES_BossIceTouch");
		AddSource((int)eSound.TouchSoundWeapon00, "Sound_ES_TouchWeapon00");
		AddSource((int)eSound.TouchSoundWeapon00, "Sound_ES_TouchWeapon01");
		AddSource((int)eSound.TouchSoundWeapon00, "Sound_ES_TouchWeapon02");
		AddSource((int)eSound.FeverTouch1, "Sound_ES_FeverTouch1");
		AddSource((int)eSound.FeverTouch2, "Sound_ES_FeverTouch2");
		AddSource((int)eSound.FeverTouch1, "Sound_ES_FeverTouch1");
		AddSource((int)eSound.FeverTouch2, "Sound_ES_FeverTouch2");
		AddSource((int)eSound.FeverTouch1, "Sound_ES_FeverTouch1");
		AddSource((int)eSound.FeverTouch2, "Sound_ES_FeverTouch2");
		AddSource((int)eSound.FeverTouch1, "Sound_ES_FeverTouch1");
		AddSource((int)eSound.FeverTouch2, "Sound_ES_FeverTouch2");
		AddSource((int)eSound.BossIceTouch, "Sound_ES_BossIceTouch");
		AddSource((int)eSound.BossIceTouch, "Sound_ES_BossIceTouch");
		AddSource((int)eSound.BossIceTouch, "Sound_ES_BossIceTouch");
		AddSource((int)eSound.BossIceTouch, "Sound_ES_BossIceTouch");
		AddSource((int)eSound.BossIceTouch, "Sound_ES_BossIceTouch");
		AddSource((int)eSound.TouchSoundWeapon00, "Sound_ES_TouchWeapon00");
		AddSource((int)eSound.TouchSoundWeapon00, "Sound_ES_TouchWeapon01");
		AddSource((int)eSound.TouchSoundWeapon00, "Sound_ES_TouchWeapon02");
		AddSource((int)eSound.TouchSoundWeapon00, "Sound_ES_TouchWeapon00");
		AddSource((int)eSound.TouchSoundWeapon00, "Sound_ES_TouchWeapon01");
		AddSource((int)eSound.TouchSoundWeapon00, "Sound_ES_TouchWeapon02");


		FeverSoundArray_Obj.Add (SoundArray [13]);
		FeverSoundArray_Obj.Add (SoundArray [14]);
		FeverSoundArray_Obj.Add (SoundArray [38]);
		FeverSoundArray_Obj.Add (SoundArray [39]);
		FeverSoundArray_Obj.Add (SoundArray [40]);
		FeverSoundArray_Obj.Add (SoundArray [41]);
		FeverSoundArray_Obj.Add (SoundArray [42]);
		FeverSoundArray_Obj.Add (SoundArray [43]);
		FeverSoundArray_Obj.Add (SoundArray [44]);
		FeverSoundArray_Obj.Add (SoundArray [45]);
	
		IceTouchArray_Obj.Add (SoundArray [19]);
		IceTouchArray_Obj.Add (SoundArray [30]);
		IceTouchArray_Obj.Add (SoundArray [31]);
		IceTouchArray_Obj.Add (SoundArray [32]);
		IceTouchArray_Obj.Add (SoundArray [33]);
		IceTouchArray_Obj.Add (SoundArray [34]);
		IceTouchArray_Obj.Add (SoundArray [46]);
		IceTouchArray_Obj.Add (SoundArray [47]);
		IceTouchArray_Obj.Add (SoundArray [48]);
		IceTouchArray_Obj.Add (SoundArray [49]);
		IceTouchArray_Obj.Add (SoundArray [50]);

		//456 
		NormalTouch_Obj.Add (SoundArray [4]);
		NormalTouch_Obj.Add (SoundArray [5]);
		NormalTouch_Obj.Add (SoundArray [6]);
		NormalTouch_Obj.Add (SoundArray [35]);
		NormalTouch_Obj.Add (SoundArray [36]);
		NormalTouch_Obj.Add (SoundArray [37]);
		NormalTouch_Obj.Add (SoundArray [51]);
		NormalTouch_Obj.Add (SoundArray [52]);
		NormalTouch_Obj.Add (SoundArray [53]);
		NormalTouch_Obj.Add (SoundArray [54]);
		NormalTouch_Obj.Add (SoundArray [55]);
		NormalTouch_Obj.Add (SoundArray [56]);
	
	}   

	// Add Source ----------------------------------------------------------------
	public void AddSource(int _sound_index, string _sound_name  ="")
	{
		
		GameObject soundObj = simpleSoundObjPool.GetObject ();
		soundObj.transform.SetParent(gameObject.transform,false);
		soundObj.transform.position = new Vector3(0, 0, 0);
		soundObj.name = _sound_name;
		//if (soundObj.name == _sound_name)
		//	return;

		CGameSoundData kTableInfo_sound = GameManager.Instance.Get_TableInfo_sound((int)_sound_index);

			
		string szPrefab = "";
		szPrefab = kTableInfo_sound.strResource;

		AudioClip obj = (AudioClip)Resources.Load("Sound/" + szPrefab, typeof(AudioClip));
		soundObj.GetComponent<AudioSource>().clip = obj;			//Add AudioClip
		soundObj.GetComponent<AudioSource>().playOnAwake = false;	//Off playOnAwake
		if(kTableInfo_sound.nLoop == 1)
			soundObj.GetComponent<AudioSource>().loop = true;	
	

		// CSndInfo
		SoundInfo soundInfo = soundObj.GetComponent<SoundInfo> ();
		if( soundInfo == null )
			soundInfo = (SoundInfo)soundObj.AddComponent<SoundInfo>();

		soundInfo.iID 			= soundObj.GetInstanceID();			
		soundInfo.audiolength 	= soundObj.GetComponent<AudioSource>().clip.length;
		soundInfo.nType = kTableInfo_sound.nType;
		soundInfo._index     = (int)_sound_index;

		//Add List
		SoundArray.Add (soundObj);



		/*
		GameObject kGO = new GameObject();
		//kGO.tag = "SoundObject";	
		kGO.transform.parent = gameObject.transform;
		kGO.transform.position = new Vector3(0, 0, 0);

		string szPrefab = "";
		if (_sound_name == "") //TableInfo_sound ���� �ε���ȣ��.
		{
			kGO.name = "Snd_" + _sound_index;
			//GetSoundData
			CGameSoundData kTableInfo_sound = GameManager.Instance.Get_TableInfo_sound((int)_sound_index);
			if (kTableInfo_sound == null)
			{
				Debug.Log("ERROR : CGameSnd Get_TableInfo_sound: " + _sound_index); 
				return null;
			}
			szPrefab = kTableInfo_sound.strResource;
			//Debug.Log("sound AddSource: " + szPrefab ); 
		}
		else 
		{
			kGO.name = "Snd_" + _sound_index;
			szPrefab = _sound_name;
		}

		// AudioSource
		//if( kGO.audio == null) 
		kGO.AddComponent(typeof(AudioSource));                

		AudioClip	obj = (AudioClip)Resources.Load("sound/" + szPrefab, typeof(AudioClip));
		if (obj == null)
		{
			Debug.Log("ERROR: CGameSound AddSource Load Failed: " + szPrefab);
			Destroy(kGO);
			return null;
		}
		kGO.GetComponent<AudioSource>().clip = obj;
		kGO.GetComponent<AudioSource>().playOnAwake = false;		
		//kGO.audio.volume = 1.0f;

		// CSndInfo
		CSndInfo kInfo = (CSndInfo)kGO.GetComponent("CSndInfo");
		if( kInfo == null )
			kInfo = (CSndInfo)kGO.AddComponent<CSndInfo>();

		kInfo.iID 			= kGO.GetInstanceID();			
		kInfo.audiolength 	= kGO.GetComponent<AudioSource>().clip.length;
		kInfo._index     = (int)_sound_index;

		// Add List
		SourceArray.Add( kGO );

		return kGO;
		*/
	}
	//사운드 재생
	public void PlaySound(eSoundArray _index)
	{
		AudioSource aSource = SoundManager.instance.SoundArray [(int)_index].gameObject.GetComponent<AudioSource> ();
		aSource.Play ();

	}
	//사운드 끔
	public void StopSound(eSoundArray _index)
	{
		AudioSource aSource = SoundManager.instance.SoundArray [(int)_index].gameObject.GetComponent<AudioSource> ();
		if (_index == eSoundArray.BGM_BossBattle)
			aSource.volume = 0.8f;
		else
			aSource.volume = 1.0f;
		aSource.Stop ();
	}
	//BGM Change
	public void ChangeBGM(eSoundArray _StopIndex, eSoundArray _StartIndex)
	{
		SoundInfo soundInfo_Stop = SoundArray [(int)_StopIndex].gameObject.GetComponent<SoundInfo> ();
		SoundInfo soundInfo_Start = SoundArray [(int)_StartIndex].gameObject.GetComponent<SoundInfo> ();



		//1이면  BGM  _ 2이면 한번 소리 나는 사운드들
		if (soundInfo_Stop.nType == 1) 
		{
			AudioSource aSource = SoundArray [(int)_StopIndex].gameObject.GetComponent<AudioSource> ();

			aSource.Stop ();
		}

		if (soundInfo_Start.nType == 1) 
		{
			AudioSource sSource = SoundArray [(int)_StartIndex].gameObject.GetComponent<AudioSource> ();
			if (_StartIndex == eSoundArray.BGM_BossBattle)
				sSource.volume = 0.8f;
			else
				sSource.volume = 1.0f;
			sSource.Play ();
		}
	}

	//Mute BGMSound
	public void MuteBGM()
	{
		for (int i = 0; i < SoundArray.Count; i++) 
		{
			SoundInfo soundInfo_Stop = SoundArray [i].gameObject.GetComponent<SoundInfo> ();
			if (soundInfo_Stop.nType == 1)
			{
				AudioSource aSource = SoundArray [i].gameObject.GetComponent<AudioSource> ();
				aSource.mute = true;
			}
		}
	}

	//Mute ESound(Effect)
	public void MuteES()
	{
		for (int i = 0; i < SoundArray.Count; i++) 
		{
			SoundInfo soundInfo_Stop = SoundArray [i].gameObject.GetComponent<SoundInfo> ();
			if (soundInfo_Stop.nType == 2)
			{
				AudioSource aSource = SoundArray [i].gameObject.GetComponent<AudioSource> ();
				aSource.mute = true;
			}
		}
	}

	//Mute BGMSound
	public void UnMuteBGM()
	{
		for (int i = 0; i < SoundArray.Count; i++) 
		{
			SoundInfo soundInfo_Stop = SoundArray [i].gameObject.GetComponent<SoundInfo> ();
			if (soundInfo_Stop.nType == 1)
			{
				AudioSource aSource = SoundArray [i].gameObject.GetComponent<AudioSource> ();
				aSource.mute = false;
			}
		}
	}

	//Mute ESound(Effect)
	public void UnMuteES()
	{
		for (int i = 0; i < SoundArray.Count; i++) 
		{
			SoundInfo soundInfo_Stop = SoundArray [i].gameObject.GetComponent<SoundInfo> ();
			if (soundInfo_Stop.nType == 2)
			{
				AudioSource aSource = SoundArray [i].gameObject.GetComponent<AudioSource> ();
				aSource.mute = false;
			}
		}
	}






	/*
	GameObject GetSource(int _index)
	{
		foreach ( GameObject kGO in SourceArray )			
		{
			if (kGO == null) continue;
			CSndInfo kObject = (CSndInfo)kGO.GetComponent("CSndInfo");			
			if( kObject._index == _index )
			{
				return kGO;
			}
		}	
		return null;
	}

	int GetID( GameObject kGO)
	{
		CSndInfo kInfo = (CSndInfo)kGO.GetComponent("CSndInfo");
		return kInfo.iID;
	}	

	// Add Fx Clone	
	GameObject AddClone(int _index, Vector3 pos )
	{
		GameObject kGO = GetSource( _index );
		if( kGO == null)
		{			
			//kGO = AddSource( _index );
		}
		if( kGO )
		{
			//kGO.audio.PlayOneShot( kGO.audio.clip );

			GameObject kGOClone = (GameObject)Instantiate( kGO );
			kGOClone.transform.position = pos;

			CSndInfo kInfo = (CSndInfo)kGOClone.GetComponent("CSndInfo");
			kInfo.iID 		= kGOClone.GetInstanceID();
			kInfo._index = _index;
			kInfo.bRemove 	= false;

			CloneArray.Add( kGOClone );

			return kGOClone;
		}
		return null;
	}

	public void RemoveClone( GameObject kGO )
	{	
		CloneArray.Remove( kGO );
		Destroy (kGO); 		
	}

	public void RemoveAllClone()
	{
		foreach ( Object obj in CloneArray )
			Destroy (obj); 
		CloneArray.Clear();
	}

	public void RemoveAll()
	{
		//GameObject[] kGOs = GameObject.FindGameObjectsWithTag ("SoundObject");
		//foreach ( Object obj in kGOs )
		//	Destroy (obj); 	

		foreach ( Object obj in CloneArray )
		{
			if(obj) Destroy (obj); 
		}
		CloneArray.Clear();

		foreach ( Object obj in SourceArray )
		{
			if(obj) Destroy (obj); 
		}
		SourceArray.Clear();

	}	

	// Play BGM ----------------------------------------------------------------

	public void PlayBGM(eSound _index )
	{
		if( kBgm != null)
		{
			CSndInfo kInfo = (CSndInfo)kBgm.GetComponent("CSndInfo");
			//print (kInfo._index + " " + _index);
			if( kInfo._index == (int)_index )
				return;

			StopBGM();
		}

		kBgm = PlaySound( (int)_index, Vector3.zero, true, fVolume_bgm );
		kBgm.transform.SetParent(gameObject.transform);
		//print("PlayBGM " + _index);
	}

	public void StopBGM()
	{
		SoundManager.instance.RemoveClone( kBgm );
		//Destroy ( kBgm );
	}
	*/

	// PlaySound  ---------------------------------------------------------------



	/*
	public GameObject PlaySound(eSound _index )
	{
		if(_index == 0)return null;
		return PlaySound( (int)_index, Vector3.zero, false, fVolume_fx );
		GameObject kGO = AddClone( _index, Vector3.zero );
		if(kGO == null)	
			return null;
	}
	*/

	/*
	public GameObject PlaySound(int _index)
	{
		if (_index == 0) return null;
		return PlaySound(_index, Vector3.zero, false, fVolume_fx);
	}

	public GameObject PlaySound(int _index, Vector3 pos, bool bLoop, float _volume )
	{
		if( ! AddCooltime( _index ) ) 
			return null;	

		GameObject kGO = AddClone( _index, pos );
		if(kGO == null)	
			return null;

		kGO.transform.parent = gameObject.transform; // AudioListener
		kGO.transform.position = new Vector3(0,0,-10);		

		CSndInfo kInfo = (CSndInfo)kGO.GetComponent("CSndInfo");

		if(bLoop)
		{			
			kGO.GetComponent<AudioSource>().loop = true;
		}
		else
		{
			kGO.GetComponent<AudioSource>().loop = false;
			kInfo.DeathTime = kInfo.audiolength;
			kInfo.bRemove = true;
		}

		kGO.GetComponent<AudioSource>().playOnAwake = true;
		kGO.GetComponent<AudioSource>().volume = _volume; 		//print("sound: " + _index + "  _volume: " + _volume);
		kGO.GetComponent<AudioSource>().Play();
		//kGO.audio.PlayOneShot( kGO.audio.clip );

		return kGO;
	}

	public void StopSound(GameObject go)
	{
		SoundManager.instance.RemoveClone(go);
	}


	public void SetVolume_bgm()
	{
		if( kBgm != null )	{
			kBgm.GetComponent<AudioSource>().volume = fVolume_bgm;
			//kBgm.audio.Play();
		}
	}

	//--------------------------------------------------------------------------
	bool AddCooltime(int _index)
	{
		bool bActive = false;

		Cooltime kCool = GetCooltime(_index);
		if (kCool == null)
		{
			kCool = new Cooltime();
			kCool.iId = _index;
			kCool.fTime = Time.time;

			CooltimeArray.Add(kCool);

			bActive = true;
		}
		else
		{
			if (Time.time > kCool.fTime + 0.2f) //time.
				bActive = true;

			kCool.fTime = Time.time;
		}

		return bActive;
	}

	Cooltime GetCooltime(int _index)
	{
		foreach (Cooltime kObject in CooltimeArray)
		{
			if (kObject == null) continue;

			if (kObject.iId == _index)
			{
				return kObject;
			}
		}
		return null;
	}
	*/
	public void SetSoundObjPool(SimpleObjectPool _SoundObjPool)
	{
		simpleSoundObjPool = _SoundObjPool;
	}

	public void PlayTouchNormalWeapon()
	{

		AudioSource aSource = SoundManager.instance.NormalTouch_Obj [curNormalTouchIndex].gameObject.GetComponent<AudioSource> ();

		//사운드가 실행중이면 다른 인덱스를 가져온다
		if (aSource.isPlaying == true) {
			curNormalTouchIndex++;
			if (curNormalTouchIndex >= 11)
				curNormalTouchIndex = 0;
			aSource = SoundManager.instance.NormalTouch_Obj [curNormalTouchIndex].gameObject.GetComponent<AudioSource> ();
			aSource.Play ();
			return;

		}
		else 
		{
			aSource.Play ();
		}
	}

	public void StartPlayTouchFeverSound()
	{
		

		AudioSource aSource = SoundManager.instance.FeverSoundArray_Obj [curFeverIndex].gameObject.GetComponent<AudioSource> ();

		//사운드가 실행중이면 다른 인덱스를 가져온다
		if (aSource.isPlaying == true) {
			curFeverIndex++;
			if (curFeverIndex >= 10)
				curFeverIndex = 0;
			aSource = SoundManager.instance.FeverSoundArray_Obj [curFeverIndex].gameObject.GetComponent<AudioSource> ();
			aSource.Play ();
			return;

		}
		else 
		{
			aSource.Play ();
		}
	}

	public void StartPlayIceTouchSound()
	{
		int randomIndex = 0;

		AudioSource aSource = SoundManager.instance.IceTouchArray_Obj [curIceTouchIndex].gameObject.GetComponent<AudioSource> ();

		//사운드가 실행중이면 다른 인덱스를 가져온다
		if (aSource.isPlaying == true)
		{
			curIceTouchIndex++;
			if (curIceTouchIndex >= 11)
				curIceTouchIndex = 0;
			aSource = SoundManager.instance.IceTouchArray_Obj [curIceTouchIndex].gameObject.GetComponent<AudioSource> ();
			aSource.Play ();
			return;

		}
		else 
		{
			aSource.Play ();
		}
	}

	public IEnumerator PlayIceTouchSound()
	{
		yield return null;
	}

	public void PlayTouchMusicNoteSound()
	{
		int randomIndex = Random.Range ((int)eSoundArray.ES_BossMusicPiano1, (int)eSoundArray.ES_BossMusicPiano2);
		AudioSource aSource = SoundManager.instance.SoundArray [randomIndex].gameObject.GetComponent<AudioSource> ();
		aSource.Play ();
	}



}

