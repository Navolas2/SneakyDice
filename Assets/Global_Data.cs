using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Global_Data : NetworkBehaviour {

	// Use this for initialization
	public static Global_Data Global_Information;

	private List<Chip> current_pot = new List<Chip>();
	public int Die_Faces = 6;
	private static int min_dice = 2;
	public int Dice_Amount = 2;
	private static int min_kept = 2;
	public int Dice_kept = 2;

	private List<Player_Data> players = new List<Player_Data>();
	int curr_player;

	void Awake(){
		if (Global_Information == null) {
			Global_Information = this;
			DontDestroyOnLoad (this);
		} else if (Global_Information != this) {
			Destroy (this);
		}
	}

	void Start(){
		GameObject[] objects = GameObject.FindGameObjectsWithTag ("Player");
		for (int i = 0; i < objects.Length; i++) {
			players.Add (objects [i].GetComponent<Player_Data>());
		}
		curr_player = 0;
	}
	/*******************/
	//POT FUNCTIONS
	/*******************/

	public int Pot_value{
		get{
			int val = 0; 
			foreach (Chip c in current_pot) {
				val += c.point;
			}
			return val;
		}
	}

	public int Pot_count{
		get{ return current_pot.Count; }
	}
		
	public void Raise(List<Chip> raise){
		foreach (Chip c in raise) {
			current_pot.Add (c);
		}
		//TODO add alert to tell player(s) of the raise
	}

	public void Raise(Chip raise, string name){
		current_pot.Add (raise);
		//TODO add alert to tell player(s) of the raise
		foreach (Player_Data p in players) {
			p.UpdatePot ();
			p.AddInfo (name + " raised");
		}
	}

	public void FindWinner(){
		Player_Data winner = null;
		int value = 0;
		bool draw = false;
		string result = "";
		foreach (Player_Data p in players) {
			result = result + p._name + " rolled: " + p.Dice_Rolled () + " with a value of " + p.FinalRollValue + "\n";
			if (winner != null) {
				if (winner.FinalRollValue < p.FinalRollValue) {
					winner = p;
					value = winner.FinalRollValue;
					draw = false;
				} else if (winner.FinalRollValue == p.FinalRollValue) {
					draw = true;
				}
			} else {
				winner = p;
			}
		}

		if (!draw) {
			result = result + "Winner is " + winner._name;
			winner.RecieveChips (current_pot);
			ClearPot ();
		} else {
			result = result +"It's a draw!";
		}
		foreach (Player_Data p in players) {
			p.ClearRoll ();
			p.AddInfo (result);
		}
	}

	public void ClearPot(){
		current_pot.Clear ();
	}

	/*******************/
	//Dice Functions
	/*******************/

	public int Dice_Faces(){
		return Die_Faces;
	}
		
	public int Dice_To_Roll(){
		return Dice_Amount;
	}
		
	public int Kept_Dice(){
		return Dice_kept;
	}

	public void UpdateDice(int faces, int rolled, int kept){
		Die_Faces = faces;
		if (rolled >= min_dice) {
			Dice_Amount = rolled;
		}
		if (kept >= min_kept) {
			Dice_kept = kept;
		}
	}

	/*******************/
	//Game Functions
	/*******************/

	public void UpdateInformation(string info){
		foreach (Player_Data p in players) {
			p.AddInfo (info);
		}
	}

	public void ClearPlayers(){
		players.Clear ();
	}

	void Update(){
		bool ready = true;
		if (players.Count > 1) {
			foreach (Player_Data p in players) {
				ready = ready && p.isReady () && (p.RollValue > 0) ;
			}
			if (ready) {
				FindWinner ();
			}
		
			//TODO update code for multiple opponents
			for (int i = 1; i < players.Count; i++) {
				Player_Data p = players [i];
				p.OpponentValue (players [i - 1].RollValue);
				if (i + 1 == players.Count) {
					players [0].OpponentValue (p.RollValue);
				}
			}
		}
	}
		
	public void AddPlayer(GameObject p){
		CmdAddPlayer (p);
	}

	[Command]
	public void CmdAddPlayer(GameObject p){
		if(!players.Contains(p.GetComponent<Player_Data>())){
			players.Add (p.GetComponent<Player_Data>());
		}
	}

}
