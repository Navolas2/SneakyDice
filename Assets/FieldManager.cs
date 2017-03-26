using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldManager : MonoBehaviour {

	public string name;
	public int value;
	public int MinValue;
	UnityEngine.UI.Text value_display;
	// Use this for initialization
	void Start () {
		value_display = GetComponentInChildren<UnityEngine.UI.Text> ();
		//value = MinValue;
		UpdateValue ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void UpdateValue(){
		value_display.text = "" + value;
	}

	public void IncreaseValue(){
		value++;
		UpdateValue ();
	}

	public void DecreaseValue(){
		if (value > MinValue) {
			value--;
			UpdateValue ();
		}
	}
}
