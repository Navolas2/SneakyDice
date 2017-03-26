using UnityEngine;
using System.Collections;

public class Chip
{
	private int point_value;
	private string given_value;

	// Use this for initialization
	public Chip(){
		point_value = 0;
		given_value = "fake";
	}

	public Chip(int val){
		point_value = val;
		given_value = "";
	}

	public Chip(string val){
		point_value = 0;
		given_value = val;
	}

	public Chip(int val, string giv){
		point_value = val;
		given_value = giv;
	}

	// Update is called once per frame
	void Update ()
	{
	
	}

	public int point{
		get{ return point_value; }
	}

	public string given{
		get{ return given_value; }
	}
}

