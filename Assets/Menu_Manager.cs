using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Menu_Manager : MonoBehaviour {

	// Use this for initialization
	public NetworkManager manager;

	void Start () {
		NetworkManager[] existing = FindObjectsOfType<NetworkManager> ();
		foreach (NetworkManager nm in existing) {
			Destroy (nm.gameObject);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void StartHost(){
		FieldManager[] fields = GetComponentsInChildren<FieldManager> ();
		List<int> values = new List<int> ();
		foreach (FieldManager f in fields) {
			values.Add (f.value);
		}

		GameObject.Instantiate (manager);
		NetworkManager.singleton.StartHost ();
		Global_Data.Global_Information.UpdateDice (values [0], values [1], values [2]);
		Destroy (this.gameObject);
	}

	public void StartClient(){
		GameObject.Instantiate (manager);
		NetworkManager.singleton.StartClient();
		Destroy (this.gameObject);
	}
}
