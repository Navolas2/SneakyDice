using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Information_Scroll : MonoBehaviour {

	private List<string> info;
	private List<UnityEngine.UI.Text> text_boxes;
	public UnityEngine.UI.Text layout;
	// Use this for initialization
	void Start () {
		info = new List<string> ();
		text_boxes = new List<UnityEngine.UI.Text> ();
	}
		
	public void AddInfo(string new_info){
		info.Add (new_info);
		foreach (UnityEngine.UI.Text tx in text_boxes) {
			tx.transform.localPosition = new Vector3 (tx.transform.localPosition.x, tx.transform.localPosition.y + 90);
		}
		UnityEngine.UI.Text n_box = GameObject.Instantiate (layout, this.transform);
		n_box.transform.localPosition = new Vector3 ();
		n_box.text = new_info;
		text_boxes.Add (n_box);
	}
}
