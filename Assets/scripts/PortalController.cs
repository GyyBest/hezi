using UnityEngine;
using System.Collections;

public class PortalController : MonoBehaviour {
	public GameObject PortalOrig;
	public GameObject PortalDest;
	private GameObject Player;
	private PlayerController playerController;

	void Start () {
        Player = GameObject.Find("Player(Clone)");
		playerController = Player.GetComponent<PlayerController> ();
	}

	void OnCollisionEnter(Collision other) {
        if (!PortalDest.activeSelf)
            return;

        if (!isApproching(other))
            return;

        if (other.gameObject == Player) {
            if (!playerController.isMoving)
                return;

            StartCoroutine (TeleportPlayer(PortalDest, other.collider));
		}
		else if (other.collider.tag == "Crate") {
			if (!other.collider.GetComponent<CubeController>().isMoving)
				return;
            StartCoroutine(TeleportCrate(PortalDest, other));
        }
	}

    bool isApproching(Collision other) {
        Vector3 diff = other.gameObject.transform.position - transform.position;
        diff.Normalize();
        if (diff != playerController.direction && diff != Vector3.zero) {
            return true;
        }
        return false;
    }

	IEnumerator TeleportPlayer(GameObject destPortal, Collider playerCollider) {
		yield return new WaitForSeconds (.3f);
		Player.transform.position = destPortal.transform.position;
		playerController.safeMove (playerController.lastMove);
        Collider destPortalCollider = destPortal.GetComponent<Collider>();
        Physics.IgnoreCollision(destPortalCollider, playerCollider, true);
        yield return new WaitForSeconds(.3f);
        Physics.IgnoreCollision(destPortalCollider, playerCollider, false);
    }

    IEnumerator TeleportCrate(GameObject destPortal, Collision crateCollision) {
        GameObject crate = crateCollision.gameObject;
        CubeController cubeController = crate.GetComponent<CubeController>();
        yield return new WaitForSeconds(.3f);
        crate.transform.position = destPortal.transform.position;

        RaycastHit hit;
        if (!Physics.Raycast(crate.transform.position, playerController.lastMove, out hit, 1f))
            StartCoroutine(cubeController.move(crate.transform));

        Collider destPortalCollider = destPortal.GetComponent<Collider>();
        Physics.IgnoreCollision(destPortalCollider, crateCollision.collider, true);
        yield return new WaitForSeconds(.3f);
        Physics.IgnoreCollision(destPortalCollider, crateCollision.collider, false);
    }
}