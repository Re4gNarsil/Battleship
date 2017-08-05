using UnityEngine;
using System.Collections;

public class Sky : MonoBehaviour {

	public Material[] skies;
	private MeshRenderer meshRenderer;

	// Use this for initialization
	void Start () {
		meshRenderer = GetComponent<MeshRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ChangeSky(int i){
		meshRenderer.material = skies[i];
	}
}
