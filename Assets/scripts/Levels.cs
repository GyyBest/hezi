using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum TileTypes {
	Empty = 0,
	Crate = 1,
	CrateOnTarget = 2,
	Ground = 3,
	PlayerOnTarget = 4,
	PlayerStart = 5,
	Target = 6,
	Wall = 7
}

public static class Symbols {
	public static char[] symbols = {
		' ', '$', '*', '-', '+', '@', '.', '#'
	};
}

[Serializable]
public class Levels : MonoBehaviour {
	/// <summary>
	/// This List holds all the Levels
	/// </summary>
	public List<Level> levels;

	// those are the Tile Prefabs used for each Level
	public GameObject PlayerPrefab;
	public GameObject CratePrefab;
	public GameObject TargetPrefab;
	public GameObject WallPrefab;
	public GameObject GroundPrefab;

	// Editor Variables
	public Texture2D playerTexture;
	public Texture2D playerOnTargetTexture;
	public Texture2D crateTexture;
	public Texture2D crateOnTargetTexture;
	public Texture2D groundTexture;
	public Texture2D wallTexture;
	public Texture2D targetTexture;
	public Texture2D gridTexture;
}