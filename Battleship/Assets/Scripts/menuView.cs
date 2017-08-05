using UnityEngine;
using System.Collections;

public class menuView : MonoBehaviour {

    private View view;
    private Vector3 centerPosition; //center of the world we make
    private bool day;
    private float maxDegrees = 360, secsPerDay = 88400, degPerSecond;
    private int timeScale = 3600;
    //private Camera scope;

    // Use this for initialization
    void Start() {
        //scope = GetComponent<Camera>();
        centerPosition = new Vector3(500f, 0f, 500f);
        degPerSecond = (((maxDegrees / secsPerDay) * timeScale) / 2); //full circle / length of a day * how fast we speed things up, then cut it in half
    }

    // Update is called once per frame
    void Update() {
        transform.RotateAround(centerPosition, Vector3.down, (degPerSecond * Time.deltaTime));
    }
}
