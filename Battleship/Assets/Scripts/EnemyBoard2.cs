using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EnemyBoard2 : MonoBehaviour {

	public void PlaceShot(string pos, bool hit){
        GameObject space = transform.Find("Board (" + pos + ")").gameObject;
        Image image = space.GetComponent<Image>();
        if (hit) { image.color = Color.red; }
        else     { image.color = Color.white; }
	}

	public bool EmptySpace(GameObject space, bool sendMessage) {//have you already fired at this space?  if so, try again
		Image image = space.GetComponent<Image>();
		if (image.color == Color.white) {
            if (sendMessage) { print("You already missed on that space"); }
			return false;
		} else if (image.color == Color.red) {
            if (sendMessage) { print("You already hit something on that space"); }
			return false;
		} else	{return true;}
	}

	public void SizeIncrease(){
		if (transform.localScale.x < 1.8f) {
			transform.localScale += new Vector3(.1f, .1f, 0);
			transform.localPosition += new Vector3(-10f, 10f, 0);
		}
	}

	public void SizeDecrease(){
		if (transform.localScale.x > .6f) {
			transform.localScale -= new Vector3(.1f, .1f, 0);
			transform.localPosition -= new Vector3(-10f, 10f, 0);
		}
	}

    public void RemoveShots() {
        foreach (Transform child in transform) {
            Image image = child.GetComponent<Image>();
            if (child.tag == "Shot") { image.color = Color.clear; }
        }
    }
}
