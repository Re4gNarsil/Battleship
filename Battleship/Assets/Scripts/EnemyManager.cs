using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;

public class EnemyManager : MonoBehaviour {

    private GameObject currentShip, newShip;
    private LevelManager levelManager;
    private EnemyShip[] enemyShips;
    private GameProgress gameProgress;
    private GameManager gameManager;
    private EnemyBoard enemyBoard;
    private EnemyBoard2 enemyBoard2;
    static string ourGameData = "", enemyGameData = ""; //temporary substitute for a real file that says if you've hit or missed
    private bool gameStarted, wereHit, autoPlace, CPUMode, hardMode, CPUTurn;
    private int shipsSunk, hitsStruck, oddEven;
    private string[] spacesConnected = new string[17];
    private float enemySize, time;

    private int testAttempts;

    // Use this for initialization
    void Start()
    {
        enemyBoard = FindObjectOfType<EnemyBoard>();
        enemyBoard2 = FindObjectOfType<EnemyBoard2>();
        gameProgress = FindObjectOfType<GameProgress>();
        gameManager = FindObjectOfType<GameManager>();
        enemyShips = FindObjectsOfType<EnemyShip>();
        levelManager = FindObjectOfType<LevelManager>();
        CPUMode = levelManager.SetOpponent();
		hardMode = levelManager.SetDifficulty();
	}

