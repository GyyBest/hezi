using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent (typeof(Camera))]
public class CameraPosition : MonoBehaviour {
	void Start () {}

	void Update () {
		// the following code could be called only once at the end of Level Loading for better performance
		GameObject[] objs = GameObject.FindGameObjectsWithTag ("Wall");

		float minX = 0, maxX = 0;
		float minY = 0, maxY = 0;

		foreach (GameObject o in objs) {
			Vector2 pos = o.transform.position;
			if (pos.x < minX)
				minX = pos.x;
			if (pos.x > maxX)
				maxX = pos.x;

			if (pos.y < minY)
				minY = pos.y;
			if (pos.y > maxY)
				maxY = pos.y;
		}

		Vector2 point = LineIntersectionPoint (
			                new Vector2 (minX, maxY),
			                new Vector2 (maxX, minY),
			                new Vector2 (minX, minY),
			                new Vector2 (maxX, maxY)
		                );

//		Debug.Log (point);

		Camera.main.transform.position = new Vector3 (point.x, point.y, Camera.main.transform.position.z);
		Camera.main.transform.LookAt (new Vector3 (point.x, point.y, 0));

//		float distanceY = Vector2.Distance(
//			new Vector2(minY, 0),                     
//			new Vector2(maxY, 0));
//
//		float distanceX = Vector2.Distance(
//			new Vector2(minX, 0),                     
//			new Vector2(maxX, 0));
//
//		if(distanceY > distanceX)
//		Camera.main.orthographicSize = 1 + (distanceY + 1) / 2;
	}

	Vector2 LineIntersectionPoint (Vector2 ps1, Vector2 pe1, Vector2 ps2, Vector2 pe2) {
		// Get A,B,C of first line - points : ps1 to pe1
		float A1 = pe1.y - ps1.y;
		float B1 = ps1.x - pe1.x;
		float C1 = A1 * ps1.x + B1 * ps1.y;
			
		// Get A,B,C of second line - points : ps2 to pe2
		float A2 = pe2.y - ps2.y;
		float B2 = ps2.x - pe2.x;
		float C2 = A2 * ps2.x + B2 * ps2.y;
			
		// Get delta and check if the lines are parallel
		float delta = A1 * B2 - A2 * B1;
		if (delta == 0)
			return new Vector2 ();
			
		// now return the Vector2 intersection point
		return new Vector2 (
			(B2 * C1 - B1 * C2) / delta,
			(A1 * C2 - A2 * C1) / delta
		);
	}
}