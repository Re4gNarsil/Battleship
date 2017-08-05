using UnityEngine;
using System.Collections;

public class DayNight : MonoBehaviour {

	//private Sky sky;

	private View view;
    private LensFlare lensFlare;
	private Vector3 centerPosition; //center of the world we make
	private bool day;
	private float maxDegrees = 360, secsPerDay = 88400, speed, degPerSecond;
	private int timeScale = 360;

	// Use this for initialization
	void Start () {
		view = FindObjectOfType<View>();
        lensFlare = GetComponent<LensFlare>();
		//sky = FindObjectOfType<Sky>();
		centerPosition = new Vector3(500f, 0f, 500f);
		degPerSecond = ((maxDegrees / secsPerDay) * timeScale * speed); //full circle / length of a day * how fast we speed things up
	}

	// Update is called once per frame
	void Update () {
        transform.RotateAround(centerPosition, Vector3.right, (degPerSecond * Time.deltaTime));
        if (transform.position.y < 0) { lensFlare.enabled = false; }
        else { lensFlare.enabled = true; }

        if (view) {
            if (transform.position.y > 200) {
                view.ChangeSky(0);
            }
            else if (transform.position.y < -200) {
                view.ChangeSky(1);
            }
            else {
                if (transform.position.x < 500) {
                    view.ChangeSky(0); //2
                }
                else {
                    view.ChangeSky(0); //3
                }
            }
        }
	}

	public void SetSpeed(float newSpeed)
	{
		speed = newSpeed;
	}
}