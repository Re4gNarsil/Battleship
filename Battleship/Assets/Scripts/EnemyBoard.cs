using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class EnemyBoard : MonoBehaviour {

	public GameObject space;

	private EnemyManager enemyManager;
	private GameObject currentShip;
	private EnemyShip myShip;
    private GameObject shotHolder;

	private float shipSize;

	// Use this for initialization
	void Start () {
        enemyManager = FindObjectOfType<EnemyManager>();
        shotHolder = transform.Find("ShotHolder").gameObject;
    }

	Vector2 SnapToGrid(Vector2 boardPosition){
		int SnapX = Mathf.RoundToInt(boardPosition.x);
		int SnapY = Mathf.RoundToInt(boardPosition.y);
		return new Vector2 (SnapX, SnapY);
	}

	void ShipPlacement(Vector2 roundPos) {
		myShip = currentShip.GetComponent<EnemyShip>();
		shipSize = myShip.FindSize();
		roundPos = roundPos - (Vector2.one * 5);
		if (myShip.FindOrientation()) {
			float newPositionX = (((roundPos.x * 20f) + ((20 * shipSize) / 2) - 20f) * transform.localScale.x);
			float newPositionY = (((-roundPos.y * 20f) - 10f) * transform.localScale.y);
            currentShip.transform.localPosition = new Vector3(newPositionX, newPositionY, currentShip.transform.position.z);
        } else {
			float newPositionY = (((-roundPos.y * 20f) + ((20 * shipSize) / 2) - 20f) * transform.localScale.y);
			float newPositionX = (((roundPos.x * 20f) - 10f) * transform.localScale.x);
            currentShip.transform.localPosition = new Vector3(newPositionX, newPositionY, currentShip.transform.position.z);
        }
    }

    public void AIShipPlacement(Vector2 ranPos) {
        currentShip = (enemyManager.ShipAssign()); //acquire the currently selected ship from the GameManager
        ShipPlacement(ranPos);
    }

	public void PlaceShot(string pos){
		float posX = float.Parse(pos.Substring(1, 1)) - 5;
		float posY = float.Parse(pos.Substring(0, 1)) - 5;

		float shotPosX = (posX * 20 * transform.localScale.x) + 10;
		float shotPosY = (-posY * 20 * transform.localScale.y) - 10;

		Vector3 newPos = new Vector3(shotPosX, shotPosY, transform.position.z);
        GameObject childObject = Instantiate(space, transform.position, Quaternion.identity);
        childObject.tag = "Shot";
        childObject.transform.SetParent(shotHolder.transform);
		childObject.transform.localPosition = newPos;
	}

    public void RemoveShots() {
        foreach(Transform child in shotHolder.transform) {
            Destroy(child.gameObject);
        }
    }
}
