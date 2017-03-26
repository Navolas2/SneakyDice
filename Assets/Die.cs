using UnityEngine;
using System.Collections;

public class Die
{
	int value;
	int Unique_ID;

	public Die(){
		Roll ();
		Unique_ID = Random.Range (int.MinValue, int.MaxValue);
	}

	public void Roll(){
		value = Random.Range (1, Global_Data.Global_Information.Dice_Faces());
	}

	public int roll_value{
		get{ return value; }
	}

	public int ID{
		get{ return Unique_ID; }
	}
}

