using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class Player_Data : NetworkBehaviour
{
	[SyncVar]
	public string _name = "";
	private static int MaxReRolls = 3;
	List<Chip> current_chips = new List<Chip>();
	List<Die> dice_roll = new List<Die>();
	UnityEngine.UI.Toggle ready_button;
	//UnityEngine.UI.Text opponent_value;
	UnityEngine.UI.Text curr_pot;
	UnityEngine.UI.Text current_chip_count;
	UnityEngine.UI.Image value_image;
	int ReRollCount;
	private int raised = 0;
	public Menu_Manager menu_prefab;
	public data_hold hold_prefab;
	private Information_Scroll infoscreen;

	[SyncVar]
	private int Dice_Roll_Value = 0;
	[SyncVar]
	private int Dice_Roll_Final = 0;
	[SyncVar]
	private bool ready_state = false;
	[SyncVar]
	public int Opponent_Roll = 0;

	private int Flash_Value = 60;
	private int flash_count = 0;
	private int current_count = 0;

	// Use this for initialization
	void Start ()
	{
		if (!isLocalPlayer) {
			UnityEngine.UI.Toggle[] toggles = GetComponentsInChildren<UnityEngine.UI.Toggle> ();
			UnityEngine.UI.Text[] texts = GetComponentsInChildren<UnityEngine.UI.Text> ();
			UnityEngine.UI.Button[] buttons = GetComponentsInChildren<UnityEngine.UI.Button> ();
			for (int i = 0; i < texts.Length; i++) {
				texts [i].transform.localPosition = new Vector3 (0, 10000);
			}
			for (int i = 0; i < buttons.Length; i++) {
				buttons [i].transform.localPosition = new Vector3 (0, 10000);
			}
			for (int i = 0; i < toggles.Length; i++) {
				toggles [i].transform.localPosition = new Vector3 (0, 10000);
			}
			UnityEngine.UI.Image[] images = GetComponentsInChildren<UnityEngine.UI.Image> ();
			for (int i = 0; i < images.Length; i++) {
				images [i].transform.localPosition = new Vector3 (0, 10000);
			}
		}

		infoscreen = GetComponentInChildren<Information_Scroll> ();
		//current_chips = new List<Chip> ();
		if (isServer) {
			for (int i = 0; i < 10; i++) {
				Chip c = new Chip (10);
				current_chips.Add (c);
			}
			RpcUpdateCount(current_chips.Count);
		}
		//dice_roll = new List<Die> ();
		//TODO Add setup system for starting amount of chips
		ready_button = GetComponentInChildren<UnityEngine.UI.Toggle>();
		//opponent_value = GetComponentInChildren<UnityEngine.UI.Text> ();
		ClearRoll ();
		UnityEngine.UI.Text[] text = GetComponentsInChildren<UnityEngine.UI.Text> ();
		for (int i = 0; i < text.Length; i++) {
			if (text [i].tag == "current_pot") {
				curr_pot = text[i];
			}
			if (text [i].tag == "current_chip") {
				current_chip_count = text [i];
			}
		}
		//TODO change for multiple opponents
		if (isServer) {
			Flash_Value *= 2;
		}
		CmdAddPlayer (this.gameObject, _name);

	}

	public override void OnStartLocalPlayer ()
	{
		//dice_roll = new List<Die> ();
		//TODO Add setup system for starting amount of chips
		ready_button = GetComponentInChildren<UnityEngine.UI.Toggle>();
		//opponent_value = GetComponentInChildren<UnityEngine.UI.Text> ();
		ClearRoll ();
		_name =  GameObject.FindGameObjectWithTag("player_settings").GetComponent<data_hold>().player_name;
		Destroy (GameObject.FindGameObjectWithTag ("player_settings"));
		UnityEngine.UI.Text[] text = GetComponentsInChildren<UnityEngine.UI.Text> ();
		for (int i = 0; i < text.Length; i++) {
			if (text [i].tag == "current_pot") {
				curr_pot = text[i];
			}
			if (text [i].tag == "current_chip") {
				current_chip_count = text [i];
			}
			if (text [i].tag == "Name_Field") {
				text [i].text = _name;
			}
		}
		value_image = GetComponentInChildren<UnityEngine.UI.Image> ();
		//TODO change for multiple opponents

		CmdAddPlayer (this.gameObject, _name);
	}

	[Command]
	private void CmdAddPlayer(GameObject p, string name_){
		this._name = name_;
		Global_Data.Global_Information.AddPlayer (this.gameObject);
	}

	// Update is called once per frame
	void Update ()
	{

		if (isLocalPlayer) {
			if (ready_button.isOn != ready_state) {
				CmdChangeReadyState (ready_button.isOn);
			}
			if (Opponent_Roll > 0) {
				if (flash_count == Flash_Value) {
					flash_count = 0;
					if (current_count != 0) {
						Color flash_color = Color.green;
						value_image.color = flash_color;
						current_count++;
						if (current_count == Opponent_Roll) {
							current_count = 0;
						}
					} else {
						Color flash_color = Color.red;
						value_image.color = flash_color;
						current_count++;
					}
				} else {

					Color flash_color = value_image.color;
					flash_color.a -= 1f / Flash_Value;
					value_image.color = flash_color;

					flash_count++;

				}
			} else {
				value_image.color = Color.clear;
			}
		}

	}

	[Command]
	private void CmdChangeReadyState(bool n_state){
		ready_state = n_state;
	}

	public void RecieveChips(List<Chip> chips){
		foreach (Chip c in chips) {
			current_chips.Add (c);
		}
		infoscreen.AddInfo ("You gained " + chips.Count + " chips");
		RpcUpdateCount(current_chips.Count);
	}

	public void Raise(){
		CmdRaise ();
	}

	[Command]
	private void CmdRaise(){
		//TODO option for raising more than one
		if (current_chips.Count > 0) {
			Chip c = current_chips [0];
			Global_Data.Global_Information.Raise (c, _name);
			current_chips.Remove (c);
			raised++;
			RpcUpdateCount(current_chips.Count);
		}

	}

	public void FakeRaise(){
		CmdFakeRaise ();
	}

	[Command]
	private void CmdFakeRaise(){
		//TODO option for raising more than one
		if (raised > 0) {
			Global_Data.Global_Information.Raise (new Chip (), _name);
		}

	}

	[ClientRpc]
	private void RpcUpdateCount(int Chip_Count){
		current_chip_count.text = "You have " + Chip_Count + " chips";
	}

	public void UpdatePot(){
		RpcUpdatePot (Global_Data.Global_Information.Pot_value, Global_Data.Global_Information.Pot_count);
	}

	[ClientRpc]
	private void RpcUpdatePot(int value, int count){
		curr_pot.text = "Pot Count: " + count;
	}

	public void Roll(){
		CmdRoll ();
	}

	[Command]
	private void CmdRoll(){
		
		if (dice_roll.Count == 0) {
			for (int i = 0; i < Global_Data.Global_Information.Dice_To_Roll(); i++) {
				dice_roll.Add (new Die ());
			}
			Roll_value ();
			Final_Value ();
			ready_button.isOn = false;
			Global_Data.Global_Information.UpdateInformation (_name + " rolled their dice");
		}

	}

	public void ReRoll(){
		CmdReRoll ();
	}

	[Command]
	private void CmdReRoll(){
		if (!ready_button.isOn && dice_roll.Count > 0) {
			ReRollCount--;
			foreach (Die d in dice_roll) {
				d.Roll ();
			}
			Roll_value ();
			Final_Value ();
			Global_Data.Global_Information.UpdateInformation (_name + " rerolled their dice");
			RpcAddInfo ("You have " + ReRollCount + " rerolls left");
		}

	}

	public void ClearRoll(){
		raised = 0;
		if (!isClient) {
			RpcClearRoll ();
		} else {
			dice_roll.Clear ();
			Dice_Roll_Value = 0;
			Dice_Roll_Final = 0;
			ready_button.isOn = false;
			CmdChangeReadyState (false);
			ReRollCount = MaxReRolls;

			OpponentValue (0);
		}
	}

	[ClientRpc]
	private void RpcClearRoll(){
		dice_roll.Clear ();
		Dice_Roll_Value = 0;
		Dice_Roll_Final = 0;
		ready_button.isOn = false;
		CmdChangeReadyState (false);
		ReRollCount = MaxReRolls;

		OpponentValue (0);
	}

	public void Roll_value(){
		
		LocalRoll_value ();

	}

	[ClientRpc]
	private void RpcRoll_value(){		
		int val = 0;
		foreach (Die d in dice_roll) {
			val += d.roll_value;
		}
		Dice_Roll_Value = val;
	}

	private void LocalRoll_value(){		
		int val = 0;
		foreach (Die d in dice_roll) {
			val += d.roll_value;
		}
		Dice_Roll_Value = val;
	}

	public void Final_Value(){
		LocalFinal_value ();
	}

	[ClientRpc]
	private void RpcFinal_value(){
		//TODO fill in for rules
		//check for multiple
		int value = 1;
		List<MultiDiceRoll> multi = MultipleDice();
		//Dice Runs (3+ kept dice only)

		//check amount kept
		MultiDiceRoll max = null;
		if (multi.Count > 0) {
			foreach (MultiDiceRoll d_roll in multi) {
				d_roll.AdjustForKept();
				if (max != null) {
					if (max.value < d_roll.value) {
						max = d_roll;
					}
				} else {
					max = d_roll;
				}
			}
			value = max.value;
		}
		if (max != null) {
			if (max.Rolled < Global_Data.Global_Information.Kept_Dice()) {
				//add more to value rolled
			}
		} else {
			RpcRoll_value();
			value = Dice_Roll_Value;
		}
		Dice_Roll_Final = value;
	}

	private void LocalFinal_value(){
		//TODO fill in for rules
		//check for multiple
		int value = 1;
		List<MultiDiceRoll> multi = MultipleDice();
		//Dice Runs (3+ kept dice only)

		//check amount kept
		MultiDiceRoll max = null;
		if (multi.Count > 0) {
			foreach (MultiDiceRoll d_roll in multi) {
				d_roll.AdjustForKept();
				if (max != null) {
					if (max.value < d_roll.value) {
						max = d_roll;
					}
				} else {
					max = d_roll;
				}
			}
			value = max.value;
		}
		if (max != null) {
			if (max.Rolled < Global_Data.Global_Information.Kept_Dice()) {
				//add more to value rolled
			}
		} else {
			LocalRoll_value();
			value = Dice_Roll_Value;
		}
		Dice_Roll_Final = value;
	}

	private List<MultiDiceRoll> MultipleDice(){
		List<MultiDiceRoll> mult = new List<MultiDiceRoll>();
		for (int i = 0; i < dice_roll.Count; i++) {
			for (int j = i + 1; j < dice_roll.Count; j++) {
				if (dice_roll [i].roll_value == dice_roll [j].roll_value) {
					MultiDiceRoll d_roll = mult.Find (delegate(MultiDiceRoll obj) {
						return obj.Face == dice_roll [i].roll_value;
					});
					if (d_roll != null) {
						d_roll.AddDie (dice_roll [j].ID);
					} else {
						mult.Add (new MultiDiceRoll (2, dice_roll [i].roll_value, dice_roll [i].ID, dice_roll [j].ID));
					}
				}
			}
		}
		return mult;
	}

	public string Dice_Rolled(){
		string dies = "";
		foreach (Die d in dice_roll) {
			dies = dies + d.roll_value + " ";
		}
		return dies;
	}

	public bool isReady(){
		return ready_state;
	}

	public void OpponentValue(int val){
		if(val != Opponent_Roll){
			Opponent_Roll = val;
			RpcOpponentValue (val);
		}
	}

	[ClientRpc]
	private void RpcOpponentValue(int val){
		//opponent_value.text = ""+val;
		current_count = 0;
	}

	public int RollValue{
		get{ return Dice_Roll_Value; }
	}

	public int FinalRollValue{
		get{ return Dice_Roll_Final; }
	}

	public void QuitGame(){
		
		NetworkManager.Shutdown ();

		GameObject.Instantiate (menu_prefab);
		GameObject.Instantiate (hold_prefab);
		Global_Data.Global_Information.ClearPlayers ();
	}

	public void AddInfo(string n_info){
		RpcAddInfo (n_info);
	}

	[ClientRpc]
	private void RpcAddInfo(string info){
		infoscreen.AddInfo (info);
	}

	private class MultiDiceRoll{
		int diceRolled;
		int dicevalue;
		int diceface;
		List<int> Dice_IDs;

		public MultiDiceRoll(int count, int face, int ID_one, int ID_two){
			diceRolled = count;
			diceface = face;
			SolveDiceValue();
			Dice_IDs = new List<int>();
			Dice_IDs.Add(ID_one);
			Dice_IDs.Add(ID_two);
		}

		private void SolveDiceValue(){
			dicevalue = diceRolled * diceface;
			if(dicevalue == diceRolled){
				int val = 0;
				for(int i = 0; i < diceRolled; i++){
					val += 9 * Mathf.FloorToInt(Mathf.Pow(10, i));
				}
				if(val < Global_Data.Global_Information.Dice_Faces() * diceRolled){
					val += 9 * Mathf.FloorToInt(Mathf.Pow(10, diceRolled));
				}
			}
		}

		public void AddDie(int ID){
			if (!Dice_IDs.Contains (ID)) {
				diceRolled++;
				SolveDiceValue ();
				Dice_IDs.Add (ID);
			}
		}

		public void RemoveDie(){
			if (diceRolled > 1) {
				diceRolled--;
				Dice_IDs.Remove (0);
				SolveDiceValue ();
			}
		}

		public void AdjustForKept(){
			while (diceRolled > Global_Data.Global_Information.Kept_Dice()) {
				RemoveDie ();
			}
		}

		public int Face{
			get{ return diceface; }
		}

		public int Rolled{
			get{ return diceRolled; }
		}

		public List<int> ID_list{
			get{ return Dice_IDs; }
		}
		
		public int value{
			get{ return dicevalue; }
		}
								
	}
}

