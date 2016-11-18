using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.SceneManagement;//use the method SceneManager.LoadScene

public class PlayerController : MonoBehaviour {
    public float moveSpeed = 5f;

	[HideInInspector]
	public float gridSize = 1f;

	[HideInInspector]
	public Vector2 input;

	[HideInInspector]
	public bool isMoving = false;

    [HideInInspector]
    public Vector3 direction = Vector3.zero;

    [HideInInspector]
    public Vector3 lastMove = Vector3.zero;

    [HideInInspector]
    public bool onSwitch = false;

    private GameObject portal1;
    private GameObject portal2;

    private Vector3 startPosition;
	private Vector3 endPosition;
	private float t;
	private float factor;

	public string tagCrate = "Crate";
	public string tagWall = "Wall";
	public string tagSwitch = "Switch";

    void Start() {
        portal1 = (GameObject) Helpers.Find("Portal1", typeof(GameObject));
        portal2 = (GameObject) Helpers.Find("Portal2", typeof(GameObject));
    }

    /// <summary>
    /// Determines whether this instance can move the specified dir.
    /// </summary>
    /// <returns><c>true</c> if this instance can move the specified dir; otherwise, <c>false</c>.</returns>
    /// <param name="dir">Dir.</param>
    bool CanMove (Vector3 direction) {
		RaycastHit hit;

        // nothing is hit, allow move
        if (!Physics.Raycast(transform.position, direction, out hit, 1f))
            return true;

        // wall detected, avoid move
        if (hit.collider.tag == tagWall) {
			return false;
		}

		// we hit a crate? test if there is a wall or another crate behind the crate
		else if (hit.collider.tag == tagCrate) {
			RaycastHit[] hits = Physics.RaycastAll (transform.position, direction, 2f).OrderBy (h => h.distance).ToArray ();

			// no other obstacle behind the crate, allow push
			if (hits.Length == 1) {
				return true;
			}
			// obstacle detected
			else if (hits.Length > 1) {
				int crateCount = 0;
				bool bSwitch = false;
				foreach (RaycastHit h in hits) {
					// count crates
					if (h.collider.tag == tagCrate)
						crateCount++;

					// if more than 1 crate avoid push
					if (crateCount >= 2) {
						return false;
					}

					// is there a switch?
					if (h.collider.tag == tagSwitch)
						bSwitch = true;

					// avoid push if there is a wall
					if (h.collider.tag == tagWall)
						return false;
				}

				if (bSwitch)
					return true;
			}
		}

		return true;
	}

	public void Update () {
		if (Input.GetKeyDown (KeyCode.Space) || SwipeManager.IsSingleTap ()) {
			spawnPortal (portal1, portal2);
		} else if (Input.GetKeyDown (KeyCode.LeftControl) || SwipeManager.IsDoubleTap ()) {
			spawnPortal (portal2, portal1);
		} 
		if (Input.GetKeyDown (KeyCode.Escape)) {
			SceneManager.LoadScene ("Paused");
		}

        Vector3 direction = inputToDirection ();
		safeMove(direction);
	}

    public void spawnPortal(GameObject portal, GameObject otherPortal) {
        if (isMoving)
            return;

        if (onSwitch)
            return;

        if (transform.position == otherPortal.transform.position)
            return;

        portal.SetActive(true);
        portal.transform.position = transform.position;
    }

	public Vector3 inputToDirection () {
		if (Input.GetKeyDown (KeyCode.A) || SwipeManager.IsSwipingLeft()) {
            direction = Vector3.left;
			return Vector3.left;
		}
		else if (Input.GetKeyDown (KeyCode.D) || SwipeManager.IsSwipingRight()) {
            direction = Vector3.right;
            return Vector3.right;
		}
		else if (Input.GetKeyDown (KeyCode.S) || SwipeManager.IsSwipingDown()) {
            direction = Vector3.down;
            return Vector3.down;
		}
		else if (Input.GetKeyDown (KeyCode.W) || SwipeManager.IsSwipingUp()) {
            direction = Vector3.up;
            return Vector3.up;
		}
		return Vector3.zero;
	}

	public void safeMove (Vector3 direction) {
		if (direction == Vector3.zero)
			return;
		
		if (!isMoving) {
			sbyte xIn = 0;
			sbyte yIn = 0;

            if (direction == Vector3.left) {
                if (CanMove(direction))
                    xIn = -1;
            }
            else if (direction == Vector3.right) {
                if (CanMove(direction))
                    xIn = 1;
            }
            else if (direction == Vector3.down) {
                if (CanMove(direction))
                    yIn = -1;
            }
            else if (direction == Vector3.up) {
                if (CanMove(direction))
                    yIn = 1;
            }

            lastMove = direction;

            input = new Vector2 ((float)xIn, (float)yIn);

			if (input != Vector2.zero) {
				StartCoroutine (move (transform));
				GameObject.Find ("GameHud").GetComponent<GameHud> ().IncreaseMoves ();
			}
		}
	}

	public IEnumerator move (Transform transform) {
		isMoving = true;
		startPosition = transform.position;
		t = 0;
		
		endPosition = new Vector3 (startPosition.x + System.Math.Sign (input.x) * gridSize,
			startPosition.y + System.Math.Sign (input.y) * gridSize, 
			startPosition.z);
		
		factor = 1f;
		
		while (t < 1f) {
			t += Time.deltaTime * (moveSpeed / gridSize) * factor;
			transform.position = Vector3.Lerp (startPosition, endPosition, t);
			yield return null;
		}

		isMoving = false;
		yield return 0;
	}
}