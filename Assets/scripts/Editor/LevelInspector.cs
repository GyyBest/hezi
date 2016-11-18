using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEditor;

[Serializable]
[CustomEditor (typeof(Levels))]
public class LevelInspector : Editor {
	Levels targetLevels;
	GUISkin skin;

	bool foldoutTextures = false;
	bool foldoutPrefabs = false;
	bool foldoutLevels = false;

	TileTypes tileTypes = TileTypes.Empty;

	void Start () {

	}

	/// <summary>
	/// Setups a custom skin
	/// </summary>
	void SetupSkin () {
		if (skin == null)
			skin = ScriptableObject.CreateInstance (typeof(GUISkin)) as GUISkin;

		skin.button.border = new RectOffset (0, 0, 0, 0);
		skin.button.padding = new RectOffset (0, 0, 0, 0);
		skin.button.margin = new RectOffset (0, 0, 0, 0);
		skin.button.fixedHeight = 24;
		skin.button.fixedWidth = 24;
		skin.button.stretchHeight = true;
		skin.button.stretchWidth = true;
	}

	/// <summary>
	/// Instantiate the specified Game Object at Position x/y.
	/// </summary>
	/// <param name="gObject">GameObject.</param>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public GameObject Instantiate (GameObject gObject, int x, int y) {
		return GameObject.Instantiate (gObject, new Vector3 (x, y), Quaternion.identity) as GameObject;
	}

	public void ClearLevel () {
		// clear current Level
		foreach (GameObject o in GameObject.FindGameObjectsWithTag("Player"))
			DestroyImmediate (o);
		DestroyImmediate (GameObject.Find ("Crates"));
		DestroyImmediate (GameObject.Find ("Walls"));
		DestroyImmediate (GameObject.Find ("Grounds"));
		DestroyImmediate (GameObject.Find ("Targets"));
		
		// Create new Parent Objects
		GameObject go;
		go = new GameObject ("Walls");
		go.transform.position = new Vector3 ();
		go = new GameObject ("Crates");
		go.transform.position = new Vector3 ();
		go = new GameObject ("Grounds");
		go.transform.position = new Vector3 ();
		go = new GameObject ("Targets");
		go.transform.position = new Vector3 ();
	}

	public override void OnInspectorGUI () {
		targetLevels = (Levels)target;

		List<Level> levels = targetLevels.levels;

		SetupSkin ();

		EditorGUILayout.BeginVertical ();
		EditorGUILayout.Separator ();

		foldoutTextures = EditorGUILayout.Foldout (foldoutTextures, "Editor Textures");
		if (foldoutTextures) {
			// editor textures
			targetLevels.playerTexture = EditorGUILayout.ObjectField ("Player Texture", targetLevels.playerTexture, typeof(Texture2D), false) as Texture2D;
			targetLevels.playerOnTargetTexture = EditorGUILayout.ObjectField ("Player On Target Texture", targetLevels.playerOnTargetTexture, typeof(Texture2D), false) as Texture2D;
			targetLevels.crateTexture = EditorGUILayout.ObjectField ("Crate Texture", targetLevels.crateTexture, typeof(Texture2D), false) as Texture2D;
			targetLevels.crateOnTargetTexture = EditorGUILayout.ObjectField ("Crate On Target Texture", targetLevels.crateOnTargetTexture, typeof(Texture2D), false) as Texture2D;
			targetLevels.wallTexture = EditorGUILayout.ObjectField ("Wall Texture", targetLevels.wallTexture, typeof(Texture2D), false) as Texture2D;
			targetLevels.targetTexture = EditorGUILayout.ObjectField ("Target Texture", targetLevels.targetTexture, typeof(Texture2D), false) as Texture2D;
			targetLevels.gridTexture = EditorGUILayout.ObjectField ("Grid Texture", targetLevels.gridTexture, typeof(Texture2D), false) as Texture2D;
			targetLevels.groundTexture = EditorGUILayout.ObjectField ("Ground Texture", targetLevels.groundTexture, typeof(Texture2D), false) as Texture2D;
		}

		EditorGUILayout.Separator ();

		foldoutPrefabs = EditorGUILayout.Foldout (foldoutPrefabs, "Tile Prefabs");
		if (foldoutPrefabs) {
			// the 5 prefabs
			targetLevels.PlayerPrefab = EditorGUILayout.ObjectField ("Player Prefab", targetLevels.PlayerPrefab, typeof(GameObject), false) as GameObject;
			targetLevels.CratePrefab = EditorGUILayout.ObjectField ("Crate Prefab", targetLevels.CratePrefab, typeof(GameObject), false) as GameObject;
			targetLevels.TargetPrefab = EditorGUILayout.ObjectField ("Target Prefab", targetLevels.TargetPrefab, typeof(GameObject), false) as GameObject;
			targetLevels.WallPrefab = EditorGUILayout.ObjectField ("Wall Prefab", targetLevels.WallPrefab, typeof(GameObject), false) as GameObject;
			targetLevels.GroundPrefab = EditorGUILayout.ObjectField ("Ground Prefab", targetLevels.GroundPrefab, typeof(GameObject), false) as GameObject;
		}

		EditorGUILayout.Separator ();
		
		foldoutLevels = EditorGUILayout.Foldout (foldoutLevels, "Levels");
		if (foldoutLevels) {
			int i = 1;

			foreach (Level lvl in levels) {
				EditorGUI.indentLevel++;

				lvl.foldout = EditorGUILayout.Foldout (lvl.foldout, "Level " + i);
				if (lvl.foldout) {
					EditorGUILayout.Separator ();
					EditorGUILayout.BeginHorizontal ();

					// Remove the current Level from the Levels List
					bool removeButton = GUILayout.Button ("Remove", GUILayout.Width (80.0f));
					if (removeButton) {
						levels.Remove (lvl);
						EditorGUILayout.EndHorizontal ();
						break;
					}

					// Import a Level from a Text Document
					bool importButton = GUILayout.Button ("Import", GUILayout.Width (80.0f));
					if (importButton) {   
						string file = EditorUtility.OpenFilePanel ("Import Sokoban Level", "", "txt");

						if (file.Length > 0) {
							System.IO.StreamReader reader = new System.IO.StreamReader (file);

							lvl.SizeX = Convert.ToInt32 (reader.ReadLine ());
							lvl.SizeY = Convert.ToInt32 (reader.ReadLine ());

							// create and resize the Level Definition Array
							if (lvl.levelDef == null)
								lvl.levelDef = new string[0];
							Array.Resize (ref lvl.levelDef, lvl.SizeX * lvl.SizeY);

							for (int _y = 0; _y < lvl.SizeY; _y++) {
								string s = new string (' ', lvl.SizeX);

								s = s.ReplaceAt (0, reader.ReadLine ());

								for (int _x = 0; _x < lvl.SizeX; _x++) {
									int index = _x * lvl.SizeY + _y;
									lvl.levelDef [index] = s.Substring (_x, 1);
								}
							}

							reader.Close ();
						}
					}

					bool loadButton = GUILayout.Button ("Load Level", GUILayout.Width (80.0f));
					if (loadButton) {   
						ClearLevel ();
						GameObject go;

						// Load the current Level
						for (int _y = 0; _y < lvl.SizeY; _y++) {
							for (int _x = 0; _x < lvl.SizeX; _x++) {
								char s = lvl.levelDef [_x * lvl.SizeY + _y] [0];

								if (s == Symbols.symbols [(int)TileTypes.Crate]) {
									go = Instantiate (targetLevels.GroundPrefab, _x, -_y);
									go.transform.parent = GameObject.Find ("Grounds").transform;
									go = Instantiate (targetLevels.CratePrefab, _x, -_y);
									go.transform.parent = GameObject.Find ("Crates").transform;
								}
								else if (s == Symbols.symbols [(int)TileTypes.CrateOnTarget]) {
									go = Instantiate (targetLevels.TargetPrefab, _x, -_y);
									go.transform.parent = GameObject.Find ("Targets").transform;
									go = Instantiate (targetLevels.CratePrefab, _x, -_y);
									go.transform.parent = GameObject.Find ("Crates").transform;
								}
								else if (s == Symbols.symbols [(int)TileTypes.Ground]) {
									go = Instantiate (targetLevels.GroundPrefab, _x, -_y);
									go.transform.parent = GameObject.Find ("Grounds").transform;
								}
								else if (s == Symbols.symbols [(int)TileTypes.PlayerOnTarget]) {
									go = Instantiate (targetLevels.TargetPrefab, _x, -_y);
									go.transform.parent = GameObject.Find ("Targets").transform;

									if (GameObject.FindGameObjectWithTag ("Player") == null)
										go = Instantiate (targetLevels.PlayerPrefab, _x, -_y);
								}
								else if (s == Symbols.symbols [(int)TileTypes.PlayerStart]) {
									go = Instantiate (targetLevels.GroundPrefab, _x, -_y);
									go.transform.parent = GameObject.Find ("Grounds").transform;

									if (GameObject.FindGameObjectWithTag ("Player") == null)
										go = Instantiate (targetLevels.PlayerPrefab, _x, -_y);
								}
								else if (s == Symbols.symbols [(int)TileTypes.Target]) {
									go = Instantiate (targetLevels.TargetPrefab, _x, -_y);
									go.transform.parent = GameObject.Find ("Targets").transform;
								}
								else if (s == Symbols.symbols [(int)TileTypes.Wall]) {
									go = Instantiate (targetLevels.WallPrefab, _x, -_y);
									go.transform.parent = GameObject.Find ("Walls").transform;
								}
							}
						}
					}
					
					EditorGUILayout.EndHorizontal ();
					EditorGUILayout.Separator ();
					EditorGUILayout.BeginHorizontal ();

					// Size Fields
					EditorGUILayout.LabelField ("Size:", GUILayout.Width (80.0f));

					lvl.SizeX = EditorGUILayout.IntField (lvl.SizeX, GUILayout.Width (48.0f));
					lvl.SizeY = EditorGUILayout.IntField (lvl.SizeY, GUILayout.Width (48.0f));

					EditorGUILayout.EndHorizontal ();
					EditorGUILayout.Separator ();

					// Set and/or resize the Level Array
					if (lvl.levelDef == null)
						lvl.levelDef = new string[0];
					Array.Resize (ref lvl.levelDef, lvl.SizeX * lvl.SizeY);

					EditorGUILayout.BeginHorizontal ();

					tileTypes = (TileTypes)EditorGUILayout.EnumPopup ("Tile Type", tileTypes);

					EditorGUILayout.EndHorizontal ();

					EditorGUILayout.Separator ();

					// Create TextFields foreach Tile
					for (int _y = 0; _y < lvl.SizeY; _y++) {
						EditorGUILayout.BeginHorizontal ();
						for (int _x = 0; _x < lvl.SizeX; _x++) {
							int index = _x * lvl.SizeY + _y;
							//lvl.levelDef[index] = EditorGUILayout.TextField(lvl.levelDef[index], GUILayout.Width(24));

							char chr = ' ';
							try {
								chr = lvl.levelDef [index] [0];
							} catch {
							}

							int buttonState = SymbolToState (chr);

							Texture2D texture = null;

							switch (buttonState) {
							case 1:
								texture = targetLevels.crateTexture != null ? targetLevels.crateTexture : null;
								break;

							case 2:
								texture = targetLevels.crateOnTargetTexture != null ? targetLevels.crateOnTargetTexture : null;
								break;

							case 3:
								texture = targetLevels.groundTexture != null ? targetLevels.groundTexture : null;
								break;

							case 4:
								texture = targetLevels.playerOnTargetTexture != null ? targetLevels.playerOnTargetTexture : null;
								break;

							case 5:
								texture = targetLevels.playerTexture != null ? targetLevels.playerTexture : null;
								break;

							case 6:
								texture = targetLevels.targetTexture != null ? targetLevels.targetTexture : null;
								break;

							case 7:
								texture = targetLevels.wallTexture != null ? targetLevels.wallTexture : null;
								break;

							case 0:
								texture = targetLevels.gridTexture != null ? targetLevels.gridTexture : null;
								break;
							}

							if (GUILayout.Button (texture, skin.button)) {
								buttonState = (int)tileTypes;
							}

							lvl.levelDef [index] = StateToSymbol (buttonState).ToString ();
						}
						EditorGUILayout.EndHorizontal ();
					}
				}
				EditorGUI.indentLevel--;
				EditorUtility.SetDirty (lvl);

				i++;
			}

			EditorGUILayout.BeginHorizontal ();
			bool addButton = GUILayout.Button ("Add Level", GUILayout.Width (70.0f));
			if (addButton) {
				levels.Add (ScriptableObject.CreateInstance ("Level") as Level);
			}
			
			EditorGUILayout.EndHorizontal ();
		}

		EditorGUILayout.EndVertical ();  
		        

		EditorUtility.SetDirty (targetLevels);
	}

	int SymbolToState (char symbol) {
		int i = 0;
		foreach (char c in Symbols.symbols) {
			if (c == symbol)
				return i;

			i++;
		}

		return 0;
	}

	char StateToSymbol (int state) {
		return Symbols.symbols [state];
	}
}