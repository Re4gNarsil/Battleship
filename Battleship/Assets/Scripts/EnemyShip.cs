using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyShip : MonoBehaviour {

	static int shipsSunk;

	private EnemyManager enemyManager;

	private float lastTime;
	private int intrusions = 0, hits = 0;
    private Vector3 startPosition;
    private Quaternion startRotation;
	private bool doubleClick;

	// Use this for initialization
	void Start () {
        enemyManager = FindObjectOfType<EnemyManager>();
        startPosition = transform.position;
        startRotation = transform.rotation;
	}

	public float FindSize(){
		return transform.localScale.x;
	}

	public bool FindOrientation() {
		if ((transform.localRotation.z == 0) || (transform.localRotation.z == 1))	{return true;}
		else																		{return false;}
	}

    public void ComputerSelect(bool rotate) {
        if (rotate) { RotateShip(); }
        enemyManager.AssignShip(gameObject); //give currently selected ship to the GameManager
    }

	void OnTriggerEnter2D(Collider2D collider){
		if ((collider.GetComponent<EnemyShip>()) || (collider.tag == ("Barrier"))){
			gameObject.GetComponent<Image>().color = Color.red;
			intrusions++;
		} else {
            collider.GetComponent<Image>().color = Color.red;
			hits++;
            enemyManager.AssignShip(gameObject);
			if (hits >= transform.localScale.x)	{
                shipsSunk++;
                enemyManager.RecordHit(transform.localScale.x);
            }
            else { enemyManager.RecordHit(-1); }
		}
	}

	void OnTriggerExit2D(Collider2D collider) {
		if ((collider.GetComponent<EnemyShip>()) || (collider.tag == ("Barrier"))){
			intrusions--;
			if (intrusions == 0) {gameObject.GetComponent<Image>().color = Color.white;}
		}
	}

	void RotateShip(){
		transform.Rotate(0f,0f,90f); //turn currently selected ship 90 degrees
		if (transform.localScale.x %2 == 0) {transform.Translate(10f,10f,0f);}
	}

    public void ResetShip() {
        transform.position = startPosition;
        transform.rotation = startRotation;
        hits = 0;
        shipsSunk = 0;
    }
}
