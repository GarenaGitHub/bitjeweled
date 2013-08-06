using UnityEngine;
using System.Collections;

public class ApiManagerSpawner : MonoBehaviour {

    public GameObject prefab;

    void Start() {
        ApiManager api = FindObjectOfType(typeof(ApiManager)) as ApiManager;
        if (api == null) {
            GameObject.Instantiate(prefab);
        }
    }

}
