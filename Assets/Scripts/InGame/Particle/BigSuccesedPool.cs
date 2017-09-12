﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigSuccesedPool : SimpleObjectPool {

	public void Init()
	{
		// 실제 경로는  /Assets/Resources/ 하위입니다.
		strPrefabName = "Prefabs/FxPrefab/BigSuccessed";

		//게임오브젝트를 미리 할당해줌
		prefab = Resources.Load(strPrefabName) as GameObject;

		// 초기에 만들 오브젝트 수를 정합니다.
		nPoolSize = 20;
		// 오브젝트 풀의 게임 오브젝트 이름을 설정합니다.
		gameObject.name = "BigSuccessed";
		// 오브젝트를 미리 생성해둡니다.
		PreloadPool();
	}
}
