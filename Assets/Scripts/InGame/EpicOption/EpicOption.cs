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

	public virtual bool CheckOption(){ return false; }

	public virtual void Relive(){ }
}
