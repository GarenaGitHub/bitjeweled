using UnityEngine;
using System.Collections;

public class AutoDestroyShuriken : MonoBehaviour {
	
	private ParticleSystem ps = null;

	// Use this for initialization
	void Start () {
		ps = gameObject.transform.GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
		if (!ps.IsAlive())
			Destroy(gameObject);
	}
}
