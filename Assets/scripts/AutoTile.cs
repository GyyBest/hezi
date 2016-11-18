using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public static class Extension {
	public static string ReplaceAt (this string str, int index, string replace) {
		return str.Remove (index, Math.Min (replace.Length, str.Length - index))
			.Insert (index, replace);
	}
}

[ExecuteInEditMode]
public class AutoTile : MonoBehaviour {
	public Sprite[] spriteSheet;

	int SpriteByID (string id) {
		for (int i = 0; i < tileFrames.Length; i++) {
			if (tileFrames [i] == id) {
				return i;
			}
		}

		return 0;
	}

	void Start () {
		SetTileFrame ();
	}

	/// <summary>
	/// unique identifier for each possible frame
	/// 0 = none connection
	/// 1 = connection
	/// 2 = edge
	/// </summary>
	string[] tileFrames = {
		"000000000", // 1
		"102100102", // 2
		"111000202", // 3
		"111100102", // 4
		"201001201", // 5
		"101101101", // 6
		"111001201", // 7
		"111101101", // 8
		"202000111", // 9
		"102100111", // 10
		"111000111", // 11
		"111100111", // 12
		"201001111", // 13
		"101101111", // 14
		"111001111", // 15
		"111101111", // 16
		"102000202", // 17
		"201000202", // 18
		"101000202", // 19
		"202000201", // 20
		"102000201", // 21
		"201000201", // 22
		"101000201", // 23
		"202000102", // 24
		"102000102", // 25
		"201000102", // 26
		"101000102", // 27
		"202000101", // 28
		"102000101", // 29
		"201000101", // 30
		"101000101", // 31
		"111100101", // 32
		"111001101", // 33
		"101001111", // 34
		"101100111", // 35
		"101100102", // 36
		"101001201", // 37
		"102100101", // 38
		"201001101", // 39
		"111000102", // 40
		"111000201", // 41
		"102000111", // 42
		"201000111", // 43
		"", // 44
		"", // 45
		"101100101", // 46
		"101001101", // 47
		"111000101", // 48
		"101000111", // 49
	};

	/// <summary>
	/// Sets the Tile's Sprite/Frame depending on the surrounding Tiles.
	/// </summary>
	void SetTileFrame () {
		// default frame
		// ###
		// # #
		// ###
		string frame = "111101111";

		RaycastHit hitTop, hitRight, hitBottom, hitLeft;
		
		// test the 4 directions
		Physics.Raycast (transform.position + new Vector3 (0, 0.6f, 1),
			Vector3.back, out hitTop, 2f);
		Physics.Raycast (transform.position + new Vector3 (0, -0.6f, 1),
			Vector3.back, out hitBottom, 2f);
		Physics.Raycast (transform.position + new Vector3 (0.6f, 0, 1),
			Vector3.back, out hitRight, 2f);
		Physics.Raycast (transform.position + new Vector3 (-0.6f, 0, 1),
			Vector3.back, out hitLeft, 2f);
		
		if (hitTop.collider != null && hitTop.collider.tag == this.tag) {
			frame = frame.ReplaceAt (1, "0");
		}
		if (hitRight.collider != null && hitRight.collider.tag == this.tag) {
			frame = frame.ReplaceAt (5, "0");
		}
		if (hitBottom.collider != null && hitBottom.collider.tag == this.tag) {
			frame = frame.ReplaceAt (7, "0");
		}
		if (hitLeft.collider != null && hitLeft.collider.tag == this.tag) {
			frame = frame.ReplaceAt (3, "0");
		}

		// now the 4 corner test
		RaycastHit hitTopLeft, hitTopRight, hitBottomRight, hitBottomLeft;

		Physics.Raycast (transform.position + new Vector3 (-1, 1, 1),
			Vector3.back, out hitTopLeft, 2f);
		Physics.Raycast (transform.position + new Vector3 (1, 1, 1),
			Vector3.back, out hitTopRight, 2f);
		Physics.Raycast (transform.position + new Vector3 (1, -1, 1),
			Vector3.back, out hitBottomRight, 2f);
		Physics.Raycast (transform.position + new Vector3 (-1, -1, 1),
			Vector3.back, out hitBottomLeft, 2f);

		// top left
		if ((hitTopLeft.collider != null && hitTopLeft.collider.tag == this.tag) &&
		    (frame [3] == '0') && (frame [1] == '0')) {
			frame = frame.ReplaceAt (0, "2");
		}
		// top right
		if ((hitTopRight.collider != null && hitTopRight.collider.tag == this.tag) &&
		    (frame [5] == '0') && (frame [1] == '0')) {
			frame = frame.ReplaceAt (2, "2");
		}
		// bottom right
		if ((hitBottomRight.collider != null && hitBottomRight.collider.tag == this.tag) &&
		    (frame [7] == '0') && (frame [5] == '0')) {
			frame = frame.ReplaceAt (8, "2");
		}
		// bottom left
		if ((hitBottomLeft.collider != null && hitBottomLeft.collider.tag == this.tag) &&
		    (frame [7] == '0') && (frame [3] == '0')) {
			frame = frame.ReplaceAt (6, "2");
		}

		// set sprite to correct frame
		this.GetComponent<SpriteRenderer> ().sprite = spriteSheet [SpriteByID (frame)];
	}

	void Update () {
		if (Application.platform == RuntimePlatform.WindowsEditor)
			SetTileFrame ();
	}
}