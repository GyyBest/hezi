using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class Level : ScriptableObject {
	[SerializeField]
	int sizeX;
	[SerializeField]
	int sizeY;

	public int SizeX {
		get { 
			return sizeX; 
		}
		set {
			if (value < 4)
				sizeX = 4;
			else
				sizeX = value;
		}
	}

	public int SizeY {
		get { 
			return sizeY; 
		}
		set {
			if (value < 4)
				sizeY = 4;
			else
				sizeY = value;
		}
	}

	[SerializeField]
	public string[] levelDef;

	[SerializeField]
	public bool foldout;
}