using UnityEngine;
using System.Collections;

public class View : MonoBehaviour {

	public Material[] skyboxes;

	//private GameObject background;
	//private Color dayWater, nightWater;
	//private Camera scope;

	// Use this for initialization
	void Start () {
		//scope = GetComponent<Camera>();
		//background = GameObject.FindGameObjectWithTag("Water");
		//dayWater = new Color (1f,1f,1f,1f);
		//nightWater = new Color (.3f,.3f,.3f,.7f);
	}


	public void ChangeSky(int i){
		RenderSettings.skybox = skyboxes[i];
		if (i == 0) {
			//background.GetComponent<Color>().Equals (dayWater);
		} else {
			//background.GetComponent<Color>().Equals (nightWater);
		}
	}
}
