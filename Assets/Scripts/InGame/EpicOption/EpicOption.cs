using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadOnlys;

public class EpicOption {

	public int nIndex = 0;
	public string strExplain = "";
	public Player cPlayerData;

	public virtual void Init(int _nDay,Player _player) { }

	public virtual string GetExplain() {return null;}

	public virtual int CheckOption(){ return (int)E_EPIC_INDEX.E_EPIC_FAIEL; }


}
