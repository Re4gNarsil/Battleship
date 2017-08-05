using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameBoard : MonoBehaviour {

	public GameObject space;

	private GameManager gameManager;
	private GameObject currentShip;
	private Ship myShip;
    private GameObject shotHolder;

	private float shipSize;

	// Use this for initialization
	void Start () {
		gameManager = FindObjectOfType<GameManager>();
        shotHolder = transform.Find("ShotHolder").gameObject;
    }

	public void OnMouseClick(){ //OnMouseDown with a 2d box collider SHOULD work but isn't for some reason
		Vector2 rawPos = RawMousePoint();
		Vector2 calPos = CalculatePointOnBoard(rawPos);
		Vector2 roundPos = SnapToGrid(calPos);
		currentShip = (gameManager.ShipAssign()); //acquire the currently selected ship from the GameManager
		if (currentShip) {
			ShipPlacement(roundPos);
		} else {print ("no ship selected yet");}
	}

	public void SizeIncrease(){
		if (transform.localScale.x < 1.8f) {
			transform.localScale += new Vector3(.1f, .1f, 0);
			transform.localPosition += new Vector3(10f, 10f, 0);
		}
	}

	public void SizeDecrease(){
		if (transform.localScale.x > .6f) {
			transform.localScale -= new Vector3(.1f, .1f, 0);
			transform.localPosition -= new Vector3(10f, 10f, 0);
		}
	}

	Vector2 RawMousePoint(){
		float MouseX = Input.mousePosition.x;
		float MouseY = Input.mousePosition.y;
		Vector2 worldPosition = new Vector2 (MouseX, MouseY);
		return worldPosition;

	}

	Vector2 CalculatePointOnBoard(Vector2 worldPosition){
		float calX = ((10 + worldPosition.x) / (20 * transform.localScale.x));
		float calY = ((10 + worldPosition.y) / (20 * transform.localScale.y));
		Vector2 calPosition = new Vector2 (calX, calY);
		return calPosition;
	}

	Vector2 SnapToGrid(Vector2 boardPosition){
		int SnapX = Mathf.RoundToInt(boardPosition.x);
		int SnapY = Mathf.RoundToInt(boardPosition.y);
		return new Vector2 (SnapX, SnapY);
	}

	void ShipPlacement(Vector2 roundPos) {
		myShip = currentShip.GetComponent<Ship>();
		shipSize = myShip.FindSize();
		if (myShip.FindOrientation()) {
			float newPositionX = (((roundPos.x * 20f) + ((20 * shipSize) / 2) - 20f) * transform.localScale.x);
			float newPositionY = (((roundPos.y * 20f) - 10f) * transform.localScale.y);
			currentShip.transform.position = new Vector3(newPositionX, newPositionY, currentShip.transform.position.z);
		} else {
			float newPositionY = (((roundPos.y * 20f) + ((20 * shipSize) / 2) - 20f) * transform.localScale.y);
			float newPositionX = (((roundPos.x * 20f) - 10f) * transform.localScale.x);
			currentShip.transform.position = new Vector3(newPositionX, newPositionY, currentShip.transform.position.z);
		}
	}

    public void AIShipPlacement(Vector2 ranPos, Ship curShip) {
        currentShip = curShip.gameObject;
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