    void StartGame()
    {
        oddEven = Random.Range(0, 2);
        hitsStruck = 0;
        shipsSunk = 0;
        for (int i = 0; i < 17; i++)
        {
            spacesConnected[i] = "";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (CPUMode)
        {
            if (gameStarted) {
                if (CPUTurn) {
                    if (enemyGameData == "") { TakeShot(); }
                    if (enemyGameData.Length > 2) { ReceiveOurShotData(); }
                } else {
                    if (ourGameData.Length > 1) {
                        if (time == 0f) { ReceiveEnemyShotData(); }
                        else if ((Time.time - time) > .25f) {
                            time = 0f;
                            SendEnemyShotData();
                        }
                    }
                }
            }
            else {
                if (autoPlace)
                {
                    PlaceShips();
                    if (gameProgress.CountShips(false)) {
                        autoPlace = false;
                        gameStarted = true;
                    }
                }
            }
        }
    }

    public void GiveOurData(bool theirData, string newData) {
        if (theirData) {  enemyGameData = newData; }
        else { ourGameData = newData; }
    }

    public void SwitchSides() {
        CPUTurn = true;
    }

    public void TakeShot()
    {//this will have an AI fire at the opponent, might eventually automate
        string[] ourTargets = new string[16];
        string nextTarget = "";
        int targetCount = 0;
        //bool tryReversing = false;
        for (int i = 16; i >= 0; i--)
        {//start looking for most recently spotted ship first
            if ((!(spacesConnected[i] == "")) && (spacesConnected[i].Length < 3))
            {
                ourTargets[targetCount] = (spacesConnected[i]);
                targetCount++;
            }
        }
        //SecondTry:
        if (targetCount > 0)
        {
            if (targetCount == 1)
            {//have we only landed one hit so far
                nextTarget = (OneHit(ourTargets));
                if (nextTarget == "") { nextTarget = (NoHits()); }
            }
            else if (ourTargets[0].Substring(0, 1) == ourTargets[1].Substring(0, 1))
            {//is our enemy placed horizontally
                nextTarget = (MultipleHits(ourTargets, targetCount, true));
                if (nextTarget == "")
                {
                    //if (!(tryReversing)) { //this might not be needed at all
                    //    ourTargets = (SecondAttempt(ourTargets)); //try retracing our steps
                    //    tryReversing = true;
                    //   goto SecondTry;
                    //}
                    if (nextTarget == "")
                    {
                        nextTarget = (MultipleHits(ourTargets, targetCount, false)); //eventually true
                        if (nextTarget == "") { nextTarget = (NoHits()); }
                    }
                }
            }
            else if (ourTargets[0].Substring(1, 1) == ourTargets[1].Substring(1, 1))
            {//is our enemy not placed horizontally
                nextTarget = (MultipleHits(ourTargets, targetCount, false));
                if (nextTarget == "")
                {
                    //if (!(tryReversing)) { //this might not be needed at all
                    //    ourTargets = (SecondAttempt(ourTargets)); //try retracing our steps
                    //    tryReversing = true;
                    //   goto SecondTry;
                    //}
                    if (nextTarget == "")
                    {
                        nextTarget = (MultipleHits(ourTargets, targetCount, true)); //eventually false
                        if (nextTarget == "") { nextTarget = (NoHits()); }
                    }
                }
            }
            else {
                nextTarget = (OneHit(ourTargets));
            }
        }
        else { nextTarget = (NoHits()); }
        SendOurShotData(nextTarget, false);
    }

    string NoHits()
    {
        int attempts = 0;
    HereFirst:
        attempts++;
        if (attempts > 119) { oddEven = (1 - oddEven); }
        string newTarget = "";
        int ranRow = Random.Range(0, 10);
        int ranCol = Random.Range(0, 10);
        GameObject spaceTwo = enemyBoard2.transform.Find("Board (" + ranRow.ToString() + ranCol.ToString() + ")").gameObject;
        if (enemyBoard2.EmptySpace(spaceTwo, false))
        {
			if (hardMode) { if (!(((ranRow % 2 == oddEven) && (ranCol % 2 == oddEven)) || ((ranRow % 2 == (1 - oddEven)) && (ranCol % 2 == (1 - oddEven))))) { goto HereFirst; } }
            //the above code determines if the computer is checking the board more strategically or not, and if so did it do so successfully
            newTarget = ((ranRow.ToString()) + (ranCol.ToString()));
        }
        else { goto HereFirst; }
        return newTarget;
    }

    string OneHit(string[] ourTarget)
    {
        int attempts = 0;
    HereSecond:
        attempts++;
        if (attempts > 19) { return ""; }
        char[] nextTarget = ourTarget[0].ToCharArray();
        int slot = Random.Range(0, 2);
    TryAgain:
        int direction = Random.Range(-1, 2);
        if (direction == 0) { goto TryAgain; }
        else {
            int newNumber = (int.Parse(ourTarget[0].Substring(slot, 1)) + direction);
            if ((newNumber > 9) || (newNumber < 0)) { goto TryAgain; }
            nextTarget[slot] = char.Parse(newNumber.ToString());
        }
        GameObject space = enemyBoard2.transform.Find("Board (" + nextTarget[0] + nextTarget[1].ToString() + ")").gameObject;
        if (enemyBoard2.EmptySpace(space, false)) { } else { goto HereSecond; }
        return (nextTarget[0].ToString() + nextTarget[1].ToString());
    }

    string MultipleHits(string[] ourTargets, int targetCount, bool horizontal)
    {
        string nextTarget = "";
        int spotsLeft = (targetCount - 1);
    Again:
        int numTry = Random.Range(-1, 2);
        if (numTry == 0) { goto Again; }
        if ((spotsLeft + 1) < 1) { return ""; }
        int targetX = int.Parse(ourTargets[(targetCount - 1) - spotsLeft].Substring(1, 1));
        int targetY = int.Parse(ourTargets[(targetCount - 1) - spotsLeft].Substring(0, 1));
        if (horizontal) { nextTarget = (Horizontal(targetX, targetY, numTry)); }
        else { nextTarget = (Vertical(targetX, targetY, numTry)); }
        if (nextTarget == "")
        {
            spotsLeft--;
            goto Again;
        }
        return nextTarget;
    }

    string SecondAttempt()
    {
        string newTargets = "";
        for (int i = 0; i < 17; i++)
        {
            if (spacesConnected[i].Length > 2)
            {//this option may never be needed
            }
            else if (spacesConnected[i].Length == 2)
            {
                spacesConnected[i] = (spacesConnected[i] + "R");
            }
        }
        return newTargets;
    }

    string Horizontal(int targetX, int targetY, int difference)
    {
        //these have been accidently flipped on the physical board and need to remain this way for the time being
        if ((targetX + difference) >= 0 && (targetX + difference) <= 9)
        {
            GameObject space = enemyBoard2.transform.Find("Board (" + targetY.ToString() + (targetX + difference).ToString() + ")").gameObject;
            if (enemyBoard2.EmptySpace(space, false))
            {
                return (targetY.ToString() + (targetX + difference).ToString());
            }
        }
        if ((targetX - difference) >= 0 && (targetX - difference) <= 9)
        {
            GameObject SecondSpace = enemyBoard2.transform.Find("Board (" + targetY.ToString() + (targetX - difference).ToString() + ")").gameObject;
            if (enemyBoard2.EmptySpace(SecondSpace, false))
            {
                return (targetY.ToString() + (targetX - difference).ToString());
            }
        }
        return "";
    }

    string Vertical(int targetX, int targetY, int difference)
    {
        //these have been accidently flipped on the physical board and need to remain this way for the time being
        if ((targetY + difference) >= 0 && (targetY + difference) <= 9)
        {
            GameObject space = enemyBoard2.transform.Find("Board (" + (targetY + difference).ToString() + targetX.ToString() + ")").gameObject;
            if (enemyBoard2.EmptySpace(space, false))
            {
                return ((targetY + difference).ToString() + targetX.ToString());
            }
        }
        if ((targetY - difference) >= 0 && (targetY - difference) <= 9)
        {
            GameObject secondSpace = enemyBoard2.transform.Find("Board (" + (targetY - difference).ToString() + targetX.ToString() + ")").gameObject;
            if (enemyBoard2.EmptySpace(secondSpace, false))
            {
                return ((targetY - difference).ToString() + targetX.ToString());
            }
        }
        return "";
    }

    public void SendOurShotData(string pos, bool userShot)
    {//temporarily firing at our own board for testing purposes; eventually will export to a file
        enemyGameData = pos;//flipped on this side
        gameManager.GiveOurData(false, enemyGameData);
    }

    public void ReceiveOurShotData()
    {//temporarily receiving data from our own board for testing purposes
        string pos = enemyGameData.Substring(0, 2);//flipped on this side
        if (enemyGameData.Substring(2, 1) == "H")
        {
            HitsAndSunks();
            enemyBoard2.PlaceShot(pos, true);
        }
        else { enemyBoard2.PlaceShot(pos, false); }
        CPUTurn = false;
        enemyGameData = "";//flipped on this side
        gameManager.SwitchSides();
    }

    public void ReceiveEnemyShotData()
    {
        wereHit = false;
        enemySize = -1;
        enemyBoard.PlaceShot(ourGameData);//flipped on this side
        time = Time.time;
    }

    void SendEnemyShotData()
    {
        if (wereHit) { ourGameData = (ourGameData + "H"); }//flipped on this side
        else { ourGameData = (ourGameData + "M"); }
        if (enemySize > 0) { ourGameData = (ourGameData + enemySize); }
        gameManager.GiveOurData(true, ourGameData);
        ourGameData = "";
    }

    public void RecordHit(float shipSize)
    {
        wereHit = true;
        if (shipSize > 0) { enemySize = shipSize; }
    }

    void HitsAndSunks()
    {
        if (enemyGameData.Substring(2, 1) == "H")
        {//flipped on this side
            spacesConnected[hitsStruck] = (enemyGameData.Substring(0, 1) + enemyGameData.Substring(1, 1));
            hitsStruck++;
            if (enemyGameData.Length > 3)
            {
                shipsSunk++;
                if (shipsSunk < 5) {
                    if (!(RemoveShip(0)))
                    {//need to check for an almost complete ship in case part of it was removed along with another ship
                        if (!(RemoveShip(1))) { print("ship not found"); }
                    }
                }
            }
        }
    }

    bool RemoveShip(int partsSent)
    {
        string firstTarget = (enemyGameData.Substring(0, 1) + enemyGameData.Substring(1, 1));
        int shipSize = int.Parse(enemyGameData.Substring(3, 1));
        int shipParts = partsSent, numSlot = -1, attempts = 0;
        bool shipFound = false;
        for (int i = 16; i >= 0; i--)
        {//we need to check every hit and see if it belongs to the ship we just sunk
            if (!(spacesConnected[i] == ""))
            {
                if (firstTarget.Substring(0, 1) == spacesConnected[i].Substring(0, 1)) { numSlot = 1; }
                else if (firstTarget.Substring(1, 1) == spacesConnected[i].Substring(1, 1)) { numSlot = 0; }
                if (!(numSlot == -1))
                {
                    int direction = (int.Parse(firstTarget.Substring(numSlot, 1)) - int.Parse(spacesConnected[i].Substring(numSlot, 1)));
                    if ((direction == 1) || (direction == -1))
                    {
                        OtherDirection:
                        attempts++;
                        if (attempts == 3) { numSlot = (1 - numSlot); }
                        else if (attempts > 5) {//the reason for this test is to possibly check other sunken vessels if needed
                            attempts = 0;
                            shipParts = partsSent;
                            numSlot = -1;
                            RevertRemoveSegments(false);
                            return false; //for now just bail out; eventually may leave AT 10ish
                        }
                        shipParts = FindSegments(direction, numSlot, firstTarget, shipSize, shipParts, shipFound);
                        if (shipParts == shipSize)
                        {
                            RevertRemoveSegments(true);
                            return true;
                        }
                        else if (shipParts < shipSize)
                        {
                            direction += (direction * -2);
                            shipFound = true;
                            //attempts--;
                            goto OtherDirection;
                        }
                        else {
                            RevertRemoveSegments(false);
                            direction += (direction * -2);
                            shipParts = partsSent;
                            goto OtherDirection;
                        }
                    }
                }
            }
        }
        return false;
    }

    int FindSegments(int direction, int slot, string firstTarget, int shipSize, int partsFound, bool shipFound)
    {
        string sunkShipSegment;//starting at our last hit, look in all directions for more of the ship, one at a time
        int shipPieces = partsFound;
        int nextSpot = int.Parse(firstTarget.Substring(slot, 1));
        int constSpot = int.Parse(firstTarget.Substring((1 - slot), 1));
        for (int i = nextSpot; ((i >= 0) && (i <= 9)); i += direction)
        {
            if (slot == 0) { sunkShipSegment = (i.ToString() + constSpot.ToString()); }
            else { sunkShipSegment = (constSpot.ToString() + i.ToString()); }
            for (int n = (hitsStruck - 1); n >= 0; n--)
            {
                if (spacesConnected[n] == sunkShipSegment)
                {
                    spacesConnected[n] = (spacesConnected[n] + "T");
                    shipPieces++;
                    if ((shipFound) && (shipPieces == shipSize)) { return shipPieces; }
                }
            }
        }
        return shipPieces;
    }

    void RevertRemoveSegments(bool remove)
    {
        for (int i = 0; i < 17; i++)
        {
            if (spacesConnected[i].Length > 2)
            {
                if (spacesConnected[i].Substring(2, 1) == "T")
                {
                    if (!(remove)) { spacesConnected[i] = spacesConnected[i].Substring(0, 2); }
                    else { spacesConnected[i] = (spacesConnected[i].Substring(0, 2) + "R"); }
                }
            }
        }
    }

    void FindAnotherShip()
    {//another function that may well not be needed
    }

    public void PlaceShips()
    {
        foreach (EnemyShip ship in enemyShips)
        {
            if (ship.GetComponent<Image>().color == Color.red)
            {
                int ranNum = Random.Range(0, 2);
                if (ranNum == 0) { ship.ComputerSelect(true); }
                Vector2 ranPos = new Vector2(Random.Range(0, 9), Random.Range(0, 9));
                AssignShip(ship.gameObject);
                enemyBoard.AIShipPlacement(ranPos);
            }
        }
    }

    public void AutoAssign()
    {
        autoPlace = true;
    }

    public void AssignShip(GameObject ship)
    {
        currentShip = ship;
    }

    public GameObject ShipAssign()
    {
        if (!gameStarted) { return currentShip; }
        else { return null; }
    }

    public void ResetGame()
    {
        if (CPUMode)
        {
            gameStarted = false;
            foreach (EnemyShip ship in enemyShips) { ship.ResetShip(); }
            foreach (EnemyShip ship in enemyShips) { ship.ResetShip(); }
            enemyBoard.RemoveShots();
            enemyBoard2.RemoveShots();
        }
    }

    public void NewGame() {
        AutoAssign();
        StartGame();
    }
}
