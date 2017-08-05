using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameProgress : MonoBehaviour {

	private Ship[] ships;
    private EnemyShip[] enemyShips;
    private GameBoard gameBoard;
    private EnemyBoard enemyBoard;
	private GameManager gameManager;
    private Text text;

	// Use this for initialization
	void Start () {
        gameManager = FindObjectOfType<GameManager>();
        gameBoard = FindObjectOfType<GameBoard>();
        enemyBoard = FindObjectOfType<EnemyBoard>();
        text = GetComponent<Text>();
        ships = gameBoard.GetComponentsInChildren<Ship>();
        enemyShips = enemyBoard.GetComponentsInChildren<EnemyShip>();
    }

	public void OnMouseClick(){
		if (text.text == ("Place/Start")) {
            if (CountShips(true))	{
				gameManager.StartGame();
                text.text = "Take Shot";
			} else {
                gameManager.AutoAssign();
            }
		} else if (text.text == ("Reset Game")){
			SceneManager.LoadScene("Scene00");
			//gameManager.NewGame();
			//text.text = "Place/Start";
		} else {gameManager.TakeShot();}
	}

    public bool CountShips(bool playersShips) {
        int count = 0;
        if (playersShips) { 
            foreach (Ship ship in ships) {
                if (ship.GetComponent<Image>().color == Color.white) {
                    count++;
                }
            }
        } else {
            foreach (EnemyShip ship in enemyShips) {
                if (ship.GetComponent<Image>().color == Color.white) {
                    count++;
                }
            }
        }
        if (count == 5) { return true; }
        else            { return false; }
    }

    public void Restart() {
        text.text = "Reset Game";
    }

}
